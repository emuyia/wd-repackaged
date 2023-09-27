# White Day Repackaged

## Description

![wdlaunch.exe](https://i.imgur.com/Stu390Um.png)

White Day Repackaged is a project aimed at reviving the classic Korean abandonware horror game, White Day, released in 2001. This project repackages the original game with a custom launcher, which provides several enhancements to help ensure compatibility with modern hardware. It also includes many modifications to the game itself, including an English translation and various bug fixes.

This repository is intended to centralise all of these features into a Visual Studio solution, so that they can be maintained in one place, and built into a NSIS installer. It's also intended to allow anyone else to pick up my work on this mod in the future.

## Features

![Main Menu](https://i.imgur.com/Kcr0ndxm.jpg)

- **Modern Compatibility**: The repackaged game aims to run seamlessly on modern hardware by emulating Korean locale and wrapping Direct3D 8. AA and Texture Filtering are possible using dgVoodoo as well.
- **Translation**: 99.9% of the game has been translated into English, including textures. This translation can be switched on and off from the launcher, so you can play the game in original Korean as well.
- **Bug Fixes**: Various bugs from the original game have been fixed, including but not limited to crashes, softlocks and missing subtitles.

## Acknowledgments

This project uses software / source code from a few other projects:

- [Locale Emulator](https://github.com/xupefei/Locale-Emulator)
- [dgVoodoo](https://github.com/dege-diosg/dgVoodoo2)
- [d3d8to9](https://github.com/crosire/d3d8to9)
- [xdelta](https://github.com/jmacd/xdelta)

The [keyboard image](NSIS/data/console/keyboard.png) is credited to Yes0song and is licensed under [CC BY-SA 3.0](https://creativecommons.org/licenses/by-sa/3.0), via Wikimedia Commons.

## Usage

Although you can use this repository to build the installer yourself, you will still need a copy of the game to do it (as this repository does *not* include the game itself).

If you just want to play the game, you're better off using a prebuilt installer than trying to build it here.

## How To Build

To build the installer, build the "build_all" project in Visual Studio.

### Requirements
- [Visual Studio](https://visualstudio.microsoft.com/)
- [.NET Framework 4.8.1 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net481-developer-pack-offline-installer)
- [Nullsoft Scriptable Install System (NSIS)](https://nsis.sourceforge.io/Download)
	- [NSIS Registry Plug-In](https://nsis.sourceforge.io/Registry_plug-in#Links)
	- [NSIS ShellExecAsUser Plug-In](https://nsis.sourceforge.io/ShellExecAsUser_plug-in#Download)

Please note that this repository does not include the game itself, and as such many of the files needed to build are missing.

However, all of the necessary files are linked in the Solution Explorer so that you can source them yourself.

<details>
  <summary>Click here to see a breakdown of the files you will need to source, and where they need to go.</summary>

##### Location: `NSIS`:

Files | CRC32
--- | ---
`whiteday100.nop`	| `CBA1D94E`

##### Location: `NSIS\data`:

Files | CRC32
--- | ---
`whiteday101.nop` | `65E56DB2`
`whiteday102.nop` | `FC950EB2`
`whiteday103.nop` | `72F3B4A5`
`whiteday110.nop` | `D81296E5`
`whiteday111.nop` | `36F8FB7D`
`whiteday112.nop` | `3890E262`
`whiteday113.nop` | `7CD4C86E`
`whiteday115.nop` | `D5B918AD`
`mod_beanbag099.nop`	| `22A94FB7`
`mod_beanbag100.nop`	| `28F5B051`
`skinpreview.exe` | `150FA18A`
`wangdx7.dll` | `7DD2AE3E`
`wangreal.dll` | `6B0FED72`
`ijl15.dll` | `876FDDA6`
`Mss32.dll` | `33A84B71`

##### Location: `NSIS\data\custom`:

Files | CRC32
--- | ---
`user_player.bmp` | `B37DAFC4`
`user_suwee1.bmp` | `F3529C1A`
`user_suwee2.bmp` | `CACCE4EE`

##### Location: `NSIS\data\custom\guide`:

Files | CRC32
--- | ---
`user_player_guide.bmp` | `2341DAFB`
`user_player_wire.bmp` | `F9703A0A`
`user_player_suwee1_guide.bmp` | `6F858C56`
`user_player_suwee1_wire.bmp` | `8FE0293C`
`user_player_suwee2_guide.bmp` | `C7EE7438`
`user_player_suwee2_wire.bmp` | `01427907`

##### Location: `NSIS\data\Mss`:

Files | CRC32
--- | ---
`Mp3dec.asi`		| `3F341B9F`
`Mssa3d.m3d`		| `2005F2DF`
`Mssa3d2.m3d`		| `22F1956A`
`Mssds3dh.m3d`	| `ED99E6FE`
`Mssds3ds.m3d`	| `D0B1BCA6`
`Mssdx7sh.m3d`	| `83355D11`
`Mssdx7sl.m3d`	| `891BF7A8`
`Mssdx7sn.m3d`	| `B0699622`
`Msseax.m3d`		| `60DD82C7`
`Msseax2.m3d`		| `2963BF34`
`Mssfast.m3d`		| `6270AFC5`
`Mssrsx.m3d`		| `E6B5EC39`
`Mssv12.asi`		| `2497535E`
`Mssv24.asi`		| `BD9C70DF`
`Mssv29.asi`		| `FAF94D32`

##### Location: `NSIS\data\mss65`:

Files | CRC32
--- | ---
`crc32_mdtable.exe` | `82089A9E`
`mssa3d.m3d` | `EED0959E`
`mssds3d.m3d` | `BF475838`
`mssdsp.flt` | `81DE6CB6`
`mssdx7.m3d` | `D4E9A26B`
`msseax.m3d` | `E8C80FC4`
`mssmp3.asi` | `2C408F87`
`mssrsx.m3d` | `A3790CBC`
`msssoft.m3d` | `F6C6AC0F`
`mssvoice.asi` | `6761ED9D`
`vssver.scc` | `7E2868DF`

##### Location: `wdhelper\patches\files_kr`:

Files | CRC32
--- | --- 
`Launcher.dll`		| `2B9A5288`
`mod_beanbag.dll`	| `98D80CF2`
`WhiteDay.dll`		| `B8A2476B`
`whiteday.exe`		| `C787D5F6`
`WhiteDay_p4.dll`	| `270D3524`

After obtaining `Launcher.dll`, `mod_beanbag.dll`, `WhiteDay.dll`, `whiteday.exe` and `WhiteDay_p4.dll`, patch them with the k2e vcdiff files in `wdhelper\patches` to get the English versions. Then, place the English files in both `wdhelper\patches\files_en` and `NSIS\data` to complete the solution.
</details>

## License

More details on the licenses of the used software [Locale-Emulator](licenses/Locale-Emulator.md), [d3d8to9](licenses/d3d8to9.md), and [xdelta](licenses/xdelta.md) can be found in the respective files. dgVoodoo has been used with permission from the developer.

## Contact Information

- [Discord](https://discord.com/users/92902509584072704)
- [ModDB](https://www.moddb.com/members/emuyia)
- [Email](emuyiahere@gmail.com)

Feel free to join the [White Day Community Discord Server](https://discord.gg/Fp7ywEm).
