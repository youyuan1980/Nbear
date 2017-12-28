namespace NBear.Tools.EntityDesignToEntity
{
    partial class Form1
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
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnGen = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyAllToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputLanguage = new System.Windows.Forms.ComboBox();
            this.output = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOutputNamespace = new System.Windows.Forms.TextBox();
            this.btnGenConfig = new System.Windows.Forms.Button();
            this.btnGenDbScript = new System.Windows.Forms.Button();
            this.btnAdvOpt = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(12, 30);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.ReadOnly = true;
            this.txtFileName.Size = new System.Drawing.Size(270, 20);
            this.txtFileName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Choose an Entitiy Interface Assembly";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(288, 28);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(67, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnGen
            // 
            this.btnGen.Enabled = false;
            this.btnGen.Location = new System.Drawing.Point(361, 28);
            this.btnGen.Name = "btnGen";
            this.btnGen.Size = new System.Drawing.Size(122, 23);
            this.btnGen.TabIndex = 3;
            this.btnGen.Text = "Generate Entities";
            this.btnGen.UseVisualStyleBackColor = true;
            this.btnGen.Click += new System.EventHandler(this.btnGen_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "dll";
            this.openFileDialog1.Filter = "Entity Design Assembly|*.dll";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Output";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyAllToClipboardToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(161, 26);
            // 
            // copyAllToClipboardToolStripMenuItem
            // 
            this.copyAllToClipboardToolStripMenuItem.Name = "copyAllToClipboardToolStripMenuItem";
            this.copyAllToClipboardToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.copyAllToClipboardToolStripMenuItem.Text = "Copy to Clipboard";
            this.copyAllToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyAllToClipboardToolStripMenuItem_Click);
            // 
            // outputLanguage
            // 
            this.outputLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.outputLanguage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputLanguage.FormattingEnabled = true;
            this.outputLanguage.Items.AddRange(new object[] {
            "C#",
            "VB.NET"});
            this.outputLanguage.Location = new System.Drawing.Point(57, 70);
            this.outputLanguage.Name = "outputLanguage";
            this.outputLanguage.Size = new System.Drawing.Size(100, 21);
            this.outputLanguage.TabIndex = 19;
            // 
            // output
            // 
            this.output.ContextMenuStrip = this.contextMenuStrip1;
            this.output.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.output.Location = new System.Drawing.Point(12, 101);
            this.output.Name = "output";
            this.output.Size = new System.Drawing.Size(770, 287);
            this.output.TabIndex = 21;
            this.output.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(183, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Output Namespace";
            // 
            // txtOutputNamespace
            // 
            this.txtOutputNamespace.Location = new System.Drawing.Point(289, 70);
            this.txtOutputNamespace.Name = "txtOutputNamespace";
            this.txtOutputNamespace.Size = new System.Drawing.Size(350, 20);
            this.txtOutputNamespace.TabIndex = 23;
            this.txtOutputNamespace.Text = "Entities";
            this.txtOutputNamespace.TextChanged += new System.EventHandler(this.txtOutputNamespace_TextChanged);
            // 
            // btnGenConfig
            // 
            this.btnGenConfig.Enabled = false;
            this.btnGenConfig.Location = new System.Drawing.Point(489, 28);
            this.btnGenConfig.Name = "btnGenConfig";
            this.btnGenConfig.Size = new System.Drawing.Size(150, 23);
            this.btnGenConfig.TabIndex = 24;
            this.btnGenConfig.Text = "Generate Configuration";
            this.btnGenConfig.UseVisualStyleBackColor = true;
            this.btnGenConfig.Click += new System.EventHandler(this.btnGenConfig_Click);
            // 
            // btnGenDbScript
            // 
            this.btnGenDbScript.Enabled = false;
            this.btnGenDbScript.Location = new System.Drawing.Point(645, 28);
            this.btnGenDbScript.Name = "btnGenDbScript";
            this.btnGenDbScript.Size = new System.Drawing.Size(137, 23);
            this.btnGenDbScript.TabIndex = 25;
            this.btnGenDbScript.Text = "Generate DB Script";
            this.btnGenDbScript.UseVisualStyleBackColor = true;
            this.btnGenDbScript.Click += new System.EventHandler(this.btnGenDbScript_Click);
            // 
            // btnAdvOpt
            // 
            this.btnAdvOpt.Enabled = false;
            this.btnAdvOpt.Location = new System.Drawing.Point(645, 68);
            this.btnAdvOpt.Name = "btnAdvOpt";
            this.btnAdvOpt.Size = new System.Drawing.Size(137, 23);
            this.btnAdvOpt.TabIndex = 26;
            this.btnAdvOpt.Text = "Advanced Options";
            this.btnAdvOpt.UseVisualStyleBackColor = true;
            this.btnAdvOpt.Click += new System.EventHandler(this.btnAdvOpt_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 400);
            this.Controls.Add(this.btnAdvOpt);
            this.Controls.Add(this.btnGenDbScript);
            this.Controls.Add(this.txtOutputNamespace);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnGenConfig);
            this.Controls.Add(this.output);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnGen);
            this.Controls.Add(this.outputLanguage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "EntityDesign To Entity";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnGen;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox outputLanguage;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyAllToClipboardToolStripMenuItem;
        private System.Windows.Forms.RichTextBox output;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOutputNamespace;
        private System.Windows.Forms.Button btnGenConfig;
        private System.Windows.Forms.Button btnGenDbScript;
        private System.Windows.Forms.Button btnAdvOpt;
    }
}

