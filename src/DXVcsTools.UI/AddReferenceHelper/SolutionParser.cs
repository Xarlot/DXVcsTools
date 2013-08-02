using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using DevExpress.Utils.Text.Internal;
using DevExpress.Xpf.Mvvm.Native;
using Microsoft.Build.Construction;
using Microsoft.Build.Execution;

namespace DXVcsTools.UI.AddReferenceHelper {
    public class SolutionParser {
        readonly string solutionPath;
        public SolutionParser(string path) {
            solutionPath = path;
        }
        public IEnumerable<string> Parse() {
            object solutionWrapper = GetSolutionWrapper();
            SetSolutionPath(solutionWrapper, solutionPath);
            DoParse(solutionWrapper);
            return GetAssembliesForAddReference(solutionWrapper);
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
            return typeof(BuildManager).Assembly.CreateInstance("Microsoft.Build.Construction.SolutionParser", true, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] {}, CultureInfo.CurrentCulture, null);
        }
    }
}
