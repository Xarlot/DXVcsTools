using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DXVcsTools.Core;

namespace DXVcsTools.UI {
    public class PortWindowModel {
        readonly IList<string> branches;
        readonly DXPortConfiguration configuration;
        readonly string vcsServer;
        int selectedBranchIndex;
        string targetVcsFile;
        public PortWindowModel(string sourceFile, string projectFile, DXPortConfiguration configuration) {
            if (string.IsNullOrEmpty(sourceFile))
                throw new ArgumentException("sourceFile");

            if (string.IsNullOrEmpty(projectFile))
                throw new ArgumentException("projectFile");

            if (configuration == null)
                throw new ArgumentNullException("configuration");

            SourceFile = sourceFile;
            ProjectFile = projectFile;
            this.configuration = configuration;

            var locator = new DXVcsFileLocator(new DXVcsBindingInfo());
            OriginalVcsFile = locator.GetVcsLocation(sourceFile, projectFile, out vcsServer);
            TargetVcsFile = OriginalVcsFile;

            branches = new ReadOnlyCollection<string>(GetBranchesList());

            for (int i = 0; i < branches.Count; i++) {
                if (targetVcsFile.StartsWith(branches[i])) {
                    selectedBranchIndex = i;
                    break;
                }
            }
        }

        public string SourceFile { get; private set; }
        public string ProjectFile { get; private set; }
        public string OriginalVcsFile { get; private set; }

        public bool ReviewTarget {
            get { return configuration.ReviewTarget; }
        }
        public bool CheckInTarget {
            get { return configuration.CheckInTarget; }
        }
        public bool CloseAfterMerge {
            get { return configuration.CloseAfterMerge; }
        }
        public string DiffTool {
            get { return configuration.DiffTool; }
        }

        public IList<string> Branches {
            get { return branches; }
        }

        public int SelectedBranchIndex {
            get { return selectedBranchIndex; }
            set {
                targetVcsFile = targetVcsFile.Replace(branches[selectedBranchIndex], branches[value]);
                selectedBranchIndex = value;
            }
        }

        public string TargetVcsFile {
            get { return targetVcsFile; }
            set {
                if (value == null)
                    throw new ArgumentNullException("value");
                targetVcsFile = value;
            }
        }

        public string VcsServer {
            get { return vcsServer; }
        }

        // public bool IsKnownTargetVcsFileBranch() - proposed rename
        public bool TargetVcsFileStartsWithBranch() {
            foreach (string branch in branches)
                if (TargetVcsFile.StartsWith(branch))
                    return true;
            return false;
        }

        IList<string> GetBranchesList() {
            IList<string> branchesList = new List<string>();
            foreach (DXVcsBranch branch in configuration.Branches)
                branchesList.Add(branch.Path);
            return branchesList;
        }
    }
}