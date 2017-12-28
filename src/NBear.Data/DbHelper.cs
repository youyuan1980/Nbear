using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NBear.Common;

namespace NBear.Data
{
    /// <summary>
    /// The DbHelper.
    /// </summary>
    public sealed class DbHelper
    {
        #region Private Members

        private DbCommand PrepareSqlStringCommand(string[] paramNames, DbType[] paramTypes, object[] paramValues, string cmdText)
        {
            DbCommand cmd = db.GetSqlStringCommand(cmdText);
            if (paramNames != null)
            {
                if (db.ParameterCache.IsCached(cmdText))
                {
                    db.ParameterCache.AddParametersFromCache(cmdText, cmd, paramTypes, paramValues);
                }
                else
                {
                    lock (db.ParameterCache)
                    {
                        if (db.ParameterCache.IsCached(cmdText))
                        {
                            db.ParameterCache.AddParametersFromCache(cmdText, cmd, paramTypes, paramValues);
                        }
                        else
                        {
                            db.ParameterCache.CreateAndCacheParameters(cmdText, cmd, paramNames, paramTypes, paramValues);
                        }
                    }
                }
            }
            return cmd;
        }

        private DbCommand PrepareStoredProcCommand(string procedureName, string[] paramNames, DbType[] paramTypes, object[] paramValues, string[] outParamNames, DbType[] outParamTypes)
        {
            DbCommand cmd = db.GetStoredProcCommand(procedureName);
            string cacheKey = procedureName;
            if (paramNames != null && paramNames.Length > 0)
            {
                cacheKey += "_" + string.Join("_", paramNames);
            }

            if (db.ParameterCache.IsCached(cacheKey))
            {
                db.ParameterCache.AddParametersFromCache(cacheKey, cmd, paramTypes, paramValues);
            }
            else
            {
                lock (db.ParameterCache)
                {
                    if (db.ParameterCache.IsCached(cacheKey))
                    {
                        db.ParameterCache.AddParametersFromCache(cacheKey, cmd, paramTypes, paramValues);
                    }
                    else
                    {
                        db.ParameterCache.CreateAndCacheParameters(cacheKey, cmd, paramNames, paramTypes, paramValues);
                    }
                }
            }

            //add out params
            if (outParamNames != null)
            {
                for (int i = 0; i < outParamNames.Length; i++)
                {
                    db.AddOutParameter(cmd, outParamNames[i], outParamTypes[i], 4000);
                }
            }

            return cmd;
        }

        private object[] GetOutParamResults(string[] outParamNames, DbCommand cmd)
        {
            object[] outParamResults;
            if (outParamNames != null && outParamNames.Length > 0)
            {
                outParamResults = new object[outParamNames.Length];
                int j = 0;
                foreach (string name in outParamNames)
                {
                    outParamResults[j++] = cmd.Parameters[db.DbProvider.BuildParameterName(name)].Value;
                }
            }
            else
            {
                outParamResults = null;
            }
            return outParamResults;
        }

        private Database db;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DbHelper"/> class.
        /// </summary>
        public DbHelper() : this(Database.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbHelper"/> class.
        /// </summary>
        /// <param name="db">The db.</param>
        public DbHelper(Database db)
        {
            Check.Require(db != null, "db could not be null.");

            this.db = db;
        }

        #endregion

        #region Public Members

        #region Insert

        /// <summary>
        /// Inserts the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="values">The values.</param>
        /// <param name="identyColumn">The identy column.</param>
        /// <returns>The auto incremental column value generate by databse. If there is no auto id column, the return value is 0.</returns>
        public int Insert(string table, string[] columns, object[] values, string identyColumn)
        {
            return Insert(table, columns, null, values, identyColumn);
        }

        /// <summary>
        /// Inserts the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="values">The values.</param>
        /// <param name="tran">The tran.</param>
        /// <param name="identyColumn">The identy column.</param>
        /// <returns>The auto incremental column value generate by databse. If there is no auto id column, the return value is 0.</returns>
        public int Insert(string table, string[] columns, object[] values, DbTransaction tran, string identyColumn)
        {
            return Insert(table, columns, null, values, tran, identyColumn);
        }

        /// <summary>
        /// Inserts the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="types">The types.</param>
        /// <param name="values">The values.</param>
        /// <param name="identyColumn">The identy column.</param>
        /// <returns>The auto incremental column value generate by databse. If there is no auto id column, the return value is 0.</returns>
        public int Insert(string table, string[] columns, DbType[] types, object[] values, string identyColumn)
        {
            return Insert(table, columns, types, values, null, identyColumn);
        }

        /// <summary>
        /// Inserts the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="types">The types.</param>
        /// <param name="values">The values.</param>
        /// <param name="tran">The tran.</param>
        /// <param name="identyColumn">The identy column.</param>
        /// <returns>The auto incremental column value generate by databse. If there is no auto id column, the return value is 0.</returns>
        public int Insert(string table, string[] columns, DbType[] types, object[] values, DbTransaction tran, string identyColumn)
        {
            Check.Require(db != null, "db could not be null.");
            Check.Require(table != null, "table could not be null.");
            Check.Require(columns != null, "columns could not be null.");
            Check.Require(values != null, "values could not be null.");
            Check.Require(columns.Length == values.Length, "columns.length should = values.length.");

            if (types != null && types.Length > 0)
            {
                Check.Require(types.Length == values.Length, "types.length should = values.length.");
            }

            string cmdText = db.GetStatementFactory().CreateInsertStatement(table, columns);
            DbCommand cmd = PrepareSqlStringCommand(columns, types, values, cmdText);

            if ((!db.IsBatchConnection) && identyColumn != null && db.DbProvider.SelectLastInsertedRowAutoIDStatement != null)
            {
                object retVal;

                if (!db.DbProvider.SelectLastInsertedRowAutoIDStatement.Contains("{1}"))
                {
                    cmd.CommandText = cmd.CommandText.Trim().TrimEnd(';') + "; " + db.DbProvider.SelectLastInsertedRowAutoIDStatement;
                    if (tran == null)
                    {
                        retVal = db.ExecuteScalar(cmd);
                    }
                    else
                    {
                        retVal = db.ExecuteScalar(cmd, tran);
                    }

                    if (retVal != DBNull.Value)
                    {
                        return Convert.ToInt32(retVal);
                    }
                }
                else
                {
                    if (db.DbProvider.SelectLastInsertedRowAutoIDStatement.StartsWith("SELECT SEQ_"))
                    {
                        retVal = db.ExecuteScalar(CommandType.Text, string.Format(db.DbProvider.SelectLastInsertedRowAutoIDStatement, null, table));
                        if (tran == null)
                        {
                            db.ExecuteNonQuery(cmd);
                        }
                        else
                        {
                            db.ExecuteNonQuery(cmd, tran);
                        }

                        if (retVal != DBNull.Value)
                        {
                            return Convert.ToInt32(retVal);
                        }
                    }
                    else
                    {
                        retVal = db.ExecuteScalar(CommandType.Text, string.Format(db.DbProvider.SelectLastInsertedRowAutoIDStatement, db.DbProvider.BuildColumnName(identyColumn), db.DbProvider.BuildColumnName(table)));
                        if (tran == null)
                        {
                            db.ExecuteNonQuery(cmd);
                        }
                        else
                        {
                            db.ExecuteNonQuery(cmd, tran);
                        }

                        if (retVal != DBNull.Value)
                        {
                            return Convert.ToInt32(retVal) + 1;
                        }
                   }
                }
            }
            else
            {
                if (tran == null)
                {
                    db.ExecuteNonQuery(cmd);
                }
                else
                {
                    db.ExecuteNonQuery(cmd, tran);
                }
            }

            return 0;
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="values">The values.</param>
        /// <param name="where">The where.</param>
        /// <param name="whereValues">The where values.</param>
        public void Update(string table, string[] columns, object[] values, string where, object[] whereValues)
        {
            Update(table, columns, null, values, where, null, whereValues);
        }

        /// <summary>
        /// Updates the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="values">The values.</param>
        /// <param name="where">The where.</param>
        /// <param name="whereValues">The where values.</param>
        /// <param name="tran">The tran.</param>
        public void Update(string table, string[] columns, object[] values, string where, object[] whereValues, DbTransaction tran)
        {
            Update(table, columns, null, values, where, null, whereValues, tran);
        }

        /// <summary>
        /// Updates the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="types">The types.</param>
        /// <param name="values">The values.</param>
        /// <param name="where">The where.</param>
        /// <param name="whereTypes">The where types.</param>
        /// <param name="whereValues">The where values.</param>
        public void Update(string table, string[] columns, DbType[] types, object[] values, string where, DbType[] whereTypes, object[] whereValues)
        {
            Update(table, columns, types, values, where, whereTypes, whereValues, null);
        }

        /// <summary>
        /// Updates the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="types">The types.</param>
        /// <param name="values">The values.</param>
        /// <param name="where">The where.</param>
        /// <param name="whereTypes">The where types.</param>
        /// <param name="whereValues">The where values.</param>
        /// <param name="tran">The tran.</param>
        public void Update(string table, string[] columns, DbType[] types, object[] values, string where, DbType[] whereTypes, object[] whereValues, DbTransaction tran)
        {
            Check.Require(db != null, "db could not be null.");
            Check.Require(table != null, "table could not be null.");
            Check.Require(columns != null, "columns could not be null.");
            Check.Require(values != null, "values could not be null.");
            Check.Require(columns.Length == values.Length, "columns.length should = values.length.");

            if (types != null && types.Length > 0)
            {
                Check.Require(types.Length == values.Length, "types.length should = values.length.");
            }

            if (whereTypes != null)
            {
                Check.Require(whereValues != null && whereValues.Length == whereTypes.Length, "whereValues.length should = whereValues.length.");
            }

            string cmdText = db.GetStatementFactory().CreateUpdateStatement(table, where, columns);

            List<string> paramNames = new List<string>();
            List<DbType> paramTypes = new List<DbType>();
            List<object> paramValues = new List<object>();
            for (int i = 0; i < columns.Length; i++)
            {
                if (values[i] != null && values[i] is PropertyItemParam)
                {
                    PropertyItemParam pip = (PropertyItemParam)values[i];
                    System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(db.DbProvider.ParamPrefix + columns[i] + @"(\s|,)");
                    cmdText = r.Replace(cmdText, pip.CustomValue + "$1");
                }
                else
                {
                    paramNames.Add(columns[i]);
                    if (types != null)
                    {
                        paramTypes.Add(types[i]);
                    }
                    paramValues.Add(values[i]);
                }
            }

            if (where != null)
            {
                string[] whereNames = db.ParseParamNames(where);

                if (whereNames != null && whereNames.Length > 0)
                {
                    for (int i = 0; i < whereNames.Length; i++)
                    {
                        if (whereValues[i] != null && whereValues[i] is PropertyItemParam)
                        {
                            PropertyItemParam pip = (PropertyItemParam)whereValues[i];
                            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(whereNames[i] + @"(\s|,)");
                            cmdText = r.Replace(cmdText, pip.CustomValue + "$1");
                        }
                        else
                        {
                            paramNames.Add(whereNames[i]);
                            if (whereTypes != null)
                            {
                                paramTypes.Add(whereTypes[i]);
                            }
                            paramValues.Add(whereValues[i]);
                        }
                    }
                }
            }

            while (paramTypes.Count > 0 && paramTypes.Count < paramNames.Count)
            {
                paramTypes.Add(DbType.String);
            }

            DbCommand cmd = PrepareSqlStringCommand(paramNames.ToArray(), paramTypes.Count == 0 ? null : paramTypes.ToArray(), paramValues.ToArray(), cmdText);

            if (tran == null)
            {
                db.ExecuteNonQuery(cmd);
            }
            else
            {
                db.ExecuteNonQuery(cmd, tran);
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="where">The where.</param>
        /// <param name="whereValues">The where values.</param>
        public void Delete(string table, string where, object[] whereValues)
        {
            Delete(table, where, null, whereValues, null);
        }

        /// <summary>
        /// Deletes the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="where">The where.</param>
        /// <param name="whereValues">The where values.</param>
        /// <param name="tran">The tran.</param>
        public void Delete(string table, string where, object[] whereValues, DbTransaction tran)
        {
            Delete(table, where, null, whereValues, tran);
        }

        /// <summary>
        /// Deletes the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="where">The where.</param>
        /// <param name="whereTypes">The where types.</param>
        /// <param name="whereValues">The where values.</param>
        public void Delete(string table, string where, DbType[] whereTypes, object[] whereValues)
        {
            Delete(table, where, whereTypes, whereValues, null);
        }

        /// <summary>
        /// Deletes the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="where">The where.</param>
        /// <param name="whereTypes">The where types.</param>
        /// <param name="whereValues">The where values.</param>
        /// <param name="tran">The tran.</param>
        public void Delete(string table, string where, DbType[] whereTypes, object[] whereValues, DbTransaction tran)
        {
            string cmdText = db.GetStatementFactory().CreateDeleteStatement(table, where);

            List<string> paramNames = new List<string>();
            List<DbType> paramTypes = new List<DbType>();
            List<object> paramValues = new List<object>();

            if (where != null)
            {
                string[] whereNames = db.ParseParamNames(where);

                if (whereNames != null && whereNames.Length > 0)
                {
                    for (int i = 0; i < whereNames.Length; i++)
                    {
                        if (whereValues[i] != null && whereValues[i] is PropertyItemParam)
                        {
                            PropertyItemParam pip = (PropertyItemParam)whereValues[i];
                            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(whereNames[i] + @"(\s|,)");
                            cmdText = r.Replace(cmdText, pip.CustomValue + "$1");
                        }
                        else
                        {
                            paramNames.Add(whereNames[i]);
                            if (whereTypes != null)
                            {
                                paramTypes.Add(whereTypes[i]);
                            }
                            paramValues.Add(whereValues[i]);
                        }
                    }
                }
            }

            while (paramTypes.Count > 0 && paramTypes.Count < paramNames.Count)
            {
                paramTypes.Add(DbType.String);
            }

            DbCommand cmd = PrepareSqlStringCommand(paramNames.ToArray(), paramTypes.Count == 0 ? null : paramTypes.ToArray(), paramValues.ToArray(), cmdText);
            if (tran == null)
            {
                db.ExecuteNonQuery(cmd);
            }
            else
            {
                db.ExecuteNonQuery(cmd, tran);
            }
        }

        #endregion

        #region Select

        /// <summary>
        /// Selects the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="where">The where.</param>
        /// <param name="whereValues">The where values.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>The select result.</returns>
        public DataSet Select(string table, string[] columns, string where, object[] whereValues, string orderBy)
        {
            string cmdText = db.GetStatementFactory().CreateSelectStatement(table, where, orderBy, columns);
            List<string> paramNames = new List<string>();
            List<object> paramValues = new List<object>();

            if (where != null)
            {
                string[] whereNames = db.ParseParamNames(where);

                if (whereNames != null && whereNames.Length > 0)
                {
                    for (int i = 0; i < whereNames.Length; i++)
                    {
                        if (whereValues[i] != null && whereValues[i] is PropertyItemParam)
                        {
                            PropertyItemParam pip = (PropertyItemParam)whereValues[i];
                            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(whereNames[i] + @"(\s|,)");
                            cmdText = r.Replace(cmdText, pip.CustomValue + "$1");
                        }
                        else
                        {
                            paramNames.Add(whereNames[i]);
                            paramValues.Add(whereValues[i]);
                        }
                    }
                }
            }

            DbCommand cmd = PrepareSqlStringCommand(paramNames.ToArray(), null, paramValues.ToArray(), cmdText);
            return db.ExecuteDataSet(cmd);
        }

        /// <summary>
        /// Selects the read only.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="where">The where.</param>
        /// <param name="whereValues">The where values.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>The select result.</returns>
        public IDataReader SelectReadOnly(string table, string[] columns, string where, object[] whereValues, string orderBy)
        {
            string cmdText = db.GetStatementFactory().CreateSelectStatement(table, where, orderBy, columns);
            List<string> paramNames = new List<string>();
            List<object> paramValues = new List<object>();

            if (where != null)
            {
                string[] whereNames = db.ParseParamNames(where);

                if (whereNames != null && whereNames.Length > 0)
                {
                    for (int i = 0; i < whereNames.Length; i++)
                    {
                        if (whereValues[i] != null && whereValues[i] is PropertyItemParam)
                        {
                            PropertyItemParam pip = (PropertyItemParam)whereValues[i];
                            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(whereNames[i] + @"(\s|,)");
                            cmdText = r.Replace(cmdText, pip.CustomValue + "$1");
                        }
                        else
                        {
                            paramNames.Add(whereNames[i]);
                            paramValues.Add(whereValues[i]);
                        }
                    }
                }
            }

            DbCommand cmd = PrepareSqlStringCommand(paramNames.ToArray(), null, paramValues.ToArray(), cmdText);
            return db.ExecuteReader(cmd);
        }

        /// <summary>
        /// Selects the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="where">The where.</param>
        /// <param name="whereTypes">The where types.</param>
        /// <param name="whereValues">The where values.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>The select result.</returns>
        public DataSet Select(string table, string[] columns, string where, DbType[] whereTypes, object[] whereValues, string orderBy)
        {
            string cmdText = db.GetStatementFactory().CreateSelectStatement(table, where, orderBy, columns);
            List<string> paramNames = new List<string>();
            List<DbType> paramTypes = new List<DbType>();
            List<object> paramValues = new List<object>();

            if (where != null)
            {
                string[] whereNames = db.ParseParamNames(where);

                if (whereNames != null && whereNames.Length > 0)
                {
                    for (int i = 0; i < whereNames.Length; i++)
                    {
                        if (whereValues[i] != null && whereValues[i] is PropertyItemParam)
                        {
                            PropertyItemParam pip = (PropertyItemParam)whereValues[i];
                            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(whereNames[i] + @"(\s|,)");
                            cmdText = r.Replace(cmdText, pip.CustomValue + "$1");
                        }
                        else
                        {
                            paramNames.Add(whereNames[i]);
                            if (whereTypes != null)
                            {
                                paramTypes.Add(whereTypes[i]);
                            }
                            paramValues.Add(whereValues[i]);
                        }
                    }
                }
            }

            while (paramTypes.Count > 0 && paramTypes.Count < paramNames.Count)
            {
                paramTypes.Add(DbType.String);
            }

            DbCommand cmd = PrepareSqlStringCommand(paramNames.ToArray(), paramTypes.Count == 0 ? null : paramTypes.ToArray(), paramValues.ToArray(), cmdText);
            return db.ExecuteDataSet(cmd);
        }

        /// <summary>
        /// Selects the read only.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="where">The where.</param>
        /// <param name="whereTypes">The where types.</param>
        /// <param name="whereValues">The where values.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>The select result.</returns>
        public IDataReader SelectReadOnly(string table, string[] columns, string where, DbType[] whereTypes, object[] whereValues, string orderBy)
        {
            string cmdText = db.GetStatementFactory().CreateSelectStatement(table, where, orderBy, columns);
            List<string> paramNames = new List<string>();
            List<DbType> paramTypes = new List<DbType>();
            List<object> paramValues = new List<object>();

            if (where != null)
            {
                string[] whereNames = db.ParseParamNames(where);

                if (whereNames != null && whereNames.Length > 0)
                {
                    for (int i = 0; i < whereNames.Length; i++)
                    {
                        if (whereValues[i] != null && whereValues[i] is PropertyItemParam)
                        {
                            PropertyItemParam pip = (PropertyItemParam)whereValues[i];
                            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(whereNames[i] + @"(\s|,)");
                            cmdText = r.Replace(cmdText, pip.CustomValue + "$1");
                        }
                        else
                        {
                            paramNames.Add(whereNames[i]);
                            if (whereTypes != null)
                            {
                                paramTypes.Add(whereTypes[i]);
                            }
                            paramValues.Add(whereValues[i]);
                        }
                    }
                }
            }

            while (paramTypes.Count > 0 && paramTypes.Count < paramNames.Count)
            {
                paramTypes.Add(DbType.String);
            }

            DbCommand cmd = PrepareSqlStringCommand(paramNames.ToArray(), paramTypes.Count == 0 ? null : paramTypes.ToArray(), paramValues.ToArray(), cmdText);
            return db.ExecuteReader(cmd);
        }

        /// <summary>
        /// Selects the page split.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="keyColumn">The DEFAULT_KEY column.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns>The select result.</returns>
        public IPageSplit SelectPageSplit(string table, string[] columns, string where, string orderBy, string keyColumn, object[] paramValues)
        {
            string cmdText = db.GetStatementFactory().CreateSelectStatement(table, where, orderBy, columns);

            List<object> _paramValues = new List<object>();

            if (where != null)
            {
                string[] whereNames = db.ParseParamNames(where);

                if (whereNames != null && whereNames.Length > 0)
                {
                    for (int i = 0; i < whereNames.Length; i++)
                    {
                        if (paramValues[i] != null && paramValues[i] is PropertyItemParam)
                        {
                            PropertyItemParam pip = (PropertyItemParam)paramValues[i];
                            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(whereNames[i] + @"(\s|,)");
                            cmdText = r.Replace(cmdText, pip.CustomValue + "$1");
                        }
                        else
                        {
                            _paramValues.Add(paramValues[i]);
                        }
                    }
                }
            }

            return db.GetPageSplit(cmdText, keyColumn, _paramValues.ToArray());
        }

        /// <summary>
        /// Selects the specified CMD text.
        /// </summary>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns>The select result.</returns>
        public DataSet Select(string cmdText, object[] paramValues)
        {
            List<string> paramNames = new List<string>();
            List<object> _paramValues = new List<object>();

            string[] whereNames = db.ParseParamNames(cmdText);

            if (whereNames != null && whereNames.Length > 0)
            {
                for (int i = 0; i < whereNames.Length; i++)
                {
                    if (paramValues[i] != null && paramValues[i] is PropertyItemParam)
                    {
                        PropertyItemParam pip = (PropertyItemParam)paramValues[i];
                        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(whereNames[i] + @"(\s|,)");
                        cmdText = r.Replace(cmdText, pip.CustomValue + "$1");
                    }
                    else
                    {
                        paramNames.Add(whereNames[i]);
                        _paramValues.Add(paramValues[i]);
                    }
                }
            }

            DbCommand cmd = PrepareSqlStringCommand(paramNames.ToArray(), null, _paramValues.ToArray(), cmdText);
            return db.ExecuteDataSet(cmd);
        }

        /// <summary>
        /// Selects the read only.
        /// </summary>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns>The select result.</returns>
        public IDataReader SelectReadOnly(string cmdText, object[] paramValues)
        {
            List<string> paramNames = new List<string>();
            List<object> _paramValues = new List<object>();

            string[] whereNames = db.ParseParamNames(cmdText);

            if (whereNames != null && whereNames.Length > 0)
            {
                for (int i = 0; i < whereNames.Length; i++)
                {
                    if (paramValues[i] != null && paramValues[i] is PropertyItemParam)
                    {
                        PropertyItemParam pip = (PropertyItemParam)paramValues[i];
                        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(whereNames[i] + @"(\s|,)");
                        cmdText = r.Replace(cmdText, pip.CustomValue + "$1");
                    }
                    else
                    {
                        paramNames.Add(whereNames[i]);
                        _paramValues.Add(paramValues[i]);
                    }
                }
            }

            DbCommand cmd = PrepareSqlStringCommand(paramNames.ToArray(), null, _paramValues.ToArray(), cmdText);
            return db.ExecuteReader(cmd);
        }

        /// <summary>
        /// Selects the scalar.
        /// </summary>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns>The select result.</returns>
        public object SelectScalar(string cmdText, object[] paramValues)
        {
            List<string> paramNames = new List<string>();
            List<object> _paramValues = new List<object>();

            string[] whereNames = db.ParseParamNames(cmdText);

            if (whereNames != null && whereNames.Length > 0)
            {
                for (int i = 0; i < whereNames.Length; i++)
                {
                    if (paramValues[i] != null && paramValues[i] is PropertyItemParam)
                    {
                        PropertyItemParam pip = (PropertyItemParam)paramValues[i];
                        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(whereNames[i] + @"(\s|,)");
                        cmdText = r.Replace(cmdText, pip.CustomValue + "$1");
                    }
                    else
                    {
                        paramNames.Add(whereNames[i]);
                        _paramValues.Add(paramValues[i]);
                    }
                }
            }

            DbCommand cmd = PrepareSqlStringCommand(paramNames.ToArray(), null, _paramValues.ToArray(), cmdText);
            return db.ExecuteScalar(cmd);
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="paramValues">The param values.</param>
        public void ExecuteNonQuery(string cmdText, object[] paramValues)
        {
            DbCommand cmd = PrepareSqlStringCommand(db.ParseParamNames(cmdText), null, paramValues, cmdText);
            db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="tran">The tran.</param>
        public void ExecuteNonQuery(string cmdText, object[] paramValues, DbTransaction tran)
        {
            DbCommand cmd = PrepareSqlStringCommand(db.ParseParamNames(cmdText), null, paramValues, cmdText);
            if (tran == null)
            {
                db.ExecuteNonQuery(cmd);
            }
            else
            {
                db.ExecuteNonQuery(cmd, tran);
            }
        }

        #endregion

        #region ExecuteStoredProcedure

        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="outParamNames">The out param names.</param>
        /// <param name="outParamTypes">The out param types.</param>
        /// <param name="outParamResults">The out param results.</param>
        /// <returns>The select result.</returns>
        public DataSet ExecuteStoredProcedure(string procedureName, string[] paramNames, object[] paramValues, string[] outParamNames, DbType[] outParamTypes, out object[] outParamResults)
        {
            DbCommand cmd = PrepareStoredProcCommand(procedureName, paramNames, null, paramValues, outParamNames, outParamTypes);
            DataSet ds = db.ExecuteDataSet(cmd);

            outParamResults = GetOutParamResults(outParamNames, cmd);

            return ds;
        }

        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns>The select result.</returns>
        public DataSet ExecuteStoredProcedure(string procedureName, string[] paramNames, object[] paramValues)
        {
            DbCommand cmd = PrepareStoredProcCommand(procedureName, paramNames, null, paramValues, null, null);
            return db.ExecuteDataSet(cmd);
        }

        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="outParamNames">The out param names.</param>
        /// <param name="outParamTypes">The out param types.</param>
        /// <param name="outParamResults">The out param results.</param>
        /// <param name="tran">The tran.</param>
        /// <returns>The select result.</returns>
        public DataSet ExecuteStoredProcedure(string procedureName, string[] paramNames, object[] paramValues, string[] outParamNames, DbType[] outParamTypes, out object[] outParamResults, DbTransaction tran)
        {
            DbCommand cmd = PrepareStoredProcCommand(procedureName, paramNames, null, paramValues, outParamNames, outParamTypes);
            DataSet ds;
            if (tran == null)
            {
                ds = db.ExecuteDataSet(cmd);
            }
            else
            {
                ds = db.ExecuteDataSet(cmd, tran);
            }

            outParamResults = GetOutParamResults(outParamNames, cmd);

            return ds;
        }

        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="tran">The tran.</param>
        /// <returns>The select result.</returns>
        public DataSet ExecuteStoredProcedure(string procedureName, string[] paramNames, object[] paramValues, DbTransaction tran)
        {
            DbCommand cmd = PrepareStoredProcCommand(procedureName, paramNames, null, paramValues, null, null);
            if (tran == null)
            {
                return db.ExecuteDataSet(cmd);
            }
            else
            {
                return db.ExecuteDataSet(cmd, tran);
            }
        }

        /// <summary>
        /// Executes the stored procedure read only.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="outParamNames">The out param names.</param>
        /// <param name="outParamTypes">The out param types.</param>
        /// <param name="outParamResults">The out param results.</param>
        /// <returns>The select result.</returns>
        public IDataReader ExecuteStoredProcedureReadOnly(string procedureName, string[] paramNames, object[] paramValues, string[] outParamNames, DbType[] outParamTypes, out object[] outParamResults)
        {
            DbCommand cmd = PrepareStoredProcCommand(procedureName, paramNames, null, paramValues, outParamNames, outParamTypes);
            IDataReader reader = db.ExecuteReader(cmd);

            outParamResults = GetOutParamResults(outParamNames, cmd);

            return reader;
        }

        /// <summary>
        /// Executes the stored procedure read only.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns>The select result.</returns>
        public IDataReader ExecuteStoredProcedureReadOnly(string procedureName, string[] paramNames, object[] paramValues)
        {
            DbCommand cmd = PrepareStoredProcCommand(procedureName, paramNames, null, paramValues, null, null);
            return db.ExecuteReader(cmd);
        }

        /// <summary>
        /// Executes the stored procedure read only.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="outParamNames">The out param names.</param>
        /// <param name="outParamTypes">The out param types.</param>
        /// <param name="outParamResults">The out param results.</param>
        /// <param name="tran">The tran.</param>
        /// <returns>The select result.</returns>
        public IDataReader ExecuteStoredProcedureReadOnly(string procedureName, string[] paramNames, object[] paramValues, string[] outParamNames, DbType[] outParamTypes, out object[] outParamResults, DbTransaction tran)
        {
            DbCommand cmd = PrepareStoredProcCommand(procedureName, paramNames, null, paramValues, outParamNames, outParamTypes);
            IDataReader reader;
            if (tran == null)
            {
                reader =  db.ExecuteReader(cmd);
            }
            else
            {
                reader = db.ExecuteReader(cmd, tran);
            }

            outParamResults = GetOutParamResults(outParamNames, cmd);

            return reader;
        }

        /// <summary>
        /// Executes the stored procedure read only.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="tran">The tran.</param>
        /// <returns>The select result.</returns>
        public IDataReader ExecuteStoredProcedureReadOnly(string procedureName, string[] paramNames, object[] paramValues, DbTransaction tran)
        {
            DbCommand cmd = PrepareStoredProcCommand(procedureName, paramNames, null, paramValues, null, null);
            if (tran == null)
            {
                return db.ExecuteReader(cmd);
            }
            else
            {
                return db.ExecuteReader(cmd, tran);
            }
        }

        /// <summary>
        /// Executes the stored procedure scalar.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="outParamNames">The out param names.</param>
        /// <param name="outParamTypes">The out param types.</param>
        /// <param name="outParamResults">The out param results.</param>
        /// <returns>The select result.</returns>
        public object ExecuteStoredProcedureScalar(string procedureName, string[] paramNames, object[] paramValues, string[] outParamNames, DbType[] outParamTypes, out object[] outParamResults)
        {
            DbCommand cmd = PrepareStoredProcCommand(procedureName, paramNames, null, paramValues, outParamNames, outParamTypes);
            object retObj = db.ExecuteScalar(cmd);

            outParamResults = GetOutParamResults(outParamNames, cmd);

            return retObj;
        }

        /// <summary>
        /// Executes the stored procedure scalar.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns>The select result.</returns>
        public object ExecuteStoredProcedureScalar(string procedureName, string[] paramNames, object[] paramValues)
        {
            DbCommand cmd = PrepareStoredProcCommand(procedureName, paramNames, null, paramValues, null, null);
            return db.ExecuteScalar(cmd);
        }

        /// <summary>
        /// Executes the stored procedure scalar.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="outParamNames">The out param names.</param>
        /// <param name="outParamTypes">The out param types.</param>
        /// <param name="outParamResults">The out param results.</param>
        /// <param name="tran">The tran.</param>
        /// <returns>The select result.</returns>
        public object ExecuteStoredProcedureScalar(string procedureName, string[] paramNames, object[] paramValues, string[] outParamNames, DbType[] outParamTypes, out object[] outParamResults, DbTransaction tran)
        {
            DbCommand cmd = PrepareStoredProcCommand(procedureName, paramNames, null, paramValues, outParamNames, outParamTypes);
            object retObj = db.ExecuteScalar(cmd);

            outParamResults = GetOutParamResults(outParamNames, cmd);

            return retObj;
        }

        /// <summary>
        /// Executes the stored procedure scalar.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="tran">The tran.</param>
        /// <returns>The select result.</returns>
        public object ExecuteStoredProcedureScalar(string procedureName, string[] paramNames, object[] paramValues, DbTransaction tran)
        {
            DbCommand cmd = PrepareStoredProcCommand(procedureName, paramNames, null, paramValues, null, null);
            if (tran == null)
            {
                return db.ExecuteScalar(cmd);
            }
            else
            {
                return db.ExecuteScalar(cmd, tran);
            }
        }

        /// <summary>
        /// Executes the stored procedure non query.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="outParamNames">The out param names.</param>
        /// <param name="outParamTypes">The out param types.</param>
        /// <param name="outParamResults">The out param results.</param>
        public void ExecuteStoredProcedureNonQuery(string procedureName, string[] paramNames, object[] paramValues, string[] outParamNames, DbType[] outParamTypes, out object[] outParamResults)
        {
            DbCommand cmd = PrepareStoredProcCommand(procedureName, paramNames, null, paramValues, outParamNames, outParamTypes);
            db.ExecuteNonQuery(cmd);

            outParamResults = GetOutParamResults(outParamNames, cmd);
        }

        /// <summary>
        /// Executes the stored procedure non query.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        public void ExecuteStoredProcedureNonQuery(string procedureName, string[] paramNames, object[] paramValues)
        {
            DbCommand cmd = PrepareStoredProcCommand(procedureName, paramNames, null, paramValues, null, null);
            db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Executes the stored procedure non query.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="outParamNames">The out param names.</param>
        /// <param name="outParamTypes">The out param types.</param>
        /// <param name="outParamResults">The out param results.</param>
        /// <param name="tran">The tran.</param>
        public void ExecuteStoredProcedureNonQuery(string procedureName, string[] paramNames, object[] paramValues, string[] outParamNames, DbType[] outParamTypes, out object[] outParamResults, DbTransaction tran)
        {
            DbCommand cmd = PrepareStoredProcCommand(procedureName, paramNames, null, paramValues, outParamNames, outParamTypes);
            if (tran == null)
            {
                db.ExecuteNonQuery(cmd);
            }
            else
            {
                db.ExecuteNonQuery(cmd, tran);
            }

            outParamResults = GetOutParamResults(outParamNames, cmd);
        }

        /// <summary>
        /// Executes the stored procedure non query.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="tran">The tran.</param>
        public void ExecuteStoredProcedureNonQuery(string procedureName, string[] paramNames, object[] paramValues, DbTransaction tran)
        {
            DbCommand cmd = PrepareStoredProcCommand(procedureName, paramNames, null, paramValues, null, null);
            if (tran == null)
            {
                db.ExecuteNonQuery(cmd);
            }
            else
            {
                db.ExecuteNonQuery(cmd, tran);
            }
        }

        #endregion

        #endregion

        #region Helper Methods

        /// <summary>
        /// Formats the param val.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        [Obsolete("This method is obsoleted. Please use NBear.Common.Util.FormatParamVal() instead.")]
        public static string FormatParamVal(object val)
        {
            return Util.FormatParamVal(val);
        }

        #endregion
    }
}
