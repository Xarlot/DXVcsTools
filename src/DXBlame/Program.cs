using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using DXVcsTools.Core;
using DXVcsTools.UI;
using DXVcsTools.UI.Wpf;
using DXVcsTools.Version;
using ViewFactory = DXVcsTools.UI.WinForms.ViewFactory;

namespace DXBlame {
    class Program {
        [STAThread]
        static void Main(string[] args) {
            Console.WriteLine("DXBlame v{0}", VersionInfo.FullVersion);

            if (args.Length < 2) {
                PrintUsage();
                return;
            }

            try {
                switch (args[0].ToLower()) {
                    case "blame": {
                        int? lineNumber = null;
                        if (args.Length > 2)
                            lineNumber = Convert.ToInt32(args[2]);

                        BlameDirect(ConfigurationManager.AppSettings["DXVcsService"], args[1], lineNumber);
                    }
                        break;

                    case "vsblame":
                        VSBlame(args[1], args[2], Convert.ToInt32(args[3]));
                        break;

                    default:
                        PrintUsage();
                        break;
                }
            }
            catch (Exception exception) {
                Console.WriteLine(exception);
                Console.WriteLine("Press any key...");
                Console.ReadKey();
            }
        }

        static void BlameDirect(string vcsService, string vcsFile, int? lineNumber) {
            if (GetBlameType() == DXBlameType.External) {
                HistoryImporter importer = CreateHistoryImporter();
                Uri importedFile = importer.ImportFileDirect(vcsService, vcsFile);
                CreateUI().Show(importedFile, lineNumber);
            }
            else {
                ShowNative(vcsFile, vcsService, lineNumber.HasValue ? lineNumber.Value : 0, true);
            }
        }

        static void VSBlame(string file, string projectFile, int lineNumber) {
            if (GetBlameType() == DXBlameType.External) {
                HistoryImporter importer = CreateHistoryImporter();
                Uri importedFile = importer.ImportFile(file, projectFile);
                CreateUI().Show(importedFile, lineNumber);
            }
            else {
                ShowNative(file, projectFile, lineNumber, false);
            }
        }

        static HistoryImporter CreateHistoryImporter() {
            return new HistoryImporter(ConfigurationManager.AppSettings["SvnRepository"], ConfigurationManager.AppSettings["WorkingCopy"]);
        }

        static IDXBlameUI CreateUI() {
            return new DXBlameViewer(ConfigurationManager.AppSettings["TortoiseProc"]);
        }

        static DXBlameType GetBlameType() {
            try {
                return (DXBlameType)Enum.Parse(typeof(DXBlameType), ConfigurationManager.AppSettings["BlameType"]);
            }
            catch {
                return DXBlameType.External;
            }
        }

        static void ShowNative(string file, string fileSource, int lineNumber, bool direct) {
            var model = new BlameWindowModel(file, fileSource, lineNumber, direct);
            IBlameWindowView ui = new ViewFactory().CreateBlameWindow();

            var presenter = new BlameWindowPresenter(ui, model);
            presenter.Show();
        }

        static void PrintUsage() {
            string programName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine("Usage:\r\n" + "    {0} blame <vcsFile> [lineNumber]\r\n" + "    {0} vsblame <file> <projectFile> <lineNumber>", programName);
        }
    }
}