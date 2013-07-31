using System.Globalization;
using System.Reflection;
using DevExpress.Utils.Text.Internal;
using Microsoft.Build.Execution;

namespace DXVcsTools.UI.AddReferenceHelper {
    public class SolutionParser {
        string solutionPath;
        object solutionWrapper;
        public SolutionParser(string path) {
            solutionPath = path;
        }
        public void Parse() {
            solutionWrapper = typeof(BuildManager).Assembly.CreateInstance("Microsoft.Build.Construction.SolutionParser", true, BindingFlags.NonPublic | BindingFlags.Instance, null , new object[] { }, CultureInfo.CurrentCulture, null);
            SetSolutionPath(solutionWrapper, solutionPath);
        }
        void SetSolutionPath(object solution, string path) {
            var propertyInfo = solution.GetType().GetProperty("SolutionFile", BindingFlags.Instance | BindingFlags.NonPublic);
            propertyInfo.SetValue(solution, path);
        }
    }
}
