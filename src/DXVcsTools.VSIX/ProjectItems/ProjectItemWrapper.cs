namespace DXVcsTools.Core {
    public class ProjectItemWrapper : IProjectItemWrapper {
        public ProjectItemWrapper(EnvDTE.ProjectItem item) {
            Item = item;
        }
        EnvDTE.ProjectItem Item { get; set; }
        public bool IsSaved {
            get { return Item.Saved; }
        }
        public void Save() {
            if (!IsSaved)
                Item.Save();
        }
    }
}