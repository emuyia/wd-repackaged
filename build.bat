@echo off
:: This script requires admin privileges

setlocal enabledelayedexpansion

:: Check for admin
net session >nul 2>&1
if errorlevel 1 (
    echo ERROR: This script must be run as Administrator
    echo Please use build.ps1 which will auto-elevate, or run as admin
    pause
    exit /b 1
)

set CONFIG=Release

:: Get version from env variable set by build.ps1
if not defined WDR_VERSION set WDR_VERSION=0.00

echo.
echo ================================================
echo White Day Repackaged - Build Script
echo ================================================
echo.

:: Store solution directory
set SOLUTION_DIR=%~dp0
cd /d "%SOLUTION_DIR%"

:: Locate MSBuild using vswhere if available
echo Locating MSBuild...
set MSBUILD=

if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" (
    for /f "usebackq delims=" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe -prerelease`) do (
        set MSBUILD=%%i
    )
)

if not defined MSBUILD (
    echo ERROR: Could not find MSBuild
    echo Please install Visual Studio or MSBuild Tools
    goto BuildFailed
)

echo Found MSBuild: !MSBUILD!

echo.
echo ================================================
echo [1/4] Building WDHelper
echo ================================================
echo.

echo [WDHelper Pre-build] Copying DLLs to NSIS data...
xcopy /Y /S /E "wdhelper\patches\files_en\*.*" "NSIS\data\"
if errorlevel 1 (
    echo ERROR: Failed to copy English DLLs to NSIS data
    goto BuildFailed
)

:: Check if REBUILD_PATCHES flag is set
if defined REBUILD_PATCHES (
    echo [WDHelper Pre-build] Creating DLL patches...
    cd "wdhelper\patches"

    echo "EN > KR..."
    "xdelta3.exe" -e -f -s "%~dp0wdhelper\patches\files_en\Launcher.dll" 	"%~dp0wdhelper\patches\files_kr\Launcher.dll" 	    "%~dp0wdhelper\patches\e2k.Launcher.dll.vcdiff"
    "xdelta3.exe" -e -f -s "%~dp0wdhelper\patches\files_en\WhiteDay.dll" 	"%~dp0wdhelper\patches\files_kr\WhiteDay.dll" 	    "%~dp0wdhelper\patches\e2k.WhiteDay.dll.vcdiff"
    "xdelta3.exe" -e -f -s "%~dp0wdhelper\patches\files_en\WhiteDay_p4.dll"	"%~dp0wdhelper\patches\files_kr\WhiteDay_p4.dll"	"%~dp0wdhelper\patches\e2k.WhiteDay_p4.dll.vcdiff"
    "xdelta3.exe" -e -f -s "%~dp0wdhelper\patches\files_en\whiteday.exe" 	"%~dp0wdhelper\patches\files_kr\whiteday.exe" 	    "%~dp0wdhelper\patches\e2k.whiteday.exe.vcdiff"
    "xdelta3.exe" -e -f -s "%~dp0wdhelper\patches\files_en\mod_beanbag.dll" "%~dp0wdhelper\patches\files_kr\mod_beanbag.dll"    "%~dp0wdhelper\patches\e2k.mod_beanbag.dll.vcdiff"
    echo "Done."

    echo "KR > EN..."
    "xdelta3.exe" -e -f -s "%~dp0wdhelper\patches\files_kr\Launcher.dll" 	"%~dp0wdhelper\patches\files_en\Launcher.dll" 	    "%~dp0wdhelper\patches\k2e.Launcher.dll.vcdiff"
    "xdelta3.exe" -e -f -s "%~dp0wdhelper\patches\files_kr\WhiteDay.dll" 	"%~dp0wdhelper\patches\files_en\WhiteDay.dll" 	    "%~dp0wdhelper\patches\k2e.WhiteDay.dll.vcdiff"
    "xdelta3.exe" -e -f -s "%~dp0wdhelper\patches\files_kr\WhiteDay_p4.dll" "%~dp0wdhelper\patches\files_en\WhiteDay_p4.dll"    "%~dp0wdhelper\patches\k2e.WhiteDay_p4.dll.vcdiff"
    "xdelta3.exe" -e -f -s "%~dp0wdhelper\patches\files_kr\whiteday.exe" 	"%~dp0wdhelper\patches\files_en\whiteday.exe" 	    "%~dp0wdhelper\patches\k2e.whiteday.exe.vcdiff"
    "xdelta3.exe" -e -f -s "%~dp0wdhelper\patches\files_kr\mod_beanbag.dll" "%~dp0wdhelper\patches\files_en\mod_beanbag.dll"    "%~dp0wdhelper\patches\k2e.mod_beanbag.dll.vcdiff"
    echo "Done."

    cd "%SOLUTION_DIR%"
) else (
    echo [WDHelper Pre-build] Skipping DLL patch generation (use REBUILD_PATCHES=1 to rebuild)
)

echo [WDHelper Build] Compiling WDHelper.csproj...
"!MSBUILD!" "wdhelper\WDHelper.csproj" /p:Configuration=%CONFIG% /p:PreBuildEvent= /p:PostBuildEvent= /v:minimal /nologo
if errorlevel 1 (
    echo ERROR: Failed to build WDHelper project
    goto BuildFailed
)

echo.
echo ================================================
echo [2/4] Building WDLaunch
echo ================================================
echo.

echo [WDLaunch Pre-build] Copying wdhelper.exe to wdlaunch project files...
xcopy /Y "wdhelper\bin\%CONFIG%\wdhelper.exe" "wdlaunch\Resources\"
if errorlevel 1 (
    echo ERROR: Failed to copy wdhelper.exe to wdlaunch resources
    goto BuildFailed
)

echo [WDLaunch Pre-build] Copying dependencies to build directory...
xcopy /Y /S "wdlaunch\Dependencies\*.*" "wdlaunch\bin\%CONFIG%\"
if errorlevel 1 (
    echo ERROR: Failed to copy dependencies
    goto BuildFailed
)

echo [WDLaunch Build] Compiling WDLaunch.csproj...
"!MSBUILD!" "wdlaunch\WDLaunch.csproj" /p:Configuration=%CONFIG% /p:PreBuildEvent= /p:PostBuildEvent= /v:minimal /nologo
if errorlevel 1 (
    echo ERROR: Failed to build WDLaunch project
    goto BuildFailed
)

echo [WDLaunch Post-build] Cleaning up build artifacts...
del /Q "wdlaunch\bin\%CONFIG%\System.ValueTuple.*" 2>nul
rmdir /s /q "wdlaunch\bin\%CONFIG%\listnetworks" 2>nul

echo [WDLaunch Post-build] Copying wdlaunch build to NSIS data...
xcopy /Y /S /E "wdlaunch\bin\%CONFIG%\*.*" "NSIS\data\"
if errorlevel 1 (
    echo ERROR: Failed to copy wdlaunch build to NSIS data
    goto BuildFailed
)

echo [WDLaunch Post-build] Cleaning pdb files from NSIS data...
del /Q /S "NSIS\data\*.pdb" 2>nul

echo.
echo ================================================
echo [3/4] Building NOP Files
echo ================================================
echo.

cd "%~dp0nop"

call :CreateAndMoveNOP whiteday119

call :CreateAndMoveNOP whiteday120

call :CreateAndMoveNOP whiteday121

call :CreateAndMoveNOP mod_beanbag101

call :CreateAndMoveNOP mod_beanbag102

echo "Moving NOP files to NSIS data folder..."
cd "%~dp0"
move /y "nop\mod_beanbag101.nop" "NSIS\data\mod_beanbag101.nop"
if errorlevel 1 (
    echo ERROR: Failed to move mod_beanbag101.nop
    goto BuildFailed
)
move /y "nop\mod_beanbag102.nop" "NSIS\data\mod_beanbag102.nop"
if errorlevel 1 (
    echo ERROR: Failed to move mod_beanbag102.nop
    goto BuildFailed
)
move /y "nop\whiteday119.nop"    "NSIS\data\whiteday119.nop"
if errorlevel 1 (
    echo ERROR: Failed to move whiteday119.nop
    goto BuildFailed
)
move /y "nop\whiteday120.nop"    "NSIS\data\whiteday120.nop"
if errorlevel 1 (
    echo ERROR: Failed to move whiteday120.nop
    goto BuildFailed
)
move /y "nop\whiteday121.nop"    "NSIS\data\whiteday121.nop"
if errorlevel 1 (
    echo ERROR: Failed to move whiteday121.nop
    goto BuildFailed
)

echo.
echo ================================================
echo [4/4] Building NSIS Installers
echo ================================================
echo.

echo "Cleaning build folder..."
if not exist "build" mkdir "build"
del /Q /F "build\*" 2>nul

echo "Creating NSIS installer (update)..."
"%programfiles(x86)%\NSIS\makensis.exe" /V4 /DVERSION=%WDR_VERSION% "NSIS\wd.nsi"

echo "Moving NSIS installer to build folder..."
for %%i in ("NSIS\*.exe") do move /y "%%i" "build\"

echo "Creating NSIS installer (full)..."
"%programfiles(x86)%\NSIS\makensis.exe" /V4 /DVERSION=%WDR_VERSION% "NSIS\wd_full.nsi"

echo "Moving NSIS installer to build folder..."
for %%i in ("NSIS\*.exe") do move /y "%%i" "build\"

echo.
echo ================================================
echo Build completed
echo ================================================
echo.

exit /b 0

:: Subroutine for creating and moving NOP files
:CreateAndMoveNOP
echo "%~1.nop..."
LEProc.exe -run "noppack.exe" %~1\data %~1\script
:CHECK_RUNNING
    set errorlevel=
    tasklist /fi "imagename eq noppack.exe" | find /i "noppack.exe"
    if /i %errorlevel% GTR 0 goto CONTINUE
    ping -n 1 -w 5000 1.1.1.1 > nul
    goto CHECK_RUNNING
:CONTINUE

:: Check if noppack actually created the file
if not exist "whiteday000.nop" (
    echo ERROR: whiteday000.nop was not created for %~1
    echo noppack.exe may have crashed or failed
    cd "%SOLUTION_DIR%"
    goto BuildFailed
)

move /y "whiteday000.nop" "%~1.nop"
if errorlevel 1 (
    echo ERROR: Failed to rename whiteday000.nop to %~1.nop
    cd "%SOLUTION_DIR%"
    goto BuildFailed
)

:: Wait 3 seconds to let Locale Emulator fully clean up before next NOP creation
ping -n 4 -w 1000 127.0.0.1 > nul

goto :eof

:BuildFailed
echo.
echo ================================================
echo Build failed
echo ================================================
echo.
pause
exit /b 1
