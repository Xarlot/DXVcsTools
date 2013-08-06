using System;
using System.IO.Packaging;
using DXVcsTools.Core;
using DXVcsTools.UI.Navigator;

namespace DXVcsTools.UI {
    public class AddReferenceHelper {
        public void AddReferences(IDteWrapper dte, NavigateItem item) {
            dte.ClearReferences();
            SolutionParser parser = new SolutionParser(item.Path);
            foreach (var assembly in parser.GetReferencedAssemblies(true))
                dte.AddReference(assembly);
        }
        public void AddProjectReferences(IDteWrapper dte, NavigateItem item) {
            dte.ClearReferences();
            SolutionParser parser = new SolutionParser(item.Path);
            foreach (var assembly in parser.GetReferencedAssemblies(false))
                dte.AddReference(assembly);
            foreach (var path in parser.GetProjectPathes()) 
                dte.AddProjectReference(path);
        }
        public ProjectType GetProjectType(string path) {
            SolutionParser parser = new SolutionParser(path);
            return parser.GetProjectType();
        }
    }
}
