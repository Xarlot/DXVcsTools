using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using DXVcsTools.Core;
using DXVcsTools.DXVcsClient;

namespace DXVcsTools.UI {
    public class PortOptionsViewModel {
        public PortOptionsViewModel(SolutionItem solution, OptionsViewModel options) {
            Options = options;

            Locator = new DXVcsFileLocator(new DXVcsBindingInfo());
            Branches = new ReadOnlyCollection<string>(GetBranchesList());
            string server;
            string filePath;
            CalcProjectPath(solution, out filePath, out server);
            VcsServer = server;
            ProjectFilePath = filePath;
        }
        string CalcProjectPath(SolutionItem solution, out string projectFilePath, out string serverPath) {
            string result = string.Empty;
            serverPath = string.Empty;
            projectFilePath = string.Empty;
            try {
                result = GetProjectPath(solution.Path, out serverPath);
                projectFilePath = solution.Path;
            }
            catch {
                foreach (var project in solution.Children) {
                    try {
                        result = GetProjectPath(project.Path, out serverPath);
                        projectFilePath = project.Path;
                        break;
                    }
                    catch {
                    }
                }
            }
            return result;
        }
        string GetProjectPath(string path, out string serverPath) {
            return Locator.GetVcsLocation(path, path, out serverPath);
        }
        public bool IsAttached { get { return !string.IsNullOrEmpty(VcsServer); }}
        public OptionsViewModel Options { get; private set; }
        public IEnumerable<string> Branches { get; private set; }
        public string ProjectFilePath { get; private set; }
        DXVcsFileLocator Locator { get; set; }
        public DXVcsBranch MasterBranch { get; set; }
        public string VcsServer { get; private set; }
        IList<string> GetBranchesList() {
            return Options.Branches.Select(branch => branch.Path).ToList();
        }
        public string GetRelativePath(string filePath) {
            return GetRelativePath(filePath, ProjectFilePath);
        }
        public string GetRelativePath(string filePath, DXVcsBranch currentBranch) {
            return GetRelativePath(filePath, GetProjectPath(currentBranch));
        }
        string GetProjectPath(DXVcsBranch currentBranch) {
            if (currentBranch == MasterBranch)
                return ProjectFilePath;
            string vcsPath = GetRelativePath(ProjectFilePath);
            string vcsTargetProjectPath = vcsPath.Replace(MasterBranch.Path, currentBranch.Path);
            IDXVcsRepository repository = DXVcsRepositoryFactory.Create(VcsServer);
            //bug - project file path returns as directory path
            return FindRootProject(repository, repository.GetFileWorkingPath(vcsTargetProjectPath));
        }
        string FindRootProject(IDXVcsRepository repository, string rootPath) {
            string firstCandidate = rootPath + Path.GetFileName(ProjectFilePath);
            if (Locator.IsUnderScc(firstCandidate))
                return firstCandidate;
            foreach (var file in Directory.GetFiles(rootPath, "*.sln", SearchOption.AllDirectories)) {
                if (Locator.IsUnderScc(file))
                    return file;
            }
            return firstCandidate;
        }
        string GetRelativePath(string filePath, string projectPath) {
            string server;
            return Locator.GetVcsLocation(filePath, projectPath, out server);
        }
    }
}