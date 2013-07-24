using System;
using System.Diagnostics;
using System.IO;
using DXVcsTools.Core;

namespace DXVcsTools.UI {
    public class BlameHelper {
        readonly OptionsViewModel options;
        readonly PortOptionsViewModel portOptions;
        public BlameHelper(OptionsViewModel options, PortOptionsViewModel portOptions) {
            this.options = options;
            this.portOptions = portOptions;
        }
        public void ShowExternalBlame(string filePath, int? lineNumber = null) {
            Logger.Logger.AddInfo("ShowExternalBlame. Start.");
            try {
                HistoryImporter importer = CreateHistoryImporter();
                MergeHelper helper = new MergeHelper(options, portOptions);
                string vcsFile = helper.GetMergeVcsPathByOriginalPath(filePath, portOptions.MasterBranch);
                Uri importedFile = importer.ImportFileDirect(portOptions.VcsServer, vcsFile);
                ShowExternalBlameInternal(importedFile, lineNumber);
            }
            catch (Exception e) {
                Logger.Logger.AddError("ShowExternalBlame. Failed.", e);
            }
            Logger.Logger.AddInfo("ShowExternalBlame. End.");
        }
        void ShowExternalBlameInternal(Uri svnFile, int? lineNumber) {
            if (string.IsNullOrEmpty(svnFile.ToString()))
                throw new ArgumentException("svnFile");

            var startInfo = new ProcessStartInfo();
            startInfo.FileName = GetTortoiseProcPath();
            startInfo.Arguments = string.Format("/command:blame /path:{0} /startrev:0 /endrev:-1", svnFile.AbsoluteUri);

            if (lineNumber.HasValue)
                startInfo.Arguments += string.Format(" /line:{0}", lineNumber);

            using (Process process = Process.Start(startInfo)) {
                process.WaitForExit();
            }
        }
        string GetTortoiseProcPath() {
            string path = SerializeHelper.ResolveAppPath(options.TortoiseProc);
            if (!File.Exists(path))
                throw new ArgumentException("tortoise not found");
            return path;
        }
        HistoryImporter CreateHistoryImporter() {
            return new HistoryImporter(options.SvnRepository, options.TempSvnDirectory);
        }

    }
}
