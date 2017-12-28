using System;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Xml;
using NBear.Common;
using NBear.Data;

namespace NBear.Data.Oracle
{
    /// <summary>
    /// <para>Represents an Oracle Database.</para>
    /// </summary>
    /// <remarks> 
    /// <para>
    /// Internally uses Oracle .NET Managed Provider from Microsoft (System.Data.OracleClient) to connect to the database.
    /// </para>  
    /// </remarks>
    public class OracleDbProvider : DbProvider
    {
        #region Private Members

        private const char PARAMETER_TOKEN = ':';
        private static OracleStatementFactory _StatementFactory = new OracleStatementFactory();

        #endregion

        #region Public Members

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDbProvider"/> class.
        /// </summary>
        /// <param name="connStr">The conn STR.</param>
        public OracleDbProvider(string connStr)
            : base(connStr, OracleClientFactory.Instance)
        {
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
        public override IPageSplit CreatePageSplit(Database db, string selectStatement, string keyColumn, params object[] paramValues)
        {
            return new OraclePageSplit(db, selectStatement, keyColumn, paramValues);
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
            OracleParameter oracleParam = (OracleParameter)param;
            object value = param.Value;
            DbType type = param.DbType;

            if (value == null || value == DBNull.Value)
            {
                oracleParam.Value = DBNull.Value;
                if (oracleParam.DbType != DbType.Binary && oracleParam.DbType != DbType.Int32)
                {
                    oracleParam.OracleType = OracleType.NVarChar;
                }
                return;
            }

            if (value.GetType().IsEnum)
            {
                oracleParam.OracleType = OracleType.Int32;
                return;
            }

            if (value.GetType() == typeof(byte[]))
            {
                oracleParam.OracleType = OracleType.Blob;
                return;
            }

            if (value.GetType() == typeof(Guid))
            {
                oracleParam.OracleType = OracleType.VarChar;
                oracleParam.Value = value.ToString();
                return;
            }

            if (value.GetType() == typeof(Byte) || value.GetType() == typeof(SByte) ||
                value.GetType() == typeof(Int16) || value.GetType() == typeof(Int32) ||
                value.GetType() == typeof(Int64) || value.GetType() == typeof(UInt16) ||
                value.GetType() == typeof(UInt32) || value.GetType() == typeof(UInt64))
            {
                oracleParam.OracleType = OracleType.Number;
                return;
            }

            if (value.GetType() == typeof(Single) || value.GetType() == typeof(Double))
            {
                oracleParam.OracleType = OracleType.Float;
                return;
            }

            if (value.GetType() == typeof(Boolean))
            {
                oracleParam.OracleType = OracleType.Int16;
                oracleParam.Value = (((bool)value) ? 1 : 0);
                return;
            }

            if (value.GetType() == typeof(Char))
            {
                oracleParam.OracleType = OracleType.NChar;
                return;
            }

            if (value.GetType() == typeof(Decimal))
            {
                oracleParam.OracleType = OracleType.Number;
                return;
            }

            if (value.GetType() == typeof(DateTime))
            {
                oracleParam.OracleType = OracleType.DateTime;
                return;
            }

            if (value.GetType() == typeof(string))
            {
                oracleParam.OracleType = OracleType.NVarChar;
                if (value.ToString().Length > 2000)
                {
                    oracleParam.OracleType = OracleType.NClob;
                }
                return;
            }

            //by default, threat as string
            oracleParam.OracleType = OracleType.NClob;
            oracleParam.Value = NBear.Common.SerializationManager.Serialize(value);
        }

        /// <summary>
        /// Builds the name of the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override string BuildParameterName(string name)
        {
            string nameStr = name.Trim('\"');
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
            return "\"" + name + "\"";
        }

        /// <summary>
        /// Gets the select last inserted row auto ID statement.
        /// </summary>
        /// <value>The select last inserted row auto ID statement.</value>
        public override string SelectLastInsertedRowAutoIDStatement
        {
            get
            {
                return "SELECT SEQ_{1}.nextval FROM DUAL";
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
            get { return "\""; }
        }

        /// <summary>
        /// Gets the right token of table name or column name.
        /// </summary>
        /// <value>The right token.</value>
        public override string RightToken
        {
            get { return "\""; }
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