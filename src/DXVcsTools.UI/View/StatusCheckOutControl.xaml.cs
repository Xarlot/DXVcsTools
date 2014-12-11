using System.Windows.Controls;
using DXVcsTools.UI.ViewModel;

namespace DXVcsTools.UI.View {
    /// <summary>
    /// Interaction logic for StatusCheckOutControl.xaml
    /// </summary>
    public partial class StatusCheckOutControl : UserControl {
        public StatusCheckOutControl() {
            AssemblyLoadingGuard.Protect();
            InitializeComponent();
        }
    }
}
