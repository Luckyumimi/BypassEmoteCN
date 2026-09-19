#requires -Version 7
<#
    BypassEmote 汉化校验工具

    -Coverage : 把源码里所有 L.T("...") 用到的键，和 Localization/Zh/*.cs 里登记的键对比，
                报告「用了但没登记」（会显示英文）与「登记了但没用」（冗余）。
    -Scan     : 扫描疑似用户可见、但没有被 L.T(...) 包住的英文字面量，输出 file:line 清单。

    用法:
        pwsh -File _l10n_check.ps1 -Mode Coverage
        pwsh -File _l10n_check.ps1 -Mode Scan
#>
param(
    [ValidateSet('Coverage', 'Scan')]
    [string]$Mode = 'Coverage',
    [string]$Root = '',
    # 明细最多打印多少条，避免把整份清单倒进上下文
    [int]$MaxDetail = 12
)

$ErrorActionPreference = 'Stop'

# 默认校验脚本所在仓库里的插件目录，换机器 / 换 clone 路径都不用改
if (-not $Root) { $Root = Join-Path (Split-Path -Parent $PSScriptRoot) 'BypassEmote' }
if (-not (Test-Path $Root)) { throw "找不到插件目录: $Root" }

$sourceFiles = Get-ChildItem $Root -Recurse -File -Filter *.cs |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }

$tableFiles = Get-ChildItem (Join-Path $Root 'Localization\Zh') -File -Filter *.cs -ErrorAction SilentlyContinue

# 在 $text 中从 $start（"(" 之后）开始，取出第一个参数（到顶层逗号或右括号为止）
function Get-FirstArgument {
    param([string]$Text, [int]$Start)

    $depth = 0
    $i = $Start
    $sb = [System.Text.StringBuilder]::new()
    $inString = $false
    $verbatim = $false

    while ($i -lt $Text.Length) {
        $c = $Text[$i]

        if ($inString) {
            if ($verbatim) {
                if ($c -eq '"') {
                    if ($i + 1 -lt $Text.Length -and $Text[$i + 1] -eq '"') { [void]$sb.Append('""'); $i += 2; continue }
                    $inString = $false
                }
                [void]$sb.Append($c); $i++; continue
            }

            if ($c -eq '\') { [void]$sb.Append($c); if ($i + 1 -lt $Text.Length) { [void]$sb.Append($Text[$i + 1]) }; $i += 2; continue }
            if ($c -eq '"') { $inString = $false }
            [void]$sb.Append($c); $i++; continue
        }

        if ($c -eq '"') {
            $inString = $true
            $verbatim = ($i -gt 0 -and $Text[$i - 1] -eq '@')
            [void]$sb.Append($c); $i++; continue
        }

        if ($c -eq '(' -or $c -eq '[') { $depth++; [void]$sb.Append($c); $i++; continue }
        if ($c -eq ')' -or $c -eq ']') {
            if ($depth -eq 0) { break }
            $depth--; [void]$sb.Append($c); $i++; continue
        }
        if ($c -eq ',' -and $depth -eq 0) { break }

        [void]$sb.Append($c); $i++
    }

    $script:lastEnd = $i
    return $sb.ToString()
}

# 把一段「只由字符串字面量 + 拼接」组成的表达式还原成它运行时的文本
function Get-ConcatenatedLiteral {
    param([string]$Expression)

    $rx = [regex]'"((?:[^"\\]|\\.|"")*)"'
    $parts = [System.Collections.Generic.List[string]]::new()

    foreach ($m in $rx.Matches($Expression)) {
        $inner = $m.Groups[1].Value
        if ($inner.StartsWith('"') -and $inner.EndsWith('"') -and $inner.Length -ge 2) {
            $inner = $inner.Substring(1, $inner.Length - 2)   # 逐字字符串 "" -> "
        }
        $parts.Add($inner)
    }

    return ($parts -join '')
}

$script:used = [System.Collections.Generic.Dictionary[string, System.Collections.Generic.List[string]]]::new([System.StringComparer]::Ordinal)

foreach ($file in $sourceFiles) {
    $rel = $file.FullName.Substring($Root.Length + 1)
    $lines = [System.IO.File]::ReadAllLines($file.FullName)
    $text = [System.IO.File]::ReadAllText($file.FullName)

    foreach ($m in [regex]::Matches($text, 'L\.T\s*\(')) {
        $argStart = $m.Index + $m.Length
        $expr = Get-FirstArgument -Text $text -Start $argStart
        $key = Get-ConcatenatedLiteral -Expression $expr

        if ([string]::IsNullOrEmpty($key)) { continue }

        $line = 1
        for ($i = 0; $i -lt $m.Index; $i++) { if ($text[$i] -eq "`n") { $line++ } }

        if (-not $script:used.ContainsKey($key)) { $script:used[$key] = [System.Collections.Generic.List[string]]::new() }
        $script:used[$key].Add("${rel}:$line")
    }
}

$script:declared = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::Ordinal)
$script:declaredValue = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::Ordinal)
$script:dupes = [System.Collections.Generic.List[string]]::new()
$script:conflicts = [System.Collections.Generic.List[string]]::new()

foreach ($file in $tableFiles) {
    $rel = $file.FullName.Substring($Root.Length + 1)
    $text = [System.IO.File]::ReadAllText($file.FullName)

    # 词典行形如  ("English", "中文"),  —— 两侧都可能是多行 "a" + "b" 拼接，
    # 所以用和调用点相同的解析器取参数，而不是正则去匹配单个字面量。
    foreach ($m in [regex]::Matches($text, '\(\s*"')) {
        $keyExpr = Get-FirstArgument -Text $text -Start ($m.Index + 1)
        $key = Get-ConcatenatedLiteral -Expression $keyExpr
        if ([string]::IsNullOrEmpty($key)) { continue }

        $j = $script:lastEnd
        while ($j -lt $text.Length -and ($text[$j] -eq ',' -or [char]::IsWhiteSpace($text[$j]))) { $j++ }

        $value = Get-ConcatenatedLiteral -Expression (Get-FirstArgument -Text $text -Start $j)

        if ($script:declared.ContainsKey($key)) {
            $script:dupes.Add("$key  ($rel)")
            if ($script:declaredValue[$key] -ne $value) {
                $script:conflicts.Add("$key  先登记=$($script:declaredValue[$key])  本次=$value  ($rel)")
            }
        }
        else {
            $script:declared[$key] = $rel
            $script:declaredValue[$key] = $value
        }
    }
}

if ($Mode -eq 'Coverage') {
    $missing = $script:used.Keys | Where-Object { -not $script:declared.ContainsKey($_) } | Sort-Object
    $unused = $script:declared.Keys | Where-Object { -not $script:used.ContainsKey($_) } | Sort-Object

    "=== 汇总 ==="
    "L.T 调用点（去重后）: $($script:used.Count)"
    "词典条目（去重后）:   $($script:declared.Count)"
    "未登记（会显示英文）: $($missing.Count)"
    "未使用（冗余条目）:   $($unused.Count)"
    "重复登记:             $($script:dupes.Count)"
    ""

    if ($missing.Count -gt 0) {
        "=== 未登记 —— 这些地方运行时会退回英文 ==="
        foreach ($k in ($missing | Select-Object -First $MaxDetail)) {
            $where = ($script:used[$k] | Select-Object -First 3) -join ', '
            "  [$where]"
            "      $k"
        }
        if ($missing.Count -gt $MaxDetail) { "  …还有 $($missing.Count - $MaxDetail) 条（明细已截断，去掉 -MaxDetail 可看全）" }
        ""
    }

    if ($unused.Count -gt 0) {
        "=== 未使用 —— 词典里多余/键名写错的条目 ==="
        foreach ($k in ($unused | Select-Object -First $MaxDetail)) { "  [$($script:declared[$k])]  $k" }
        if ($unused.Count -gt $MaxDetail) { "  …还有 $($unused.Count - $MaxDetail) 条" }
        ""
    }

    if ($script:conflicts.Count -gt 0) {
        "=== 真冲突 —— 同一个键在不同词典里译法不同（先登记的生效） ==="
        foreach ($c in ($script:conflicts | Select-Object -First $MaxDetail)) { "  $c" }
        ""
    }

    if ($script:dupes.Count -gt 0) {
        "=== 重复登记（译法相同，无害） ==="
        foreach ($d in ($script:dupes | Select-Object -First $MaxDetail)) { "  $d" }
        if ($script:dupes.Count -gt $MaxDetail) { "  …还有 $($script:dupes.Count - $MaxDetail) 条" }
    }

    exit 0
}

# ---------------- Scan：找漏译 ----------------
$uiCalls = @(
    'ImGui\.Text\b', 'ImGui\.TextUnformatted', 'ImGui\.TextWrapped', 'ImGui\.TextDisabled',
    'ImGui\.TextColored', 'ImGui\.TextColoredWrapped', 'ImGui\.Button', 'ImGui\.SmallButton',
    'ImGui\.Selectable', 'ImGui\.MenuItem', 'ImGui\.SetTooltip', 'ImGui\.BeginTooltip',
    'ImGui\.BeginCombo', 'ImGui\.Combo', 'ImGui\.Checkbox', 'ImGui\.Begin\b',
    'ImGui\.BeginTabItem', 'ImGui\.TableSetupColumn', 'ImGui\.InputText',
    '\.WithHelp\s*\(', 'SettingsLayout\.Help\s*\(', 'SettingsLayout\.Heading\s*\(',
    'SettingsLayout\.Name\s*\(', 'SettingsLayout\.Marker\s*\(',
    'LogHelper\.(Info|Error|Notice|Success|NoticeAlways)\s*\(',
    'NoireText\.\w+\s*\(', 'NoireModal\.ConfirmAsync\s*\(',
    'AddText\s*\(', 'AddIconText\s*\(', '\.AddText\s*\(',
    'AddNotification', 'Title\s*=', 'Content\s*=',
    'WithTitle\s*\(', 'SetTitle\s*\('
) -join '|'

$rxLiteral = [regex]'"((?:[^"\\]|\\.)*)"'
$hits = [System.Collections.Generic.List[string]]::new()

foreach ($file in $sourceFiles) {
    $rel = $file.FullName.Substring($Root.Length + 1)
    if ($rel -like 'Localization\*') { continue }

    $lines = [System.IO.File]::ReadAllLines($file.FullName)

    for ($n = 0; $n -lt $lines.Count; $n++) {
        $line = $lines[$n]
        if ($line -notmatch $uiCalls) { continue }
        if ($line -match 'L\.T\s*\(') { continue }
        if ($line -match '^\s*(//|///|\*)') { continue }

        foreach ($m in $rxLiteral.Matches($line)) {
            $s = $m.Groups[1].Value
            if ($s -match '^#') { continue }
            if ($s.Length -lt 3) { continue }
            if ($s -notmatch '[A-Za-z]') { continue }
            if ($s -notmatch '[a-z]') { continue }
            if ($s -match '^[\w\.\-/\\:]+$') { continue }
            if ($s -match '^(BypassEmote|Bypass Emote|Penumbra|Dalamud|GPose|NoireLib|FFXIV|ImGui|##)') { continue }

            $hits.Add(("{0}:{1}  {2}" -f $rel, ($n + 1), $s))
        }
    }
}

"=== 疑似漏译（用户可见位置的英文字面量，未被 L.T 包住）: $($hits.Count) ==="
$sorted = $hits | Sort-Object -Unique
$sorted | Select-Object -First $MaxDetail
if ($sorted.Count -gt $MaxDetail) { "…还有 $($sorted.Count - $MaxDetail) 条（去掉 -MaxDetail 可看全）" }