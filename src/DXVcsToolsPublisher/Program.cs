using System.IO;
using System.Reflection;
using CommandLine;
using DXVcsTools.UI.AutoUpdate;
using DXVcsTools.Version;

namespace DXVcsToolsPublisher {
    class Program {
        static int Main(string[] args) {
            var options = new CommandParameters();
            var parser = new Parser();
            int errorCode = 0x0;
            if (parser.ParseArguments(args, options)) {
                var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (!options.SkipNetworkUpdate) {
                    AutoUpdateOptions update = AutoUpdateHelper.CreateAutoUpdateOptions();
                    update.Path = string.Format("{0}.{1}.{2}", VersionInfo.Major, VersionInfo.Minor, VersionInfo.Build);
                    update.ShowAll = options.ShowAll;
                    if (!AutoUpdateHelper.Publish(update, location, options.PublishPath))
                        errorCode |= 0x1;
                }
                if (!options.SkipGalleryUpdate) {
                    if (!AutoUpdateHelper.PublishToGallery(location))
                        errorCode |= 0x2;
                }
            } else {
                errorCode = 0x4;
            }
            return errorCode;
        }
    }

}
