using System;
using System.IO;
using SharpSvn.Diff;

namespace DXVcsTools.Core {
    public class FileDiffException : Exception {
        public FileDiffException() : this(null) {
        }
        public FileDiffException(string message) : base(message) {
        }
    }

    public abstract class FileDiffBase {
        public bool Merge(string originalFile, string modifiedFile, string targetFile) {
            if (string.IsNullOrEmpty(originalFile))
                throw new ArgumentException("originalFile");

            if (string.IsNullOrEmpty(modifiedFile))
                throw new ArgumentException("modifiedFile");

            if (string.IsNullOrEmpty(targetFile))
                throw new ArgumentException("targetFile");

            return MergeCore(originalFile, modifiedFile, targetFile);
        }

        protected abstract bool MergeCore(string originalFile, string modifiedFile, string targetFile);
    }

    public class FileDiff : FileDiffBase {
        protected override bool MergeCore(string originalFile, string modifiedFile, string targetFile) {
            using(var targetStream = new MemoryStream()) {
                SvnFileDiff diff = null;
                if(!SvnFileDiff.TryCreate(originalFile, modifiedFile, targetFile, new SvnFileDiffArgs(), out diff))
                    throw new FileDiffException("SvnFileDiff.TryCreate failed");

                try {
                    if(diff.HasConflicts)
                        return false;

                    if(!diff.WriteMerged(targetStream, new SvnDiffWriteMergedArgs()))
                        throw new FileDiffException("SvnFileDiff.WriteMerged failed");
                }
                finally {
                    diff.Dispose();
                }

                File.WriteAllBytes(targetFile, targetStream.ToArray());
            }
            return true;
        }
    }

    public class OverwriteFile : FileDiffBase {
        protected override bool MergeCore(string originalFile, string modifiedFile, string targetFile) {
            File.Copy(modifiedFile, targetFile, true);
            return true;
        }

        public static bool Write(string originalFile, string targetFile) {
            return new OverwriteFile().Merge(originalFile, originalFile, targetFile);
        }
    }
}