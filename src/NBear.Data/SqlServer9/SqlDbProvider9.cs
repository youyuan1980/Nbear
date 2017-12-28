using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using NBear.Common;
using NBear.Data;

namespace NBear.Data.SqlServer9
{
    /// <summary>
    /// Db provider implementation for SQL Server 9.X (2005)
    /// </summary>
    public class SqlDbProvider9 : SqlServer.SqlDbProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SqlDatabase"/> class.
        /// </summary>
        /// <param name="connStr"></param>
        public SqlDbProvider9(string connStr)
            : base(connStr)
        {
        }

        /// <summary>
        /// When overridden in a derived class, creates an <see cref="IPageSplit"/> for a basic SQL select query.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="selectStatement">The text of the query.</param>
        /// <param name="keyColumn">The sigle main DEFAULT_KEY of the query.</param>
        /// <param name="paramValues">The param values of the query.</param>
        /// <returns>
        /// The <see cref="IPageSplit"/> for the SQL query.
        /// </returns>
        public override IPageSplit CreatePageSplit(Database db, string selectStatement, string keyColumn, params object[] paramValues)
        {
            return new SqlPageSplit9(db, selectStatement, keyColumn, paramValues);
        }
    }
}
