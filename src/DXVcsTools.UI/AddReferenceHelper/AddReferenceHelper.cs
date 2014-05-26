using System;
using System.IO.Packaging;
using DXVcsTools.Core;
using DXVcsTools.UI.Navigator;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.Mvvm.Native;
using System.Collections.Generic;
using Microsoft.Build.Construction;

namespace DXVcsTools.UI {
    public class DXControlsVersionHelper {
        static readonly Regex versionRegex;
        static readonly Regex dxReferenceRegex;
        static DXControlsVersionHelper() {
            versionRegex = new Regex(".v\\d{2}.\\d");
            dxReferenceRegex = new Regex("DevExpress[^,\\s]*");
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
        public static string SimplifyDXVersion(string source) {
            return dxReferenceRegex.Match(source).If(x => x.Success).With(x => x.Value);
        }
    }
    public enum MultiReferenceType { Assembly, Project };
    public class MultiReference {
        MultiReferenceType type;
        public string AssemblySource { get; private set; }
        public string ProjectSource { get; private set; }
        public ReferenceInfo ReferenceSource { get; private set; }
        public string FullAssemblySource { get { return ReferenceSource.With(x => x.FullName ?? x.Name) ?? AssemblySource; } }
        public MultiReferenceType Type { get { return type; } set { type = value; hCode = type == MultiReferenceType.Assembly ? aHCode : pHCode; } }
        static int pHCode = "ProjectSource".GetHashCode();
        static int aHCode = "AssemblySource".GetHashCode();
        int hCode = 0;
        public MultiReference(string source, MultiReferenceType type) {
            if(type == MultiReferenceType.Assembly) {
                AssemblySource = source;
            } else {
                ProjectSource = source;
                AssemblySource = SolutionParser.GetAssemblyNameFromProject(ProjectSource);
            }
            this.Type = type;
        }
        public MultiReference(ReferenceInfo source)
            : this(source.Name, MultiReferenceType.Assembly) {
            this.ReferenceSource = source;
        }
        public void ReplaceVersion(string newVersion) {
            AssemblySource = DXControlsVersionHelper.ReplaceDXVersion(AssemblySource, newVersion);
        }
        public override string ToString() {
            return Convert.ToString(AssemblySource);
        }
        public override bool Equals(object obj) {
            var mRef = obj as MultiReference;
            if(mRef == null)
                return base.Equals(obj);
            return mRef.ProjectSource == ProjectSource && mRef.AssemblySource == AssemblySource;
        }
        public override int GetHashCode() {
            return AssemblySource.GetHashCode() | Type.GetHashCode() | hCode;
        }
    }
    public class AddReferenceHelper {
        public void AddReferences(IDteWrapper dte, NavigateItem item) {
            AddReferencesImpl(dte, item, false);
        }
        public void AddReferencesImpl(IDteWrapper dte, NavigateItem item, bool addAsProjectReference) {
            dte.LockCurrentProject();
            try {
                BusyIndicator.Show();
                
                SolutionParser parser = new SolutionParser(item.Path);
                var projectType = parser.GetProjectType();

                var newAssemblies = parser.GetReferencedAssemblies(!addAsProjectReference).Select(x => new MultiReference(x));
                var newVersion = newAssemblies.Select(x=>x.AssemblySource).FirstOrDefault(DXControlsVersionHelper.HasDXVersionInfo).With(DXControlsVersionHelper.GetDXVersionString);

                var updatedReferences = ChangeVersion(dte, projectType, newVersion);

                dte.ClearReferences();
                dte.ClearProjectReferences();

                var actualReferences = Concat(newAssemblies, updatedReferences);
                if(addAsProjectReference) {
                    actualReferences = Concat(actualReferences, parser.GetProjectPathes().Select(x => new MultiReference(x, MultiReferenceType.Project)));
                }
                foreach(var reference in actualReferences)
                    try {
                        if(reference.Type == MultiReferenceType.Assembly)
                            dte.AddReference(projectType == ProjectType.SL ? reference.FullAssemblySource : reference.AssemblySource);
                        else
                            dte.AddProjectReference(reference.ProjectSource);
                    } catch { }                    
            }
            finally {
                BusyIndicator.Close();
                dte.UnlockCurrentProject();
                dte.ActivateConfiguration("DebugTest");
            }
        }
        public static IEnumerable<MultiReference> Concat(IEnumerable<MultiReference> first, IEnumerable<MultiReference> second) {
            var tResult = first.Concat(second).Distinct();
            foreach(var element in tResult) {
                if(element.Type == MultiReferenceType.Assembly && tResult.Any(x => x.Type == MultiReferenceType.Project && DXControlsVersionHelper.SimplifyDXVersion(x.AssemblySource) == DXControlsVersionHelper.SimplifyDXVersion(element.AssemblySource)))
                    continue;                
                yield return element;
            }
        }
        static IEnumerable<MultiReference> ChangeVersion(IDteWrapper dte, ProjectType projectType, string newVersion) {
            var assemblyReferences = dte.GetReferences(x => x.Name.Contains("DevExpress")).Select(x => new MultiReference(x.Name, MultiReferenceType.Assembly).Do(mr=>mr.ReplaceVersion(newVersion)));
            var projectReferences = dte.GetProjects(x => x.Name.Contains("DevExpress")).Select(x => new MultiReference(x.FullName, MultiReferenceType.Project).Do(mr=>mr.ReplaceVersion(newVersion)));

            var assemplyReferenceNames = assemblyReferences.Select(x => x.AssemblySource);
            var projectReferenceNames = projectReferences.Select(x => x.AssemblySource);
            
            var model = SerializeHelper.DeSerializeNavigationConfig();

            var projects = model.NavigateItems.Where(x => x.GeneratedProjects != null).SelectMany(x => x.GeneratedProjects).Distinct();
            var targetProjects = projects.AsParallel().Select(x => new MultiReference(x, MultiReferenceType.Project)).Where(project => {
                var projectRoot = ProjectRootElement.Open(project.ProjectSource);
                if(project.ProjectSource.Contains("Localization") || SolutionParser.GetProjectType(projectRoot).Conflicts(projectType))
                    return false;
                var assemblyNameProperty = SolutionParser.GetAssemblyNameFromProject(projectRoot).If(DXControlsVersionHelper.HasDXVersionInfo);
                if(assemblyNameProperty == null)
                    return false;
                if(projectReferenceNames.Contains(assemblyNameProperty)) {
                    project.Type = MultiReferenceType.Project;
                    return true;
                }
                if(assemplyReferenceNames.Contains(assemblyNameProperty)) {
                    project.Type = MultiReferenceType.Assembly;
                    return true;
                }
                return false;
            });
            var dependentProjects = targetProjects.SelectMany(x => SolutionParser.GetDXReferencePaths(x.ProjectSource, true)).Where(x => !ReferenceInfo.IsEmpty(x)).Select(x => new MultiReference(x));
            return Concat(targetProjects.ToList(), dependentProjects.ToList());
        }

        public void AddProjectReferences(IDteWrapper dte, NavigateItem item) {
            AddReferencesImpl(dte, item, true);
        }
        public ProjectType GetProjectType(string path) {
            SolutionParser parser = new SolutionParser(path);
            return parser.GetProjectType();
        }
    }
}
