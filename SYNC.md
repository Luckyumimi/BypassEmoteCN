# 同步上游说明（给 AI agent 看的）

本仓库是 **Bypass Emote 的汉化 + 国服适配分支**，不是原版。原版：https://github.com/Aspher0/BypassEmote

## 一条命令

```powershell
pwsh -File tools/sync-upstream.ps1            # 只体检，不改任何文件
pwsh -File tools/sync-upstream.ps1 -Build     # 体检 + 编译
```

脚本会输出一份短报告（几十行），并在结尾给出「N 项待处理」。**读那份报告就够了**。

## 省 token 的硬性要求（同步时请照做）

1. **不要 `git diff upstream/main..HEAD`**（约 2800 行）。要看范围用 `git diff --stat`。
2. **不要整文件读**。先跑 `tools/sync-upstream.ps1`，只打开报告里点名的文件。
3. **不要重新推导「哪些是 fork 改动」**，第 2 节的清单就是全部，脚本已经查过。
4. 汉化是否完整**只信** `tools/l10n-check.ps1` 的输出，不要靠读源码判断：
   ```powershell
   pwsh -File tools/l10n-check.ps1 -Mode Coverage   # 未登记必须为 0
   pwsh -File tools/l10n-check.ps1 -Mode Scan       # 找没被 L.T 包住的用户可见文案
   ```
   输出默认只打 12 条明细，需要全量再加 `-MaxDetail 999`。
5. 编译用：`$env:DALAMUD_HOME="$env:APPDATA\XIVLauncherCN\addon\Hooks\dev"`，GitHub 走代理 `http://127.0.0.1:7897`。

## 必须保留的 fork 改动

脚本第 2 节会自动核对。丢失**不会编译报错**，只会静默退回上游行为，所以必须查：

| 内容 | 位置 | 丢了会怎样 |
|---|---|---|
| 国服热键栏特征码修复 | `Service.Hooks.cs` 的 `ExecuteSlotSignature` | 国服热键栏点击失效（本 fork 存在的首要原因） |
| 防 AV 崩溃安全读取 | `Helpers/SafeText.cs` + `Service.EmoteUi.cs` / `EmoteListDimmer.cs` 的 `SafeText.Utf8` | 打开情感动作菜单直接 CTD |
| 汉化体系 | `Localization/*`、`Plugin.Language.cs`、`GlobalUsings.cs` 的 `global using BypassEmote.Localization;`、`Configuration.cs` 的 `PluginLanguage Language`、`Plugin.cs` 的 `L.DetectClientLanguage()` | 汉化失效 |
| 语言切换 UI | `UI/ConfigWindow.cs` 的 `DrawLanguageRow`、`RefreshTabLabels`；`Plugin.Commands.cs` 的 `AddSubCommand("lang"` | 无法切换语言 / 标签页不跟着变 |
| 仓库指向 | `Plugin.cs` 的 repo.json 地址、`PatchApprovalGate.cs` 的 patch-approval.json 地址、`repo.json` / `BypassEmote.json` / `BypassEmote.csproj` 里的 `Luckyumimi/...` | 更新检查和安装会回到上游原版 |
| 报告 bug 按钮 | `Service.cs` 的 `OpenIssueTracker` | 未测试客户端提示里的按钮失效 |

**故意保留上游值的地方**：`Service.Swap.cs` 的 `PenumbraModWebsite = "https://github.com/Aspher0/BypassEmote"`（生成的模组要标明原始出处），不要改。

## 危险写法（国服上会崩或失效）

上游新代码如果出现下面三类，必须按 fork 的做法处理：

| 写法 | 为什么危险 | 正确做法 |
|---|---|---|
| 裸数字判节点类型，如 `(ushort)node->Type == 1007` / `< 1000` | 国服 FFXIVClientStructs 的 `NodeType` 枚举里没有 1007（组件是 `Component = 10000`），转型会错，读到垃圾指针 | 用正规枚举，或先 `SafeText` 校验 |
| `RawString.ToString()` / `EvaluatedString.ToString()` / `AtkTextNode.NodeText.ToString()` | `Utf8String.AsSpan()` 无条件按 `StringPtr` 解码，指针是垃圾值就是 `AccessViolationException`，.NET 无法 catch，直接带走游戏 | 一律走 `SafeText.Utf8(...)` |
| 遍历 Atk 节点树时不校验 `NodeList` / `NodeListCount` | 布局不符时会走垃圾内存 | 加合理性上限 |

## 版本号

**镜像上游 `BypassEmote/BypassEmote.csproj` 的 `<Version>`，不自己升。**
改了版本号要同时更新 `repo.json`、`MyDalamudPlugins` 的 `pluginmaster.json`，并重新打包。

注意：版本号不变时，已装用户**收不到更新**（Dalamud 和插件自带的更新检查都比版本号）。

## 定义完成

1. `tools/sync-upstream.ps1` 报告 **0 项待处理**
2. 编译 0 错误
3. 新增/改动的用户可见文案已进词典（`-Mode Coverage` 未登记 = 0）
4. 重新打包 `latest.zip` 并同步到 `MyDalamudPlugins/plugins/BypassEmote/`（含 `BypassEmote.json`），刷新 `pluginmaster.json` 的 `LastUpdate`
5. 进游戏验证：**点开情感动作菜单不崩**、**热键栏点击有效**、**语言可切换**

## 不要做的事

- 不要改 `InternalName`（保持 `BypassEmote`）：`[NoireIpcClass("BypassEmote")]` 是 IPC 命名空间，改名会断掉其他插件的互通，配置目录也会变。
- 不要 `git push`：由仓库所有者自己推。
- 不要因为"看起来一样"就删掉 `Localization/Zh/ZhSwapAdvice.cs` 里被标注为「运行时查表」的条目（`loop` / `turn` / `sound` / `rules` / `modded` / `water underfoot`），它们不在源码里以字面量出现，静态检查会误报为未使用。
