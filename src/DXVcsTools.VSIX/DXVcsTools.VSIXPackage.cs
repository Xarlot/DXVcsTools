using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using DXVcsTools.Core;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace DXVcsTools.VSIX {
    /// <summary>
    ///     This is the class that implements the package exposed by this assembly.
    ///     The minimum requirement for a class to be considered a valid package for Visual Studio
    ///     is to implement the IVsPackage interface and register itself with the shell.
    ///     This package uses the helper classes defined inside the Managed Package Framework (MPF)
    ///     to do it: it derives from the Package class that provides the implementation of the
    ///     IVsPackage interface and uses the registration attributes defined in the framework to
    ///     register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // This attribute registers a tool window exposed by this package.
    [ProvideToolWindow(typeof(MyToolWindow))]
    [ProvideBindingPath(SubPath = "Lib")]
    [Guid(GuidList.guidDXVcsTools_VSIXPkgString)]
    public sealed class DXVcsTools_VSIXPackage : Package, IVsSolutionEvents {
        MenuViewModel Menu { get; set; }
        ToolWindowViewModel ToolWindowViewModel { get; set; }
        OptionsViewModel Options { get; set; }
        uint solutionEventsCookie;
        readonly string currentDXVersion = "13.2.1.0";

        public DXVcsTools_VSIXPackage() {
            DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;

            Options = new OptionsViewModel();

            Menu = new MenuViewModel();
            Menu.DoConnect(dte);

            ToolWindowViewModel = new ToolWindowViewModel(dte, Options);
            
        }

        /// <summary>
        ///     This function is called when the user clicks the menu item that shows the
        ///     tool window. See the Initialize method to see how the menu item is associated to
        ///     this function using the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        void ShowToolWindow() {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            //try {
            //    AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                var window = GetMyToolWindow();
                window.Initialize(ToolWindowViewModel);
            //}
            //catch {
            //    AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            //}
        }
        MyToolWindow GetMyToolWindow() {
            MyToolWindow window = (MyToolWindow)FindToolWindow(typeof(MyToolWindow), 0, true);
            if ((null == window) || (null == window.Frame)) {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            var windowFrame = (IVsWindowFrame)window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
            return window;
        }


        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation

        /// <summary>
        ///     This function is the callback used to execute a command when the a menu item is clicked.
        ///     See the Initialize method to see how the menu item is associated to this function using
        ///     the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        void MenuItemCallback(object sender, EventArgs e) {
            ShowToolWindow();
        }

        #region Package Members
        /// <summary>
        ///     Initialization of the package; this method is called right after the package is sited, so this is the place
        ///     where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize() {
            base.Initialize();

            VSDevExpressMenu devExpressMenu = new VSDevExpressMenu(GetService(typeof(DTE)) as DTE);
            VSDevExpressMenuItem wizardMenu = devExpressMenu.CreateOrGetItem("Show tool window");
            wizardMenu.Click += wizardMenu_Click;

            var solution = ServiceProvider.GlobalProvider.GetService(typeof(SVsSolution)) as IVsSolution2;
            if (solution != null) {
                solution.AdviseSolutionEvents(this, out solutionEventsCookie);
            }
        }

        void wizardMenu_Click(object sender, EventArgs e) {
            ShowToolWindow();
        }
        #endregion

        int IVsSolutionEvents.OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded) {
            return VSConstants.S_OK;
        }
        int IVsSolutionEvents.OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel) {
            return VSConstants.S_OK;
        }
        int IVsSolutionEvents.OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved) {
            return VSConstants.S_OK;
        }
        int IVsSolutionEvents.OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy) {
            return VSConstants.S_OK;
        }
        int IVsSolutionEvents.OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel) {
            return VSConstants.S_OK;
        }
        int IVsSolutionEvents.OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy) {
            return VSConstants.S_OK;
        }
        int IVsSolutionEvents.OnAfterOpenSolution(object pUnkReserved, int fNewSolution) {
            var window = GetMyToolWindow();
            window.Initialize(ToolWindowViewModel);
            return VSConstants.S_OK;
        }
        int IVsSolutionEvents.OnQueryCloseSolution(object pUnkReserved, ref int pfCancel) {
            return VSConstants.S_OK;
        }
        int IVsSolutionEvents.OnBeforeCloseSolution(object pUnkReserved) {
            ToolWindowViewModel.Update();
            return VSConstants.S_OK;
        }
        int IVsSolutionEvents.OnAfterCloseSolution(object pUnkReserved) {
            return VSConstants.S_OK;
        }
    }
}
