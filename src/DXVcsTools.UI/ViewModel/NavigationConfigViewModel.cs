using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using DevExpress.Xpf.Mvvm;
using DXVcsTools.UI.Navigator;
using log4net.Repository.Hierarchy;

namespace DXVcsTools.UI {
    public class NavigationConfigViewModel : BindableBase {
        string content;
        IEnumerable<string> roots;
        ObservableCollection<NavigateTreeItem> navigateHierarchy;

        public IEnumerable<string> Roots {
            get { return roots; }
            set { SetProperty(ref roots, value, () => Roots); }
        }
        public ObservableCollection<NavigateTreeItem> NavigateHierarchy {
            get { return navigateHierarchy; }
            private set { SetProperty(ref navigateHierarchy, value, () => NavigateHierarchy); }
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
        public bool ShouldSerializeNavigateHierarchy() {
            return false;
        }
        public IEnumerable<NavigateItem> NavigateItems { get; set; }
        public ICommand GenerateCommand { get; private set; }
        public ICommand OpenConfigLocationCommand { get; private set; }

        public NavigationConfigViewModel() {
            GenerateCommand = new DelegateCommand(Generate, CanGenerate);
            OpenConfigLocationCommand = new DelegateCommand(OpenConfigLocation);
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
            NavigateItems = NavigateHelper.Scan(Roots);
            NavigateHierarchy = CreateNavigateHierarchy(NavigateItems);
        }
        ObservableCollection<NavigateTreeItem> CreateNavigateHierarchy(IEnumerable<NavigateItem> navigateItems) {
            nodesCache.Clear();
            ObservableCollection<NavigateTreeItem> list = new ObservableCollection<NavigateTreeItem>();
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
