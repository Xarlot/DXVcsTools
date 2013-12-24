using System;
using System.Collections.Generic;
using System.IO;
using DXVCS;

namespace DXVcsTools.DXVcsClient {
    class DXVcsRepository : IDXVcsRepository {
        readonly IDXVCSService service;

        internal DXVcsRepository(IDXVCSService service) {
            if (service == null)
                throw new ArgumentNullException("service");

            this.service = service;

            bool isAdmin;
            if (!this.service.IsCorrectUser(out isAdmin))
                throw new ApplicationException("Invalid user name");
        }

        public FileVersionInfo[] GetFileHistory(string vcsFile, out string fileName) {
            if (string.IsNullOrEmpty(vcsFile))
                throw new ArgumentException("vcsFile");

            fileName = Path.GetFileName(vcsFile);

            var fileHistory = new FileHistory(vcsFile, service);
            var result = new List<FileVersionInfo>(fileHistory.Count);
            result.AddRange(fileHistory);
            result.Reverse();
            return result.ToArray();
        }

        public FileDiffInfo GetFileDiffInfo(string vcsFile, SpacesAction spacesAction = SpacesAction.IgnoreAll) {
            return GetFileDiffInfo(vcsFile, null, spacesAction);
        }

        public FileDiffInfo GetFileDiffInfo(string vcsFile, Action<int, int> progressAction, SpacesAction spacesAction) {
            var history = new FileHistory(vcsFile, service);
            FileDiffInfo diffInfo = new FileDiffInfo(history.Count);
            diffInfo.SpacesAction = spacesAction;

            int index = 0;
            foreach (FileVersionInfo fileVersionInfo in history) {
                if (progressAction != null)
                    progressAction(index, history.Count);

                diffInfo.AddItem(index, fileVersionInfo);
                index++;
            }
            return diffInfo;
        }

        public void GetLatestVersion(string vcsFile, string fileName) {
            if (string.IsNullOrEmpty(vcsFile))
                throw new ArgumentException("vcsFile");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("path");

            byte[] data = DXVCSHelpers.TryToDecompressData(service.GetFileData(vcsFile, null));
            File.WriteAllBytes(fileName, data);
        }

        public void Get(string vcsFile, string fileName, int version) {
            if (string.IsNullOrEmpty(vcsFile))
                throw new ArgumentException("vcsFile");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("path");

            if (version <= 0)
                throw new ArgumentException("version");

            byte[] data = DXVCSHelpers.TryToDecompressData(service.GetFileData(vcsFile, version));
            File.WriteAllBytes(fileName, data);
        }

        public void CheckOutFile(string vcsFile, string localFile, string comment) {
            if (string.IsNullOrEmpty(vcsFile))
                throw new ArgumentException("vcsFile");

            if (string.IsNullOrEmpty(localFile))
                throw new ArgumentException("localFile");

            bool getLocalCopy = !File.Exists(localFile) || !service.GetFile(vcsFile).CheckedOutMe;
            service.CheckOut(Environment.MachineName, new[] { vcsFile }, new[] { Path.GetDirectoryName(localFile) }, new[] { comment }, null);

            if (File.Exists(localFile))
                File.SetAttributes(localFile, FileAttributes.Normal);

            if (getLocalCopy) {
                GetLatestVersion(vcsFile, localFile);
            }
        }

        public void CheckInFile(string vcsFile, string localFile, string comment) {
            if (string.IsNullOrEmpty(vcsFile))
                throw new ArgumentException("vcsFile");

            if (string.IsNullOrEmpty(localFile))
                throw new ArgumentException("localFile");

            if (!service.GetFile(vcsFile).CheckedOutMe)
                throw new InvalidOperationException("Can't check-in: the file is not checked out: " + vcsFile);

            var data = new byte[1][] { File.ReadAllBytes(localFile) };
            string result = service.CheckIn(new[] { vcsFile }, data, new[] { File.GetLastWriteTimeUtc(localFile) }, new[] { comment }, false);
            File.SetAttributes(localFile, File.GetAttributes(localFile) | FileAttributes.ReadOnly);
        }

        public string GetFileWorkingPath(string vcsFile) {
            string workingFolder = GetWorkingFolder(Path.GetDirectoryName(vcsFile).Replace("\\", "/"));
            if (string.IsNullOrEmpty(workingFolder))
                return null;

            return Path.Combine(workingFolder, Path.GetFileName(vcsFile));
        }
        string GetWorkingFolder(string vcsProject) {
            if (string.IsNullOrEmpty(vcsProject))
                throw new ArgumentException("vcsProject");

            return service.GetWorkingFolder(Environment.MachineName, vcsProject);
        }
        public void UndoCheckout(string vcsFile, string localFile) {
            if (string.IsNullOrEmpty(vcsFile))
                throw new ArgumentException("vcsFile");
            if (string.IsNullOrEmpty(localFile))
                throw new ArgumentException("localFile");

            if (!service.GetFile(vcsFile).CheckedOutMe)
                throw new InvalidOperationException("Can't undo check out: the file is not checked out: " + vcsFile);
            service.UndoCheckOut(new[] { vcsFile }, new[] { false });
        }
        public bool IsUnderVss(string vcsFile) {
            if (string.IsNullOrEmpty(vcsFile))
                throw new ArgumentException("vcsFile");

            var file = service.FindFile(vcsFile);
            return !file.IsNull;
        }
    }
}