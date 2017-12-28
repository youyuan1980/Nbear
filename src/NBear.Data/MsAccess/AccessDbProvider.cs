using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Xml;

namespace NBear.Data.MsAccess
{
	/// <summary>
	/// <para>Represents a Access Server Database Provider.</para>
	/// </summary>
	/// <remarks> 
	/// <para>
	/// Internally uses Access Server .NET Managed Provider from Microsoft (System.Data.OleDb) to connect to the database.
	/// </para>  
	/// </remarks>
    public class AccessDbProvider : DbProvider
    {
        #region Private Members

        private const char PARAMETER_TOKEN = '@';
        private static AccessStatementFactory _StatementFactory = new AccessStatementFactory();

        #endregion

        #region Public Members

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessDbProvider"/> class.
        /// </summary>
        /// <param name="connStr">The conn STR.</param>
        public AccessDbProvider(string connStr)
            : base(connStr, OleDbFactory.Instance)
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

            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(PARAMETER_TOKEN + @"([\w\d_]+)");
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
            OleDbParameter oleDbParam = (OleDbParameter)param;
            object value = param.Value;
            DbType type = param.DbType;

            if (value == null || value == DBNull.Value)
            {
                oleDbParam.Value = DBNull.Value;
                if (oleDbParam.DbType != DbType.Binary && oleDbParam.DbType != DbType.Int32)
                {
                    oleDbParam.OleDbType = OleDbType.VarWChar;
                }
                return;
            }

            if (value.GetType().IsEnum)
            {
                oleDbParam.OleDbType = OleDbType.Integer;
                return;
            }

            if (value.GetType() == typeof(byte[]))
            {
                oleDbParam.OleDbType = OleDbType.LongVarBinary;
                return;
            }

            if (value.GetType() == typeof(Guid))
            {
                oleDbParam.OleDbType = OleDbType.VarChar;
                oleDbParam.Value = value.ToString();
                return;
            }

            if (value.GetType() == typeof(Byte) || value.GetType() == typeof(SByte) || 
                value.GetType() == typeof(Int16) || value.GetType() == typeof(Int32) || 
                value.GetType() == typeof(Int64) || value.GetType() == typeof(UInt16) || 
                value.GetType() == typeof(UInt32) || value.GetType() == typeof(UInt64))
            {
                oleDbParam.OleDbType = OleDbType.Integer;
                return;
            }

            if (value.GetType() == typeof(Single) || value.GetType() == typeof(Double))
            {
                oleDbParam.OleDbType = OleDbType.Double;
                return;
            }

            if (value.GetType() == typeof(Boolean))
            {
                oleDbParam.OleDbType = OleDbType.Boolean;
                return;
            }

            if (value.GetType() == typeof(Char))
            {
                oleDbParam.OleDbType = OleDbType.WChar;
                return;
            }

            if (value.GetType() == typeof(Decimal))
            {
                oleDbParam.OleDbType = OleDbType.Decimal;
                return;
            }

            //datetime is special here
            if (value.GetType() == typeof(DateTime) || type.Equals(DbType.DateTime) || 
                type.Equals(DbType.Date) || type.Equals(DbType.Time))
            {
                oleDbParam.OleDbType = OleDbType.LongVarWChar;
                oleDbParam.Value = value.ToString();
                return;
            }

            if (value.GetType() == typeof(string))
            {
                oleDbParam.OleDbType = OleDbType.VarWChar;
                if (value.ToString().Length > 2000)
                {
                    oleDbParam.OleDbType = OleDbType.LongVarWChar;
                }
                return;
            }

            //by default, threat as string
            oleDbParam.OleDbType = OleDbType.LongVarWChar;
            oleDbParam.Value = NBear.Common.SerializationManager.Serialize(value);
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
            return new AccessPageSplit(db, selectStatement, keyColumn, paramValues);
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
            string nameStr = name.Trim('[', ']');
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
            if ((!name.StartsWith("[")) && (!name.EndsWith("]")))
            {
                return "[" + name + "]";
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
                return "SELECT MAX({0}) from {1}";
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
            get { return "["; }
        }

        /// <summary>
        /// Gets the right token of table name or column name.
        /// </summary>
        /// <value>The right token.</value>
        public override string RightToken
        {
            get { return "]"; }
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