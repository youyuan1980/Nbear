using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Xml;
using NBear.Common;
using NBear.Data;

namespace NBear.Data.SqlServer
{
    /// <summary>
    /// <para>Represents a Sql Server Database.</para>
    /// </summary>
    /// <remarks> 
    /// <para>
    /// Internally uses Sql Server .NET Managed Provider from Microsoft (System.Data.SqlClient) to connect to the database.
    /// </para>  
    /// </remarks>
    public class SqlDbProvider : DbProvider
    {
        #region Private Members

        private const char PARAMETER_TOKEN = '@';
        private static SqlStatementFactory _StatementFactory = new SqlStatementFactory();

        #endregion

        #region Public Members

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDbProvider"/> class.
        /// </summary>
        /// <param name="connStr">The conn STR.</param>
        public SqlDbProvider(string connStr)
            : base(connStr, SqlClientFactory.Instance)
        {
        }

        /// <summary>
        /// Creates the page split.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="selectStatement">The select statement.</param>
        /// <param name="keyColumn">The DEFAULT_KEY column.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns></returns>
        public override IPageSplit CreatePageSplit(Database db, string selectStatement, string keyColumn, params object[] paramValues)
        {
            return new SqlPageSplit(db, selectStatement, keyColumn, paramValues);
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
        /// Creates the SQL statement factory.
        /// </summary>
        /// <returns></returns>
        public override IStatementFactory CreateStatementFactory()
        {
            return _StatementFactory;
        }

        /// <summary>
        /// Adjusts the parameter.
        /// </summary>
        /// <param name="param">The param.</param>
        public override void AdjustParameter(DbParameter param)
        {
            SqlParameter sqlParam = (SqlParameter)param;
            object value = param.Value;
            DbType type = param.DbType;

            if (value == null || value == DBNull.Value)
            {
                sqlParam.Value = DBNull.Value;
                if (sqlParam.DbType != DbType.Binary && sqlParam.DbType != DbType.Int32)
                {
                    sqlParam.SqlDbType = SqlDbType.NVarChar;
                }
                return;
            }

            if (value.GetType() == typeof(byte[]))
            {
                sqlParam.SqlDbType = SqlDbType.Image;
                return;
            }

            if (value.GetType().IsEnum)
            {
                sqlParam.SqlDbType = SqlDbType.Int;
                return;
            }

            if (value.GetType() == typeof(Guid))
            {
                sqlParam.SqlDbType = SqlDbType.UniqueIdentifier;
                return;
            }

            if (value.GetType() == typeof(Byte) || value.GetType() == typeof(SByte) || 
                value.GetType() == typeof(Int16) || value.GetType() == typeof(Int32) || 
                value.GetType() == typeof(Int64) || value.GetType() == typeof(UInt16) || 
                value.GetType() == typeof(UInt32) || value.GetType() == typeof(UInt64))
            {
                sqlParam.SqlDbType = SqlDbType.Int;
                return;
            }

            if (value.GetType() == typeof(Single) || value.GetType() == typeof(Double))
            {
                sqlParam.SqlDbType = SqlDbType.Float;
                return;
            }

            if (value.GetType() == typeof(Boolean))
            {
                sqlParam.SqlDbType = SqlDbType.Bit;
                sqlParam.Value = (((bool)value) ? 1 : 0);
                return;
            }

            if (value.GetType() == typeof(Char))
            {
                sqlParam.SqlDbType = SqlDbType.NChar;
                return;
            }

            if (value.GetType() == typeof(Decimal))
            {
                sqlParam.SqlDbType = SqlDbType.Decimal;
                return;
            }

            if (value.GetType() == typeof(DateTime))
            {
                sqlParam.SqlDbType = SqlDbType.DateTime;
                return;
            }

            if (value.GetType() == typeof(string))
            {
                sqlParam.SqlDbType = SqlDbType.NVarChar;
                if (value.ToString().Length > 2000)
                {
                    sqlParam.SqlDbType = SqlDbType.Text;
                }
                return;
            }

            //by default, threat as string
            sqlParam.SqlDbType = SqlDbType.NText;
            sqlParam.Value = NBear.Common.SerializationManager.Serialize(value);
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
                return "SELECT SCOPE_IDENTITY()";
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
            get { return true; }
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