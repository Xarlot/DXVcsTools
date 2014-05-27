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
        BlameHelper blameHelper;
        IBlameLine currentLine;
        Stack<InternalBlameViewState> undoStack = new Stack<InternalBlameViewState>();
        Stack<InternalBlameViewModel> redoStack = new Stack<InternalBlameViewModel>();
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
        public ICommand NavigateToPreviousRevisionCommand { get; private set; }

        public InternalBlameViewModel(string filePath, int? lineNumber, BlameHelper blameHelper) {
            this.blameHelper = blameHelper;
            PreviousCommand = new DelegateCommand(ExecutePrevious, CanExecutePrevious);
            NextCommand = new DelegateCommand(NextPrevious, CanExecuteNext);
            NavigateToPreviousRevisionCommand = new DelegateCommand<IBlameLine>(NavigateToPreviousRevision, CanNavigateToPreviousRevision);
            this.lineNumber = lineNumber - 1 ?? 0;
            FilePath = filePath;
            Revision = blameHelper.GetLastRevision(filePath, lineNumber);
            Blame = blameHelper.BlameAtRevision(filePath, lineNumber, Revision);
            CurrentLine = Blame.ElementAtOrDefault(this.lineNumber);
        }
        bool CanNavigateToPreviousRevision(IBlameLine line) {
            return line.Revision > 0;
        }
        void NavigateToPreviousRevision(IBlameLine line) {
            InternalBlameViewState state = new InternalBlameViewState() {FilePath = filePath, Line = lineNumber, Revision = Revision};
            undoStack.Push(state);
            redoStack.Clear();
         //   Blame = blameHelper.BlameAtRevision(filePath, line.SourceLine, line.Revision - 1);
        }
        bool CanExecuteNext() {
            return redoStack.Any();
        }
        void NextPrevious() {
        }
        bool CanExecutePrevious() {
            return undoStack.Any();
        }
        void ExecutePrevious() {
        }
    }

    public class InternalBlameViewState {
        protected bool Equals(InternalBlameViewState other) {
            return Revision == other.Revision && Line == other.Line && string.Equals(FilePath, other.FilePath);
        }
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((InternalBlameViewState)obj);
        }
        public override int GetHashCode() {
            unchecked {
                var hashCode = Revision;
                hashCode = (hashCode * 397) ^ Line;
                hashCode = (hashCode * 397) ^ (FilePath != null ? FilePath.GetHashCode() : 0);
                return hashCode;
            }
        }
        public int Revision { get; set; }
        public int Line { get; set; }
        public string FilePath { get; set; }
    }
}
