using System.Windows.Controls;
using DXVcsTools.UI.ViewModel;

namespace DXVcsTools.UI.View {
    /// <summary>
    ///     Interaction logic for ManualMergeControl.xaml
    /// </summary>
    public partial class ManualMergeControl : UserControl {
        public ManualMergeControl() {
            AssemblyLoadingGuard.Protect();
            InitializeComponent();
        }
    }
}