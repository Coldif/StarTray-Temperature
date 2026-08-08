<div align="center">
  <img src="https://github.com/user-attachments/assets/65abf490-58d8-42d7-acb4-bd4e593dbdf7" alt="StarTray" height="200">
</div>


## StarTray

**StarTray** - super lightweight, aesthetic, and easy-to-use open-source application for monitoring your computer's processor's and graphics card's temperatures from the system tray.

**Supports**: Windows 10 and Windows 11 64-bit (x64) operating systems.

**Original developed and designed by** [@justinnas](https://github.com/justinnas)

**This fork was modified with AI assistance** to add multi-device support and new features.

<br>

## What's New in v1.2

- **Multi-CPU Support** - Each detected CPU gets its own independent tray icon
- **Multi-GPU Support** - Each detected GPU gets its own independent tray icon
- **Thermal Theme** - Dynamic color that changes based on temperature (blue < 40°C, green < 60°C, yellow < 70°C, orange < 80°C, red > 80°C)
- **Per-device themes** - Each icon can have its own independent theme
- **Fixed Exit crash** - Resolved application crash when clicking Exit
- **Hidden main window** - Application now runs as a pure tray application

<br>

## Download

You can download the latest version from [GitHub Releases](https://github.com/Coldif/StarTray-Temperature/releases). Choose between the installer or the portable version.

<br>

## Installation

### Installer (Recommended)
1. Download `StarTraySetup-1.2.exe` from [Releases](https://github.com/Coldif/StarTray-Temperature/releases)
2. Run the installer as Administrator
3. Follow the installation wizard
4. StarTray will start automatically after installation

### Portable
1. Download `StarTray-Portable-1.2.zip` from [Releases](https://github.com/Coldif/StarTray-Temperature/releases)
2. Extract to any folder
3. Run `StarTray.exe` as Administrator

<br>

## Usage

After launching, you will see CPU and/or GPU icons in the system tray. Right-click any icon to open its menu.

### Menu Panel

**Theme** *(Per icon)*
- Change icon theme independently for each device

**Options** *(Global)*
- Show/Hide GPU or CPU icons
- Run on Startup
- Change Temperature Units (Celsius/Fahrenheit)

**Info** *(Per icon)*
- View hardware information

**Exit**
- Close the application

<br>

## Building from Source

**Requirements:**
- Visual Studio 2022 or MSBuild
- .NET Framework 4.7.2
- Inno Setup 6 (for installer)

**Build:**
```
MSBuild StarTrayTemperature.sln /p:Configuration=Release
```

**Create Installer:**
```
ISCC.exe setup\StarTray.iss
```

<br>

## License

This project uses the following libraries: LibreHardwareMonitorLib (MPL 2.0 License), HidSharp (Apache License), System.CodeDom (MIT License), System.Management (MIT License), TaskScheduler (MIT License).

This project uses the Open Sans font, designed by Steve Matteson, licensed under SIL Open Font License, Version 1.1.

StarTray is licensed under GNU General Public License v3.0 license, please see the [license file](https://github.com/Coldif/StarTray-Temperature/blob/main/LICENSE.txt) for more details.
