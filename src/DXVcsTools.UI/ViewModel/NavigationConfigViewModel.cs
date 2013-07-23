using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using DevExpress.Xpf.Mvvm;
using DXVcsTools.UI.Navigator;

namespace DXVcsTools.UI {
    public class NavigationConfigViewModel : BindableBase {
        string content;
        string replacePath;
        IEnumerable<string> roots;

        public IEnumerable<string> Roots {
            get { return roots; }
            set { SetProperty(ref roots, value, () => Roots); }
        }
        public string Content {
            get { return content; }
            set { SetProperty(ref content, value, () => Content); }
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
        public void Save() {
            if (Content != null)
                SerializeHelper.SerializeNavigationConfig(Content);
        }
        void Generate() {
            NavigateItems = NavigateHelper.Scan(Roots);
            Content = SerializeHelper.SerializeNavigationConfigToString(this);
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
    }
}
