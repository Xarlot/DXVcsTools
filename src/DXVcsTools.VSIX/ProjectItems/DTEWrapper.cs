using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace DXVcsTools.Core {
    public class DTEWrapper : IDteWrapper {
        DTE dte;
        public DTEWrapper(DTE dte) {
            this.dte = dte;
        }
        public SolutionItem GetSolution() {
            throw new NotImplementedException();
        }
        public IEnumerable<ProjectItem> GetProjects() {
            throw new NotImplementedException();
        }
        public IEnumerable<FileItemBase> GetFiles() {
            throw new NotImplementedException();
        }
        public bool IsCheckedOut(FileItemBase file) {
            throw new NotImplementedException();
        }
    }
}
