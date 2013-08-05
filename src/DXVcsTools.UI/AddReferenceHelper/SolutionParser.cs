using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using DevExpress.Utils.Text.Internal;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Mvvm.Native;
using DXVcsTools.UI.Navigator;
using Microsoft.Build.Construction;
using Microsoft.Build.Execution;

namespace DXVcsTools.UI {
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
            object solution = GetSolutionWrapper();
            SetSolutionPath(solution, solutionPath);
            DoParse(solution);
            return solution;
        }
        public IEnumerable<string> Parse() {
            object solution = GetSolution();
            return GetAssembliesForAddReference(solution);
        }
        IEnumerable<string> GetAssembliesForAddReference(object solutionWrapper) {
            var items = (IEnumerable)solutionWrapper.GetType().GetProperty("ProjectsInOrder", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(solutionWrapper);
            if (items == null || !items.Cast<object>().Any())
                yield break;
            foreach (object projectItem in items) {
                ProjectRootElement root = ProjectRootElement.Open(GetAbsolutePath(projectItem));
                string location = root.DirectoryPath;
                foreach (var item in GetDXReferences(root))
                    yield return GetAsemblyPath(location, item.Include, item.Metadata.FirstOrDefault(x => x.Name == "HintPath").With(x => x.Value));
                yield return root.Properties.First(item => item.Name == "AssemblyName").Value;
            }
        }
        string GetAsemblyPath(string location, string assemblyInclude, string assemblyHintPath) {
            if (!assemblyInclude.Contains("PublicKeyToken"))
                return assemblyInclude;
            return assemblyHintPath == null ? string.Empty : Path.Combine(location, assemblyHintPath);
        }
        IEnumerable<ProjectItemElement> GetDXReferences(ProjectRootElement root) {
            return root.Items.Where(item => item.ItemType == "Reference").Where(item => item.Include.StartsWith("DevExpress"));
        }
        string GetAbsolutePath(object projectItem) {
            return (string)projectItem.GetType().GetProperty("AbsolutePath", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(projectItem);
        }
        void SetSolutionPath(object solution, string path) {
            var propertyInfo = solution.GetType().GetProperty("SolutionFile", BindingFlags.Instance | BindingFlags.NonPublic);
            propertyInfo.SetValue(solution, path);
        }
        void DoParse(object solution) {
            var methodInfo = solution.GetType().GetMethod("ParseSolutionFile", BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo.Invoke(solution, null);
        }
        object GetSolutionWrapper() {
            return typeof(BuildManager).Assembly.CreateInstance("Microsoft.Build.Construction.SolutionParser", true, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { }, CultureInfo.CurrentCulture, null);
        }
        IEnumerable GetProjects(object solution) {
            return (IEnumerable)solution.GetType().GetProperty("ProjectsInOrder", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(solution);
        }
        public ProjectType GetProjectType() {
            try {
                object solution = GetSolution();
                var items = GetProjects(solution);
                if (items == null || !items.Cast<object>().Any())
                    return ProjectType.Unknown;
                foreach (var projectItem in items) {
                    ProjectRootElement root = ProjectRootElement.Open(GetAbsolutePath(projectItem));
                    string guides = root.Properties.FirstOrDefault(x => x.Name == "ProjectTypeGuids").With(x => x.Value);
                    if (string.IsNullOrEmpty(guides)) {
                        foreach (string strguid in guides.Split(';')) {
                            Guid guid = Guid.Parse(strguid);
                            if (guid == Wpf)
                                return ProjectType.WPF;
                            if (guid == SL)
                                return ProjectType.SL;
                        }
                    }

                    string defines = root.Properties.FirstOrDefault(x => x.Name == "DefineConstants").With(x => x.Value);
                    if (string.IsNullOrEmpty(defines))
                        return ProjectType.Unknown;
                    if (defines.Contains("WPF"))
                        return ProjectType.WPF;
                    if (defines.Contains("SL") || defines.Contains("SILVERLIGHT"))
                        return ProjectType.SL;
                }
                return ProjectType.Unknown;
            }
            catch {
            }
            return ProjectType.Unknown;
        }
    }
}
