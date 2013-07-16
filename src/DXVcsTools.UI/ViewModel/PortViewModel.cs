using System;
using System.Collections.Generic;

namespace DXVcsTools.UI {
    public class PortViewModel {
        readonly IList<string> branches;
        readonly OptionsViewModel configuration;
        readonly string vcsServer;
        int selectedBranchIndex;
        string targetVcsFile;
        public PortViewModel(PortOptionsViewModel portOptions, OptionsViewModel configuration) {
            PortOptions = portOptions;
            Options = configuration;
        }

        PortOptionsViewModel PortOptions { get; set; }
        OptionsViewModel Options { get; set; }
        public string VcsServer {
            get { return vcsServer; }
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
        public bool TargetVcsFileStartsWithBranch() {
            foreach (string branch in branches)
                if (TargetVcsFile.StartsWith(branch))
                    return true;
            return false;
        }
    }
}