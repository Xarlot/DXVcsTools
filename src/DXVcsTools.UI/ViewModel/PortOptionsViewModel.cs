using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using DXVcsTools.Core;
using DXVcsTools.DXVcsClient;

namespace DXVcsTools.UI {
    public class PortOptionsViewModel {
        public PortOptionsViewModel(string projectPath, OptionsViewModel options) {
            Options = options;
            ProjectPath = projectPath;

            Locator = new DXVcsFileLocator(new DXVcsBindingInfo());
            Branches = new ReadOnlyCollection<string>(GetBranchesList());
            string server;
            Locator.GetVcsLocation(projectPath, projectPath, out server);
            VcsServer = server;
        }
        public OptionsViewModel Options { get; private set; }
        public IEnumerable<string> Branches { get; private set; }
        public string ProjectPath { get; private set; }
        DXVcsFileLocator Locator { get; set; }
        public DXVcsBranch MasterBranch { get; set; }
        public string VcsServer { get; private set; }
        IList<string> GetBranchesList() {
            return Options.Branches.Select(branch => branch.Path).ToList();
        }
        public string GetRelativePath(string filePath) {
            return GetRelativePath(filePath, ProjectPath);
        }
        public string GetRelativePath(string filePath, DXVcsBranch currentBranch) {
            return GetRelativePath(filePath, GetProjectPath(currentBranch));
        }
        string GetProjectPath(DXVcsBranch currentBranch) {
            if (currentBranch == MasterBranch)
                return ProjectPath;
            string vcsPath = GetRelativePath(ProjectPath);
            string vcsTargetProjectPath = vcsPath.Replace(MasterBranch.Path, currentBranch.Path);
            IDXVcsRepository repository = DXVcsRepositoryFactory.Create(VcsServer);
            //bug - project file path returns as directory path
            return repository.GetFileWorkingPath(vcsTargetProjectPath + Path.GetFileName(ProjectPath));
        }
        string GetRelativePath(string filePath, string projectPath) {
            string server;
            return Locator.GetVcsLocation(filePath, projectPath, out server);
        }
    }
}