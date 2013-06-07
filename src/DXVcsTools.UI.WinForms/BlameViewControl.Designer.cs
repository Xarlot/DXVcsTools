namespace DXVcsTools.UI.WinForms
{
    partial class BlameViewControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.sourceBox = new DXVcsTools.UI.WinForms.ExRichTextBox();
            this.infoBox = new DXVcsTools.UI.WinForms.ExRichTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Revision";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(62, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Author";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(171, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "line #";
            // 
            // toolTip1
            // 
            this.toolTip1.ShowAlways = true;
            // 
            // sourceBox
            // 
            this.sourceBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceBox.BackColor = System.Drawing.Color.White;
            this.sourceBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sourceBox.ExtendBackColor = false;
            this.sourceBox.Location = new System.Drawing.Point(206, 16);
            this.sourceBox.Margin = new System.Windows.Forms.Padding(1);
            this.sourceBox.Name = "sourceBox";
            this.sourceBox.ReadOnly = true;
            this.sourceBox.ScrollPos = new System.Drawing.Point(0, 0);
            this.sourceBox.Size = new System.Drawing.Size(187, 210);
            this.sourceBox.TabIndex = 0;
            this.sourceBox.Text = "";
            this.sourceBox.WordWrap = false;
            this.sourceBox.VScroll += new System.EventHandler(this.sourceBox_VScroll);
            this.sourceBox.SizeChanged += new System.EventHandler(this.sourceBox_SizeChanged);
            // 
            // infoBox
            // 
            this.infoBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.infoBox.BackColor = System.Drawing.Color.White;
            this.infoBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.infoBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.infoBox.ExtendBackColor = false;
            this.infoBox.Location = new System.Drawing.Point(0, 16);
            this.infoBox.Margin = new System.Windows.Forms.Padding(1);
            this.infoBox.Name = "infoBox";
            this.infoBox.ReadOnly = true;
            this.infoBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.infoBox.ScrollPos = new System.Drawing.Point(0, 0);
            this.infoBox.Size = new System.Drawing.Size(204, 210);
            this.infoBox.TabIndex = 0;
            this.infoBox.Text = "";
            this.infoBox.WordWrap = false;
            this.infoBox.SizeChanged += new System.EventHandler(this.sourceBox_SizeChanged);
            this.infoBox.Enter += new System.EventHandler(this.infoBox_Enter);
            this.infoBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.infoBox_MouseMove);
            // 
            // BlameViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sourceBox);
            this.Controls.Add(this.infoBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "BlameViewControl";
            this.Size = new System.Drawing.Size(393, 226);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ExRichTextBox infoBox;
        private ExRichTextBox sourceBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
