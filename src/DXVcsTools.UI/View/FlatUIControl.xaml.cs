using System.Windows.Controls;
using DXVcsTools.UI.ViewModel;

namespace DXVcsTools.UI.View {
    /// <summary>
    ///     Interaction logic for FlatUIControl.xaml
    /// </summary>
    public partial class FlatUIControl : UserControl {
        public FlatUIControl() {
            AssemblyLoadingGuard.Protect();
            InitializeComponent();
        }
    }
}