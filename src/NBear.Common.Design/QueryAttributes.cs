using System;
using System.Collections.Generic;
using System.Text;

namespace NBear.Common.Design
{
    /// <summary>
    /// Types of query attributes
    /// </summary>
    public enum QueryType
    {
        /// <summary>
        /// PkQuery
        /// </summary>
        PkQuery,
        /// <summary>
        /// PkReverseQuery
        /// </summary>
        PkReverseQuery,
        /// <summary>
        /// FkQuery
        /// </summary>
        FkQuery,
        /// <summary>
        /// FkReverseQuery
        /// </summary>
        FkReverseQuery,
        /// <summary>
        /// ManyToManyQuery
        /// </summary>
        ManyToManyQuery,
        /// <summary>
        /// CustomQuery
        /// </summary>
        CustomQuery,
    }

    /// <summary>
    /// Base class of all query attributes.
    /// </summary>
    public abstract class QueryAttribute : Attribute
    {
        #region Private Memebrs

        private bool lazyLoad = true;
        private QueryType queryType;

        #endregion

        #region Protected Members

        /// <summary>
        /// whether the related property values is contained in owner entity's cascade update.
        /// </summary>
        protected bool contained = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the property is lazyload.
        /// </summary>
        /// <value><c>true</c> if lazyload; otherwise, <c>false</c>.</value>
        public bool LazyLoad
        {
            get { return lazyLoad; }
            set { lazyLoad = true; }
        }

        /// <summary>
        /// Gets the type of the query.
        /// </summary>
        /// <value>The type of the query.</value>
        public QueryType QueryType
        {
            get { return queryType; }
        }

        /// <summary>
        /// Gets a value indicating whether this property is contained in cascade update.
        /// </summary>
        /// <value><c>true</c> if contained; otherwise, <c>false</c>.</value>
        public bool Contained
        {
            get
            {
                return contained;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryAttribute"/> class.
        /// </summary>
        /// <param name="queryType">Type of the query.</param>
        public QueryAttribute(QueryType queryType)
        {
            this.queryType = queryType;
        }

        #endregion
    }

    /// <summary>
    /// Mark a property as a primary key one to one related property.
    /// </summary>
    public class PkQueryAttribute : QueryAttribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PkQueryAttribute"/> class.
        /// </summary>
        public PkQueryAttribute()
            : base(QueryType.PkQuery)
        {
            base.contained = true;
        }

        #endregion
    }

    /// <summary>
    /// Mark a property as a friend key one to one / one to many related property.
    /// </summary>
    public class FkQueryAttribute : QueryAttribute
    {
        #region Private Members

        private string where;
        private string relatedManyToOneQueryPropertyName;
        private string orderBy;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the related property which is the friend key of the related type or a property marked by FkReverseQuery.
        /// </summary>
        /// <value>The name of the related many to one query property.</value>
        public string RelatedManyToOneQueryPropertyName
        {
            get
            {
                return relatedManyToOneQueryPropertyName;
            }
        }

        /// <summary>
        /// Gets or sets the order by condition, when is used when it is a onte to many related property.
        /// </summary>
        /// <value>The order by.</value>
        public string OrderBy
        {
            get { return orderBy; }
            set { orderBy = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this property values is contained in owner entity's cascade update.
        /// </summary>
        /// <value><c>true</c> if contained; otherwise, <c>false</c>.</value>
        public new bool Contained
        {
            get { return base.contained; }
            set { base.contained = value; }
        }

        /// <summary>
        /// Gets or sets the additional where.
        /// </summary>
        /// <value>The additional where.</value>
        public string AdditionalWhere
        {
            get { return where; }
            set { where = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FkQueryAttribute"/> class.
        /// </summary>
        /// <param name="relatedManyToOneQueryPropertyName">The name of the related property which is the friend key of the related type or a property marked by FkReverseQuery</param>
        public FkQueryAttribute(string relatedManyToOneQueryPropertyName)
            : base(QueryType.FkQuery)
        {
            this.relatedManyToOneQueryPropertyName = relatedManyToOneQueryPropertyName;
        }

        #endregion
    }

    /// <summary>
    /// Mark a property as a custom query property. A custom quey can use custom query creterias to query one to one or one to many related entities.
    /// </summary>
    public class CustomQueryAttribute : QueryAttribute
    {
        #region Private Members

        private string where;
        private string orderBy;
        private Type relationType;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the where creteria.
        /// </summary>
        /// <value>The where.</value>
        public string Where
        {
            get { return where; }
        }

        /// <summary>
        /// Gets or sets the order by.
        /// </summary>
        /// <value>The order by.</value>
        public string OrderBy
        {
            get { return orderBy; }
            set { orderBy = value; }
        }

        /// <summary>
        /// Gets or sets the type of the relation entity.
        /// </summary>
        /// <value>The type of the relation.</value>
        public Type RelationType
        {
            get { return relationType; }
            set { relationType = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomQueryAttribute"/> class.
        /// </summary>
        /// <param name="where">The where.</param>
        public CustomQueryAttribute(string where)
            : base(QueryType.CustomQuery)
        {
            this.where = where;
        }

        #endregion
    }

    /// <summary>
    /// Mark a property as a Reverse PkQuery property.
    /// </summary>
    public class PkReverseQueryAttribute : QueryAttribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PkReverseQueryAttribute"/> class.
        /// </summary>
        public PkReverseQueryAttribute()
            : base(QueryType.PkReverseQuery)
        {
        }

        #endregion
    }

    /// <summary>
    /// Mark a property as a Reverse FkQuery property.
    /// </summary>
    public class FkReverseQueryAttribute : QueryAttribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FkReverseQueryAttribute"/> class.
        /// </summary>
        public FkReverseQueryAttribute()
            : base(QueryType.FkReverseQuery)
        {
            base.contained = true;
        }

        #endregion
    }

    /// <summary>
    /// Mark a property as a many to many related property.
    /// </summary>
    public class ManyToManyQueryAttribute : QueryAttribute
    {
        #region Private Members

        private string where;
        private Type relationType;
        private string orderBy;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this property is contained in owner entity's cascade update.
        /// </summary>
        /// <value><c>true</c> if contained; otherwise, <c>false</c>.</value>
        public new bool Contained
        {
            get { return base.contained; }
            set { base.contained = value; }
        }

        /// <summary>
        /// Gets the type of the relation entity.
        /// </summary>
        /// <value>The type of the relation.</value>
        public Type RelationType
        {
            get
            {
                return relationType;
            }
        }

        /// <summary>
        /// Gets or sets the order by.
        /// </summary>
        /// <value>The order by.</value>
        public string OrderBy
        {
            get { return orderBy; }
            set { orderBy = value; }
        }

        /// <summary>
        /// Gets or sets the additional where.
        /// </summary>
        /// <value>The additional where.</value>
        public string AdditionalWhere
        {
            get { return where; }
            set { where = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ManyToManyQueryAttribute"/> class.
        /// </summary>
        /// <param name="relationType">Type of the relation entity.</param>
        public ManyToManyQueryAttribute(Type relationType)
            : base(QueryType.ManyToManyQuery)
        {
            if (relationType.GetCustomAttributes(typeof(RelationAttribute), true).Length == 0)
            {
                throw new NotSupportedException("A entity type must be a relation entity type, if you use it as ManyToMany attribute's relation type parameter.");
            }

            this.relationType = relationType;
        }

        #endregion
    }
}
