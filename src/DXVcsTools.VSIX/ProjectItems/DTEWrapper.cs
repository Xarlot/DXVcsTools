using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Mvvm.Native;
using EnvDTE;

namespace DXVcsTools.Core {
    public class DteWrapper : IDteWrapper {
        DTE dte;
        public DteWrapper(DTE dte) {
            this.dte = dte;
        }
        public SolutionItem BuildTree() {
            return new SolutionItem(GetProjects(dte.Solution)) { Name = dte.Solution.FullName };
        }
        IEnumerable<ProjectItem> GetProjects(Solution solution) {
            string name = solution.FileName;
            return solution.Projects.Cast<EnvDTE.Project>().Select(item => new ProjectItem(GetFilesAndDirectories(item)) { Name = name });
        }
        IEnumerable<FileItemBase> GetFilesAndDirectories(EnvDTE.Project project) {
            var children = project.ProjectItems;
            if (children.If(x => x.Count == 0).ReturnSuccess())
                yield break;
            foreach (EnvDTE.ProjectItem item in children) {
                FolderItem folder = new FolderItem(GetChildrenItems(item)) { Name = item.Name };
                yield return folder;
            }
        }
        IEnumerable<FileItemBase> GetChildrenItems(EnvDTE.ProjectItem projectItem) {
            if (projectItem.FileCount > 0) {
                for (short i = 0; i < projectItem.FileCount; i++) {
                    var file = projectItem.FileNames[i];
                    yield return new FileItem() { Name = i.ToString(), Path = file };
                }
            }
            var children = projectItem.ProjectItems;
            if (children.If(x => x.Count == 0).ReturnSuccess())
                yield break;
            foreach (EnvDTE.ProjectItem item in children)
                yield return new FolderItem(GetChildrenItems(item));
        }
    }
}
