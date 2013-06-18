using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using DXVcsTools.DXVcsClient;
using DXVcsTools.UI;

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

                    string tmpTargetFile = repository.GetFileWorkingPath(vcsTargetFile);
                    if (string.IsNullOrEmpty(tmpTargetFile))
                        return MergeState.TargetFileError;
                    try {
                        repository.CheckOutFile(vcsTargetFile, tmpTargetFile, string.Empty);
                    }
                    catch {
                        return MergeState.TargetFileError;
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
            return relativePath.Replace(Port.MasterBranch.Path, currentBranch.Path);
        }
        string GetMergeVcsPathByTargetPath(string filePath, DXVcsBranch currentBranch) {
            return Port.GetRelativePath(filePath, currentBranch);
        }
        public void CheckIn(CheckInViewModel checkInViewModel) {
            IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
            string vcsOriginalPath = Port.GetRelativePath(checkInViewModel.FilePath);
            repository.CheckInFile(vcsOriginalPath, checkInViewModel.FilePath, checkInViewModel.Comment);
            if (checkInViewModel.StaysChecked)
                repository.CheckOutFile(vcsOriginalPath, checkInViewModel.FilePath, checkInViewModel.Comment);
        }
        public void CompareWithCurrentVersion(string filePath) {
            IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
            string vcsOriginalPath = Port.GetRelativePath(filePath);
            PreviewTarget(repository, vcsOriginalPath, filePath);
        }
        public void CompareWithPortVersion(string filePath, DXVcsBranch current) {
            IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
            string vcsTargetPath = GetMergeVcsPathByOriginalPath(filePath, current);
            PreviewTarget(repository, vcsTargetPath, repository.GetFileWorkingPath(vcsTargetPath));
        }
        public MergeState ManualMerge(DXVcsBranch currentBranch, ManualMergeViewModel mergeModel, Func<bool> showManualMergeUIHandler) {
            try {
                string filePath = mergeModel.OriginalFilePath;
                string mergePath = mergeModel.TargetFilePath;
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);

                string vcsTargetFile = mergePath == null ? GetMergeVcsPathByOriginalPath(filePath, currentBranch) : GetMergeVcsPathByTargetPath(mergePath, currentBranch);
                string tmpTargetFile = repository.GetFileWorkingPath(vcsTargetFile);
                mergeModel.TargetFilePath = tmpTargetFile;

                if (!showManualMergeUIHandler())
                    return MergeState.None;

                vcsTargetFile = GetMergeVcsPathByTargetPath(mergeModel.TargetFilePath, currentBranch);
                string tmpOriginalFile = mergeModel.OriginalFilePath;
                tmpTargetFile = repository.GetFileWorkingPath(vcsTargetFile);
                if (string.IsNullOrEmpty(tmpTargetFile))
                    return MergeState.TargetFileError;

                repository.CheckOutFile(vcsTargetFile, tmpTargetFile, string.Empty);
                LaunchDiffTool(tmpOriginalFile, tmpTargetFile);
            }
            catch {
                return MergeState.UnknownError;
            }
            return MergeState.Success;
        }
        public void NavigateToSolution(DXVcsBranch currentBranch, IDteWrapper dte) {
            try {
                string filePath = Port.ProjectPath;
                string vcsFilePath = GetMergeVcsPathByOriginalPath(filePath, currentBranch) + Path.GetFileName(filePath);
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
                string targetPath = repository.GetFileWorkingPath(vcsFilePath);
                dte.OpenSolution(targetPath);
            }
            catch {
                MessageBox.Show("Can`t navigate to solution");
            }
        }
    }
}