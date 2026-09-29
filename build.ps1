$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$csc = Join-Path $env:WINDIR "Microsoft.NET\Framework64\v4.0.30319\csc.exe"
$out = Join-Path $root "bin"
New-Item -ItemType Directory -Force -Path $out | Out-Null
& $csc /nologo /platform:x64 /target:winexe `
    /reference:System.Windows.Forms.dll `
    /reference:System.Drawing.dll `
    /out:"$out\MiddleScroll.exe" `
    "$root\MiddleScroll.cs"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Output "Built $out\MiddleScroll.exe"
