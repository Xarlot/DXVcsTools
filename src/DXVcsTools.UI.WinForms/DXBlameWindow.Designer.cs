namespace DXVcsTools.UI.WinForms
{
    partial class DXBlameWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyCommentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.blamePreviousRevisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showChangesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyCommentToolStripMenuItem,
            this.toolStripSeparator1,
            this.blamePreviousRevisionToolStripMenuItem,
            this.showChangesToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(198, 76);
            this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // copyCommentToolStripMenuItem
            // 
            this.copyCommentToolStripMenuItem.Name = "copyCommentToolStripMenuItem";
            this.copyCommentToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.copyCommentToolStripMenuItem.Text = "Copy comment";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(194, 6);
            // 
            // blamePreviousRevisionToolStripMenuItem
            // 
            this.blamePreviousRevisionToolStripMenuItem.Name = "blamePreviousRevisionToolStripMenuItem";
            this.blamePreviousRevisionToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.blamePreviousRevisionToolStripMenuItem.Text = "Blame previous revision";
            // 
            // showChangesToolStripMenuItem
            // 
            this.showChangesToolStripMenuItem.Name = "showChangesToolStripMenuItem";
            this.showChangesToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.showChangesToolStripMenuItem.Text = "Show changes";
            // 
            // tabControl1
            // 
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(819, 504);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tabControl1_MouseClick);
            // 
            // DXBlameWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 504);
            this.Controls.Add(this.tabControl1);
            this.Name = "DXBlameWindow";
            this.Text = "BlameView";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem blamePreviousRevisionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showChangesToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.ToolStripMenuItem copyCommentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;

    }
}
