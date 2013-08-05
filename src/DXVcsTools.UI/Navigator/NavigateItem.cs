namespace DXVcsTools.UI.Navigator {
    public enum ProjectType {
        Unknown,
        NoPlatform,
        WPF,
        SL,
        Win,
        WinRT,
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
    }
}
