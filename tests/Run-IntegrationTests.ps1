$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$buildDirectory = Join-Path $repositoryRoot 'src\FanControl.AutoUpdater\bin\Release\net10.0-windows\win-x64'
$runtimeDirectory = Join-Path $repositoryRoot 'artifacts\tests\runtime'
$fixtures = Join-Path $PSScriptRoot 'fixtures'
$installedFanControl = 'C:\Program Files (x86)\FanControl'

if (-not (Test-Path -LiteralPath (Join-Path $buildDirectory 'FanControl.AutoUpdater.dll'))) {
    throw 'Build output is missing. Run dotnet build -c Release first.'
}

New-Item -ItemType Directory -Path $runtimeDirectory -Force | Out-Null
Copy-Item -LiteralPath (Join-Path $buildDirectory 'FanControl.AutoUpdater.dll') -Destination $runtimeDirectory -Force
Copy-Item -LiteralPath (Join-Path $buildDirectory 'FanControl.AutoUpdater.deps.json') -Destination $runtimeDirectory -Force
Copy-Item -LiteralPath (Join-Path $buildDirectory 'FanControl.AutoUpdater.runtimeconfig.json') -Destination $runtimeDirectory -Force
Copy-Item -LiteralPath (Join-Path $installedFanControl 'FanControl.exe') -Destination $runtimeDirectory -Force
Copy-Item -LiteralPath (Join-Path $installedFanControl 'Updater.exe') -Destination $runtimeDirectory -Force
Copy-Item -LiteralPath (Join-Path $fixtures 'settings-enabled.json') -Destination (Join-Path $runtimeDirectory 'FanControl.AutoUpdater.json') -Force

$runner = Join-Path $runtimeDirectory 'FanControl.AutoUpdater.dll'

function Invoke-Case {
    param(
        [string]$Name,
        [string]$VersionFixture,
        [string]$ReleaseFixture,
        [int]$ExpectedExitCode
    )

    $env:FANCONTROL_AUTOUPDATE_VERSION_URL = Join-Path $fixtures $VersionFixture
    $env:FANCONTROL_AUTOUPDATE_RELEASE_URL = Join-Path $fixtures $ReleaseFixture
    & dotnet $runner --check-only
    $actual = $LASTEXITCODE
    if ($actual -ne $ExpectedExitCode) {
        throw "$Name failed: expected exit code $ExpectedExitCode, got $actual."
    }
    Write-Host "PASS $Name ($actual)"
}

function Write-VersionFixture {
    param(
        [string]$Path,
        [string]$UpdaterHash
    )

    @{
        Number = 999
        Checksums = @{
            Updater = $UpdaterHash
        }
    } | ConvertTo-Json -Depth 3 | Set-Content -LiteralPath $Path -Encoding utf8
}

function Invoke-UpdaterPreparationCase {
    param(
        [string]$Name,
        [string]$ExpectedHash,
        [int]$ExpectedExitCode,
        [bool]$ExpectUpdater
    )

    $generatedVersion = Join-Path $runtimeDirectory 'version-preparation.json'
    $runtimeUpdater = Join-Path $runtimeDirectory 'Updater.exe'
    $sourceUpdater = Join-Path $installedFanControl 'Updater.exe'

    Write-VersionFixture -Path $generatedVersion -UpdaterHash $ExpectedHash
    Remove-Item -LiteralPath $runtimeUpdater -Force -ErrorAction SilentlyContinue
    Remove-Item -LiteralPath "$runtimeUpdater.download" -Force -ErrorAction SilentlyContinue

    $env:FANCONTROL_AUTOUPDATE_VERSION_URL = $generatedVersion
    $env:FANCONTROL_AUTOUPDATE_RELEASE_URL = Join-Path $fixtures 'release-999.json'
    $env:FANCONTROL_AUTOUPDATE_UPDATER_URL = $sourceUpdater
    & dotnet $runner --prepare-only
    $actual = $LASTEXITCODE

    if ($actual -ne $ExpectedExitCode) {
        throw "$Name failed: expected exit code $ExpectedExitCode, got $actual."
    }
    if ((Test-Path -LiteralPath $runtimeUpdater) -ne $ExpectUpdater) {
        throw "$Name failed: unexpected Updater.exe presence."
    }
    if (Test-Path -LiteralPath "$runtimeUpdater.download") {
        throw "$Name failed: temporary download was not removed."
    }
    if ($ExpectUpdater -and (Get-FileHash -LiteralPath $runtimeUpdater -Algorithm MD5).Hash -ne $ExpectedHash) {
        throw "$Name failed: prepared Updater.exe hash does not match."
    }

    Write-Host "PASS $Name ($actual)"
}

try {
    Invoke-Case -Name 'current version' -VersionFixture 'version-current.json' -ReleaseFixture 'release-mismatch.json' -ExpectedExitCode 0
    Invoke-Case -Name 'valid update' -VersionFixture 'version-update.json' -ReleaseFixture 'release-999.json' -ExpectedExitCode 10
    Invoke-Case -Name 'release mismatch is rejected' -VersionFixture 'version-update.json' -ReleaseFixture 'release-mismatch.json' -ExpectedExitCode 1
    $officialUpdaterHash = (Get-FileHash -LiteralPath (Join-Path $installedFanControl 'Updater.exe') -Algorithm MD5).Hash
    Invoke-UpdaterPreparationCase -Name 'verified updater preparation' -ExpectedHash $officialUpdaterHash -ExpectedExitCode 10 -ExpectUpdater $true
    Invoke-UpdaterPreparationCase -Name 'bad updater checksum is rejected' -ExpectedHash ('0' * 32) -ExpectedExitCode 1 -ExpectUpdater $false
}
finally {
    Remove-Item Env:FANCONTROL_AUTOUPDATE_VERSION_URL -ErrorAction SilentlyContinue
    Remove-Item Env:FANCONTROL_AUTOUPDATE_RELEASE_URL -ErrorAction SilentlyContinue
    Remove-Item Env:FANCONTROL_AUTOUPDATE_UPDATER_URL -ErrorAction SilentlyContinue
}
