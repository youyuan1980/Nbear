using System;

namespace NBear.Common.Design
{
    /// <summary>
    /// Mark a property as one of the primary keys of the owned entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PrimaryKeyAttribute : Attribute
    {
    }

    /// <summary>
    /// Mark a property as a FriendKey property. You must specify the related entity type, which this friend key depending on.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FriendKeyAttribute : Attribute
    {
        private Type relatedEntityType;

        /// <summary>
        /// Gets the type of the related entity.
        /// </summary>
        /// <value>The type of the related entity.</value>
        public Type RelatedEntityType
        {
            get
            {
                return relatedEntityType;
            }
        }

        /// <summary>
        /// Gets the related entity pk.
        /// </summary>
        /// <value>The related entity pk.</value>
        public string RelatedEntityPk
        {
            get
            {
                return QueryDescriber.GetPkPropertyName(relatedEntityType);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FriendKeyAttribute"/> class.
        /// </summary>
        /// <param name="relatedEntityType">Type of the related entity.</param>
        public FriendKeyAttribute(Type relatedEntityType)
        {
            this.relatedEntityType = relatedEntityType;
        }
    }

    /// <summary>
    /// A draft entity will be ignored in code generating.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class DraftAttribute : Attribute
    {
    }

    /// <summary>
    /// Interfaces makred with this attribute will be generated into the implementing interface list of output entities.
    /// The interface specified here must be defined in Entities assembly or a shared assembly that Entities assembly referencing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class ImplementInterfaceAttribute : Attribute
    {
        private string interfaceFullName;

        /// <summary>
        /// Gets the full name of the interface.
        /// </summary>
        /// <value>The full name of the interface.</value>
        public string InterfaceFullName
        {
            get
            {
                return interfaceFullName;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplementInterfaceAttribute"/> class.
        /// </summary>
        /// <param name="interfaceFullName">Full name of the interface.</param>
        public ImplementInterfaceAttribute(string interfaceFullName)
        {
            this.interfaceFullName = interfaceFullName;
        }
    }

    /// <summary>
    /// Mark a property as needed to add index when creating the table in database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IndexPropertyAttribute : Attribute
    {
        private bool isDesc = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexPropertyAttribute"/> class.
        /// </summary>
        public IndexPropertyAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexPropertyAttribute"/> class.
        /// </summary>
        /// <param name="isDesc">if set to <c>true</c> [is desc].</param>
        public IndexPropertyAttribute(bool isDesc)
        {
            this.isDesc = isDesc;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is desc.
        /// </summary>
        /// <value><c>true</c> if this instance is desc; otherwise, <c>false</c>.</value>
        public bool IsDesc
        {
            get { return isDesc; }
        }
    }

    /// <summary>
    /// Whether the mapping column in database could not be NULL.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotNullAttribute : Attribute
    {
    }

    /// <summary>
    /// Whether a property should not included in default XML serialization. This attribute maps to a XmlIgnore attribute in actual generated entity code by EntityDesignToEntity.exe tool.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SerializationIgnoreAttribute : Attribute
    {
    }

    /// <summary>
    /// Mark an entity as readonly, which means it can be used in finding entities and cannot be updated. Generally, a readonly entity maps to a database view.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class ReadOnlyAttribute : Attribute
    {
    }

    /// <summary>
    /// Mark an entity as a relation entity, which is used to realize many to many relation mapping.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class RelationAttribute : Attribute
    {
    }

    /// <summary>
    /// Additional sql script clip which will be included into the sql script batch.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class AdditionalSqlScriptAttribute : Attribute
    {
        private string sql;
        private string preCleanSql = null;

        public AdditionalSqlScriptAttribute(string sql)
        {
            this.sql = sql;
        }

        /// <summary>
        /// Gets the SQL.
        /// </summary>
        /// <value>The SQL.</value>
        public string Sql
        {
            get
            {
                return sql;
            }
        }

        /// <summary>
        /// Gets or sets the pre clean SQL.
        /// </summary>
        /// <value>The pre clean SQL.</value>
        public string PreCleanSql
        {
            get
            {
                return preCleanSql;
            }
            set
            {
                preCleanSql = value;
            }
        }
    }

    /// <summary>
    /// Mark an entity as needed to save all related base entity and property entity values in a batch gateway to improve performance.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class BatchUpdateAttribute : Attribute
    {
        private int batchSize;

        /// <summary>
        /// Gets the size of the batch save.
        /// </summary>
        /// <value>The size of the batch save.</value>
        public int BatchSize
        {
            get
            {
                return batchSize;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchUpdateAttribute"/> class.
        /// </summary>
        /// <param name="batchSize">Size of the batch save.</param>
        public BatchUpdateAttribute(int batchSize)
        {
            this.batchSize = batchSize;
        }
    }

    /// <summary>
    /// Whether instances of the entity are automatically preloaded.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class AutoPreLoadAttribute : Attribute
    {
    }

    /// <summary>
    /// Mark a property of a relation entity as a relation key, which is used to relate an entity's primary key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RelationKeyAttribute : Attribute
    {
        private Type relatedType;

        /// <summary>
        /// Gets the type of the related entity.
        /// </summary>
        /// <value>The type of the related.</value>
        public Type RelatedType
        {
            get
            {
                return relatedType;
            }
        }

        /// <summary>
        /// Gets the related pk.
        /// </summary>
        /// <value>The pk.</value>
        public string RelatedPk
        {
            get
            {
                return QueryDescriber.GetPkPropertyName(relatedType);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationKeyAttribute"/> class.
        /// </summary>
        /// <param name="relatedType">Type of the related.</param>
        public RelationKeyAttribute(Type relatedType)
        {
            this.relatedType = relatedType;
        }
    }

    /// <summary>
    /// Mark a property as a CompoundUnit property.
    /// </summary>
    /// <remarks>
    /// A compound unit can be a serializable struct or class. It can contains properties which is mapping to an single data column. A classic compound unit example is UserName: FirstName, LastName.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CompoundUnitAttribute : Attribute
    {
    }

    /// <summary>
    /// Specify the actual mapping name  of an entity to a table/view/procedure or a property to a data column. 
    /// By default, entity/properties with same names map to table/view/procedure/data columns with same name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class MappingNameAttribute : Attribute
    {
        private string name;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public MappingNameAttribute(string name)
        {
            this.name = name;
        }
    }

    /// <summary>
    /// Comment content set with this attribute will be generated into the generated Entities code.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class CommentAttribute : Attribute
    {
        private string content;

        /// <summary>
        /// Gets the comment content.
        /// </summary>
        /// <value>The content.</value>
        public string Content
        {
            get
            {
                return content;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CommentAttribute(string content)
        {
            this.content = content;
        }
    }

    /// <summary>
    /// Specify the output namespace of this entity
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class OutputNamespaceAttribute : Attribute
    {
        private string ns;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Namespace
        {
            get
            {
                return ns;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputNamespaceAttribute"/> class.
        /// </summary>
        /// <param name="ns">The namespace.</param>
        public OutputNamespaceAttribute(string ns)
        {
            this.ns = ns;
        }
    }

    /// <summary>
    /// Custom data of entity or property, by which you can take advantage of for custom use. 
    /// When you set some custom data for an entity or property, you can get these data at runtime 
    /// by calling NBear.Common.MetaDataManager.GetEntityConfiguration() and EntityConfiguration.GetPropertyConfiguration().
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class CustomDataAttribute : Attribute
    {
        private string data;

        /// <summary>
        /// Gets the custom data.
        /// </summary>
        /// <value>The name.</value>
        public string Data
        {
            get
            {
                return data;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDataAttribute"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public CustomDataAttribute(string data)
        {
            this.data = data;
        }
    }

    /// <summary>
    /// Specify the database column's sql type mapping to the property. If there is no SqlType specified for a property, NBear will generate a default according to it's .Net type.
    /// By default, value types maps to relevant value types in database, and string type maps to database type - nvarchar(127).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SqlTypeAttribute : Attribute
    {
        private string type;
        private string defaultValue;

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type
        {
            get
            {
                return type;
            }
        }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public string DefaultValue
        {
            get
            {
                return defaultValue;
            }
            set
            {
                defaultValue = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlTypeAttribute"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public SqlTypeAttribute(string type)
        {
            this.type = type;
        }
    }
}
