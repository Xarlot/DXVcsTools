using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DevExpress.Mvvm;
using DXVcsTools.Core;
using DXVcsTools.Data;

namespace DXVcsTools.UI.ViewModel {
    public class InternalBlameViewModel : BindableBase {
        readonly Dictionary<int, InternalBlameViewState> blameCache = new Dictionary<int, InternalBlameViewState>();
        readonly int maxRevision;
        IList<IBlameLine> blame;
        string filePath;
        readonly BlameHelper blameHelper;
        readonly MergeHelper mergeHelper;
        IBlameLine currentLine;
        public BlameHelper BlameHelper { get { return blameHelper; } }
        int currentRevision;
        public int CurrentRevision {
            get { return currentRevision; }
            set { SetProperty(ref currentRevision, value, () => CurrentRevision, NavigateToRevision); }
        }
        public string FilePath {
            get { return filePath; }
            set { SetProperty(ref filePath, value, () => FilePath); }
        }
        public IBlameLine CurrentLine {
            get { return currentLine; }
            set { SetProperty(ref currentLine, value, () => CurrentLine); }
        }
        public IList<IBlameLine> Blame {
            get { return blame; }
            set { SetProperty(ref blame, value, () => Blame); }
        }
        public ICommand CompareWithPreviousCommand { get; private set; }
        public ICommand PreviousRevisionCommand { get; private set; }
        public ICommand SpecifiedRevisionCommand { get; private set; }
        public ICommand NextRevisionCommand { get; private set; }
        public ICommand MaxRevisionCommand { get; private set; }
        public ICommand CompareCurrentFileCommand { get; private set; }

        public InternalBlameViewModel(string filePath, int? lineNumber, BlameHelper blameHelper) {
            this.blameHelper = blameHelper;
            this.mergeHelper = new MergeHelper(blameHelper.Options, blameHelper.PortOptions);
            FilePath = filePath;
            PreviousRevisionCommand = new DelegateCommand(NavigateToPreviousRevision, CanNavigateToPreviousRevision);
            NextRevisionCommand = new DelegateCommand(NavigateToNextRevision, CanNavigateToNextRevision);
            SpecifiedRevisionCommand = new DelegateCommand(NavigateToSpecifiedRevision, CanNavigateToSpecifiedRevision);
            MaxRevisionCommand = new DelegateCommand(NavigateToMaxRevision, CanNavigateToMaxRevision);
            CompareWithPreviousCommand = new DelegateCommand(CompareWithPrevious, CanCompareWithPrevious);
            CompareCurrentFileCommand = new DelegateCommand(CompareCurrentFile, CanCompareCurrentFile);

            int line = lineNumber - 1 ?? 0;
            maxRevision = blameHelper.GetLastRevision(filePath, line);
            CurrentRevision = maxRevision;
            CurrentLine = Blame.ElementAtOrDefault(line);
        }
        bool CanCompareCurrentFile() {
            return CurrentLine != null;
        }
        void CompareCurrentFile() {
            mergeHelper.CompareWithCurrentVersion(filePath, CurrentRevision, 0, true);
        }
        bool CanCompareWithPrevious() {
            return CurrentRevision > 0;
        }
        void CompareWithPrevious() {
            mergeHelper.CompareWithCurrentVersion(filePath, CurrentRevision - 1, CurrentRevision);
        }
        bool CanNavigateToMaxRevision() {
            return CurrentRevision != maxRevision;
        }
        void NavigateToMaxRevision() {
            CurrentRevision = maxRevision;
        }
        bool CanNavigateToSpecifiedRevision() {
            return CurrentLine != null && CurrentRevision != CurrentLine.Revision;
        }
        void NavigateToSpecifiedRevision() {
            CurrentRevision = CurrentLine.Revision;
        }
        bool CanNavigateToNextRevision() {
            return CurrentRevision < maxRevision;
        }
        void NavigateToNextRevision() {
            CurrentRevision += 1;
        }
        bool CanNavigateToPreviousRevision() {
            return CurrentRevision > 0;
        }
        void NavigateToPreviousRevision() {
            CurrentRevision -= 1;
        }
        void NavigateToRevision() {
            int revision = CurrentRevision;
            InternalBlameViewState state;
            if (!blameCache.TryGetValue(revision, out state)) {
                var currentBlame = BlameHelper.BlameAtRevision(filePath, null, revision);
                state = new InternalBlameViewState() { Blame = currentBlame, CurrentLine = CurrentLine, Revision = revision };
                blameCache[revision] = state;
            }
            Blame = state.Blame;
            CurrentLine = state.CurrentLine;
        }
    }

    public class InternalBlameViewState {
        public int Revision { get; set; }
        public IList<IBlameLine> Blame { get; set; }
        public IBlameLine CurrentLine { get; set; }
    }
}
