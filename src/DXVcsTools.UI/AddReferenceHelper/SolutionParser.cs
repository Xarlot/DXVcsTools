using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Controls;
using DevExpress.Utils.Text.Internal;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using DXVcsTools.UI.Navigator;
using Microsoft.Build.Construction;
using Microsoft.Build.Execution;

namespace DXVcsTools.UI {
    public class ReferenceInfo {
        public string Name { get; set; }
        public string FullName { get; set; }
        public static bool IsEmpty(ReferenceInfo value){
            if(value == null)
                return true;
            return String.IsNullOrEmpty(value.Name);
        }
    }
    public class SolutionParser {
        public static readonly Guid Unknown = Guid.Empty;
        public static readonly Guid Wpf = Guid.Parse("{60DC8134-EBA5-43B8-BCC9-BB4BC16C2548}");
        public static readonly Guid SL = Guid.Parse("{A1591282-1198-4647-A2B1-27E5FF5F6F3B}");
        public static readonly Guid Win = Guid.Parse("{70E6D39B-F9F4-40DA-BAB2-2C604077941A}");
        readonly string solutionPath;
        public SolutionParser(string path) {
            solutionPath = path;
        }
        object GetSolution() {
            return DoParse();
        }
        public IEnumerable<ReferenceInfo> GetReferencedAssemblies(bool includeRoot) {
            object solution = GetSolution();
            return GetAssembliesForAddReference(solution, includeRoot);
        }
        public IEnumerable<string> GetProjectPathes() {
            object solution = GetSolution();
            return GetProjectPathesInternal(solution);
        }
        IEnumerable<string> GetProjectPathesInternal(object solutionWrapper) {
            if (solutionWrapper == null)
                yield break;
            var items = (IEnumerable)solutionWrapper.GetType().GetProperty("ProjectsInOrder", BindingFlags.Instance | BindingFlags.Public).GetValue(solutionWrapper);
            if (items == null || !items.Cast<object>().Any())
                yield break;
            foreach (object projectItem in items) {
                var absolutePath = GetAbsolutePath(projectItem);                
                if(!IsValidProject(absolutePath))
                    continue;
                ProjectRootElement root = null;
                try {
                    root = ProjectRootElement.Open(absolutePath);
                } catch {
                    continue;
                }
                yield return root.ProjectFileLocation.LocationString;
            }
        }
        IEnumerable<ReferenceInfo> GetAssembliesForAddReference(object solutionWrapper, bool includeRoot) {
            if (solutionWrapper == null)
                yield break;
            var items = (IEnumerable)solutionWrapper.GetType().GetProperty("ProjectsInOrder", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(solutionWrapper);
            if (items == null || !items.Cast<object>().Any())
                yield break;
            foreach (object projectItem in items) {
                foreach(var item in GetDXReferencePaths(GetAbsolutePath(projectItem), includeRoot))
                    yield return item;                
            }
        }
        static string GetAsemblyPath(string location, string assemblyInclude, string assemblyHintPath) {            
            return assemblyHintPath == null ? assemblyInclude : Path.Combine(location, assemblyHintPath);
        }
        static IEnumerable<ProjectItemElement> GetDXReferences(ProjectRootElement root) {
            return root.Items.Where(item => item.ItemType == "Reference").Where(item => item.Include.StartsWith("DevExpress"));
        }
        public static IEnumerable<ReferenceInfo> GetDXReferencePaths(string absolutePath, bool includeRoot) {
            return GetDXReferencePaths(ProjectRootElement.Open(absolutePath), includeRoot);
        }
        public static IEnumerable<ReferenceInfo> GetDXReferencePaths(ProjectRootElement root, bool includeRoot) {
            string location = root.DirectoryPath;
            foreach(var item in GetDXReferences(root))
                yield return new ReferenceInfo() { FullName = GetAsemblyPath(location, item.Include, item.Metadata.FirstOrDefault(x => x.Name == "HintPath").With(x => x.Value)), Name = item.Include };
            if(includeRoot){
                var assemblyName = root.Properties.First(item => item.Name == "AssemblyName").Value;
                yield return new ReferenceInfo() { FullName = Path.Combine(root.DirectoryPath, root.Properties.First(x=>x.Name=="OutputPath").Value, String.Format("{0}.dll",assemblyName) ), Name = assemblyName };
            }
                
        }
        string GetAbsolutePath(object projectItem) {
            return (string)projectItem.GetType().GetProperty("AbsolutePath", BindingFlags.Instance | BindingFlags.Public).GetValue(projectItem);
        }
        void SetSolutionPath(object solution, string path) {
            try {
                var propertyInfo = solution.GetType().GetProperty("SolutionFile", BindingFlags.Instance | BindingFlags.Public);
                propertyInfo.SetValue(solution, path);
            }
            catch { }
        }
        object DoParse() {
            try {
                Type solutionFileType = typeof(BuildManager).Assembly.GetType("Microsoft.Build.Construction.SolutionFile");
                var methodInfo = solutionFileType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public);
                return methodInfo.Invoke(null, new [] { solutionPath });
            }
            catch {
                return null;
            }
        }
        IEnumerable GetProjects(object solution) {
            return solution == null ? Enumerable.Empty<object>() : (IEnumerable)solution.GetType().GetProperty("ProjectsInOrder", BindingFlags.Instance | BindingFlags.Public).GetValue(solution);
        }
        public ProjectType GetProjectType() {
            try {
                object solution = GetSolution();
                var items = GetProjects(solution);
                if (items == null || !items.Cast<object>().Any())
                    return ProjectType.Unknown;
                foreach (var projectItem in items) {
                    var type = GetProjectType(GetAbsolutePath(projectItem));
                    if(type != ProjectType.Unknown)
                        return type;
                }
                return ProjectType.Unknown;
            }
            catch {
            }
            return ProjectType.Unknown;
        }
        public static ProjectType GetProjectType(string source) {
            if(!IsValidProject(source))
                return ProjectType.Unknown;
            return GetProjectType(ProjectRootElement.Open(source));
        }
        public static ProjectType GetProjectType(ProjectRootElement source) {
            string guides = source.Properties.FirstOrDefault(x => x.Name == "ProjectTypeGuids").With(x => x.Value);
            if(!string.IsNullOrEmpty(guides)) {
                foreach(string strguid in guides.Split(';')) {
                    Guid guid = Guid.Parse(strguid);
                    if(guid == Wpf)
                        return ProjectType.WPF;
                    if(guid == SL)
                        return ProjectType.SL;
                }
            }

            string defines = source.Properties.FirstOrDefault(x => x.Name == "DefineConstants").With(x => x.Value);
            if(string.IsNullOrEmpty(defines))
                return ProjectType.Unknown;
            if(defines.Contains("WPF"))
                return ProjectType.WPF;
            if(defines.Contains("SL") || defines.Contains("SILVERLIGHT"))
                return ProjectType.SL;
            return ProjectType.Unknown;
        }
        static bool IsValidProject(string absolutePath) {
            return Path.HasExtension(absolutePath) && File.Exists(absolutePath);
        }

        public static string GetAssemblyNameFromProject(string proj) {
            return GetAssemblyNameFromProject(ProjectRootElement.Open(proj));
        }

        public static string GetAssemblyNameFromProject(ProjectRootElement projectRoot) {
            return projectRoot.Properties.FirstOrDefault(x => String.IsNullOrEmpty(x.Condition) && String.Compare("AssemblyName", x.Name, true) == 0).With(x=>x.Value);
        }
    }
}
