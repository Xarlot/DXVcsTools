using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DXVcsTools.Data;
using DXVcsTools.DXVcsClient;
using DXVcsTools.Core;
using System.Configuration;

namespace TortoiseProc
{
    class BlameRunner
    {
        Options options;
        ProgressForm progressForm;
        string blameFile;
        string logFile;
        string revisionString;
        string fileName;
        string fileSource;
        bool direct;

        private BlameRunner(Options options, bool direct)
        {
            this.options = options;
            this.direct = direct;
        }

        public static bool Run(Options options, bool direct)
        {
            return new BlameRunner(options, direct).Run();
        }

        bool Run()
        {
            string path = options.GetValue("path", null);
            if (string.IsNullOrEmpty(path))
                return false;

            Program.ParsePath(path, out fileName, out fileSource);

            if(!options.IsPresent("startrev") && !options.IsPresent("endrev"))
                LoadWhitespaceOptions();

            progressForm = new ProgressForm();
            progressForm.Load += new EventHandler(progressForm_Load);
            progressForm.ShowDialog();

            return true;
        }

        void LoadWhitespaceOptions() {
            SpacesAction spacesAction = GetWhitespaceOption();
            if(ConfigurationManager.AppSettings["ShowOptions"] != "False")
            {
                using(OptionsForm optionsForm = new OptionsForm()) 
                {
                    optionsForm.SpacesAction = spacesAction;
                    optionsForm.ShowDialog();
                    spacesAction = optionsForm.SpacesAction;
                }
            }
            if(spacesAction == SpacesAction.IgnoreChange)
                options.SetValue("ignorespaces", null);
            if(spacesAction == SpacesAction.IgnoreAll)
                options.SetValue("ignoreallspaces", null);

        }

        SpacesAction GetWhitespaceOption()
        {
            string settingValue = ConfigurationManager.AppSettings["WhitespacesOptionDefaultValue"];
            if(settingValue == "IgnoreChange")
                return SpacesAction.IgnoreChange;
            if(settingValue == "IgnoreAll")
                return SpacesAction.IgnoreAll;
            return SpacesAction.Compare;
        }

        void progressForm_Load(object sender, EventArgs e)
        {
            ProgressForm progressForm = (ProgressForm)sender;

            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(
                delegate(object o)
                {
                    try
                    {
                        IDXVcsRepository dxRepository = Connect();

                        SpacesAction spacesAction = SpacesAction.Compare;
                        if(options.IsPresent("ignorespaces"))
                            spacesAction = SpacesAction.IgnoreChange;
                        else if(options.IsPresent("ignoreallspaces"))
                            spacesAction = SpacesAction.IgnoreAll;
                        
                        FileDiffInfo diffInfo = dxRepository.GetFileDiffInfo(fileName, Progress, spacesAction);

                        revisionString = options.GetValue("endrev", "HEAD");
                        int revision = revisionString == "HEAD" ? diffInfo.LastRevision : int.Parse(revisionString);

                        IList<IBlameLine> blame = diffInfo.BlameAtRevision(revision);
                        blameFile = MakeBlameFile(fileName, blame);
                        logFile = MakeLog(blame);
                        Finish();
                    }
                    catch (Exception ex)
                    {
                        Error(ex);
                    }
                }
            ));
        }

        private IDXVcsRepository Connect()
        {
            IDXVcsRepository dxRepository;
            if (direct)
            {
                dxRepository = DXVcsConnectionHelper.Connect(string.IsNullOrEmpty(fileSource) ? ConfigurationManager.AppSettings["DXVcsService"] : fileSource);
            }
            else
            {
                string vcsFile;
                string vcsService;
                dxRepository = DXVcsConnectionHelper.Connect(fileName, fileSource, out vcsService, out vcsFile);
                fileName = vcsFile;
                fileSource = vcsService;
            }
            return dxRepository;
        }

        void Progress(int current, int total)
        {
            progressForm.BeginInvoke(new Action(delegate()
            {
                progressForm.progressBar1.Minimum = 0;
                progressForm.progressBar1.Maximum = total;
                progressForm.progressBar1.Value = current;
            }
            ));
        }

        void Finish()
        {
            progressForm.BeginInvoke(new Action(delegate()
            {
                string path = fileName + "|" + fileSource;
                string blameArgs = string.Format("\"{0}\" \"{1}\" \"{2}\" /path:\"{3}\" /revrange:\"{4}\"", blameFile, logFile, Path.GetFileName(fileName), path, revisionString);
                
                string line = options.GetValue("line", null);
                if (options.GetValue("line", null) != null)
                    blameArgs += " /line:" + line;

                if(options.IsPresent("ignorespaces"))
                    blameArgs += " /ignorespaces";
                else if(options.IsPresent("ignoreallspaces"))
                    blameArgs += " /ignoreallspaces";

                System.Diagnostics.Process process;
                try 
                {
                    string exeFile = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "TortoiseBlame.exe");
                    process = System.Diagnostics.Process.Start(exeFile, blameArgs);
                } 
                finally 
                {
                    HideForm();
                }
                
                process.WaitForExit();

                File.Delete(blameFile);
                File.Delete(logFile);
            }
            ));
        }

        void Error(Exception e)
        {
            progressForm.BeginInvoke(new Action(delegate()
            {
                HideForm();
                throw new Exception("Error", e);
            }
            ));
        }

        void HideForm()
        {
            progressForm.Hide();
            progressForm.Dispose();
        }

        static string MakeBlameFile(string vcsFile, IList<IBlameLine> blame)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("  {0,-6} {1,-6} {1,-6} {2,-30} {3,-60} {4, -30} {5} ", "line", "rev", "date", "path", "author", "content");
            sb.AppendLine();
            sb.AppendLine();

            int i = 0;
            foreach (var line in blame)
            {
                sb.AppendFormat("{0,8} {1,6} {1,6} {2,-30:dd/MM/yyyy hh:mm:ss} {3,-60} {4, -30} {5} ",
                    i++, line.Revision, line.CommitDate, "", line.User, line.SourceLine);
                sb.AppendLine();
            }
            string fileName = Path.GetTempFileName() + Path.GetExtension(vcsFile);
            File.WriteAllText(fileName, sb.ToString());
            return fileName;
        }

        private static string MakeLog(IList<IBlameLine> blame)
        {
            SortedList<int, string> comments = new SortedList<int, string>();

            foreach (var line in blame)
                comments[line.Revision] = line.Comment;

            BinaryWriter bw = new BinaryWriter(new MemoryStream());
            for (int i = comments.Keys.Count - 1; i >= 0; i--)
            {
                bw.Write(comments.Keys[i]);
                byte[] comment = Encoding.Default.GetBytes(comments.Values[i]);
                bw.Write(comment.Length);
                bw.Write(comment);
            }
            string fileName = Path.GetTempFileName();
            File.WriteAllBytes(fileName, ((MemoryStream)bw.BaseStream).ToArray());
            return fileName;
        }
    }
}
