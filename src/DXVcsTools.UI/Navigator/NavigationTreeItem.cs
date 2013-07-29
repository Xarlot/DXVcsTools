using System;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Mvvm;
using DXVcsTools.UI.Navigator;

namespace DXVcsTools.UI.Navigator {
    public class NavigateTreeItem : BindableBase, IEquatable<NavigateTreeItem> {
        public bool Equals(NavigateTreeItem other) {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(key, other.key) && string.Equals(parentKey, other.parentKey);
        }
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((NavigateTreeItem)obj);
        }
        public override int GetHashCode() {
            unchecked {
                return ((key != null ? key.GetHashCode() : 0) * 397) ^ (parentKey != null ? parentKey.GetHashCode() : 0);
            }
        }
        public static bool operator ==(NavigateTreeItem left, NavigateTreeItem right) {
            return Equals(left, right);
        }
        public static bool operator !=(NavigateTreeItem left, NavigateTreeItem right) {
            return !Equals(left, right);
        }
        readonly NavigateItem item;
        readonly string key;
        readonly string parentKey;
        bool used;
        public bool Used {
            get { return Item.Return(x => x.Used, () => used); }
            set {
                used = value;
                Item.Do(x => x.Used = value);
            }
        }
        public string RelativePath {
            get { return Item.Return(x => x.RelativePath, () => string.Empty); }
            set { Item.Do(x => x.RelativePath = value); }
        }
        public string DisplayText { get { return Key; }}
        public NavigateItem Item { get { return item; } }
        public string Key { get { return key; } }
        public string ParentKey { get { return parentKey; } }
        public NavigateTreeItem(string parentKey, string key, NavigateItem item = null) {
            this.parentKey = parentKey;
            this.key = key;
            this.item = item;
            this.used = item.Return(x => x.Used, () => false);
        }
    }
}
