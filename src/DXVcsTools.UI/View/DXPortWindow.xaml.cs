using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DXVcsTools.UI.Wpf {
    public partial class DXPortWindow : Window, IPortWindowView {
        public DXPortWindow() {
            InitializeComponent();
            Loaded += DXPortWindow_Loaded;
        }

        void DXPortWindow_Loaded(object sender, RoutedEventArgs e) {
            Debug.Assert(ActualHeight > clientArea.ActualHeight);
            MinHeight = clientArea.DesiredSize.Height + (ActualHeight - clientArea.ActualHeight);
        }

        void sourceFile_TextChanged(object sender, TextChangedEventArgs e) {
            if (SourceFileChanged != null)
                SourceFileChanged(this, EventArgs.Empty);
        }

        void originalFile_TextChanged(object sender, TextChangedEventArgs e) {
            if (OriginalFileChanged != null)
                OriginalFileChanged(this, EventArgs.Empty);
        }

        void targetFile_TextChanged(object sender, TextChangedEventArgs e) {
            if (TargetFileChanged != null)
                TargetFileChanged(this, EventArgs.Empty);
        }

        void checkInComment_TextChanged(object sender, TextChangedEventArgs e) {
            if (CheckInCommentChanged != null)
                CheckInCommentChanged(this, EventArgs.Empty);
        }

        void checkInTarget_Toggled(object sender, RoutedEventArgs e) {
            if (CheckInTargetToggled != null)
                CheckInTargetToggled(this, EventArgs.Empty);
        }

        void cancelButton_Click(object sender, RoutedEventArgs e) {
            if (CancelButtonClick != null)
                CancelButtonClick(this, EventArgs.Empty);
        }

        void okButton_Click(object sender, RoutedEventArgs e) {
            if (OkButtonClick != null)
                OkButtonClick(this, EventArgs.Empty);
        }

        void compareButton_Click(object sender, RoutedEventArgs e) {
            if (CompareButtonClick != null)
                CompareButtonClick(this, EventArgs.Empty);
        }

        void branchSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (BranchSelectionChanged != null)
                BranchSelectionChanged(this, EventArgs.Empty);
        }

        #region IPortWindowView Members
        bool IPortWindowView.CanUpdate {
            get { return IsInitialized; }
        }

        string IPortWindowView.Title {
            get { return Title; }
            set { Title = value; }
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
            get { return reviewTarget.IsChecked == true; }
            set { reviewTarget.IsChecked = value; }
        }

        bool IPortWindowView.CheckInTarget {
            get { return checkInTarget.IsChecked == true; }
            set { checkInTarget.IsChecked = value; }
        }

        bool IPortWindowView.OkButtonEnabled {
            get { return okButton.IsEnabled; }
            set { okButton.IsEnabled = value; }
        }

        bool IPortWindowView.CompareButtonEnabled {
            get { return compareButton.IsEnabled; }
            set { compareButton.IsEnabled = value; }
        }

        bool IPortWindowView.BranchSelectorEnabled {
            get { return branchSelector.IsEnabled; }
            set { branchSelector.IsEnabled = value; }
        }

        int IPortWindowView.SelectedBranchIndex {
            get { return branchSelector.SelectedIndex; }
            set { branchSelector.SelectedIndex = value; }
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
            MessageBox.Show(this, message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void IPortWindowView.ShowInfo(string title, string message) {
            MessageBox.Show(this, message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        bool IPortWindowView.ShowQuestion(string title, string message) {
            return MessageBox.Show(this, message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        void IPortWindowView.ShowModal() {
            ShowDialog();
        }
        #endregion
    }
}