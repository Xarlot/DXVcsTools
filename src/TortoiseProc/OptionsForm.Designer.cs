namespace TortoiseProc {
    partial class OptionsForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbIgnoreAll = new System.Windows.Forms.RadioButton();
            this.rbIgnoreChange = new System.Windows.Forms.RadioButton();
            this.rbCompare = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(37, 122);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbIgnoreAll);
            this.groupBox1.Controls.Add(this.rbIgnoreChange);
            this.groupBox1.Controls.Add(this.rbCompare);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(129, 94);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Whitespace";
            // 
            // rbIgnoreAll
            // 
            this.rbIgnoreAll.AutoSize = true;
            this.rbIgnoreAll.Location = new System.Drawing.Point(6, 65);
            this.rbIgnoreAll.Name = "rbIgnoreAll";
            this.rbIgnoreAll.Size = new System.Drawing.Size(68, 17);
            this.rbIgnoreAll.TabIndex = 0;
            this.rbIgnoreAll.Text = "Ignore all";
            this.rbIgnoreAll.UseVisualStyleBackColor = true;
            // 
            // rbIgnoreChange
            // 
            this.rbIgnoreChange.AutoSize = true;
            this.rbIgnoreChange.Location = new System.Drawing.Point(6, 42);
            this.rbIgnoreChange.Name = "rbIgnoreChange";
            this.rbIgnoreChange.Size = new System.Drawing.Size(94, 17);
            this.rbIgnoreChange.TabIndex = 0;
            this.rbIgnoreChange.Text = "Ignore change";
            this.rbIgnoreChange.UseVisualStyleBackColor = true;
            // 
            // rbCompare
            // 
            this.rbCompare.AutoSize = true;
            this.rbCompare.Checked = true;
            this.rbCompare.Location = new System.Drawing.Point(6, 19);
            this.rbCompare.Name = "rbCompare";
            this.rbCompare.Size = new System.Drawing.Size(67, 17);
            this.rbCompare.TabIndex = 0;
            this.rbCompare.TabStop = true;
            this.rbCompare.Text = "Compare";
            this.rbCompare.UseVisualStyleBackColor = true;
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(156, 165);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OptionsForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbCompare;
        private System.Windows.Forms.RadioButton rbIgnoreChange;
        private System.Windows.Forms.RadioButton rbIgnoreAll;
    }
}