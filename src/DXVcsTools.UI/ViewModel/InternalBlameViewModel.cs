using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using DevExpress.Mvvm;
using DXVcsTools.Data;

namespace DXVcsTools.UI.ViewModel {
    public class InternalBlameUndoManager {
        readonly Stack<InternalBlameViewState> undoStack = new Stack<InternalBlameViewState>();
        readonly Stack<InternalBlameViewModel> redoStack = new Stack<InternalBlameViewModel>();
        InternalBlameViewState currentState;

        public void RegisterState(InternalBlameViewState state) {
            if (Equals(state, currentState))
                return;
            if (currentState == null) {
                currentState = state;
                return;
            }
            redoStack.Clear();
            undoStack.Push(currentState);
            currentState = state;
        }
        public bool CanUndo() {
            return undoStack.Count > 0;
        }
        public void Undo(InternalBlameViewModel model) {
            if (undoStack.Count == 0)
                return;
            currentState = undoStack.Pop();
            ApplyCurrentState(model);
        }
        public void ApplyCurrentState(InternalBlameViewModel model) {
            if (currentState == null)
                return;
            model.Blame = currentState.Blame;
            model.CurrentLine = currentState.CurrentLine;
        }
    }
    public class InternalBlameViewModel : BindableBase {
        IEnumerable<IBlameLine> blame;
        string filePath;
        readonly BlameHelper blameHelper;
        IBlameLine currentLine;
        public BlameHelper BlameHelper { get { return blameHelper; } }
        InternalBlameUndoManager undoManager = new InternalBlameUndoManager();
        public string FilePath {
            get { return filePath; }
            set { SetProperty(ref filePath, value, () => FilePath); }
        }
        public IBlameLine CurrentLine {
            get { return currentLine; }
            set { SetProperty(ref currentLine, value, () => CurrentLine); }
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
            int line = lineNumber - 1 ?? 0;
            FilePath = filePath;
            int revision = blameHelper.GetLastRevision(filePath, line);
            Blame = blameHelper.BlameAtRevision(filePath, line, revision);
            CurrentLine = Blame.ElementAtOrDefault(line);
            undoManager.RegisterState(new InternalBlameViewState() {Blame = Blame, CurrentLine = currentLine, });
        }
        bool CanNavigateToPreviousRevision(IBlameLine line) {
            return line.Revision > 0;
        }
        void NavigateToPreviousRevision(IBlameLine line) {
            //int revision = line.Revision - 1;
            //InternalBlameViewState state = new InternalBlameViewState() { FilePath = filePath, Line = lineNumber, Revision = revision };
            //undoStack.Push(state);
            //redoStack.Clear();
            //int index = Blame.ToList().FindIndex(item => item == line);
            //Blame = blameHelper.BlameAtRevision(filePath, index > 0 ? new int?(index) : null, revision);
            //CurrentLine = Blame.FirstOrDefault();
        }
        bool CanExecuteNext() {
            return false;
        }
        void NextPrevious() {
        }
        bool CanExecutePrevious() {
            return undoManager.CanUndo();
        }
        void ExecutePrevious() {
        }
    }

    public class InternalBlameViewState {
        public IEnumerable<IBlameLine> Blame { get; set; }
        public IBlameLine CurrentLine { get; set; }
    }
}
