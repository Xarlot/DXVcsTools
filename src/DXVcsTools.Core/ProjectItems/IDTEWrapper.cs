using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcsTools.Core {
    public interface IDteWrapper {
        SolutionItem GetSolution();
        IEnumerable<ProjectItem> GetProjects();
        IEnumerable<FileItemBase> GetFiles();
        bool IsCheckedOut(FileItemBase file);
    }
}
