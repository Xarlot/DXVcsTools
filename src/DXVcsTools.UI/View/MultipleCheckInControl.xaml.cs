using System.Windows.Controls;
using DXVcsTools.UI.ViewModel;

namespace DXVcsTools.UI.View {
    /// <summary>
    ///     Interaction logic for MultipleCheckInControl.xaml
    /// </summary>
    public partial class MultipleCheckInControl : UserControl {
        public MultipleCheckInControl() {
            AssemblyLoadingGuard.Protect();
            InitializeComponent();
        }
    }
}