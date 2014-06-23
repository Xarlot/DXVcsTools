using CommandLine;

namespace DXVcsToolsPublisher {
    public class CommandParameters : BaseOptionAttribute {
        [Option('p', "path", DefaultValue = @"\\corp\internal\common\4all\DXVcsTools_2.0", HelpText = "Define the network target path")]
        public string PublishPath { get; set; }
        [Option('s', "showall", DefaultValue = false, HelpText = "Define show update for all users")]
        public bool ShowAll { get; set; }
    }
}
