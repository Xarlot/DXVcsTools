using System;
using System.IO.Packaging;
using DXVcsTools.Core;
using DXVcsTools.UI.Navigator;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.Xpf.Mvvm.Native;
using System.Collections.Generic;
using Microsoft.Build.Construction;

namespace DXVcsTools.UI {
    public class DXControlsVersionHelper {
        static readonly Regex versionRegex;
        static DXControlsVersionHelper() {
            versionRegex = new Regex(".v\\d{2}.\\d");
        }
        public static bool HasDXVersionInfo(string value) {
            if(value == null)
                return false;
            return versionRegex.Match(value).Success;
        }
        public static string GetDXVersionString(string value) {
            if(value == null)
                return null;
            return versionRegex.Match(value).Value;
        }
        public static string ReplaceDXVersion(string target, string versionString) {
            if(target == null || versionString == null) 
                return target;
            if(!HasDXVersionInfo(target) || !HasDXVersionInfo(versionString) || !GetDXVersionString(versionString).Equals(versionString))
                return target;
            return versionRegex.Replace(target, versionString);
        }
    }
    public class AddReferenceHelper {
        public void AddReferences(IDteWrapper dte, NavigateItem item) {
            try {
                BusyIndicator.Show();
                var dxReferences = dte.GetReferences(x => x.Name.Contains("DevExpress")).ToList();
                foreach(var reference in dxReferences)
                    reference.Remove();                
                SolutionParser parser = new SolutionParser(item.Path);
                var newAssemblies = parser.GetReferencedAssemblies(true);
                var newVersion = newAssemblies.FirstOrDefault(DXControlsVersionHelper.HasDXVersionInfo).With(DXControlsVersionHelper.GetDXVersionString);                                

                var modifiedReferences = dxReferences.Select(x => DXControlsVersionHelper.ReplaceDXVersion(x.Name, newVersion));
                var model = SerializeHelper.DeSerializeNavigationConfig();

                var projects = model.NavigateItems.Where(x => x.GeneratedProjects != null).SelectMany(x => x.GeneratedProjects).Distinct();
                List<string> currentProjects = projects.AsParallel().Where(project => {
                    var projectRoot = ProjectRootElement.Open(project);
                    if(project.Contains("Localization") || SolutionParser.GetProjectType(projectRoot).Conflicts(parser.GetProjectType()))
                        return false;
                    var assemblyNameProperty = SolutionParser.GetAssemblyNameFromProject(projectRoot).If(DXControlsVersionHelper.HasDXVersionInfo);
                    if(assemblyNameProperty == null)
                        return false;
                    if(modifiedReferences.Contains(assemblyNameProperty))
                        return true;
                    return false;
                }).ToList();                
                var updatedReferences = currentProjects.AsParallel().SelectMany(x => SolutionParser.GetDXReferencePaths(x, true)).Where(x => !String.IsNullOrEmpty(x)).ToList();
                var actualReferences = newAssemblies.Concat(updatedReferences).Distinct();
                foreach(var reference in actualReferences)
                    try {
                        dte.AddReference(reference);               
                    } catch { }                    
            }
            finally {
                BusyIndicator.Close();
            }
        }
        public void AddProjectReferences(IDteWrapper dte, NavigateItem item) {
            try {
                BusyIndicator.Show();
                dte.ClearReferences();
                dte.ClearProjectReferences();
                SolutionParser parser = new SolutionParser(item.Path);
                foreach (var assembly in parser.GetReferencedAssemblies(false))
                    dte.AddReference(assembly);
                foreach (var path in parser.GetProjectPathes())
                    dte.AddProjectReference(path);
            }
            finally {
                BusyIndicator.Close();
            }
        }
        public ProjectType GetProjectType(string path) {
            SolutionParser parser = new SolutionParser(path);
            return parser.GetProjectType();
        }
    }
}
