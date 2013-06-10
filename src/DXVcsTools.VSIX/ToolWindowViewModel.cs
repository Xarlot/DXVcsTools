using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXVcsTools.Core;
using DXVcsTools.UI;
using DevExpress.Xpf.Mvvm;
using DevExpress.Xpf.Mvvm.Native;
using EnvDTE;

namespace DXVcsTools.VSIX {
    public class ToolWindowViewModel : BindableBase, IUpdatableViewModel {
        readonly DTE dte;
        SolutionItem solutionItem;
        FileItemBase selectedItem;

        public SolutionItem Solution {
            get { return solutionItem; }
            set { SetProperty(ref solutionItem, value, "Solution"); }
        }
        public FileItemBase SelectedItem {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value, "SelectedItem"); }
        }
        public DelegateCommand MergeCommand { get; private set; }
        public DelegateCommand MergeAllCommand { get; private set; }
        public void Update() {
            DteWrapper dteWrapper = new DteWrapper(dte);
            Solution = dteWrapper.BuildTree();
            SelectedItem = null;
        }

        public ToolWindowViewModel(DTE dte) {
            this.dte = dte;
            MergeCommand = new DelegateCommand(Merge, CanMerge);
            MergeAllCommand = new DelegateCommand(MergeAll, CanMergeAll);
        }
        void Merge() {
        }
        bool CanMerge() {
            return SelectedItem.Return(x => x.IsChecked, () => false);
        }
        void MergeAll() {
        }
        bool CanMergeAll() {
            return true;
        }
    }
}
