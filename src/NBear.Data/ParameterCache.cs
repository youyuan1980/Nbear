using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using NBear.Common;

namespace NBear.Data
{
    /// <summary>
    /// The db param cache.
    /// </summary>
    internal class DbParameterCache
    {
        private Database db;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbParameterCache"/> class.
        /// </summary>
        /// <param name="db">The db.</param>
        public DbParameterCache(Database db)
        {
            this.db = db;
        }

        private Dictionary<string, DbParameter[]> cache = new Dictionary<string, DbParameter[]>();

        /// <summary>
        /// Determines whether the specified key is cached.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the specified key is cached; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCached(string key)
        {
            Check.Require(key != null, "key could not be null.");

            return cache.ContainsKey(key);
        }

        /// <summary>
        /// Adds the parameters from cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cmd">The CMD.</param>
        /// <param name="types">The types.</param>
        /// <param name="values">The values.</param>
        public void AddParametersFromCache(string key, DbCommand cmd, DbType[] types, object[] values)
        {
            DbParameterCollection parms = cmd.Parameters;
            parms.Clear();
            DbParameter[] cachedParams = cache[key];
            if (cachedParams != null && cachedParams.Length > 0)
            {
                if (types == null)
                {
                    for (int i = 0; i < cachedParams.Length; i++)
                    {
                        parms.Add(((ICloneable)cachedParams[i]).Clone());
                        parms[i].Value = values[i];
                        db.DbProvider.AdjustParameter(parms[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < cachedParams.Length; i++)
                    {
                        parms.Add(((ICloneable)cachedParams[i]).Clone());
                        parms[i].Value = values[i];
                        parms[i].DbType = types[i];
                        db.DbProvider.AdjustParameter(parms[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Creates the and cache parameters.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cmd">The CMD.</param>
        /// <param name="names">The names.</param>
        /// <param name="types">The types.</param>
        /// <param name="values">The values.</param>
        public void CreateAndCacheParameters(string key, DbCommand cmd, string[] names, DbType[] types, object[] values)
        {
            DbParameterCollection parms = cmd.Parameters;
            parms.Clear();
            if (names != null && names.Length > 0)
            {
                if (types == null)
                {
                    for (int i = 0; i < names.Length; i++)
                    {
                        db.AddInParameter(cmd, db.DbProvider.BuildParameterName(names[i]), values[i]);

                        AdjustParamNameForOracle(cmd, db.DbProvider.BuildParameterName(names[i]));
                    }
                }
                else
                {
                    for (int i = 0; i < names.Length; i++)
                    {
                        db.AddInParameter(cmd, db.DbProvider.BuildParameterName(names[i]), types[i], values[i]);

                        AdjustParamNameForOracle(cmd, db.DbProvider.BuildParameterName(names[i]));
                    }
                }
                cache.Add(key, CreateCachableParamsClone(parms));
            }
        }

        private DbParameter[] CreateCachableParamsClone(DbParameterCollection parms)
        {
            DbParameter[] cachableParams = new DbParameter[parms.Count];
            for (int i = 0; i < parms.Count; i++)
            {
                cachableParams[i] = (DbParameter)((ICloneable)parms[i]).Clone();
                cachableParams[i].Value = null;
            }
            return cachableParams;
        }

        private static void AdjustParamNameForOracle(DbCommand cmd, string paramName)
        {
            //For oracle, be careful to avoid paramNames having same ?XXX prefix as param name.
            if (paramName[0] == '?')
            {
                cmd.CommandText = cmd.CommandText.Replace(paramName, paramName.Replace("?", ":"));
            }

            if (paramName[0] == ':' && paramName.Length > 25)
            {
                string truncatedParamName = paramName.Substring(0, 15) + paramName.Substring(paramName.Length - 11, 10);
                cmd.Parameters[paramName].ParameterName = truncatedParamName;
                cmd.CommandText = cmd.CommandText.Replace(paramName, truncatedParamName);
            }
        }
    }
}