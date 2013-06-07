using System;
using System.Collections.Generic;
using System.IO;
using DXVCS;

namespace DXVcsTools.DXVcsClient
{
    class DXVcsRepository : IDXVcsRepository
    {
        IDXVCSService _service;

        internal DXVcsRepository(IDXVCSService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            _service = service;

            bool isAdmin;
            if (!_service.IsCorrectUser(out isAdmin))
                throw new ApplicationException("Invalid user name");
        }

        public FileVersionInfo[] GetFileHistory(string vcsFile, out string fileName)
        {
            if (string.IsNullOrEmpty(vcsFile))
                throw new ArgumentException("vcsFile");

            fileName = Path.GetFileName(vcsFile);

            FileHistory fileHistory = new FileHistory(vcsFile, _service);
            List<FileVersionInfo> result = new List<FileVersionInfo>(fileHistory.Count);
            
            foreach(FileVersionInfo fileVersionInfo in fileHistory)
                result.Add(fileVersionInfo);

            result.Reverse();

            return result.ToArray();
        }

        public FileDiffInfo GetFileDiffInfo(string vcsFile)
        {
            return GetFileDiffInfo(vcsFile, null, SpacesAction.Compare);
        }

        public FileDiffInfo GetFileDiffInfo(string vcsFile, Action<int, int> progressAction, SpacesAction spacesAction)
        {
            FileDiffInfo diffInfo;
            
            FileHistory history = new FileHistory(vcsFile, _service);
            diffInfo = new FileDiffInfo(history.Count);
            diffInfo.SpacesAction = spacesAction;

            int index = 0;
            foreach (FileVersionInfo fileVersionInfo in history)
            {
                if (progressAction != null)
                    progressAction(index, history.Count);

                diffInfo.AddItem(index, fileVersionInfo);
                index++;
            }
            return diffInfo;
        }

        public void GetLatestVersion(string vcsFile, string fileName)
        {
            if (string.IsNullOrEmpty(vcsFile))
                throw new ArgumentException("vcsFile");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("path");

            byte[] data = DXVCSHelpers.TryToDecompressData(_service.GetFileData(vcsFile, null));
            File.WriteAllBytes(fileName, data);
        }

        public void Get(string vcsFile, string fileName, int version)
        {
            if (string.IsNullOrEmpty(vcsFile))
                throw new ArgumentException("vcsFile");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("path");

            if (version <= 0)
                throw new ArgumentException("version");

            byte[] data = DXVCSHelpers.TryToDecompressData(_service.GetFileData(vcsFile, version));
            File.WriteAllBytes(fileName, data);
        }

        public void CheckOutFile(string vcsFile, string localFile, string comment)
        {
            if (string.IsNullOrEmpty(vcsFile))
                throw new ArgumentException("vcsFile");

            if (string.IsNullOrEmpty(localFile))
                throw new ArgumentException("localFile");

            bool getLocalCopy = !File.Exists(localFile) || !_service.GetFile(vcsFile).CheckedOutMe;
            _service.CheckOut(Environment.MachineName, new string[] { vcsFile }, new string[] { Path.GetDirectoryName(localFile) }, new string[] { comment }, null);

            if (File.Exists(localFile))
                File.SetAttributes(localFile, FileAttributes.Normal);

            if (getLocalCopy)
            {
                GetLatestVersion(vcsFile, localFile);
            }
        }

        public void CheckInFile(string vcsFile, string localFile, string comment)
        {
            if (string.IsNullOrEmpty(vcsFile))
                throw new ArgumentException("vcsFile");

            if (string.IsNullOrEmpty(localFile))
                throw new ArgumentException("localFile");

            if (!_service.GetFile(vcsFile).CheckedOutMe)
                throw new InvalidOperationException("Can't check-in: the file is not checked out: " + vcsFile);

            byte[][] data = new byte[1][] { File.ReadAllBytes(localFile) };
            string result = _service.CheckIn(new string[] { vcsFile }, data, new DateTime[] { File.GetLastWriteTimeUtc(localFile) }, new string[] { comment }, false);
            File.SetAttributes(localFile, File.GetAttributes(localFile) | FileAttributes.ReadOnly);

        }

        public string GetFileWorkingPath(string vcsFile)
        {
            string workingFolder = GetWorkingFolder(Path.GetDirectoryName(vcsFile).Replace("\\", "/"));
            if (string.IsNullOrEmpty(workingFolder))
                return null;

            return Path.Combine(workingFolder, Path.GetFileName(vcsFile));
        }

        private string GetWorkingFolder(string vcsProject)
        {
            if (string.IsNullOrEmpty(vcsProject))
                throw new ArgumentException("vcsProject");

            return _service.GetWorkingFolder(Environment.MachineName, vcsProject);
        }
    }
}
