using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DevExpress.Data.Linq;
using DevExpress.Xpf.Mvvm;
using DXVcsTools.UI.Navigator;

namespace DXVcsTools.UI {
    public class NavigationConfigViewModel : BindableBase {
        string rootPath;
        string content;
        string replacePath;

        public string RootPath {
            get { return rootPath; }
            set { SetProperty(ref rootPath, value, () => RootPath); }
        }
        public string ReplacePath {
            get { return replacePath; }
            set { SetProperty(ref replacePath, value, () => ReplacePath); }
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
        public bool ShouldSerializeSaveCommand() {
            return false;
        }
        public bool ShouldSerializeOpenConfigLocationCommand() {
            return false;
        }
        public IEnumerable<NavigateItem> NavigateItems { get; set; }
        public ICommand GenerateCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand OpenConfigLocationCommand { get; private set; }

        public NavigationConfigViewModel() {
            GenerateCommand = new DelegateCommand(Generate, CanGenerate);
            SaveCommand = new DelegateCommand(Save, CanSave);
            OpenConfigLocationCommand = new DelegateCommand(OpenConfigLocation);
        }
        void OpenConfigLocation() {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = SerializeHelper.NavigationConfigFilePath;
            Process.Start(startInfo);
        }
        bool CanSave() {
            return NavigateItems != null;
        }
        void Save() {
            SerializeHelper.SerializeNavigationConfig(Content);
        }
        bool CanGenerate() {
            return !string.IsNullOrEmpty(RootPath) && Directory.Exists(RootPath) && !string.IsNullOrEmpty(ReplacePath) && ReplacePath.Contains("$");
        }
        void Generate() {
            NavigateItems = NavigateHelper.Scan(RootPath, ReplacePath);
            SerializeHelper.SerializeNavigationConfig(this);
            Content = SerializeHelper.DeSerializeNavigationConfigToString();
            CommandManager.InvalidateRequerySuggested();
        }
        public string GetRelativePath(NavigateItem item) {
            string path = item.Path;
            return path.Replace(RootPath, ReplacePath);
        }
    }
}
