using System;
using System.IO.Packaging;
using DXVcsTools.Core;
using DXVcsTools.UI.Navigator;

namespace DXVcsTools.UI {
    public class AddReferenceHelper {
        public void AddReferences(IDteWrapper dte, NavigateItem item) {
            dte.ClearReferences();
            SolutionParser parser = new SolutionParser(item.Path);
            foreach (var assembly in parser.Parse()) {
                dte.AddReference(assembly);
            }
        }
        public ProjectType GetProjectType(string path) {
            SolutionParser parser = new SolutionParser(path);
            return parser.GetProjectType();
        }
    }
}
