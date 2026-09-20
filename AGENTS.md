# Repository Guidelines

## Project Structure & Module Organization

This repository is a single C# Dalamud plugin solution (`BypassEmote.sln`). Production code lives in `BypassEmote/`, with feature areas split into `UI/`, `EmoteSwap/`, `EmoteManager/`, `IPC/`, `Safety/`, `Helpers/`, and `Changelog/`. The composition root is `Plugin.cs`; shared integration is in `Service*.cs`, including the native hook path in `Service.Hooks.cs`. Metadata and dependencies are in `BypassEmote/BypassEmote.json` and `BypassEmote/BypassEmote.csproj`; `Data/` contains assets. There is no separate test project.

## Build, Test, and Development Commands

Use a current .NET SDK with the Dalamud SDK and package restore available:

```powershell
dotnet restore .\BypassEmote.sln
dotnet build .\BypassEmote.sln -c Release
dotnet build .\BypassEmote.sln -c Testing
```

`Release` matches normal plugin packaging; `Testing` enables the testing configuration and trace constant. Use `dotnet build` after edits for a fast compile check. Validate behavior in a matching CN Dalamud/FFXIV client, especially emote playback, Penumbra swaps, IPC, and hook-dependent hotbar behavior. Run `git diff --check` before submitting.

## Coding Style & Naming Conventions

Follow `.editorconfig`: UTF-8, LF endings, final newlines, four spaces, and braces on new lines. Use file-scoped namespaces, PascalCase for types and public members, camelCase for private instance fields, and `On...` prefixes for events. Keep feature-specific code in its existing folder and preserve the plugin’s partial-class organization. Prefer the project’s existing logging, service, and ImGui helper patterns over introducing parallel abstractions.

## Testing Guidelines

No unit-test framework or coverage threshold is configured. Treat a successful `dotnet build` as the baseline, then perform focused manual checks in-game and record the client/build when touching hooks, Dalamud APIs, or patch approval behavior. Remove temporary probes before release.

## Commit & Pull Request Guidelines

Existing commits use short, imperative or direct descriptive subjects, for example `Update README.md` and `Add TMB swapper`. Follow that style, keep each commit focused, and mention user-visible behavior or compatibility changes in the body when needed. Pull requests should explain the motivation, affected areas, build configuration tested, and manual in-game validation. Include relevant `/xllog` output or screenshots for UI/runtime issues, and call out any Dalamud, Penumbra, or game-build dependency changes.

## Security & Configuration Tips

Do not commit personal Dalamud logs, generated plugin output, credentials, or private debug archives. Treat `patch-approval.json` and hook changes as compatibility-sensitive; verify target game builds before enabling or changing native hooks.
