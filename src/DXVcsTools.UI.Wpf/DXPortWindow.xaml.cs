using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DXVcsTools.Core;
using DXVcsTools.UI;
using System.Diagnostics;

namespace DXVcsTools.UI.Wpf
{
    public partial class DXPortWindow : Window, IPortWindowView
    {
        public DXPortWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(DXPortWindow_Loaded);
        }

        void DXPortWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.Assert(ActualHeight > clientArea.ActualHeight);
            MinHeight = clientArea.DesiredSize.Height + (ActualHeight - clientArea.ActualHeight);
        }
      
        private void sourceFile_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SourceFileChanged != null)
                SourceFileChanged(this, EventArgs.Empty);
        }

        private void originalFile_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (OriginalFileChanged != null)
                OriginalFileChanged(this, EventArgs.Empty);
        }

        private void targetFile_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TargetFileChanged != null)
                TargetFileChanged(this, EventArgs.Empty);
        }

        private void checkInComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CheckInCommentChanged != null)
                CheckInCommentChanged(this, EventArgs.Empty);
        }

        private void checkInTarget_Toggled(object sender, RoutedEventArgs e)
        {
            if (CheckInTargetToggled != null)
                CheckInTargetToggled(this, EventArgs.Empty);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (CancelButtonClick != null)
                CancelButtonClick(this, EventArgs.Empty);
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (OkButtonClick != null)
                OkButtonClick(this, EventArgs.Empty);
        }

        private void compareButton_Click(object sender, RoutedEventArgs e)
        {
            if (CompareButtonClick != null)
                CompareButtonClick(this, EventArgs.Empty);
        }

        private void branchSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BranchSelectionChanged != null)
                BranchSelectionChanged(this, EventArgs.Empty);
        }

        #region IPortWindowView Members

        bool IPortWindowView.CanUpdate
        {
            get { return IsInitialized; }
        }

        string IPortWindowView.Title
        {
            get { return this.Title; }
            set { this.Title = value; }
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

        string IPortWindowView.CheckInComment {
            get { return this.checkInComment.Text; }
            set { this.checkInComment.Text = value; }
        }

        bool IPortWindowView.ReviewTarget
        {
            get { return this.reviewTarget.IsChecked == true; }
            set { this.reviewTarget.IsChecked = (bool?)value; }
        }

        bool IPortWindowView.CheckInTarget
        {
            get { return this.checkInTarget.IsChecked == true; }
            set { this.checkInTarget.IsChecked = (bool?)value; }
        }

        bool IPortWindowView.OkButtonEnabled
        {
            get { return this.okButton.IsEnabled; }
            set { this.okButton.IsEnabled = value; }
        }

        bool IPortWindowView.CompareButtonEnabled
        {
            get { return this.compareButton.IsEnabled; }
            set { this.compareButton.IsEnabled = value; }
        }

        bool IPortWindowView.BranchSelectorEnabled
        {
            get { return this.branchSelector.IsEnabled; }
            set { this.branchSelector.IsEnabled = value; }
        }

        int IPortWindowView.SelectedBranchIndex 
        { 
            get { return this.branchSelector.SelectedIndex; }
            set { this.branchSelector.SelectedIndex = value; }
        }

        public event EventHandler SourceFileChanged;

        public event EventHandler OriginalFileChanged;

        public event EventHandler TargetFileChanged;

        public event EventHandler CheckInCommentChanged;

        public event EventHandler CheckInTargetToggled;

        public event EventHandler CancelButtonClick;

        public event EventHandler OkButtonClick;

        public event EventHandler CompareButtonClick;

        public event EventHandler BranchSelectionChanged;

        void IPortWindowView.FillBranchSelector(IEnumerable<string> branches)
        {
            branchSelector.Items.Clear();
            foreach (string branch in branches)
            {
                branchSelector.Items.Add(branch);
            }
        }

        void IPortWindowView.Close() {
            Close();
        }

        void IPortWindowView.ShowError(string title, string message)
        {
            MessageBox.Show(this, message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void IPortWindowView.ShowInfo(string title, string message)
        {
            MessageBox.Show(this, message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        bool IPortWindowView.ShowQuestion(string title, string message)
        {
            return MessageBox.Show(this, message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        void IPortWindowView.ShowModal()
        {
            ShowDialog();
        }

        #endregion
    }
}
