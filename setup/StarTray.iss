[Setup]
AppName=StarTray Temperature
AppVersion=1.2
AppPublisher=Coldif
DefaultDirName={autopf}\StarTray Temperature
DefaultGroupName=StarTray Temperature
OutputDir=..\setup
OutputBaseFilename=StarTraySetup-1.2
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
DisableProgramGroupPage=yes
PrivilegesRequired=admin
UninstallDisplayIcon={app}\StarTray.exe
SetupIconFile=..\StarTrayTemperature\startray_icon.ico
VersionInfoVersion=1.2.0.0
VersionInfoCompany=Coldif
VersionInfoDescription=StarTray Temperature Setup
VersionInfoProductName=StarTray Temperature
VersionInfoProductVersion=1.2

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "startupicon"; Description: "Run on Windows startup"; GroupDescription: "Startup:"

[Files]
Source: "..\StarTrayTemperature\bin\Release\StarTray.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\StarTrayTemperature\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\StarTrayTemperature\bin\Release\StarTray.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\StarTrayTemperature\bin\Release\Resources\*"; DestDir: "{app}\Resources"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\Licenses\*"; DestDir: "{app}\Licenses"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\StarTrayTemperature\bin\Release\de\*"; DestDir: "{app}\de"; Flags: ignoreversion
Source: "..\StarTrayTemperature\bin\Release\es\*"; DestDir: "{app}\es"; Flags: ignoreversion
Source: "..\StarTrayTemperature\bin\Release\fr\*"; DestDir: "{app}\fr"; Flags: ignoreversion
Source: "..\StarTrayTemperature\bin\Release\it\*"; DestDir: "{app}\it"; Flags: ignoreversion
Source: "..\StarTrayTemperature\bin\Release\ja\*"; DestDir: "{app}\ja"; Flags: ignoreversion
Source: "..\StarTrayTemperature\bin\Release\pl\*"; DestDir: "{app}\pl"; Flags: ignoreversion
Source: "..\StarTrayTemperature\bin\Release\ru\*"; DestDir: "{app}\ru"; Flags: ignoreversion
Source: "..\StarTrayTemperature\bin\Release\sv\*"; DestDir: "{app}\sv"; Flags: ignoreversion
Source: "..\StarTrayTemperature\bin\Release\tr\*"; DestDir: "{app}\tr"; Flags: ignoreversion
Source: "..\StarTrayTemperature\bin\Release\zh-CN\*"; DestDir: "{app}\zh-CN"; Flags: ignoreversion
Source: "..\StarTrayTemperature\bin\Release\zh-Hant\*"; DestDir: "{app}\zh-Hant"; Flags: ignoreversion

[Icons]
Name: "{group}\StarTray Temperature"; Filename: "{app}\StarTray.exe"
Name: "{group}\{cm:UninstallProgram,StarTray Temperature}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\StarTray Temperature"; Filename: "{app}\StarTray.exe"; Tasks: desktopicon

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "StarTray"; ValueData: """{app}\StarTray.exe"""; Tasks: startupicon; Flags: uninsdeletevalue

[Run]
Filename: "{app}\StarTray.exe"; Description: "{cm:LaunchProgram,StarTray Temperature}"; Flags: nowait postinstall skipifsilent shellexec
