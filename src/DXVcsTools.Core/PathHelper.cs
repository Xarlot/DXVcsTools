using System;
using System.IO;
using System.Reflection;

namespace DXVcsTools.Core {
    public static class PathHelper {
        public static string ResolvePath(string path) {
            return ResolvePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), path);
        }

        public static string ResolvePath(string basePath, string path) {
            if (!Path.IsPathRooted(basePath))
                throw new ArgumentException("basePath must be rooted");

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (Path.IsPathRooted(path))
                return path;

            return Path.Combine(basePath, path);
        }
    }
}