# Update on new release
$VERSION = "0.48"

# Check if running as administrator
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "ERROR: This script must be run as Administrator" -ForegroundColor Red
    Write-Host "Please open PowerShell as Administrator and run this script again" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "Running with administrator privileges" -ForegroundColor Green
Write-Host ""

# Call the batch file and log output
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$batPath = Join-Path $scriptDir "build.bat"
$logPath = Join-Path $scriptDir "build.log"

# Delete old log if it exists
if (Test-Path $logPath) {
    Remove-Item $logPath -Force
}

Write-Host "Logging build output to: $logPath" -ForegroundColor Cyan
Write-Host "Version: $VERSION" -ForegroundColor Cyan
Write-Host ""

# Set environment variable for build.bat
$env:WDR_VERSION = $VERSION

& cmd.exe /c "`"$batPath`"" 2>&1 | Tee-Object -FilePath $logPath

$exitCode = $LASTEXITCODE
Write-Host ""
if ($exitCode -eq 0) {
    Write-Host "Build completed successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Opening build folder..." -ForegroundColor Cyan
    $buildPath = Join-Path $scriptDir "build"
    Start-Process explorer.exe -ArgumentList $buildPath
} else {
    Write-Host "Build failed with exit code $exitCode" -ForegroundColor Red
}

Write-Host ""
Write-Host "Done." -ForegroundColor Yellow
