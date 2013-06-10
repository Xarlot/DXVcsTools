using System;
using System.Collections.Generic;
using System.IO;
using DXVcsTools.DXVcsClient;
using SharpSvn;

namespace DXVcsTools.Core {
    public class HistoryImporter {
        readonly Uri svnRepository;
        readonly string workingCopy;

        public HistoryImporter(string svnRepository, string workingCopy) {
            if (string.IsNullOrEmpty(svnRepository))
                throw new ArgumentException("svnRepository");

            if (string.IsNullOrEmpty(workingCopy))
                throw new ArgumentException("workingCopy");

            this.svnRepository = new Uri(PathHelper.ResolvePath(svnRepository));
            this.workingCopy = PathHelper.ResolvePath(workingCopy);
        }

        public Uri ImportFile(string file, string projectFile) {
            string vcsFile;
            IDXVcsRepository dxRepository = DXVcsConnectionHelper.Connect(file, projectFile, out vcsFile);
            return ImportFile(vcsFile, dxRepository);
        }

        public Uri ImportFileDirect(string vcsService, string vcsFile) {
            return ImportFile(vcsFile, DXVcsConnectionHelper.Connect(vcsService));
        }

        Uri ImportFile(string vcsFile, IDXVcsRepository dxRepository) {
            if (string.IsNullOrEmpty(vcsFile))
                throw new ArgumentException("vcsFile");

            Console.WriteLine("Requesting history items for {0}", vcsFile);
            string fileName;
            FileVersionInfo[] fileHistory = dxRepository.GetFileHistory(vcsFile, out fileName);
            Console.WriteLine("Read {0} history items for file {1}", fileHistory.Length, fileName);

            return ImportFileHistory(fileName, fileHistory);
        }

        Uri ImportFileHistory(string fileName, IEnumerable<FileVersionInfo> fileHistory) {
            using (var svn = new SvnClient()) {
                Uri svnImportFolder = CreateTempRepositoryDirectory(svn);
                CreateWorkingCopy(svn, new SvnUriTarget(svnImportFolder), workingCopy);

                string filePath = Path.Combine(workingCopy, fileName);
                File.Create(filePath).Close();
                if (!svn.Add(filePath))
                    throw new ApplicationException(string.Format("Can't add file {0} under SVN control", filePath));
                Console.WriteLine("Data file {0} added to SVN", filePath);

                foreach (FileVersionInfo fileVersionInfo in fileHistory) {
                    File.WriteAllBytes(filePath, fileVersionInfo.Data);

                    var commitArgs = new SvnCommitArgs();
                    commitArgs.LogMessage = string.Format("Revision: {0}\r\nUser: {1}\r\nDate: {2}\r\nComment: {3}", fileVersionInfo.Version, fileVersionInfo.User, fileVersionInfo.Date,
                        fileVersionInfo.Comment);
                    svn.Commit(filePath, commitArgs);
                    Console.WriteLine("Committed revision {0}", fileVersionInfo.Version);
                }

                return new Uri(svnImportFolder, fileName);
            }
        }

        Uri CreateTempRepositoryDirectory(SvnClient client) {
            var tempDirectory = new Uri(client.GetRepositoryRoot(svnRepository), string.Concat(Guid.NewGuid(), "/"));

            if (!client.RemoteCreateDirectory(tempDirectory, new SvnCreateDirectoryArgs {LogMessage = string.Empty}))
                throw new ApplicationException(string.Format("Can't create remote directory {0}", tempDirectory));

            return tempDirectory;
        }

        void CreateWorkingCopy(SvnClient client, SvnUriTarget target, string path) {
            if (Directory.Exists(path))
                DirectoryHelper.DeleteDirectory(path);

            client.CheckOut(target, path);
            Console.WriteLine("Checked out {0} to {1}", target, path);
        }
    }
}