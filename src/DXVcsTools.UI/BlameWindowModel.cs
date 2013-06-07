using System.Collections.Generic;
using DXVcsTools.Core;
using DXVcsTools.DXVcsClient;
using DXVcsTools.Data;

namespace DXVcsTools.UI {
    public class BlameWindowModel {
        readonly bool direct;
        readonly string fileName;
        readonly string fileSource;
        readonly int lineNumber;
        FileDiffInfo fileDiffInfo;
        public BlameWindowModel(string fileName, string fileSource, int lineNumber, bool direct) {
            this.lineNumber = lineNumber;
            this.fileName = fileName;
            this.fileSource = fileSource;
            this.direct = direct;
        }

        public int LineNumber {
            get { return lineNumber; }
        }

        public string FileName {
            get { return fileName; }
        }

        public int LastRevision {
            get { return FileDiffInfo.LastRevision; }
        }

        FileDiffInfo FileDiffInfo {
            get {
                if (fileDiffInfo == null)
                    fileDiffInfo = GetHistoryData();
                return fileDiffInfo;
            }
        }

        public IList<IBlameLine> BlameAtRevision(int revision) {
            return FileDiffInfo.BlameAtRevision(revision);
        }

        FileDiffInfo GetHistoryData() {
            string vcsFile;
            IDXVcsRepository dxRepository;
            if (direct) {
                dxRepository = DXVcsConnectionHelper.Connect(fileSource);
                vcsFile = fileName;
            }
            else {
                dxRepository = DXVcsConnectionHelper.Connect(fileName, fileSource, out vcsFile);
            }
            return dxRepository.GetFileDiffInfo(vcsFile);
        }
    }
}