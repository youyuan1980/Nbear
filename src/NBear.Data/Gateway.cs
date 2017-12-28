using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Transactions;
using System.Reflection;

using NBear.Common;
using NBear.Common.Caching;
using System.Collections;

namespace NBear.Data
{
    /// <summary>
    /// Type of a database.
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// Common SqlServer, including SQL Server 7.X, 8.X and 9.X
        /// </summary>
        SqlServer = 0,
        /// <summary>
        /// Access
        /// </summary>
        MsAccess = 1,
        /// <summary>
        /// Other
        /// </summary>
        Other = 2,
        /// <summary>
        /// SQL Server 9.X (2005) only
        /// </summary>
        SqlServer9 = 3,
        /// <summary>
        /// Oracle
        /// </summary>
        Oracle = 4,
        /// <summary>
        /// MySql
        /// </summary>
        MySql = 5
    }

    /// <summary>
    /// The data access gateway.
    /// </summary>
    public sealed class Gateway
    {
        #region Default Gateway

        /// <summary>
        /// Get the default gateway, which mapping to the default Database.
        /// </summary>
        public static Gateway Default;

        /// <summary>
        /// Sets the default database.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="connStr">The conn STR.</param>
        public static void SetDefaultDatabase(DatabaseType dt, string connStr)
        {
            if (dt == DatabaseType.Other)
            {
                throw new NotSupportedException("Please use \"SetDefaultDatabase(string assemblyName, string className, string connStr)\" for databases other than SqlServer, MsAccess, MySql or Oracle Database!");
            }

            DbProvider provider = CreateDbProvider(dt, connStr);

            Default = new Gateway(new Database(provider));
        }

        /// <summary>
        /// Creates the db provider.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="connStr">The conn STR.</param>
        /// <returns>The db provider.</returns>
        private static DbProvider CreateDbProvider(DatabaseType dt, string connStr)
        {
            DbProvider provider = null;
            if (dt == DatabaseType.SqlServer9)
            {
                provider = DbProviderFactory.CreateDbProvider(null, typeof(NBear.Data.SqlServer9.SqlDbProvider9).FullName, connStr);
            }
            else if (dt == DatabaseType.SqlServer)
            {
                provider = DbProviderFactory.CreateDbProvider(null, typeof(NBear.Data.SqlServer.SqlDbProvider).FullName, connStr);
            }
            else if (dt == DatabaseType.Oracle)
            {
                provider = DbProviderFactory.CreateDbProvider(null, typeof(NBear.Data.Oracle.OracleDbProvider).FullName, connStr);
            }
            else if (dt == DatabaseType.MySql)
            {
                provider = DbProviderFactory.CreateDbProvider(null, typeof(NBear.Data.MySql.MySqlDbProvider).FullName, connStr);
            }
            else  //Ms Access
            {
                provider = DbProviderFactory.CreateDbProvider(null, typeof(NBear.Data.MsAccess.AccessDbProvider).FullName, connStr);
            }
            return provider;
        }

        /// <summary>
        /// Sets the default database.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="connStr">The conn STR.</param>
        public static void SetDefaultDatabase(string assemblyName, string className, string connStr)
        {
            DbProvider provider = DbProviderFactory.CreateDbProvider(assemblyName, className, connStr);
            if (provider == null)
            {
                throw new NotSupportedException(string.Format("Cannot construct DbProvider by specified parameters: {0}, {1}, {2}",
                    assemblyName, className, connStr));
            }

            Default = new Gateway(new Database(provider));
        }

        /// <summary>
        /// Sets the default database.
        /// </summary>
        /// <param name="connStrName">Name of the conn STR.</param>
        public static void SetDefaultDatabase(string connStrName)
        {
            DbProvider provider = DbProviderFactory.CreateDbProvider(connStrName);
            if (provider == null)
            {
                throw new NotSupportedException(string.Format("Cannot construct DbProvider by specified ConnectionStringName: {0}", connStrName));
            }

            Default = new Gateway(new Database(provider));
        }

        #endregion

        #region Private Members

        private Database db;

        private DbHelper dbHelper;

        private void InitGateway(Database db)
        {
            this.db = db;
            this.dbHelper = new DbHelper(db);

            object cacheConfig = System.Configuration.ConfigurationManager.GetSection("cacheConfig");
            if (cacheConfig != null)
            {
                cacheConfigSection = (CacheConfigurationSection)cacheConfig;
                tableExpireSecondsMap =  new Dictionary<string, int>();

                foreach (string key in cacheConfigSection.CachingTables.AllKeys)
                {
                    if (key.Contains("."))
                    {
                        string[] splittedKey = key.Split('.');
                        System.Configuration.ConnectionStringSettings connStr = System.Configuration.ConfigurationManager.ConnectionStrings[splittedKey[0].Trim()];
                        if (connStr != null)
                        {
                            int expireSeconds = CacheConfigurationSection.DEFAULT_EXPIRE_SECONDS;
                            try
                            {
                                expireSeconds = int.Parse(cacheConfigSection.CachingTables[key].Value);
                            }
                            catch
                            {
                            }

                            string tableName = splittedKey[1].ToLower().Trim();

                            if (!tableExpireSecondsMap.ContainsKey(tableName))
                            {
                                tableExpireSecondsMap.Add(tableName, expireSeconds);
                            }
                            else
                            {
                                tableExpireSecondsMap[tableName] = expireSeconds;
                            }
                        }
                    }
                }
            }
        }

        private static WhereClip BuildEqualWhereClip(object[] values, string[] names)
        {
            WhereClip where = null;

            for (int i = 0; i < names.Length; i++)
            {
                if (where == null)
                {
                    where = (new PropertyItem(names[i]) == values[i]);
                }
                else
                {
                    where = where & (new PropertyItem(names[i]) == values[i]);
                }
            }
            return where;
        }

        private object GetAggregateValue<EntityType>(WhereClip where, string column, bool readCache)
            where EntityType : Entity, new()
        {
            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            string cacheKey = null;
            if (readCache && IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0)
            {
                cacheKey = ComputeCacheKey(typeof(EntityType).ToString() + "|GetAggregateValue" + column, where);
                object cachedObj = GetCache(cacheKey);
                if (cachedObj != null)
                {
                    return cachedObj;
                }
            }

            object obj = dbHelper.SelectScalar(string.Format("select {2} from {0} {1}", BuildDbColumnName(ec.ViewName), where == null || where.ToString() == null ? string.Empty : "where " + ParseExpressionByMetaData(ec, where.ToString()), column), where.ParamValues);

            if (readCache && IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0 && cacheKey != null)
            {
                AddCache(cacheKey, obj, GetTableCacheExpireSeconds(ec.ViewName));
            }

            return obj;
        }

        private static MethodInfo GetGatewayMethodInfo(string signiture)
        {
            MethodInfo mi = null;
            foreach (MethodBase mb in typeof(Gateway).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (mb.ToString() == signiture)
                {
                    mi = (MethodInfo)mb;
                    break;
                }
            }
            return mi;
        }

        private string ParseExpressionByMetaData(EntityConfiguration ec, string sql)
        {
            if (sql == null)
            {
                return null;
            }
            sql = PropertyItem.ParseExpressionByMetaData(sql, new PropertyToColumnMapHandler(ec.GetMappingColumnName), db.DbProvider.LeftToken, db.DbProvider.RightToken, db.DbProvider.ParamPrefix);
            return sql;
        }

        private void DoCascadeUpdate<EntityType>(EntityType obj, DbTransaction tran, EntityConfiguration ec, WhereClip where, Dictionary<string, object> modifiedProperties) where EntityType : Entity, new()
        {
            string[] columnNames = null;
            object[] columnValues = null;
            List<string> byteArrayColumns = null;
            List<string> nullableNumberColumns = null;

            if (modifiedProperties.Count > 0)
            {
                columnNames = ec.GetMappingColumnNames(new List<string>(modifiedProperties.Keys).ToArray());
                columnValues = new List<object>(modifiedProperties.Values).ToArray();
                byteArrayColumns = MetaDataManager.GetByteArrayColumns(ec.Name);
                nullableNumberColumns = MetaDataManager.GetNullableNumberColumns(ec.Name);
                if (byteArrayColumns.Count == 0)
                {
                    dbHelper.Update(ec.MappingName, columnNames, columnValues, ParseExpressionByMetaData(ec, where.ToString()), where.ParamValues, tran);
                }
                else
                {
                    DbType[] types = BuildDbTypes(columnNames, columnValues, byteArrayColumns, nullableNumberColumns);
                    dbHelper.Update(ec.MappingName, columnNames, types, columnValues, ParseExpressionByMetaData(ec, where.ToString()), null, where.ParamValues, tran);
                }
                RemovedUpdatedModifiedProperties<EntityType>(obj, modifiedProperties);
            }

            //update base entities
            if (ec.BaseEntity != null)
            {
                Type baseType = Util.GetType(ec.BaseEntity);
                while (baseType != null)
                {
                    EntityConfiguration baseEc = MetaDataManager.GetEntityConfiguration(baseType.ToString());

                    modifiedProperties = obj.GetModifiedProperties(baseType);
                    if (modifiedProperties.Count > 0)
                    {
                        columnNames = baseEc.GetMappingColumnNames(new List<string>(modifiedProperties.Keys).ToArray());
                        columnValues = new List<object>(modifiedProperties.Values).ToArray();
                        byteArrayColumns = MetaDataManager.GetByteArrayColumns(baseEc.Name);
                        nullableNumberColumns = MetaDataManager.GetNullableNumberColumns(baseEc.Name);
                        if (byteArrayColumns.Count == 0)
                        {
                            dbHelper.Update(baseEc.MappingName, columnNames, columnValues, ParseExpressionByMetaData(baseEc, where.ToString()), where.ParamValues, tran);
                        }
                        else
                        {
                            DbType[] types = BuildDbTypes(columnNames, columnValues, byteArrayColumns, nullableNumberColumns);
                            dbHelper.Update(baseEc.MappingName, columnNames, types, columnValues, ParseExpressionByMetaData(baseEc, where.ToString()), null, where.ParamValues, tran);
                        }
                        RemovedUpdatedModifiedProperties<EntityType>(obj, modifiedProperties);
                    }
                    if (baseEc.BaseEntity == null)
                    {
                        baseType = null;
                    }
                    else
                    {
                        baseType = Util.GetType(baseEc.BaseEntity);
                    }
                }
            }
        }

        private void DoCascadeDelete<EntityType>(EntityType obj, DbTransaction tran, EntityConfiguration ec, WhereClip where) where EntityType : Entity, new()
        {
            //CascadePreLoadContainedProperties(obj);

            foreach (PropertyConfiguration pc in ec.Properties)
            {
                if (pc.IsQueryProperty && (pc.IsContained || pc.QueryType == "ManyToManyQuery"))
                {
                    Type propertyType = Util.GetType(pc.PropertyType);
                    if (typeof(IEntityArrayList).IsAssignableFrom(propertyType))
                    {
                        propertyType = ((IEntityArrayList)Activator.CreateInstance(propertyType)).GetArrayItemType();
                    }

                    EntityConfiguration propertyTypeEc = MetaDataManager.GetEntityConfiguration(propertyType.ToString());
                    PropertyInfo[] pis = Util.DeepGetProperties(typeof(EntityType));

                    object propertyValue = null;
                    foreach (PropertyInfo pi in pis)
                    {
                        if (pi.Name == pc.Name)
                        {
                            propertyValue = pi.GetValue(obj, null);
                        }
                    }

                    MethodInfo deleteMethod = null;

                    if (propertyValue != null)
                    {
                        if (pc.IsContained && pc.QueryType == "FkQuery" && MetaDataManager.IsNonRelatedEntity(pc.RelatedType))
                        {
                            deleteMethod = GetGatewayMethodInfo("Void Delete[EntityType](NBear.Common.WhereClip, System.Data.Common.DbTransaction)");
                            deleteMethod.MakeGenericMethod(propertyType).Invoke(this, new object[] { new PropertyItem(pc.RelatedForeignKey) == Entity.GetPrimaryKeyValues(obj)[0], tran });
                        }
                        else
                        {
                            if (pc.QueryType == "ManyToManyQuery")
                            {
                                Type relationType = Util.GetType(pc.RelationType);
                                EntityConfiguration relationEc = MetaDataManager.GetEntityConfiguration(pc.RelationType);

                                deleteMethod = GetGatewayMethodInfo("Void Delete[EntityType](NBear.Common.WhereClip, System.Data.Common.DbTransaction)");
                                string relationKey = null;
                                foreach (PropertyConfiguration relationKeyPc in relationEc.Properties)
                                {
                                    if (relationKeyPc.RelatedType != null)
                                    {
                                        Type pcRelatedType = Util.GetType(relationKeyPc.RelatedType);
                                        if (pcRelatedType.IsAssignableFrom(obj.GetType()))
                                        {
                                            relationKey = relationKeyPc.Name;
                                            break;
                                        }
                                    }
                                }

                                deleteMethod.MakeGenericMethod(relationType).Invoke(this, new object[] { new PropertyItem(relationKey) == Entity.GetPrimaryKeyValues(obj)[0], tran });
                            }

                            if (pc.IsContained)
                            {
                                deleteMethod = GetGatewayMethodInfo("Void Delete[EntityType](EntityType, System.Data.Common.DbTransaction)");

                                if (propertyValue is IEntityArrayList)
                                {
                                    foreach (object item in (IEnumerable)propertyValue)
                                    {
                                        deleteMethod.MakeGenericMethod(propertyType).Invoke(this, new object[] { item, tran });
                                    }
                                }
                                else
                                {
                                    deleteMethod.MakeGenericMethod(propertyType).Invoke(this, new object[] { propertyValue, tran });
                                }
                            }
                        }
                    }
                }
            }

            dbHelper.Delete(ec.MappingName, ParseExpressionByMetaData(ec, where.ToString()), where.ParamValues, tran);

            if (ec.BaseEntity != null)
            {
                Type baseType = Util.GetType(ec.BaseEntity);
                while (baseType != null)
                {
                    EntityConfiguration baseEc = MetaDataManager.GetEntityConfiguration(baseType.ToString());

                    dbHelper.Delete(baseEc.MappingName, ParseExpressionByMetaData(baseEc, where.ToString()), where.ParamValues, tran);

                    if (baseEc.BaseEntity == null)
                    {
                        baseType = null;
                    }
                    else
                    {
                        baseType = Util.GetType(baseEc.BaseEntity);
                    }
                }
            }
        }

        //private void CascadePreLoadContainedProperties(Entity obj)
        //{
        //    EntityConfiguration ec = obj.GetEntityConfiguration();
        //    object tempObj;
        //    foreach (PropertyInfo pi in Util.DeepGetProperties(obj.GetType()))
        //    {
        //        PropertyConfiguration pc = ec.GetPropertyConfiguration(pi.Name);
        //        if (pc.IsQueryProperty && (pc.IsContained || pc.QueryType == "ManyToManyQuery") && (!obj.IsQueryPropertyLoaded(pc.Name)))
        //        {
        //            tempObj = pi.GetValue(obj, null);
        //            if (tempObj != null && pc.IsContained)
        //            {
        //                if (typeof(IEntityArrayList).IsAssignableFrom(tempObj.GetType()))
        //                {
        //                    foreach (Entity item in (IEnumerable)tempObj)
        //                    {
        //                        CascadePreLoadContainedProperties(item);
        //                    }
        //                }
        //                else
        //                {
        //                    CascadePreLoadContainedProperties((Entity)tempObj);
        //                }
        //            }
        //        }
        //    }
        //}

        private bool DeleteAsChildEntity(string typeName, object[] pkValues, DbTransaction tran)
        {
            bool deletedAsChildEntity = false;

            List<EntityConfiguration> childEntities = MetaDataManager.GetChildEntityConfigurations(typeName);
            foreach (EntityConfiguration ec in childEntities)
            {
                Type childType = Util.GetType(ec.Name);
                MethodInfo findMethod = GetGatewayMethodInfo("EntityType Find[EntityType](System.Object[])");
                Gateway findGateway = this;
                if (db.IsBatchConnection)
                {
                    findGateway = new Gateway(new Database(db.DbProvider));
                    findGateway.Db.OnLog += new LogHandler(db.WriteLog);
                }
                object childObj = findMethod.MakeGenericMethod(childType).Invoke(findGateway, new object[] { pkValues });
                if (childObj != null)
                {
                    if (MetaDataManager.GetChildEntityConfigurations(ec.Name).Count == 0)
                    {
                        //do deletion
                        MethodInfo deleteMethod = GetGatewayMethodInfo("Void Delete[EntityType](EntityType, System.Data.Common.DbTransaction)");
                        deleteMethod.MakeGenericMethod(childType).Invoke(this, new object[] { childObj, tran });

                        deletedAsChildEntity = true;
                    }
                    else
                    {
                        deletedAsChildEntity = DeleteAsChildEntity(ec.Name, pkValues, tran);
                    }
                    if (deletedAsChildEntity)
                    {
                        return true;
                    }
                }
            }

            return deletedAsChildEntity;
        }

        private int DoCascadeInsert<EntityType>(EntityType obj, DbTransaction tran, EntityConfiguration ec, string keyColumn) where EntityType : Entity, new()
        {
            int retAutoID = 0;

            string[] columnNames = null;
            object[] columnValues = null;
            List<string> byteArrayColumns = null;
            List<string> nullableNumberColumns = null;
            List<string> sqlDefaultValueColumns = null;

            if (ec.BaseEntity != null)
            {
                Stack<EntityConfiguration> stackEc = new Stack<EntityConfiguration>();
                Stack<Type> stackBaseType = new Stack<Type>();

                Type baseType = Util.GetType(ec.BaseEntity);
                while (baseType != null)
                {
                    EntityConfiguration baseEc = MetaDataManager.GetEntityConfiguration(baseType.ToString());

                    stackEc.Push(baseEc);
                    stackBaseType.Push(baseType);

                    if (baseEc.BaseEntity == null)
                    {
                        baseType = null;
                    }
                    else
                    {
                        baseType = Util.GetType(baseEc.BaseEntity);
                    }
                }

                while (stackEc.Count > 0)
                {
                    EntityConfiguration ecToInsert = stackEc.Pop();
                    Type baseTypeToInsert = stackBaseType.Pop();

                    columnNames = Entity.GetCreatePropertyMappingColumnNames(baseTypeToInsert);
                    columnValues = Entity.GetCreatePropertyValues(baseTypeToInsert, obj);
                    byteArrayColumns = MetaDataManager.GetByteArrayColumns(ecToInsert.Name);
                    nullableNumberColumns = MetaDataManager.GetNullableNumberColumns(ecToInsert.Name);

                    sqlDefaultValueColumns = MetaDataManager.GetSqlTypeWithDefaultValueColumns(ecToInsert.Name);
                    if (sqlDefaultValueColumns.Count > 0)
                    {
                        FilterNullSqlDefaultValueColumns(sqlDefaultValueColumns, columnNames, columnValues, out columnNames, out columnValues);
                    }

                    if (ecToInsert.BaseEntity == null)
                    {
                        if (byteArrayColumns.Count == 0 && nullableNumberColumns.Count == 0)
                        {
                            retAutoID = dbHelper.Insert(ecToInsert.MappingName, columnNames, columnValues, tran, keyColumn);
                        }
                        else
                        {
                            DbType[] types = BuildDbTypes(columnNames, columnValues, byteArrayColumns, nullableNumberColumns);
                            retAutoID = dbHelper.Insert(ecToInsert.MappingName, columnNames, types, columnValues, tran, keyColumn);
                        }

                        if (retAutoID > 0)
                        {
                            //save the retAutoID value to entity's ID property
                            string autoIdProperty = MetaDataManager.GetEntityAutoId(ec.Name);
                            if (autoIdProperty != null)
                            {
                                Util.DeepGetProperty(obj.GetType(), autoIdProperty).SetValue(obj, retAutoID, null);
                            }
                        }
                    }
                    else
                    {
                        if (byteArrayColumns.Count == 0 && nullableNumberColumns.Count == 0)
                        {
                            dbHelper.Insert(ecToInsert.MappingName, columnNames, columnValues, tran, keyColumn);
                        }
                        else
                        {
                            DbType[] types = BuildDbTypes(columnNames, columnValues, byteArrayColumns, nullableNumberColumns);
                            dbHelper.Insert(ecToInsert.MappingName, columnNames, types, columnValues, tran, keyColumn);
                        }
                    }
                }
            }

            columnNames = Entity.GetCreatePropertyMappingColumnNames(obj.GetEntityConfiguration());
            columnValues = Entity.GetCreatePropertyValues(obj.GetType(), obj);
            byteArrayColumns = MetaDataManager.GetByteArrayColumns(ec.Name);
            nullableNumberColumns = MetaDataManager.GetNullableNumberColumns(ec.Name);

            sqlDefaultValueColumns = MetaDataManager.GetSqlTypeWithDefaultValueColumns(ec.Name);
            if (sqlDefaultValueColumns.Count > 0)
            {
                FilterNullSqlDefaultValueColumns(sqlDefaultValueColumns, columnNames, columnValues, out columnNames, out columnValues);
            }

            if (retAutoID == 0)
            {
                if (byteArrayColumns.Count == 0 && nullableNumberColumns.Count == 0)
                {
                    retAutoID = dbHelper.Insert(ec.MappingName, columnNames, columnValues, tran, keyColumn);
                }
                else
                {
                    DbType[] types = BuildDbTypes(columnNames, columnValues, byteArrayColumns, nullableNumberColumns);
                    retAutoID = dbHelper.Insert(ec.MappingName, columnNames, types, columnValues, tran, keyColumn);
                }
            }
            else
            {
                if (byteArrayColumns.Count == 0 && nullableNumberColumns.Count == 0)
                {
                    dbHelper.Insert(ec.MappingName, columnNames, columnValues, tran, keyColumn);
                }
                else
                {
                    DbType[] types = BuildDbTypes(columnNames, columnValues, byteArrayColumns, nullableNumberColumns);
                    dbHelper.Insert(ec.MappingName, columnNames, types, columnValues, tran, keyColumn);
                }
            }

            if (ec.BaseEntity == null && retAutoID > 0)
            {
                //save the retAutoID value to entity's ID property
                string autoIdProperty = MetaDataManager.GetEntityAutoId(ec.Name);
                if (autoIdProperty != null)
                {
                    Util.DeepGetProperty(obj.GetType(), autoIdProperty).SetValue(obj, retAutoID, null);
                }
            }

            return retAutoID;
        }

        private void DoCascadePropertySave<EntityType>(object obj, DbTransaction tran, EntityConfiguration ec) where EntityType : Entity, new()
        {
            foreach (PropertyInfo pi in Util.DeepGetProperties(obj.GetType()))
            {
                PropertyConfiguration pc = ec.GetPropertyConfiguration(pi.Name);
                if (pc != null && pc.IsQueryProperty && (pc.IsContained || pc.QueryType == "ManyToManyQuery") && (pc.QueryType == "ManyToManyQuery" || pc.QueryType == "FkQuery" || pc.QueryType == "PkQuery"))
                {
                    if (((Entity)obj).IsQueryPropertyLoaded(pi.Name))
                    {
                        object propertyValue = pi.GetValue(obj, null);
                        Dictionary<string, List<object>> toSaveRelatedObjectsAll = ((Entity)obj).GetToSaveRelatedPropertyObjects();
                        List<object> toSaveRelatedObjs = toSaveRelatedObjectsAll.ContainsKey(pi.Name) ? toSaveRelatedObjectsAll[pi.Name] : null;
                        if (propertyValue != null)
                        {
                            object ownerPkValue = null;
                            if (pc.RelationType == null)
                            {
                                ownerPkValue = Entity.GetPrimaryKeyValues(obj)[0];
                            }

                            if (typeof(IEntityArrayList).IsAssignableFrom(pi.PropertyType))
                            {
                                foreach (object item in (IEnumerable)propertyValue)
                                {
                                    if (pc.RelationType == null)
                                    {
                                        SetRelatedObjFriendKeyValue(pc, obj, ownerPkValue, item);
                                    }
                                    DoPropertySave(obj, toSaveRelatedObjs, item, pc, pc.RelationType, tran);
                                }
                            }
                            else
                            {
                                if (pc.RelationType == null)
                                {
                                    SetRelatedObjFriendKeyValue(pc, obj, ownerPkValue, propertyValue);
                                }
                                DoPropertySave(obj, toSaveRelatedObjs, propertyValue, pc, pc.RelationType, tran);
                            }
                        }
                    }
                }
            }

            ((Entity)obj).ClearToSaveRelatedPropertyObjects();
        }

        private void SetRelatedObjFriendKeyValue(PropertyConfiguration pc, object obj, object objPkValue, object relatedObj)
        {
            if (pc.RelationType == null)
            {
                //set relatedobj's related key = ownerObj's id
                foreach (PropertyConfiguration item in ((Entity)relatedObj).GetEntityConfiguration().Properties)
                {
                    if (item.Name == pc.RelatedForeignKey)
                    {
                        if (item.IsQueryProperty)
                        {
                            if (item.QueryType == "FkReverseQuery")
                            {
                                Util.DeepGetField(relatedObj.GetType(), "_" + item.Name + "_" + item.RelatedForeignKey, false).SetValue(relatedObj, objPkValue);
                            }
                            else
                            {
                                Util.DeepGetProperty(relatedObj.GetType(), item.Name).SetValue(relatedObj, obj, null);
                            }
                        }
                        else
                        {
                            Util.DeepGetProperty(relatedObj.GetType(), item.Name).SetValue(relatedObj, objPkValue, null);
                        }
                        break;
                    }
                }
            }
        }

        private void DoPropertySave(object ownerObj, List<object> toSaveRelatedObjs, object relatedObj, PropertyConfiguration propertyConfig, string relationTypeName, DbTransaction tran)
        {
            if (relatedObj != null)
            {
                Type ownerType = ownerObj.GetType();
                Type relatedType = relatedObj.GetType();
                MethodInfo entitySaveMethod = GetGatewayMethodInfo("Int32 Save[EntityType](EntityType, System.Data.Common.DbTransaction)");

                if (propertyConfig.IsContained)
                {
                    entitySaveMethod.MakeGenericMethod(relatedType).Invoke(this, new object[] { relatedObj, tran });
                }

                if (relationTypeName != null && propertyConfig.QueryType == "ManyToManyQuery" && toSaveRelatedObjs != null && toSaveRelatedObjs.Contains(relatedObj))
                {
                    if (relationTypeName != relatedType.ToString())
                    {
                        //create relation relatedType instance
                        Type relationType = Util.GetType(relationTypeName);
                        EntityConfiguration relationEc = MetaDataManager.GetEntityConfiguration(relationTypeName);
                        object relationTypeInstance = Activator.CreateInstance(relationType);
                        foreach (PropertyConfiguration pc in relationEc.Properties)
                        {
                            if (pc.RelatedType != null)
                            {
                                Type pcRelatedType = Util.GetType(pc.RelatedType);
                                if (pcRelatedType.IsAssignableFrom(ownerType))
                                {
                                    relationType.GetProperty(pc.Name).SetValue(relationTypeInstance, Entity.GetPropertyValues(ownerObj, MetaDataManager.GetEntityConfiguration(pc.RelatedType).GetMappingColumnName(pc.RelatedForeignKey))[0], null);
                                }
                                else if (pcRelatedType.IsAssignableFrom(relatedType))
                                {
                                    relationType.GetProperty(pc.Name).SetValue(relationTypeInstance, Entity.GetPropertyValues(relatedObj, MetaDataManager.GetEntityConfiguration(pc.RelatedType).GetMappingColumnName(pc.RelatedForeignKey))[0], null);
                                }
                            }
                        }

                        if (((Entity)relatedObj).IsAttached())
                        {
                            //try
                            //{
                                entitySaveMethod.MakeGenericMethod(relationType).Invoke(this, new object[] { relationTypeInstance, tran });
                            //}
                            //catch
                            //{
                                //just ignore insert error
                            //}
                        }
                    }
                }
            }
        }

        private void DoDeleteToDeleteObjects(Entity obj, DbTransaction tran)
        {
            if (obj.GetToDeleteRelatedPropertyObjects() != null)
            {
                foreach (string propertyName in obj.GetToDeleteRelatedPropertyObjects().Keys)
                {
                    //create relation relatedType instance
                    EntityConfiguration ec = obj.GetEntityConfiguration();
                    PropertyConfiguration propertyConfig = ec.GetPropertyConfiguration(propertyName);
                    string relationTypeName = propertyConfig.RelationType;
                    Type relationType = null;
                    EntityConfiguration relationEc = null;
                    object relationTypeInstance = null;
                    if (relationTypeName != null)
                    {
                        relationType = Util.GetType(relationTypeName);
                        relationEc = MetaDataManager.GetEntityConfiguration(relationTypeName);
                        relationTypeInstance = Activator.CreateInstance(relationType);
                        foreach (PropertyConfiguration pc in relationEc.Properties)
                        {
                            if (pc.RelatedType != null)
                            {
                                Type pcRelatedType = Util.GetType(pc.RelatedType);
                                if (pcRelatedType.IsAssignableFrom(obj.GetType()))
                                {
                                    relationType.GetProperty(pc.Name).SetValue(relationTypeInstance, Entity.GetPropertyValues(obj, MetaDataManager.GetEntityConfiguration(pc.RelatedType).GetMappingColumnName(pc.RelatedForeignKey))[0], null);
                                }
                            }
                        }
                    }

                    foreach (object item in obj.GetToDeleteRelatedPropertyObjects()[propertyName])
                    {
                        if (item != null && item is Entity)
                        {
                            MethodInfo deleteMethod = GetGatewayMethodInfo("Void Delete[EntityType](EntityType, System.Data.Common.DbTransaction)");

                            if (relationTypeName != null)
                            {
                                //delete relationTypeInstance
                                foreach (PropertyConfiguration pc in relationEc.Properties)
                                {
                                    if (pc.RelatedType != null)
                                    {
                                        Type pcRelatedType = Util.GetType(pc.RelatedType);
                                        if (pcRelatedType.IsAssignableFrom(item.GetType()))
                                        {
                                            relationType.GetProperty(pc.Name).SetValue(relationTypeInstance, Entity.GetPropertyValues(item, MetaDataManager.GetEntityConfiguration(pc.RelatedType).GetMappingColumnName(pc.RelatedForeignKey))[0], null);
                                        }
                                    }
                                }
                                deleteMethod.MakeGenericMethod(relationType).Invoke(this, new object[] { relationTypeInstance, tran });
                            }

                            if (propertyConfig.IsContained)
                            {
                                //delete property obj
                                deleteMethod.MakeGenericMethod(item.GetType()).Invoke(this, new object[] { item, tran });
                            }
                        }
                    }
                }

                obj.ClearToDeleteRelatedPropertyObjects();
            }
        }

        private static void RemovedUpdatedModifiedProperties<EntityType>(EntityType obj, Dictionary<string, object> modifiedProperties) where EntityType : Entity, new()
        {
            Dictionary<string, object> otherModifiedProperties = new Dictionary<string,object>();
            Dictionary<string, object> oldModifiedPropeties = obj.GetModifiedProperties();
            foreach (string key in oldModifiedPropeties.Keys)
            {
                otherModifiedProperties.Add(key, oldModifiedPropeties[key]);
            }
            lock (otherModifiedProperties)
            {
                foreach (string propertyName in modifiedProperties.Keys)
                {
                    if (otherModifiedProperties.ContainsKey(propertyName))
                    {
                        otherModifiedProperties.Remove(propertyName);
                    }
                }
            }
            obj.SetModifiedProperties(otherModifiedProperties);
        }

        private static void SetModifiedProperties<EntityType>(Dictionary<string, object> changedProperties, EntityType obj) where EntityType : Entity, new()
        {
            Dictionary<string, object> cloneProperties = new Dictionary<string, object>();
            foreach (string key in changedProperties.Keys)
            {
                cloneProperties.Add(key, changedProperties[key]);
            }
            obj.SetModifiedProperties(cloneProperties);
        }

        private DbType[] BuildDbTypes(string[] columnNames, object[] columnValues, List<string> byteArrayColumns, List<string> nullableNumberColumns)
        {
            if (columnNames == null)
            {
                return null;
            }

            DbType[] types = new DbType[columnNames.Length];

            for (int i = 0; i < columnNames.Length; i++)
            {
                if (columnValues[i] == null && byteArrayColumns.Contains(columnNames[i]))
                {
                    types[i] = DbType.Binary;
                }
                else if (columnValues[i] == null && nullableNumberColumns.Contains(columnNames[i]))
                {
                    types[i] = DbType.Int32;
                }
                else
                {
                    types[i] = DbType.String;
                }
            }

            return types;
        }

        private void ParseValuesForPropertyItemParams(object[] values, EntityConfiguration ec)
        {
            if (values == null)
            {
                return;
            }

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] != null)
                {
                    if (values[i] is PropertyItem)
                    {
                        PropertyItem pi = (PropertyItem)values[i];
                        PropertyItemParam pip = new PropertyItemParam(db.DbProvider.BuildColumnName(ec.GetMappingColumnName(pi.PropertyName)));
                        values[i] = pip;
                    }
                    else if (values[i] is PropertyItemParam)
                    {
                        PropertyItemParam pip = (PropertyItemParam)values[i];
                        pip.CustomValue = ParseExpressionByMetaData(ec, pip.CustomValue);
                    }
                }
            }
        }

        private string FilterNTextPrefix(string sql)
        {
            if (sql == null)
            {
                return sql;
            }

            return sql.Replace(" N'", " '");
        }

        private void FilterNullSqlDefaultValueColumns(List<string> sqlDefaultValueColumns, string[] createColumnNames, object[] createColumnValues, out string[] outNames, out object[] outValues)
        {
            List<string> names = new List<string>();
            List<object> values = new List<object>();
            for (int i = 0; i < createColumnNames.Length; i++)
            {
                if (!(sqlDefaultValueColumns.Contains(createColumnNames[i]) && createColumnValues[i] == null))
                {
                    names.Add(createColumnNames[i]);
                    values.Add(createColumnValues[i]);
                }
            }
            outNames = names.ToArray();
            outValues = values.ToArray();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="Gateway"/> class.
        /// </summary>
        static Gateway()
        {
            if (Database.Default != null)
            {
                Default = new Gateway(Database.Default);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gateway"/> class.
        /// </summary>
        /// <param name="connStrName">Name of the conn STR.</param>
        public Gateway(string connStrName)
        {
            InitGateway(new Database(DbProviderFactory.CreateDbProvider(connStrName)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gateway"/> class.
        /// </summary>
        /// <param name="db">The db.</param>
        public Gateway(Database db)
        {
            InitGateway(db);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gateway"/> class.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="connStr">The conn STR.</param>
        public Gateway(DatabaseType dt, string connStr)
        {
            if (dt == DatabaseType.Other)
            {
                throw new NotSupportedException("Please use \"new Gateway(string assemblyName, string className, string connStr)\" for databases other than SqlServer, MsAccess, MySql or Oracle Database!");
            }

            DbProvider provider = CreateDbProvider(dt, connStr);

            InitGateway(new Database(provider));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gateway"/> class.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="connStr">The conn STR.</param>
        public Gateway(string assemblyName, string className, string connStr)
        {
            DbProvider provider = DbProviderFactory.CreateDbProvider(assemblyName, className, connStr);
            if (provider == null)
            {
                throw new NotSupportedException(string.Format("Cannot construct DbProvider by specified parameters: {0}, {1}, {2}",
                    assemblyName, className, connStr));
            }

            InitGateway(new Database(provider));
        }

        #endregion

        #region Database

        /// <summary>
        /// Registers the SQL logger.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public void RegisterSqlLogger(LogHandler handler)
        {
            db.OnLog += handler;
        }

        /// <summary>
        /// Unregisters the SQL logger.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public void UnregisterSqlLogger(LogHandler handler)
        {
            db.OnLog -= handler;
        }

        /// <summary>
        /// Gets the db.
        /// </summary>
        /// <value>The db.</value>
        public Database Db
        {
            get
            {
                return this.db;
            }
        }

        /// <summary>
        /// Gets the db helper.
        /// </summary>
        /// <value>The db helper.</value>
        public DbHelper DbHelper
        {
            get
            {
                return dbHelper;
            }
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns>The begined transaction.</returns>
        public DbTransaction BeginTransaction()
        {
            return db.BeginTransaction();
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="il">The il.</param>
        /// <returns>The begined transaction.</returns>
        public DbTransaction BeginTransaction(System.Data.IsolationLevel il)
        {
            return db.BeginTransaction(il);
        }

        /// <summary>
        /// Closes the transaction.
        /// </summary>
        /// <param name="tran">The tran.</param>
        public void CloseTransaction(DbTransaction tran)
        {
            if (tran.Connection != null && tran.Connection.State != ConnectionState.Closed)
            {
                db.CloseConnection(tran);
            }
        }

        /// <summary>
        /// Builds the name of the db param.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The name of the db param</returns>
        public string BuildDbParamName(string name)
        {
            Check.Require(name != null, "Arguments error.", new ArgumentNullException("name"));

            return db.DbProvider.BuildParameterName(name);
        }

        /// <summary>
        /// Builds the name of the db column.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The name of the db column</returns>
        public string BuildDbColumnName(string name)
        {
            Check.Require(name != null, "Arguments error.", new ArgumentNullException("name"));

            return db.DbProvider.BuildColumnName(name);
        }

        #endregion

        #region Create Entity

        /// <summary>
        /// The query handler.
        /// </summary>
        /// <param name="returnEntityType">Type of the return entity.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="baseEntity">The base entity.</param>
        /// <returns>The query result.</returns>
        internal object OnQueryHandler(Type returnEntityType, string propertyName, string where, string orderBy, Entity baseEntity)
        {
            EntityConfiguration returnEntityEc = MetaDataManager.GetEntityConfiguration(returnEntityType.ToString());
            EntityConfiguration baseEntityEc = MetaDataManager.GetEntityConfiguration(baseEntity.GetType().ToString());
            string viewName = returnEntityEc.ViewName;

            //replace {Property Name}s -> {Mapping Name}s
            where = ReplacePropertyNamesWithColumnNames(where, returnEntityEc);
            orderBy = ReplacePropertyNamesWithColumnNames(orderBy, returnEntityEc);

            //where = ParseExpressionByMetaData(returnEntityEc, where);
            //orderBy = ParseExpressionByMetaData(returnEntityEc, orderBy);

            WhereClip combinedWhere = null;

            if (where != null)
            {
                string[] paramNames = db.ParseParamNames(where);
                if (paramNames != null)
                {
                    for (int i = 0; i < paramNames.Length; i++)
                    {
                        paramNames[i] = baseEntityEc.GetMappingColumnName(paramNames[i].TrimStart(db.DbProvider.ParamPrefix.ToCharArray()));
                    }
                }
                object[] paramValues = (paramNames != null ? Entity.GetPropertyValues(baseEntity, paramNames) : null);

                combinedWhere = new WhereClip(where, paramValues);
            }

            PropertyConfiguration pc = baseEntityEc.GetPropertyConfiguration(propertyName);

            if (pc.RelationType != null)
            {
                EntityConfiguration relationEc = MetaDataManager.GetEntityConfiguration(pc.RelationType);
                viewName = string.Format("v_{0}_{1}", returnEntityEc.ViewName, relationEc.ViewName);
                List<string> foreignKeys = new List<string>();
                List<string> relatedColumnNames = new List<string>();
                foreach (PropertyConfiguration item in relationEc.Properties)
                {
                    if (item.IsRelationKey && Util.GetType(item.RelatedType).IsAssignableFrom(baseEntity.GetType()))
                    {
                        foreignKeys.Add(MetaDataManager.GetEntityConfiguration(item.RelatedType).GetMappingColumnName(item.RelatedForeignKey));
                        relatedColumnNames.Add(item.MappingName);
                    }
                }
                object[] foreignKeyValues = Entity.GetPropertyValues(baseEntity, foreignKeys.ToArray());
                for (int i = 0; i < foreignKeys.Count; i++)
                {
                    if (combinedWhere == null)
                    {
                        combinedWhere = new PropertyItem(relationEc.MappingName + "_" + relatedColumnNames[i]) == foreignKeyValues[i];
                    }
                    else
                    {
                        combinedWhere = combinedWhere & (new PropertyItem(relationEc.MappingName + "_" + relatedColumnNames[i]) == foreignKeyValues[i]);
                    }
                }
            }

            Gateway findGateway = this;
            if (db.IsBatchConnection)
            {
                findGateway = new Gateway(new Database(db.DbProvider));
                findGateway.Db.OnLog += new LogHandler(db.WriteLog);
            }

            MethodInfo findArrayMethod = GetGatewayMethodInfo("EntityType[] FindArray[EntityType](NBear.Common.EntityConfiguration, System.String, NBear.Common.WhereClip, NBear.Common.OrderByClip)");
            return findArrayMethod.MakeGenericMethod(returnEntityType).Invoke(findGateway, new object[] { returnEntityEc, viewName, combinedWhere, new OrderByClip(orderBy) });
        }

        private string RemoveTypePrefix(string typeName)
        {
            string name = typeName;
            while (name.Contains("."))
            {
                name = name.Substring(name.IndexOf(".")).TrimStart('.');
            }
            return name;
        }

        private static string ReplacePropertyNamesWithColumnNames(string sql, EntityConfiguration ec)
        {
            if (sql == null)
            {
                return null;
            }

            while (sql.Contains("{"))
            {
                int begin = sql.IndexOf("{");
                int end = sql.IndexOf("}");
                string name = sql.Substring(begin, end - begin).Trim('{', '}');
                sql = sql.Replace("{" + name + "}", "[" + ec.GetMappingColumnName(name) + "]");
            }
            sql = sql.Replace("[", "{").Replace("]", "}");
            return sql;
        }

        /// <summary>
        /// Creates and initiate an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        internal EntityType CreateEntity<EntityType>()
            where EntityType : Entity, new()
        {
            EntityType obj = new EntityType();
            obj.SetQueryProxy(new Entity.QueryProxyHandler(OnQueryHandler));
            obj.Attach();
            return obj;
        }

        #endregion

        #region Batch Gateway

        /// <summary>
        /// Begins the batch gateway.
        /// </summary>
        /// <param name="batchSize">Size of the batch.</param>
        /// <returns>The begined batch gateway.</returns>
        public Gateway BeginBatchGateway(int batchSize)
        {
            Gateway gateway = new Gateway(new Database(db.DbProvider));
            gateway.db.OnLog += new LogHandler(this.db.WriteLog);
            gateway.BeginBatch(batchSize);
            return gateway;
        }

        /// <summary>
        /// Begins the batch gateway.
        /// </summary>
        /// <param name="batchSize">Size of the batch.</param>
        /// <param name="tran">The tran.</param>
        /// <returns>The begined batch gateway.</returns>
        public Gateway BeginBatchGateway(int batchSize, DbTransaction tran)
        {
            Gateway gateway = new Gateway(new Database(db.DbProvider));
            gateway.db.OnLog += new LogHandler(this.db.WriteLog);
            gateway.BeginBatch(batchSize, tran);
            return gateway;
        }

        /// <summary>
        /// Begins the gateway as a batch gateway.
        /// </summary>
        /// <param name="batchSize">Size of the batch.</param>
        public void BeginBatch(int batchSize)
        {
            db.BeginBatchConnection(batchSize >= 1 ? batchSize : 1);
        }

        /// <summary>
        /// Begins the gateway as a batch gateway.
        /// </summary>
        /// <param name="batchSize">Size of the batch.</param>
        /// <param name="tran">The tran.</param>
        public void BeginBatch(int batchSize, DbTransaction tran)
        {
            db.BeginBatchConnection(batchSize >= 1 ? batchSize : 1, tran);
        }

        /// <summary>
        /// Ends the batch.
        /// </summary>
        public void EndBatch()
        {
            db.EndBatchConnection();
        }

        /// <summary>
        /// Executes the pending batch operations.
        /// </summary>
        public void ExecutePendingBatchOperations()
        {
            db.ExecutePendingBatchOperations();
        }

        #endregion

        #region Non-StrongTyped Gateways

        /// <summary>
        /// Selects the data set.
        /// </summary>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns>The result.</returns>
        public DataSet SelectDataSet(string cmdText, object[] paramValues)
        {
            return dbHelper.Select(cmdText, paramValues);
        }

        /// <summary>
        /// Selects the data reader.
        /// </summary>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns>The result.</returns>
        public IDataReader SelectDataReader(string cmdText, object[] paramValues)
        {
            return dbHelper.SelectReadOnly(cmdText, paramValues);
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="paramValues">The param values.</param>
        public void ExecuteNonQuery(string cmdText, object[] paramValues)
        {
            dbHelper.ExecuteNonQuery(cmdText, paramValues);
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="tran">The tran.</param>
        /// <param name="paramValues">The param values.</param>
        public void ExecuteNonQuery(string cmdText, DbTransaction tran, object[] paramValues)
        {
            dbHelper.ExecuteNonQuery(cmdText, paramValues, tran);
        }

        /// <summary>
        /// Selects the scalar.
        /// </summary>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns>The result.</returns>
        public ReturnColumnType SelectScalar<ReturnColumnType>(string cmdText, object[] paramValues)
        {
            object retVal = dbHelper.SelectScalar(cmdText, paramValues);
            if (retVal == DBNull.Value)
            {
                retVal = default(ReturnColumnType);
            }
            return (ReturnColumnType)retVal;
        }

        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns>The result.</returns>
        public DataSet ExecuteStoredProcedure(string procedureName, string[] paramNames, object[] paramValues)
        {
            return dbHelper.ExecuteStoredProcedure(procedureName, paramNames, paramValues);
        }

        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="tran">The tran.</param>
        /// <returns>The result.</returns>
        public DataSet ExecuteStoredProcedure(string procedureName, string[] paramNames, object[] paramValues, DbTransaction tran)
        {
            return dbHelper.ExecuteStoredProcedure(procedureName, paramNames, paramValues, tran);
        }

        /// <summary>
        /// Executes the stored procedure read only.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns>The result.</returns>
        public IDataReader ExecuteStoredProcedureReadOnly(string procedureName, string[] paramNames, object[] paramValues)
        {
            return dbHelper.ExecuteStoredProcedureReadOnly(procedureName, paramNames, paramValues);
        }

        /// <summary>
        /// Executes the stored procedure read only.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="tran">The tran.</param>
        /// <returns>The result.</returns>
        public IDataReader ExecuteStoredProcedureReadOnly(string procedureName, string[] paramNames, object[] paramValues, DbTransaction tran)
        {
            return dbHelper.ExecuteStoredProcedureReadOnly(procedureName, paramNames, paramValues, tran);
        }

        /// <summary>
        /// Executes the stored procedure scalar.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns>The result.</returns>
        public object ExecuteStoredProcedureScalar(string procedureName, string[] paramNames, object[] paramValues)
        {
            return dbHelper.ExecuteStoredProcedureScalar(procedureName, paramNames, paramValues);
        }

        /// <summary>
        /// Executes the stored procedure scalar.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="tran">The tran.</param>
        /// <returns>The result.</returns>
        public object ExecuteStoredProcedureScalar(string procedureName, string[] paramNames, object[] paramValues, DbTransaction tran)
        {
            return dbHelper.ExecuteStoredProcedureScalar(procedureName, paramNames, paramValues, tran);
        }

        /// <summary>
        /// Executes the stored procedure non query.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        public void ExecuteStoredProcedureNonQuery(string procedureName, string[] paramNames, object[] paramValues)
        {
            dbHelper.ExecuteStoredProcedureNonQuery(procedureName, paramNames, paramValues);
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
            dbHelper.ExecuteStoredProcedureNonQuery(procedureName, paramNames, paramValues, tran);
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
        /// <returns>The result.</returns>
        public DataSet ExecuteStoredProcedure(string procedureName, string[] paramNames, object[] paramValues, string[] outParamNames, DbType[] outParamTypes, out object[] outParamResults)
        {
            return dbHelper.ExecuteStoredProcedure(procedureName, paramNames, paramValues, outParamNames, outParamTypes, out outParamResults);
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
        /// <returns>The result.</returns>
        public DataSet ExecuteStoredProcedure(string procedureName, string[] paramNames, object[] paramValues, string[] outParamNames, DbType[] outParamTypes, out object[] outParamResults, DbTransaction tran)
        {
            return dbHelper.ExecuteStoredProcedure(procedureName, paramNames, paramValues, outParamNames, outParamTypes, out outParamResults, tran);
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
        /// <returns>The result.</returns>
        public IDataReader ExecuteStoredProcedureReadOnly(string procedureName, string[] paramNames, object[] paramValues, string[] outParamNames, DbType[] outParamTypes, out object[] outParamResults)
        {
            return dbHelper.ExecuteStoredProcedureReadOnly(procedureName, paramNames, paramValues, outParamNames, outParamTypes, out outParamResults);
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
        /// <returns>The result.</returns>
        public IDataReader ExecuteStoredProcedureReadOnly(string procedureName, string[] paramNames, object[] paramValues, string[] outParamNames, DbType[] outParamTypes, out object[] outParamResults, DbTransaction tran)
        {
            return dbHelper.ExecuteStoredProcedureReadOnly(procedureName, paramNames, paramValues, outParamNames, outParamTypes, out outParamResults, tran);
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
        /// <returns>The result.</returns>
        public object ExecuteStoredProcedureScalar(string procedureName, string[] paramNames, object[] paramValues, string[] outParamNames, DbType[] outParamTypes, out object[] outParamResults)
        {
            return dbHelper.ExecuteStoredProcedureScalar(procedureName, paramNames, paramValues, outParamNames, outParamTypes, out outParamResults);
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
        /// <returns>The result.</returns>
        public object ExecuteStoredProcedureScalar(string procedureName, string[] paramNames, object[] paramValues, string[] outParamNames, DbType[] outParamTypes, out object[] outParamResults, DbTransaction tran)
        {
            return dbHelper.ExecuteStoredProcedureScalar(procedureName, paramNames, paramValues, outParamNames, outParamTypes, out outParamResults, tran);
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
            dbHelper.ExecuteStoredProcedureNonQuery(procedureName, paramNames, paramValues, outParamNames, outParamTypes, out outParamResults);
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
            dbHelper.ExecuteStoredProcedureNonQuery(procedureName, paramNames, paramValues, outParamNames, outParamTypes, out outParamResults, tran);
        }

        #endregion

        #region StrongTyped Gateways

        /// <summary>
        /// Finds the specified pk values.
        /// </summary>
        /// <param name="pkValues">The pk values.</param>
        /// <returns>The result.</returns>
        public EntityType Find<EntityType>(params object[] pkValues)
            where EntityType : Entity, new()
        {
            Check.Require(pkValues != null && pkValues.Length > 0, "pkValues could not be null or empty.");
            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            string[] pks = Entity.GetPrimaryKeyMappingColumnNames(ec);

            WhereClip where = BuildEqualWhereClip(pkValues, pks);

            if (IsCacheTurnedOn && ec.IsAutoPreLoad)
            {
                return FindFromPreLoad<EntityType>(ec, where);
            }

            string cacheKey = null;

            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0)
            {
                cacheKey = ComputeCacheKey(typeof(EntityType).ToString() + "|Find", where);
                if (cache.Contains(cacheKey))
                {
                    object cachedObj = GetCache(cacheKey);
                    if (cachedObj != null)
                    {
                        return (EntityType)cachedObj;
                    }
                }
            }

            IDataReader reader = dbHelper.SelectReadOnly(ec.ViewName, Entity.GetPropertyMappingColumnNames(ec), ParseExpressionByMetaData(ec, where.ToString()), where.ParamValues, null);
            EntityType obj = null;
            if (reader.Read())
            {
                obj = CreateEntity<EntityType>();
                obj.SetPropertyValues(reader);
            }
            reader.Close();

            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0 && cacheKey != null)
            {
                AddCache(cacheKey, obj, GetTableCacheExpireSeconds(ec.ViewName));
            }

            return obj;
        }

        /// <summary>
        /// Finds the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns>The result.</returns>
        public EntityType Find<EntityType>(WhereClip where)
            where EntityType : Entity, new()
        {
            Check.Require(where != null, "where could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            ParseValuesForPropertyItemParams(where.ParamValues, ec);

            if (IsCacheTurnedOn && ec.IsAutoPreLoad)
            {
                return FindFromPreLoad<EntityType>(ec, where);
            }

            string cacheKey = null;
            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0)
            {
                cacheKey = ComputeCacheKey(typeof(EntityType).ToString() + "|Find", where);
                object cachedObj = GetCache(cacheKey);
                if (cachedObj != null)
                {
                    return (EntityType)cachedObj;
                }
            }

            IDataReader reader = dbHelper.SelectReadOnly(ec.ViewName, Entity.GetPropertyMappingColumnNames(ec), ParseExpressionByMetaData(ec, where.ToString()), where.ParamValues, null);
            EntityType obj = null;
            if (reader.Read())
            {
                obj = CreateEntity<EntityType>();
                obj.SetPropertyValues(reader);
            }
            reader.Close();

            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0 && cacheKey != null)
            {
                AddCache(cacheKey, obj, GetTableCacheExpireSeconds(ec.ViewName));
            }

            return obj;
        }

        /// <summary>
        /// Whether entity exists.
        /// </summary>
        /// <param name="pkValues">The pk values.</param>
        /// <returns>Whether existing.</returns>
        public bool Exists<EntityType>(params object[] pkValues)
            where EntityType : Entity, new()
        {
            Check.Require(pkValues != null && pkValues.Length > 0, "pkValues could not be null or empty.");
            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            string[] pks = Entity.GetPrimaryKeyMappingColumnNames(ec);

            WhereClip where = BuildEqualWhereClip(pkValues, pks);

            if (IsCacheTurnedOn && ec.IsAutoPreLoad)
            {
                return FindFromPreLoad<EntityType>(ec, where) != null;
            }

            string cacheKey = null;
            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0)
            {
                cacheKey = ComputeCacheKey(typeof(EntityType).ToString() + "|Exists", where);
                object cachedObj = GetCache(cacheKey);
                if (cachedObj != null)
                {
                    return (bool)cachedObj;
                }
            }

            IDataReader reader = dbHelper.SelectReadOnly(ec.ViewName, pks, ParseExpressionByMetaData(ec, where.ToString()), where.ParamValues, null);
            bool retBool = false;
;
            if (reader.Read())
            {
                retBool = true;
            }
            reader.Close();

            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0 && cacheKey != null)
            {
                AddCache(cacheKey, retBool, GetTableCacheExpireSeconds(ec.ViewName));
            }

            return retBool;
        }

        /// <summary>
        /// Whether entity exists.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns>Whether existing.</returns>
        public bool Exists<EntityType>(WhereClip where)
            where EntityType : Entity, new()
        {
            Check.Require(where != null, "where could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            ParseValuesForPropertyItemParams(where.ParamValues, ec);
            
            if (IsCacheTurnedOn && ec.IsAutoPreLoad)
            {
                return FindFromPreLoad<EntityType>(ec, where) != null;
            }

            string cacheKey = null;
            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0)
            {
                cacheKey = ComputeCacheKey(typeof(EntityType).ToString() + "|Exists", where);
                object cachedObj = GetCache(cacheKey);
                if (cachedObj != null)
                {
                    return (bool)cachedObj;
                }
            }

            IDataReader reader = dbHelper.SelectReadOnly(ec.ViewName, Entity.GetPrimaryKeyMappingColumnNames(ec), ParseExpressionByMetaData(ec, where.ToString()), where.ParamValues, null);
            bool retBool = false;
            if (reader.Read())
            {
                retBool = true;
            }
            reader.Close();

            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0 && cacheKey != null)
            {
                AddCache(cacheKey, retBool, GetTableCacheExpireSeconds(ec.ViewName));
            }

            return retBool;
        }

        /// <summary>
        /// Finds the array.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns></returns>
        public EntityType[] FindArray<EntityType>(string sql, params object[] paramValues)
            where EntityType : Entity, new()
        {
            Check.Require(sql != null, "sql could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();
            
            ParseValuesForPropertyItemParams(paramValues, ec);

            //if (IsCacheTurnedOn && ec.IsAutoPreLoad)
            //{
            //    return FindArrayFromPreLoad<EntityType>(ec, new WhereClip(sql, paramValues), OrderByClip.Default);
            //}

            string cacheKey = null;
            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0)
            {
                cacheKey = ComputeCacheKey(typeof(EntityType).ToString() + "|FindArray", new WhereClip(sql, paramValues));
                object cachedObj = GetCache(cacheKey);
                if (cachedObj != null)
                {
                    return (EntityType[])cachedObj;
                }
            }

            IDataReader reader = dbHelper.SelectReadOnly(ParseExpressionByMetaData(ec, sql), paramValues);
            List<EntityType> objs = new List<EntityType>();
            while (reader.Read())
            {
                EntityType obj = CreateEntity<EntityType>();
                obj.SetPropertyValues(reader);
                objs.Add(obj);
            }
            reader.Close();

            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0 && cacheKey != null)
            {
                AddCache(cacheKey, objs.ToArray(), GetTableCacheExpireSeconds(ec.ViewName));
            }

            return objs.ToArray();
        }

        /// <summary>
        /// Finds the array.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>The result.</returns>
        public EntityType[] FindArray<EntityType>(WhereClip where, OrderByClip orderBy)
            where EntityType : Entity, new()
        {
            EntityConfiguration ec = new EntityType().GetEntityConfiguration();
            return FindArray<EntityType>(ec, ec.ViewName, where, orderBy);
        }

        /// <summary>
        /// Finds the array.
        /// </summary>
        /// <param name="orderBy">The order by.</param>
        /// <returns></returns>
        public EntityType[] FindArray<EntityType>(OrderByClip orderBy)
            where EntityType : Entity, new()
        {
            return FindArray<EntityType>(WhereClip.All, orderBy);
        }

        /// <summary>
        /// Finds the array.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public EntityType[] FindArray<EntityType>(WhereClip where)
            where EntityType : Entity, new()
        {
            return FindArray<EntityType>(where, OrderByClip.Default);
        }

        /// <summary>
        /// Finds the array.
        /// </summary>
        /// <returns></returns>
        public EntityType[] FindArray<EntityType>()
            where EntityType : Entity, new()
        {
            return FindArray<EntityType>(WhereClip.All, OrderByClip.Default);
        }

        /// <summary>
        /// Finds the array.
        /// </summary>
        /// <param name="ec">The ec.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>The result.</returns>
        internal EntityType[] FindArray<EntityType>(EntityConfiguration ec, string viewName, WhereClip where, OrderByClip orderBy)
            where EntityType : Entity, new()
        {
            Check.Require(where != null, "where could not be null.");
            Check.Require(orderBy != null, "orderBy could not be null.");

            ParseValuesForPropertyItemParams(where.ParamValues, ec);

            if (IsCacheTurnedOn && ec.IsAutoPreLoad && ec.ViewName == viewName)
            {
                return FindArrayFromPreLoad<EntityType>(ec, where, orderBy);
            }

            string cacheKey = null;
            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0)
            {
                cacheKey = ComputeCacheKey(typeof(EntityType).ToString() + "|FindArray_" + viewName, where, orderBy);
                object cachedObj = GetCache(cacheKey);
                if (cachedObj != null)
                {
                    return (EntityType[])cachedObj;
                }
            }

            IDataReader reader = dbHelper.SelectReadOnly(viewName, Entity.GetPropertyMappingColumnNames(ec), where == null ? null : ParseExpressionByMetaData(ec, where.ToString()), where == null ? null : where.ParamValues, orderBy == null ? null : ParseExpressionByMetaData(ec, orderBy.ToString()));
            List<EntityType> objs = new List<EntityType>();
            while (reader.Read())
            {
                EntityType obj = CreateEntity<EntityType>();
                obj.SetPropertyValues(reader);
                objs.Add(obj);
            }
            reader.Close();

            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0 && cacheKey != null)
            {
                AddCache(cacheKey, objs.ToArray(), GetTableCacheExpireSeconds(ec.ViewName));
            }

            return objs.ToArray();
        }

        /// <summary>
        /// Finds the data table.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns></returns>
        public DataTable FindDataTable<EntityType>(WhereClip where, OrderByClip orderBy)
            where EntityType : Entity, new()
        {
            Check.Require(where != null, "where could not be null.");
            Check.Require(orderBy != null, "orderBy could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            ParseValuesForPropertyItemParams(where.ParamValues, ec);

            if (IsCacheTurnedOn && ec.IsAutoPreLoad)
            {
                return FindTableFromPreLoad<EntityType>(ec, where, orderBy);
            }

            string cacheKey = null;
            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0)
            {
                cacheKey = ComputeCacheKey(typeof(EntityType).ToString() + "|FindDataTable", where, orderBy);
                object cachedObj = GetCache(cacheKey);
                if (cachedObj != null)
                {
                    return (DataTable)cachedObj;
                }
            }

            DataSet ds = dbHelper.Select(ec.ViewName, Entity.GetPropertyMappingColumnNames(ec), where == null ? null : ParseExpressionByMetaData(ec, where.ToString()), where == null ? null : where.ParamValues, orderBy == null ? null : ParseExpressionByMetaData(ec, orderBy.ToString()));
            DataTable retTable = (ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : null);
            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0 && cacheKey != null)
            {
                AddCache(cacheKey, retTable, GetTableCacheExpireSeconds(ec.ViewName));
            }

            return retTable;
        }

        /// <summary>
        /// Finds the array by stored procedure.
        /// </summary>
        /// <param name="storedProcName">Name of the stored proc.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="outParamNames">The out param names.</param>
        /// <param name="outParamTypes">The out param types.</param>
        /// <param name="outParamResults">The out param results.</param>
        /// <param name="tran">The tran.</param>
        /// <returns>The result.</returns>
        public EntityType[] FindArrayByStoredProcedure<EntityType>(string storedProcName, string[] paramNames, object[] paramValues, string[] outParamNames, DbType[] outParamTypes, out object[] outParamResults, DbTransaction tran)
            where EntityType : Entity, new()
        {
            Check.Require(!string.IsNullOrEmpty(storedProcName), "storedProcName could not be null or empty.");

            DataSet ds = ExecuteStoredProcedure(storedProcName, paramNames, paramValues, outParamNames, outParamTypes, out outParamResults, tran);
            DataTable dt = null;
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
                EntityType[] objs = new EntityType[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objs[i] = CreateEntity<EntityType>();
                    objs[i].SetPropertyValues(dt.Rows[i]);
                }
                return objs;
            }
            else
            {
                return new EntityType[0];
            }
        }

        /// <summary>
        /// Finds the array by stored procedure.
        /// </summary>
        /// <param name="storedProcName">Name of the stored proc.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="outParamNames">The out param names.</param>
        /// <param name="outParamTypes">The out param types.</param>
        /// <param name="outParamResults">The out param results.</param>
        /// <returns>The result.</returns>
        public EntityType[] FindArrayByStoredProcedure<EntityType>(string storedProcName, string[] paramNames, object[] paramValues, string[] outParamNames, DbType[] outParamTypes, out object[] outParamResults)
            where EntityType : Entity, new()
        {
            return FindArrayByStoredProcedure<EntityType>(storedProcName, paramNames, paramValues, outParamNames, outParamTypes, out outParamResults, null);
        }

        /// <summary>
        /// Finds the array by stored procedure.
        /// </summary>
        /// <param name="storedProcName">Name of the stored proc.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <param name="tran">The tran.</param>
        /// <returns>The result.</returns>
        public EntityType[] FindArrayByStoredProcedure<EntityType>(string storedProcName, string[] paramNames, object[] paramValues, DbTransaction tran)
            where EntityType : Entity, new()
        {
            Check.Require(!string.IsNullOrEmpty(storedProcName), "storedProcName could not be null or empty.");

            string cacheKey = null;
            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(storedProcName) > 0)
            {
                cacheKey = ComputeCacheKey(typeof(EntityType).ToString() + "|" + storedProcName, paramNames, paramValues);
                object cachedObj = GetCache(cacheKey);
                if (cachedObj != null)
                {
                    return (EntityType[])cachedObj;
                }
            }

            IDataReader reader = ExecuteStoredProcedureReadOnly(storedProcName, paramNames, paramValues, tran);
            List<EntityType> objs = new List<EntityType>();
            while (reader.Read())
            {
                EntityType obj = CreateEntity<EntityType>();
                obj.SetPropertyValues(reader);
                objs.Add(obj);
            }
            reader.Close();

            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(storedProcName) > 0 && cacheKey != null)
            {
                AddCache(cacheKey, objs.ToArray(), GetTableCacheExpireSeconds(storedProcName));
            }

            return objs.ToArray();
        }

        /// <summary>
        /// Finds the array by stored procedure.
        /// </summary>
        /// <param name="storedProcName">Name of the stored proc.</param>
        /// <param name="paramNames">The param names.</param>
        /// <param name="paramValues">The param values.</param>
        /// <returns>The result.</returns>
        public EntityType[] FindArrayByStoredProcedure<EntityType>(string storedProcName, string[] paramNames, object[] paramValues)
            where EntityType : Entity, new()
        {
            return FindArrayByStoredProcedure<EntityType>(storedProcName, paramNames, paramValues, null);
        }

        /// <summary>
        /// Gets the page selector.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>The page selector.</returns>
        public PageSelector<EntityType> GetPageSelector<EntityType>(WhereClip where, OrderByClip orderBy, int pageSize)
            where EntityType : Entity, new()
        {
            Check.Require(where != null, "where could not be null.");
            Check.Require(orderBy != null, "orderBy could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            ParseValuesForPropertyItemParams(where.ParamValues, ec);

            string[] pks = Entity.GetPrimaryKeyMappingColumnNames(ec);
            string keyColumn = null;
            if (pks != null && pks.Length == 1)
            {
                keyColumn = pks[0];
            }
            Check.Require(keyColumn != null, "GetPageSelector only supports Entity which having single primary key column.");
            IPageSplit ps = dbHelper.SelectPageSplit(ec.ViewName, Entity.GetPropertyMappingColumnNames(ec), where == null ? null : ParseExpressionByMetaData(ec, where.ToString()), orderBy == null ? null : ParseExpressionByMetaData(ec, orderBy.ToString()), keyColumn, where == null ? null : where.ParamValues);
            ps.PageSize = pageSize;
            return new PageSelector<EntityType>(this, ps);
        }

        /// <summary>
        /// Counts the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="property">The property.</param>
        /// <param name="isDistinct">if set to <c>true</c> [is distinct].</param>
        /// <returns>The result.</returns>
        public int Count<EntityType>(WhereClip where, PropertyItem property, bool isDistinct)
            where EntityType : Entity, new()
        {
            Check.Require(where != null, "where could not be null.");
            Check.Require(!property.Equals(null), "property could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            ParseValuesForPropertyItemParams(where.ParamValues, ec);

            string column = ParseExpressionByMetaData(ec, property.ColumnName);
            if (isDistinct)
            {
                column = "DISTINCT " + column;
            }
            column = string.Format("COUNT({0})", column);

            if (IsCacheTurnedOn && ec.IsAutoPreLoad)
            {
                return Convert.ToInt32(FindScalarFromPreLoad<EntityType>(column, ec, where));
            }

            return Convert.ToInt32(GetAggregateValue<EntityType>(where, column, true));
        }

        /// <summary>
        /// Counts the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns>The result.</returns>
        public int Count<EntityType>(WhereClip where)
            where EntityType : Entity, new()
        {
            return Count<EntityType>(where, PropertyItem.All, false);
        }

        /// <summary>
        /// Maxes the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="property">The property.</param>
        /// <returns>The result.</returns>
        public object Max<EntityType>(WhereClip where, PropertyItem property)
            where EntityType : Entity, new()
        {
            Check.Require(where != null, "where could not be null.");
            Check.Require(!property.Equals(null), "property could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            ParseValuesForPropertyItemParams(where.ParamValues, ec);

            string column = ParseExpressionByMetaData(ec, property.ColumnName);
            column = string.Format("MAX({0})", column);

            if (IsCacheTurnedOn && ec.IsAutoPreLoad)
            {
                return FindScalarFromPreLoad<EntityType>(column, ec, where);
            }

            return GetAggregateValue<EntityType>(where, column, true);
        }

        /// <summary>
        /// Maxes the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public object Max<EntityType>(WhereClip where, PropertyItemParam property)
            where EntityType : Entity, new()
        {
            Check.Require(where != null, "where could not be null.");
            Check.Require(!property.Equals(null), "property could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            ParseValuesForPropertyItemParams(where.ParamValues, ec);

            string column = ParseExpressionByMetaData(ec, property.CustomValue);
            column = string.Format("MAX({0})", column);

            if (IsCacheTurnedOn && ec.IsAutoPreLoad)
            {
                return FindScalarFromPreLoad<EntityType>(column, ec, where);
            }

            return GetAggregateValue<EntityType>(where, column, false);
        }

        /// <summary>
        /// Mins the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="property">The property.</param>
        /// <returns>The result.</returns>
        public object Min<EntityType>(WhereClip where, PropertyItem property)
            where EntityType : Entity, new()
        {
            Check.Require(where != null, "where could not be null.");
            Check.Require(!property.Equals(null), "property could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            ParseValuesForPropertyItemParams(where.ParamValues, ec);

            string column = ParseExpressionByMetaData(ec, property.ColumnName);
            column = string.Format("MIN({0})", column);

            if (IsCacheTurnedOn && ec.IsAutoPreLoad)
            {
                return FindScalarFromPreLoad<EntityType>(column, ec, where);
            }

            return GetAggregateValue<EntityType>(where, column, true);
        }

        /// <summary>
        /// Mins the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public object Min<EntityType>(WhereClip where, PropertyItemParam property)
            where EntityType : Entity, new()
        {
            Check.Require(where != null, "where could not be null.");
            Check.Require(!property.Equals(null), "property could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            ParseValuesForPropertyItemParams(where.ParamValues, ec);

            string column = ParseExpressionByMetaData(ec, property.CustomValue);
            column = string.Format("MIN({0})", column);

            if (IsCacheTurnedOn && ec.IsAutoPreLoad)
            {
                return FindScalarFromPreLoad<EntityType>(column, ec, where);
            }

            return GetAggregateValue<EntityType>(where, column, false);
        }

        /// <summary>
        /// Sums the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="property">The property.</param>
        /// <returns>The result.</returns>
        public object Sum<EntityType>(WhereClip where, PropertyItem property)
            where EntityType : Entity, new()
        {
            Check.Require(where != null, "where could not be null.");
            Check.Require(!property.Equals(null), "property could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            ParseValuesForPropertyItemParams(where.ParamValues, ec);

            string column = ParseExpressionByMetaData(ec, property.ColumnName);
            column = string.Format("SUM({0})", column);

            if (IsCacheTurnedOn && ec.IsAutoPreLoad)
            {
                return FindScalarFromPreLoad<EntityType>(column, ec, where);
            }

            return GetAggregateValue<EntityType>(where, column, true);
        }

        /// <summary>
        /// Sums the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public object Sum<EntityType>(WhereClip where, PropertyItemParam property)
            where EntityType : Entity, new()
        {
            Check.Require(where != null, "where could not be null.");
            Check.Require(!property.Equals(null), "property could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            ParseValuesForPropertyItemParams(where.ParamValues, ec);

            string column = ParseExpressionByMetaData(ec, property.CustomValue);
            column = string.Format("SUM({0})", column);

            if (IsCacheTurnedOn && ec.IsAutoPreLoad)
            {
                return FindScalarFromPreLoad<EntityType>(column, ec, where);
            }

            return GetAggregateValue<EntityType>(where, column, false);
        }

        /// <summary>
        /// Avgs the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="property">The property.</param>
        /// <returns>The result.</returns>
        public object Avg<EntityType>(WhereClip where, PropertyItem property)
            where EntityType : Entity, new()
        {
            Check.Require(where != null, "where could not be null.");
            Check.Require(!property.Equals(null), "property could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            ParseValuesForPropertyItemParams(where.ParamValues, ec);

            string column = ParseExpressionByMetaData(ec, property.ColumnName);
            column = string.Format("AVG({0})", column);

            if (IsCacheTurnedOn && ec.IsAutoPreLoad)
            {
                return FindScalarFromPreLoad<EntityType>(column, ec, where);
            }

            return GetAggregateValue<EntityType>(where, column, true);
        }

        /// <summary>
        /// Avgs the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public object Avg<EntityType>(WhereClip where, PropertyItemParam property)
            where EntityType : Entity, new()
        {
            Check.Require(where != null, "where could not be null.");
            Check.Require(!property.Equals(null), "property could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            ParseValuesForPropertyItemParams(where.ParamValues, ec);

            string column = ParseExpressionByMetaData(ec, property.CustomValue);
            column = string.Format("AVG({0})", column);

            if (IsCacheTurnedOn && ec.IsAutoPreLoad)
            {
                return FindScalarFromPreLoad<EntityType>(column, ec, where);
            }

            return GetAggregateValue<EntityType>(where, column, false);
        }

        /// <summary>
        /// Finds the single property array.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="property">The property.</param>
        /// <returns>The result.</returns>
        public object[] FindSinglePropertyArray<EntityType>(WhereClip where, OrderByClip orderBy, PropertyItem property)
            where EntityType : Entity, new()
        {
            Check.Require(where != null, "where could not be null.");
            Check.Require(orderBy != null, "orderBy could not be null.");
            Check.Require(!property.Equals(null), "property could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            ParseValuesForPropertyItemParams(where.ParamValues, ec);

            string column = ParseExpressionByMetaData(ec, property.ColumnName);

            if (IsCacheTurnedOn && ec.IsAutoPreLoad)
            {
                return FindSinglePropertyArrayFromPreLoad<EntityType>(column, ec, where, orderBy);
            }

            string cacheKey = null;
            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0)
            {
                cacheKey = ComputeCacheKey(typeof(EntityType).ToString() + "|FindSinglePropertyArray_" + property.PropertyName, where, orderBy);
                object cachedObj = GetCache(cacheKey);
                if (cachedObj != null)
                {
                    return (object[])cachedObj;
                }
            }

            IDataReader reader = dbHelper.SelectReadOnly(ec.ViewName, new string[] { column }, ParseExpressionByMetaData(ec, where.ToString()), where.ParamValues, ParseExpressionByMetaData(ec, orderBy.ToString()));
            List<object> list = new List<object>();
            while (reader.Read())
            {
                list.Add(reader.GetValue(0));
            }
            reader.Close();

            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0 && cacheKey != null)
            {
                AddCache(cacheKey, list.ToArray(), GetTableCacheExpireSeconds(ec.ViewName));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Finds the scalar.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="property">The property.</param>
        /// <returns>The result.</returns>
        public object FindScalar<EntityType>(WhereClip where, OrderByClip orderBy, PropertyItem property)
            where EntityType : Entity, new()
        {
            Check.Require(where != null, "where could not be null.");
            Check.Require(orderBy != null, "orderBy could not be null.");
            Check.Require(!property.Equals(null), "property could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            ParseValuesForPropertyItemParams(where.ParamValues, ec);

            string column = ParseExpressionByMetaData(ec, property.ColumnName);

            if (IsCacheTurnedOn && ec.IsAutoPreLoad)
            {
                return FindScalarFromPreLoad<EntityType>(column, ec, where);
            }

            string cacheKey = null;
            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0)
            {
                cacheKey = ComputeCacheKey(typeof(EntityType).ToString() + "|FindScalar" + property.PropertyName, where, orderBy);
                object cachedObj = GetCache(cacheKey);
                if (cachedObj != null)
                {
                    return cachedObj;
                }
            }

            IDataReader reader = dbHelper.SelectReadOnly(ec.ViewName, new string[] { column }, ParseExpressionByMetaData(ec, where.ToString()), where.ParamValues, ParseExpressionByMetaData(ec, orderBy.ToString()));
            object retObj = null;
            if (reader.Read())
            {
                retObj = reader.GetValue(0);
            }
            reader.Close();

            if (IsCacheTurnedOn && GetTableCacheExpireSeconds(ec.ViewName) > 0 && cacheKey != null)
            {
                AddCache(cacheKey, retObj, GetTableCacheExpireSeconds(ec.ViewName));
            }

            return retObj;
        }

        /// <summary>
        /// Creates the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="tran">The tran.</param>
        /// <returns>The auto incrememtal column value. If there is no auto column, the value is 0.</returns>
        internal int Create<EntityType>(EntityType obj, DbTransaction tran)
            where EntityType : Entity, new()
        {
            Check.Require(obj != null, "relatedObj could not be null.");
            EntityConfiguration ec = obj.GetEntityConfiguration();

            if ((!db.IsBatchConnection) && ec.IsBatchUpdate)
            {
                Gateway batchGateway = BeginBatchGateway(ec.BatchSize, tran);
                int retInt = batchGateway.Create<EntityType>(obj, tran);
                batchGateway.EndBatch();
                return retInt;
            }

            string[] pks = Entity.GetPrimaryKeyMappingColumnNames(ec);
            string keyColumn = null;
            if (pks != null && pks.Length == 1)
            {
                PropertyConfiguration pc = null;

                foreach (PropertyConfiguration item in ec.Properties)
                {
                    if (item.MappingName == pks[0])
                    {
                        pc = item;
                        if (item.IsReadOnly)
                        {
                            keyColumn = pks[0];
                        }
                        break;
                    }
                }

                object pkValue = Entity.GetPropertyValues(obj, pks)[0];
                if (pc.IsPrimaryKey && (!ec.IsRelation) && pc.SqlType.Trim().ToLower() == "uniqueidentifier" && (pkValue == null || ((Guid)pkValue) == default(Guid)))
                {
                    Util.DeepGetProperty(obj.GetType(), pc.Name).SetValue(obj, Guid.NewGuid(), null);
                }
            }

            //bind query proxy handler to relatedObj
            obj.SetQueryProxy(new Entity.QueryProxyHandler(OnQueryHandler));

            int retAutoID = 0;

            if (MetaDataManager.IsNonRelatedEntity(ec.Name))
            {
                List<string> byteArrayColumns = MetaDataManager.GetByteArrayColumns(ec.Name);
                List<string> nullableNumberColumns = MetaDataManager.GetNullableNumberColumns(ec.Name);
                string[] createColumnNames = Entity.GetCreatePropertyMappingColumnNames(obj.GetEntityConfiguration());
                object[] createColumnValues = Entity.GetCreatePropertyValues(obj.GetType(), obj);
                List<string> sqlDefaultValueColumns = MetaDataManager.GetSqlTypeWithDefaultValueColumns(ec.Name);
                if (sqlDefaultValueColumns.Count > 0)
                {
                    FilterNullSqlDefaultValueColumns(sqlDefaultValueColumns, createColumnNames, createColumnValues, out createColumnNames, out createColumnValues);
                }
                if (byteArrayColumns.Count == 0 && nullableNumberColumns.Count == 0)
                {
                    retAutoID = dbHelper.Insert(ec.MappingName, createColumnNames, createColumnValues, tran, keyColumn);
                }
                else
                {
                    DbType[] dbTypes = BuildDbTypes(createColumnNames, createColumnValues, byteArrayColumns, nullableNumberColumns);
                    retAutoID = dbHelper.Insert(ec.MappingName, createColumnNames, dbTypes, createColumnValues, tran, keyColumn);
                }

                if (retAutoID > 0)
                {
                    //save the retAutoID value to entity's ID property
                    string autoIdProperty = MetaDataManager.GetEntityAutoId(ec.Name);
                    if (autoIdProperty != null)
                    {
                        PropertyInfo pi = Util.DeepGetProperty(obj.GetType(), autoIdProperty);
                        pi.SetValue(obj, Convert.ChangeType(retAutoID, pi.PropertyType), null);
                    }
                }

                if (GetTableCacheExpireSeconds(ec.ViewName) > 0 || ec.IsAutoPreLoad) RemoveCaches(typeof(EntityType).ToString());
            }
            else
            {
                if (db.DbProvider.SupportADO20Transaction && tran == null)
                {
                    //ado2.0 tran
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                    {
                        retAutoID = DoCascadeInsert<EntityType>(obj, tran, ec, keyColumn);
                        DoCascadePropertySave<EntityType>(obj, tran, ec);

                        scope.Complete();
                    }
                }
                else
                {
                    DbTransaction t;
                    if (tran == null)
                    {
                        t = BeginTransaction();
                    }
                    else
                    {
                        t = tran;
                    }

                    try
                    {
                        retAutoID = DoCascadeInsert<EntityType>(obj, t, ec, keyColumn);
                        DoCascadePropertySave<EntityType>(obj, tran, ec);

                        if (tran == null)
                        {
                            t.Commit();
                        }
                    }
                    catch
                    {
                        if (tran == null)
                        {
                            t.Rollback();
                        }
                        throw;
                    }
                    finally
                    {
                        if (tran == null)
                        {
                            CloseTransaction(t);
                        }
                    }
                }

                CascadeRemoveEntityCaches(ec);
            }

            obj.Attach();
            obj.SetQueryProxy(new Entity.QueryProxyHandler(OnQueryHandler));
            return retAutoID;
        }

        /// <summary>
        /// Creates the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The auto incrememtal column value. If there is no auto column, the value is 0.</returns>
        internal int Create<EntityType>(EntityType obj)
            where EntityType : Entity, new()
        {
            return Create<EntityType>(obj, null);
        }

        /// <summary>
        /// Updates the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="tran">The tran.</param>
        internal void Update<EntityType>(EntityType obj, DbTransaction tran)
            where EntityType : Entity, new()
        {
            Check.Require(obj != null, "relatedObj could not be null.");
            EntityConfiguration ec = obj.GetEntityConfiguration();

            if ((!db.IsBatchConnection) && ec.IsBatchUpdate)
            {
                Gateway batchGateway = BeginBatchGateway(ec.BatchSize, tran);
                batchGateway.Update<EntityType>(obj, tran);
                batchGateway.EndBatch();
                return;
            }

            string[] pks = Entity.GetPrimaryKeyMappingColumnNames(ec);
            object[] pkValues = Entity.GetPropertyValues(obj, pks);

            WhereClip where = BuildEqualWhereClip(pkValues, pks);

            Dictionary<string, object> modifiedProperties = obj.GetModifiedProperties(obj.GetType());

            if (MetaDataManager.IsNonRelatedEntity(ec.Name))
            {
                if (modifiedProperties.Count > 0)
                {
                    string[] columnNames = ec.GetMappingColumnNames(new List<string>(modifiedProperties.Keys).ToArray());
                    object[] columnValues = new List<object>(modifiedProperties.Values).ToArray();
                    List<string> byteArrayColumns = MetaDataManager.GetByteArrayColumns(ec.Name);
                    List<string> nullableNumberColumns = MetaDataManager.GetNullableNumberColumns(ec.Name);
                    if (byteArrayColumns.Count == 0 && nullableNumberColumns.Count == 0)
                    {
                        dbHelper.Update(ec.MappingName, columnNames, columnValues, ParseExpressionByMetaData(ec, where.ToString()), where.ParamValues, tran);
                    }
                    else
                    {
                        DbType[] types = BuildDbTypes(columnNames, columnValues, byteArrayColumns, nullableNumberColumns);
                        dbHelper.Update(ec.MappingName, columnNames, types, columnValues, ParseExpressionByMetaData(ec, where.ToString()), null, where.ParamValues, tran);
                    }
                }

                if (GetTableCacheExpireSeconds(ec.ViewName) > 0 || ec.IsAutoPreLoad) RemoveCaches(typeof(EntityType).ToString());
            }
            else
            {
                if (db.DbProvider.SupportADO20Transaction && tran == null)
                {
                    //ado2.0 tran
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                    {
                        DoDeleteToDeleteObjects(obj, tran);
                        DoCascadeUpdate<EntityType>(obj, tran, ec, where, modifiedProperties);
                        DoCascadePropertySave<EntityType>(obj, tran, ec);

                        scope.Complete();
                    }
                }
                else
                {
                    DbTransaction t;
                    if (tran == null)
                    {
                        t = BeginTransaction();
                    }
                    else
                    {
                        t = tran;
                    }

                    try
                    {
                        DoDeleteToDeleteObjects(obj, tran);
                        DoCascadeUpdate<EntityType>(obj, t, ec, where, modifiedProperties);
                        DoCascadePropertySave<EntityType>(obj, tran, ec);

                        if (tran == null)
                        {
                            t.Commit();
                        }
                    }
                    catch
                    {
                        if (tran == null)
                        {
                            t.Rollback();
                        }
                        throw;
                    }
                    finally
                    {
                        if (tran == null)
                        {
                            CloseTransaction(t);
                        }
                    }
                }

                CascadeRemoveEntityCaches(ec);
            }

            obj.SetQueryProxy(new Entity.QueryProxyHandler(OnQueryHandler));
        }

        /// <summary>
        /// Updates the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        internal void Update<EntityType>(EntityType obj)
            where EntityType : Entity, new()
        {
            Update<EntityType>(obj, null);
        }

        /// <summary>
        /// Saves the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="tran">The tran.</param>
        /// <returns>The auto incrememtal column value. If there is no auto column or the entity is already persisted, the value is 0.</returns>
        public int Save<EntityType>(EntityType obj, DbTransaction tran)
            where EntityType : Entity, new()
        {
            Check.Require(obj != null, "relatedObj could not be null.");

            int retAutoID = 0;

            if (obj.IsAttached())
            {
                Update<EntityType>(obj, tran);
            }
            else
            {
                retAutoID = Create<EntityType>(obj, tran);
            }

            return retAutoID;
        }

        /// <summary>
        /// Saves the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The auto incrememtal column value. If there is no auto column or the entity is already persisted, the value is 0.</returns>
        public int Save<EntityType>(EntityType obj)
            where EntityType : Entity, new()
        {
            return Save<EntityType>(obj, null);
        }

        /// <summary>
        /// Deletes the specified tran.
        /// </summary>
        /// <param name="tran">The tran.</param>
        /// <param name="pkValues">The pk values.</param>
        public void Delete<EntityType>(DbTransaction tran, params object[] pkValues)
            where EntityType : Entity, new()
        {
            Check.Require(pkValues != null && pkValues.Length > 0, "relatedObj could not be null or empty.");
            EntityConfiguration ec = new EntityType().GetEntityConfiguration();
            string[] pks = Entity.GetPrimaryKeyMappingColumnNames(ec);

            WhereClip where = BuildEqualWhereClip(pkValues, pks);

            if (MetaDataManager.IsNonRelatedEntity(ec.Name))
            {
                dbHelper.Delete(ec.MappingName, ParseExpressionByMetaData(ec, where.ToString()), where.ParamValues, tran);
                if (GetTableCacheExpireSeconds(ec.ViewName) > 0 || ec.IsAutoPreLoad) RemoveCaches(typeof(EntityType).ToString());
            }
            else
            {
                Gateway findGateway = this;
                if (db.IsBatchConnection)
                {
                    findGateway = new Gateway(new Database(db.DbProvider));
                    findGateway.Db.OnLog += new LogHandler(db.WriteLog);
                }

                EntityType obj = findGateway.Find<EntityType>(pkValues);
                if (obj != null)
                {
                    Delete<EntityType>(obj, tran);
                }

                CascadeRemoveEntityCaches(ec);
            }
        }

        /// <summary>
        /// Deletes the specified pk values.
        /// </summary>
        /// <param name="pkValues">The pk values.</param>
        public void Delete<EntityType>(params object[] pkValues)
            where EntityType : Entity, new()
        {
            Delete<EntityType>(null, pkValues);
        }

        /// <summary>
        /// Deletes the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="tran">The tran.</param>
        public void Delete<EntityType>(EntityType obj, DbTransaction tran)
            where EntityType : Entity, new()
        {
            Check.Require(obj != null, "relatedObj could not be null.");

            object[] pkValues = Entity.GetPrimaryKeyValues(obj);

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            if ((!db.IsBatchConnection) && ec.IsBatchUpdate)
            {
                Gateway batchGateway = BeginBatchGateway(ec.BatchSize, tran);
                batchGateway.Delete<EntityType>(obj, tran);
                batchGateway.EndBatch();
                return;
            }

            string[] pks = Entity.GetPrimaryKeyMappingColumnNames(ec);

            WhereClip where = BuildEqualWhereClip(pkValues, pks);

            if (MetaDataManager.IsNonRelatedEntity(ec.Name))
            {
                dbHelper.Delete(ec.MappingName, ParseExpressionByMetaData(ec, where.ToString()), where.ParamValues, tran);
                if (GetTableCacheExpireSeconds(ec.ViewName) > 0 || ec.IsAutoPreLoad) RemoveCaches(typeof(EntityType).ToString());
            }
            else
            {
                bool deletedAsChildEntity = DeleteAsChildEntity(ec.Name, pkValues, tran);

                if (!deletedAsChildEntity)
                {
                    if (db.DbProvider.SupportADO20Transaction && tran == null)
                    {
                        //ado2.0 tran
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                        {
                            DoCascadeDelete<EntityType>(obj, tran, ec, where);

                            scope.Complete();
                        }
                    }
                    else
                    {
                        DbTransaction t;
                        if (tran == null)
                        {
                            t = BeginTransaction();
                        }
                        else
                        {
                            t = tran;
                        }

                        try
                        {
                            DoCascadeDelete<EntityType>(obj, t, ec, where);

                            if (tran == null)
                            {
                                t.Commit();
                            }
                        }
                        catch
                        {
                            if (tran == null)
                            {
                                t.Rollback();
                            }
                            throw;
                        }
                        finally
                        {
                            if (tran == null)
                            {
                                CloseTransaction(t);
                            }
                        }
                    }
                }

                CascadeRemoveEntityCaches(ec);
            }

            obj.SetQueryProxy(null);
            obj.Detach();
        }

        /// <summary>
        /// Deletes the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public void Delete<EntityType>(EntityType obj)
            where EntityType : Entity, new()
        {
            Delete<EntityType>(obj, null);
        }

        /// <summary>
        /// Batch delete.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="tran">The tran.</param>
        public void Delete<EntityType>(WhereClip where, DbTransaction tran)
           where EntityType : Entity, new()
        {
            Check.Require(where != null, "where could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            ParseValuesForPropertyItemParams(where.ParamValues, ec);

            if (MetaDataManager.IsNonRelatedEntity(ec.Name))
            {
                dbHelper.Delete(ec.MappingName, ParseExpressionByMetaData(ec, where.ToString()), where.ParamValues, tran);
                if (GetTableCacheExpireSeconds(ec.ViewName) > 0 || ec.IsAutoPreLoad) RemoveCaches(typeof(EntityType).ToString());
            }
            else
            {
                Gateway findGateway = this;
                if (db.IsBatchConnection)
                {
                    findGateway = new Gateway(new Database(db.DbProvider));
                    findGateway.Db.OnLog += new LogHandler(db.WriteLog);
                }

                EntityType[] objs = findGateway.FindArray<EntityType>(where, OrderByClip.Default);

                if (db.DbProvider.SupportADO20Transaction && tran == null)
                {
                    TransactionOptions option = new TransactionOptions();
                    option.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option))
                    {
                        foreach (EntityType obj in objs)
                        {
                            Delete<EntityType>(obj);
                        }

                        scope.Complete();
                    }
                }
                else
                {
                    DbTransaction t = (tran == null ? BeginTransaction(System.Data.IsolationLevel.ReadUncommitted) : tran);

                    try
                    {
                        foreach (EntityType obj in objs)
                        {
                            Delete<EntityType>(obj, t);
                        }

                        if (tran == null)
                        {
                            t.Commit();
                        }
                    }
                    catch
                    {
                        if (tran == null)
                        {
                            t.Rollback();
                        }
                        throw;
                    }
                    finally
                    {
                        if (tran == null)
                        {
                            CloseTransaction(t);
                        }
                    }
                }

                CascadeRemoveEntityCaches(ec);
            }
        }

        /// <summary>
        /// Batches the delete.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="tran">The tran.</param>
        [Obsolete("This method is obsoleted. Please use Delete() instead.")]
        public void BatchDelete<EntityType>(WhereClip where, DbTransaction tran)
           where EntityType : Entity, new()
        {
            Delete<EntityType>(where, tran);
        }

        /// <summary>
        /// Batch delete.
        /// </summary>
        /// <param name="where">The where.</param>
        [Obsolete("This method is obsoleted. Please use Delete() instead.")]
        public void BatchDelete<EntityType>(WhereClip where)
            where EntityType : Entity, new()
        {
            Delete<EntityType>(where, null);
        }

        /// <summary>
        /// Batch Deletes the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        public void Delete<EntityType>(WhereClip where)
            where EntityType : Entity, new()
        {
            Delete<EntityType>(where, null);
        }

        /// <summary>
        /// Batch update.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="values">The values.</param>
        /// <param name="where">The where.</param>
        /// <param name="tran">The tran.</param>
        public void Update<EntityType>(PropertyItem[] properties, object[] values, WhereClip where, DbTransaction tran)
           where EntityType : Entity, new()
        {
            Check.Require(properties != null && properties.Length > 0, "properties to update could not be null or empty.");
            Check.Require(values != null && values.Length > 0, "values to update could not be null or empty.");
            Check.Require(properties.Length == values.Length, "length of properties and values should be equal.");
            Check.Require(where != null, "where could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();

            ParseValuesForPropertyItemParams(values, ec);
            ParseValuesForPropertyItemParams(where.ParamValues, ec);

            if (MetaDataManager.IsNonRelatedEntity(ec.Name))
            {
                string[] columns = new string[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    columns[i] = ec.GetMappingColumnName(properties[i].PropertyName);
                }

                dbHelper.Update(ec.MappingName, columns, values, ParseExpressionByMetaData(ec, where.ToString()), where.ParamValues, tran);
                if (GetTableCacheExpireSeconds(ec.ViewName) > 0 || ec.IsAutoPreLoad) RemoveCaches(typeof(EntityType).ToString());
            }
            else
            {
                Gateway findGateway = this;
                if (db.IsBatchConnection)
                {
                    findGateway = new Gateway(new Database(db.DbProvider));
                    findGateway.Db.OnLog += new LogHandler(db.WriteLog);
                }
                EntityType[] objs = findGateway.FindArray<EntityType>(where, OrderByClip.Default);
                Dictionary<string, object> changedProperties = new Dictionary<string, object>();
                for (int i = 0; i < properties.Length; i++)
                {
                    changedProperties.Add(properties[i].PropertyName, values[i]);
                }

                if (db.DbProvider.SupportADO20Transaction && tran == null)
                {
                    TransactionOptions option = new TransactionOptions();
                    option.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option))
                    {
                        foreach (EntityType obj in objs)
                        {
                            SetModifiedProperties<EntityType>(changedProperties, obj);
                            Update<EntityType>(obj);
                        }

                        scope.Complete();
                    }
                }
                else
                {
                    DbTransaction t = (tran == null ? BeginTransaction(System.Data.IsolationLevel.ReadUncommitted) : tran);

                    try
                    {
                        foreach (EntityType obj in objs)
                        {
                            SetModifiedProperties<EntityType>(changedProperties, obj);
                            Update<EntityType>(obj, t);
                        }

                        if (tran == null)
                        {
                            t.Commit();
                        }
                    }
                    catch
                    {
                        if (tran == null)
                        {
                            t.Rollback();
                        }
                        throw;
                    }
                    finally
                    {
                        if (tran == null)
                        {
                            CloseTransaction(t);
                        }
                    }
                }

                CascadeRemoveEntityCaches(ec);
            }
        }

        /// <summary>
        /// Batches the update.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="values">The values.</param>
        /// <param name="where">The where.</param>
        /// <param name="tran">The tran.</param>
        [Obsolete("This method is obsoleted. Please use Update() instead.")]
        public void BatchUpdate<EntityType>(PropertyItem[] properties, object[] values, WhereClip where, DbTransaction tran)
           where EntityType : Entity, new()
        {
            Update<EntityType>(properties, values, where, tran);
        }

        /// <summary>
        /// Batch update.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="values">The values.</param>
        /// <param name="where">The where.</param>
        [Obsolete("This method is obsoleted. Please use Update() instead.")]
        public void BatchUpdate<EntityType>(PropertyItem[] properties, object[] values, WhereClip where)
           where EntityType : Entity, new()
        {
            Update<EntityType>(properties, values, where, null);
        }

        /// <summary>
        /// Batches the update.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="values">The values.</param>
        /// <param name="where">The where.</param>
        public void Update<EntityType>(PropertyItem[] properties, object[] values, WhereClip where)
           where EntityType : Entity, new()
        {
            Update<EntityType>(properties, values, where, null);
        }

       #endregion

        #region Caching

        private string FilterParams(string sql)
        {
            if (sql == null)
            {
                return null;
            }

            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"@([\w\d_]+)");

            return r.Replace(sql, "?");
        }

        /// <summary>
        /// Computes the cache key.
        /// </summary>
        /// <param name="customPrefix">The custom prefix.</param>
        /// <param name="where">The where.</param>
        /// <returns>The cache key.</returns>
        public string ComputeCacheKey(string customPrefix, WhereClip where)
        {
            Check.Require(where != null, "where could not be null.");

            StringBuilder sb = new StringBuilder();
            sb.Append(customPrefix);
            sb.Append(FilterParams(where.ToString()));
            if (where.ParamValues != null)
            {
                foreach (object obj in where.ParamValues)
                {
                    sb.Append(SerializationManager.Serialize(obj));
                }
            }
            sb.Append(db.ConnectionString);
            return sb.ToString();
        }

        /// <summary>
        /// Computes the cache key.
        /// </summary>
        /// <param name="customPrefix">The custom prefix.</param>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns>The cache key.</returns>
        public string ComputeCacheKey(string customPrefix, WhereClip where, OrderByClip orderBy)
        {
            Check.Require(where != null, "where could not be null.");
            Check.Require(orderBy != null, "orderBy could not be null.");

            return ComputeCacheKey(string.Format("{0}{1}", customPrefix, orderBy.ToString()), where);
        }

        /// <summary>
        /// Computes the cache key.
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="inParamNames">The in param names.</param>
        /// <param name="inParamValues">The in param values.</param>
        /// <returns>The cache key.</returns>
        public string ComputeCacheKey(string storedProcedureName, string[] inParamNames, object[] inParamValues)
        {
            Check.Require(storedProcedureName != null, "storedProcedureName could not be null.");
            Check.Require(inParamNames == null || inParamNames.Length == 0 || (inParamValues != null && inParamNames.Length == inParamValues.Length), "inParamNames must be null/empty or has same number of items as inParamValues.");

            StringBuilder sb = new StringBuilder();
            sb.Append(storedProcedureName);
            if (inParamNames != null)
            {
                for (int i = 0; i < inParamNames.Length; i++)
                {
                    sb.Append(inParamNames[i]);
                    sb.Append(SerializationManager.Serialize(inParamValues[i]));
                }
            }
            sb.Append(db.ConnectionString);
            return sb.ToString();
        }

        internal static readonly Cache cache = new Cache();

        private CacheConfigurationSection cacheConfigSection = null;

        private Dictionary<string, int> tableExpireSecondsMap = null;

        /// <summary>
        /// Gets the table cache expire seconds.
        /// </summary>
        /// <param name="tableOrViewName">Name of the table or view.</param>
        /// <returns>The cache key.</returns>
        public int GetTableCacheExpireSeconds(string tableOrViewName)
        {
            if (!IsCacheTurnedOn)
            {
                return 0;
            }

            string key = tableOrViewName.ToLower();
            if (tableExpireSecondsMap.ContainsKey(key))
            {
                return tableExpireSecondsMap[key];
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is cache turned on.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is cache turned on; otherwise, <c>false</c>.
        /// </value>
        public bool IsCacheTurnedOn
        {
            get
            {
                return (cacheConfigSection != null && cacheConfigSection.Enable);
            }
        }

        /// <summary>
        /// Turns on the cache of this gateway.
        /// </summary>
        public void TurnOnCache()
        {
            if (cacheConfigSection != null)
            {
                cacheConfigSection.Enable = true;
            }
        }

        /// <summary>
        /// Turns off the cache of this gateway.
        /// </summary>
        public void TurnOffCache()
        {
            if (cacheConfigSection != null)
            {
                cacheConfigSection.Enable = false;
            }
        }

        /// <summary>
        /// Adds object into the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expireAfterSeconds">The expire after seconds.</param>
        public void AddCache(string key, object value, int expireAfterSeconds)
        {
            Check.Require(key != null, "key could not be null.");
            //Check.Require(value != null, "value could not be null.");
            Check.Require(expireAfterSeconds > 0, "expireAfterSeconds must > 0.");

            cache.Add(key, value, new AbsoluteTime(DateTime.Now.AddSeconds(expireAfterSeconds)));
        }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The cached object, if not exists, return null.</returns>
        public object GetCache(string key)
        {
            Check.Require(key != null, "key could not be null.");

            return cache.Get(key);
        }

        /// <summary>
        /// Removes the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        public void RemoveCache(string key)
        {
            Check.Require(key != null, "key could not be null.");

            cache.Remove(key);
        }

        /// <summary>
        /// Removes all the caches related to specified table, view or stored procedure.
        /// </summary>
        /// <param name="keyPrefix">Name of the table view or stored proc.</param>
        public void RemoveCaches(string keyPrefix)
        {
            Check.Require(keyPrefix != null, "keyPrefix could not be null.");

            if (!IsCacheTurnedOn)
            {
                return;
            }

            cache.RemoveByKeyPrefix(keyPrefix + "|");
        }

        /// <summary>
        /// Removes the caches.
        /// </summary>
        public void RemoveCaches<EntityType>()
        {
            RemoveCaches(typeof(EntityType).ToString());
        }

        private void CascadeRemoveEntityCaches(EntityConfiguration ec)
        {
            if (!IsCacheTurnedOn)
            {
                return;
            }

            if (GetTableCacheExpireSeconds(ec.ViewName) > 0 || ec.IsAutoPreLoad)
            {
                //remove cache of self
                RemoveCaches(ec.Name);

                //remove cache of contained properties
                foreach (PropertyConfiguration pc in ec.Properties)
                {
                    if (pc.IsContained || pc.QueryType == "ManyToManyQuery")
                    {
                        //if (!string.IsNullOrEmpty(pc.RelatedType))
                        //{
                            RemoveCaches(pc.RelatedType);
                        //}
                        if (!string.IsNullOrEmpty(pc.RelationType))
                        {
                            RemoveCaches(pc.RelationType);
                        }
                    }
                }
            }

            List<EntityConfiguration> childList = MetaDataManager.GetChildEntityConfigurations(ec.Name);
            foreach (EntityConfiguration childEc in childList)
            {
                CascadeRemoveEntityCaches(childEc);
            }
        }

        internal void PreLoadEntities<EntityType>(EntityConfiguration ec)
            where EntityType : Entity, new()
        {
            if (ec.IsAutoPreLoad)
            {
                //preload all
                int cacheSeconds = GetTableCacheExpireSeconds(ec.ViewName);
                if (cacheSeconds == 0)
                {
                    cacheSeconds = int.MaxValue; //never expire by time
                }
                DataSet ds = dbHelper.Select(ec.ViewName, Entity.GetPropertyMappingColumnNames(ec), null, null, null);
                DataTable dt = (ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : null);
                AddCache(ComputeCacheKey(typeof(EntityType).ToString() + "|PreLoad", new WhereClip(string.Empty)), dt, cacheSeconds);
            }
        }

        private EntityType FindFromPreLoad<EntityType>(EntityConfiguration ec, WhereClip where)
            where EntityType : Entity, new()
        {
            string cacheKey = ComputeCacheKey(typeof(EntityType).ToString() + "|PreLoad", new WhereClip(string.Empty));
            if (!cache.Contains(cacheKey))
            {
                PreLoadEntities<EntityType>(ec);
            }
            DataTable dt = (DataTable)GetCache(cacheKey);
            if (dt == null)
            {
                PreLoadEntities<EntityType>(ec);
                dt = (DataTable)GetCache(cacheKey);
            }
            if (dt != null)
            {
                DataRow[] rows = dt.Select(FilterNTextPrefix(ParseExpressionByMetaData(ec, ToFlatWhereClip(where, ec).ToString())));
                if (rows != null && rows.Length > 0)
                {
                    EntityType retObj = CreateEntity<EntityType>();
                    retObj.SetPropertyValues(rows[0]);
                    return retObj;
                }
            }
            return null;
        }

        private EntityType[] FindArrayFromPreLoad<EntityType>(EntityConfiguration ec, WhereClip where, OrderByClip orderBy)
            where EntityType : Entity, new()
        {
            string cacheKey = ComputeCacheKey(typeof(EntityType).ToString() + "|PreLoad", new WhereClip(string.Empty));
            if (!cache.Contains(cacheKey))
            {
                PreLoadEntities<EntityType>(ec);
            }
            DataTable dt = (DataTable)GetCache(cacheKey);
            if (dt == null)
            {
                PreLoadEntities<EntityType>(ec);
                dt = (DataTable)GetCache(cacheKey);
            }
            if (dt != null)
            {
                DataRow[] rows;
                if (where == WhereClip.All && orderBy == OrderByClip.Default)
                {
                    rows = dt.Select();
                }
                else if (where == WhereClip.All)
                {
                    rows = dt.Select(null, ParseExpressionByMetaData(ec, orderBy.ToString()));
                }
                else if (orderBy == OrderByClip.Default)
                {
                    rows = dt.Select(FilterNTextPrefix(ParseExpressionByMetaData(ec, ToFlatWhereClip(where, ec).ToString())));
                }
                else
                {
                    rows = dt.Select(FilterNTextPrefix(ParseExpressionByMetaData(ec, ToFlatWhereClip(where, ec).ToString())), ParseExpressionByMetaData(ec, orderBy.ToString()));
                }
                if (rows != null && rows.Length > 0)
                {
                    List<EntityType> list = new List<EntityType>();
                    foreach (DataRow row in rows)
                    {
                        EntityType retObj = CreateEntity<EntityType>();
                        retObj.SetPropertyValues(row);
                        list.Add(retObj);
                    }
                    return list.ToArray();
                }
            }
            return new EntityType[0];
        }

        private object[] FindSinglePropertyArrayFromPreLoad<EntityType>(string column, EntityConfiguration ec, WhereClip where, OrderByClip orderBy)
            where EntityType : Entity, new()
        {
            DataTable dt = FindTableFromPreLoad<EntityType>(ec, where, orderBy);
            List<object> list = new List<object>();

            if (dt != null && dt.Rows.Count > 0)
            {
                column = column.TrimStart(db.DbProvider.LeftToken.ToCharArray()).TrimEnd(db.DbProvider.RightToken.ToCharArray());
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(row[column]);
                }
            }

            return list.ToArray();
        }

        private DataTable FindTableFromPreLoad<EntityType>(EntityConfiguration ec, WhereClip where, OrderByClip orderBy)
            where EntityType : Entity, new()
        {
            string cacheKey = ComputeCacheKey(typeof(EntityType).ToString() + "|PreLoad", new WhereClip(string.Empty));
            if (!cache.Contains(cacheKey))
            {
                PreLoadEntities<EntityType>(ec);
            }
            DataTable dt = (DataTable)GetCache(cacheKey);
            if (dt == null)
            {
                PreLoadEntities<EntityType>(ec);
                dt = (DataTable)GetCache(cacheKey);
            }
            if (dt != null)
            {
                DataRow[] rows;
                if (where == WhereClip.All && orderBy == OrderByClip.Default)
                {
                    rows = dt.Select();
                }
                else if (where == WhereClip.All)
                {
                    rows = dt.Select(null, ParseExpressionByMetaData(ec, orderBy.ToString()));
                }
                else if (orderBy == OrderByClip.Default)
                {
                    rows = dt.Select(FilterNTextPrefix(ParseExpressionByMetaData(ec, ToFlatWhereClip(where, ec).ToString())));
                }
                else
                {
                    rows = dt.Select(FilterNTextPrefix(ParseExpressionByMetaData(ec, ToFlatWhereClip(where, ec).ToString())), ParseExpressionByMetaData(ec, orderBy.ToString()));
                }
                if (rows != null && rows.Length > 0)
                {
                    dt.Rows.Clear();
                    foreach (DataRow row in rows)
                    {
                        dt.Rows.Add(row);
                    }
                    return dt;
                }
            }
            return null;
        }

        private object FindScalarFromPreLoad<EntityType>(string column, EntityConfiguration ec, WhereClip where)
            where EntityType : Entity, new()
        {
            string cacheKey = ComputeCacheKey(typeof(EntityType).ToString() + "|PreLoad", new WhereClip(string.Empty));
            if (!cache.Contains(cacheKey))
            {
                PreLoadEntities<EntityType>(ec);
            }
            DataTable dt = (DataTable)GetCache(cacheKey);
            if (dt == null)
            {
                PreLoadEntities<EntityType>(ec);
                dt = (DataTable)GetCache(cacheKey);
            }
            if (dt != null)
            {
                if (column.Contains("("))
                {
                    //aggregate query
                    if (column.StartsWith("COUNT(DISTINCT "))
                    {
                        string columName = column.Substring(15).Trim(' ', ')').Replace(db.DbProvider.LeftToken, string.Empty).Replace(db.DbProvider.RightToken, string.Empty);
                        List<object> list = new List<object>();
                        foreach (DataRow row in dt.Rows)
                        {
                            object columnValue = row[columName];
                            if (!list.Contains(columnValue))
                            {
                                list.Add(columnValue);
                            }
                        }
                        return list.Count;
                    }
                    else if (column.StartsWith("COUNT("))
                    {
                        return dt.Rows.Count;
                    }
                    else
                    {
                        return dt.Compute(column, where == WhereClip.All ? null : ToFlatWhereClip(where, ec).ToString());
                    }
                }
                else
                {
                    //scalar query
                    DataRow[] rows;
                    if (where == WhereClip.All)
                    {
                        rows = dt.Select();
                    }
                    else
                    {
                        rows = dt.Select(FilterNTextPrefix(ParseExpressionByMetaData(ec, ToFlatWhereClip(where, ec).ToString())));
                    }
                    if (rows != null && rows.Length > 0)
                    {
                        return rows[0][column.TrimStart(db.DbProvider.LeftToken.ToCharArray()).TrimEnd(db.DbProvider.RightToken.ToCharArray())];
                    }
                }
            }

            return 0;
        }

        #endregion

        #region Additional Entity Query Helper Methods

        /// <summary>
        /// Ins the sub query.
        /// </summary>
        /// <param name="leftProperty">The left property.</param>
        /// <param name="rightProperty">The right property.</param>
        /// <param name="where">The where condition.</param>
        /// <returns>The In Sub Query Where Clip.</returns>
        public WhereClip InSubQuery<EntityType>(PropertyItem leftProperty, PropertyItem rightProperty, WhereClip where)
            where EntityType : Entity, new()
        {
            Check.Require(!leftProperty.Equals(null), "leftProperty could not be null.");
            Check.Require(!rightProperty.Equals(null), "rightProperty could not be null.");
            Check.Require(where != null, "where could not be null.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();
            WhereClip flatWhere = ToFlatWhereClip(where, ec);
            return new WhereClip(string.Format("{0} IN (SELECT {1} FROM {2}{3})", leftProperty.ColumnName, BuildDbColumnName(ec.GetMappingColumnName(rightProperty.PropertyName)), BuildDbColumnName(ec.ViewName), (string.IsNullOrEmpty(flatWhere.ToString()) ? string.Empty : " WHERE " + flatWhere.ToString())));
        }

        /// <summary>
        /// To flat where clip.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="ec">The ec.</param>
        /// <returns></returns>
        public WhereClip ToFlatWhereClip(WhereClip where, EntityConfiguration ec)
        {
            Check.Require(ec != null, "ec could not be null.");

            if (where == null || where == WhereClip.All)
            {
                return where;
            }
            else
            {
                string whereStr = where.ToString();
                if (where.ParamValues != null)
                {
                    foreach (object p in where.ParamValues)
                    {
                        System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("(" + "@" + @"[\w\d_]+)");
                        if (p != null && p is string)
                        {
                            whereStr = r.Replace(whereStr, Util.FormatParamVal(p.ToString().Replace("@", "\007")), 1);
                        }
                        else
                        {
                            whereStr = r.Replace(whereStr, Util.FormatParamVal(p), 1);
                        }
                    }
                }
                whereStr = ParseExpressionByMetaData(ec, whereStr.Replace("\007", "@"));
                return new WhereClip(whereStr);
            }
        }

        #endregion
    }
}
