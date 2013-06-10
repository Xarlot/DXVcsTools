using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DXVcsTools.Core;
using DXVcsTools.UI;
using DevExpress.Xpf.Mvvm;
using DevExpress.Xpf.Mvvm.Native;
using EnvDTE;

namespace DXVcsTools.VSIX {
    public class ToolWindowViewModel : BindableBase, IUpdatableViewModel {
        readonly DTE dte;
        SolutionItem solutionItem;
        ProjectItemBase selectedItem;
        OptionsViewModel options;
        DXVcsBranch currentBranch;

        public DXVcsBranch CurrentBranch {
            get { return currentBranch; }
            set { SetProperty(ref currentBranch, value, "CurrentBrunch"); }
        }
        public IEnumerable<DXVcsBranch> AvailableBranches { get { return this.options.Branches; } }
        public SolutionItem Solution {
            get { return solutionItem; }
            set { SetProperty(ref solutionItem, value, "Solution"); }
        }
        public ProjectItemBase SelectedItem {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value, "SelectedItem", CommandManager.InvalidateRequerySuggested); }
        }
        public RelayCommand MergeCommand { get; private set; }
        public RelayCommand MergeAllCommand { get; private set; }
        public void Update() {
            DteWrapper dteWrapper = new DteWrapper(dte);
            Solution = dteWrapper.BuildTree();
            SelectedItem = null;
        }

        public ToolWindowViewModel(DTE dte, OptionsViewModel options) {
            this.dte = dte;
            this.options = options;
            MergeCommand = new RelayCommand(Merge, CanMerge);
            MergeAllCommand = new RelayCommand(MergeAll, CanMergeAll);
        }
        void Merge() {
            MergeHelper helper = new MergeHelper(options, new PortWindowModel(SelectedItem.Path, Solution.Path, options));
            helper.MergeChanges();
        }
        bool CanMerge() {
            return SelectedItem.Return(x => x.IsCheckOut && x.MergeState == MergeState.None, () => false);
        }
        void MergeAll() {
        }
        bool CanMergeAll() {
            return true;
        }
    }
}
