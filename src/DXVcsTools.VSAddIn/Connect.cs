using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;
using DXVcsTools.Core;
using DXVcsTools.UI;
using DXVcsTools.UI.WinForms;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Microsoft.VisualStudio.CommandBars;
using Configuration = System.Configuration.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using Process = System.Diagnostics.Process;

namespace DXVcsTools.VSAddIn {
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget {
        const string BlameCommandName = "DXVcsToolsBlame";
        const string PortCommandName = "DXVcsToolsPort";

        AddIn _addInInstance;
        DTE2 _applicationObject;
        Configuration _configuration;
        /// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
        /// <param term='commandName'>The name of the command to determine state for.</param>
        /// <param term='neededText'>Text that is needed for the command.</param>
        /// <param term='status'>The state of the command in the user interface.</param>
        /// <param term='commandText'>Text requested by the neededText parameter.</param>
        /// <seealso class='Exec' />
        public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText) {
            if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone) {
                if (commandName == GetClassQualifiedCommandName(BlameCommandName) || commandName == GetClassQualifiedCommandName(PortCommandName)) {
                    status = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                    return;
                }
            }
        }

        /// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
        /// <param term='commandName'>The name of the command to execute.</param>
        /// <param term='executeOption'>Describes how the command should be run.</param>
        /// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
        /// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
        /// <param term='handled'>Informs the caller if the command was handled or not.</param>
        /// <seealso class='Exec' />
        public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled) {
            handled = false;
            if (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault) {
                try {
                    if (commandName == GetClassQualifiedCommandName(BlameCommandName)) {
                        ExecBlameCommand();
                        handled = true;
                        return;
                    }

                    if (commandName == GetClassQualifiedCommandName(PortCommandName)) {
                        ExecPortCommand();
                        handled = true;
                        return;
                    }
                }
                catch (Exception exception) {
                    MessageBox.Show(exception.Message, null, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom) {
            _applicationObject = (DTE2)application;
            _addInInstance = (AddIn)addInInst;

            _configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

            if (connectMode == ext_ConnectMode.ext_cm_UISetup) {
                var contextGUIDS = new object[] {};
                var commands = (Commands2)_applicationObject.Commands;
                string toolsMenuName;

                try {
                    //If you would like to move the command to a different menu, change the word "Tools" to the 
                    //  English version of the menu. This code will take the culture, append on the name of the menu
                    //  then add the command to that menu. You can find a list of all the top-level menus in the file
                    //  CommandBar.resx.
                    string resourceName;
                    var resourceManager = new ResourceManager("DXVcsTools.VSAddIn.CommandBar", Assembly.GetExecutingAssembly());
                    var cultureInfo = new CultureInfo(_applicationObject.LocaleID);

                    if (cultureInfo.TwoLetterISOLanguageName == "zh") {
                        CultureInfo parentCultureInfo = cultureInfo.Parent;
                        resourceName = String.Concat(parentCultureInfo.Name, "Tools");
                    }
                    else {
                        resourceName = String.Concat(cultureInfo.TwoLetterISOLanguageName, "Tools");
                    }
                    toolsMenuName = resourceManager.GetString(resourceName);
                }
                catch {
                    //We tried to find a localized version of the word Tools, but one was not found.
                    //  Default to the en-US word, which may work for the current culture.
                    toolsMenuName = "Tools";
                }

                //Place the command on the tools menu.
                //Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
                CommandBar menuBarCommandBar = ((CommandBars)_applicationObject.CommandBars)["MenuBar"];

                //Find the Tools command bar on the MenuBar command bar:
                CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
                var toolsPopup = (CommandBarPopup)toolsControl;

                AddCommand(BlameCommandName, "DX Blame", "Executes the command for DX Blame", toolsPopup, 1);
                AddCommand(PortCommandName, "DX Port", "Executes the command for DX Port", toolsPopup, 2);
            }
        }

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom) {
        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnAddInsUpdate(ref Array custom) {
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom) {
        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom) {
        }
        void AddCommand(string name, string buttonText, string tooltip, CommandBarPopup toolsPopup, int imageId) {
            var contextGUIDS = new object[] {};
            var commands = (Commands2)_applicationObject.Commands;

            try {
                //Add a command to the Commands collection:
                Command command = commands.AddNamedCommand2(_addInInstance, name, buttonText, tooltip, false, imageId, // pass as parameter
                    ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText,
                    vsCommandControlType.vsCommandControlTypeButton);

                //Add a control for the command to the tools menu:
                if ((command != null) && (toolsPopup != null)) {
                    command.AddControl(toolsPopup.CommandBar, 1);
                }
            }
            catch (ArgumentException) {
                //If we are here, then the exception is probably because a command with that name
                //  already exists. If so there is no need to recreate the command and we can 
                //  safely ignore the exception.
            }
        }

        void ExecBlameCommand() {
            const string projectKindWeb = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";
            string fileName = null;
            if (!CanHandleActiveDocument(ref fileName))
                return;

            var selection = (TextSelection)_applicationObject.ActiveDocument.Selection;
            int lineNumber = selection.TextRanges.Item(1).StartPoint.Line;

            Project containingProject = _applicationObject.ActiveDocument.ProjectItem.ContainingProject;
            string projectFile = containingProject.Kind.ToUpper() != projectKindWeb ? containingProject.FullName : _applicationObject.Solution.FullName;

            var dxPortConfiguration = ConfigurationHelper.GetSection<DXPortConfiguration>(_configuration, "dxPortConfiguration");
            if (dxPortConfiguration.BlameType == DXBlameType.External)
                ExecExternalBlame(fileName, projectFile, lineNumber);
            else if (dxPortConfiguration.BlameType == DXBlameType.Mixture)
                ExecMixtureBlame(fileName, projectFile, lineNumber);
            else
                ExecNativeBlame(fileName, projectFile, lineNumber);
        }

        void ExecMixtureBlame(string fileName, string projectFile, int lineNumber) {
            string blame = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "TortoiseProc.exe");
            string args = string.Format("/command:vsblame /path:\"{0}|{1}\" /line:{2} ", fileName, projectFile, lineNumber);

            Process.Start(blame, args);
        }

        void ExecExternalBlame(string fileName, string projectFile, int lineNumber) {
            string blame = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "dxblame.exe");
            string args = string.Format("vsblame \"{0}\" \"{1}\" {2}", fileName, projectFile, lineNumber);

            Process.Start(blame, args);
        }

        void ExecNativeBlame(string fileName, string projectFile, int lineNumber) {
            var model = new BlameWindowModel(fileName, projectFile, lineNumber, false);
            IBlameWindowView ui = new ViewFactory().CreateBlameWindow();

            var presenter = new BlameWindowPresenter(ui, model);
            presenter.Show();
        }

        void ExecPortCommand() // TODO: introduce CommandHandler class hierarchy instead of multiple "handle..." methods
        {
            string fileName = null;
            if (!CanHandleActiveDocument(ref fileName))
                return;

            var dxPortConfiguration = ConfigurationHelper.GetSection<DXPortConfiguration>(_configuration, "dxPortConfiguration");
            IViewFactory factory = CreateViewFactory(dxPortConfiguration.UIType);

            var model = new PortWindowModel(fileName, _applicationObject.ActiveDocument.ProjectItem.ContainingProject.FullName, dxPortConfiguration);

            IPortWindowView ui = factory.CreatePortWindow();

            var presenter = new PortWindowPresenter(ui, model);
            presenter.Initialize();

            ui.ShowModal();
        }

        IViewFactory CreateViewFactory(DXPortUIType uiType) {
            if (uiType == DXPortUIType.WinForms)
                return new ViewFactory();

            if (uiType == DXPortUIType.Wpf)
                return new UI.Wpf.ViewFactory();

            throw new ArgumentException("Unexpected value: " + uiType, "uiType");
        }

        string GetClassQualifiedCommandName(string name) {
            return string.Format("{0}.{1}", GetType().FullName, name);
        }

        bool CanHandleActiveDocument(ref string fileName) {
            if (_applicationObject.ActiveDocument == null) {
                MessageBox.Show("No current document.", _addInInstance.Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            fileName = _applicationObject.ActiveDocument.FullName;
            var sourceControl = (SourceControl2)_applicationObject.SourceControl;

            if (!sourceControl.IsItemUnderSCC(fileName)) {
                MessageBox.Show(string.Concat("File ", fileName, " is not under source control."), _addInInstance.Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }
    }
}