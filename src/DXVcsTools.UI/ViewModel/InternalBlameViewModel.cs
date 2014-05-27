using System.Collections.Generic;
using DevExpress.Mvvm;
using DXVcsTools.Data;

namespace DXVcsTools.UI.ViewModel {
    public class InternalBlameViewModel : BindableBase {
        IEnumerable<IBlameLine> blame;
        int revision;
        string filePath;
        int? lineNumber;
        public int Revision {
            get { return revision; }
            set { SetProperty(ref revision, value, () => Revision); }
        }
        public IEnumerable<IBlameLine> Blame {
            get { return blame; }
            set { SetProperty(ref blame, value, () => Blame); }
        }

        public InternalBlameViewModel(string filePath, int? lineNumber, BlameHelper blameHelper) {
            this.filePath = filePath;
            this.lineNumber = lineNumber;
            Revision = blameHelper.GetLastRevision(filePath, lineNumber);
            Blame = blameHelper.BlameAtRevision(filePath, lineNumber, Revision);
        }
    }
}
