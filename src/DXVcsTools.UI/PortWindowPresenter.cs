using System;
using System.Diagnostics;
using System.IO;
using DXVcsTools.Core;
using DXVcsTools.DXVcsClient;
using DXVcsTools.Version;

namespace DXVcsTools.UI {
    public class PortWindowPresenter {
        readonly PortWindowModel model;
        readonly IPortWindowView view;

        public PortWindowPresenter(IPortWindowView view, PortWindowModel model) {
            this.view = view;
            this.model = model;
        }
        public void Initialize() {
            view.Title = "DXPort v" + VersionInfo.FullVersion;

            view.SourceFile = model.SourceFile;
            view.OriginalFile = model.OriginalVcsFile;
            view.TargetFile = model.TargetVcsFile;

            FillBranchSelector();

            view.ReviewTarget = model.ReviewTarget;
            view.CheckInTarget = model.CheckInTarget;

            view.SourceFileChanged += view_UpdateControls;
            view.OriginalFileChanged += view_UpdateControls;
            view.TargetFileChanged += view_TargetFileChanged;
            view.CheckInCommentChanged += view_UpdateControls;
            view.CheckInTargetToggled += view_UpdateControls;
            view.CancelButtonClick += view_Cancel;
            view.OkButtonClick += view_Submit;
            view.CompareButtonClick += view_CompareButtonClick;
            view.BranchSelectionChanged += view_BranchSelectionChanged;

            SelectAnotherBranch();

            UpdateControls();
        }

        void view_TargetFileChanged(object sender, EventArgs e) {
            model.TargetVcsFile = view.TargetFile;
            UpdateControls();
        }

        void view_UpdateControls(object sender, EventArgs e) {
            UpdateControls();
        }

        void view_Cancel(object sender, EventArgs e) {
            view.Close();
        }

        void view_Submit(object sender, EventArgs e) {
            MergeChanges();
            if (model.CloseAfterMerge)
                view.Close();
        }

        void view_CompareButtonClick(object sender, EventArgs e) {
            CompareWithBranch();
        }

        void view_BranchSelectionChanged(object sender, EventArgs e) {
            ChangeSelectedBranchIndex(view.SelectedBranchIndex);
        }

        void FillBranchSelector() {
            view.FillBranchSelector(model.Branches);
            view.SelectedBranchIndex = model.SelectedBranchIndex;
        }

        void ChangeSelectedBranchIndex(int newBranchIndex) {
            model.SelectedBranchIndex = newBranchIndex;
            view.TargetFile = model.TargetVcsFile;
        }

        void SelectAnotherBranch() {
            int newSelectedIndex = model.Branches.Count - 1;
            if (newSelectedIndex == model.SelectedBranchIndex)
                newSelectedIndex -= 1;
            ChangeSelectedBranchIndex(newSelectedIndex);
            view.SelectedBranchIndex = newSelectedIndex;
        }

        void UpdateControls() {
            if (!view.CanUpdate)
                return;

            if (string.IsNullOrEmpty(view.SourceFile) || string.IsNullOrEmpty(view.OriginalFile) || string.IsNullOrEmpty(view.TargetFile) || view.TargetFile == view.OriginalFile ||
                (view.CheckInTarget && string.IsNullOrEmpty(view.CheckInComment))) {
                view.OkButtonEnabled = false;
            }
            else {
                view.OkButtonEnabled = true;
            }

            if (string.IsNullOrEmpty(view.SourceFile) || string.IsNullOrEmpty(view.TargetFile) || view.TargetFile == view.OriginalFile) {
                view.CompareButtonEnabled = false;
            }
            else {
                view.CompareButtonEnabled = true;
            }

            view.BranchSelectorEnabled = model.TargetVcsFileStartsWithBranch();
        }

        void MergeChanges() {
            try {
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(model.VcsServer);

                string tmpOriginalFile = Path.GetTempFileName();
                try {
                    repository.GetLatestVersion(view.OriginalFile, tmpOriginalFile);

                    string tmpTargetFile = repository.GetFileWorkingPath(view.TargetFile);
                    if (string.IsNullOrEmpty(tmpTargetFile))
                        throw new ApplicationException("Can't proceed because target file doesn't have working directory configured.");

                    repository.CheckOutFile(view.TargetFile, tmpTargetFile, string.Empty);

                    var diff = new FileDiff();
                    if (!diff.Merge(tmpOriginalFile, view.SourceFile, tmpTargetFile)) {
                        view.ShowError(view.Title, "Automatic merge failed, please port changes manually");

                        return;
                    }

                    if (view.ReviewTarget) {
                        ReviewTargetFile(repository, view.TargetFile, tmpTargetFile);
                    }

                    if (view.CheckInTarget) {
                        if (view.ReviewTarget) {
                            if (!view.ShowQuestion(view.Title, "Check in file " + tmpTargetFile + "?"))
                                return;
                        }

                        repository.CheckInFile(view.TargetFile, tmpTargetFile, view.CheckInComment);
                    }

                    if (!view.ReviewTarget)
                        view.ShowInfo(view.Title, "Merge finished successfully.");
                }
                finally {
                    File.Delete(tmpOriginalFile);
                }
            }
            catch (Exception exception) {
                view.ShowError(null, exception.Message);
            }
        }

        void CompareWithBranch() {
            try {
                IDXVcsRepository repository = DXVcsRepositoryFactory.Create(model.VcsServer);
                LaunchDiffTool(view.SourceFile, repository.GetFileWorkingPath(view.TargetFile));
            }
            catch (Exception exception) {
                view.ShowError(null, exception.Message);
            }
        }

        void ReviewTargetFile(IDXVcsRepository repository, string vcsFile, string file) {
            string tmpVcsFile = Path.GetTempFileName();

            try {
                repository.GetLatestVersion(vcsFile, tmpVcsFile);
                LaunchDiffTool(tmpVcsFile, file);
            }
            finally {
                File.Delete(tmpVcsFile);
            }
        }
        void LaunchDiffTool(string leftFile, string rightFile) {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = model.DiffTool;
            startInfo.Arguments = string.Format("\"{0}\" \"{1}\"", leftFile, rightFile);

            Process process = Process.Start(startInfo);
            process.WaitForExit();
        }
    }
}