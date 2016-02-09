using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using System.Xml.Linq;
//using Microsoft.Build.Execution;
using System.Diagnostics;

namespace DXVcsToolsUpdater {
    class CommandArgs {
        [Option('p', Required = true)]
        public string Source { get; set; }
    }
    class Program {
        static void Main(string[] args) {
            var commandArgs = Parser.Default.ParseArguments<CommandArgs>(args).MapResult(x => x, x => new CommandArgs());
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var dxLibDirectory = Path.GetFullPath(Path.Combine(currentDirectory, @"..\lib\DX"));
            var inLibDirectory = commandArgs.Source;
            Console.WriteLine("Copying files");
            Console.WriteLine();
            foreach(var file in Directory.GetFiles(dxLibDirectory)) {
                Console.Write("Copying {0} ... ", Path.GetFileName(file));
                try {
                    File.Copy(Path.Combine(inLibDirectory, Path.GetFileName(file)), file, true);
                } catch {
                    Console.Write("FAILED");
                    Console.ReadLine();
                    return;
                }
                Console.Write("DONE");
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.Write("Updating manifest version... ");
            var manifestFile = Path.Combine(currentDirectory, @"..\src\DXVcsTools.VSIX\source.extension.vsixmanifest");
            var versionFile = Path.Combine(currentDirectory, @"..\src\DXVcsTools.Version\VersionInfo.cs");
            var manifestDoc = XDocument.Load(manifestFile);
            var identity = manifestDoc.Descendants(XName.Get("Identity", @"http://schemas.microsoft.com/developer/vsx-schema/2011")).First();
            var versionName = XName.Get("Version");
            var version = Version.Parse(identity.Attribute(versionName).Value);
            version = new Version(version.Major, version.Minor, version.Build + 1);
            identity.SetAttributeValue(versionName, version.ToString());
            manifestDoc.Save(manifestFile);
            Console.WriteLine("DONE");
            Console.Write("Updating version... ");
            string versionFileContent;
            using (var reader = new StreamReader(versionFile))
                versionFileContent = reader.ReadToEnd();
            var str = "public const string Build = \"";
            var index = versionFileContent.IndexOf(str) + str.Length;
            var endIndex = versionFileContent.IndexOf("\"", index);
            versionFileContent = versionFileContent.Remove(index, endIndex - index);
            versionFileContent = versionFileContent.Insert(index, version.Build.ToString());
            using (var writer = new StreamWriter(versionFile, false))
                writer.Write(versionFileContent);
            Console.WriteLine("DONE");
            Console.WriteLine();
            //Console.Write("Building... ");
            //var props = new Dictionary<string, string>();
            //props["Configuration"] = "Release";
            //props["AlwaysCompileMarkupFilesInSeparateDomain"] = "True";
            //props["Platform"] = "x86";
            //var request = new BuildRequestData(Path.GetFullPath(Path.Combine(currentDirectory, @"..\src\DXVcsTools.sln")), props, null, new string[] { "Build" }, null);
            //var parms = new BuildParameters() {
            //    Loggers = new Microsoft.Build.Framework.ILogger[] { new Microsoft.Build.Logging.ConsoleLogger(Microsoft.Build.Framework.LoggerVerbosity.Quiet) },
            //    DefaultToolsVersion = "12.0"
            //};
            //var result = BuildManager.DefaultBuildManager.Build(parms, request);
            //if(result.OverallResult == BuildResultCode.Success) {                
            //    Console.WriteLine("DONE");
            //} else {
            //    Console.WriteLine("FAILURE");
            //    Console.ReadLine();
            //    return;
            //}
            //Console.Write("Publishing... ");
            //Process.Start(Path.Combine(currentDirectory, "DXVcsToolsPublisher.exe"));
            Console.ReadLine();
        }
    }
}
