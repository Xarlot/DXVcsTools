using System.IO;
using System.Reflection;
using CommandLine;
using DXVcsTools.UI.AutoUpdate;
using DXVcsTools.Version;

namespace DXVcsToolsPublisher {
    class Program {
        static void Main(string[] args) {
            var options = new CommandParameters();
            var parser = new Parser();
            if (parser.ParseArguments(args, options)) {
                AutoUpdateOptions update = AutoUpdateHelper.CreateAutoUpdateOptions();
                update.Path = string.Format("{0}.{1}.{2}", VersionInfo.Major, VersionInfo.Minor, VersionInfo.Build);
                update.ShowAll = options.ShowAll;
                AutoUpdateHelper.Publish(update, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options.PublishPath);
            }
        }
    }

}
