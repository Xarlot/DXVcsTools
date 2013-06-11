using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DXVcsTools.Core;
using DXVcsTools.DXVcsClient;
using DXVcsTools.UI;
using DevExpress.Xpf.Mvvm;
using DevExpress.Xpf.Mvvm.Native;
using EnvDTE;

namespace DXVcsTools.VSIX {
    public class ToolWindowViewModel : BindableBase, IUpdatableViewModel {
        readonly DTE dte;
        SolutionItem solutionItem;
        ProjectItemBase selectedItem;
        DXVcsBranch currentBranch;
        DXVcsBranch masterBranch;
        bool canTotalMerge;

        public bool CanTotalMerge {
            get { return canTotalMerge; }
            set { SetProperty(ref canTotalMerge, value, "CanTotalMerge", CommandManager.InvalidateRequerySuggested); }
        }
        public DXVcsBranch MasterBranch {
            get { return masterBranch; }
            private set { SetProperty(ref masterBranch, value, "MasterBranch", CommandManager.InvalidateRequerySuggested); }
        }
        public DXVcsBranch CurrentBranch {
            get { return currentBranch; }
            set { SetProperty(ref currentBranch, value, "CurrentBrunch"); }
        }
        public IEnumerable<DXVcsBranch> AvailableBranches { get { return Options.Branches; } }
        public SolutionItem Solution {
            get { return solutionItem; }
            set { SetProperty(ref solutionItem, value, "Solution"); }
        }
        public ProjectItemBase SelectedItem {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value, "SelectedItem", CommandManager.InvalidateRequerySuggested); }
        }
        PortViewModel Port { get; set; }
        public OptionsViewModel Options { get; private set; }

        public RelayCommand MergeCommand { get; private set; }
        public RelayCommand MergeAllCommand { get; private set; }
        public RelayCommand UpdateCommand { get; private set; }

        DXVcsBranch FindMasterBranch(PortOptionsViewModel portOptions) {
            string relativePath = portOptions.GetRelativePath(Solution.Path);
            return Options.Branches.FirstOrDefault(branch => relativePath.StartsWith(branch.Path));
        }
        public ToolWindowViewModel(DTE dte, OptionsViewModel options) {
            this.dte = dte;
            Options = options;
            MergeCommand = new RelayCommand(Merge, CanMerge);
            MergeAllCommand = new RelayCommand(MergeAll, CanMergeAll);
            UpdateCommand = new RelayCommand(Update, CanUpdate);
        }
        void Merge() {
            MergeHelper helper = new MergeHelper(Options, Port);
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
        public void Update() {
            DteWrapper dteWrapper = new DteWrapper(dte);
            Solution = dteWrapper.BuildTree();
            SelectedItem = null;
            PortOptionsViewModel portOptions = new PortOptionsViewModel(Solution.Path, Options);
            Port = new PortViewModel(portOptions, Options);
            MasterBranch = FindMasterBranch(portOptions);
            CanTotalMerge = MasterBranch != null;
        }
        bool CanUpdate() {
            return true;
        }
    }
}
