; This setup script is based on a sample: http://www.mztools.com/articles/2008/MZ2008010.aspx

[Setup]
; Ensure that you use YOUR OWN APP_ID; DO NOT REUSE THIS ONE
#define APP_ID "{{2052B0AB-073D-41F7-A5EE-8DE1F883DEFAULT_GROUP_NAMECF35}"

#define APP_NAME "DXVcsTools"
#define APP_VERSION "1.2.1.0"
#define PUBLISHER_NAME ".NET Reports Team"
#define OUTPUT_FOLDER_NAME "bin"
#define OUTPUT_BASE_FILE_NAME "DXVcsToolsSetup"
#define COPYRIGHT_YEAR = "2011"

; Ensure that these values are used for the Connect class
#define CONNECT_CLASS_FULL_NAME = "DXVcsTools.VSAddIn.Connect"
#define ADDIN_FILE_NAME "DXVcsTools.VSAddIn.dll"
#define ADDIN_XML_FILE_NAME "DXVcsTools.AddIn"


OutputDir={#OUTPUT_FOLDER_NAME}
OutputBaseFilename={#OUTPUT_BASE_FILE_NAME}
AppID={#APP_ID}
VersionInfoVersion={#APP_VERSION}
;PrivilegesRequired=lowest
AppName={#APP_NAME}
AppVerName={#APP_NAME} {#APP_VERSION}
DefaultGroupName={#APP_NAME}
AppPublisher={#PUBLISHER_NAME}
DefaultDirName={pf}\{#APP_NAME}
Compression=lzma
SolidCompression=true
DisableReadyPage=true
UninstallLogMode=append
DisableProgramGroupPage=false
VersionInfoCompany={#PUBLISHER_NAME}
AppCopyright=Copyright © {#COPYRIGHT_YEAR} {#PUBLISHER_NAME}
AlwaysUsePersonalGroup=false
InternalCompressLevel=ultra
AllowNoIcons=true

[Languages]
Name: English; MessagesFile: compiler:Default.isl

[Types]
Name: Custom; Description: Custom; Flags: iscustom

[Files]
Source: ..\src\bin\*; Excludes: *.pdb, *.vshost.exe*; DestDir: {app}; Flags: ignoreversion;
Source: ..\src\bin\en-US\*; Excludes: *.pdb, *.vshost.exe*; DestDir: {app}\en-US; Flags: ignoreversion;
Source: svnrepo\*; DestDir: {app}\svnrepo; Flags: recursesubdirs createallsubdirs;
Source: TortoiseSVN\*; DestDir: {app}\TortoiseSVN; Flags: ignoreversion recursesubdirs createallsubdirs;
Source: TortoiseSVN\bin\SciLexer.dll; DestDir: {app}; Flags: ignoreversion;
Source: TortoiseSVN\bin\TortoiseBlame.exe; DestDir: {app}; Flags: ignoreversion;
Source: stub.txt; DestDir: {tmp}; Flags: ignoreversion; AfterInstall: CreateAddInXMLFileVS2005(); Check: IsVS2005Selected()
Source: stub.txt; DestDir: {tmp}; Flags: ignoreversion; AfterInstall: CreateAddInXMLFileVS2008(); Check: IsVS2008Selected()
Source: stub.txt; DestDir: {tmp}; Flags: ignoreversion; AfterInstall: CreateAddInXMLFileVS2010(); Check: IsVS2010Selected()

[Icons]
Name: {group}\{cm:UninstallProgram,{#APP_NAME}}; Filename: {uninstallexe}

[Messages]
English.FinishedLabel=Setup has finished installing [name] on your computer.
English.UninstallStatusLabel=The uninstallation can take up to a minute while the commands of the add-in are removed from the Visual Studio and Macros IDEs.

[CustomMessages]
English.VSNeedsToBeClosed=Close Visual Studio before continuing.
English.RegisteringAddIn=Registering the add-in...
English.VSRequired=Visual Studio Standard Edition or higher is required to install this product (Express editions of Visual Studio don't support add-ins).

[UninstallDelete]
Type: filesandordirs; Name: {app}\TempCopy
Type: filesandordirs; Name: {app}\svnrepo

[Code]
var
   IDEsPage: TWizardPage;
   IDEsCheckListBox: TNewCheckListBox;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(***************************************************************************************************)
function GetXMLAddInFullFolderName(Param: String): String;
var
   sCommonAppDataFolder: String;
   sResult: String;
begin
   sCommonAppDataFolder := ExpandConstant('{commonappdata}');

   (* Param is the version such as '8.0' or '9.0' *)
   sResult := sCommonAppDataFolder + '\Microsoft\VisualStudio\' + Param + '\Addins\'

   Result := sResult;
end;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(***************************************************************************************************)
procedure CreateAddInXMLFile(sVersion: String; sConnectClassFullName: String; sDLLFileName: String);
var
   sLines: TArrayOfString;
   sXMLAddInFullFileName: String;
   sAddInDLLFullFileName: String;
   sInstallationFolder: String;
   sFolder: String;
begin
   (* Compose the full name of the add-in DLL *)
   sInstallationFolder := ExpandConstant('{app}');
   sAddInDLLFullFileName := sInstallationFolder + '\' + sDLLFileName;

   (* Get the folder where to put the .AddIn XML registration file *)
   sFolder := GetXMLAddInFullFolderName(sVersion);

   (* Ensure that the folder is created *)
   CreateDir(sFolder);

   (* Compose the full name of the .AddIn XML registration file *)
   sXMLAddInFullFileName := sFolder + '{#ADDIN_XML_FILE_NAME}';

   (* Create the .AddIn XML registration file *)
   SetArrayLength(sLines,16);

   sLines[0]  := '<?xml version="1.0" encoding="windows-1252" standalone="no"?>';
   sLines[1]  := '<Extensibility xmlns="http://schemas.microsoft.com/AutomationExtensibility">';
   sLines[2]  := '   <HostApplication>';
   sLines[3]  := '      <Name>Microsoft Visual Studio</Name>';
   sLines[4]  := '      <Version>' + sVersion + '</Version>';
   sLines[5]  := '   </HostApplication>';
   sLines[6]  := '   <Addin>';
   sLines[7]  := '      <FriendlyName>{#APP_NAME}</FriendlyName>';
   sLines[8]  := '      <Description>{#APP_NAME}</Description>';
   sLines[9]  := '      <Assembly>' + sAddInDLLFullFileName + '</Assembly>';
   sLines[10] := '      <FullClassName>' + sConnectClassFullName + '</FullClassName>';
   sLines[11] := '      <LoadBehavior>0</LoadBehavior>';
   sLines[12] := '      <CommandPreload>1</CommandPreload>';
   sLines[13] := '      <CommandLineSafe>0</CommandLineSafe>';
   sLines[14] := '   </Addin>';
   sLines[15] := '</Extensibility>';

   SaveStringsToFile(sXMLAddInFullFileName, sLines, False);

end;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(***************************************************************************************************)
procedure CreateAddInXMLFileVS2005();
begin
   CreateAddInXMLFile('8.0','{#CONNECT_CLASS_FULL_NAME}', '{#ADDIN_FILE_NAME}')
end ;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(***************************************************************************************************)
procedure CreateAddInXMLFileVS2008();
begin
   CreateAddInXMLFile('9.0','{#CONNECT_CLASS_FULL_NAME}', '{#ADDIN_FILE_NAME}')
end ;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(***************************************************************************************************)
procedure CreateAddInXMLFileVS2010();
begin
   CreateAddInXMLFile('10.0','{#CONNECT_CLASS_FULL_NAME}', '{#ADDIN_FILE_NAME}')
end ;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(***************************************************************************************************)
procedure ShowCloseVisualStudioMessage();
var
   sMsg: String;
begin
   sMsg := CustomMessage('VSNeedsToBeClosed');
   MsgBox(sMsg, mbCriticalError, mb_Ok)
end;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(***************************************************************************************************)
procedure ShowVisualStudioRequiredMessage();
var
   sMsg: String;
begin
   sMsg := CustomMessage('VSRequired');
   MsgBox(sMsg, mbCriticalError, mb_Ok)
end;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(***************************************************************************************************)
function IsVSInstalledByProgID(ProgID: String):Boolean;
begin

   if RegKeyExists(HKCR, ProgID) then
      Result := True
   else
      Result := False

end;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(***************************************************************************************************)
function IsVSRunningByProgID(ProgID: String):Boolean;
var
   IDE: Variant;
begin

   try
      IDE := GetActiveOleObject(ProgID);
   except
   end;

   if VarIsEmpty(IDE) then
      Result := False
   else
      Result := True
end;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(***************************************************************************************************)
function IsVS2005Installed():Boolean;
begin
   if IsVSInstalledByProgID('VisualStudio.DTE.8.0') then
      Result := True
   else
      Result := False
end;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(***************************************************************************************************)
function IsVS2008Installed():Boolean;
begin
   if IsVSInstalledByProgID('VisualStudio.DTE.9.0') then
      Result := True
   else
      Result := False
end;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(***************************************************************************************************)
function IsVS2010Installed():Boolean;
begin
   if IsVSInstalledByProgID('VisualStudio.DTE.10.0') then
      Result := True
   else
      Result := False
end;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(***************************************************************************************************)
function IsVSInstalled():Boolean;
begin

   if IsVS2005Installed() then
      Result := True
   else if IsVS2008Installed() then
      Result := True
   else if IsVS2010Installed() then
      Result := True
   else
      begin
         Result := False
         ShowVisualStudioRequiredMessage();
      end
end;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(* NOTE: DOES NOT work from LUA (always returns false)                                             *)
(***************************************************************************************************)
function IsSomeVSRunning():Boolean;
begin

   if IsVSRunningByProgID('VisualStudio.DTE.8.0') then
      Result := True
   else if IsVSRunningByProgID('VisualStudio.DTE.9.0') then
      Result := True
   else if IsVSRunningByProgID('VisualStudio.DTE.10.0') then
      Result := True
   else
      Result := False
end;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(***************************************************************************************************)
procedure UnregisterAddInIfInstalled(sVersion: String; sConnectClassFullName: String);
var
   sXMLAddInFullFileName: String;
   sFolder: String;
   sIDEFullFileName: String;
   iResultCode: Integer;
begin

   (* Compose the full name of the .AddIn XML registration file *)
   sFolder := GetXMLAddInFullFolderName(sVersion);
   sXMLAddInFullFileName := sFolder + '{#ADDIN_XML_FILE_NAME}';

   if FileExists(sXMLAddInFullFileName) then
      begin
         (* Delete the .AddIn XML registration file to unregister the add-in *)
         DeleteFile(sXMLAddInFullFileName);

         (* Compose the full name of the Visual Studio IDE *)
         sIDEFullFileName := ExpandConstant('{reg:HKLM\SOFTWARE\Microsoft\VisualStudio\' + sVersion + ',InstallDir}') + 'devenv.exe'

         (* Remove the commands of the IDE *)
         Exec(sIDEFullFileName, '/ResetAddin ' + sConnectClassFullName + ' /command File.Exit', '', SW_HIDE, ewWaitUntilTerminated, iResultCode);

      end

end;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(***************************************************************************************************)
function IsVS2005Selected():Boolean;
begin
   Result := IDEsCheckListBox.Checked[0]
end;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(***************************************************************************************************)
function IsVS2008Selected():Boolean;
begin
   Result := IDEsCheckListBox.Checked[1]
end;

(***************************************************************************************************)
(* Auxiliar function                                                                               *)
(***************************************************************************************************)
function IsVS2010Selected():Boolean;
begin
   Result := IDEsCheckListBox.Checked[2]
end;

(***************************************************************************************************)
(* InnoSetup event function                                                                        *)
(***************************************************************************************************)
function InitializeSetup(): Boolean;
begin

   if not IsVSInstalled() then
      begin
         Result := False;
      end
   else
      begin
         if IsSomeVSRunning() then
            begin
               ShowCloseVisualStudioMessage()
               Result := False;
            end
         else
            Result := True;
      end

end;

(***************************************************************************************************)
(* InnoSetup event function                                                                        *)
(***************************************************************************************************)
procedure InitializeWizard();
var
   IDEsLabel: TLabel;
   bIsVS2005Installed: Boolean;
   bIsVS2008Installed: Boolean;
   bIsVS2010Installed: Boolean;

begin
   IDEsPage := CreateCustomPage(wpSelectComponents, SetupMessage(msgWizardSelectComponents), SetupMessage(msgSelectComponentsDesc));

   IDEsLabel := TLabel.Create(IDEsPage);
   IDEsLabel.Caption := SetupMessage(msgSelectComponentsLabel2);
   IDEsLabel.Width := IDEsPage.SurfaceWidth;
   IDEsLabel.Height := ScaleY(40);
   IDEsLabel.AutoSize := False;
   IDEsLabel.WordWrap := True;
   IDEsLabel.Parent := IDEsPage.Surface;

   IDEsCheckListBox := TNewCheckListBox.Create(IDEsPage);
   IDEsCheckListBox.Top := IDEsLabel.Top + IDEsLabel.Height + ScaleY(8);
   IDEsCheckListBox.Width := IDEsPage.SurfaceWidth;
   IDEsCheckListBox.Height := ScaleX(100);
   IDEsCheckListBox.Flat := True;
   IDEsCheckListBox.Parent := IDEsPage.Surface;

   bIsVS2005Installed := IsVS2005Installed();
   bIsVS2008Installed := IsVS2008Installed();
   bIsVS2010Installed := IsVS2010Installed();

   IDEsCheckListBox.AddCheckBox('{#APP_NAME} -  Visual Studio 2005', '', 0, bIsVS2005Installed, bIsVS2005Installed, False, True, nil)
   IDEsCheckListBox.AddCheckBox('{#APP_NAME} -  Visual Studio 2008', '', 0, bIsVS2008Installed, bIsVS2008Installed, False, True, nil)
   IDEsCheckListBox.AddCheckBox('{#APP_NAME} -  Visual Studio 2010', '', 0, bIsVS2010Installed, bIsVS2010Installed, False, True, nil)

end;

(***************************************************************************************************)
(* InnoSetup event function                                                                        *)
(***************************************************************************************************)
function InitializeUninstall(): Boolean;
begin

   if IsSomeVSRunning() then
      begin
         ShowCloseVisualStudioMessage()
         Result := False;
      end
   else
       begin
         Result := True;
      end
end;

(***************************************************************************************************)
(* InnoSetup event function                                                                        *)
(***************************************************************************************************)
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
   if CurUninstallStep = usUninstall then
      begin
         UnregisterAddInIfInstalled('8.0', '{#CONNECT_CLASS_FULL_NAME}' );
         UnregisterAddInIfInstalled('9.0', '{#CONNECT_CLASS_FULL_NAME}' );
         UnregisterAddInIfInstalled('10.0', '{#CONNECT_CLASS_FULL_NAME}' );
      end;
end;








