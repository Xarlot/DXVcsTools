using System.Collections.Generic;
using System.IO;

namespace DXVcsTools.UI.Navigator {
    public static class NavigateHelper {
        public static IEnumerable<NavigateItem> Scan(string path, string replace) {
            if (!Directory.Exists(path))
                yield break;
            string dir = Path.HasExtension(path) ? Path.GetDirectoryName(path) : path;
            foreach (var fileInfo in Directory.EnumerateFiles(dir, "*.sln", SearchOption.AllDirectories)) {
                NavigateItem item = new NavigateItem();
                item.Path = fileInfo;
                item.Name = Path.GetFileName(fileInfo);
                yield return item;
            }
        }
    }
}
