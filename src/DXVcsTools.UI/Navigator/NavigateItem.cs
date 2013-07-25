namespace DXVcsTools.UI.Navigator {
    public class NavigateItem {
        public bool Used { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool ShouldSerializeUsed() {
            return false;
        }
    }
}
