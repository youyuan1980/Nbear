using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NBear.Data
{
    /// <summary>
    /// Interface of all PageSplits.
    /// </summary>
    public interface IPageSplit
    {
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        int PageSize { get; set; }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <returns></returns>
        int GetRowCount();

        /// <summary>
        /// Gets the page count.
        /// </summary>
        /// <returns></returns>
        int GetPageCount();

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <returns>DataSet</returns>
        DataSet GetPage(int pageNo);

        /// <summary>
        /// Gets the page read only.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <returns>IDataReader</returns>
        IDataReader GetPageReadOnly(int pageNo);

        /// <summary>
        /// Gets or sets the db.
        /// </summary>
        /// <value>The db.</value>
        Database Db { get; set; }

        /// <summary>
        /// Gets the where.
        /// </summary>
        /// <value>The where.</value>
        string Where { get; }
        /// <summary>
        /// Gets the order by.
        /// </summary>
        /// <value>The order by.</value>
        string OrderBy { get; }
        /// <summary>
        /// Gets the param values.
        /// </summary>
        /// <value>The param values.</value>
        object[] ParamValues { get; }
    }
}
