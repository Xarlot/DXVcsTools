﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpf.Core;
using DXVcsTools.Core;
using DXVcsTools.DXVcsClient;
using DXVcsTools.UI;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DXVcsTools.UI.Logger;
using DXVcsTools.UI.ViewModel;
using DXVcsTools.ViewModels;
using EnvDTE;
using ProjectItem = DXVcsTools.Core.ProjectItem;

namespace DXVcsTools.VSIX {
    public class ToolWindowViewModel : BindableBaseCore, IToolWindowViewModel, IUpdatableViewModel, ISupportServices {
        const string Checkinwindow = "CheckInWindow";
        const string MultipleCheckinWindow = "MultipleCheckInWindow";
        const string ManualMergeWindow = "ManualMergeWindow";
        const string NavigationConfigWindow = "NavigationConfigWindow";
        readonly IDteWrapper dte;
        bool canTotalMerge;
        DXVcsBranch currentBranch;
        IEnumerable flatSource;
        DXVcsBranch masterBranch;
        double mergeProgress;
        ProjectItemBase selectedItem;
        ObservableCollection<ProjectItemBase> selectedItems;
        SolutionItem solutionItem;
        CriteriaOperator filterCriteria;
        readonly Func<InternalBlameWindow> internalBlameWindowAccessor;
        readonly Locker currentBranchLocker = new Locker();
        readonly GenerateMenuItemsHelper generateMenuItemsHelper;
        Func<ProjectItemBase, bool> filterPredicate = x => true;

        public ToolWindowViewModel(DTE dte, OptionsViewModel options, GenerateMenuItemsHelper generateMenuHelper, Func<InternalBlameWindow> internalBlameWindowAccessor) {
            this.generateMenuItemsHelper = generateMenuHelper;
            this.dte = new DteWrapper(dte);
            this.internalBlameWindowAccessor = internalBlameWindowAccessor;
            Options = options;
            ServiceContainer = new ServiceContainer(this);

            MergeCommand = new DelegateCommand<bool?>(Merge, CanMerge);
            MergeAllCommand = new DelegateCommand(MergeAll, CanMergeAll);
            UpdateCommand = new DelegateCommand(Update, CanUpdate);
            BlameCommand = new DelegateCommand(Blame, CanBlame);
            CheckInCommand = new DelegateCommand<CheckInTarget>(CheckIn, CanCheckIn);
            CompareCurrentVersionCommand = new DelegateCommand(CompareWithCurrentVersion, CanCompareWithCurrentVersion);
            ComparePortVersionCommand = new DelegateCommand(CompareWithPortVersion, CanCompareWithPortVersion);
            CompareCurrentWithPortCommand = new DelegateCommand(CompareCurrentWithPortVersion, CanCompareCurrentWithPortVersion);
            ManualMergeCommand = new DelegateCommand(ManualMerge, CanManualMerge);
            NavigateToSolutionCommand = new DelegateCommand(NavigateToSolution, CanNavigateToSolution);
            UndoCheckoutCommand = new DelegateCommand(UndoCheckout, CanUndoCheckout);
            ShowLogCommand = new DelegateCommand(ShowLog, CanShowLog);
            NavigateToFileCommand = new DelegateCommand<ProjectItemBase>(NavigateToItem, CanNavigateToItem);
            ShowAllCommand = new DelegateCommand(ShowAlllItems, CanShowAllItems);
            ShowCheckoutOnlyCommand = new DelegateCommand(ShowCheckoutOnly, CanSHowCheckoutOnly);
            ShowNewOnlyCommand = new DelegateCommand(ShowNewOnly, CanShowNewOnly);

            FilterCriteria = CreateFilterCriteria(Options.StartupFilterType);
        }
        bool CanShowNewOnly() {
            return IsCorrectlyLoaded;
        }
        void ShowNewOnly() {
            FilterCriteria = CreateFilterCriteria(StartupFilterType.New);
        }
        bool CanSHowCheckoutOnly() {
            return IsCorrectlyLoaded;
        }
        void ShowCheckoutOnly() {
            FilterCriteria = CreateFilterCriteria(StartupFilterType.CheckedOut);
        }
        CriteriaOperator CreateFilterCriteria(StartupFilterType filterType) {
            if (filterType == StartupFilterType.All)
                return null;
            if (filterType == StartupFilterType.CheckedOut)
                return CriteriaOperator.Parse("[IsCheckOut] = True");
            if (filterType == StartupFilterType.New)
                return CriteriaOperator.Parse("[IsNew] = true");
            throw new ArgumentException("filter");
        }
        bool CanShowAllItems() {
            return IsCorrectlyLoaded;
        }
        void ShowAlllItems() {
            FilterCriteria = CreateFilterCriteria(StartupFilterType.All);
        }
        bool UseFlatUI { get { return Options.UseFlatUI; } }
        StartupFilterType FilterType { get; set; }
        bool CanCompareCurrentWithPortVersion() {
            if (!IsCorrectlyLoaded)
                return false;
            if (CurrentBranch == null)
                return false;
            return IsSingleSelection && SelectedItem.Return(x => !x.IsNew, () => false);
        }
        void CompareCurrentWithPortVersion() {
            var helper = new MergeHelper(this);
            helper.CompareCurrentWithPortVersion(SelectedItem.Path, CurrentBranch);
        }
        void NavigateToItem(ProjectItemBase parameter) {
            var item = parameter ?? SelectedItem;
            dte.NavigateToFile(item);
        }
        bool CanNavigateToItem(ProjectItemBase parameter) {
            return IsCorrectlyLoaded && parameter != null;
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
        public CriteriaOperator FilterCriteria {
            get { return filterCriteria; }
            set {
                SetProperty(ref filterCriteria, value, "FilterCriteria", () => {
                    UpdateFilterPredicate();
                    CommandManager.InvalidateRequerySuggested();
                });
            }
        }
        void UpdateFilterPredicate() {
            if (object.Equals(FilterCriteria, null))
                filterPredicate = x => true;
            else {
                CriteriaCompiledContextDescriptorTyped criteria = new CriteriaCompiledContextDescriptorTyped(typeof(ProjectItemBase));
                var filter = CriteriaCompiler.ToPredicate<ProjectItemBase>(FilterCriteria, criteria);
                filterPredicate = x => {
                    bool fileItem = x is FileItem;
                    return !fileItem || filter(x);
                };
            }
        }

        void SelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs) {
            CommandManager.InvalidateRequerySuggested();
        }
        void SelectedItemsChanged() {
            CommandManager.InvalidateRequerySuggested();
        }
        public PortOptionsViewModel PortOptions { get; private set; }
        public OptionsViewModel Options { get; private set; }
        public bool IsSingleSelection { get { return SelectedItems.If(x => x.Count <= 1).ReturnSuccess(); } }

        public DelegateCommand<bool?> MergeCommand { get; private set; }
        public DelegateCommand MergeAllCommand { get; private set; }
        public DelegateCommand BlameCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand<CheckInTarget> CheckInCommand { get; private set; }
        public DelegateCommand CompareCurrentVersionCommand { get; private set; }
        public DelegateCommand ComparePortVersionCommand { get; private set; }
        public DelegateCommand ManualMergeCommand { get; private set; }
        public DelegateCommand CompareCurrentWithPortCommand { get; private set; }
        public DelegateCommand NavigateToSolutionCommand { get; private set; }
        public DelegateCommand UndoCheckoutCommand { get; private set; }
        public DelegateCommand ShowLogCommand { get; private set; }
        public DelegateCommand StatusCheckOutCommand { get; private set; }
        public DelegateCommand ShowCheckoutOnlyCommand { get; private set; }
        public DelegateCommand ShowNewOnlyCommand { get; private set; }
        public DelegateCommand ShowAllCommand { get; private set; }
        public DelegateCommand<ProjectItemBase> NavigateToFileCommand { get; private set; }
        public ChildNodesSelector Selector { get; private set; }

        public IServiceContainer ServiceContainer { get; private set; }
        bool IsCorrectlyLoaded { get { return PortOptions.If(x => x.IsAttached).ReturnSuccess(); } }
        string CalcVsTheme(VSTheme theme) {
            switch (theme) {
                case VSTheme.Light:
                    return Options.LightThemeName;
                case VSTheme.Dark:
                    return Options.DarkThemeName;
                case VSTheme.Blue:
                    return Options.BlueThemeName;
                case VSTheme.Unknown:
                    return Options.LightThemeName;
                default:
                    return Options.LightThemeName;
            }
        }
        public void UpdateConnection() {
            InitializeConnection();
        }
        public void Update() {
            Logger.AddInfo("UpdateCommand. Start");

            if (!InitializeConnection())
                return;
            if (!PortOptions.IsAttached) {
                Logger.AddInfo("UpdateCommand. End - cant merge since port is not initialized");
                CanTotalMerge = false;
                return;
            }


            MasterBranch = FindMasterBranch(PortOptions);
            CanTotalMerge = MasterBranch != null;
            PortOptions.MasterBranch = MasterBranch;

            MergeHelper helper = new MergeHelper(this);
            SelectedItems = new ObservableCollection<ProjectItemBase>();
            Selector = new ChildNodesSelector(item => FilterItems(item, helper));
            var source = GenerateSource(helper);
            Source = source;

            currentBranchLocker.DoIfNotLocked(() => CurrentBranch = Options.Branches.LastOrDefault(item => item != MasterBranch));
            MergeProgress = 0;

            Logger.AddInfo("UpdateCommand. End - successful initialized");
        }
        IEnumerable GenerateSource(MergeHelper helper) {
            if (UseFlatUI)
                return GetFlatItemsSource().Where(item => !FilterItems(item, helper)).ToList();
            return GetTreeItemsSource();
        }
        bool InitializeConnection() {
            ThemeProvider.Instance.ThemeName = dte.GetVSTheme(CalcVsTheme);
            Solution = dte.BuildTree();
            if (string.IsNullOrEmpty(Solution.Path)) {
                CanTotalMerge = false;
                Logger.AddInfo("UpdateCommand. End - cant merge since solution is empty or not under vss");
                return false;
            }
            //since grid bugs we must initialize startup collection

            PortOptions = new PortOptionsViewModel(Solution, Options);
            return true;
        }
        bool FilterItems(ProjectItemBase item, MergeHelper helper) {
            return (item.IsNew && !string.IsNullOrEmpty(item.FullPath) && !helper.IsItemUnderVss(item.FullPath, MasterBranch));
        }
        DXVcsBranch FindMasterBranch(PortOptionsViewModel portOptions) {
            string relativePath = portOptions.GetRelativePath(PortOptions.ProjectFilePath);
            return Options.Branches.FirstOrDefault(branch => relativePath.StartsWith(branch.Path, StringComparison.OrdinalIgnoreCase));
        }
        void Merge(bool? parameter) {
            bool showPreview = parameter.HasValue ? parameter.Value : Options.ReviewTarget;
            var items = IsSingleSelection ? (IEnumerable<ProjectItemBase>)new List<ProjectItemBase> { SelectedItem } : SelectedItems;
            List<ProjectItemBase> itemsForMerge = CalcItemsForMerge(items);
            foreach (var item in itemsForMerge) {
                item.MergeState = PerformMerge(item, showPreview);
            }
        }
        List<ProjectItemBase> CalcItemsForMerge(IEnumerable<ProjectItemBase> items) {
            IEnumerable<ProjectItemBase> projectItemBase = items as IList<ProjectItemBase> ?? items.ToList();
            var result = projectItemBase;
            result = projectItemBase.Aggregate(result, (current, item) => current.Concat(GetTotalChildren(item)));
            var itemsForMerge = result.Where(x => filterPredicate(x)).Distinct().ToList();
            return itemsForMerge;
        }
        bool CanMerge(bool? parameter) {
            var items = IsSingleSelection ? (IEnumerable<ProjectItemBase>)SelectedItem.Return(x => new List<ProjectItemBase> { x }, () => new List<ProjectItemBase>()) : SelectedItems;
            List<ProjectItemBase> itemsForMerge = CalcItemsForMerge(items);
            return itemsForMerge.Return(x => x.Any(CalcCanMergeItem), () => false);
        }
        bool CalcCanMergeItem(ProjectItemBase item) {
            var folderItem = item as FolderItem;
            if (folderItem != null)
                return CanMergeFolderItem(folderItem);
            return CanMergeFileItem(item);
        }
        bool CanMergeFolderItem(FolderItem folderItem) {
            var children = GetTotalChildren(folderItem);
            return children.Any(CanMergeFileItem);
        }
        bool CanMergeFileItem(ProjectItemBase x) {
            return (x.IsCheckOut || x.IsNew) && x.MergeState == MergeState.None;
        }
        MergeState PerformMerge(ProjectItemBase item, bool showPreview) {
            if (item is FolderItem)
                return MergeState.Success;
            var helper = new MergeHelper(this);
            item.ItemWrapper.Save();
            return helper.MergeChanges(CurrentBranch, item.Path, null, showPreview, item.IsNew);
        }
        void MergeAll() {
            var items = CalcItemsForMerge(new[] {Solution});
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
        IEnumerable<ProjectItemBase> GetTreeItemsSource() {
            if (Solution == null)
                return new List<ProjectItemBase>();
            return new List<ProjectItemBase>() { Solution };
        }
        IEnumerable<ProjectItemBase> GetChildren(ProjectItemBase root) {
            if (root.Children == null)
                yield break;
            foreach (ProjectItemBase item in root.Children) {
                if (item is ProjectItem)
                    yield return item;
                if (item is FileItem)
                    yield return item;
                foreach (var subItem in GetChildren(item)) {
                    if (subItem is FileItem)
                        yield return subItem;
                }
            }
        }
        IEnumerable<ProjectItemBase> GetTotalChildren(ProjectItemBase root) {
            if (root.Children == null)
                yield break;
            foreach (ProjectItemBase item in root.Children) {
                yield return item;

                foreach (ProjectItemBase subItem in GetTotalChildren(item)) {
                    yield return subItem;
                }
            }
        }
        void CurrentBranchChanged() {
            currentBranchLocker.DoLockedAction(Update);
            CommandManager.InvalidateRequerySuggested();
        }
        void Blame() {
            var helper = new MergeHelper(this);
        }
        bool CanBlame() {
            return IsCorrectlyLoaded && SelectedItem != null;
        }
        bool CanCheckIn(CheckInTarget target) {
            if (!IsCorrectlyLoaded)
                return false;
            if (IsSingleSelection)
                return CanCheckInItem(target, SelectedItem);
            return SelectedItems.All(item => CanCheckInItem(target, item));
        }
        bool CanCheckInItem(CheckInTarget target, ProjectItemBase item) {
            bool canCheckInMaster = IsCorrectlyLoaded && item.If(x => x.IsCheckOut || x.IsNew).ReturnSuccess();
            if (target == CheckInTarget.Master)
                return canCheckInMaster;
            return canCheckInMaster && item.If(x => x.MergeState == MergeState.Success).ReturnSuccess();
        }
        string GetCheckInPath(CheckInTarget target, string selectedItemPath) {
            switch (target) {
                case CheckInTarget.Master:
                    return selectedItemPath;
                case CheckInTarget.Port:
                    MergeHelper helper = new MergeHelper(this);
                    return helper.GetFilePathForBranch(selectedItemPath, CurrentBranch);
                default:
                    throw new ArgumentException("target");
            }
        }
        void CheckIn(CheckInTarget target) {
            if (IsSingleSelection) {
                Logger.AddInfo("CheckInCommand. Start single check in.");

                var model = new CheckInViewModel(GetCheckInPath(target, SelectedItem.Path), false);
                MessageResult result = GetService<IDialogService>(Checkinwindow).ShowDialog(MessageButton.OKCancel, "Check in", model);
                if (result == MessageResult.OK) {
                    var helper = new MergeHelper(this);
                    helper.CheckIn(new CheckInViewModel(SelectedItem.Path, model.StaysChecked) { Comment = model.Comment }, GetCheckInBranch(target), SelectedItem.IsNew);
                    SelectedItem.IsChecked = model.StaysChecked;
                }

                Logger.AddInfo("CheckInCommand. End single check in.");
            }
            else {
                Logger.AddInfo("CheckInCommand. Start multiple check in.");

                var model = new CheckInViewModel(GetCheckInPath(target, Solution.Path), false);
                var result = GetService<IDialogService>(MultipleCheckinWindow).ShowDialog(MessageButton.OKCancel, "Multiple Check in", model);
                if (result == MessageResult.OK) {
                    var helper = new MergeHelper(this);
                    foreach (var item in SelectedItems) {
                        var currentFileModel = new CheckInViewModel(item.Path, model.StaysChecked) { Comment = model.Comment };
                        bool success = helper.CheckIn(currentFileModel, GetCheckInBranch(target), item.IsNew);
                        item.IsChecked = success && model.StaysChecked;
                    }
                }

                Logger.AddInfo("CheckInCommand. End multiple check in.");
            }
            ReloadProject();
        }
        DXVcsBranch GetCheckInBranch(CheckInTarget target) {
            return target == CheckInTarget.Master ? MasterBranch : CurrentBranch;
        }
        bool CanCompareWithCurrentVersion() {
            return IsSingleSelection && SelectedItem.If(x => x.IsCheckOut).ReturnSuccess();
        }
        void CompareWithCurrentVersion() {
            var helper = new MergeHelper(this);
            helper.CompareWithCurrentVersion(SelectedItem.Path, SelectedItem.IsNew);
        }
        bool CanCompareWithPortVersion() {
            if (!IsCorrectlyLoaded)
                return false;
            if (CurrentBranch == null)
                return false;
            return IsSingleSelection && SelectedItem.If(x => x.IsCheckOut || (x.IsNew && x.MergeState == MergeState.Success)).ReturnSuccess();
        }
        void CompareWithPortVersion() {
            var helper = new MergeHelper(this);
            helper.CompareWithPortVersion(SelectedItem.Path, CurrentBranch, SelectedItem.IsNew);
        }
        bool CanManualMerge() {
            return IsCorrectlyLoaded && IsSingleSelection && SelectedItem.If(x => x is FileItem).ReturnSuccess();
        }
        void ManualMerge() {
            Logger.AddInfo("ManualMergeCommand. Merge start.");

            var helper = new MergeHelper(this);
            var manualMerge = new ManualMergeViewModel(SelectedItem.Path);
            SelectedItem.MergeState = helper.ManualMerge(CurrentBranch, manualMerge,
                () => GetService<IDialogService>(ManualMergeWindow).ShowDialog(MessageButton.OKCancel, "Manual merge", manualMerge) == MessageResult.OK);
            Logger.AddInfo("ManualMergeCommand. Merge end.");
        }
        bool CanNavigateToSolution() {
            return IsCorrectlyLoaded && CurrentBranch != null;
        }
        void NavigateToSolution() {
            var helper = new MergeHelper(this);
            helper.NavigateToSolution(CurrentBranch, dte);
            Update();
        }
        public void NavigateToSolution(string path) {
            var helper = new MergeHelper(this);
            helper.NavigateToSolution(path, dte);
        }
        bool CanUndoCheckout() {
            return IsCorrectlyLoaded && (IsSingleSelection ? SelectedItem != null : SelectedItems.Count > 0);
        }
        void UndoCheckout() {
            var helper = new MergeHelper(this);
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
            dte.ReloadProject();
            Update();
        }
        public void ShowNavigationConfig() {
            Logger.AddInfo("ShowNavigationConfigCommand. Start.");

            try {
                NavigationConfigViewModel model = SerializeHelper.DeSerializeNavigationConfig();
                if (PortOptions != null) {
                    MergeHelper helper = new MergeHelper(this);
                    IEnumerable<string> roots = helper.FindWorkingFolders(Options.Branches);
                    model.Roots = roots;
                }

                model.GenerateTreeSource();
                if (GetService<IDialogService>(NavigationConfigWindow).ShowDialog(MessageButton.OKCancel, "Navigation config", model) == MessageResult.OK) {
                    model.Save();
                    generateMenuItemsHelper.Release();
                    generateMenuItemsHelper.GenerateDefault();
                    generateMenuItemsHelper.GenerateMenus();
                }
            }
            catch (Exception e) {
                Logger.AddError("ShowNavigationConfigCommand. Failed.", e);
            }

            Logger.AddInfo("ShowNavigationConfigCommand. End.");
        }
        public void ShowBlame() {
            string path = dte.GetActiveDocument();
            if (!CanHandleActiveDocument(path))
                return;


            int? lineNumber = dte.GetSelectedLine();
            BlameHelper helper = new BlameHelper(this);
            if (Options.BlameType == DXBlameType.External)
                helper.ShowExternalBlame(path, lineNumber);
            else
                helper.ShowInternalBlame(path, lineNumber, model => internalBlameWindowAccessor().Initialize(model));
        }
        private bool CanHandleActiveDocument(string fileName) {
            if (string.IsNullOrEmpty(fileName)) {
                MessageBox.Show("No current document.", "DXVcsTools", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (!dte.IsItemUnderScc(fileName)) {
                MessageBox.Show(string.Concat("File ", fileName, " is not under source control."), "DXVcsTools", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }

    }

    public enum CheckInTarget {
        Master,
        Port
    }
}