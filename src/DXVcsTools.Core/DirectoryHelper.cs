using System.IO;

namespace DXVcsTools.Core {
    static class DirectoryHelper {
        public static void DeleteDirectory(string path) {
            foreach (string file in Directory.GetFiles(path)) {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string directory in Directory.GetDirectories(path)) {
                DeleteDirectory(directory);
            }

            Directory.Delete(path);
        }
    }
}