using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace SetupNBearVsPlugin
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string myDocDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string vs2005AddInDir = myDocDir + "\\Visual Studio 2005\\Addins";
            string addInFile = vs2005AddInDir + "\\NBear.Tools.EntityDesignToEntityVsPlugin.AddIn";

            if (Directory.Exists(myDocDir + "\\Visual Studio 2005") && (!Directory.Exists(vs2005AddInDir)))
            {
                Directory.CreateDirectory(vs2005AddInDir);
            }

            if (Directory.Exists(vs2005AddInDir))
            {
                if (args != null && args.Length > 0)
                {
                    if (args[0].ToLower() == "-u")
                    {
                        File.Delete(addInFile);
                        MessageBox.Show("Uninstalled NBear VsPlugin successfully!");
                    }
                }
                else
                {
                    try
                    {
                        string[] existingAddIns = Directory.GetFiles(vs2005AddInDir);
                        if (existingAddIns != null)
                        {
                            foreach (string addin in existingAddIns)
                            {
                                if (addin.Contains("NBear.Tools.EntityDesignToEntityVsPlugin - For Testing.AddIn"))
                                {
                                    File.SetAttributes(addin, File.GetAttributes(addin) ^ FileAttributes.ReadOnly);
                                    File.Delete(addin);
                                    break;
                                }
                            }
                        }
                        string content = File.ReadAllText("NBear.Tools.EntityDesignToEntityVsPlugin.AddIn");
                        content = string.Format(content, AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'));
                        File.WriteAllText(addInFile, content);

                        MessageBox.Show("Installed NBear VsPlugin successfully!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("You must install Visual Studio 2005 correctly first.");
            }
        }
    }
}