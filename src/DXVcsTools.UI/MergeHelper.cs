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
        OptionsViewModel ViewModel { get; set; }
        PortWindowModel Model { get; set; }
        public MergeHelper(OptionsViewModel options, PortWindowModel model) {
            ViewModel = options;
            Model = model;
        }

            //        view.Title = "DXPort v" + VersionInfo.FullVersion;

            //view.SourceFile = model.SourceFile;
            //view.OriginalFile = model.OriginalVcsFile;
            //view.TargetFile = model.TargetVcsFile;

            //FillBranchSelector();

            //view.ReviewTarget = model.ReviewTarget;
            //view.CheckInTarget = model.CheckInTarget;

        public void MergeChanges() {
            try {
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Model.VcsServer);

                string tmpOriginalFile = Path.GetTempFileName();
                try {
                    repository.GetLatestVersion(Model.OriginalVcsFile, tmpOriginalFile);

                    string tmpTargetFile = repository.GetFileWorkingPath(Model.TargetVcsFile);
                    if (string.IsNullOrEmpty(tmpTargetFile))
                        throw new ApplicationException("Can't proceed because target file doesn't have working directory configured.");

                    repository.CheckOutFile(Model.TargetVcsFile, tmpTargetFile, string.Empty);

                    var diff = new FileDiff();
                    if (!diff.Merge(tmpOriginalFile, Model.SourceFile, tmpTargetFile)) {
                        throw new ApplicationException("Automatic merge failed, please port changes manually");
                    }

                    if (ViewModel.ReviewTarget) {
                        ReviewTargetFile(repository, Model.TargetVcsFile, tmpTargetFile);
                    }

                    //if (view.CheckInTarget) {
                    //    if (view.ReviewTarget) {
                    //        if (!view.ShowQuestion(view.Title, "Check in file " + tmpTargetFile + "?"))
                    //            return;
                    //    }

                    //    repository.CheckInFile(view.TargetFile, tmpTargetFile, view.CheckInComment);
                    //}

                    //if (!view.ReviewTarget)
                    //    view.ShowInfo(view.Title, "Merge finished successfully.");
                }
                finally {
                    File.Delete(tmpOriginalFile);
                }
            }
            catch (Exception exception) {
//                view.ShowError(null, exception.Message);
            }
        }
        void ReviewTargetFile(IDXVcsRepository repository, string vcsFile, string file) {
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
            startInfo.FileName = Model.DiffTool;
            startInfo.Arguments = string.Format("\"{0}\" \"{1}\"", leftFile, rightFile);

            Process process = Process.Start(startInfo);
            process.WaitForExit();
        }

    }
}
