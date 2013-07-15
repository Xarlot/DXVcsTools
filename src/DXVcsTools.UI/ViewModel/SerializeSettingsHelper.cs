using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DXVcsTools.Core;
using DXVcsTools.UI;
using Newtonsoft.Json;

namespace DXVcsTools {
    public static class SerializeSettingsHelper {
        static readonly string SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\DXVcsTools\\";
        const string SettingsFile = "settings.txt";
        static string SettingsFilePath { get { return SettingsPath + SettingsFile; } }

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
            options.Branches.Add(new DXVcsBranch { Name = "12.1", Path = "$/2012.1/" });
            options.Branches.Add(new DXVcsBranch { Name = "12.2", Path = "$/2012.2/" });
            options.Branches.Add(new DXVcsBranch { Name = "13.1", Path = "$/2013.1/" });
            options.Branches.Add(new DXVcsBranch { Name = "13.2", Path = "$/2013.2/" });
            options.LightThemeName = "Seven";
            options.DarkThemeName = "MetropolisDark";
            options.BlueThemeName = "VS2010";

            options.DiffTool = @"C:\Program Files (x86)\WinMerge\WinMergeU.exe";
            return options;
        }
        static void StoreFile(string path) {
            string fileName = Path.GetFileName(path);
            File.Move(fileName, Path.GetFileNameWithoutExtension(fileName) + ".bak");
        }
    }
}
