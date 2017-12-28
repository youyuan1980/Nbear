using System;
using System.Collections.Generic;
using System.Text;
using NBear.Data;
using NBear.Common;

namespace NBear.Data.SqlServer9
{
    /// <summary>
    /// PageSplit implementation for SQL Server 9.x (2005)
    /// </summary>
    public class SqlPageSplit9 : SqlServer.SqlPageSplit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SqlPageSplit9"/> class.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="selectStatement"></param>
        /// <param name="keyColumn"></param>
        /// <param name="paramValues"></param>
        public SqlPageSplit9(Database db, string selectStatement, string keyColumn, params object[] paramValues)
            : base(db, selectStatement, keyColumn, paramValues)
        {
        }

        /// <summary>
        /// construct an page splitable select statement from a simple select statement like 
        /// " select [columns] from [table_name] sql [condition] order by [order_list] "
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="keyColumn"></param>
        /// <returns></returns>
        protected override string ConstructPageSplitableSelectStatement(string sql, string keyColumn)
        {
            Check.Require(sql != null && keyColumn != null);

            string retStr;

            if (_SplitableStatementCache.ContainsKey(sql))
            {
                retStr = _SplitableStatementCache[sql];
            }
            else
            {
                lock (_SplitableStatementCache)
                {
                    if (_SplitableStatementCache.ContainsKey(sql))
                    {
                        retStr = _SplitableStatementCache[sql];
                    }
                    else
                    {
                        retStr = ConstructAndCacheSplitableSelectStatement(sql, keyColumn);
                    }
                }
            }

            return retStr;
        }

        private string ConstructAndCacheSplitableSelectStatement(string sql, string keyColumn)
        {
            string retStr;
            retStr = "Input sql statement is not legal!";
            if (sql.ToLower().Substring(0, 6) == "select" && sql.ToLower().Substring(0, 10) != "select top" && keyColumn != null)
            {
                string columnList = string.Empty;
                string fromTable = string.Empty;
                string whereClause = string.Empty;
                string orderByClause = string.Empty;

                //parse input select statement
                string str = sql.Substring(6).TrimStart(new char[] { ' ' });

                int fromBegin = str.ToLower().IndexOf(" from ");
                columnList = str.Substring(0, fromBegin);
                str = str.Substring(fromBegin).TrimStart(new char[] { ' ' });

                if (str.ToLower().IndexOf(" where ") > 0)
                {
                    int whereBegin = str.ToLower().IndexOf(" where ");
                    fromTable = str.Substring(0, whereBegin);
                    str = str.Substring(whereBegin).TrimStart(new char[] { ' ' });

                    if (str.ToLower().IndexOf(" order by ") > 0)
                    {
                        int orderBegin = str.ToLower().IndexOf(" order by ");
                        whereClause = str.Substring(0, orderBegin);
                        str = str.Substring(orderBegin).TrimStart(new char[] { ' ' });

                        orderByClause = str.TrimEnd(new char[] { ' ', ';' });

                        if (orderByClause.ToLower().IndexOf(String.Format("[{0}]", keyColumn.ToLower().TrimStart('[').TrimEnd(']'))) < 0)
                        {
                            orderByClause += String.Format(", [{0}]", keyColumn.TrimStart('[').TrimEnd(']'));
                        }
                    }
                    else
                    {
                        orderByClause = String.Format(" order by [{0}]", keyColumn.TrimStart('[').TrimEnd(']'));
                        whereClause = str.TrimEnd(new char[] { ' ', ';' });
                    }
                }
                else
                {
                    if (str.ToLower().IndexOf(" order by ") > 0)
                    {
                        int orderBegin = str.ToLower().IndexOf(" order by ");
                        fromTable = str.Substring(0, orderBegin);
                        str = str.Substring(orderBegin).TrimStart(new char[] { ' ' });

                        orderByClause = str.TrimEnd(new char[] { ' ', ';' });

                        if (orderByClause.ToLower().IndexOf(String.Format("[{0}]", keyColumn.ToLower().TrimStart('[').TrimEnd(']'))) < 0)
                        {
                            orderByClause += String.Format(", [{0}]", keyColumn.TrimStart('[').TrimEnd(']'));
                        }
                    }
                    else
                    {
                        orderByClause = String.Format(" order by [{0}]", keyColumn.TrimStart('[').TrimEnd(']'));
                        fromTable = str.TrimEnd(new char[] { ' ', ';' });
                    }
                }

                if (whereClause != string.Empty)
                {
                    _Where = whereClause.Substring(6);
                }
                if (orderByClause != string.Empty)
                {
                    _OrderBy = orderByClause.Substring(8);
                }

                retStr = string.Format("WITH tmp AS ( SELECT TOP #BeforeCount# + #PageSize# {0}, ROW_NUMBER() OVER ({3}) as DataRow_Pos {1} {2} ) SELECT {0} FROM tmp where tmp.DataRow_Pos > #BeforeCount# and tmp.DataRow_Pos <= #BeforeCount# + #PageSize#",
                    columnList, fromTable, whereClause, orderByClause).Replace("#PageSize#", "{0}").Replace("#BeforeCount#", "{1}");
            }

            _SplitableStatementCache.Add(sql, retStr);
            return retStr;
        }

        /// <summary>
        /// ConstructPageSplitableSelectStatementForFirstPage
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="keyColumn">keyColumn</param>
        /// <returns></returns>
        protected override string ConstructPageSplitableSelectStatementForFirstPage(string sql, string keyColumn)
        {
            string retSql = "SELECT TOP {0}" + sql.Substring(6);
            int orderByPos = retSql.ToLower().IndexOf("order by");
            if (orderByPos > 0)
            {
                string orderByClause = retSql.Substring(orderByPos).ToLower();
                if (orderByClause.IndexOf(String.Format("[{0}]", keyColumn.TrimStart('[').TrimEnd(']')).ToLower()) < 0)
                {
                    retSql += String.Format(", [{0}]", keyColumn.TrimStart('[').TrimEnd(']'));
                }
            }
            else
            {
                retSql += String.Format(" order by [{0}]", keyColumn.TrimStart('[').TrimEnd(']'));
            }
            return retSql;
        }
    }
}
