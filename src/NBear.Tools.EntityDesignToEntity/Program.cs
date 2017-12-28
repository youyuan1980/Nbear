using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NBear.Common;

namespace NBear.Tools.EntityDesignToEntity
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool slient = false;

            if (args != null && args.Length > 0)
            {
                //MessageBox.Show(args[0]);

                string designRootPath = args[0];
                string configFile = designRootPath + "\\EntityDesignToEntityConfig.xml";

                try
                {
                    XmlTextReader reader = new XmlTextReader(configFile);
                    XmlSerializer serializer = new XmlSerializer(typeof(EntityDesignToEntityConfiguration));
                    EntityDesignToEntityConfiguration config = (EntityDesignToEntityConfiguration)serializer.Deserialize(reader);

                    if (config != null)
                    {
                        Assembly ass = Assembly.LoadFrom(designRootPath + "\\bin\\" + config.CompileMode + "\\" + config.InputDllName);

                        if (!string.IsNullOrEmpty(config.EntityCodePath))
                        {
                            if (config.EntityCodePath.IndexOf(";") > 0)
                            {
                                string[] entityCodePaths = config.EntityCodePath.Split(';');
                                foreach (string entityCodePath in entityCodePaths)
                                {
                                    File.WriteAllText(Util.ParseRelativePath(designRootPath, entityCodePath), new CodeGenHelper(config.OutputNamespace, new AdvOptForm()).GenEntitiesEx(ass, config.OutputLanguage.ToLower() == "c#" ? 0 : 1));
                                }
                            }
                            else
                            {
                                File.WriteAllText(Util.ParseRelativePath(designRootPath, config.EntityCodePath), new CodeGenHelper(config.OutputNamespace, new AdvOptForm()).GenEntitiesEx(ass, config.OutputLanguage.ToLower() == "c#" ? 0 : 1));
                            }
                        }

                        if (!string.IsNullOrEmpty(config.EntityConfigPath))
                        {
                            if (config.EntityConfigPath.IndexOf(";") > 0)
                            {
                                string[] entityConfigPaths = config.EntityConfigPath.Split(';');
                                foreach (string entityConfigPath in entityConfigPaths)
                                {
                                    File.WriteAllText(Util.ParseRelativePath(designRootPath, entityConfigPath), new CodeGenHelper(config.OutputNamespace, new AdvOptForm()).GenEntityConfigurations(ass));
                                }
                            }
                            else
                            {
                                File.WriteAllText(Util.ParseRelativePath(designRootPath, config.EntityConfigPath), new CodeGenHelper(config.OutputNamespace, new AdvOptForm()).GenEntityConfigurations(ass));
                            }
                        }

                        if (config.SqlSync != null)
                        {
                            File.WriteAllText(designRootPath + "\\db.sql", string.Format("use {0}\r\nGO\r\n", config.SqlSync.DatabaseName) + new CodeGenHelper(config.OutputNamespace, new AdvOptForm()).GenDbScript(ass));

                            if (config.SqlSync.Enable)
                            {
                                string paramStr = string.Format("-S{0} -U{1} -P{2} -i\"{3}\" -o\"{4}\"", config.SqlSync.ServerName, config.SqlSync.UserID, config.SqlSync.Password, designRootPath + "\\db.sql", designRootPath + "\\db.log");
                                System.Diagnostics.Process.Start(config.SqlSync.SqlServerFolder.TrimEnd('\\') + "\\osql.exe", paramStr).WaitForExit();
                                string dbLog = System.IO.File.ReadAllText(designRootPath + "\\db.log");
                                if (new System.Text.RegularExpressions.Regex(@"[a-zA-Z]", System.Text.RegularExpressions.RegexOptions.Multiline).IsMatch(dbLog.Replace("affected", string.Empty).Replace("row", string.Empty).Replace("rows", string.Empty)))
                                {
                                    MessageBox.Show("Error raised when executing sql script, please check " + designRootPath + "\\db.log" + " for details.");
                                    System.Diagnostics.Process.Start(designRootPath + "\\db.log");
                                }
                            }
                        }
                    }

                    slient = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            if (!slient)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}