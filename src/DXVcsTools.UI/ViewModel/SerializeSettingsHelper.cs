using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using DevExpress.Mvvm.Native;
using DXVcsTools.Core;
using DXVcsTools.UI;
using Newtonsoft.Json;

namespace DXVcsTools {
    public static class SerializeHelper {
        static readonly string AppPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static readonly string SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\DXVcsTools\\";
        const string SettingsFile = "settings.txt";
        const string NavigationConfigFile = "navigationconfig.txt";
        static string SettingsFilePath {
            get { return SettingsPath + SettingsFile; }
        }
        public static string NavigationConfigFilePath { get { return SettingsPath + NavigationConfigFile; } }

        public static void SerializeNavigationConfig(string config) {
            SerializeNavigationConfigInternal(() => config);
        }
        public static void SerializeNavigationConfig(NavigationConfigViewModel config) {
            SerializeNavigationConfigInternal(() => JsonConvert.SerializeObject(config, Formatting.Indented));
        }
        static void SerializeNavigationConfigInternal(Func<string> getConfigStringHandler) {
            string path = NavigationConfigFilePath;
            if (File.Exists(path))
                StoreFile(path);
            EnsureDirectory(path);
            using (StreamWriter writer = File.CreateText(path)) {
                var json = getConfigStringHandler();
                writer.Write(json);
            }
        }
        public static NavigationConfigViewModel DeSerializeNavigationConfig() {
            string path = NavigationConfigFilePath;
            NavigationConfigViewModel model = null;
            if (File.Exists(path)) {
                using (StreamReader reader = File.OpenText(path)) {
                    string json = reader.ReadToEnd();
                    model = JsonConvert.DeserializeObject<NavigationConfigViewModel>(json, new JsonSerializerSettings());
                }
            }
            return model.Return(x => x, CreateDefaultNavigationConfig);
        }
        static NavigationConfigViewModel CreateDefaultNavigationConfig() {
            return new NavigationConfigViewModel() {Presets = GenerateDefaultPresets()};
        }
        static ObservableCollection<NavigatePreset> GenerateDefaultPresets() {
            var presets = new ObservableCollection<NavigatePreset>();
            presets.Add(new NavigatePreset() {
                Name = "General",
                Value = @"(Contains([DisplayText], '2014.2') Or Contains([DisplayText], '2013.2') Or Contains([DisplayText], '2014.1')) And Not Contains([DisplayText], 'Localization')"
            });
            presets.Add(new NavigatePreset() {
                Name = "Wpf",
                Value = @"(Contains([DisplayText], '2014.2') Or Contains([DisplayText], '2013.2') Or Contains([DisplayText], '2014.1')) And Not Contains([DisplayText], 'Localization')"
            });
            presets.Add(new NavigatePreset() {
                Name = "Win",
                Value = @"(Contains([DisplayText], '2014.2') Or Contains([DisplayText], '2013.2') Or Contains([DisplayText], '2014.1')) And Not Contains([DisplayText], 'Localization') And Contains([DisplayText], 'Win') Or Contains([DisplayText], 'RealLife')"
            });
            return presets;
        }
        public static void SerializeSettings(OptionsViewModel model) {
            string path = SettingsFilePath;
            if (File.Exists(path))
                StoreFile(path);
            EnsureDirectory(path);
            using (StreamWriter writer = File.CreateText(path)) {
                var json = JsonConvert.SerializeObject(model, Formatting.Indented);
                writer.Write(json);
            }
        }
        static void EnsureDirectory(string path) {
            string dirName = Path.GetDirectoryName(path);
            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);
        }
        public static OptionsViewModel DeSerializeSettings() {
            string path = SettingsFilePath;

            if (File.Exists(path)) {
                using (StreamReader reader = File.OpenText(path)) {
                    string json = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<OptionsViewModel>(json, new JsonSerializerSettings());
                }
            }
            return CreateDefault();
        }
        static OptionsViewModel CreateDefault() {
            var options = new OptionsViewModel();
            options.Branches = new List<DXVcsBranch>();
            if (Debugger.IsAttached) {
                options.Branches.Add(new DXVcsBranch { Name = "test13.1", Path = "$/Sandbox/litvinov/13.1/".ToLower() });
                options.Branches.Add(new DXVcsBranch { Name = "test13.2", Path = "$/Sandbox/litvinov/13.2/".ToLower() });
            }
            options.Branches.Add(new DXVcsBranch { Name = "13.2", Path = "$/2013.2/" });
            options.Branches.Add(new DXVcsBranch { Name = "14.1", Path = "$/2014.1/" });
            options.Branches.Add(new DXVcsBranch { Name = "14.2", Path = "$/2014.2/" });
            options.LightThemeName = "Seven";
            options.DarkThemeName = "MetropolisDark";
            options.BlueThemeName = "VS2010";
            options.UseNavigateMenu = true;
            options.UpdateNavigateMenuAsync = true;
            options.LayoutOptions = new LayoutOptionsViewModel();
            options.TortoiseProc = @"Lib/TortoiseSvn/bin/TortoiseBlame.exe";

            options.DiffTool = @"C:\Program Files (x86)\WinMerge\WinMergeU.exe";
            return options;
        }
        static void StoreFile(string path) {
            string fileName = Path.GetFileName(path);
            string bakPath = SettingsPath + Path.GetFileNameWithoutExtension(fileName) + ".bak";
            if (File.Exists(bakPath))
                File.Delete(bakPath);

            File.Move(path, bakPath);
        }
        public static string DeSerializeNavigationConfigToString() {
            string path = NavigationConfigFilePath;

            if (File.Exists(path)) {
                using (StreamReader reader = File.OpenText(path)) {
                    string json = reader.ReadToEnd();
                    return json;
                }
            }
            return string.Empty;
        }
        public static string SerializeNavigationConfigToString(NavigationConfigViewModel model) {
            return JsonConvert.SerializeObject(model, Formatting.Indented);
        }
        public static string ResolveSettingsPath(string path) {
            return ResolvePath(SettingsPath, path);
        }
        public static string ResolveAppPath(string path) {
            return ResolvePath(AppPath, path);
        }
        static string ResolvePath(string basePath, string path) {
            if (!Path.IsPathRooted(basePath))
                throw new ArgumentException("basePath must be rooted");

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (Path.IsPathRooted(path))
                return path;

            return Path.Combine(basePath, path);
        }

    }
}
