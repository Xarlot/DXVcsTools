using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DXVcsTools.Data;

namespace DXVcsTools.UI.WinForms
{
    public partial class DXBlameWindow : Form, IBlameWindowView 
    {
        object CopyComment = new object();
        object ShowPreviousRevision = new object();
        object ShowDifferences = new object();
        
        string IBlameWindowView.Caption {
            get { return this.Text; }
            set { this.Text = value; }
        }

        int IBlameWindowView.CurrentLineIndex {
            get {
                BlameViewControl currentBlameView = GetActiveBlameViewControl();
                if(currentBlameView != null)
                    return currentBlameView.CurrentLineIndex;
                return -1;
            }
        }

        IList<IBlameLine> IBlameWindowView.Lines {
            get {
                BlameViewControl currentBlameView = GetActiveBlameViewControl();
                if(currentBlameView != null)
                    return currentBlameView.Lines;
                return null;
            }
        }

        event EventHandler IBlameWindowView.CopyComment {
            add { Events.AddHandler(CopyComment, value); }
            remove { Events.AddHandler(CopyComment, value); }
        }

        event EventHandler IBlameWindowView.ShowPreviousRevision {
            add { Events.AddHandler(ShowPreviousRevision, value); }
            remove { Events.AddHandler(ShowPreviousRevision, value); }
        }

        event EventHandler IBlameWindowView.ShowDifferences {
            add { Events.AddHandler(ShowDifferences, value); }
            remove { Events.AddHandler(ShowDifferences, value); }
        }

        public DXBlameWindow()
        {
            InitializeComponent();
        }

        void IBlameWindowView.ClipboardSetText(string text)
        {
            if (string.IsNullOrEmpty(text))
                Clipboard.Clear();
            else
                Clipboard.SetText(text);
        }
        
        void IBlameWindowView.ShowModal() {
            this.ShowDialog();
        }
        
        void IBlameWindowView.ShowRevision(IList<IBlameLine> data, string caption,  int lineNumber)
        {
            TabPage tabPage = new TabPage();
            BlameViewControl blameViewControl = new BlameViewControl();
            blameViewControl.Fill(data);
            blameViewControl.Dock = DockStyle.Fill;
            blameViewControl.InfoContextMenuStrip = this.contextMenuStrip1;

            tabPage.Controls.Add(blameViewControl);
            tabPage.Text = caption;
            tabControl1.TabPages.Add(tabPage);
            tabControl1.SelectedTab = tabPage;
        }
        
        void IBlameWindowView.ShowError(string title, string message)
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
       
        void RaiseEvent(object eventObject) {
            EventHandler handler = (EventHandler)Events[eventObject];
            if(handler != null)
                handler(this, EventArgs.Empty);
        }

        BlameViewControl GetActiveBlameViewControl() { 
            TabPage selectedTabPage = tabControl1.SelectedTab;
            if(selectedTabPage.Controls.Count > 0 && selectedTabPage.Controls[0] is BlameViewControl)
                return selectedTabPage.Controls[0] as BlameViewControl;
            return null;
        }

        void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            if(e.ClickedItem == this.copyCommentToolStripMenuItem) {
                RaiseEvent(CopyComment);
            } else if(e.ClickedItem == this.blamePreviousRevisionToolStripMenuItem) {
                RaiseEvent(ShowPreviousRevision);
            } else if(e.ClickedItem == this.showChangesToolStripMenuItem) {
                RaiseEvent(ShowDifferences);
            }
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button != MouseButtons.Middle && e.Clicks != 1)
                return;

            for (int i = 1; i < tabControl1.TabCount; i++) 
            {
                if (tabControl1.GetTabRect(i).Contains(e.Location)) 
                {
                    tabControl1.TabPages.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
