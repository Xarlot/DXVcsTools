using System.Collections.Generic;

namespace DXVcsTools.Core {
    public class FolderItem : FileItemBase {
        public FolderItem(IEnumerable<FileItemBase> items) : base(items) {
        }
        public override int Priority {
            get { return 100; }
        }
    }
}