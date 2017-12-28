using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace NBear.Data
{
    /// <summary>
    /// The base class of all db providers.
    /// </summary>
    public abstract class DbProvider
    {
        #region Protected Members

        /// <summary>
        /// The db provider factory.
        /// </summary>
        protected System.Data.Common.DbProviderFactory dbProviderFactory;
        /// <summary>
        /// The db connection string builder
        /// </summary>
        protected DbConnectionStringBuilder dbConnStrBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DbProvider"/> class.
        /// </summary>
        /// <param name="connStr">The conn STR.</param>
        /// <param name="dbProviderFactory">The db provider factory.</param>
        protected DbProvider(string connStr, System.Data.Common.DbProviderFactory dbProviderFactory)
        {
            dbConnStrBuilder = new DbConnectionStringBuilder();
            dbConnStrBuilder.ConnectionString = connStr;
            this.dbProviderFactory = dbProviderFactory;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get
            {
                return dbConnStrBuilder.ConnectionString;
            }
        }

        /// <summary>
        /// Gets the db provider factory.
        /// </summary>
        /// <value>The db provider factory.</value>
        public System.Data.Common.DbProviderFactory DbProviderFactory
        {
            get
            {
                return dbProviderFactory;
            }
        }

        #endregion

        #region Abstract Members

        /// <summary>
        /// Adjusts the parameter.
        /// </summary>
        /// <param name="param">The param.</param>
        public abstract void AdjustParameter(DbParameter param);

        /// <summary>
        /// Creates the page split.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="selectStatement">The select statement.</param>
        /// <param name="keyColumn">The DEFAULT_KEY column.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns></returns>
        public abstract IPageSplit CreatePageSplit(Database db, string selectStatement, string keyColumn, object[] paramValues);

        /// <summary>
        /// Creates the SQL statement factory.
        /// </summary>
        /// <returns></returns>
        public abstract IStatementFactory CreateStatementFactory();

        /// <summary>
        /// Discovers params from SQL text.
        /// </summary>
        /// <param name="sql">The full or part of SQL text.</param>
        /// <returns>The discovered params.</returns>
        public abstract string[] DiscoverParams(string sql);

        /// <summary>
        /// Builds the name of the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public abstract string BuildParameterName(string name);

        /// <summary>
        /// Builds the name of the column.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public abstract string BuildColumnName(string name);

        /// <summary>
        /// Gets the select last inserted row auto ID statement.
        /// </summary>
        /// <value>The select last inserted row auto ID statement.</value>
        public abstract string SelectLastInsertedRowAutoIDStatement
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether [support AD o20 transaction].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [support AD o20 transaction]; otherwise, <c>false</c>.
        /// </value>
        public abstract bool SupportADO20Transaction
        {
            get;
        }

        /// <summary>
        /// Gets the param prefix.
        /// </summary>
        /// <value>The param prefix.</value>
        public abstract string ParamPrefix { get; }

        /// <summary>
        /// Gets the left token of table name or column name.
        /// </summary>
        /// <value>The left token.</value>
        public abstract string LeftToken { get; }

        /// <summary>
        /// Gets the right token of table name or column name.
        /// </summary>
        /// <value>The right token.</value>
        public abstract string RightToken { get; }

        #endregion
    }
}
