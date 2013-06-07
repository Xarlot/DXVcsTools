using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DXVcsTools.UI.WinForms {
    public partial class DXPortWindow : Form, IPortWindowView {
        readonly object BranchSelectionChanged = new object();
        readonly object CancelButtonClick = new object();
        readonly object CheckInCommentChanged = new object();
        readonly object CheckInTargetToggled = new object();
        readonly object CompareButtonClick = new object();
        readonly object OkButtonClick = new object();
        readonly object OriginalFileChanged = new object();
        readonly object SourceFileChanged = new object();
        readonly object TargetFileChanged = new object();
        public DXPortWindow() {
            InitializeComponent();
        }

        void sourceFile_TextChanged(object sender, EventArgs e) {
            RaiseEvent(SourceFileChanged);
        }

        void originalFile_TextChanged(object sender, EventArgs e) {
            RaiseEvent(OriginalFileChanged);
        }

        void targetFile_TextChanged(object sender, EventArgs e) {
            RaiseEvent(TargetFileChanged);
        }

        void checkInComment_TextChanged(object sender, EventArgs e) {
            RaiseEvent(CheckInCommentChanged);
        }

        void checkInTarget_CheckedChanged(object sender, EventArgs e) {
            RaiseEvent(CheckInTargetToggled);
        }

        void cancelButton_Click(object sender, EventArgs e) {
            RaiseEvent(CancelButtonClick);
        }

        void okButton_Click(object sender, EventArgs e) {
            RaiseEvent(OkButtonClick);
        }

        void compareButton_Click(object sender, EventArgs e) {
            RaiseEvent(CompareButtonClick);
        }

        void branches_SelectedIndexChanged(object sender, EventArgs e) {
            RaiseEvent(BranchSelectionChanged);
        }

        void RaiseEvent(object eventObject) {
            var handler = (EventHandler)Events[eventObject];
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #region IPortWindowView Members
        bool IPortWindowView.CanUpdate {
            get { return true; }
        }

        string IPortWindowView.Title {
            get { return Text; }
            set { Text = value; }
        }

        string IPortWindowView.SourceFile {
            get { return sourceFile.Text; }
            set { sourceFile.Text = value; }
        }

        string IPortWindowView.TargetFile {
            get { return targetFile.Text; }
            set { targetFile.Text = value; }
        }

        string IPortWindowView.OriginalFile {
            get { return originalFile.Text; }
            set { originalFile.Text = value; }
        }

        string IPortWindowView.CheckInComment {
            get { return checkInComment.Text; }
            set { checkInComment.Text = value; }
        }

        bool IPortWindowView.ReviewTarget {
            get { return reviewTarget.Checked; }
            set { reviewTarget.Checked = value; }
        }

        bool IPortWindowView.CheckInTarget {
            get { return checkInTarget.Checked; }
            set { checkInTarget.Checked = value; }
        }

        bool IPortWindowView.OkButtonEnabled {
            get { return okButton.Enabled; }
            set { okButton.Enabled = value; }
        }

        bool IPortWindowView.CompareButtonEnabled {
            get { return compareButton.Enabled; }
            set { compareButton.Enabled = value; }
        }

        bool IPortWindowView.BranchSelectorEnabled {
            get { return branchSelector.Enabled; }
            set { branchSelector.Enabled = value; }
        }

        int IPortWindowView.SelectedBranchIndex {
            get { return branchSelector.SelectedIndex; }
            set { branchSelector.SelectedIndex = value; }
        }

        event EventHandler IPortWindowView.SourceFileChanged {
            add { Events.AddHandler(SourceFileChanged, value); }
            remove { Events.AddHandler(SourceFileChanged, value); }
        }

        event EventHandler IPortWindowView.OriginalFileChanged {
            add { Events.AddHandler(OriginalFileChanged, value); }
            remove { Events.AddHandler(OriginalFileChanged, value); }
        }

        event EventHandler IPortWindowView.TargetFileChanged {
            add { Events.AddHandler(TargetFileChanged, value); }
            remove { Events.AddHandler(TargetFileChanged, value); }
        }
        event EventHandler IPortWindowView.CheckInCommentChanged {
            add { Events.AddHandler(CheckInCommentChanged, value); }
            remove { Events.AddHandler(CheckInCommentChanged, value); }
        }
        event EventHandler IPortWindowView.CheckInTargetToggled {
            add { Events.AddHandler(CheckInTargetToggled, value); }
            remove { Events.AddHandler(CheckInTargetToggled, value); }
        }
        event EventHandler IPortWindowView.CancelButtonClick {
            add { Events.AddHandler(CancelButtonClick, value); }
            remove { Events.AddHandler(CancelButtonClick, value); }
        }
        event EventHandler IPortWindowView.OkButtonClick {
            add { Events.AddHandler(OkButtonClick, value); }
            remove { Events.AddHandler(OkButtonClick, value); }
        }
        event EventHandler IPortWindowView.CompareButtonClick {
            add { Events.AddHandler(CompareButtonClick, value); }
            remove { Events.AddHandler(CompareButtonClick, value); }
        }
        event EventHandler IPortWindowView.BranchSelectionChanged {
            add { Events.AddHandler(BranchSelectionChanged, value); }
            remove { Events.AddHandler(BranchSelectionChanged, value); }
        }
        void IPortWindowView.FillBranchSelector(IEnumerable<string> branches) {
            branchSelector.Items.Clear();
            foreach (string branch in branches) {
                branchSelector.Items.Add(branch);
            }
        }

        void IPortWindowView.Close() {
            Close();
        }

        void IPortWindowView.ShowError(string title, string message) {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        void IPortWindowView.ShowInfo(string title, string message) {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        bool IPortWindowView.ShowQuestion(string title, string message) {
            return MessageBox.Show(this, message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        void IPortWindowView.ShowModal() {
            ShowDialog();
        }
        #endregion
    }
}