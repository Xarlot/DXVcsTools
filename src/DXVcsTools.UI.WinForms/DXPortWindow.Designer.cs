namespace DXVcsTools.UI.WinForms
{
    partial class DXPortWindow
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.checkInComment = new System.Windows.Forms.TextBox();
            this.checkInTarget = new System.Windows.Forms.CheckBox();
            this.reviewTarget = new System.Windows.Forms.CheckBox();
            this.sourceFile = new System.Windows.Forms.TextBox();
            this.originalFile = new System.Windows.Forms.TextBox();
            this.targetFile = new System.Windows.Forms.TextBox();
            this.branchSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.compareButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(326, 237);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "Merge";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(488, 237);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Close";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // checkInComment
            // 
            this.checkInComment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkInComment.Location = new System.Drawing.Point(12, 140);
            this.checkInComment.Multiline = true;
            this.checkInComment.Name = "checkInComment";
            this.checkInComment.Size = new System.Drawing.Size(551, 91);
            this.checkInComment.TabIndex = 1;
            this.checkInComment.TextChanged += new System.EventHandler(this.checkInComment_TextChanged);
            // 
            // checkInTarget
            // 
            this.checkInTarget.AutoSize = true;
            this.checkInTarget.Location = new System.Drawing.Point(12, 117);
            this.checkInTarget.Name = "checkInTarget";
            this.checkInTarget.Size = new System.Drawing.Size(170, 17);
            this.checkInTarget.TabIndex = 2;
            this.checkInTarget.Text = "Check-in target file after merge";
            this.checkInTarget.UseVisualStyleBackColor = true;
            this.checkInTarget.CheckedChanged += new System.EventHandler(this.checkInTarget_CheckedChanged);
            // 
            // reviewTarget
            // 
            this.reviewTarget.AutoSize = true;
            this.reviewTarget.Location = new System.Drawing.Point(12, 94);
            this.reviewTarget.Name = "reviewTarget";
            this.reviewTarget.Size = new System.Drawing.Size(164, 17);
            this.reviewTarget.TabIndex = 2;
            this.reviewTarget.Text = "Review target file after merge";
            this.reviewTarget.UseVisualStyleBackColor = true;
            // 
            // sourceFile
            // 
            this.sourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceFile.Location = new System.Drawing.Point(71, 12);
            this.sourceFile.Name = "sourceFile";
            this.sourceFile.Size = new System.Drawing.Size(492, 20);
            this.sourceFile.TabIndex = 3;
            this.sourceFile.TextChanged += new System.EventHandler(this.sourceFile_TextChanged);
            // 
            // originalFile
            // 
            this.originalFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.originalFile.Location = new System.Drawing.Point(71, 38);
            this.originalFile.Name = "originalFile";
            this.originalFile.Size = new System.Drawing.Size(492, 20);
            this.originalFile.TabIndex = 3;
            this.originalFile.TextChanged += new System.EventHandler(this.originalFile_TextChanged);
            // 
            // targetFile
            // 
            this.targetFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.targetFile.Location = new System.Drawing.Point(71, 64);
            this.targetFile.Name = "targetFile";
            this.targetFile.Size = new System.Drawing.Size(492, 20);
            this.targetFile.TabIndex = 3;
            this.targetFile.TextChanged += new System.EventHandler(this.targetFile_TextChanged);
            // 
            // branchSelector
            // 
            this.branchSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.branchSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.branchSelector.Location = new System.Drawing.Point(442, 90);
            this.branchSelector.Name = "branchSelector";
            this.branchSelector.Size = new System.Drawing.Size(121, 21);
            this.branchSelector.TabIndex = 4;
            this.branchSelector.SelectedIndexChanged += new System.EventHandler(this.branches_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Source file:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Original file:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Target file:";
            // 
            // compareButton
            // 
            this.compareButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.compareButton.Location = new System.Drawing.Point(407, 237);
            this.compareButton.Name = "compareButton";
            this.compareButton.Size = new System.Drawing.Size(75, 23);
            this.compareButton.TabIndex = 0;
            this.compareButton.Text = "Compare";
            this.compareButton.UseVisualStyleBackColor = true;
            this.compareButton.Click += new System.EventHandler(this.compareButton_Click);
            // 
            // DXPortWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 272);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.compareButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.branchSelector);
            this.Controls.Add(this.targetFile);
            this.Controls.Add(this.originalFile);
            this.Controls.Add(this.sourceFile);
            this.Controls.Add(this.reviewTarget);
            this.Controls.Add(this.checkInTarget);
            this.Controls.Add(this.checkInComment);
            this.Name = "DXPortWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PorterSpikeWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox checkInComment;
        private System.Windows.Forms.CheckBox checkInTarget;
        private System.Windows.Forms.CheckBox reviewTarget;
        private System.Windows.Forms.TextBox sourceFile;
        private System.Windows.Forms.TextBox originalFile;
        private System.Windows.Forms.TextBox targetFile;
        private System.Windows.Forms.ComboBox branchSelector;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button compareButton;
    }
}