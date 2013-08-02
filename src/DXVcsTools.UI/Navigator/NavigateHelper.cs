using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DXVcsTools.UI.Navigator {
    public static class NavigateHelper {
        public static IEnumerable<NavigateItem> Scan(IEnumerable<string> roots) {
            return roots.SelectMany(ScanRoot);
        }
        static IEnumerable<NavigateItem> ScanRoot(string path) {
            if (!Directory.Exists(path))
                yield break;
            var addReferenceHelper = new AddReferenceHelper();
            foreach (var fileInfo in Directory.EnumerateFiles(path, "*.sln", SearchOption.AllDirectories)) {
                NavigateItem item = new NavigateItem();
                item.Path = fileInfo;
                item.Name = Path.GetFileName(fileInfo);
                item.ProjectType = addReferenceHelper.GetProjectType(fileInfo);
                yield return item;
            }
        }
    }
}
