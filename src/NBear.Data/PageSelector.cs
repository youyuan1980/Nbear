using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using NBear.Common;

namespace NBear.Data
{
    /// <summary>
    /// The strong typed page selector.
    /// </summary>
    /// <typeparam name="EntityType">the type of the entity.</typeparam>
    public class PageSelector<EntityType>
        where EntityType : Entity, new()
    {
        private Gateway gateway;
        private IPageSplit ps;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageSelector&lt;EntityType&gt;"/> class.
        /// </summary>
        /// <param name="gateway">The gateway.</param>
        /// <param name="ps">The ps.</param>
        public PageSelector(Gateway gateway, IPageSplit ps)
        {
            Check.Require(gateway != null, "gateway could not be null.");
            Check.Require(ps != null, "ps could not be null.");

            this.gateway = gateway;
            this.ps = ps;
        }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize
        {
            get
            {
                return ps.PageSize;
            }
            set
            {
                ps.PageSize = value;
            }
        }

        /// <summary>
        /// Finds the page.
        /// </summary>
        /// <param name="pageNo">The page no.</param>
        /// <returns>The result.</returns>
        public EntityType[] FindPage(int pageNo)
        {
            Check.Require(pageNo > 0, "pageNo must > 0.");

            EntityConfiguration ec = new EntityType().GetEntityConfiguration();
            string cacheKey = null;

            if (gateway.IsCacheTurnedOn && ec.IsAutoPreLoad)
            {
                cacheKey = gateway.ComputeCacheKey(typeof(EntityType).ToString() + "|PreLoad", new WhereClip(string.Empty));
                if (!Gateway.cache.Contains(cacheKey))
                {
                    gateway.PreLoadEntities<EntityType>(ec);
                }
                DataTable dt = (DataTable)gateway.GetCache(cacheKey);
                if (dt == null)
                {
                    gateway.PreLoadEntities<EntityType>(ec);
                    dt = (DataTable)gateway.GetCache(cacheKey);
                }
                if (dt != null)
                {
                    DataRow[] rows;
                    if (string.IsNullOrEmpty(ps.Where) && string.IsNullOrEmpty(ps.OrderBy))
                    {
                        rows = dt.Select();
                    }
                    else if (string.IsNullOrEmpty(ps.Where))
                    {
                        rows = dt.Select(null, ps.OrderBy);
                    }
                    else if (string.IsNullOrEmpty(ps.OrderBy))
                    {
                        rows = dt.Select(gateway.ToFlatWhereClip(new WhereClip(ps.Where, ps.ParamValues), ec).ToString());
                    }
                    else
                    {
                        rows = dt.Select(gateway.ToFlatWhereClip(new WhereClip(ps.Where, ps.ParamValues), ec).ToString(), ps.OrderBy);
                    }

                    if (rows != null && rows.Length > 0)
                    {
                        List<DataRow> pageList = new List<DataRow>();
                        for (int i = (pageNo - 1) * PageSize; i < rows.Length && i < pageNo * PageSize; i++)
                        {
                            pageList.Add(rows[i]);
                        }
                        rows = pageList.ToArray();

                        List<EntityType> list = new List<EntityType>();
                        foreach (DataRow row in rows)
                        {
                            EntityType retObj = gateway.CreateEntity<EntityType>();
                            retObj.SetPropertyValues(row);
                            list.Add(retObj);
                        }
                        return list.ToArray();
                    }
                }
                return new EntityType[0];
            }

            if (gateway.IsCacheTurnedOn && gateway.GetTableCacheExpireSeconds(ec.ViewName) > 0)
            {
                PageSplit pageSplit = (PageSplit)ps;
                cacheKey = gateway.ComputeCacheKey(typeof(EntityType).ToString() + "|FindPage_" + pageNo.ToString(), new WhereClip(pageNo == 1 ? pageSplit._PageSplitableSelectStatementForFirstPage : pageSplit._PageSplitableSelectStatement, pageSplit.paramValues));
                object cachedObj = gateway.GetCache(cacheKey);
                if (cachedObj != null)
                {
                    return (EntityType[])cachedObj;
                }
            }

            IDataReader reader = ps.GetPageReadOnly(pageNo);
            List<EntityType> objs = new List<EntityType>();
            while (reader.Read())
            {
                EntityType obj = gateway.CreateEntity<EntityType>();
                obj.SetPropertyValues(reader);
                objs.Add(obj);
            }
            reader.Close();

            if (gateway.IsCacheTurnedOn && gateway.GetTableCacheExpireSeconds(ec.ViewName) > 0 && cacheKey != null)
            {
                gateway.AddCache(cacheKey, objs.ToArray(), gateway.GetTableCacheExpireSeconds(ec.ViewName));
            }

            return objs.ToArray();
        }

        /// <summary>
        /// Gets the page count.
        /// </summary>
        /// <value>The page count.</value>
        public int PageCount
        {
            get
            {
                return (int)Math.Ceiling(1.0 * RowCount / PageSize);
            }
        }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <value>The row count.</value>
        public int RowCount
        {
            get
            {
                EntityConfiguration ec = new EntityType().GetEntityConfiguration();
                string cacheKey = null;

                if (gateway.IsCacheTurnedOn && ec.IsAutoPreLoad)
                {
                    cacheKey = gateway.ComputeCacheKey(typeof(EntityType).ToString() + "|PreLoad", new WhereClip(string.Empty));
                    if (!Gateway.cache.Contains(cacheKey))
                    {
                        gateway.PreLoadEntities<EntityType>(ec);
                    }
                    DataTable dt = (DataTable)gateway.GetCache(cacheKey);
                    if (dt == null)
                    {
                        gateway.PreLoadEntities<EntityType>(ec);
                        dt = (DataTable)gateway.GetCache(cacheKey);
                    }
                    if (dt != null)
                    {
                        return dt.Rows.Count;
                    }
                    return 0;
                }

                if (gateway.IsCacheTurnedOn && gateway.GetTableCacheExpireSeconds(ec.ViewName) > 0)
                {
                    PageSplit pageSplit = (PageSplit)ps;
                    cacheKey = gateway.ComputeCacheKey(typeof(EntityType).ToString() + "|FindRowCount", new WhereClip(pageSplit._SelectCountStatement, pageSplit.paramValues));
                    object cachedObj = gateway.GetCache(cacheKey);
                    if (cachedObj != null)
                    {
                        return (int)cachedObj;
                    }
                }

                int retInt = ps.GetRowCount();

                if (gateway.IsCacheTurnedOn && gateway.GetTableCacheExpireSeconds(ec.ViewName) > 0 && cacheKey != null)
                {
                    gateway.AddCache(cacheKey, retInt, gateway.GetTableCacheExpireSeconds(ec.ViewName));
                }

                return retInt;
            }
        }
    }
}
