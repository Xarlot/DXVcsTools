using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcsTools.Core {
    public class FolderItem : FileItemBase {
        public override int Priority { get { return 100; } }
        public FolderItem(IEnumerable<FileItemBase> items) : base(items) {
        }
    }
}
