
:: ----------------- ::
::   build_wdr.bat   ::
:: ----------------- ::

:: Admin Check
echo "Checking for admin..."

:: Check for permissions
    IF "%PROCESSOR_ARCHITECTURE%" EQU "amd64" (
>nul 2>&1 "%SYSTEMROOT%\SysWOW64\cacls.exe" "%SYSTEMROOT%\SysWOW64\config\system"
) ELSE (
>nul 2>&1 "%SYSTEMROOT%\system32\cacls.exe" "%SYSTEMROOT%\system32\config\system"
)

:: If error flag set, we do not have admin.
if '%errorlevel%' NEQ '0' (
    echo Requesting administrative privileges...
    goto UACPrompt
) else ( goto gotAdmin )

:UACPrompt
    echo Set UAC = CreateObject^("Shell.Application"^) > "%temp%\getadmin.vbs"
    set params= %*
    echo UAC.ShellExecute "cmd.exe", "/c ""%~s0"" %params:"=""%", "", "runas", 1 >> "%temp%\getadmin.vbs"

    "%temp%\getadmin.vbs"
    del "%temp%\getadmin.vbs"
    exit /B

:gotAdmin
    pushd "%CD%"
    CD /D "%~dp0"

setlocal enabledelayedexpansion

:: Creating NOP Files
echo "Building NOP files..."
cd "%~dp0nop"

:: whiteday119.nop
call :CreateAndMoveNOP whiteday119

:: whiteday120.nop
call :CreateAndMoveNOP whiteday120

:: whiteday121.nop
call :CreateAndMoveNOP whiteday121

:: mod_beanbag101.nop
call :CreateAndMoveNOP mod_beanbag101

:: Moving NOP files
echo "Moving NOP files to NSIS data folder..."
cd "%~dp0"
move /y "nop\mod_beanbag101.nop" "NSIS\data\mod_beanbag101.nop"
move /y "nop\whiteday119.nop"    "NSIS\data\whiteday119.nop"
move /y "nop\whiteday120.nop"    "NSIS\data\whiteday120.nop"
move /y "nop\whiteday121.nop"    "NSIS\data\whiteday121.nop"

:: Creating NSIS Installers
echo "Cleaning build folder..."
mkdir "build"
del /Q /F "build\*"

echo "Creating NSIS installer (update)..."
"%programfiles(x86)%\NSIS\makensis.exe" /V2 "NSIS\wd.nsi"

echo "Moving NSIS installer to build folder..."
for %%i in ("NSIS\*.exe") do move /y "%%i" "build\"

echo "Creating NSIS installer (full)..."
"%programfiles(x86)%\NSIS\makensis.exe" /V2 "NSIS\wd_full.nsi"

echo "Moving NSIS installer to build folder..."
for %%i in ("NSIS\*.exe") do move /y "%%i" "build\"

echo "Done."

goto :eof

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
move /y "whiteday000.nop" "%~1.nop"

goto :eof
