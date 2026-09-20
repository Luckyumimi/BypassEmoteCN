#requires -Version 7
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$project = Join-Path $root 'BypassEmote\BypassEmote.csproj'
$source = Join-Path $root 'BypassEmote'
$table = Join-Path $root 'translations.tsv'
$generated = Join-Path $root 'BypassEmote\obj\CnGenerated'
$env:DALAMUD_HOME = "$env:APPDATA\XIVLauncherCN\addon\Hooks\dev"
dotnet run --project (Join-Path $PSScriptRoot 'LocalizationGenerator\LocalizationGenerator.csproj') -- $source $table $generated
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build $project -c Release -p:UseCnGeneratedSources=true
exit $LASTEXITCODE
