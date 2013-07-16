using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.Xpf.Mvvm.Native;
using DXVcsTools.UI;
using DXVcsTools.UI.Logger;
using EnvDTE;
using EnvDTE80;

namespace DXVcsTools.Core {
    public class DteWrapper : IDteWrapper {
        readonly DTE2 dte;
        public DteWrapper(DTE dte) {
            this.dte = (DTE2)dte;
        }
        public SolutionItem BuildTree() {
            return new SolutionItem(GetProjects(dte.Solution).ToList()) {Name = dte.Solution.FullName, Path = dte.Solution.FileName};
        }
        IEnumerable<ProjectItem> GetProjects(EnvDTE.Solution solution) {
            string name = solution.FullName;
            return solution.Projects.Cast<Project>().Select(item => new ProjectItem(GetFilesAndDirectories(item).ToList()) {Name = name, Path = item.FileName});
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
                item = new FileItem(GetChildrenItems(projectItem).ToList()) { Name = fileInfo.Name, Path = fileName };
            }
            if (Directory.Exists(fileName)) {
                var info = new DirectoryInfo(fileName);
                item = new FolderItem(GetChildrenItems(projectItem).ToList()) {Name = info.Name, Path = fileName};
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
        const string CategoryTextGeneral = "General";
        const string PropertyNameCurrentTheme = "CurrentTheme";
        const string ThemeLight = "de3dbbcd-f642-433c-8353-8f1df4370aba";
        const string ThemeDark = "1ded0138-47ce-435e-84ef-9ec1f439b749";
        string GetThemeId() {
            return Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\VisualStudio\11.0\" + CategoryTextGeneral, PropertyNameCurrentTheme, "").ToString();
        }
        public string GetVSTheme(Func<VSTheme,string> getThemeFunc) {
            switch (GetThemeId()) {
                case ThemeDark:
                    return getThemeFunc(VSTheme.Dark);
                case ThemeLight:
                    return getThemeFunc(VSTheme.Light);
                default:
                    return getThemeFunc(VSTheme.Unknown);
            }
        }
        public void ReloadProject() {
            Logger.AddInfo("ReloadProjectCommand. Start.");
            try {
                var solExp = dte.ToolWindows.SolutionExplorer;
                solExp.Parent.Activate();
                dte.ExecuteCommand("View.Refresh", string.Empty);
            }
            catch (Exception e) {
                Logger.AddError("ReloadProjectCommand. Failed.", e);
            }
            Logger.AddInfo("ReloadProjectCommand. End.");
        }
        public void NavigateToFile(ProjectItemBase item) {
            Logger.AddInfo("NavigateToFileCommand. Start.");
            try {
                item.Open();
            }
            catch (Exception e) {
                Logger.AddError("NavigateToFileCommand. Failed.", e);
            }
            Logger.AddInfo("NavigateToFileCommand. Ehd.");
        }
    }
}