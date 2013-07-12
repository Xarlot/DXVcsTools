using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DXVcsTools.Core;
using DXVcsTools.DXVcsClient;
using DXVcsTools.UI;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Mvvm;
using DevExpress.Xpf.Mvvm.Native;
using DXVcsTools.UI.Logger;
using DXVcsTools.UI.ViewModel;
using EnvDTE;
using ProjectItem = DXVcsTools.Core.ProjectItem;

namespace DXVcsTools.VSIX {
    public class ToolWindowViewModel : BindableBase, IUpdatableViewModel, ISupportServices {
        const string Checkinwindow = "CheckInWindow";
        const string MultipleCheckinWindow = "MultipleCheckInWindow";
        const string ManualMergeWindow = "ManualMergeWindow";
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

            MergeCommand = new DelegateCommand<bool?>(Merge, CanMerge);
            MergeAllCommand = new DelegateCommand(MergeAll, CanMergeAll);
            UpdateCommand = new DelegateCommand(Update, CanUpdate);
            BlameCommand = new DelegateCommand(Blame, CanBlame);
            CheckInCommand = new DelegateCommand(CheckIn, CanCheckIn);
            CompareCurrentVersionCommand = new DelegateCommand(CompareWithCurrentVersion, CanCompareWithCurrentVersion);
            ComparePortVersionCommand = new DelegateCommand(CompareWithPortVersion, CanCompareWithPortVersion);
            ManualMergeCommand = new DelegateCommand(ManualMerge, CanManualMerge);
            NavigateToSolutionCommand = new DelegateCommand(NavigateToSolution, CanNavigateToSolution);
            UndoCheckoutCommand = new DelegateCommand(UndoCheckout, CanUndoCheckout);
            ShowLogCommand = new DelegateCommand(ShowLog, CanShowLog);
        }
        void ShowLog() {
            string log = Logger.GetLog();
            ShowLogHelper.ShowLog(log);
        }
        bool CanShowLog() {
            return true;
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

        public DelegateCommand<bool?> MergeCommand { get; private set; }
        public DelegateCommand MergeAllCommand { get; private set; }
        public DelegateCommand BlameCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand CheckInCommand { get; private set; }
        public DelegateCommand CompareCurrentVersionCommand { get; private set; }
        public DelegateCommand ComparePortVersionCommand { get; private set; }
        public DelegateCommand ManualMergeCommand { get; private set; }
        public DelegateCommand NavigateToSolutionCommand { get; private set; }
        public DelegateCommand UndoCheckoutCommand { get; private set; }
        public DelegateCommand ShowLogCommand { get; private set; }

        public IServiceContainer ServiceContainer { get; private set; }
        bool IsCorrectlyLoaded { get { return PortOptions.If(x => x.IsAttached).ReturnSuccess(); }}
        public void Update() {
            Logger.AddInfo("UpdateCommand. Start");

            var dteWrapper = new DteWrapper(dte);
            ThemeProvider.Instance.ThemeName = dteWrapper.GetVSTheme();
            Solution = dteWrapper.BuildTree();
            if (string.IsNullOrEmpty(Solution.Path)) {
                CanTotalMerge = false;
                Logger.AddInfo("UpdateCommand. End - cant merge since solution is empty or not under vss");
                return;
            }

            //since grid bugs we must initialize startup collection
            SelectedItems = new ObservableCollection<ProjectItemBase>();
            var source = GetFlatItemsSource().Where(item => item.IsCheckOut).ToList();
            Source = source;

            PortOptions = new PortOptionsViewModel(Solution, Options);
            if (!PortOptions.IsAttached) {
                Logger.AddInfo("UpdateCommand. End - cant merge since port is not initialized");
                CanTotalMerge = false;  
                return;
            }
            MasterBranch = FindMasterBranch(PortOptions);
            CanTotalMerge = MasterBranch != null;
            PortOptions.MasterBranch = MasterBranch;

            currentBranchLocker.DoIfNotLocked(() => CurrentBranch = Options.Branches.LastOrDefault(item => item != MasterBranch));
            MergeProgress = 0;

            Logger.AddInfo("UpdateCommand. End - successful initialized");
        }
        DXVcsBranch FindMasterBranch(PortOptionsViewModel portOptions) {
            string relativePath = portOptions.GetRelativePath(PortOptions.ProjectFilePath);
            return Options.Branches.FirstOrDefault(branch => relativePath.StartsWith(branch.Path, StringComparison.OrdinalIgnoreCase));
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
            return IsCorrectlyLoaded;
        }
        bool CanUpdate() {
            return IsCorrectlyLoaded;
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
            return IsCorrectlyLoaded && SelectedItem != null;
        }
        bool CanCheckIn() {
            return IsCorrectlyLoaded && SelectedItem.If(x => x.IsCheckOut).ReturnSuccess();
        }
        void CheckIn() {
            if (IsSingleSelection) {
                Logger.AddInfo("CheckInCommand. Start single check in.");

                var model = new CheckInViewModel(SelectedItem.Path, false);
                MessageBoxResult result = GetService<IDialogService>(Checkinwindow).ShowDialog(MessageBoxButton.OKCancel, "Check in", model);
                if (result == MessageBoxResult.OK) {
                    var helper = new MergeHelper(Options, PortOptions);
                    helper.CheckIn(model);
                    SelectedItem.IsChecked = model.StaysChecked;
                }
                Logger.AddInfo("CheckInCommand. End single check in.");
            }
            else {
                Logger.AddInfo("CheckInCommand. Start multiple check in.");

                var model = new CheckInViewModel(Solution.Path, false);
                var result = GetService<IDialogService>(MultipleCheckinWindow).ShowDialog(MessageBoxButton.OKCancel, "Multiple Check in", model);
                if (result == MessageBoxResult.OK) {
                    var helper = new MergeHelper(Options, PortOptions);
                    foreach (var item in SelectedItems) {
                        var currentFileModel = new CheckInViewModel(item.Path, model.StaysChecked) {Comment = model.Comment};
                        bool success = helper.CheckIn(currentFileModel);
                        item.IsChecked = success && model.StaysChecked;
                    }
                }

                Logger.AddInfo("CheckInCommand. End multiple check in.");
            }
            ReloadProject();
        }
        bool CanCompareWithCurrentVersion() {
            return IsSingleSelection && SelectedItem.If(x => x.IsCheckOut).ReturnSuccess();
        }
        void CompareWithCurrentVersion() {
            var helper = new MergeHelper(Options, PortOptions);
            helper.CompareWithCurrentVersion(SelectedItem.Path);
        }
        bool CanCompareWithPortVersion() {
            if (!IsCorrectlyLoaded)
                return false;
            if (CurrentBranch == null)
                return false;
            return IsSingleSelection && SelectedItem.If(x => x.IsCheckOut).ReturnSuccess();
        }
        void CompareWithPortVersion() {
            var helper = new MergeHelper(Options, PortOptions);
            helper.CompareWithPortVersion(SelectedItem.Path, CurrentBranch);
        }
        bool CanManualMerge() {
            return IsCorrectlyLoaded && IsSingleSelection && SelectedItem.If(x => x.IsCheckOut).ReturnSuccess();
        }
        void ManualMerge() {
            Logger.AddInfo("ManualMergeCommand. Merge start.");
            
            var helper = new MergeHelper(Options, PortOptions);
            var manualMerge = new ManualMergeViewModel(SelectedItem.Path);
            SelectedItem.MergeState = helper.ManualMerge(CurrentBranch, manualMerge,
                () => GetService<IDialogService>(ManualMergeWindow).ShowDialog(MessageBoxButton.OKCancel, "Manual merge", manualMerge) == MessageBoxResult.OK);
            Logger.AddInfo("ManualMergeCommand. Merge end.");
        }
        bool CanNavigateToSolution() {
            return IsCorrectlyLoaded && CurrentBranch != null;
        }
        void NavigateToSolution() {
            var helper = new MergeHelper(Options, PortOptions);
            helper.NavigateToSolution(CurrentBranch, new DteWrapper(dte));
            Update();
        }
        bool CanUndoCheckout() {
            return IsCorrectlyLoaded && (IsSingleSelection ? SelectedItem != null : SelectedItems.Count > 0);
        }
        void UndoCheckout() {
            var helper = new MergeHelper(Options, PortOptions);
            if (IsSingleSelection) {
                SelectedItem.IsCheckOut = helper.UndoCheckout(SelectedItem.Path);
                SelectedItem.Save();
            }
            else {
                foreach (var item in SelectedItems) {
                    item.IsCheckOut = helper.UndoCheckout(item.Path);
                    item.Save();
                }
            }
            ReloadProject();
        }


        protected virtual T GetService<T>(string key, ServiceSearchMode searchMode = ServiceSearchMode.PreferLocal) where T : class {
            return ServiceContainer.GetService<T>(key, searchMode);
        }
        void ReloadProject() {
//            dte.ExecuteCommand("Project.ReloadProject");
        }
    }
}