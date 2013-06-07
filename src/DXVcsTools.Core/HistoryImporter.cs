using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DXVcsTools.DXVcsClient;
using SharpSvn;
using System.IO;

namespace DXVcsTools.Core
{
    public class HistoryImporter
    {
        readonly Uri _svnRepository;
        readonly string _workingCopy;

        public HistoryImporter(string svnRepository, string workingCopy)
        {
            if (string.IsNullOrEmpty(svnRepository))
                throw new ArgumentException("svnRepository");

            if (string.IsNullOrEmpty(workingCopy))
                throw new ArgumentException("workingCopy");

            _svnRepository = new Uri(PathHelper.ResolvePath(svnRepository));
            _workingCopy = PathHelper.ResolvePath(workingCopy);
        }

        public Uri ImportFile(string file, string projectFile)
        {
            string vcsFile;
            IDXVcsRepository dxRepository = DXVcsConnectionHelper.Connect(file, projectFile, out vcsFile);
            return ImportFile(vcsFile, dxRepository);
        }

        public Uri ImportFileDirect(string vcsService, string vcsFile)
        {
            return ImportFile(vcsFile, DXVcsConnectionHelper.Connect(vcsService));
        }

        private Uri ImportFile(string vcsFile, IDXVcsRepository dxRepository) {
            if (string.IsNullOrEmpty(vcsFile))
                throw new ArgumentException("vcsFile");

            Console.WriteLine("Requesting history items for {0}", vcsFile);
            string fileName;
            FileVersionInfo[] fileHistory = dxRepository.GetFileHistory(vcsFile, out fileName);
            Console.WriteLine("Read {0} history items for file {1}", fileHistory.Length, fileName);

            return ImportFileHistory(fileName, fileHistory);
        }

        private Uri ImportFileHistory(string fileName, FileVersionInfo[] fileHistory)
        {
            using (SvnClient svn = new SvnClient())
            {
                Uri svnImportFolder = CreateTempRepositoryDirectory(svn);
                CreateWorkingCopy(svn, new SvnUriTarget(svnImportFolder), _workingCopy);

                string filePath = Path.Combine(_workingCopy, fileName);
                File.Create(filePath).Close();
                if (!svn.Add(filePath))
                    throw new ApplicationException(string.Format("Can't add file {0} under SVN control", filePath));
                Console.WriteLine("Data file {0} added to SVN", filePath);

                foreach (FileVersionInfo fileVersionInfo in fileHistory)
                {
                    File.WriteAllBytes(filePath, fileVersionInfo.Data);

                    SvnCommitArgs commitArgs = new SvnCommitArgs();
                    commitArgs.LogMessage = string.Format(
                        "Revision: {0}\r\nUser: {1}\r\nDate: {2}\r\nComment: {3}",
                        fileVersionInfo.Version,
                        fileVersionInfo.User,
                        fileVersionInfo.Date,
                        fileVersionInfo.Comment);
                    svn.Commit(filePath, commitArgs);
                    Console.WriteLine("Committed revision {0}", fileVersionInfo.Version);
                }

                return new Uri(svnImportFolder, fileName);
            }
        }

        private Uri CreateTempRepositoryDirectory(SvnClient client)
        {
            Uri tempDirectory = new Uri(client.GetRepositoryRoot(_svnRepository), string.Concat(Guid.NewGuid(), "/"));

            if (!client.RemoteCreateDirectory(tempDirectory, new SvnCreateDirectoryArgs() { LogMessage = string.Empty }))
                throw new ApplicationException(string.Format("Can't create remote directory {0}", tempDirectory));

            return tempDirectory;
        }

        private void CreateWorkingCopy(SvnClient client, SvnUriTarget target, string path)
        {
            if (Directory.Exists(path))
                DirectoryHelper.DeleteDirectory(path);

            client.CheckOut(target, path);
            Console.WriteLine("Checked out {0} to {1}", target, path);
        }

    }
}
