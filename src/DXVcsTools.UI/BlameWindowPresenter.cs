using System;
using System.Collections.Generic;
using DXVcsTools.Data;

namespace DXVcsTools.UI {
    public class BlameWindowPresenter {
        readonly BlameWindowModel model;
        readonly IBlameWindowView view;

        public BlameWindowPresenter(IBlameWindowView view, BlameWindowModel model) {
            this.view = view;
            this.model = model;
            view.CopyComment += view_CopyComment;
            view.ShowDifferences += view_ShowDifferences;
            view.ShowPreviousRevision += view_ShowPreviousRevision;
        }
        public void Show() {
            try {
                view.Caption = "DXBlame - " + model.FileName;
                ShowRevision(model.LastRevision, model.LineNumber);
                view.ShowModal();
            }
            catch (Exception exception) {
                view.ShowError(null, exception.Message);
            }
        }
        void view_CopyComment(object sender, EventArgs e) {
            IList<IBlameLine> lines = view.Lines;
            view.ClipboardSetText(lines[view.CurrentLineIndex].Comment);
        }
        void view_ShowDifferences(object sender, EventArgs e) {
            throw new NotImplementedException();
        }
        void view_ShowPreviousRevision(object sender, EventArgs e) {
            IList<IBlameLine> lines = view.Lines;
            ShowRevision(lines[view.CurrentLineIndex].Revision, view.CurrentLineIndex);
        }
        void ShowRevision(int revision, int lineNumber) {
            string caption = model.LastRevision == revision ? string.Format("Head({0})", revision) : revision.ToString();
            view.ShowRevision(model.BlameAtRevision(revision), caption, lineNumber);
        }
    }
}