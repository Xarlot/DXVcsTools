using System.Windows.Controls;
using DXVcsTools.UI.ViewModel;

namespace DXVcsTools.UI.View {
    /// <summary>
    ///     Interaction logic for ToolWindowControl.xaml
    /// </summary>
    public partial class ToolWindowControl : UserControl {
        public ToolWindowControl() {
            AssemblyLoadingGuard.Protect();
            InitializeComponent();
        }
    }
}