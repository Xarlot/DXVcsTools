using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using DevExpress.Xpf.Core;
using DXVcsTools.DXVcsClient;
using DXVcsTools.UI;
using DXVcsTools.UI.Logger;

namespace DXVcsTools.Core {
    public class MergeHelper {
        public MergeHelper(OptionsViewModel options, PortOptionsViewModel model) {
            Options = options;
            Port = model;
        }
        OptionsViewModel Options { get; set; }
        PortOptionsViewModel Port { get; set; }
        public MergeState MergeChanges(DXVcsBranch currentBranch, string filePath, string mergePath, bool showPreview) {
            try {
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
                string tmpOriginalFile = Path.GetTempFileName();

                string vcsOriginalPath = Port.GetRelativePath(filePath);
                string vcsTargetFile = mergePath == null ? GetMergeVcsPathByOriginalPath(filePath, currentBranch) : Port.GetRelativePath(mergePath, currentBranch);

                try {
                    repository.GetLatestVersion(vcsOriginalPath, tmpOriginalFile);
                    string tmpTargetFile = string.Empty;
                    try {
                        tmpTargetFile = repository.GetFileWorkingPath(vcsTargetFile);
                    }
                    catch (Exception e) {
                        Logger.AddError("MergeCommand. Target file error.", e);
                        return MergeState.TargetFileError;
                    }
                    if (string.IsNullOrEmpty(tmpTargetFile)) {
                        Logger.AddError("MergeCommand. Target file path is empty.");
                        return MergeState.TargetFileError;
                    }

                    try {
                        repository.CheckOutFile(vcsTargetFile, tmpTargetFile, string.Empty);
                    }
                    catch (Exception e) {
                        Logger.AddError("MergeCommand. Check out file error.", e);
                        return MergeState.CheckOutFileError;
                    }


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
        string GetMergeVcsPathByOriginalPath(string filePath, DXVcsBranch currentBranch) {
            string relativePath = Port.GetRelativePath(filePath);
            string result = relativePath.Replace(Port.MasterBranch.Path, currentBranch.Path);
            string ext = Path.GetExtension(filePath);
            if (ext == ".sln" || ext == ".csproj")
                return result + Path.GetFileName(filePath);
            return result;
        }
        string GetMergeVcsPathByTargetPath(string filePath, DXVcsBranch currentBranch) {
            return Port.GetRelativePath(filePath, currentBranch);
        }
        public bool CheckIn(CheckInViewModel checkInViewModel, DXVcsBranch targetBranch) {
            Logger.AddInfo("CheckInCommand. Perform checkin file: " + checkInViewModel.FilePath);
            try {
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
                string filePath = GetFilePathForBranch(checkInViewModel.FilePath, targetBranch);
                string vcsOriginalPath = GetMergeVcsPathByTargetPath(filePath, targetBranch);

                repository.CheckInFile(vcsOriginalPath, filePath, checkInViewModel.Comment);
                if (checkInViewModel.StaysChecked)
                    repository.CheckOutFile(vcsOriginalPath, filePath, checkInViewModel.Comment);
            }
            catch(Exception e) {
                Logger.AddError("CheckInCommand. CheckIn failed.", e);
                return false;
            }
            return true;
        }
        public void CompareWithCurrentVersion(string filePath) {
            IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
            string vcsOriginalPath = Port.GetRelativePath(filePath);
            PreviewTarget(repository, vcsOriginalPath, filePath);
        }
        public void CompareWithPortVersion(string filePath, DXVcsBranch current) {
            try {
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
                string vcsTargetPath = GetMergeVcsPathByOriginalPath(filePath, current);
                PreviewTarget(repository, vcsTargetPath, repository.GetFileWorkingPath(vcsTargetPath));
            }
            catch (Exception e) {
                DXMessageBox.Show(e.Message);
            }
        }
        public MergeState ManualMerge(DXVcsBranch currentBranch, ManualMergeViewModel mergeModel, Func<bool> showManualMergeUIHandler) {
            try {
                string filePath = mergeModel.OriginalFilePath;
                string mergePath = mergeModel.TargetFilePath;
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);

                string vcsTargetFile = mergePath == null ? GetMergeVcsPathByOriginalPath(filePath, currentBranch) : GetMergeVcsPathByTargetPath(mergePath, currentBranch);
                string tmpTargetFile = repository.GetFileWorkingPath(vcsTargetFile);
                mergeModel.TargetFilePath = tmpTargetFile;

                if (!showManualMergeUIHandler()) {
                    Logger.AddInfo("ManualMergeCommand. Result = MergeState.None.");
                    return MergeState.None;
                }

                vcsTargetFile = GetMergeVcsPathByTargetPath(mergeModel.TargetFilePath, currentBranch);
                string tmpOriginalFile = mergeModel.OriginalFilePath;
                tmpTargetFile = repository.GetFileWorkingPath(vcsTargetFile);
                if (string.IsNullOrEmpty(tmpTargetFile)) {
                    Logger.AddInfo("ManualMergeCommand. Result = MergeState.TargetFileError.");
                    return MergeState.TargetFileError;
                }

                repository.CheckOutFile(vcsTargetFile, tmpTargetFile, string.Empty);
                LaunchDiffTool(tmpOriginalFile, tmpTargetFile);
            }
            catch(Exception e) {
                Logger.AddError("ManualMergeCommand. Unknown error.", e);
                Logger.AddInfo("ManualMergeCommand. Result = MergeState.UnknownError.");
                return MergeState.UnknownError;
            }
            Logger.AddInfo("ManualMergeCommand. Result = MergeState.Success");
            return MergeState.Success;
        }
        public void NavigateToSolution(DXVcsBranch currentBranch, IDteWrapper dte) {
            try {
                string filePath = Port.ProjectFilePath;
                string vcsFilePath = GetMergeVcsPathByOriginalPath(filePath, currentBranch) + Path.GetFileName(filePath);
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
                string targetPath = repository.GetFileWorkingPath(vcsFilePath);
                dte.OpenSolution(targetPath);
            }
            catch {
                MessageBox.Show("Can`t navigate to solution");
            }
        }
        public void NavigateToSolution(string path, IDteWrapper dte) {
            Logger.AddInfo("NavigateToSolutionCommand. Start Navigate to: " + path);
            try {
                dte.OpenSolution(path);
            }
            catch(Exception e) {
                Logger.AddError("Can`t navigate to solution", e);
                MessageBox.Show("Can`t navigate to solution");
            }
            Logger.AddInfo("NavigateToSolutionCommand. End navigate");
        }
        public bool UndoCheckout(string filePath) {
            string vcsFilePath = Port.GetRelativePath(filePath);
            try {
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
                repository.UndoCheckout(vcsFilePath, filePath);
                repository.GetLatestVersion(vcsFilePath, filePath);
                FileInfo info = new FileInfo(filePath);
                info.IsReadOnly = true;
            }
            catch {
                return true;
            }
            return false;
        }
        public string GetFilePathForBranch(string path, DXVcsBranch currentBranch) {
            try {
                string relativePath = GetMergeVcsPathByOriginalPath(path, currentBranch);
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
                return repository.GetFileWorkingPath(relativePath);
            }
            catch(Exception e) {
                Logger.AddError("GetFilePathForBranch failed.", e);
            }
            return string.Empty;
        }
    }
}