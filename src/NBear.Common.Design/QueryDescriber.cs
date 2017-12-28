using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace NBear.Common.Design
{
    /// <summary>
    /// Used by NBear self to describe the details of a QueryAttribute.
    /// </summary>
    public class QueryDescriber
    {
        #region Private Members

        private QueryAttribute qa;
        private string propertyName;
        private string relatedEntityPropertyName;
        private Type relatedEntityType;
        private string relatedEntityPk;
        private Type relatedEntityPkType;
        private PropertyInfo relatedEntityPkPropertyInfo;
        //private string entityName;
        //private Type entityType;
        private string entityPk;
        //private Dictionary<string, string> relationKeysMap;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the where.
        /// </summary>
        /// <value>The where.</value>
        public string Where
        {
            get
            {
                switch (qa.QueryType)
                {
                    case QueryType.PkQuery:
                        return string.Format("[{0}] = @{1}", relatedEntityPk, entityPk).Replace("[", "{").Replace("]", "}");
                    case QueryType.PkReverseQuery:
                        return string.Format("[{0}] = @{1}", relatedEntityPk, entityPk).Replace("[", "{").Replace("]", "}");
                    case QueryType.FkQuery:
                        string additionalWhere = ((FkQueryAttribute)qa).AdditionalWhere;
                        return string.Format("[{0}] = @{1}{2}", relatedEntityPropertyName, entityPk, string.IsNullOrEmpty(additionalWhere) ? string.Empty : " AND ( " + additionalWhere + ")").Replace("[", "{").Replace("]", "}");
                    case QueryType.FkReverseQuery:
                        return string.Format("[{0}] = @{1}", relatedEntityPk, propertyName).Replace("[", "{").Replace("]", "}");
                    case QueryType.ManyToManyQuery:
                        return ((ManyToManyQueryAttribute)qa).AdditionalWhere;
                    case QueryType.CustomQuery:
                        return ((CustomQueryAttribute)qa).Where;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the order by.
        /// </summary>
        /// <value>The order by.</value>
        public string OrderBy
        {
            get
            {
                switch (qa.QueryType)
                {
                    case QueryType.CustomQuery:
                        return ((CustomQueryAttribute)qa).OrderBy;
                    case QueryType.FkQuery:
                        return ((FkQueryAttribute)qa).OrderBy;
                    case QueryType.ManyToManyQuery:
                        return ((ManyToManyQueryAttribute)qa).OrderBy;
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Gets the type of the relation.
        /// </summary>
        /// <value>The type of the relation.</value>
        public Type RelationType
        {
            get
            {
                switch (qa.QueryType)
                {
                    case QueryType.CustomQuery:
                        return ((CustomQueryAttribute)qa).RelationType;
                    case QueryType.ManyToManyQuery:
                        return ((ManyToManyQueryAttribute)qa).RelationType;
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="QueryDescriber"/> is contained in cascade update.
        /// </summary>
        /// <value><c>true</c> if contained; otherwise, <c>false</c>.</value>
        public bool Contained
        {
            get
            {
                if (qa.QueryType == QueryType.PkQuery)
                {
                    return true;
                }
                else
                {
                    switch (qa.QueryType)
                    {
                        case QueryType.FkQuery:
                            return ((FkQueryAttribute)qa).Contained;
                        case QueryType.ManyToManyQuery:
                            return ((ManyToManyQueryAttribute)qa).Contained;
                        case QueryType.CustomQuery:
                            return ((CustomQueryAttribute)qa).Contained;
                        default:
                            return false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the related foreign key.
        /// </summary>
        /// <value>The related foreign key.</value>
        public string RelatedForeignKey
        {
            get
            {
                switch (qa.QueryType)
                {
                    case QueryType.ManyToManyQuery:
                        return relatedEntityPk;
                    case QueryType.CustomQuery:
                        if (((CustomQueryAttribute)qa).RelationType != null)
                        {
                            return relatedEntityPk;
                        }
                        else
                        {
                            return relatedEntityPropertyName;
                        }
                    default:
                        return relatedEntityPropertyName ?? relatedEntityPk;
                }
            }
        }

        /// <summary>
        /// Gets the type of the related foreign key.
        /// </summary>
        /// <value>The type of the related foreign key.</value>
        public Type RelatedForeignKeyType
        {
            get
            {
                return relatedEntityPkType;
            }
        }

        /// <summary>
        /// Gets the related foreign key property info.
        /// </summary>
        /// <value>The related foreign key property info.</value>
        public PropertyInfo RelatedForeignKeyPropertyInfo
        {
            get
            {
                return relatedEntityPkPropertyInfo;
            }
        }

        /// <summary>
        /// Gets the type of the related entity.
        /// </summary>
        /// <value>The type of the related.</value>
        public Type RelatedType
        {
            get
            {
                return relatedEntityType;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the property is lazyload.
        /// </summary>
        /// <value><c>true</c> if lazyload; otherwise, <c>false</c>.</value>
        public bool LazyLoad
        {
            get
            {
                return qa.LazyLoad;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Gets the name of the pk property or specified entity type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The pk name.</returns>
        internal static string GetPkPropertyName(Type type)
        {
            string retName = null;

            foreach (PropertyInfo item in type.GetProperties())
            {
                if (item.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).Length > 0)
                {
                    retName = item.Name;
                    break;
                }
            }

            if (retName == null)
            {
                //check base entities
                foreach (Type baseType in type.GetInterfaces())
                {
                    foreach (PropertyInfo item in baseType.GetProperties())
                    {
                        if (item.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).Length > 0)
                        {
                            retName = item.Name;
                            break;
                        }
                    }
                    if (retName != null)
                    {
                        break;
                    }
                }
            }

            return retName;
        }

        /// <summary>
        /// Gets the pk property info of the specified entity type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The property info.</returns>
        internal static PropertyInfo GetPkPropertyInfo(Type type)
        {
            PropertyInfo retPi = null;

            foreach (PropertyInfo item in type.GetProperties())
            {
                if (item.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).Length > 0)
                {
                    retPi = item;
                    break;
                }
            }

            if (retPi == null)
            {
                //check base entities
                foreach (Type baseType in type.GetInterfaces())
                {
                    foreach (PropertyInfo item in baseType.GetProperties())
                    {
                        if (item.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).Length > 0)
                        {
                            retPi = item;
                            break;
                        }
                    }
                    if (retPi != null)
                    {
                        break;
                    }
                }
            }

            return retPi;
        }

        /// <summary>
        /// Gets the type of the pk property.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type.</returns>
        internal static Type GetPkPropertyType(Type type)
        {
            Type retType = null;

            foreach (PropertyInfo item in type.GetProperties())
            {
                if (item.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).Length > 0)
                {
                    retType = item.PropertyType;
                    break;
                }
            }

            if (retType == null)
            {
                //check base entities
                foreach (Type baseType in type.GetInterfaces())
                {
                    foreach (PropertyInfo item in baseType.GetProperties())
                    {
                        if (item.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).Length > 0)
                        {
                            retType = item.PropertyType;
                            break;
                        }
                    }
                    if (retType != null)
                    {
                        break;
                    }
                }
            }

            return retType;
        }

        private string GetRelatedEntityPropertyName(QueryAttribute qa)
        {
            switch (qa.QueryType)
            {
                case QueryType.FkQuery:
                    return ((FkQueryAttribute)qa).RelatedManyToOneQueryPropertyName;
                default:
                    return null;
            }
        }

        private Dictionary<string, string> GetRelationKeysMap(QueryAttribute qa)
        {
            switch (qa.QueryType)
            {
                case QueryType.ManyToManyQuery:
                    return GetRelationKeysMapFromRelationType(((ManyToManyQueryAttribute)qa).RelationType);
                case QueryType.CustomQuery:
                    if (((CustomQueryAttribute)qa).RelationType != null)
                    {
                        return GetRelationKeysMapFromRelationType(((CustomQueryAttribute)qa).RelationType);
                    }
                    else
                    {
                        return null;
                    }
                default:
                    return null;
            }
        }

        private Dictionary<string, string> GetRelationKeysMapFromRelationType(Type type)
        {
            Dictionary<string, string> retMap = new Dictionary<string, string>();

            foreach (PropertyInfo item in type.GetProperties())
            {
                object[] attrs = item.GetCustomAttributes(typeof(RelationKeyAttribute), true);
                if (attrs.Length > 0)
                {
                    RelationKeyAttribute rka = (RelationKeyAttribute)attrs[0];
                    try
                    {
                        retMap.Add(rka.RelatedType.Name, item.Name);
                    }
                    catch
                    {
                        throw new NotSupportedException("Many to many relaion only supports single primary key entities.");
                    }
                }
            }

            if (retMap.Count != 2)
            {
                throw new NotSupportedException("Relation entity could and must exactly contain two related key properties.");
            }

            return retMap;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDescriber"/> class.
        /// </summary>
        /// <param name="qa">The query attribute instance.</param>
        /// <param name="pi">The property's property info.</param>
        /// <param name="propertyEntityType">The entity type that the property returns.</param>
        /// <param name="entityType">The entity type.</param>
        public QueryDescriber(QueryAttribute qa, PropertyInfo pi, Type propertyEntityType, Type entityType)
        {
            this.qa = qa;

            //parse info from pi
            propertyName = pi.Name;
            relatedEntityPropertyName = GetRelatedEntityPropertyName(qa);
            //entityName = pi.ReflectedType.Name;
            //entityType = pi.ReflectedType;
            entityPk = GetPkPropertyName(entityType);
            relatedEntityType = propertyEntityType;
            relatedEntityPk = GetPkPropertyName(propertyEntityType);
            relatedEntityPkPropertyInfo = GetPkPropertyInfo(propertyEntityType);
            //relationKeysMap = GetRelationKeysMap(qa);
            relatedEntityPkType = GetPkPropertyType(propertyEntityType);
        }

        #endregion
    }
}
