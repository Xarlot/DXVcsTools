#if UNIT_TEST

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace DXVcsTools.Core.Tests
{
    [TestFixture]
    public class PathHelperTests
    {
        // 1. Can't use Assembly.GetEntryAssembly() in NUnit test - method returns null.
        // 2. Make sure baseFolder doesn't end with '\' (backslash) symbol - we want to test Resolve handles such parameter correctly 
        private const string _basePath = @"x:\foobar";

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void PathArgumentValidation([Values(null, "")] string path)
        {
            PathHelper.ResolvePath(_basePath, path);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void BasePathArgumentValidation([Values(null, "")] string basePath)
        {
            PathHelper.ResolvePath(basePath, "foobar");
        }

        [Test]
        public void ResolveAbsolutePath()
        {
            const string path = "c:\\folder\\subfolder";
            string resolvedPath = PathHelper.ResolvePath(_basePath, path);
            Assert.AreEqual(path, resolvedPath);
        }

        [Test]
        public void ResolveRelativePath()
        {
            const string path = "..\\repository\\";
            string resolvedPath = PathHelper.ResolvePath(_basePath, path);
            Assert.AreEqual(@"x:\foobar\..\repository\", resolvedPath);
        }

        [Test]
        public void ResolveDirectoryName()
        {
            const string folderName = "RepoDir";
            string resolvedPath = PathHelper.ResolvePath(_basePath, folderName);
            Assert.AreEqual(@"x:\foobar\RepoDir", resolvedPath);
        }

        [Test]
        public void ResolveRelativePathNotAffectedByCurrentDirectory()
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            try
            {
                string tempDirectory = Path.GetTempPath();
                Directory.SetCurrentDirectory(tempDirectory);

                string resolvedPath = PathHelper.ResolvePath(_basePath, "RepoDir");
                StringAssert.DoesNotStartWith(tempDirectory, resolvedPath);
            }
            finally
            {
                Directory.SetCurrentDirectory(currentDirectory);
            }
        }
    }
}

#endif // UNIT_TEST
