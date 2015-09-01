using System;

namespace DXVcsTools.Core {
    public class SolutionWrapper : IProjectWrapper {
        public SolutionWrapper(EnvDTE.Solution item) {
            Item = item;
        }
        public EnvDTE.Solution Item { get; set; }
        public string FullPath { get { return Item.Properties.Item("FullName").Value.ToString(); } }
        public string Name { get { return Item.FullName; } }
        public string FullName { get { return Item.FullName; } }
        public bool IsSaved {
            get { return Item.Saved; }
        }
        public void Save() {
        }
        public void Open() {
            throw new NotSupportedException();
        }
    }
    public class ProjectWrapper : IProjectWrapper {
        public ProjectWrapper(EnvDTE.Project item) {
            Item = item;
        }
        public EnvDTE.Project Item { get; set; }
        public string FullPath { get { return Item.Properties.Item("FullName").Value.ToString(); } }
        public string Name { get { return Item.Name; } }
        public string FullName { get { return Item.FullName; } }
        public bool IsSaved {
            get { return Item.Saved; }
        }
        public void Save() {
            if (!IsSaved)
                Item.Save();
        }
        public void Open() {
            throw new NotSupportedException();
        }
    }
    public class ProjectItemWrapper : IProjectItemWrapper {
        public ProjectItemWrapper(EnvDTE.ProjectItem item) {
            Item = item;
        }
        EnvDTE.ProjectItem Item { get; set; }
        public string FullPath { get { return Item.Properties.Item("FullName").Value.ToString(); }}
        public bool IsSaved {
            get { return Item.Saved; }
        }
        public void Save() {
            if (!IsSaved)
                Item.Save();
        }
        public void Open() {
            if (Item.IsOpen)
                Item.Document.Activate();
            else {
                var win = Item.Open(EnvDTE.Constants.vsViewKindCode);
                win.Visible = true;
                win.SetFocus();
            }
        }
    }
}