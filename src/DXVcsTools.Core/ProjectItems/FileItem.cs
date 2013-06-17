namespace DXVcsTools.Core {
    public class FileItem : FileItemBase {
        public FileItem() : base(null) {
        }
        public override int Priority {
            get { return 50; }
        }
    }
}