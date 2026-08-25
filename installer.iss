; Inno Setup script for DOB NOW Trident Host
; Produces the STANDARD installer:  <root>\Executable\02 - dob-trident.exe
; Packages the portable exe (built into <root>\Portable) and installs per-user (no admin).

#define AppName "DOB NOW Trident Host"
#define AppVer "1.0.0"
#define AppExe "DOBNowTrident.exe"
#define PortableExe "C:\Users\Sunny\Desktop\claude code\Portable\02 - dob-trident Portable.exe"

[Setup]
AppId={{A1B2C3D4-E5F6-47A8-9B0C-1D2E3F4A5B6C}}
AppName={#AppName}
AppVersion={#AppVer}
AppPublisher=Sunny
DefaultDirName={autopf}\{#AppName}
DisableProgramGroupPage=yes
UninstallDisplayIcon={app}\{#AppExe}
OutputDir=C:\Users\Sunny\Desktop\claude code\Executable
OutputBaseFilename=02 - dob-trident
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=lowest
WizardStyle=modern

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional icons:"

[Files]
Source: "{#PortableExe}"; DestDir: "{app}"; DestName: "{#AppExe}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExe}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExe}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExe}"; Description: "Launch {#AppName}"; Flags: nowait postinstall skipifsilent
