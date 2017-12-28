using System;
using System.Collections.Generic;
using System.Text;

namespace NBear.Data
{
    /// <summary>
    /// Base statement factory.
    /// </summary>
    public abstract class StatementFactory : IStatementFactory
    {
        #region IStatementFactory Members

        /// <summary>
        /// Creates the insert statement.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="includeColumns">The include columns.</param>
        /// <returns>The sql.</returns>
        public abstract string CreateInsertStatement(string tableName, params string[] includeColumns);

        /// <summary>
        /// Creates the update statement.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereStr">The where STR.</param>
        /// <param name="includeColumns">The include columns.</param>
        /// <returns>The sql.</returns>
        public abstract string CreateUpdateStatement(string tableName, string whereStr, params string[] includeColumns);

        /// <summary>
        /// Creates the delete statement.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereStr">The where STR.</param>
        /// <returns>The sql.</returns>
        public abstract string CreateDeleteStatement(string tableName, string whereStr);

        /// <summary>
        /// Creates the select statement.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereStr">The where STR.</param>
        /// <param name="orderByStr">The order by STR.</param>
        /// <param name="includeColumns">The include columns.</param>
        /// <returns>The sql.</returns>
        public abstract string CreateSelectStatement(string tableName, string whereStr, string orderByStr, params string[] includeColumns);

        #endregion
    }
}
