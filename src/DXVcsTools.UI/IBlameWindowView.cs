using System;
using System.Collections.Generic;
using DXVcsTools.Data;

namespace DXVcsTools.UI
{
    public interface IBlameWindowView
    {
        string Caption { get; set; }
        int CurrentLineIndex { get; }
        IList<IBlameLine> Lines { get; }

        event EventHandler CopyComment;
        event EventHandler ShowPreviousRevision;
        event EventHandler ShowDifferences;

        void ShowModal();
        void ClipboardSetText(string text);
        void ShowRevision(IList<IBlameLine> data, string caption, int lineNumber);
        void ShowError(string title, string message);
    }
}
