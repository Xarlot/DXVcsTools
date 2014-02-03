using System;
using System.Collections.Generic;

namespace DXVcsTools.Core {
    public enum VSTheme {
        Dark, 
        Light, 
        Blue,
        Unknown,
    }
    public interface IDteWrapper {
        SolutionItem BuildTree();
        void OpenSolution(string path);
        void ReloadProject();
        string GetVSTheme(Func<VSTheme, string> getThemeFunc);
        void NavigateToFile(ProjectItemBase item);
        string GetActiveDocument();
        int? GetSelectedLine();
        bool IsItemUnderScc(string fileName);
        void AddReference(string assembly);
        void ClearReferences();
        IEnumerable<IReferenceWrapper> GetReferences(Predicate<IReferenceWrapper> predicate);
        void AddProjectReference(string path);
        void ClearProjectReferences();
    }    
    public interface IReferenceWrapper{
        string Name { get; }
        int MajorVersion { get; }
        int MinorVersion { get; }
        //Project SourceProject { get; }

        void Remove();
    }
}