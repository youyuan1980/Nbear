using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using NBear.Common;

namespace NBear.Tools.ConfigurationEncrypter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtKey.Text = CryptographyManager.DEFAULT_KEY;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFile.Text = openFileDialog1.FileName;
                btnEncrypt.Enabled = true;
                btnDecrypt.Enabled = true;
            }
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(txtFile.Text);
            string content = sr.ReadToEnd();
            sr.Close();

            if (content.TrimStart().Substring(0, 5).ToLower() == "<?xml")
            {
                StreamWriter sw = null;
                try
                {
                    File.Delete(txtFile.Text);
                    sw = new StreamWriter(txtFile.Text);
                    sw.Write(new CryptographyManager().SymmetricEncrpyt(content, System.Security.Cryptography.Rijndael.Create(), txtKey.Text));
                }
                catch
                {
                    MessageBox.Show("Error! The file is not writable.");
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Close();
                    }
                }

                MessageBox.Show("Encrypt OK!");
            }
            else
            {
                MessageBox.Show("The file is already encrypted or it is not a valid configuration file!");
            }
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(txtFile.Text);
            string content = sr.ReadToEnd();
            sr.Close();

            if (content.TrimStart().Substring(0, 5).ToLower() == "<?xml")
            {
                MessageBox.Show("The file has not been encrypted!");
            }
            else
            {
                StreamWriter sw = null;

                try
                {
                    File.Delete(txtFile.Text);
                    sw = new StreamWriter(txtFile.Text);
                    sw.Write(new CryptographyManager().SymmetricDecrpyt(content, System.Security.Cryptography.Rijndael.Create(), txtKey.Text));
                }
                catch
                {
                    MessageBox.Show("Error! The file is not writable.");
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Close();
                    }
                }

                MessageBox.Show("Decrypt OK!");
            }
        }

    }
}