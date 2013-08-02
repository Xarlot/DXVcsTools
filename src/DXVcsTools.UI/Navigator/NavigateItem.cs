namespace DXVcsTools.UI.Navigator {
    public class NavigateItem {
        public bool Used { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string RelativePath { get; set; }
        public bool UsedForAddReference { get; set; }
        public string ProjectType { get; set; }
    }
}
