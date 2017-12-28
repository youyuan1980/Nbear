using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace NBear.Tools.EntityDesignToEntity
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFileName.Text = openFileDialog1.FileName;
                btnGen.Enabled = true;
                btnGenConfig.Enabled = true;
                btnGenDbScript.Enabled = true;
                btnAdvOpt.Enabled = true;
                advForm.RefreshEntities(Assembly.LoadFrom(txtFileName.Text).GetTypes());
                AppDomain.CurrentDomain.SetupInformation.PrivateBinPath = System.IO.Path.GetDirectoryName(txtFileName.Text);
            }
        }

        private void btnGen_Click(object sender, EventArgs e)
        {
            Assembly ass = Assembly.LoadFrom(txtFileName.Text);
            output.Text = string.Empty;
            Application.DoEvents();
            //output.Text = new CodeGenHelper(txtOutputNamespace.Text, advForm).GenEntities(ass, outputLanguage.SelectedIndex);
            output.Text = new CodeGenHelper(txtOutputNamespace.Text, advForm).GenEntitiesEx(ass, outputLanguage.SelectedIndex);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            outputLanguage.SelectedIndex = 0;

            if (File.Exists("LastTimeOutputNamespace.txt"))
            {
                try
                {
                    txtOutputNamespace.Text = File.ReadAllText("LastTimeOutputNamespace.txt");
                }
                catch
                {
                }
            }
        }

        private void copyAllToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(output.Text);
        }

        private void btnGenConfig_Click(object sender, EventArgs e)
        {
            Assembly ass = Assembly.LoadFrom(txtFileName.Text);
            output.Text = string.Empty;
            Application.DoEvents();
            output.Text = new CodeGenHelper(txtOutputNamespace.Text, advForm).GenEntityConfigurations(ass);
        }

        private void btnGenDbScript_Click(object sender, EventArgs e)
        {
            Assembly ass = Assembly.LoadFrom(txtFileName.Text);
            output.Text = string.Empty;
            Application.DoEvents();
            output.Text = new CodeGenHelper(txtOutputNamespace.Text, advForm).GenDbScript(ass);
        }

        private void txtOutputNamespace_TextChanged(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllText("LastTimeOutputNamespace.txt", txtOutputNamespace.Text);
            }
            catch
            {
            }
        }

        private AdvOptForm advForm = new AdvOptForm();

        private void btnAdvOpt_Click(object sender, EventArgs e)
        {
            advForm.ShowDialog();
        }
    }

    public interface IAdvOpt
    {
        bool EnableAdvOpt { get; }
        bool IsEntityEnabled(string name);
        void RefreshEntities(Type[] types);
    }
}