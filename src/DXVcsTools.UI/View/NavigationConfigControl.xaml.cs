using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using DXVcsTools.UI.Navigator;
using DXVcsTools.UI.ViewModel;

namespace DXVcsTools.UI.View {
    /// <summary>
    /// Interaction logic for NavigationConfigUserControl.xaml
    /// </summary>
    public partial class NavigationConfigUserControl : UserControl {
        public NavigationConfigUserControl() {
            AssemblyLoadingGuard.Protect();
            InitializeComponent();
        }
        void TreeListView_OnCellValueChanging(object sender, TreeListCellValueChangedEventArgs e) {
            if (e.Column.FieldName != "UseForAddReference")
                return;
            TreeListView list = (TreeListView)sender;
            e.Node.ExpandAll();
            bool isChecked = (bool)e.Value;
            TreeListNodeIterator iterator = new TreeListNodeIterator(e.Node);
            foreach (var node in iterator) {
                NavigateTreeItem item = (NavigateTreeItem)node.Content;
                item.UseForAddReference = isChecked;
            }
        }
    }
}
