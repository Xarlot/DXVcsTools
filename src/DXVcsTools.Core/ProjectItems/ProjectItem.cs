using System.Collections.Generic;

namespace DXVcsTools.Core {
    public class ProjectItem : ProjectItemBase {
        public ProjectItem(IEnumerable<FileItemBase> items) : base(items) {
        }
        public override bool IsNew {
            get { return false; }
            set { }
        }
    }
}