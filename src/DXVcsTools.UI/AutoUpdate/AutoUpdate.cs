using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using DXVcsTools.Version;
using Newtonsoft.Json;
using MessageBox = System.Windows.MessageBox;

namespace DXVcsTools.UI.AutoUpdate {
    public static class AutoUpdateHelper {
        const string FileName = "DXVcsTools.VSIX.vsix";
        const string updateFileName = "update.ini";
        public static bool Publish(AutoUpdateOptions update, string source, string target) {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
                return false;
            string sourceFile = Path.Combine(source, FileName);
            if (!File.Exists(sourceFile))
                return false;

            string targetFile = Path.Combine(target, update.Path, FileName);
            string updateFile = Path.Combine(target, updateFileName);

            var result = JsonConvert.SerializeObject(update);
            try {
                var bytes = Encoding.UTF8.GetBytes(result);
                File.WriteAllBytes(updateFile, bytes);

                string dir = Path.GetDirectoryName(targetFile);
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }
                if (File.Exists(targetFile))
                    File.Delete(targetFile);
                File.Copy(sourceFile, targetFile);
            }
            catch {
                return false;
            }
            return true;
        }
        public static AutoUpdateOptions GetUpdateOptions(string path) {
            try {
                WebClient client = new WebClient();
                var data = client.DownloadData(Path.Combine(path, updateFileName));
                return JsonConvert.DeserializeObject<AutoUpdateOptions>(Encoding.UTF8.GetString(data));
            }
            catch {
            }
            return new AutoUpdateOptions();
        }
        public static AutoUpdateOptions CreateAutoUpdateOptions() {
            return new AutoUpdateOptions() { Version = VersionInfo.ToIntVersion() };
        }
        public static void Update(AutoUpdateOptions updateOptions, string path) {
            ProcessStartInfo info = new ProcessStartInfo("vsixinstaller.exe");
            info.Arguments = "/q " + Path.Combine(path, updateOptions.Path, FileName);
            info.UseShellExecute = false;
            try {
                var process = Process.Start(info);
                process.WaitForExit();
            }
            catch {
                MessageBox.Show("Update failed!");
                return;
            }
            MessageBox.Show("Update successful. I recommend to restart Visual Studio.");
        }
    }
}
