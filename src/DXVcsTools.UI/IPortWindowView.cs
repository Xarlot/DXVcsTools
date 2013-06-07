using System;
using System.Collections.Generic;

namespace DXVcsTools.UI {
    public interface IPortWindowView {
        bool CanUpdate { get; }
        string Title { get; set; }
        string SourceFile { get; set; }
        string TargetFile { get; set; }
        string OriginalFile { get; set; }
        string CheckInComment { get; set; }
        bool ReviewTarget { get; set; }
        bool CheckInTarget { get; set; }
        bool OkButtonEnabled { get; set; }
        bool CompareButtonEnabled { get; set; }
        bool BranchSelectorEnabled { get; set; }
        int SelectedBranchIndex { get; set; }

        event EventHandler SourceFileChanged;
        event EventHandler OriginalFileChanged;
        event EventHandler TargetFileChanged;
        event EventHandler CheckInCommentChanged;
        event EventHandler CheckInTargetToggled;
        event EventHandler CancelButtonClick;
        event EventHandler OkButtonClick;
        event EventHandler CompareButtonClick;
        event EventHandler BranchSelectionChanged;

        void FillBranchSelector(IEnumerable<string> branches);
        void Close();
        void ShowError(string title, string message);
        void ShowInfo(string title, string message);
        bool ShowQuestion(string title, string message);

        void ShowModal();
    }
}