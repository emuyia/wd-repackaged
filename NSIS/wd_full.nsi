; -----------------------------------------------------------
;     W H I T E  D A Y  full install
;     wd_full.nsi
; -----------------------------------------------------------

; includes
!include LogicLib.nsh
!include Registry.nsh
!include MUI2.nsh
!include WordFunc.nsh
Unicode True
!insertmacro WordReplace

; Version - overridden with /DVERSION=x.xx from cli
!ifndef VERSION
  !define VERSION "0.00"
!endif

; attributes
Name "White Day Repackaged"
OutFile "wdr_setup_${VERSION}.exe"
RequestExecutionLevel highest
InstallDir $LOCALAPPDATA\WhiteDay
ShowInstDetails nevershow
ShowUninstDetails nevershow
BrandingText " "
Caption "White Day: a labyrinth named School"
UninstallCaption "White Day: a labyrinth named School"

!define MUI_ICON "wd_full.ico"
!define MUI_UNICON "wd_un.ico"
!define MUI_BGCOLOR "DBDBDB"

!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_LANGUAGE "English"

Section install

	SetOutPath $INSTDIR
	
	; write files
	File /r data\*.*
	File whiteday100.nop
	
	; write registry entries
	Var /GLOBAL regSuccess
	${registry::CreateKey} "HKEY_CURRENT_USER\Software\Sonnori" $regSuccess
	${registry::CreateKey} "HKEY_CURRENT_USER\Software\Sonnori\AutoUpdate" $regSuccess
	${registry::CreateKey} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay" $regSuccess
	${registry::CreateKey} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay\KeySet" $regSuccess
	${registry::CreateKey} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay\Option" $regSuccess
	${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\AutoUpdate" "date" "20120531" "REG_SZ" $regSuccess
	${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\AutoUpdate" "Notice" "1" "REG_SZ" $regSuccess
	${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay" "Name" "113048116048117048116048099049105056098049102053099050126054122052141057124049107057" "REG_SZ" $regSuccess
	${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay" "Path" $INSTDIR "REG_SZ" $regSuccess
	${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay" "version" "1.00" "REG_SZ" $regSuccess
	${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay" "newversion" "1.18" "REG_SZ" $regSuccess ; KOR VERSION
	${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay" "engversion" "${VERSION}" "REG_SZ" $regSuccess ; ENG VERSION
	${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay" "Language" "English" "REG_SZ" $regSuccess
	; ${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay\Option" "NowComplete" "111 93 160" "REG_SZ" $regSuccess
	${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay\Option" "lang" "0" "REG_SZ" $regSuccess
	${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay\Option" "SoundVolume" "1.000000" "REG_SZ" $regSuccess
	${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay\Option" "BGMVolume" "1.000000" "REG_SZ" $regSuccess
	${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay\Option" "Bright" "0.000000" "REG_SZ" $regSuccess
	${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay\Option" "ProjectionShadow" "1" "REG_SZ" $regSuccess
	${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay\Option" "Antial" "1" "REG_SZ" $regSuccess
	; ${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay\Option" "CostumeChange" "113 93 150" "REG_SZ" $regSuccess
	; ${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay\Option" "CostumeShow" "0" "REG_SZ" $regSuccess
	; ${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay\Option" "PatrolManPlay" "113 93 126" "REG_SZ" $regSuccess
	; ${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay\Option" "PatrolManChange" "0" "REG_SZ" $regSuccess
	${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay\KeySet" "Bag" "{key keystate 15} {key keystate 9} {mouse button 2}" "REG_SZ" $regSuccess
	; ${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay\KeySet" "Escape" "{key keystate 27} {key keystate 84} " "REG_SZ" $regSuccess
	${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay\KeySet" "PrintScreen" "{key keystate 132} {key keystate 25}" "REG_SZ" $regSuccess
	${registry::Write} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay\KeySet" "Console" "{key keystate 96}" "REG_SZ" $regSuccess
	
	; remove official files
	RMDir /r "$INSTDIR\help"
	Delete "$INSTDIR\custom\À¯ÀúÄ¿½ºÅÒ½ºÅ²¸¸µå´Â¹ý.txt"
	
	; remove obsolete files, pre wdr
	Delete "$INSTDIR\alm.exe"
	Delete "$INSTDIR\applocale.exe"
	Delete "$INSTDIR\changelog.txt"
	Delete "$INSTDIR\d3d8.dll0"
	Delete "$INSTDIR\enbconvertor.ini"
	Delete "$INSTDIR\ico 1.ico"
	Delete "$INSTDIR\ico 2.ico"
	Delete "$INSTDIR\ico.ico"
	Delete "$INSTDIR\ico-install.ico"
	Delete "$INSTDIR\le.ini"
	Delete "$INSTDIR\Oh!jaemi Instructions.txt"
	Delete "$INSTDIR\mod_beanbag122.nop"
	Delete "$INSTDIR\whiteday116.nop"
	Delete "$INSTDIR\whiteday117.nop"
	Delete "$INSTDIR\whiteday118.nop"
	Delete "$INSTDIR\whiteday122.nop"
	Delete "$INSTDIR\Uninstall HF pAppLoc.lnk"
	Delete "$INSTDIR\Uninstall White Day.exe"
	Delete "$INSTDIR\Uninstall.exe"
	Delete "$INSTDIR\UninstallLE.exe"
	Delete "$INSTDIR\wdem.exe"
	Delete "$INSTDIR\WhiteDay - Start.exe"
	Delete "$INSTDIR\whiteday.reg"
	RMDir /r /REBOOTOK "$INSTDIR\LE"
	RMDir /r /REBOOTOK "$INSTDIR\dxwnd"
	RMDir /r "$INSTDIR\mp3 player"
	; console
	Delete "$INSTDIR\console\changelog.txt"
	Delete "$INSTDIR\console\Commands.docx"
	Delete "$INSTDIR\console\Custom Skins.htm"
	Delete "$INSTDIR\console\Instructions.htm"
	RMDir /r "$INSTDIR\console\Cheat Patch"
	RMDir /r /REBOOTOK "$INSTDIR\console\dxwnd"
	RMDir /r /REBOOTOK "$INSTDIR\console\LE"
	RMDir /r "$INSTDIR\console\NoCheat Patch"
	RMDir /r "$INSTDIR\console\Other"
	; custom
	Delete "$INSTDIR\custom\guide\user_player_guide.png"
	Delete "$INSTDIR\custom\guide\user_player_wire.png"
	Delete "$INSTDIR\custom\guide\user_suwee1_guide.png"
	Delete "$INSTDIR\custom\guide\user_suwee1_wire.png"
	Delete "$INSTDIR\custom\guide\user_suwee2_guide.png"
	Delete "$INSTDIR\custom\guide\user_suwee2_wire.png"
	Delete "$INSTDIR\custom\Custom Skins.htm"
	Delete "$INSTDIR\custom.htm"
	; mss65
	Delete "$INSTDIR\mss65\whiteday.reg"
	Delete "$INSTDIR\mss65\whitedayreg.exe"
	
	; remove obsolete files, post 17
	Delete "$INSTDIR\whiteday120.bak"
	Delete "$INSTDIR\whiteday121.bak"
	Delete "$INSTDIR\mod_beanbag101.bak"
	Delete "$INSTDIR\console\commands.txt"
	
	; create uninstaller
	WriteUninstaller "$INSTDIR\uninstall.exe"
	
	; create new shortcuts
	SetShellVarContext all
	RMDir /r "$STARTMENU\Programs\White Day"
	CreateDirectory "$STARTMENU\Programs\White Day"
	CreateShortcut "$STARTMENU\Programs\White Day\White Day.lnk" "$INSTDIR\wdlaunch.exe" "" "$INSTDIR\whiteday.exe" 0
	CreateShortcut "$DESKTOP\White Day.lnk" "$INSTDIR\wdlaunch.exe" "" "$INSTDIR\whiteday.exe" 0
	CreateShortcut "$STARTMENU\Programs\White Day\Uninstall White Day.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
	
	ShellExecAsUser::ShellExecAsUser "" "$INSTDIR\wdlaunch.exe" "" ; run in non-admin mode to avoid save file location confusion
	
	Quit

SectionEnd

; uninstall script
Section uninstall

	MessageBox MB_YESNO "Are you sure you want to uninstall White Day?" IDYES yes IDNO no
	
	yes:
	; remove directory
	RMDir /r /REBOOTOK $INSTDIR
	
	; remove shortcuts
	SetShellVarContext current
	RMDir /r /REBOOTOK "$LOCALAPPDATA\VirtualStore\Program Files (x86)\whiteday" ; non-admin save file location
	Delete "$DESKTOP\White Day.lnk"
	SetShellVarContext all
	RMDir /r /REBOOTOK "$STARTMENU\Programs\White Day"
	
	; delete registry entries
	Var /GLOBAL regDelSuccess
	${registry::DeleteKey} "HKEY_CURRENT_USER\Software\Sonnori\WhiteDay" $regDelSuccess
	${registry::DeleteKey} "HKEY_CURRENT_USER\Software\Sonnori\AutoUpdate" $regDelSuccess
	
	no:
	Quit
	
SectionEnd
