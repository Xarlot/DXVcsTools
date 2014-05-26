using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using DevExpress.Data.Access;
using DevExpress.Mvvm;
using DXVcsTools.UI.Navigator;

namespace DXVcsTools.UI {
    public class NavigationConfigViewModel : BindableBase {
        IEnumerable<string> roots;
        ObservableCollection<NavigateTreeItem> navigateHierarchy;
        string filterCriteriaString;

        public IEnumerable<ProjectType> ProjectTypes { get; private set; }
        public IEnumerable<string> Roots {
            get { return roots; }
            set { SetProperty(ref roots, value, () => Roots); }
        }
        public ObservableCollection<NavigateTreeItem> NavigateHierarchy {
            get { return navigateHierarchy; }
            private set { SetProperty(ref navigateHierarchy, value, () => NavigateHierarchy); }
        }
        public ObservableCollection<NavigatePreset> Presets { get; set; }
        public string FilterCriteriaString { 
            get { return filterCriteriaString; }
            set { SetProperty(ref filterCriteriaString, value, () => FilterCriteriaString); }
        }
        public bool ShouldSerializeProjectTypes() {
            return false;
        }
        public bool ShouldSerializeContent() {
            return false;
        }
        public bool ShouldSerializeGenerateCommand() {
            return false;
        }
        public bool ShouldSerializeOpenConfigLocationCommand() {
            return false;
        }
        public bool ShouldSerializeChoosePresetCommand() {
            return false;
        }
        public bool ShouldSerializeNavigateHierarchy() {
            return false;
        }
        public bool ShouldSerializeFilterCriteriaString() {
            return false;
        }
        public IEnumerable<NavigateItem> NavigateItems { get; set; }
        public ICommand GenerateCommand { get; private set; }
        public ICommand OpenConfigLocationCommand { get; private set; }
        public ICommand ChoosePresetCommand { get; private set; }

        public NavigationConfigViewModel() {
            ProjectTypes = new ObservableCollection<ProjectType>() { ProjectType.Unknown, ProjectType.SL, ProjectType.WPF, ProjectType.WinRT };
            GenerateCommand = new DelegateCommand(Generate, CanGenerate);
            OpenConfigLocationCommand = new DelegateCommand(OpenConfigLocation);
            ChoosePresetCommand = new DelegateCommand<string>(ChoosePreset);
        }
        void ChoosePreset(string filter) {
            FilterCriteriaString = filter;
        }
        void OpenConfigLocation() {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = SerializeHelper.NavigationConfigFilePath;
            Process.Start(startInfo);
        }
        bool CanGenerate() {
            return Roots != null;
        }
        readonly Dictionary<NavigateTreeItem, NavigateTreeItem> nodesCache = new Dictionary<NavigateTreeItem, NavigateTreeItem>();
        void Generate() {
            try {
                BusyIndicator.Show();
                NavigateItems = NavigateHelper.Scan(Roots);
                GenerateTreeSource();
            }
            finally {
                BusyIndicator.Close();
            }
        }
        public void GenerateTreeSource() {
            NavigateHierarchy = CreateNavigateHierarchy(NavigateItems);
        }
        ObservableCollection<NavigateTreeItem> CreateNavigateHierarchy(IEnumerable<NavigateItem> navigateItems) {
            nodesCache.Clear();
            ObservableCollection<NavigateTreeItem> list = new ObservableCollection<NavigateTreeItem>();
            if (navigateItems == null)
                return list;
            foreach (var item in navigateItems) {
                var hierarchy = GetHierarchy(item);
                foreach (var navItem in hierarchy)
                    if (navItem != null)
                        list.Add(navItem);
            }
            return list;
        }
        IEnumerable<NavigateTreeItem> GetHierarchy(NavigateItem item) {
            string rootPath = FindRootPath(item.Path);
            if (string.IsNullOrEmpty(rootPath) || !item.Path.StartsWith(rootPath))
                yield break;
            yield return GetUniqueItem(null, rootPath, null);
            string path = Path.GetDirectoryName(item.Path);
            yield return GetUniqueItem(path, item.Path, item);
            while (path != rootPath) {
                string next = Path.GetDirectoryName(path);
                yield return GetUniqueItem(next, path, null);
                path = next;
            }
        }
        NavigateTreeItem GetUniqueItem(string parentKey, string key, NavigateItem item) {
            var root = new NavigateTreeItem(parentKey, key, item);
            if (nodesCache.ContainsKey(root))
                return null;
            nodesCache.Add(root, root);
            return root;
        }
        public string GetRelativePath(NavigateItem item) {
            string path = item.Path;
            string rootPath = FindRootPath(item.Path);
            string replace = "$" + Path.GetFileName(rootPath);
            return path.Replace(rootPath, replace);
        }
        string FindRootPath(string path) {
            return Roots.First(path.StartsWith);
        }
        public void Save() {
            SerializeHelper.SerializeNavigationConfig(this);
        }
    }
}
