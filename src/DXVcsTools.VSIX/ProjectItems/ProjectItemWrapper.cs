using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcsTools.Core {
    public class ProjectItemWrapper : IProjectItemWrapper {
        EnvDTE.ProjectItem Item { get; set; }
        public ProjectItemWrapper(EnvDTE.ProjectItem item) {
            Item = item;
        }
        public bool IsSaved { get { return Item.Saved; } }
        public void Save() {
            if (!IsSaved)
                Item.Save();
        }
    }
}
