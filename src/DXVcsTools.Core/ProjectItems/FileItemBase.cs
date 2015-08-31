using System.Collections.Generic;

namespace DXVcsTools.Core {
    public abstract class FileItemBase : ProjectItemBase {
        protected FileItemBase(IEnumerable<FileItemBase> items) : base(items) {
        }
        public override bool IsNew {
            get { return false; }
            set { }
        }
    }
}