using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DXVcsTools.UI.Navigator {
    public static class NavigateHelper {
        public static IEnumerable<NavigateItem> Scan(IEnumerable<string> roots) {
            var files = roots.SelectMany(ScanForFiles).ToList();
            var addReferenceHelper = new AddReferenceHelper();
            int index = 0;
            BusyIndicator.UpdateText("Progress: {0} of {1}");
            foreach (var file in files) {
                BusyIndicator.UpdateProgress(index + 1, files.Count);
                NavigateItem item = new NavigateItem();
                item.Path = file;
                item.Name = Path.GetFileName(file);
                item.ProjectType = addReferenceHelper.GetProjectType(file);
                item.GeneratedProjects = new SolutionParser(file).GetProjectPathes().ToList();
                index++;
                yield return item;
            }
        }
        static IEnumerable<string> ScanForFiles(string path) {
            if (!Directory.Exists(path))
                yield break;
            foreach (var fileInfo in Directory.EnumerateFiles(path, "*.sln", SearchOption.AllDirectories))
                yield return fileInfo;
        }
    }
}
