param(
    [string]$FanControlDirectory = 'C:\Program Files (x86)\FanControl'
)

$ErrorActionPreference = 'Stop'
$targetExecutable = Join-Path $FanControlDirectory 'FanControl.AutoUpdater.exe'

$identity = [Security.Principal.WindowsIdentity]::GetCurrent()
$principal = [Security.Principal.WindowsPrincipal]::new($identity)
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    $arguments = @(
        '-NoProfile',
        '-ExecutionPolicy', 'Bypass',
        '-File', "`"$PSCommandPath`"",
        '-FanControlDirectory', "`"$FanControlDirectory`""
    )
    $elevated = Start-Process -FilePath 'powershell.exe' -Verb RunAs -ArgumentList $arguments -Wait -PassThru
    exit $elevated.ExitCode
}

if (Test-Path -LiteralPath $targetExecutable) {
    $configuration = Start-Process -FilePath $targetExecutable -ArgumentList '--disable' -Wait -PassThru
    if ($configuration.ExitCode -ne 0) {
        throw "FanControl.AutoUpdater configuration failed with exit code $($configuration.ExitCode)."
    }
}

Remove-Item -LiteralPath $targetExecutable -Force -ErrorAction SilentlyContinue
Remove-Item -LiteralPath (Join-Path $FanControlDirectory 'FanControl.AutoUpdater.json') -Force -ErrorAction SilentlyContinue
Remove-Item -LiteralPath (Join-Path $FanControlDirectory 'FanControl.AutoUpdater.log') -Force -ErrorAction SilentlyContinue
