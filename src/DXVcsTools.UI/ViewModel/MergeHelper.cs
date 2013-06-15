using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXVcsTools.DXVcsClient;
using DXVcsTools.UI;

namespace DXVcsTools.Core {
    public class MergeHelper {
        OptionsViewModel Options { get; set; }
        PortOptionsViewModel Port { get; set; }
        public MergeHelper(OptionsViewModel options, PortOptionsViewModel model) {
            Options = options;
            Port = model;
        }
        public MergeState MergeChanges(DXVcsBranch currentBranch, string filePath, string mergePath, bool showPreview) {
            try {
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
                string tmpOriginalFile = Path.GetTempFileName();

                string vcsOriginalPath = Port.GetRelativePath(filePath);
                string vcsTargetFile = mergePath == null ? GetMergeVcsPath(filePath, currentBranch) : Port.GetRelativePath(mergePath, currentBranch.Path);

                try {
                    repository.GetLatestVersion(vcsOriginalPath, tmpOriginalFile);

                    string tmpTargetFile = repository.GetFileWorkingPath(vcsTargetFile);
                    if (string.IsNullOrEmpty(tmpTargetFile))
                        return MergeState.TargetDirectoryError;
                    repository.CheckOutFile(vcsTargetFile, tmpTargetFile, string.Empty);


                    var diff = new FileDiff();
                    if (!diff.Merge(tmpOriginalFile, filePath, tmpTargetFile)) {
                        return MergeState.Conflict;
                    }
                    if (showPreview)
                        PreviewTarget(repository, vcsTargetFile, tmpTargetFile);
                }
                finally {
                    File.Delete(tmpOriginalFile);
                }
            }
            catch {
                return MergeState.UnknownError;
            }
            return MergeState.Success;
            //            try {
            //                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);

            //                string tmpOriginalFile = Path.GetTempFileName();
            //                try {
            //                    repository.GetLatestVersion(Port.OriginalVcsFile, tmpOriginalFile);

            //                    string tmpTargetFile = repository.GetFileWorkingPath(Port.TargetVcsFile);
            //                    if (string.IsNullOrEmpty(tmpTargetFile))
            //                        throw new ApplicationException("Can't proceed because target file doesn't have working directory configured.");

            //                    repository.CheckOutFile(Port.TargetVcsFile, tmpTargetFile, string.Empty);

            //                    var diff = new FileDiff();
            //                    if (!diff.Merge(tmpOriginalFile, Port.SourceFile, tmpTargetFile)) {
            //                        throw new ApplicationException("Automatic merge failed, please port changes manually");
            //                    }

            //                    if (Options.ReviewTarget) {
            //                        ReviewTargetFile(repository, Port.TargetVcsFile, tmpTargetFile);
            //                    }

            //                    //if (view.CheckInTarget) {
            //                    //    if (view.ReviewTarget) {
            //                    //        if (!view.ShowQuestion(view.Title, "Check in file " + tmpTargetFile + "?"))
            //                    //            return;
            //                    //    }

            //                    //    repository.CheckInFile(view.TargetFile, tmpTargetFile, view.CheckInComment);
            //                    //}

            //                    //if (!view.ReviewTarget)
            //                    //    view.ShowInfo(view.Title, "Merge finished successfully.");
            //                }
            //                finally {
            //                    File.Delete(tmpOriginalFile);
            //                }
            //            }
            //            catch (Exception exception) {
            ////                view.ShowError(null, exception.Message);
            //            }
        }
        void PreviewTarget(IDXVcsRepository repository, string vcsFile, string file) {
            string tmpVcsFile = Path.GetTempFileName();

            try {
                repository.GetLatestVersion(vcsFile, tmpVcsFile);
                LaunchDiffTool(tmpVcsFile, file);
            }
            finally {
                File.Delete(tmpVcsFile);
            }
        }
        void LaunchDiffTool(string leftFile, string rightFile) {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = Options.DiffTool;
            startInfo.Arguments = string.Format("\"{0}\" \"{1}\"", leftFile, rightFile);

            Process process = Process.Start(startInfo);
            process.WaitForExit();

        }
        string GetMergeVcsPath(string filePath, DXVcsBranch currentBranch) {
            string relativePath = Port.GetRelativePath(filePath);
            return relativePath.Replace(Port.MasterBranch.Path, currentBranch.Path);
        }

        public void CheckIn(CheckInViewModel checkInViewModel) {
            IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
            string vcsOriginalPath = Port.GetRelativePath(checkInViewModel.FilePath);
            repository.CheckInFile(vcsOriginalPath, checkInViewModel.FilePath, checkInViewModel.Comment);
            if (checkInViewModel.StaysChecked)
                repository.CheckOutFile(vcsOriginalPath, checkInViewModel.FilePath, checkInViewModel.Comment);
        }
        public void ShowDiff(string filePath) {
            IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
            string vcsOriginalPath = Port.GetRelativePath(filePath);
            PreviewTarget(repository, vcsOriginalPath, filePath);
        }
    }
}
