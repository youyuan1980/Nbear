using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NBear.Data
{
    /// <summary>
    /// Interface of all statement factory
    /// </summary>
    public interface IStatementFactory
    {
        /// <summary>
        /// Creates the insert statement.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="includeColumns">The include columns.</param>
        /// <returns>The sql.</returns>
        string CreateInsertStatement(string tableName, params string[] includeColumns);

        /// <summary>
        /// Creates the update statement.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereStr">The where STR.</param>
        /// <param name="includeColumns">The include columns.</param>
        /// <returns>The sql.</returns>
        string CreateUpdateStatement(string tableName, string whereStr, params string[] includeColumns);

        /// <summary>
        /// Creates the delete statement.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereStr">The where STR.</param>
        /// <returns>The sql.</returns>
        string CreateDeleteStatement(string tableName, string whereStr);

        /// <summary>
        /// Creates the select statement.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="whereStr">The where STR.</param>
        /// <param name="orderByStr">The order by STR.</param>
        /// <param name="includeColumns">The include columns.</param>
        /// <returns>The sql.</returns>
        string CreateSelectStatement(string tableName, string whereStr, string orderByStr, params string[] includeColumns);
    }
}
