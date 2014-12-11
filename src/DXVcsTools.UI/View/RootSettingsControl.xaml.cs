using System.Windows.Controls;
using DXVcsTools.UI.ViewModel;

namespace DXVcsTools.UI.View {
    /// <summary>
    /// Interaction logic for RootSettingsControl.xaml
    /// </summary>
    public partial class RootSettingsControl : UserControl {
        public RootSettingsControl() {
            AssemblyLoadingGuard.Protect();
            InitializeComponent();
        }
    }
}
