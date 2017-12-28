using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using NBear.Common;

namespace NBear.Data
{
    /// <summary>
    /// PageSplit
    /// </summary>
    public abstract class PageSplit : IPageSplit
    {
        #region Const Members

        /// <summary>
        /// The default page size.
        /// </summary>
        public const int DEFAULT_PAGESIZE = 10;

        #endregion

        #region Protected Members

        /// <summary>
        /// the splittable statement cache.
        /// </summary>
        protected static Dictionary<string, string> _SplitableStatementCache = new Dictionary<string, string>();
        /// <summary>
        /// PageSplitableSelectStatementForFirstPage
        /// </summary>
        internal string _PageSplitableSelectStatementForFirstPage;
        /// <summary>
        /// PageSplitableSelectStatement
        /// </summary>
        internal string _PageSplitableSelectStatement;
        /// <summary>
        /// SelectCountStatement
        /// </summary>
        internal string _SelectCountStatement;
        /// <summary>
        /// PageSize
        /// </summary>
        protected int _PageSize;
        /// <summary>
        /// RowCount
        /// </summary>
        protected int _RowCount;
        /// <summary>
        /// the db
        /// </summary>
        protected Database _DB;
        /// <summary>
        /// the param values
        /// </summary>
        internal object[] paramValues;
        /// <summary>
        /// the related db helper.
        /// </summary>
        protected DbHelper dbHelper;

        /// <summary>
        /// The where
        /// </summary>
        protected string _Where;
        /// <summary>
        /// The order by
        /// </summary>
        protected string _OrderBy;

        /// <summary>
        /// Prepares the command.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <returns>The prepared cmd.</returns>
        protected DbCommand PrepareCommand(int pageNo)
        {
            DbCommand cmd;
            if (pageNo == 1)
            {
                cmd = _DB.GetSqlStringCommand(string.Format(_PageSplitableSelectStatementForFirstPage, _PageSize));
            }
            else
            {
                cmd = _DB.GetSqlStringCommand(string.Format(_PageSplitableSelectStatement.Replace("{1} + {0}", (_PageSize * pageNo).ToString()), _PageSize, (pageNo - 1) * _PageSize));
            }

            string cmdText = cmd.CommandText;
            string[] paramNames = _DB.ParseParamNames(cmdText);

            lock (_DB.ParameterCache)
            {
                if (_DB.ParameterCache.IsCached(cmdText))
                {
                    _DB.ParameterCache.AddParametersFromCache(cmdText, cmd, null, this.paramValues);
                }
                else
                {
                    _DB.ParameterCache.CreateAndCacheParameters(cmdText, cmd, paramNames, null, paramValues);
                }
            }

            return cmd;
        }

        /// <summary>
        /// Constructs the page splitable select statement.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="keyColumn">The DEFAULT_KEY column.</param>
        /// <returns>The sql.</returns>
        protected abstract string ConstructPageSplitableSelectStatement(string sql, string keyColumn);

        /// <summary>
        /// Constructs the page splitable select statement for first page.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="keyColumn">The DEFAULT_KEY column.</param>
        /// <returns>The sql.</returns>
        protected abstract string ConstructPageSplitableSelectStatementForFirstPage(string sql, string keyColumn);

        /// <summary>
        /// Constructs the select count statement.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>The sql.</returns>
        protected abstract string ConstructSelectCountStatement(string sql);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PageSplit"/> class.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="selectStatement">The select statement.</param>
        /// <param name="keyColumn">The DEFAULT_KEY column.</param>
        protected PageSplit(Database db, string selectStatement, string keyColumn)
        {
            Check.Require(db != null, "db could be null.");
            Check.Require(selectStatement != null, "selectStatement could not be null.");
            selectStatement = selectStatement.Trim();
            Check.Require(selectStatement.Substring(0, 6).ToLower() == "select", "selectStatement must be a simple select statement.");
            Check.Require(!string.IsNullOrEmpty(keyColumn), "keyColumn could not be null or empty.");

            _DB = db;
            dbHelper = new DbHelper(_DB);
            _PageSplitableSelectStatementForFirstPage = ConstructPageSplitableSelectStatementForFirstPage(selectStatement, keyColumn);
            _PageSplitableSelectStatement = ConstructPageSplitableSelectStatement(selectStatement, keyColumn);
            _SelectCountStatement = ConstructSelectCountStatement(selectStatement);
            _PageSize = DEFAULT_PAGESIZE;
            _RowCount = -1;
        }

        #endregion

        #region IPageSplit Members

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize
        {
            get
            {
                return _PageSize;
            }
            set
            {
                Check.Require(value > 0, "PageSize should > 0.");

                _PageSize = value;
            }
        }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <returns>The row count</returns>
        public int GetRowCount()
        {
            if (_RowCount == -1)
            {
                _RowCount = Convert.ToInt32(dbHelper.SelectScalar(_SelectCountStatement, paramValues));
            }
            return _RowCount;
        }

        /// <summary>
        /// Gets the page count.
        /// </summary>
        /// <returns>The page count.</returns>
        public int GetPageCount()
        {
            return (int)Math.Ceiling(1.0 * GetRowCount() / _PageSize);
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <returns>DataSet</returns>
        public System.Data.DataSet GetPage(int pageNo)
        {
            Check.Require(pageNo > 0, "pageNo should > 0.");
            DbCommand cmd = PrepareCommand(pageNo);
            try
            {
                return _DB.ExecuteDataSet(cmd);
            }
            catch
            {
                throw;
            }
            finally
            {
                _DB.CloseConnection(cmd);
            }
        }

        /// <summary>
        /// Gets the page read only.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <returns>The result.</returns>
        public System.Data.IDataReader GetPageReadOnly(int pageNo)
        {
            if (pageNo <= 0)
            {
                return null;
            }

            DbCommand cmd = PrepareCommand(pageNo);
            IDataReader reader = _DB.ExecuteReader(cmd);
            return reader;
        }

        /// <summary>
        /// Gets or sets the db.
        /// </summary>
        /// <value>The db.</value>
        public Database Db
        {
            get
            {
                return _DB;
            }
            set
            {
                _DB = value;
            }
        }

        /// <summary>
        /// Gets the where.
        /// </summary>
        /// <value>The where.</value>
        public string Where { get { return _Where; } }

        /// <summary>
        /// Gets the order by.
        /// </summary>
        /// <value>The order by.</value>
        public string OrderBy { get { return _OrderBy; } }

        public object[] ParamValues { get { return paramValues; } }

        #endregion
    }
}
