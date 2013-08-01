using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using DevExpress.Utils.Text.Internal;
using Microsoft.Build.Construction;
using Microsoft.Build.Execution;

namespace DXVcsTools.UI.AddReferenceHelper {
    public class SolutionParser {
        string solutionPath;
        object solutionWrapper;
        public SolutionParser(string path) {
            solutionPath = path;
        }
        public IEnumerable<string> Parse() {
            solutionWrapper = typeof(BuildManager).Assembly.CreateInstance("Microsoft.Build.Construction.SolutionParser", true, BindingFlags.NonPublic | BindingFlags.Instance, null , new object[] { }, CultureInfo.CurrentCulture, null);
            SetSolutionPath(solutionWrapper, solutionPath);
            DoParse(solutionWrapper);
            foreach (object projectItem in (IEnumerable)solutionWrapper.GetType().GetProperty("ProjectsInOrder", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(solutionWrapper)) {
                ProjectRootElement root = ProjectRootElement.Open(GetAbsolutePath(projectItem));
                foreach (var item in GetDXReferences(root))
                    yield return item;
                yield return root.Properties.First(item => item.Name == "AssemblyName").Value;
            }
        }
        IEnumerable<string> GetDXReferences(ProjectRootElement root) {
            return root.Items.Where(item => item.ItemType == "Reference").Where(item => item.Include.StartsWith("DevExpress")).Select(item => item.Include);
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
    }
}
