using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
        public MergeState MergeChanges(DXVcsBranch currentBranch, string filePath, string mergePath, bool showPreview, bool isNew) {
            try {
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
                string tmpOriginalFile = Path.GetTempFileName();

                string vcsOriginalPath = Port.GetRelativePath(filePath);
                string vcsTargetFile = mergePath == null ? GetMergeVcsPathByOriginalPath(filePath, currentBranch) : Port.GetRelativePath(mergePath, currentBranch);

                try {
                    if (!isNew)
                        repository.GetLatestVersion(vcsOriginalPath, tmpOriginalFile);
                    else
                        CreateNewFileIfNeeded(repository, vcsTargetFile, currentBranch);
                    string tmpTargetFile;
                    try {
                        tmpTargetFile = isNew 
                            ? GetFilePathForNewFile(repository, GetMergeVcsPathByOriginalPath(filePath, currentBranch), currentBranch) 
                            : repository.GetFileWorkingPath(vcsTargetFile);
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
                        if (!isNew)
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
                        PreviewTarget(repository, filePath, vcsTargetFile, tmpTargetFile, isNew);
                }
                catch (Exception e) {
                    Logger.AddError("MergeCommand. Unknown error.", e);
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
        void CreateNewFileIfNeeded(IDXVcsRepository repository, string vcsTargetFile, DXVcsBranch branch) {
            if (repository.IsUnderVss(vcsTargetFile))
                throw new ArgumentException("File is under vss alrealy!");
            string filePath = GetFilePathForNewFile(repository, vcsTargetFile, branch);
            if (File.Exists(filePath)) {
                var attr = File.GetAttributes(filePath);
                if (attr == FileAttributes.ReadOnly)
                    throw new ArgumentException("File is readonly!");
                File.Delete(filePath);
            }
            string dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir)) 
                Directory.CreateDirectory(dir);
            using (var f = File.Create(filePath)) {
                f.Close();
            }
        }
        void PreviewTarget(IDXVcsRepository repository, string file, string vcsFile, int leftVersion = 0, int rightVersion = 0, bool compareWithCurrent = true) {
            string leftFile = Path.GetTempFileName();
            string rightFile = Path.GetTempFileName();

            try {
                if (leftVersion > 0) 
                    repository.Get(vcsFile, leftFile, leftVersion);
                else
                    repository.GetLatestVersion(vcsFile, leftFile);

                if (compareWithCurrent)
                    CopyFileContent(file, rightFile);
                else if (rightVersion > 0)
                    repository.Get(vcsFile, rightFile, rightVersion);
                else
                    repository.GetLatestVersion(vcsFile, rightFile);

                LaunchDiffTool(leftFile, rightFile);
            }
            finally {
                File.Delete(leftFile);
                File.Delete(rightFile);
            }
        }
        void PreviewTarget(IDXVcsRepository repository, string file, string vcsFile, string targetFile, bool isNew) {
            string tmpFile = Path.GetTempFileName();

            try {
                if (!isNew)
                    repository.GetLatestVersion(vcsFile, tmpFile);
                else
                    CopyFileContent(file, tmpFile);

                LaunchDiffTool(tmpFile, targetFile);
            }
            finally {
                File.Delete(tmpFile);
            }
        }
        void CopyFileContent(string file, string tmpFile) {
            using (FileStream stream = File.OpenRead(file))
            using (FileStream writeStream = File.OpenWrite(tmpFile)) {
                byte[] buffer = new Byte[1024];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, 1024)) > 0) 
                    writeStream.Write(buffer, 0, bytesRead);
            }
        }
        void LaunchDiffTool(string leftFile, string rightFile) {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = Options.DiffTool;
            startInfo.Arguments = string.Format("\"{0}\" \"{1}\"", leftFile, rightFile);

            Process process = Process.Start(startInfo);
            process.WaitForExit();
        }
        public string GetMergeVcsPathByOriginalPath(string filePath, DXVcsBranch currentBranch) {
            string relativePath = Port.GetRelativePath(filePath);
            string result = relativePath.Replace(Port.MasterBranch.Path, currentBranch.Path);
            string ext = Path.GetExtension(filePath);
            if (ext == ".sln" || ext == ".csproj")
                return result.EndsWith(ext) ? result : result + Path.GetFileName(filePath);
            return result;
        }
        string GetMergeVcsPathByTargetPath(string filePath, DXVcsBranch currentBranch) {
            return Port.GetRelativePath(filePath, currentBranch);
        }
        public bool CheckIn(CheckInViewModel checkInViewModel, DXVcsBranch targetBranch, bool isNew) {
            Logger.AddInfo("CheckInCommand. Perform checkin file: " + checkInViewModel.FilePath);
            try {
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
                string filePath = GetFilePathForBranch(checkInViewModel.FilePath, targetBranch);
                string vcsOriginalPath = GetMergeVcsPathByTargetPath(filePath, targetBranch);

                if (isNew) 
                    repository.AddFile(vcsOriginalPath, File.ReadAllBytes(filePath), checkInViewModel.Comment);
                else
                    repository.CheckInFile(vcsOriginalPath, filePath, checkInViewModel.Comment);
                if (checkInViewModel.StaysChecked)
                    repository.CheckOutFile(vcsOriginalPath, filePath, checkInViewModel.Comment);
            }
            catch (Exception e) {
                Logger.AddError("CheckInCommand. CheckIn failed.", e);
                return false;
            }
            return true;
        }
        public void CompareWithCurrentVersion(string filePath, int leftVersion = 0, int rightVersion = 0, bool compareWithCurrent = false) {
            IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
            string vcsOriginalPath = Port.GetRelativePath(filePath);
            PreviewTarget(repository, filePath, vcsOriginalPath, leftVersion, rightVersion, compareWithCurrent);
        }
        public void CompareWithPortVersion(string filePath, DXVcsBranch current, bool isNew) {
            try {
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
                string vcsTargetPath = GetMergeVcsPathByOriginalPath(filePath, current);
                string fileTargetPath = isNew 
                    ? GetFilePathForNewFile(repository, vcsTargetPath, current)
                    : repository.GetFileWorkingPath(vcsTargetPath);
                PreviewTarget(repository, filePath, vcsTargetPath, fileTargetPath, isNew);
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
            catch (Exception e) {
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
                string vcsFilePath = GetMergeVcsPathByOriginalPath(filePath, currentBranch);
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
            catch (Exception e) {
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
        string GetFilePathForNewFile(IDXVcsRepository repository, string vcsPath, DXVcsBranch currentBranch) {
            string testVcsPath = currentBranch.Path;
            string result = repository.GetFileWorkingPath(testVcsPath) + @"\";
            return vcsPath.Replace(currentBranch.Path, result);
        }
        public string GetFilePathForBranch(string path, DXVcsBranch currentBranch) {
            try {
                string relativePath = GetMergeVcsPathByOriginalPath(path, currentBranch);
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
                return repository.GetFileWorkingPath(relativePath);
            }
            catch (Exception e) {
                Logger.AddError("GetFilePathForBranch failed.", e);
            }
            return string.Empty;
        }
        public IEnumerable<string> FindWorkingFolders(List<DXVcsBranch> branches) {
            try {
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
                return branches.Select(branch => repository.GetFileWorkingPath(branch.Path)).ToList();
            }
            catch (Exception e) {
                Logger.AddError("GetFilePathForBranch failed.", e);
            }
            return null;
        }
        public void CompareCurrentWithPortVersion(string filePath, DXVcsBranch current) {
            try {
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
                string vcsTargetPath = GetMergeVcsPathByOriginalPath(filePath, current);
                if (!repository.IsUnderVss(vcsTargetPath)) {
                    DXMessageBox.Show("Target is not under vss");
                    return;
                }
                string fileTargetPath = repository.GetFileWorkingPath(vcsTargetPath);
                PreviewTarget(repository, filePath, vcsTargetPath, fileTargetPath, false);
            }
            catch (Exception e) {
                DXMessageBox.Show(e.Message);
            }
        }
        public bool IsItemUnderVss(string filePath, DXVcsBranch current) {
            try {
                string vcsTargetPath = GetMergeVcsPathByOriginalPath(filePath, current);
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(Port.VcsServer);
                return repository.IsUnderVss(vcsTargetPath);
            }
            catch (Exception e) {
                Logger.AddError("IsItemUnderVss failed.", e);
            }
            return true;
        }
    }
}