using System;
using System.Collections.Generic;
namespace DXVcsTools.UI.Navigator {
    [Flags]
    public enum ProjectType : uint {
        Unknown = 0x10,
        NoPlatform = 0x20,
        WPF = 0x101,
        SL = 0x201,
        Win = 0x400,
        WinRT = 0x801,
    }
    //public enum ProjectType {
    //    Unknown,
    //    NoPlatform,
    //    WPF,
    //    SL,
    //    Win,
    //    WinRT,
    //}
    public static class ProjectTypeExtensions {
        public static bool Conflicts(this ProjectType first, ProjectType second) {
            return ((int)first & (int)second) != 0 && (first != second);
        }
    }
    public class NavigateItem {
        bool usedForAddReference;
        public bool Used { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string RelativePath { get; set; }
        public bool UsedForAddReference {
            get { return usedForAddReference; }
            set { usedForAddReference = value; }
        }
        public ProjectType ProjectType { get; set; }
        public List<string> GeneratedProjects { get; set; }
    }
}
