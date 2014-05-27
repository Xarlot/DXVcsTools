using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DXVcsTools.Core;
using DXVcsTools.Data;
using DXVcsTools.DXVcsClient;
using DXVcsTools.UI.View;
using DXVcsTools.UI.ViewModel;

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
                PrepareBlameFileAndShowBlameUI(filePath, lineNumber);
            }
            catch (Exception e) {
                Logger.Logger.AddError("ShowExternalBlame. Failed.", e);
            }
            Logger.Logger.AddInfo("ShowExternalBlame. End.");
        }
        //void ShowExternalBlameInternal(Uri svnFile, int? lineNumber) {
        //    if (string.IsNullOrEmpty(svnFile.ToString()))
        //        throw new ArgumentException("svnFile");

        //    var startInfo = new ProcessStartInfo();
        //    startInfo.FileName = GetTortoiseProcPath();
        //    startInfo.Arguments = string.Format("/command:blame /path:{0} /startrev:0 /endrev:-1", svnFile.AbsoluteUri);

        //    if (lineNumber.HasValue)
        //        startInfo.Arguments += string.Format(" /line:{0}", lineNumber);

        //    using (Process process = Process.Start(startInfo)) {
        //        process.WaitForExit();
        //    }
        //}
        void PrepareBlameFileAndShowBlameUI(string filePath, int? lineNumber) {
            string blameFile = null;
            string logFile = null;
            try {
                IDXVcsRepository dxRepository = DXVcsConnectionHelper.Connect(portOptions.VcsServer);
                MergeHelper helper = new MergeHelper(options, portOptions);
                string vcsFile = helper.GetMergeVcsPathByOriginalPath(filePath, portOptions.MasterBranch);

                FileDiffInfo diffInfo = dxRepository.GetFileDiffInfo(vcsFile);
                int revision = diffInfo.LastRevision;
                IList<IBlameLine> blame = diffInfo.BlameAtRevision(revision);
                blameFile = MakeBlameFile(vcsFile, blame);
                logFile = MakeLog(blame);
                ShowExternalBlameInternal(filePath, vcsFile, blameFile, logFile, revision, lineNumber);
            }
            finally {
                blameFile.Do(File.Delete);
                logFile.Do(File.Delete);
            }
        }
        void ShowExternalBlameInternal(string filePath, string vcsFile, string blameFile, string logFile, int revision, int? lineNumber) {
            string path = filePath + "|" + vcsFile;
            string blameArgs = string.Format("\"{0}\" \"{1}\" \"{2}\" /path:\"{3}\" /revrange:\"{4}\"", blameFile, logFile, Path.GetFileName(filePath), path, revision.ToString());
            if (lineNumber != null)
                blameArgs += " /line:" + lineNumber.ToString();
            blameArgs += " /ignoreallspaces";
            ShowBlameUI(blameArgs);
        }
        void ShowBlameUI(string blameArgs) {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = GetTortoiseProcPath();
            startInfo.Arguments = blameArgs;

            using (Process process = Process.Start(startInfo)) {
                process.WaitForExit();
            }
        }
        static string MakeBlameFile(string vcsFile, IEnumerable<IBlameLine> blame) {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("  {0,-6} {1,-6} {1,-6} {2,-30} {3,-60} {4, -30} {5} ", "line", "rev", "date", "path", "author", "content");
            sb.AppendLine();
            sb.AppendLine();

            int i = 0;
            foreach (var line in blame) {
                sb.AppendFormat("{0,8} {1,6} {1,6} {2,-30:dd/MM/yyyy hh:mm:ss} {3,-60} {4, -30} {5} ",
                    i++, line.Revision, line.CommitDate, "", line.User, line.SourceLine);
                sb.AppendLine();
            }
            string fileName = Path.GetTempFileName() + Path.GetExtension(vcsFile);
            File.WriteAllText(fileName, sb.ToString());
            return fileName;
        }
        static string MakeLog(IEnumerable<IBlameLine> blame) {
            SortedList<int, string> comments = new SortedList<int, string>();

            foreach (var line in blame)
                comments[line.Revision] = line.Comment;

            BinaryWriter bw = new BinaryWriter(new MemoryStream());
            for (int i = comments.Keys.Count - 1; i >= 0; i--) {
                bw.Write(comments.Keys[i]);
                byte[] comment = Encoding.Default.GetBytes(comments.Values[i]);
                bw.Write(comment.Length);
                bw.Write(comment);
            }
            string fileName = Path.GetTempFileName();
            File.WriteAllBytes(fileName, ((MemoryStream)bw.BaseStream).ToArray());
            return fileName;
        }


        string GetTortoiseProcPath() {
            string path = SerializeHelper.ResolveAppPath(options.TortoiseProc);
            if (!File.Exists(path))
                throw new ArgumentException("tortoise not found");
            return path;
        }
        public void ShowInternalBlame(string path, int? lineNumber, Action<InternalBlameViewModel> attachBlameControlHandler) {
            Logger.Logger.AddInfo("ShowInternalBlame. Start.");
            try {
                ShowInternalBlameInternal(path, lineNumber, attachBlameControlHandler);
            }
            catch (Exception e) {
                Logger.Logger.AddError("ShowInternalBlame. Failed.", e);
            }
            Logger.Logger.AddInfo("ShowInternalBlame. End.");
        }
        void ShowInternalBlameInternal(string filePath, int? lineNumber, Action<InternalBlameViewModel> attachBlameControlHandler) {
            InternalBlameViewModel model = new InternalBlameViewModel(filePath, lineNumber, this);
            attachBlameControlHandler(model);
        }
        public int GetLastRevision(string filePath, int? lineNumber) {
            IDXVcsRepository dxRepository = DXVcsConnectionHelper.Connect(portOptions.VcsServer);
            MergeHelper helper = new MergeHelper(options, portOptions);
            string vcsFile = helper.GetMergeVcsPathByOriginalPath(filePath, portOptions.MasterBranch);

            FileDiffInfo diffInfo = dxRepository.GetFileDiffInfo(vcsFile);
            return diffInfo.LastRevision;
        }
        public IEnumerable<IBlameLine> BlameAtRevision(string filePath, int? lineNumber, int revision) {
            IDXVcsRepository dxRepository = DXVcsConnectionHelper.Connect(portOptions.VcsServer);
            MergeHelper helper = new MergeHelper(options, portOptions);
            string vcsFile = helper.GetMergeVcsPathByOriginalPath(filePath, portOptions.MasterBranch);

            FileDiffInfo diffInfo = dxRepository.GetFileDiffInfo(vcsFile);
            return diffInfo.BlameAtRevision(revision);
        }
    }
}
