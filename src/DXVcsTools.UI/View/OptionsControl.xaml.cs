using System.Windows.Controls;
using DXVcsTools.UI.ViewModel;

namespace DXVcsTools.UI {
    /// <summary>
    ///     Interaction logic for OptionsControl.xaml
    /// </summary>
    public partial class OptionsControl : UserControl {
        public OptionsControl() {
            AssemblyLoadingGuard.Protect();
            InitializeComponent();
        }
    }
}