using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcsTools.Core {
    public class SolitionEnumeratorHelper {
        IDteWrapper dte;
        SolutionItem solution;
        IEnumerable<ProjectItem> projectItems;
        IEnumerable<FileItemBase> files;
        public SolitionEnumeratorHelper(IDteWrapper dte) {
            this.dte = dte;
        }
        public void Initialize() {
            solution = dte.GetSolution();
            projectItems = dte.GetProjects();
            files = dte.GetFiles();
        }

    }
}
