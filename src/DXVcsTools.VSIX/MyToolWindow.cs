using System.Runtime.InteropServices;
using System.Windows.Controls;
using DXVcsTools.Version;
using DevExpress.Xpf.Mvvm.Native;
using Microsoft.VisualStudio.Shell;

namespace DXVcsTools.VSIX {
    /// <summary>
    ///     This class implements the tool window exposed by this package and hosts a user control.
    ///     In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    ///     usually implemented by the package implementer.
    ///     This class derives from the ToolWindowPane class provided from the MPF in order to use its
    ///     implementation of the IVsUIElementPane interface.
    /// </summary>
    [Guid("c170e42d-6d77-44b1-a643-29d22df9f286")]
    public sealed class MyToolWindow : ToolWindowPane {
        ToolWindowViewModel model;
        /// <summary>
        ///     Standard constructor for the tool window.
        /// </summary>
        public MyToolWindow()
            : base(null) {
            // Set the window title reading it from the resources.
            Caption = Resources.ToolWindowTitle + " - " + VersionInfo.FullVersion;
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            BitmapResourceID = 301;
            BitmapIndex = 1;

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
            // the object returned by the Content property.
            var mycontrol = new MyControl();
            mycontrol.Loaded += MycontrolLoaded;
            Content = mycontrol;
        }

        void MycontrolLoaded(object sender, System.Windows.RoutedEventArgs e) {
            if (model.If(x => x.Options.UpdateOnShowing).ReturnSuccess())
                model.Do(x => x.Update());
        }
        MyControl Control {
            get { return base.Content as MyControl; }
        }
        public void Initialize(ToolWindowViewModel viewModel) {
            model = viewModel;
            viewModel.Update();
            Control.DataContext = viewModel;
        }
    }
}