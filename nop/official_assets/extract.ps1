param(
    [switch]$Force  # force recompilation of em_nopunpack.exe
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = Resolve-Path (Join-Path $ScriptDir "..\..")

$NopDir = Join-Path $RepoRoot "nop"
$ToolsDir = Join-Path $NopDir "tools"
$NsisDir = Join-Path $RepoRoot "NSIS"
$NsisDataDir = Join-Path $NsisDir "data"
$OutputDir = $ScriptDir
$WhiteDayOutputDir = Join-Path $OutputDir "whiteday"
$BeanbagOutputDir = Join-Path $OutputDir "mod_beanbag"
$TempDir = Join-Path $OutputDir "_temp_nop"

$UnpackerSrc = Join-Path $ToolsDir "em_nopunpack.c"
$UnpackerExe = Join-Path $ToolsDir "em_nopunpack.exe"

$WhiteDayOfficialPackages = @(
    @{ Name = "whiteday100.nop"; Path = $NsisDir },
    @{ Name = "whiteday101.nop"; Path = $NsisDataDir },
    @{ Name = "whiteday102.nop"; Path = $NsisDataDir },
    @{ Name = "whiteday103.nop"; Path = $NsisDataDir },
    @{ Name = "whiteday110.nop"; Path = $NsisDataDir },
    @{ Name = "whiteday111.nop"; Path = $NsisDataDir },
    @{ Name = "whiteday112.nop"; Path = $NsisDataDir },
    @{ Name = "whiteday113.nop"; Path = $NsisDataDir },
    @{ Name = "whiteday115.nop"; Path = $NsisDataDir }
)

$BeanbagOfficialPackages = @(
    @{ Name = "mod_beanbag098.nop"; Path = $NsisDataDir },
    @{ Name = "mod_beanbag099.nop"; Path = $NsisDataDir },
    @{ Name = "mod_beanbag100.nop"; Path = $NsisDataDir }
)

function Write-Header($text) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host " $text" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
}

function Write-Status($text) {
    Write-Host "  -> $text" -ForegroundColor Gray
}

function Write-Success($text) {
    Write-Host "  [OK] $text" -ForegroundColor Green
}

function Write-Warning($text) {
    Write-Host "  [WARN] $text" -ForegroundColor Yellow
}

function Write-Error($text) {
    Write-Host "  [ERROR] $text" -ForegroundColor Red
}

# compile em_nopunpack.exe if needed
function Compile-Unpacker {
    if ((Test-Path $UnpackerExe) -and -not $Force) {
        Write-Status "em_nopunpack.exe already exists, skipping compilation"
        return $true
    }

    Write-Header "Compiling em_nopunpack.exe"

    # try MSVC (cl.exe) via Visual Studio Developer Command Prompt environment
    $vsWhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path $vsWhere) {
        $vsPath = & $vsWhere -latest -property installationPath 2>$null
        if ($vsPath) {
            $vcvarsall = Join-Path $vsPath "VC\Auxiliary\Build\vcvarsall.bat"
            if (Test-Path $vcvarsall) {
                Write-Status "Found Visual Studio, using cl.exe"

                # create a batch file to compile using MSVC
                $batchContent = @"
@echo off
call "$vcvarsall" x86 >nul 2>&1
cd /d "$ToolsDir"
cl.exe /nologo /O2 /Fe:"$UnpackerExe" "$UnpackerSrc"
exit /b %ERRORLEVEL%
"@
                $batchFile = Join-Path $env:TEMP "compile_nopunpack.bat"
                Set-Content -Path $batchFile -Value $batchContent -Encoding ASCII

                & cmd.exe /c $batchFile
                $compileResult = $LASTEXITCODE
                Remove-Item $batchFile -Force -ErrorAction SilentlyContinue

                if ($compileResult -eq 0 -and (Test-Path $UnpackerExe)) {
                    # clean up obj file
                    $objFile = Join-Path $ToolsDir "em_nopunpack.obj"
                    Remove-Item $objFile -Force -ErrorAction SilentlyContinue
                    Write-Success "Compiled successfully with cl.exe"
                    return $true
                }
            }
        }
    }

    # try GCC (MinGW)
    $gcc = Get-Command "gcc.exe" -ErrorAction SilentlyContinue
    if ($gcc) {
        Write-Status "Using GCC"
        Push-Location $ToolsDir
        & gcc.exe -O2 -o em_nopunpack.exe em_nopunpack.c
        Pop-Location
        if ($LASTEXITCODE -eq 0 -and (Test-Path $UnpackerExe)) {
            Write-Success "Compiled successfully with gcc"
            return $true
        }
    }

    # try Clang
    $clang = Get-Command "clang.exe" -ErrorAction SilentlyContinue
    if ($clang) {
        Write-Status "Using Clang"
        Push-Location $ToolsDir
        & clang.exe -O2 -o em_nopunpack.exe em_nopunpack.c
        Pop-Location
        if ($LASTEXITCODE -eq 0 -and (Test-Path $UnpackerExe)) {
            Write-Success "Compiled successfully with clang"
            return $true
        }
    }

    Write-Error "Could not find a C compiler (MSVC, GCC, or Clang)"
    Write-Error "Please install Visual Studio, MinGW, or Clang and try again"
    return $false
}

# clean and prepare output directories
function Prepare-OutputDirectories {
    Write-Header "Preparing output directories"

    if (Test-Path $WhiteDayOutputDir) {
        Write-Status "Removing existing $WhiteDayOutputDir"
        Remove-Item $WhiteDayOutputDir -Recurse -Force
    }
    if (Test-Path $BeanbagOutputDir) {
        Write-Status "Removing existing $BeanbagOutputDir"
        Remove-Item $BeanbagOutputDir -Recurse -Force
    }
    if (Test-Path $TempDir) {
        Write-Status "Removing temp directory"
        Remove-Item $TempDir -Recurse -Force
    }

    Write-Status "Creating output directories"
    New-Item -ItemType Directory -Path $WhiteDayOutputDir -Force | Out-Null
    New-Item -ItemType Directory -Path $BeanbagOutputDir -Force | Out-Null
    New-Item -ItemType Directory -Path $TempDir -Force | Out-Null

    Write-Success "Output directories prepared"
}

# extract NOP packages
function Extract-NopPackages($packages, $outputDir, $label) {
    Write-Header "Extracting $label packages"

    $extractedCount = 0
    $missingCount = 0

    foreach ($pkg in $packages) {
        $nopPath = Join-Path $pkg.Path $pkg.Name

        if (-not (Test-Path $nopPath)) {
            Write-Warning "$($pkg.Name) not found (skipping)"
            $missingCount++
            continue
        }

        Write-Status "Extracting $($pkg.Name)..."

        # run the unpacker
        $process = Start-Process -FilePath $UnpackerExe -ArgumentList "`"$nopPath`"", "`"$outputDir`"" -Wait -PassThru -NoNewWindow

        if ($process.ExitCode -eq 0) {
            Write-Success "$($pkg.Name) extracted"
            $extractedCount++
        } else {
            Write-Error "Failed to extract $($pkg.Name)"
        }
    }

    Write-Host ""
    Write-Host "  ${label}: $extractedCount extracted, $missingCount missing" -ForegroundColor White
}

# cleanup temp files
function Cleanup-TempFiles {
    Write-Header "Cleaning up"

    if (Test-Path $TempDir) {
        Write-Status "Removing temp directory"
        Remove-Item $TempDir -Recurse -Force
    }

    Write-Success "Cleanup complete"
}

Write-Host ""
Write-Host "=============================================" -ForegroundColor Magenta
Write-Host " extract.ps1" -ForegroundColor Magenta
Write-Host "=============================================" -ForegroundColor Magenta

# compile unpacker if needed
if (-not (Compile-Unpacker)) {
    exit 1
}

# wipe and recreate output directories
Prepare-OutputDirectories

# extract White Day packages
Extract-NopPackages $WhiteDayOfficialPackages $WhiteDayOutputDir "White Day"

# extract mod_beanbag packages
Extract-NopPackages $BeanbagOfficialPackages $BeanbagOutputDir "Oh!Jaemi"

# cleanup
Cleanup-TempFiles

Write-Host ""
Write-Host "=============================================" -ForegroundColor Magenta
Write-Host " Extraction complete!" -ForegroundColor Magenta
Write-Host "=============================================" -ForegroundColor Magenta
Write-Host ""
Write-Host "Output locations:" -ForegroundColor White
Write-Host "  White Day: $WhiteDayOutputDir" -ForegroundColor Gray
Write-Host "  Oh!Jaemi:  $BeanbagOutputDir" -ForegroundColor Gray
Write-Host ""
