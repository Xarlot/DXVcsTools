using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using DXVcsTools.Core;
using DXVcsTools.DXVcsClient;

namespace TortoiseProc {
    static class Program {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            try {
                Options options = Options.ParseOptions(args);
                string command = options.GetValue("command", null);
                switch (options.GetValue("command", "")) {
                    case "blame":
                        if (BlameRunner.Run(options, true)) return;
                        break;
                    case "vsblame":
                        if (BlameRunner.Run(options, false)) return;
                        break;
                    case "diff":
                        if (RunDiff(options)) return;
                        return;
                    case "log":
                        MessageBox.Show("Showing log is not implemented yet.");
                        return;
                }
                ShowUsage();
            }
            catch (Exception e) {
                MessageBox.Show(null, e.ToString(), null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static bool RunDiff(Options options) {
            string path = options.GetValue("path", null);

            if (string.IsNullOrEmpty(path))
                return false;

            string vcsFile, vcsService;
            ParsePath(path, out vcsFile, out vcsService);

            int leftVersion, rightVersion;
            if (!int.TryParse(options.GetValue("startrev", "1"), out leftVersion))
                return false;
            if (!int.TryParse(options.GetValue("endrev", "-1"), out rightVersion))
                return false;

            IDXVcsRepository dxRepository = DXVcsConnectionHelper.Connect(string.IsNullOrEmpty(vcsService) ? ConfigurationManager.AppSettings["DXVcsService"] : vcsService);

            string leftFile = null;
            string rightFile = null;
            try {
                try {
                    leftFile = GetFile(dxRepository, vcsFile, leftVersion);
                }
                catch (ArgumentException e) {
                    if (e.Message.Equals("version", StringComparison.InvariantCultureIgnoreCase)) {
                        MessageBox.Show(null, "Previous version doesn't exist.", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return true;
                    }
                    else
                        throw;
                }
                rightFile = GetFile(dxRepository, vcsFile, rightVersion);

                var startInfo = new ProcessStartInfo();
                startInfo.FileName = ConfigurationManager.AppSettings["DiffTool"];
                startInfo.Arguments = string.Format(ConfigurationManager.AppSettings["DiffToolArguments"], leftFile, rightFile, vcsFile + ":" + leftVersion.ToString(),
                    vcsFile + ":" + rightVersion.ToString());

                Process process = Process.Start(startInfo);
                process.WaitForExit();
            }
            finally {
                if (!string.IsNullOrEmpty(leftFile))
                    File.Delete(leftFile);
                if (!string.IsNullOrEmpty(rightFile))
                    File.Delete(rightFile);
            }
            return true;
        }

        static string GetFile(IDXVcsRepository dxRepository, string path, int version) {
            string fileName = Path.GetTempFileName();
            dxRepository.Get(path, fileName, version);
            return fileName;
        }

        public static void ParsePath(string path, out string fileName, out string fileSource) {
            string[] pathElements = path.Split('|');
            fileName = pathElements[0];
            if (pathElements.Length > 1)
                fileSource = pathElements[1];
            else
                fileSource = null;
        }

        static void ShowUsage() {
            MessageBox.Show("Usage");
        }
    }
}