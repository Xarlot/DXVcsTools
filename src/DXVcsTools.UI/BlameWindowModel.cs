using System;
using System.Collections.Generic;
using DXVcsTools.Data;
using DXVcsTools.DXVcsClient;
using DXVcsTools.Core;

namespace DXVcsTools.UI
{
    public class BlameWindowModel
    {
        int lineNumber;
        string fileName;
        string fileSource;
        bool direct;
        FileDiffInfo fileDiffInfo;

        public int LineNumber
        {
            get { return lineNumber; }
        }

        public string FileName
        {
            get { return fileName; }
        }
        
        public int LastRevision
        {
            get { return FileDiffInfo.LastRevision; }
        }
        
        private FileDiffInfo FileDiffInfo { 
            get {
                if(fileDiffInfo == null)
                    fileDiffInfo = GetHistoryData();
                return fileDiffInfo;
            }
        }

        public BlameWindowModel(string fileName, string fileSource, int lineNumber, bool direct)
        {
            this.lineNumber = lineNumber;
            this.fileName = fileName;
            this.fileSource = fileSource;
            this.direct = direct;
        }

        public IList<IBlameLine> BlameAtRevision(int revision)
        {
            return FileDiffInfo.BlameAtRevision(revision);
        }

        private FileDiffInfo GetHistoryData() { 
            string vcsFile;
            IDXVcsRepository dxRepository;
            if (direct)
            {
                dxRepository = DXVcsConnectionHelper.Connect(fileSource);
                vcsFile = fileName;
            }
            else
            {
                dxRepository = DXVcsConnectionHelper.Connect(fileName, fileSource, out vcsFile);
            }
            return dxRepository.GetFileDiffInfo(vcsFile);
        }
    }
}
