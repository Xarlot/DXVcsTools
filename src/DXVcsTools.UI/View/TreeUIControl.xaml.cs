using System.Windows.Controls;
using DXVcsTools.UI.ViewModel;

namespace DXVcsTools.UI.View {
    /// <summary>
    ///     Interaction logic for TreeUIControl.xaml
    /// </summary>
    public partial class TreeUIControl : UserControl {
        public TreeUIControl() {
            AssemblyLoadingGuard.Protect();
            InitializeComponent();
        }
    }
}