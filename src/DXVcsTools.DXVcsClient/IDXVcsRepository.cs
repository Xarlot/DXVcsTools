using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXVcsTools.DXVcsClient
{
    public interface IDXVcsRepository
    {
        FileVersionInfo[] GetFileHistory(string vcsFile, out string fileName);
        FileDiffInfo GetFileDiffInfo(string vcsFile);
        FileDiffInfo GetFileDiffInfo(string vcsFile, Action<int, int> progressAction, SpacesAction spacesAction);
        void GetLatestVersion(string vcsFile, string fileName);
        void Get(string vcsFile, string fileName, int version);
        void CheckOutFile(string vcsFile, string localFile, string comment);
        void CheckInFile(string vcsFile, string localFile, string comment);
        string GetFileWorkingPath(string vcsFile);
    }
}
