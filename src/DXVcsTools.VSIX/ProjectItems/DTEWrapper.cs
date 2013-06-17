﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.Xpf.Mvvm.Native;
using EnvDTE;

namespace DXVcsTools.Core {
    public class DteWrapper : IDteWrapper {
        readonly DTE dte;
        public DteWrapper(DTE dte) {
            this.dte = dte;
        }
        public SolutionItem BuildTree() {
            return new SolutionItem(GetProjects(dte.Solution)) {Name = dte.Solution.FullName, Path = dte.Solution.FileName};
        }
        IEnumerable<ProjectItem> GetProjects(Solution solution) {
            string name = solution.FullName;
            return solution.Projects.Cast<Project>().Select(item => new ProjectItem(GetFilesAndDirectories(item)) {Name = name, Path = item.FileName});
        }
        IEnumerable<FileItemBase> GetFilesAndDirectories(Project project) {
            ProjectItems children = project.ProjectItems;
            if (children.If(x => x.Count == 0).ReturnSuccess())
                yield break;
            foreach (EnvDTE.ProjectItem projectItem in children) {
                FileItemBase item = GetItem(projectItem);
                if (item != null)
                    yield return item;
            }
        }
        FileItemBase GetItem(EnvDTE.ProjectItem projectItem) {
            if (projectItem.FileCount < 1)
                return null;
            string fileName = null;
            try {
                fileName = projectItem.FileNames[0];
            }
            catch {
            }
            if (fileName == null)
                return null;
            FileItemBase item = null;
            if (File.Exists(fileName)) {
                var fileInfo = new FileInfo(fileName);
                item = new FileItem {Name = fileInfo.Name, Path = fileName};
            }
            if (Directory.Exists(fileName)) {
                var info = new DirectoryInfo(fileName);
                item = new FolderItem(GetChildrenItems(projectItem)) {Name = info.Name, Path = fileName};
            }
            if (item == null)
                return null;
            item.ItemWrapper = new ProjectItemWrapper(projectItem);
            item.IsCheckOut = dte.SourceControl.IsItemCheckedOut(fileName);
            item.MergeState = MergeState.None;
            return item;
        }
        IEnumerable<FileItemBase> GetChildrenItems(EnvDTE.ProjectItem projectItem) {
            return projectItem.ProjectItems.Cast<EnvDTE.ProjectItem>().Select(GetItem).Where(item => item != null);
        }
        public void OpenSolution(string filePath) {
            dte.Solution.Open(filePath);
        }
    }
}