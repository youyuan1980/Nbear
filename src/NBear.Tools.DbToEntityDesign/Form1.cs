using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using ADODB;
using System.IO;

using NBear.Data;

namespace NBear.Tools.EntityGen
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (txtConnStr.Text.Trim().Length == 0)
            {
                MessageBox.Show("Connection string cannot be null!");
                return;
            }

            if (btnConnect.Text == "Disconnect")
            {
                EnableGenEntity(false);
                return;
            }

            RefreshConnectionStringAutoComplete();

            DataSet dsTables = null;
            DataSet dsViews = null;

            if (radioSql.Checked || radioAccess.Checked)
            {
                try
                {
                    if (radioSql.Checked)
                    {
                        Gateway.SetDefaultDatabase(DatabaseType.SqlServer, txtConnStr.Text);
                        if (checkSql2005.Checked)
                        {
                            dsTables = Gateway.Default.SelectDataSet("select [name] from sysobjects where xtype = 'U' and [name] <> 'sysdiagrams' order by [name]",null);
                        }
                        else
                        {
                            dsTables = Gateway.Default.SelectDataSet("select [name] from sysobjects where xtype = 'U' and status > 0 order by [name]", null);
                        }
                        foreach (DataRow row in dsTables.Tables[0].Rows)
                        {
                            tables.Items.Add(row["Name"].ToString());
                        }

                        if (checkSql2005.Checked)
                        {
                            dsViews = Gateway.Default.SelectDataSet("select [name] from sysobjects where xtype = 'V' order by [name]", null);
                        }
                        else
                        {
                            dsViews = Gateway.Default.SelectDataSet("select [name] from sysobjects where xtype = 'V' and status > 0 order by [name]", null);
                        }
                        foreach (DataRow row in dsViews.Tables[0].Rows)
                        {
                            views.Items.Add(row["Name"].ToString());
                        }
                    }
                    else if (radioAccess.Checked)
                    {
                        Gateway.SetDefaultDatabase(DatabaseType.MsAccess, txtConnStr.Text);
                        ADODB.ConnectionClass conn = new ADODB.ConnectionClass();
                        conn.Provider = "Microsoft.Jet.OLEDB.4.0";
                        string connStr = txtConnStr.Text;
                        conn.Open(connStr.Substring(connStr.ToLower().IndexOf("data source") + "data source".Length).Trim('=', ' '), null, null, 0);

                        ADODB.Recordset rsTables = conn.GetType().InvokeMember("OpenSchema", BindingFlags.InvokeMethod, null, conn, new object[] { ADODB.SchemaEnum.adSchemaTables }) as ADODB.Recordset;
                        ADODB.Recordset rsViews = conn.GetType().InvokeMember("OpenSchema", BindingFlags.InvokeMethod, null, conn, new object[] { ADODB.SchemaEnum.adSchemaViews }) as ADODB.Recordset;

                        while (!rsViews.EOF)
                        {
                            if (!(rsViews.Fields["TABLE_NAME"].Value as string).StartsWith("MSys"))
                            {
                                views.Items.Add(rsViews.Fields["TABLE_NAME"].Value.ToString());
                            }
                            rsViews.MoveNext();
                        }

                        while (!rsTables.EOF)
                        {
                            if (!(rsTables.Fields["TABLE_NAME"].Value as string).StartsWith("MSys"))
                            {
                                bool isView = false;
                                foreach (string item in views.Items)
                                {
                                    if (item.Equals(rsTables.Fields["TABLE_NAME"].Value.ToString()))
                                    {
                                        isView = true;
                                        break;
                                    }
                                }
                                if (!isView)
                                {
                                    tables.Items.Add(rsTables.Fields["TABLE_NAME"].Value.ToString());
                                }
                            }
                            rsTables.MoveNext();
                        }

                        rsTables.Close();
                        rsViews.Close();

                        conn.Close();
                    }

                    EnableGenEntity(true);
                }
                catch (Exception ex)
                {
                    EnableGenEntity(false);
                    MessageBox.Show("Read/write database error!\r\n" + ex.ToString());
                }
            }
            else if (radioOracle.Checked)
            {
                Gateway.SetDefaultDatabase(DatabaseType.Oracle, txtConnStr.Text);

                dsTables = Gateway.Default.SelectDataSet("select * from user_tables where global_stats = 'NO' and (not table_name like '%$%')", null);
                foreach (DataRow row in dsTables.Tables[0].Rows)
                {
                    tables.Items.Add(row["TABLE_NAME"].ToString());
                }

                dsViews = Gateway.Default.SelectDataSet("select * from user_views where (not view_name like '%$%') and (not view_name like 'MVIEW_%') and (not view_name like 'CTX_%') and (not view_name = 'PRODUCT_PRIVS')", null);
                foreach (DataRow row in dsViews.Tables[0].Rows)
                {
                    views.Items.Add(row["VIEW_NAME"].ToString());
                }

                EnableGenEntity(true);
            }
            else if (radioMySql.Checked)
            {
                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("(^.*database=)([^;]+)(;.*)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                string dbName = r.Replace(txtConnStr.Text, "$2").ToLower();
                Gateway.SetDefaultDatabase(DatabaseType.MySql, r.Replace(txtConnStr.Text, "$1information_schema$3"));

                dsTables = Gateway.Default.SelectDataSet("select * from TABLES where TABLE_TYPE = 'BASE TABLE' and TABLE_SCHEMA = '" + dbName + "'", null);
                foreach (DataRow row in dsTables.Tables[0].Rows)
                {
                    tables.Items.Add(row["TABLE_NAME"].ToString());
                }

                dsViews = Gateway.Default.SelectDataSet("select * from TABLES where TABLE_TYPE = 'VIEW' and TABLE_SCHEMA = '" + dbName + "'", null);
                foreach (DataRow row in dsViews.Tables[0].Rows)
                {
                    views.Items.Add(row["TABLE_NAME"].ToString());
                }

                EnableGenEntity(true);
            }
            else
            {
                EnableGenEntity(false);
                MessageBox.Show("EntityGen tool only supports SqlServer, MsAccess, MySql and Oracle Database!");
            }
        }

        private AutoCompleteStringCollection connStrs = new AutoCompleteStringCollection();
        private const string CONN_STR_HIS_File = "ConnectionStringsHistory.txt";

        public void RefreshConnectionStringAutoComplete()
        {
            if (!string.IsNullOrEmpty(txtConnStr.Text))
            {
                if (!connStrs.Contains(txtConnStr.Text))
                {
                    connStrs.Add(txtConnStr.Text);
                }
            }
        }

        private void SaveConnectionStringAutoComplete()
        {
            StreamWriter sw = new StreamWriter(CONN_STR_HIS_File);

            foreach (string line in connStrs)
            {
                sw.WriteLine(line);
            }

            sw.Close();
        }

        private void LoadConnectionStringAutoComplete()
        {
            if (File.Exists(CONN_STR_HIS_File))
            {
                connStrs.Clear();

                StreamReader sr = new StreamReader(CONN_STR_HIS_File);
                while (!sr.EndOfStream)
                {
                    connStrs.Add(sr.ReadLine().Trim());
                }
                sr.Close();
            }

            txtConnStr.AutoCompleteCustomSource = connStrs;
        }

        private void EnableGenEntity(bool enable)
        {
            if (enable)
            {
                btnGen.Enabled = true;
                txtConnStr.Enabled = false;
                btnConnect.Text = "Disconnect";

                radioAccess.Enabled = false;
                radioMySql.Enabled = false;
                radioOracle.Enabled = false;
                radioSql.Enabled = false;
            }
            else
            {
                btnGen.Enabled = false;
                txtConnStr.Enabled = true;
                btnConnect.Text = "Connect";
                selectAll.Checked = false;
                tables.Items.Clear();
                views.Items.Clear();
                output.Text = "";

                radioAccess.Enabled = true;
                radioMySql.Enabled = true;
                radioOracle.Enabled = true;
                radioSql.Enabled = true;
            }
        }

        private void selectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (selectAll.Checked)
            {
                for (int i = 0; i < tables.Items.Count; i++ )
                {
                    tables.SetItemChecked(i, true);
                }
                for (int i = 0; i < views.Items.Count; i++)
                {
                    views.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < tables.Items.Count; i++)
                {
                    tables.SetItemChecked(i, false);
                }
                for (int i = 0; i < views.Items.Count; i++)
                {
                    views.SetItemChecked(i, false);
                }
            }
        }

        private string GenType(string typeStr)
        {
            if (typeStr == typeof(string).ToString())
            {
                return "string";
            }
            else if (typeStr == typeof(int).ToString())
            {
                return "int";
            }
            else if (typeStr == typeof(long).ToString())
            {
                return "long";
            }
            else if (typeStr == typeof(short).ToString())
            {
                return "short";
            }
            else if (typeStr == typeof(byte).ToString())
            {
                return "byte";
            }
            else if (typeStr == typeof(byte[]).ToString())
            {
                return "byte[]";
            }
            else if (typeStr == typeof(bool).ToString())
            {
                return "bool";
            }
            else if (typeStr == typeof(decimal).ToString())
            {
                return "decimal";
            }
            else if (typeStr == typeof(char).ToString())
            {
                return "char";
            }
            else if (typeStr == typeof(sbyte).ToString())
            {
                return "sbyte";
            }
            else if (typeStr == typeof(float).ToString())
            {
                return "float";
            }
            else if (typeStr == typeof(double).ToString())
            {
                return "double";
            }
            else if (typeStr == typeof(object).ToString())
            {
                return "object";
            }
            else if (typeStr == typeof(Guid).ToString())
            {
                return "Guid";
            }
            else if (typeStr == typeof(DateTime).ToString())
            {
                return "DateTime";
            }
            else
            {
                return typeStr;
            }
        }

        private string GenTypeVB(string typeStr)
        {
            if (typeStr == typeof(string).ToString())
            {
                return "String";
            }
            else if (typeStr == typeof(int).ToString())
            {
                return "Integer";
            }
            else if (typeStr == typeof(uint).ToString())
            {
                return "UInteger";
            }
            else if (typeStr == typeof(long).ToString())
            {
                return "Long";
            }
            else if (typeStr == typeof(ulong).ToString())
            {
                return "ULong";
            }
            else if (typeStr == typeof(short).ToString())
            {
                return "Short";
            }
            else if (typeStr == typeof(ushort).ToString())
            {
                return "UShort";
            }
            else if (typeStr == typeof(byte).ToString())
            {
                return "Byte";
            }
            else if (typeStr == typeof(byte[]).ToString())
            {
                return "Byte()";
            }
            else if (typeStr == typeof(bool).ToString())
            {
                return "Boolean";
            }
            else if (typeStr == typeof(decimal).ToString())
            {
                return "Decimal";
            }
            else if (typeStr == typeof(char).ToString())
            {
                return "Char";
            }
            else if (typeStr == typeof(sbyte).ToString())
            {
                return "SByte";
            }
            else if (typeStr == typeof(Single).ToString())
            {
                return "Single";
            }
            else if (typeStr == typeof(double).ToString())
            {
                return "Double";
            }
            else if (typeStr == typeof(object).ToString())
            {
                return "Object";
            }
            else if (typeStr == typeof(Guid).ToString())
            {
                return "Guid";
            }
            else if (typeStr == typeof(DateTime).ToString())
            {
                return "Date";
            }
            else
            {
                return typeStr.Replace("[", "(").Replace("]", ")");
            }
        }

        private string GenEntity(string name, bool isView)
        {
            DataSet ds = null;

            if (radioAccess.Checked || radioSql.Checked)
            {
                ds = Gateway.Default.SelectDataSet(string.Format("select * from [{0}] where 1 = 2", name), null);
            }
            else if (radioMySql.Checked)
            {
                Gateway dbGateway = new Gateway(DatabaseType.MySql, txtConnStr.Text);
                ds = dbGateway.SelectDataSet(string.Format("select * from `{0}` where 1 = 2", name), null);
            }
            else
            {
                ds = Gateway.Default.SelectDataSet(string.Format("select * from \"{0}\" where 1 = 2", name), null);
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("\r\n");

            if (outputLanguage.SelectedIndex == 0)
            {
                if (isView)
                {
                    sb.Append(string.Format("    [ReadOnly]\r\n"));
                }
                sb.Append(string.Format("    public interface {0} : Entity\r\n", UpperFirstChar(ParseTableName(name))));
                sb.Append("    {\r\n");

                foreach (DataColumn column in ds.Tables[0].Columns)
                {
                    if (IsColumnPrimaryKey(name, column.ColumnName))
                    {
                        sb.Append(string.Format("        [PrimaryKey]\r\n"));
                    }
                    if (radioSql.Checked)
                    {
                        GenSqlColumnType(sb, name, column.ColumnName);
                    }
                    sb.Append(string.Format("        {0}{2} {1} ", GenType(column.DataType.ToString()), UpperFirstChar(column.ColumnName), (column.DataType.IsValueType && IsColumnNullable(name, column.ColumnName) ? "?" : "")));
                    if (isView || IsColumnReadOnly(name, column.ColumnName))
                    {
                        sb.Append("{ get; }\r\n");
                    }
                    else
                    {
                        sb.Append("{ get; set; }\r\n");
                    }
                }

                sb.Append("    }\r\n");
            }
            else if (outputLanguage.SelectedIndex == 1)
            {
                if (isView)
                {
                    sb.Append(string.Format("    <NBear.Common.Design.ReadOnly()> _\r\n"));
                }
                sb.Append(string.Format("    Public Interface {0}\r\n    Inherits Entity\r\n", UpperFirstChar(ParseTableName(name))));

                foreach (DataColumn column in ds.Tables[0].Columns)
                {
                    if (IsColumnPrimaryKey(name, column.ColumnName))
                    {
                        sb.Append(string.Format("        <PrimaryKey()> _\r\n"));
                    }
                    if (radioSql.Checked)
                    {
                        GenSqlColumnType(sb, name, column.ColumnName);
                    }
                    sb.Append("        ");
                    if (isView || IsColumnReadOnly(name, column.ColumnName))
                    {
                        sb.Append("ReadOnly ");
                    }
                    if (column.DataType.IsValueType && IsColumnNullable(name, column.ColumnName))
                    {
                        sb.Append(string.Format("Property {1}() As Nullable(Of {0})\r\n", GenTypeVB(column.DataType.ToString()), UpperFirstChar(column.ColumnName)));
                    }
                    else
                    {
                        sb.Append(string.Format("Property {1}() As {0}\r\n", GenTypeVB(column.DataType.ToString()), UpperFirstChar(column.ColumnName)));
                    }
                }

                sb.Append("    End Interface\r\n");
            }

            return sb.ToString();
        }

        private void GenSqlColumnType(StringBuilder sb, string name, string column)
        {
            int tableid = Gateway.Default.SelectScalar<int>("select id from sysobjects where [name] = @name", new object[] { name });
            DataSet ds = Gateway.Default.SelectDataSet("select xtype, length  from syscolumns where id = @id and name = @name", new object[] { tableid, column });
            switch (int.Parse(ds.Tables[0].Rows[0]["xtype"].ToString()))
            {
                case 231:
                    if (outputLanguage.SelectedIndex == 0)
                    {
                        sb.Append(string.Format("        [SqlType(\"{0}\")]\r\n", string.Format("nvarchar({0})", int.Parse(ds.Tables[0].Rows[0]["length"].ToString()) / 2)));
                    }
                    else if (outputLanguage.SelectedIndex == 1)
                    {
                        sb.Append(string.Format("        <SqlType(\"{0}\")> _\r\n", string.Format("nvarchar({0})", int.Parse(ds.Tables[0].Rows[0]["length"].ToString()) / 2)));
                    }
                    break;
                case 239:
                    if (outputLanguage.SelectedIndex == 0)
                    {
                        sb.Append(string.Format("        [SqlType(\"{0}\")]\r\n", string.Format("nchar({0})", int.Parse(ds.Tables[0].Rows[0]["length"].ToString()) / 2)));
                    }
                    else if (outputLanguage.SelectedIndex == 1)
                    {
                        sb.Append(string.Format("        <SqlType(\"{0}\")> _\r\n", string.Format("nchar({0})", int.Parse(ds.Tables[0].Rows[0]["length"].ToString()) / 2)));
                    }
                    break;
                case 99:
                    if (outputLanguage.SelectedIndex == 0)
                    {
                        sb.Append(string.Format("        [SqlType(\"ntext\")]\r\n"));
                    }
                    else if (outputLanguage.SelectedIndex == 1)
                    {
                        sb.Append(string.Format("        <SqlType(\"ntext\")> _\r\n"));
                    }
                    break;
            }
        }

        private bool IsColumnPrimaryKey(string name, string column)
        {
            if (radioSql.Checked)
            {
                int tableid = Gateway.Default.SelectScalar<int>("select id from sysobjects where [name] = @name", new object[] { name });
                DataSet ds = Gateway.Default.SelectDataSet("select a.name FROM syscolumns a inner join sysobjects d on a.id=d.id and d.xtype='U' and d.name<>'dtproperties' where (SELECT count(*) FROM sysobjects WHERE (name in (SELECT name FROM sysindexes WHERE (id = a.id) AND (indid in (SELECT indid FROM sysindexkeys WHERE (id = a.id) AND (colid in (SELECT colid FROM syscolumns WHERE (id = a.id) AND (name = a.name))))))) AND (xtype = 'PK'))>0 and d.id = @id", new object[] { tableid });
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString() == column)
                    {
                        return true;
                    }
                }
            }
            else if (radioOracle.Checked)
            {
                DataSet ds = Gateway.Default.SelectDataSet("select b.COLUMN_NAME from USER_CONSTRAINTS a,USER_CONS_COLUMNS b where a.CONSTRAINT_NAME=b.CONSTRAINT_NAME and a.table_name=b.table_name and constraint_type='P' and a.owner=b.owner and a.table_name = :name", new object[] { name });
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString() == column)
                    {
                        return true;
                    }
                }
            }
            else if (radioMySql.Checked)
            {
                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("(^.*database=)([^;]+)(;.*)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                string dbName = r.Replace(txtConnStr.Text, "$2").ToLower();

                DataSet ds = Gateway.Default.SelectDataSet("select COLUMN_NAME from KEY_COLUMN_USAGE where CONSTRAINT_SCHEMA = '" + dbName + "' and CONSTRAINT_NAME = 'PRIMARY' and TABLE_NAME = ?TABLE_NAME", new object[] { name });
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString() == column)
                    {
                        return true;
                    }
                }
            }
            else
            {
                ADODB.ConnectionClass conn = new ADODB.ConnectionClass();
                conn.Provider = "Microsoft.Jet.OLEDB.4.0";
                string connStr = txtConnStr.Text;
                conn.Open(connStr.Substring(connStr.ToLower().IndexOf("data source") + "data source".Length).Trim('=', ' '), null, null, 0);

                ADODB.Recordset rs = conn.GetType().InvokeMember("OpenSchema", BindingFlags.InvokeMethod, null, conn, new object[] { ADODB.SchemaEnum.adSchemaPrimaryKeys }) as ADODB.Recordset;
                rs.Filter = "TABLE_NAME='" + name + "'";

                while (!rs.EOF)
                {
                    if ((rs.Fields["COLUMN_NAME"].Value as string) == column)
                    {
                        return true;
                    }

                    rs.MoveNext();
                }
            }

            return false;
        }

        private bool IsColumnReadOnly(string name, string column)
        {
            if (radioSql.Checked)
            {
                int tableid = Gateway.Default.SelectScalar<int>("select id from sysobjects where [name] = @name", new object[] { name });
                byte status = Gateway.Default.SelectScalar<byte>("select status from syscolumns where [name] = @name and id = @id", new object[] { column, tableid });
                return status == 128;
            }
            else if (radioOracle.Checked)
            {
                return false;
            }
            else if (radioMySql.Checked)
            {
                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("(^.*database=)([^;]+)(;.*)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                string dbName = r.Replace(txtConnStr.Text, "$2").ToLower();

                DataSet ds = Gateway.Default.SelectDataSet("select EXTRA from COLUMNS where TABLE_SCHEMA = '" + dbName + "' and COLUMN_NAME = '" + column + "' and TABLE_NAME = ?TABLE_NAME", new object[] { name });
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString() == "auto_increment")
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                ADODB.ConnectionClass conn = new ADODB.ConnectionClass();
                conn.Provider = "Microsoft.Jet.OLEDB.4.0";
                string connStr = txtConnStr.Text;
                conn.Open(connStr.Substring(connStr.ToLower().IndexOf("data source") + "data source".Length).Trim('=', ' '), null, null, 0);

                ADODB.Recordset rs = conn.GetType().InvokeMember("OpenSchema", BindingFlags.InvokeMethod, null, conn, new object[] { ADODB.SchemaEnum.adSchemaColumns }) as ADODB.Recordset;
                rs.Filter = "TABLE_NAME='" + name + "'";

                while (!rs.EOF)
                {
                    if ((rs.Fields["COLUMN_NAME"].Value as string) == column && ((int)rs.Fields["DATA_TYPE"].Value) == 3 && Convert.ToByte(rs.Fields["COLUMN_FLAGS"].Value) == 90)
                    {
                        return true;
                    }

                    rs.MoveNext();
                }
            }

            return false;
        }

        private bool IsColumnNullable(string name, string column)
        {
            if (radioSql.Checked)
            {
                int tableid = Gateway.Default.SelectScalar<int>("select id from sysobjects where [name] = @name", new object[] { name });
                int isnullable = Convert.ToInt32(Gateway.Default.SelectScalar<object>("select isnullable from syscolumns where [name] = @name and id = @id", new object[] { column, tableid }));
                return isnullable == 1;
            }
            else
            {
                return false;
            }
        }

        private static string ParseTableName(string name)
        {
            return name.Trim().Replace(" ", "_nbsp_");
        }

        private void btnGen_Click(object sender, EventArgs e)
        {
            output.Text = "";

            foreach (string table in tables.CheckedItems)
            {
                output.Text += GenEntity(table, false);
            }

            foreach (string view in views.CheckedItems)
            {
                output.Text += GenEntity(view, true);
            }
        }

        private string UpperFirstChar(string str)
        {
            if (!checkUpperFirstChar.Checked)
            {
                return str;
            }

            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            else if (str.Length > 1)
            {
                return str.Substring(0, 1).ToUpper() + str.Substring(1);
            }
            else
            {
                return str.ToUpper();
            }
        }

        private void copyAllToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(output.Text);
        }

        private void txtConnStr_TextChanged(object sender, EventArgs e)
        {
            if (txtConnStr.Text.ToLower().Contains("microsoft.jet.oledb"))
            {
                radioAccess.Checked = true;
                checkSql2005.Enabled = false;
            }
            else if (txtConnStr.Text.ToLower().Contains("data source") && txtConnStr.Text.ToLower().Contains("user id") && txtConnStr.Text.ToLower().Contains("password"))
            {
                radioOracle.Checked = true;
                checkSql2005.Enabled = false;
            }
            else if (txtConnStr.Text.ToLower().Contains("dsn="))
            {
                radioMySql.Checked = true;
                checkSql2005.Enabled = false;
            }
            else
            {
                radioSql.Checked = true;
                checkSql2005.Enabled = true;
            }
        }

        private void radioSql_CheckedChanged(object sender, EventArgs e)
        {
            checkSql2005.Enabled = radioSql.Checked;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            outputLanguage.SelectedIndex = 0;

            LoadConnectionStringAutoComplete();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveConnectionStringAutoComplete();
        }


        #region 新增
        private void button1_Click(object sender, EventArgs e)
        {
            output.Text = "";
            output.Text += "using System;\r\n";
            output.Text += "using System.Collections.Generic;\r\n";
            output.Text += "using System.Text;\r\n";
            output.Text += "using NBear.Data;\r\n";
            output.Text += "using NBear.Common;\r\n";
            output.Text += "using System.Data;\r\n";
            output.Text += "using System.Data.Common;\r\n";
            output.Text += "\r\n";
            output.Text += "\r\n";
            output.Text += "namespace DAL\r\n";
            output.Text += "{\r\n";
            foreach (string table in tables.CheckedItems)
            {
                output.Text += GenEntityCode(table, false);
            }

            foreach (string view in views.CheckedItems)
            {
                output.Text += GenEntityCode(view, true);
            }
            output.Text += "}";
        }

        private string GenEntityCode(string name, bool isView)
        {
            DataSet ds = null;

            if (radioAccess.Checked || radioSql.Checked)
            {
                ds = Gateway.Default.SelectDataSet(string.Format("select * from [{0}] where 1 = 2", name), null);
            }
            else if (radioMySql.Checked)
            {
                Gateway dbGateway = new Gateway(DatabaseType.MySql, txtConnStr.Text);
                ds = dbGateway.SelectDataSet(string.Format("select * from `{0}` where 1 = 2", name), null);
            }
            else
            {
                ds = Gateway.Default.SelectDataSet(string.Format("select * from \"{0}\" where 1 = 2", name), null);
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("\r\n");


            if (outputLanguage.SelectedIndex == 0)
            {
                string keyColnumnName = string.Empty;
                string keyColnumnType = string.Empty;
                foreach (DataColumn column in ds.Tables[0].Columns)
                {
                    if (IsColumnPrimaryKey(name, column.ColumnName))
                    {
                        keyColnumnName = column.ColumnName;
                        keyColnumnType = GenType(column.DataType.ToString());
                        break;
                    }
                }

                sb.Append("#region " + name + "\r\n");
                sb.Append("public class " + name + "DAL:DalBase<" + name + "," + keyColnumnType + ">\r\n");
                sb.Append("{\r\n");
                sb.Append("    #region 查找信息\r\n");
                sb.Append("    public static DataSet Find" + name + "(string expression, int PageSize, int PageIndex, out int RowCount)\r\n");
                sb.Append("    {\r\n");
                sb.Append("        string SelectSql = \"SELECT * from view_" + name + "\";\r\n");
                sb.Append("        if (expression != null && expression != string.Empty)\r\n");
                sb.Append("        {\r\n");
                sb.Append("            SelectSql = SelectSql + \" WHERE \" + expression;\r\n");
                sb.Append("        }\r\n");
                sb.Append("        Gateway gateway = Gateway.Default;\r\n");
                sb.Append("        IPageSplit PageSplit = gateway.Db.GetPageSplit(SelectSql, \"" + keyColnumnName + "\", null);\r\n");
                sb.Append("        PageSplit.PageSize = PageSize;\r\n");
                sb.Append("        RowCount = PageSplit.GetRowCount();\r\n");
                sb.Append("        DataSet ds = PageSplit.GetPage(PageIndex);\r\n");
                sb.Append("        return ds;\r\n");
                sb.Append("    }\r\n");
                sb.Append("    \r\n");
                sb.Append("    \r\n");
                sb.Append("    public static DataSet Find" + name + "(string expression)\r\n");
                sb.Append("    {\r\n");
                sb.Append("        string SelectSql = \"SELECT * from view_" + name + "\";\r\n");
                sb.Append("        if (expression != null && expression != string.Empty)\r\n");
                sb.Append("        {\r\n");
                sb.Append("            SelectSql = SelectSql + \" WHERE \" + expression;\r\n");
                sb.Append("        }\r\n");
                sb.Append("        Gateway gateway = Gateway.Default;\r\n");
                sb.Append("        return gateway.Db.ExecuteDataSet(CommandType.Text, SelectSql);\r\n");
                sb.Append("    }\r\n");
                sb.Append("    #endregion\r\n");
                sb.Append("}\r\n");
                sb.Append("#endregion\r\n");
            }
            return sb.ToString();
        }
        #endregion
    }
}