using System.Runtime.InteropServices;
using System.Windows;
using DXVcsTools.UI.View;
using DXVcsTools.UI.ViewModel;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace DXVcsTools {
    [Guid("ACBBE549-099F-4F74-9849-CEEBCE8D07E1")]
    public sealed class InternalBlameWindow : ToolWindowPane {
        InternalBlameControl Control { get { return Content as InternalBlameControl; } }
        public InternalBlameWindow() : base(null) {
            Caption = "Blame window";
            Content = new InternalBlameControl();
            Control.Loaded += Control_Loaded;
        }

        void Control_Loaded(object sender, RoutedEventArgs e) {
        }
        public void Initialize(InternalBlameViewModel model) {
            Control.DataContext = model;
            Show();
        }
        public void Show() {
            var frame = (IVsWindowFrame)Frame;
            frame.Show();
        }
        protected override void Initialize() {
            base.Initialize();
        }
    }
}
