# 本仓库是 Bypass Emote 的汉化 + 国服适配分支

同步上游、打包、以及「哪些 fork 改动绝不能丢」的完整说明都在 **[SYNC.md](SYNC.md)**。

同步上游时先跑这一条，然后只读它的输出（约 300 tokens），不要读全量 diff：

```powershell
pwsh -File tools/sync-upstream.ps1
```

汉化完整性只信 `tools/l10n-check.ps1` 的输出；危险写法、版本号规则、定义完成都在 SYNC.md 里。
