param(
    [string]$FanControlDirectory = 'C:\Program Files (x86)\FanControl',
    [switch]$Enable
)

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $projectRoot 'src\FanControl.AutoUpdater\FanControl.AutoUpdater.csproj'
$publishDirectory = Join-Path $projectRoot 'artifacts\FanControl.AutoUpdater'
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
    if ($Enable) { $arguments += '-Enable' }
    $elevated = Start-Process -FilePath 'powershell.exe' -Verb RunAs -ArgumentList $arguments -Wait -PassThru
    exit $elevated.ExitCode
}

if (-not (Test-Path -LiteralPath (Join-Path $FanControlDirectory 'FanControl.exe'))) {
    throw "FanControl.exe was not found in $FanControlDirectory"
}

$existingAutoUpdaters = Get-Process -Name 'FanControl.AutoUpdater' -ErrorAction SilentlyContinue
foreach ($process in $existingAutoUpdaters) {
    Get-CimInstance Win32_Process -Filter "ParentProcessId = $($process.Id)" -ErrorAction SilentlyContinue |
        ForEach-Object { Invoke-CimMethod -InputObject $_ -MethodName Terminate -ErrorAction SilentlyContinue | Out-Null }
    Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
}

dotnet publish $project -c Release -r win-x64 --self-contained false -o $publishDirectory
if ($LASTEXITCODE -ne 0) {
    throw 'dotnet publish failed.'
}

Copy-Item -LiteralPath (Join-Path $publishDirectory 'FanControl.AutoUpdater.exe') -Destination $targetExecutable -Force
$configuration = if ($Enable) {
    Start-Process -FilePath $targetExecutable -ArgumentList '--enable' -Wait -PassThru
} else {
    Start-Process -FilePath $targetExecutable -Wait -PassThru
}
if ($configuration.ExitCode -ne 0) {
    throw "FanControl.AutoUpdater configuration failed with exit code $($configuration.ExitCode)."
}
