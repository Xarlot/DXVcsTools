using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using DevExpress.Mvvm;
using DXVcsTools.Data;

namespace DXVcsTools.UI.ViewModel {
    public class InternalBlameViewModel : BindableBase {
        IEnumerable<IBlameLine> blame;
        int revision;
        string filePath;
        int lineNumber;
        IBlameLine currentLine;
        public string FilePath {
            get { return filePath; }
            set { SetProperty(ref filePath, value, () => FilePath); }
        }
        public IBlameLine CurrentLine {
            get { return currentLine; }
            set { SetProperty(ref currentLine, value, () => CurrentLine); }
        }
        public int Revision {
            get { return revision; }
            set { SetProperty(ref revision, value, () => Revision); }
        }
        public IEnumerable<IBlameLine> Blame {
            get { return blame; }
            set { SetProperty(ref blame, value, () => Blame); }
        }
        public ICommand PreviousCommand { get; private set; }
        public ICommand NextCommand { get; private set; }

        public InternalBlameViewModel(string filePath, int? lineNumber, BlameHelper blameHelper) {
            PreviousCommand = new DelegateCommand(ExecutePrevious, CanExecutePrevious);
            NextCommand = new DelegateCommand(NextPrevious, CanExecuteNext);
            this.lineNumber = lineNumber - 1 ?? 0;
            FilePath = filePath;
            Revision = blameHelper.GetLastRevision(filePath, lineNumber);
            Blame = blameHelper.BlameAtRevision(filePath, lineNumber, Revision);
            CurrentLine = Blame.ElementAtOrDefault(this.lineNumber);
        }
        bool CanExecuteNext() {
            return true;
        }
        void NextPrevious() {
        }
        bool CanExecutePrevious() {
            return true;
        }
        void ExecutePrevious() {
        }
    }
}
