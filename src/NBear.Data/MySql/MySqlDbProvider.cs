using System;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Xml;

namespace NBear.Data.MySql
{
	/// <summary>
	/// <para>Represents a MySql Database Provider.</para>
	/// </summary>
	/// <remarks> 
	/// <para>
	/// Internally uses MySql Server .NET Managed Provider from Microsoft (System.Data.Odbc) to connect to the database.
	/// </para>  
	/// </remarks>
    public class MySqlDbProvider : DbProvider
    {
        #region Private Members

        private const char PARAMETER_TOKEN = '?';
        private static MySqlStatementFactory _StatementFactory = new MySqlStatementFactory();

        #endregion

        #region Public Members

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlDbProvider"/> class.
        /// </summary>
        /// <param name="connStr">The conn STR.</param>
        public MySqlDbProvider(string connStr)
            : base(connStr, OdbcFactory.Instance)
        {
        }

        /// <summary>
        /// Discovers the params.
        /// </summary>
        /// <param name="sql">The sql.</param>
        /// <returns></returns>
        public override string[] DiscoverParams(string sql)
        {
            if (sql == null)
            {
                return null;
            }

            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("\\" + PARAMETER_TOKEN + @"([\w\d_]+)");
            System.Text.RegularExpressions.MatchCollection ms = r.Matches(sql);

            if (ms.Count == 0)
            {
                return null;
            }

            string[] paramNames = new string[ms.Count];
            for (int i = 0; i < ms.Count; i++)
            {
                paramNames[i] = ms[i].Value;
            }
            return paramNames;
        }

        /// <summary>
        /// Adjust Db Parameter value
        /// </summary>
        /// <param name="param"></param>
        public override void AdjustParameter(DbParameter param)
        {
            OdbcParameter odbcParam = (OdbcParameter)param;
            object value = param.Value;
            DbType type = param.DbType;

            if (value == null || value == DBNull.Value)
            {
                odbcParam.Value = DBNull.Value;
                if (odbcParam.DbType != DbType.Binary && odbcParam.DbType != DbType.Int32)
                {
                    odbcParam.OdbcType = OdbcType.NVarChar;
                }
                return;
            }

            if (value.GetType().IsEnum)
            {
                odbcParam.OdbcType = OdbcType.Int;
                return;
            }

            if (value.GetType() == typeof(byte[]))
            {
                odbcParam.OdbcType = OdbcType.VarBinary;
                return;
            }

            if (value.GetType() == typeof(Guid))
            {
                odbcParam.OdbcType = OdbcType.VarChar;
                odbcParam.Value = value.ToString();
                return;
            }

            if (value.GetType() == typeof(Byte) || value.GetType() == typeof(SByte) || 
                value.GetType() == typeof(Int16) || value.GetType() == typeof(Int32) || 
                value.GetType() == typeof(Int64) || value.GetType() == typeof(UInt16) || 
                value.GetType() == typeof(UInt32) || value.GetType() == typeof(UInt64))
            {
                odbcParam.OdbcType = OdbcType.BigInt;
                return;
            }

            if (value.GetType() == typeof(Single) || value.GetType() == typeof(Double))
            {
                odbcParam.OdbcType = OdbcType.Double;
                return;
            }

            if (value.GetType() == typeof(Boolean))
            {
                odbcParam.OdbcType = OdbcType.Bit;
                odbcParam.Value = (((bool)value) ? 1 : 0);
                return;
            }

            if (value.GetType() == typeof(Char))
            {
                odbcParam.OdbcType = OdbcType.NVarChar;
                return;
            }

            if (value.GetType() == typeof(Decimal))
            {
                odbcParam.OdbcType = OdbcType.Decimal;
                return;
            }

            //datetime is special here
            if (value.GetType() == typeof(DateTime) || type.Equals(DbType.DateTime) || 
                type.Equals(DbType.Date) || type.Equals(DbType.Time))
            {
                odbcParam.OdbcType = OdbcType.NText;
                odbcParam.Value = value.ToString();
                return;
            }

            if (value.GetType() == typeof(string))
            {
                odbcParam.OdbcType = OdbcType.NVarChar;
                if (value.ToString().Length > 2000)
                {
                    odbcParam.OdbcType = OdbcType.NText;
                }
                return;
            }

            //by default, threat as string
            odbcParam.OdbcType = OdbcType.NText;
            odbcParam.Value = NBear.Common.SerializationManager.Serialize(value);
        }

        /// <summary>
        /// When overridden in a derived class, creates an <see cref="IPageSplit"/> for a SQL page splitable select query.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="selectStatement">The text of the basic select query for all rows.</param>
        /// <param name="keyColumn">The sigle main DEFAULT_KEY of the query.</param>
        /// <param name="paramValues">The param values of the query.</param>
        /// <returns>
        /// The <see cref="IPageSplit"/> for the SQL query.
        /// </returns>
        public override IPageSplit CreatePageSplit(Database db, string selectStatement, string keyColumn, object[] paramValues)
        {
            return new MySqlPageSplit(db, selectStatement, keyColumn, paramValues);
        }

        /// <summary>
        /// Creates the SQL statement factory.
        /// </summary>
        /// <returns></returns>
        public override IStatementFactory CreateStatementFactory()
        {
            return _StatementFactory;
        }

        /// <summary>
        /// Builds the name of the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override string BuildParameterName(string name)
        {
            string nameStr = name.Trim('`');
            if (nameStr[0] != PARAMETER_TOKEN)
            {
                return nameStr.Insert(0, new string(PARAMETER_TOKEN, 1));
            }
            return nameStr;
        }

        /// <summary>
        /// Builds the name of the column.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override string BuildColumnName(string name)
        {
            if ((!name.StartsWith("`")) && (!name.EndsWith("`")))
            {
                return "`" + name + "`";
            }
            return name;
        }

        /// <summary>
        /// Gets the select last inserted row auto ID statement.
        /// </summary>
        /// <value>The select last inserted row auto ID statement.</value>
        public override string SelectLastInsertedRowAutoIDStatement
        {
            get
            {
                return "\nSELECT MAX({0}) from {1}";
            }
        }

        /// <summary>
        /// Gets a value indicating whether [support AD o20 transaction].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [support AD o20 transaction]; otherwise, <c>false</c>.
        /// </value>
        public override bool SupportADO20Transaction
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the left token of table name or column name.
        /// </summary>
        /// <value>The left token.</value>
        public override string LeftToken
        {
            get { return "`"; }
        }

        /// <summary>
        /// Gets the right token of table name or column name.
        /// </summary>
        /// <value>The right token.</value>
        public override string RightToken
        {
            get { return "`"; }
        }

        /// <summary>
        /// Gets the param prefix.
        /// </summary>
        /// <value>The param prefix.</value>
        public override string ParamPrefix
        {
            get { return PARAMETER_TOKEN.ToString(); }
        }

        #endregion
    }
}