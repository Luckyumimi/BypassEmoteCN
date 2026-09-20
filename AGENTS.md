# Repository Guidelines

This repository is the Chinese-client compatibility fork of the upstream
`Aspher0/BypassEmote` Dalamud plugin. Keep the fork close to upstream and keep
CN-specific code isolated. Do not treat generated files or the old
`D:\codex\7887\BypassEmoteCN - 副本` directory as source.

## Repository Layout

- `BypassEmote/` — C# plugin source and `BypassEmote.json` package manifest.
- `BypassEmote/UI/` — main window, settings pages, tooltips, and ImGui drawing.
- `BypassEmote/Service*.cs` — plugin services, hooks, and game integration.
- `BypassEmote/Helpers/` — compatibility, memory-safety, addon, and utility code.
- `BypassEmote/EmoteManager/`, `EmoteSwap/`, `IPC/`, `Safety/` — runtime subsystems.
- `Data/` — checked-in assets. Do not put build output here.
- `translations.tsv` — the only maintained translation source.
- `tools/LocalizationGenerator/` — Roslyn source rewriter and translation-table generator.
- `tools/build-cn.ps1` — canonical CN build entry point.
- `artifacts/` — local, disposable extracted plugin packages; normally untracked.

There is no unit-test project. `obj/`, `bin/`, `BypassEmote/obj/CnGenerated/`,
ZIP files, logs, and local caches are build artifacts, not source changes.

## Required Build and Package Workflow

Use PowerShell 7 and the CN Dalamud SDK. Always build the CN package, not just
the ordinary source project:

```powershell
$env:DALAMUD_HOME = "$env:APPDATA\XIVLauncherCN\addon\Hooks\dev"
pwsh -NoLogo -NoProfile -NonInteractive -File .\tools\build-cn.ps1
git diff --check
```

The script validates `translations.tsv`, recreates `BypassEmote/obj/CnGenerated/`,
and builds Release with `UseCnGeneratedSources=true`. A successful build must
report `0` errors and `0` warnings. Do not hand-edit generated files.

To produce a loadable local package, extract the generated archive into the
existing artifact directory (replace its contents, do not place only a DLL):

```powershell
$zip = 'BypassEmote\bin\Release\BypassEmote\latest.zip'
$dest = 'artifacts\BypassEmote'
if (Test-Path -LiteralPath $dest) { [System.IO.Directory]::Delete((Resolve-Path -LiteralPath $dest).Path, $true) }
Expand-Archive -LiteralPath $zip -DestinationPath $dest -Force
```

Verify that `artifacts\BypassEmote` contains `BypassEmote.dll`,
`BypassEmote.json`, `BypassEmote.deps.json`, and all dependency DLLs. Record
the manifest version and DLL SHA-256 when handing a package to the user.

## Localization Rules

The generator scans ordinary C# string literals and constant concatenations,
then emits a temporary localized tree. Add or correct translations in
`translations.tsv` as `English<TAB>简体中文`; keep placeholders such as `{0}`
identical. Duplicate keys with different translations and placeholder mismatch
must remain build errors. Missing keys intentionally fall back to English, so a
missed translation must never throw or crash the plugin.

Do not wrap dozens of upstream files manually with `L.T(...)`; that creates
merge conflicts. For a dynamic tooltip or a multi-line string assembled from
several expressions, use one explicit `L.T(fullExpression)` at the maintained
source call site and add the exact resulting English key to `translations.tsv`.
Do not translate user/API data blindly: translate known labels/types and keep
unknown values unchanged. This is required for FFXIVCollect source categories
and future upstream text.

`BypassEmote/Localization/L.cs` is the identity fallback used by an ordinary
source build. The CN generator deliberately excludes that file and emits its
own `Localization.Generated.cs` table. Keep this separation: do not put a
second dictionary in the fallback file and do not compile both implementations.

When the game still shows English, use this checklist instead of guessing:

1. Copy the exact visible English text and find its source call site with `rg`.
2. Confirm the exact full key exists in `translations.tsv`, including spaces,
   punctuation, newlines, quotes, and placeholders.
3. Rebuild with `tools/build-cn.ps1`; never inspect a stale generated tree.
4. Inspect the matching file under `BypassEmote/obj/CnGenerated/`. The value
   passed to ImGui must be an `L.T(...)` result or already translated upstream.
5. If a concatenated tooltip remains unwrapped, fix the generator when the
   rule is general. Use one explicit `L.T(fullExpression)` only for dynamic or
   exceptional call sites. Never add `L.T` separately to every fragment.
6. Re-extract `latest.zip`, compare the new DLL timestamp/hash, reload the full
   plugin directory, and retest the exact marker shown in the screenshot.

Central UI helpers such as `SettingsLayout.Name`, `SettingsLayout.Help`, and
`SettingsLayout.Marker` already localize their string input. Preserve that
contract and avoid double-wrapping callers unless the input must be translated
before it reaches the helper.

## CN Compatibility and Runtime Safety

`Service.Hooks.cs` contains the CN hotbar `ExecuteSlot` signature. If the
signature is not found, disable that hook safely; never guess a new pattern or
dereference an unverified address. `SafeText.cs` must validate native text
memory before reading it. `AddonText.cs` and related helpers exist because the
CN NoireLib version lacks some upstream APIs; preserve their null checks and
fallback behavior. After hook or addon changes, inspect `/xllog` for hook
binding errors and repeatedly open/close the Emote window before claiming a fix.

## Upstream Synchronization

Use the configured `upstream` remote (`https://github.com/Aspher0/BypassEmote.git`):

```powershell
git fetch upstream main --prune
git diff --stat HEAD..upstream/main
git diff --name-status HEAD..upstream/main
```

For conflicts, keep upstream ordinary feature code first, then reapply only
the deliberate CN layers: hook signature and safety helpers, NoireLib
compatibility helpers, CN metadata, `translations.tsv`, and build tools.
Remove every conflict marker, rebuild, and inspect the generated translation
count. Never copy code from the obsolete duplicate tree. Do not commit
generated sources, `obj/`, `bin/`, stale ZIPs, or local caches.

## Version and Library Chain

Before changing a version, read the upstream catalog:
`https://raw.githubusercontent.com/Aspher0/BypassEmote/refs/heads/main/repo.json`.
The upstream `AssemblyVersion` is authoritative. Keep that exact value in
`BypassEmote.csproj`, root `repo.json`, packaged `BypassEmote.json`, and
`D:\codex\7887\MyDalamudPlugins\pluginmaster.json`; do not invent a CN-only
patch version. When a release is explicitly requested, copy the verified full
package to the library-chain `plugins\BypassEmote\latest.zip`, verify its
SHA-256, then update the catalog metadata consistently.

## Style, Verification, and Git Boundaries

Follow `.editorconfig`: UTF-8, LF, four spaces, file-scoped namespaces,
PascalCase public members, and the repository's private-field naming rules.
Use `var` where the type is obvious, keep unsafe code narrowly scoped, and
prefer small helpers over repeated pointer logic.

Manual smoke checks after a package build: load the full directory, open every
settings tab, hover every information marker, open/close the Emote addon
repeatedly, click a locked emote hotbar slot, and inspect `/xllog`. A local
build is not a remote release and a copied DLL is not a complete plugin.

Do not commit or push unless the user explicitly asks. If asked to push, check
`git status`, commit only intended source/metadata changes, and push both repos
through the active mihomo proxy:
`http://127.0.0.1:7890`. Never include credentials, personal logs, debug
archives, or unverified artifacts.
