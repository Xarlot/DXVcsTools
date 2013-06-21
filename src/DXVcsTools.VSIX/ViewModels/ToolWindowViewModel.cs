using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using DXVcsTools.Core;
using DXVcsTools.UI;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Mvvm;
using DevExpress.Xpf.Mvvm.Native;
using EnvDTE;

namespace DXVcsTools.VSIX {
    public class ToolWindowViewModel : BindableBase, IUpdatableViewModel, ISupportServices {
        readonly DTE dte;
        bool canTotalMerge;
        DXVcsBranch currentBranch;
        IEnumerable flatSource;
        DXVcsBranch masterBranch;
        double mergeProgress;
        ProjectItemBase selectedItem;
        ObservableCollection<ProjectItemBase> selectedItems;
        SolutionItem solutionItem;
        readonly Locker currentBranchLocker = new Locker();
        public ToolWindowViewModel(DTE dte, OptionsViewModel options) {
            this.dte = dte;
            Options = options;
            ServiceContainer = new ServiceContainer(this);

            MergeCommand = new RelayCommand<bool?>(Merge, CanMerge);
            MergeAllCommand = new RelayCommand(MergeAll, CanMergeAll);
            UpdateCommand = new DelegateCommand(Update, CanUpdate);
            BlameCommand = new RelayCommand(Blame, CanBlame);
            CheckInCommand = new RelayCommand(CheckIn, CanCheckIn);
            CompareCurrentVersionCommand = new RelayCommand(CompareWithCurrentVersion, CanCompareWithCurrentVersion);
            ComparePortVersionCommand = new RelayCommand(CompareWithPortVersion, CanCompareWithPortVersion);
            ManualMergeCommand = new RelayCommand(ManualMerge, CanManualMerge);
            NavigateToSolutionCommand = new RelayCommand(NavigateToSolution, CanNavigateToSolution);
        }

        public double MergeProgress {
            get { return mergeProgress; }
            set { SetProperty(ref mergeProgress, value, "MergeProgress"); }
        }
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
            set { SetProperty(ref currentBranch, value, "CurrentBranch", CurrentBranchChanged); }
        }
        public IEnumerable<DXVcsBranch> AvailableBranches {
            get { return Options.Branches; }
        }
        public SolutionItem Solution {
            get { return solutionItem; }
            set { SetProperty(ref solutionItem, value, "Solution"); }
        }
        public IEnumerable Source {
            get { return flatSource; }
            private set { SetProperty(ref flatSource, value, "Source", CommandManager.InvalidateRequerySuggested); }
        }
        public ProjectItemBase SelectedItem {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value, "SelectedItem", CommandManager.InvalidateRequerySuggested); }
        }
        public ObservableCollection<ProjectItemBase> SelectedItems {
            get { return selectedItems; }
            set {
                selectedItems.Do(x => x.CollectionChanged -= SelectedItemsCollectionChanged);
                SetProperty(ref selectedItems, value, "SelectedItems", SelectedItemsChanged);
                selectedItems.Do(x => x.CollectionChanged += SelectedItemsCollectionChanged);
            }
        }
        void SelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs) {
            CommandManager.InvalidateRequerySuggested();
        }
        void SelectedItemsChanged() {
            CommandManager.InvalidateRequerySuggested();
        }
        public UIType UIType { get { return UIType.Flat; } }
        PortOptionsViewModel PortOptions { get; set; }
        public OptionsViewModel Options { get; private set; }
        public bool IsSingleSelection { get { return SelectedItems.If(x => x.Count <= 1).ReturnSuccess(); } }

        public RelayCommand<bool?> MergeCommand { get; private set; }
        public RelayCommand MergeAllCommand { get; private set; }
        public RelayCommand BlameCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }
        public RelayCommand CheckInCommand { get; private set; }
        public RelayCommand CompareCurrentVersionCommand { get; private set; }
        public RelayCommand ComparePortVersionCommand { get; private set; }
        public RelayCommand ManualMergeCommand { get; private set; }
        public RelayCommand NavigateToSolutionCommand { get; private set; }

        public IServiceContainer ServiceContainer { get; private set; }
        public void Update() {
            var dteWrapper = new DteWrapper(dte);
            Solution = dteWrapper.BuildTree();
            //since grid bugs we must initialize startup collection
            SelectedItems = new ObservableCollection<ProjectItemBase>();
            var source = GetFlatItemsSource().Where(item => item.IsCheckOut).ToList();
            Source = source;

            PortOptions = new PortOptionsViewModel(Solution.Path, Options);
            MasterBranch = FindMasterBranch(PortOptions);
            CanTotalMerge = MasterBranch != null;
            PortOptions.MasterBranch = MasterBranch;

            currentBranchLocker.DoIfNotLocked(() => CurrentBranch = Options.Branches.LastOrDefault(item => item != MasterBranch));
            MergeProgress = 0;
        }

        DXVcsBranch FindMasterBranch(PortOptionsViewModel portOptions) {
            string relativePath = portOptions.GetRelativePath(Solution.Path);
            return Options.Branches.FirstOrDefault(branch => relativePath.StartsWith(branch.Path));
        }
        void Merge(bool? parameter) {
            bool showPreview = parameter.HasValue ? parameter.Value : Options.ReviewTarget;
            var items = IsSingleSelection ? (IEnumerable)new List<ProjectItemBase> { SelectedItem } : SelectedItems;
            foreach (ProjectItemBase item in items) {
                item.MergeState = PerformMerge(item, showPreview);
            }
        }
        bool CanMerge(bool? parameter) {
            return IsSingleSelection
                ? SelectedItem.Return(x => x.IsCheckOut && x.MergeState == MergeState.None, () => false)
                : SelectedItems.Return(x => x.Select(item => item.IsCheckOut && item.MergeState == MergeState.None).Any(), () => false);

        }
        MergeState PerformMerge(ProjectItemBase item, bool showPreview) {
            var helper = new MergeHelper(Options, PortOptions);
            return helper.MergeChanges(CurrentBranch, item.Path, null, showPreview);
        }
        void MergeAll() {
            List<ProjectItemBase> items = Source.Cast<ProjectItemBase>().Where(item => item.MergeState == MergeState.None).ToList();
            foreach (ProjectItemBase item in items) {
                item.MergeState = PerformMerge(item, false);
            }
        }
        bool CanMergeAll() {
            return true;
        }
        bool CanUpdate() {
            return true;
        }
        IEnumerable<ProjectItemBase> GetFlatItemsSource() {
            if (Solution == null)
                return new List<ProjectItemBase>();
            return GetChildren(Solution);
        }
        IEnumerable<ProjectItemBase> GetChildren(ProjectItemBase root) {
            if (root.Children == null)
                yield break;
            foreach (ProjectItemBase item in root.Children) {
                if (item is FileItem)
                    yield return item;
                foreach (ProjectItemBase subItem in GetChildren(item)) {
                    if (subItem is FileItem)
                        yield return subItem;
                }
            }
        }
        void CurrentBranchChanged() {
            currentBranchLocker.DoLockedAction(Update);
            CommandManager.InvalidateRequerySuggested();
        }
        void Blame() {
            var helper = new MergeHelper(Options, PortOptions);
        }
        bool CanBlame() {
            return SelectedItem != null;
        }
        bool CanCheckIn() {
            return SelectedItem.If(x => x.IsCheckOut).ReturnSuccess();
        }
        void CheckIn() {
            if (IsSingleSelection) {
                var model = new CheckInViewModel(SelectedItem.Path, false);
                bool? result = GetService<IDialogService>().ShowDialog("CheckInControl", model, "Check in");
                if (result != null && (bool)result) {
                    var helper = new MergeHelper(Options, PortOptions);
                    helper.CheckIn(model);
                    SelectedItem.IsChecked = model.StaysChecked;
                }
            }
            else {
                var model = new CheckInViewModel(Solution.Path, false);
                bool? result = GetService<IDialogService>().ShowDialog("MultipleCheckInControl", model, "Multiple Check in");
                if (result != null && (bool)result) {
                    var helper = new MergeHelper(Options, PortOptions);
                    foreach (var item in SelectedItems) {
                        helper.CheckIn(model);
                        item.IsChecked = model.StaysChecked;
                    }
                }
            }
        }
        bool CanCompareWithCurrentVersion() {
            return IsSingleSelection && SelectedItem.If(x => x.IsCheckOut).ReturnSuccess();
        }
        void CompareWithCurrentVersion() {
            var helper = new MergeHelper(Options, PortOptions);
            helper.CompareWithCurrentVersion(SelectedItem.Path);
        }
        bool CanCompareWithPortVersion() {
            if (CurrentBranch == null)
                return false;
            return IsSingleSelection && SelectedItem.If(x => x.IsCheckOut).ReturnSuccess();
        }
        void CompareWithPortVersion() {
            var helper = new MergeHelper(Options, PortOptions);
            helper.CompareWithPortVersion(SelectedItem.Path, CurrentBranch);
        }
        bool CanManualMerge() {
            return IsSingleSelection && SelectedItem.If(x => x.IsCheckOut).ReturnSuccess();
        }
        void ManualMerge() {
            var helper = new MergeHelper(Options, PortOptions);
            var manualMerge = new ManualMergeViewModel(SelectedItem.Path);
            SelectedItem.MergeState = helper.ManualMerge(CurrentBranch, manualMerge,
                () => GetService<IDialogService>().ShowDialog("ManualMergeControl", manualMerge, "Manual merge").Return(x => x.Value, () => false));
        }
        bool CanNavigateToSolution() {
            return CurrentBranch != null;
        }
        void NavigateToSolution() {
            var helper = new MergeHelper(Options, PortOptions);
            helper.NavigateToSolution(CurrentBranch, new DteWrapper(dte));
            Update();
        }

        protected virtual T GetService<T>(ServiceSearchMode searchMode = ServiceSearchMode.PreferLocal) where T : class {
            return ServiceContainer.GetService<T>(searchMode);
        }
    }
}