using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXVcsTools.Core;

namespace DXVcsTools.UI {
    public class PortOptionsViewModel {
        public OptionsViewModel Options { get; private set; }
        public IEnumerable<string> Branches { get; private set; }
        public string ProjectPath { get; private set; }
        DXVcsFileLocator Locator { get; set; }
        public PortOptionsViewModel(string projectPath, OptionsViewModel options) {
            Options = options;
            ProjectPath = projectPath;

            Locator = new DXVcsFileLocator(new DXVcsBindingInfo());
            Branches = new ReadOnlyCollection<string>(GetBranchesList());
        }
        IList<string> GetBranchesList() {
            return Options.Branches.Select(branch => branch.Path).ToList();
        }
        public string GetRelativePath(string filePath) {
            return GetRelativePath(filePath, ProjectPath);
        }
        public string GetRelativePath(string filePath, string projectPath) {
            string server;
            return Locator.GetVcsLocation(filePath, projectPath, out server);
        }
    }
}
