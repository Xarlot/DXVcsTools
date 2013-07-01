using System.Collections.Generic;

namespace DXVcsTools.Core {
    public class FileItem : FileItemBase {
        public FileItem(IEnumerable<FileItemBase> items) : base(items) {
        }
        public override int Priority {
            get { return 50; }
        }
    }
}