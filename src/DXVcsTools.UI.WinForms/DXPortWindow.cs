using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DXVcsTools.UI.WinForms
{
    public partial class DXPortWindow : Form, IPortWindowView
    {
        public DXPortWindow()
        {
            this.InitializeComponent();
        }
        object SourceFileChanged = new object();
        object OriginalFileChanged = new object();
        object TargetFileChanged = new object();
        object CheckInCommentChanged = new object();
        object CheckInTargetToggled = new object();
        object CancelButtonClick = new object();
        object OkButtonClick = new object();
        object CompareButtonClick = new object();
        object BranchSelectionChanged = new object();

        private void sourceFile_TextChanged(object sender, EventArgs e)
        {
            RaiseEvent(SourceFileChanged);
        }

        private void originalFile_TextChanged(object sender, EventArgs e)
        {
            RaiseEvent(OriginalFileChanged);
        }

        private void targetFile_TextChanged(object sender, EventArgs e)
        {
            RaiseEvent(TargetFileChanged);
        }

        private void checkInComment_TextChanged(object sender, EventArgs e)
        {
            RaiseEvent(CheckInCommentChanged);
        }

        private void checkInTarget_CheckedChanged(object sender, EventArgs e)
        {
            RaiseEvent(CheckInTargetToggled);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            RaiseEvent(CancelButtonClick);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            RaiseEvent(OkButtonClick);
        }

        private void compareButton_Click(object sender, EventArgs e)
        {
            RaiseEvent(CompareButtonClick);
        }

        void branches_SelectedIndexChanged(object sender, EventArgs e)
        {
            RaiseEvent(BranchSelectionChanged);
        }

        #region IPortWindowView Members

        bool IPortWindowView.CanUpdate
        {
            get { return true; }
        }

        string IPortWindowView.Title
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        string IPortWindowView.SourceFile
        {
            get { return this.sourceFile.Text; }
            set { this.sourceFile.Text = value; }
        }

        string IPortWindowView.TargetFile
        {
            get { return this.targetFile.Text; }
            set { this.targetFile.Text = value; }
        }

        string IPortWindowView.OriginalFile
        {
            get { return this.originalFile.Text; }
            set { this.originalFile.Text = value; }
        }

        string IPortWindowView.CheckInComment
        {
            get { return this.checkInComment.Text; }
            set { this.checkInComment.Text = value; }
        }

        bool IPortWindowView.ReviewTarget
        {
            get { return this.reviewTarget.Checked == true; }
            set { this.reviewTarget.Checked = value; }
        }

        bool IPortWindowView.CheckInTarget
        {
            get { return this.checkInTarget.Checked == true; }
            set { this.checkInTarget.Checked = value; }
        }

        bool IPortWindowView.OkButtonEnabled
        {
            get { return this.okButton.Enabled; }
            set { this.okButton.Enabled = value; }
        }

        bool IPortWindowView.CompareButtonEnabled
        {
            get { return this.compareButton.Enabled; }
            set { this.compareButton.Enabled = value; }
        }

        bool IPortWindowView.BranchSelectorEnabled
        {
            get { return this.branchSelector.Enabled; }
            set { this.branchSelector.Enabled = value; }
        }

        int IPortWindowView.SelectedBranchIndex
        {
            get { return this.branchSelector.SelectedIndex; }
            set { this.branchSelector.SelectedIndex = value; }
        }

        event EventHandler IPortWindowView.SourceFileChanged
        {
            add { Events.AddHandler(SourceFileChanged, value); }
            remove { Events.AddHandler(SourceFileChanged, value); }
        }

        event EventHandler IPortWindowView.OriginalFileChanged
        {
            add { Events.AddHandler(OriginalFileChanged, value); }
            remove { Events.AddHandler(OriginalFileChanged, value); }
        }

        event EventHandler IPortWindowView.TargetFileChanged
        {
            add { Events.AddHandler(TargetFileChanged, value); }
            remove { Events.AddHandler(TargetFileChanged, value); }
        }
        event EventHandler IPortWindowView.CheckInCommentChanged
        {
            add { Events.AddHandler(CheckInCommentChanged, value); }
            remove { Events.AddHandler(CheckInCommentChanged, value); }
        }
        event EventHandler IPortWindowView.CheckInTargetToggled
        {
            add { Events.AddHandler(CheckInTargetToggled, value); }
            remove { Events.AddHandler(CheckInTargetToggled, value); }
        }
        event EventHandler IPortWindowView.CancelButtonClick
        {
            add { Events.AddHandler(CancelButtonClick, value); }
            remove { Events.AddHandler(CancelButtonClick, value); }
        }
        event EventHandler IPortWindowView.OkButtonClick
        {
            add { Events.AddHandler(OkButtonClick, value); }
            remove { Events.AddHandler(OkButtonClick, value); }
        }
        event EventHandler IPortWindowView.CompareButtonClick
        {
            add { Events.AddHandler(CompareButtonClick, value); }
            remove { Events.AddHandler(CompareButtonClick, value); }
        }
        event EventHandler IPortWindowView.BranchSelectionChanged
        {
            add { Events.AddHandler(BranchSelectionChanged, value); }
            remove { Events.AddHandler(BranchSelectionChanged, value); }
        }
        void IPortWindowView.FillBranchSelector(IEnumerable<string> branches)
        {
            branchSelector.Items.Clear();
            foreach (string branch in branches)
            {
                branchSelector.Items.Add(branch);
            }
        }

        void IPortWindowView.Close()
        {
            Close();
        }

        void IPortWindowView.ShowError(string title, string message)
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        void IPortWindowView.ShowInfo(string title, string message)
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        bool IPortWindowView.ShowQuestion(string title, string message)
        {
            return MessageBox.Show(this, message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        void IPortWindowView.ShowModal()
        {
            ShowDialog();
        }

        #endregion

        void RaiseEvent(object eventObject)
        {
            EventHandler handler = (EventHandler)Events[eventObject];
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
