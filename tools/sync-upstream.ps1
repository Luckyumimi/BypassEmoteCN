#requires -Version 7
<#
    上游同步体检。

    目的：把「必须保留什么 / 有没有危险写法 / 汉化缺了什么 / 版本要不要跟」压成一份短报告，
    这样同步时只需要读这份报告（几十行），而不是把 66 个文件的 diff（约 2800 行）读一遍。

    只读，不改任何文件。

    用法:
        pwsh -File tools/sync-upstream.ps1                 # 体检
        pwsh -File tools/sync-upstream.ps1 -Build          # 体检 + 编译
        pwsh -File tools/sync-upstream.ps1 -MaxDetail 30   # 明细多打一点
        pwsh -File tools/sync-upstream.ps1 -NoFetch        # 跳过 git fetch

    退出码: 0 = 没有待处理项; 1 = 有待处理项
#>
param(
    [string]$Repo = '',
    [switch]$Build,
    [string]$DalamudHome = "$env:APPDATA\XIVLauncherCN\addon\Hooks\dev",
    [string]$Proxy = 'http://127.0.0.1:7897',
    [int]$MaxDetail = 12,
    [switch]$NoFetch
)

$ErrorActionPreference = 'Continue'

if (-not $Repo) { $Repo = Split-Path -Parent $PSScriptRoot }
if (-not (Test-Path (Join-Path $Repo '.git'))) { throw "不是 git 仓库: $Repo" }
$Repo = (Resolve-Path $Repo).Path

$problems = [System.Collections.Generic.List[string]]::new()
$lines    = [System.Collections.Generic.List[string]]::new()

function Add-Line { param([string]$T) $script:lines.Add($T) }
function Add-Ok { param([string]$T) $script:lines.Add("[ok] $T") }
function Add-Problem {
    param([string]$T)
    $script:problems.Add($T)
    $script:lines.Add("[!!] $T")
}

# ---------- 0. 仓库状态 ----------
Add-Line "=== 0. 仓库状态 ==="
$dirty = & git -C $Repo status --porcelain
if ($dirty) { Add-Line "  工作区有未提交改动: $(@($dirty).Count) 项" } else { Add-Line "  工作区干净" }

$unmerged = & git -C $Repo diff --name-only --diff-filter=U
if ($unmerged) {
    Add-Problem "有未解决的合并冲突 $(@($unmerged).Count) 个文件"
    $unmerged | Select-Object -First $MaxDetail | ForEach-Object { Add-Line "      $_" }
}

# ---------- 1. 上游 ----------
Add-Line ""
Add-Line "=== 1. 上游 ==="
$upRef = 'upstream/main'
if (-not $NoFetch) {
    $env:HTTP_PROXY = $Proxy
    $env:HTTPS_PROXY = $Proxy
    $null = & git -C $Repo fetch upstream --prune 2>&1
    if ($LASTEXITCODE -ne 0) { Add-Problem "git fetch upstream 失败（代理 $Proxy 是否可用？）" }
}

$null = & git -C $Repo rev-parse --verify --quiet $upRef 2>&1
if ($LASTEXITCODE -ne 0) {
    Add-Problem "没有 $upRef（git remote add upstream https://github.com/Aspher0/BypassEmote.git）"
}
else {
    $behind = [int](& git -C $Repo rev-list --count "HEAD..$upRef")
    $ahead = [int](& git -C $Repo rev-list --count "$upRef..HEAD")
    Add-Line "  上游有、我们没有: $behind 笔"
    Add-Line "  我们有、上游没有: $ahead 笔"

    if ($behind -gt 0) {
        Add-Line "  上游新提交:"
        & git -C $Repo log --oneline --no-decorate "HEAD..$upRef" |
            Select-Object -First $MaxDetail | ForEach-Object { Add-Line "      $_" }

        $changed = & git -C $Repo diff --name-only "HEAD..$upRef"
        Add-Line "  涉及 $(@($changed).Count) 个文件:"
        $changed | Select-Object -First $MaxDetail | ForEach-Object { Add-Line "      $_" }
        if (@($changed).Count -gt $MaxDetail) { Add-Line "      …还有 $(@($changed).Count - $MaxDetail) 个" }

        Add-Problem "上游有 $behind 笔新提交待合并"
    }
    else { Add-Ok "已是最新，没有上游提交待合并" }
}

# ---------- 2. fork 专属改动 ----------
Add-Line ""
Add-Line "=== 2. fork 专属改动是否还在（丢了不报错，只会静默回退到上游行为） ==="
$guardsBefore = $problems.Count
$guards = @(
    @{ F = 'BypassEmote/Plugin.cs';                   S = 'Luckyumimi/BypassEmoteCN/refs/heads/main/repo.json';         W = '更新检查指向本仓库' },
    @{ F = 'BypassEmote/Safety/PatchApprovalGate.cs'; S = 'Luckyumimi/BypassEmoteCN/refs/heads/main/patch-approval.json'; W = '审批列表指向本仓库' },
    @{ F = 'BypassEmote/BypassEmote.csproj';          S = 'Luckyumimi/BypassEmoteCN';                                   W = 'csproj 项目地址' },
    @{ F = 'BypassEmote/BypassEmote.json';            S = 'Luckyumimi/MyDalamudPlugins';                                W = '打包清单的下载/图标地址' },
    @{ F = 'repo.json';                               S = 'Luckyumimi/MyDalamudPlugins';                                W = '独立仓库的下载地址' },
    @{ F = 'BypassEmote/Service.cs';                  S = 'OpenIssueTracker';                                           W = '报告 bug 按钮' },
    @{ F = 'BypassEmote/Service.Hooks.cs';            S = 'ExecuteSlotSignature';                                       W = '国服热键栏特征码修复' },
    @{ F = 'BypassEmote/Service.EmoteUi.cs';          S = 'SafeText.Utf8';                                              W = '防 AV 崩溃的安全读取' },
    @{ F = 'BypassEmote/Helpers/EmoteListDimmer.cs';  S = 'SafeText.Utf8';                                              W = '防 AV 崩溃的安全读取' },
    @{ F = 'BypassEmote/GlobalUsings.cs';             S = 'global using BypassEmote.Localization;';                     W = 'L.T 全局 using' },
    @{ F = 'BypassEmote/Configuration.cs';            S = 'PluginLanguage Language';                                    W = '语言配置项' },
    @{ F = 'BypassEmote/Plugin.cs';                   S = 'L.DetectClientLanguage()';                                   W = '启动时识别客户端语言' },
    @{ F = 'BypassEmote/Plugin.Commands.cs';          S = 'AddSubCommand("lang"';                                       W = '/be lang 命令' },
    @{ F = 'BypassEmote/UI/ConfigWindow.cs';          S = 'DrawLanguageRow';                                            W = '语言下拉' },
    @{ F = 'BypassEmote/UI/ConfigWindow.cs';          S = 'RefreshTabLabels';                                           W = '标签页跟随语言切换' }
)
foreach ($g in $guards) {
    $path = Join-Path $Repo $g.F
    if (-not (Test-Path $path)) { Add-Problem "缺文件 $($g.F) —— $($g.W)"; continue }
    if (-not (Select-String -Path $path -Pattern $g.S -SimpleMatch -Quiet)) {
        Add-Problem "$($g.F) 里找不到「$($g.S)」—— $($g.W)"
    }
}
foreach ($f in 'BypassEmote/Localization/L.cs', 'BypassEmote/Localization/ZhTables.cs',
    'BypassEmote/Plugin.Language.cs', 'BypassEmote/Helpers/SafeText.cs') {
    if (-not (Test-Path (Join-Path $Repo $f))) { Add-Problem "缺文件 $f" }
}
if ($problems.Count -eq $guardsBefore) { Add-Ok "15 项锚点 + 4 个关键文件都在" }

# ---------- 3. 危险写法 ----------
Add-Line ""
Add-Line "=== 3. 危险写法（在国服上会崩或失效） ==="
$csFiles = Get-ChildItem (Join-Path $Repo 'BypassEmote') -Recurse -File -Filter *.cs |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' -and $_.Name -ne 'SafeText.cs' }

# 会 CTD 的一类：没走 SafeText 的游戏字符串解码
$fatal = @(
    @{ N = '游戏字符串直接解码（指针坏了就是 CTD）'; Rx = '(RawString|EvaluatedString|NodeText)\.ToString\(\)' },
    @{ N = '直接读 NodeText（未经 SafeText）';        Rx = '->NodeText' }
)
$fatalHits = 0
foreach ($d in $fatal) {
    $hits = $csFiles | Select-String -Pattern $d.Rx | Where-Object { $_.Line -notmatch 'SafeText\.Utf8' }
    if (@($hits).Count -gt 0) {
        $fatalHits += @($hits).Count
        Add-Problem "危险写法「$($d.N)」$(@($hits).Count) 处"
        $hits | Select-Object -First $MaxDetail |
            ForEach-Object { Add-Line "      $($_.Filename):$($_.LineNumber)  $($_.Line.Trim())" }
    }
}
if ($fatalHits -eq 0) { Add-Ok "没有未加固的游戏字符串解码（CTD 那一类）" }

# 仅提示：裸数字判节点类型本身不致命，只要后面读字符串走了 SafeText 就无害
$numeric = $csFiles | Select-String -Pattern '->Type\s*(==|!=|>=|<=|>|<)\s*\d'
if (@($numeric).Count -gt 0) {
    Add-Line "  [注意] 裸数字判节点类型 $(@($numeric).Count) 处（国服枚举里没有这些值；后面读字符串必须走 SafeText）"
    $numeric | Select-Object -First $MaxDetail |
        ForEach-Object { Add-Line "      $($_.Filename):$($_.LineNumber)  $($_.Line.Trim())" }
}

# ---------- 4. 汉化 ----------
Add-Line ""
Add-Line "=== 4. 汉化 ==="
$checker = Join-Path $PSScriptRoot 'l10n-check.ps1'
if (Test-Path $checker) {
    $cov = & pwsh -NoProfile -File $checker -Mode Coverage -Root (Join-Path $Repo 'BypassEmote') -MaxDetail $MaxDetail 2>&1

    # 只保留「汇总 / 未登记 / 真冲突」，未使用和重复登记都是无害噪音
    $skipSection = $false
    foreach ($l in $cov) {
        if ($l -match '^===\s*(未使用|重复登记)') { $skipSection = $true; continue }
        if ($l -match '^===') { $skipSection = $false }
        if ($skipSection) { continue }
        Add-Line "  $l"
    }

    $missing = -1
    foreach ($l in $cov) {
        if ($l -match '未登记（会显示英文）:\s*(\d+)') { $missing = [int]$Matches[1] }
    }
    if ($missing -gt 0) { Add-Problem "汉化有 $missing 条未登记（运行时会显示英文）" }
    elseif ($missing -eq 0) { Add-Ok "汉化 0 条未登记" }

    foreach ($l in $cov) { if ($l -match '真冲突') { Add-Problem "词典有真冲突（同一个键译法不同）" } }
}
else { Add-Problem "找不到 tools/l10n-check.ps1" }

# ---------- 5. 版本号 ----------
Add-Line ""
Add-Line "=== 5. 版本号（规则：镜像上游，不自己升） ==="
$csproj = Join-Path $Repo 'BypassEmote/BypassEmote.csproj'
$ours = ([regex]::Match((Get-Content $csproj -Raw), '<Version>([^<]+)</Version>')).Groups[1].Value
Add-Line "  我们 csproj: $ours"

$upCsproj = (& git -C $Repo show "upstream/main:BypassEmote/BypassEmote.csproj" 2>&1) -join "`n"
$up = ([regex]::Match($upCsproj, '<Version>([^<]+)</Version>')).Groups[1].Value
if ($up) {
    Add-Line "  上游 csproj: $up"
    if ($up -ne $ours) { Add-Problem "版本号与上游不一致，应改成 $up（并同步 repo.json / pluginmaster）" }
    else { Add-Ok "版本号与上游一致（$ours）" }
}

$repoVer = (((Get-Content (Join-Path $Repo 'repo.json') -Raw) | ConvertFrom-Json)[0]).AssemblyVersion
Add-Line "  repo.json:   $repoVer"
if ($repoVer -ne $ours) { Add-Problem "repo.json 的 AssemblyVersion ($repoVer) 与 csproj ($ours) 不一致" }

# ---------- 6. 编译（可选） ----------
if ($Build) {
    Add-Line ""
    Add-Line "=== 6. 编译 ==="
    $env:DALAMUD_HOME = $DalamudHome
    $env:HTTP_PROXY = $Proxy
    $env:HTTPS_PROXY = $Proxy
    $out = & dotnet build $csproj -c Release 2>&1
    $errs = $out | Where-Object { $_ -match 'error CS' }
    if (@($errs).Count -gt 0) {
        Add-Problem "编译失败 $(@($errs).Count) 个错误"
        $errs | Select-Object -First $MaxDetail | ForEach-Object { Add-Line "      $_" }
    }
    else {
        $sum = ($out | Where-Object { $_ -match '个错误|个警告' } | Select-Object -Last 2) -join ' / '
        Add-Ok "编译通过（$sum）"
    }
}

# ---------- 结论 ----------
Add-Line ""
Add-Line "=== 结论 ==="
if ($problems.Count -eq 0) {
    Add-Line "  没有待处理项。"
}
else {
    Add-Line "  $($problems.Count) 项待处理："
    $problems | ForEach-Object { Add-Line "    - $_" }
}

$lines | ForEach-Object { $_ }

if ($problems.Count -gt 0) { exit 1 }
exit 0
