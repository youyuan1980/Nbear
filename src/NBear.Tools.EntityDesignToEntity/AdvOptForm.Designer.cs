namespace NBear.Tools.EntityDesignToEntity
{
    partial class AdvOptForm
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
            this.checkEnableAdvOpt = new System.Windows.Forms.CheckBox();
            this.listEntities = new System.Windows.Forms.CheckedListBox();
            this.checkSelectAll = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkEnableAdvOpt
            // 
            this.checkEnableAdvOpt.AutoSize = true;
            this.checkEnableAdvOpt.Location = new System.Drawing.Point(13, 13);
            this.checkEnableAdvOpt.Name = "checkEnableAdvOpt";
            this.checkEnableAdvOpt.Size = new System.Drawing.Size(219, 17);
            this.checkEnableAdvOpt.TabIndex = 0;
            this.checkEnableAdvOpt.Text = "Generate Code for Selected Entities Only";
            this.checkEnableAdvOpt.UseVisualStyleBackColor = true;
            this.checkEnableAdvOpt.CheckedChanged += new System.EventHandler(this.checkEnableAdvOpt_CheckedChanged);
            // 
            // listEntities
            // 
            this.listEntities.CheckOnClick = true;
            this.listEntities.Enabled = false;
            this.listEntities.FormattingEnabled = true;
            this.listEntities.Location = new System.Drawing.Point(12, 70);
            this.listEntities.Name = "listEntities";
            this.listEntities.Size = new System.Drawing.Size(258, 289);
            this.listEntities.Sorted = true;
            this.listEntities.TabIndex = 1;
            // 
            // checkSelectAll
            // 
            this.checkSelectAll.AutoSize = true;
            this.checkSelectAll.Enabled = false;
            this.checkSelectAll.Location = new System.Drawing.Point(13, 47);
            this.checkSelectAll.Name = "checkSelectAll";
            this.checkSelectAll.Size = new System.Drawing.Size(117, 17);
            this.checkSelectAll.TabIndex = 2;
            this.checkSelectAll.Text = "Select/Unselect All";
            this.checkSelectAll.UseVisualStyleBackColor = true;
            this.checkSelectAll.CheckedChanged += new System.EventHandler(this.checkSelectAll_CheckedChanged);
            // 
            // AdvOptForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 373);
            this.Controls.Add(this.checkSelectAll);
            this.Controls.Add(this.listEntities);
            this.Controls.Add(this.checkEnableAdvOpt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdvOptForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Advanced Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkEnableAdvOpt;
        private System.Windows.Forms.CheckedListBox listEntities;
        private System.Windows.Forms.CheckBox checkSelectAll;
    }
}