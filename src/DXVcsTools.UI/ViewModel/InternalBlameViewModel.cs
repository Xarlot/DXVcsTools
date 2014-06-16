using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DevExpress.Mvvm;
using DXVcsTools.Core;
using DXVcsTools.Data;
using DXVcsTools.DXVcsClient;

namespace DXVcsTools.UI.ViewModel {
    public class InternalBlameViewModel : BindableBase {
        FileDiffInfo fileDiffInfo = null;
        int LastRevision { get { return fileDiffInfo.LastRevision; } }
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
        public ICommand LastRevisionCommand { get; private set; }
        public ICommand CompareCurrentFileCommand { get; private set; }

        public InternalBlameViewModel(string filePath, int? lineNumber, BlameHelper blameHelper) {
            this.blameHelper = blameHelper;
            this.mergeHelper = new MergeHelper(blameHelper.Options, blameHelper.PortOptions);
            FilePath = filePath;
            PreviousRevisionCommand = new DelegateCommand(NavigateToPreviousRevision, CanNavigateToPreviousRevision);
            NextRevisionCommand = new DelegateCommand(NavigateToNextRevision, CanNavigateToNextRevision);
            SpecifiedRevisionCommand = new DelegateCommand(NavigateToSpecifiedRevision, CanNavigateToSpecifiedRevision);
            LastRevisionCommand = new DelegateCommand(NavigateToLastRevision, CanNavigateToLastRevision);
            CompareWithPreviousCommand = new DelegateCommand(CompareWithPrevious, CanCompareWithPrevious);
            CompareCurrentFileCommand = new DelegateCommand(CompareCurrentFile, CanCompareCurrentFile);

            int line = lineNumber - 1 ?? 0;

            InitializeFileDiffInfo();
            Blame = fileDiffInfo.BlameAtRevision(LastRevision);
            CurrentRevision = LastRevision;
            CurrentLine = Blame.ElementAtOrDefault(line);
        }
        void InitializeFileDiffInfo() {
            try {
                BusyIndicator.Show();
                BusyIndicator.UpdateText("Loading file diff info: progress {0} from {1}");
                fileDiffInfo = blameHelper.GetFileDiffInfo(filePath, BusyIndicator.UpdateProgress);
            }
            finally {
                BusyIndicator.Close();
            }
        }
        bool CanCompareCurrentFile() {
            return CurrentLine != null;
        }
        void CompareCurrentFile() {
            mergeHelper.CompareWithHistoryVersion(filePath, CurrentRevision, 0, true);
        }
        bool CanCompareWithPrevious() {
            return CurrentRevision > 0;
        }
        void CompareWithPrevious() {
            mergeHelper.CompareWithHistoryVersion(filePath, CurrentRevision - 1, CurrentRevision);
        }
        bool CanNavigateToLastRevision() {
            return CurrentRevision != LastRevision;
        }
        void NavigateToLastRevision() {
            CurrentRevision = LastRevision;
        }
        bool CanNavigateToSpecifiedRevision() {
            return CurrentLine != null && CurrentRevision != CurrentLine.Revision;
        }
        void NavigateToSpecifiedRevision() {
            CurrentRevision = CurrentLine.Revision;
        }
        bool CanNavigateToNextRevision() {
            return CurrentRevision < LastRevision;
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
            Blame = fileDiffInfo.BlameAtRevision(CurrentRevision);
            CurrentLine = null;
        }
    }
}
