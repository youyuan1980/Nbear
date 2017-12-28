using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;

using NBear.Common.Types;

namespace NBear.Common
{
    /// <summary>
    /// The entity meta data manager.
    /// </summary>
    public sealed class MetaDataManager
    {
        private static List<EntityConfiguration> entities = new List<EntityConfiguration>();
        private static Dictionary<string, List<EntityConfiguration>> childEntitiesMap = new Dictionary<string, List<EntityConfiguration>>();
        private static Dictionary<string, List<string>> isLazyLoadMap = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> byteArrayColumns = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> nullableNumberColumns = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> sqlTypeWithDefaultValueColumns = new Dictionary<string, List<string>>();
        private static List<string> nonRelatedEntities = new List<string>();
        private static Dictionary<string, string> autoIdEntities = new Dictionary<string, string>();

        /// <summary>
        /// Initializes the <see cref="MetaDataManager"/> class.
        /// </summary>
        static MetaDataManager()
        {
            EntityConfigurationSection section = ConfigurationManager.GetSection("entityConfig") as EntityConfigurationSection;

            if (section != null)
            {
                foreach (KeyValueConfigurationElement item in section.Includes)
                {
                    string itemValue = item.Value;
                    if (itemValue != null)
                    {
                        //replace "~/" or "~\" or "./" or ".\" prefix with base directory path
                        if (itemValue.StartsWith("~/") || itemValue.StartsWith("~\\"))
                        {
                            itemValue = itemValue.Replace("/", "\\").Replace("~\\", AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\");
                        }
                        else if (itemValue.StartsWith("./") || itemValue.StartsWith(".\\"))
                        {
                            itemValue = itemValue.Replace("/", "\\").Replace(".\\", AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\");
                        }
                    }

                    XmlTextReader reader = null;
                    StringReader strReader = null;
                    try
                    {
                        if (section.Encrpyt)
                        {
                            StreamReader sr = new StreamReader(itemValue);
                            string configContent = sr.ReadToEnd();
                            sr.Close();
                            strReader = new StringReader(new CryptographyManager().SymmetricDecrpyt(configContent, System.Security.Cryptography.Rijndael.Create(), section.Key));
                        }
                        else
                        {
                            reader = new XmlTextReader(itemValue);
                        }

                        XmlSerializer serializer = new XmlSerializer(typeof(EntityConfiguration[]));
                        EntityConfiguration[] objs;

                        if (reader != null)
                        {
                            objs = serializer.Deserialize(reader) as EntityConfiguration[];
                        }
                        else
                        {
                            objs = serializer.Deserialize(strReader) as EntityConfiguration[];
                        }

                        AddEntityConfigurations(objs);
                    }
                    catch (Exception ex)
                    {
                        throw new CouldNotLoadEntityConfigurationException(ex);
                    }
                    finally
                    {
                        if (reader != null)
                        {
                            reader.Close();
                        }
                    }
                }

                ParseNonRelatedEntities();
            }
        }

        public static void AddEntityConfigurations(EntityConfiguration[] objs)
        {
            if (objs != null)
            {
                foreach (EntityConfiguration obj in objs)
                {
                    entities.Add(obj);

                    if (obj.BaseEntity != null)
                    {
                        if (!childEntitiesMap.ContainsKey(obj.BaseEntity))
                        {
                            childEntitiesMap.Add(obj.BaseEntity, new List<EntityConfiguration>());
                        }

                        childEntitiesMap[obj.BaseEntity].Add(obj);
                    }

                    byteArrayColumns.Add(obj.Name, new List<string>());
                    nullableNumberColumns.Add(obj.Name, new List<string>());
                    sqlTypeWithDefaultValueColumns.Add(obj.Name, new List<string>());

                    List<string> lazyLoadProperties = new List<string>();
                    foreach (PropertyConfiguration pc in obj.Properties)
                    {
                        if (pc.IsQueryProperty && pc.IsLazyLoad)
                        {
                            lazyLoadProperties.Add(pc.Name);
                        }

                        if (pc.PropertyMappingColumnType == typeof(byte[]).ToString())
                        {
                            byteArrayColumns[obj.Name].Add(pc.MappingName);
                        }

                        if (pc.PropertyMappingColumnType == typeof(int?).ToString() || pc.PropertyType == typeof(long?).ToString() || pc.PropertyType == typeof(short?).ToString() || pc.PropertyType == typeof(byte?).ToString() || pc.PropertyType == typeof(bool?).ToString() || pc.PropertyType == typeof(decimal?).ToString() || pc.PropertyType == typeof(float?).ToString() || pc.PropertyType == typeof(double?).ToString())
                        {
                            nullableNumberColumns[obj.Name].Add(pc.MappingName);
                        }

                        if (pc.SqlDefaultValue != null)
                        {
                            sqlTypeWithDefaultValueColumns[obj.Name].Add(pc.MappingName);
                        }
                    }
                    isLazyLoadMap.Add(obj.Name, lazyLoadProperties);

                    foreach (PropertyConfiguration pc in obj.Properties)
                    {
                        if (pc.IsReadOnly && pc.IsPrimaryKey)
                        {
                            autoIdEntities.Add(obj.Name, pc.Name);
                            break;
                        }
                    }
                }
            }
        }

        public static void ParseNonRelatedEntities()
        {
            foreach (EntityConfiguration ec in entities)
            {
                if (!nonRelatedEntities.Contains(ec.Name))
                {
                    bool isNonRelatedEntity = true;

                    if (ec.BaseEntity == null && GetChildEntityConfigurations(ec.Name).Count == 0)
                    {
                        foreach (PropertyConfiguration pc in ec.Properties)
                        {
                            if (pc.IsQueryProperty && (pc.IsContained || pc.QueryType == "ManyToManyQuery"))
                            {
                                isNonRelatedEntity = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        isNonRelatedEntity = false;
                    }

                    if (isNonRelatedEntity)
                    {
                        nonRelatedEntities.Add(ec.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the entity configuration.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>The entity configuration</returns>
        public static EntityConfiguration GetEntityConfiguration(string typeName)
        {
            if (entities != null)
            {
                foreach (EntityConfiguration item in entities)
                {
                    if (item.Name == typeName)
                    {
                        return item;
                    }
                }
            }

            throw new CouldNotFoundEntityConfigurationOfEntityException(typeName);
        }

        /// <summary>
        /// Gets the child entity configurations.
        /// </summary>
        /// <param name="baseTypeName">Name of the base type.</param>
        /// <returns>The entity configurations.</returns>
        public static List<EntityConfiguration> GetChildEntityConfigurations(string baseTypeName)
        {
            Check.Require(baseTypeName != null, "baseTypeName could not be null.");

            return childEntitiesMap.ContainsKey(baseTypeName) ? childEntitiesMap[baseTypeName] : new List<EntityConfiguration>();
        }

        /// <summary>
        /// Determines whether a specified property of an entity is lazyload.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// 	<c>true</c> if is lazy load ; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLazyLoad(string entityName, string propertyName)
        {
            return isLazyLoadMap[entityName].Contains(propertyName);
        }

        /// <summary>
        /// Determines whether the specified entity is non related entity, a non related entity is an entity without base/child entities and related contained query properties.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <returns>Wthether true.</returns>
        public static bool IsNonRelatedEntity(string entityName)
        {
            return nonRelatedEntities.Contains(entityName);
        }

        /// <summary>
        /// Gets name of the entity's auto id column.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <returns>The auto id column name.</returns>
        public static string GetEntityAutoId(string entityName)
        {
            if (autoIdEntities.ContainsKey(entityName))
            {
                return autoIdEntities[entityName];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the byte array type columns.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <returns>The columns.</returns>
        public static List<string> GetByteArrayColumns(string entityName)
        {
            return byteArrayColumns[entityName];
        }

        /// <summary>
        /// Gets the nullable number columns.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <returns></returns>
        public static List<string> GetNullableNumberColumns(string entityName)
        {
            return nullableNumberColumns[entityName];
        }

        /// <summary>
        /// Gets the sqltype with default value columns.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <returns></returns>
        public static List<string> GetSqlTypeWithDefaultValueColumns(string entityName)
        {
            return sqlTypeWithDefaultValueColumns[entityName];
        }
    }

    #region Meta data configuration

    /// <summary>
    /// The entity configuration section.
    /// </summary>
    public class EntityConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Whether encrpyted entity config file.
        /// </summary>
        [ConfigurationProperty("encrpyt")]
        public bool Encrpyt
        {
            get { return this["encrpyt"] == null ? false : (bool)this["encrpyt"]; }
            set { this["encrpyt"] = value; }
        }

        /// <summary>
        /// The encrpyt/decrypt key.
        /// </summary>
        [ConfigurationProperty("key")]
        public string Key
        {
            get { return this["key"] == null ? CryptographyManager.DEFAULT_KEY : (string)this["key"]; }
            set { this["key"] = value; }
        }

        /// <summary>
        /// Gets or sets the includes.
        /// </summary>
        /// <value>The includes.</value>
        [ConfigurationProperty("includes", IsRequired=true,IsDefaultCollection=false)]
        [ConfigurationCollection(typeof(KeyValueConfigurationCollection))]
        public KeyValueConfigurationCollection Includes
        {
            get
            {
                return (KeyValueConfigurationCollection)this["includes"];
            }
            set
            {
                this["includes"] = value;
            }
        }
    }

    /// <summary>
    /// An entity configuration
    /// </summary>
    [Serializable]
    public class EntityConfiguration
    {
        private string RemoveTypePrefix(string typeName)
        {
            string name = typeName;
            while (name.Contains("."))
            {
                name = name.Substring(name.IndexOf(".")).TrimStart('.');
            }
            return name;
        }

        /// <summary>
        /// Name of entity.
        /// </summary>
        [XmlAttribute("name")]
        public string Name;
        private string mappingName;
        private int batchSize;

        /// <summary>
        /// Gets or sets the name of the mapping.
        /// </summary>
        /// <value>The name of the mapping.</value>
        [XmlAttribute("mappingName")]
        public string MappingName
        {
            get
            {
                return mappingName ?? RemoveTypePrefix(Name);
            }
            set
            {
                mappingName = value;
            }
        }

        /// <summary>
        /// Commnet of entity
        /// </summary>
        [XmlAttribute("comment")]
        public string Commnet;

        /// <summary>
        /// Gets the name of the view to select data.
        /// </summary>
        /// <value>The name of the view.</value>
        public string ViewName
        {
            get
            {
                return (BaseEntity == null ? MappingName : "v" + MappingName);
            }
        }

        /// <summary>
        /// Whether the entity is readonly.
        /// </summary>
        [XmlAttribute("isReadOnly")]
        public bool IsReadOnly;
        /// <summary>
        /// Whether instances of the entity are automatically preloaded.
        /// </summary>
        [XmlAttribute("isAutoPreLoad")]
        public bool IsAutoPreLoad;
        /// <summary>
        /// Whether the entity is save all property related values in a batch to improve performance.
        /// </summary>
        [XmlAttribute("isBatchUpdate")]
        public bool IsBatchUpdate;
        /// <summary>
        /// The batch size when an entity is marked as IsBatchUpdate.
        /// </summary>
        [XmlAttribute("batchSize")]
        public int BatchSize
        {
            get
            {
                if (batchSize <= 0)
                {
                    batchSize = 10;
                }
                return batchSize;
            }
            set
            {
                batchSize = value;
            }
        }
        /// <summary>
        /// Whether the entity is a relation type.
        /// </summary>
        [XmlAttribute("isRelation")]
        public bool IsRelation;
        /// <summary>
        /// Base entity of this entity.
        /// </summary>
        [XmlAttribute("baseType")]
        public string BaseEntity;

        /// <summary>
        /// Custom data.
        /// </summary>
        [XmlAttribute("customData")]
        public string CustomData;

        /// <summary>
        /// Combined additional sql script clips which will be included into the sql script batch.
        /// </summary>
        [XmlAttribute("additionalSqlScript")]
        public string AdditionalSqlScript;

        private List<PropertyConfiguration> properties = new List<PropertyConfiguration>();

        /// <summary>
        /// Gets or sets the properties configuration.
        /// </summary>
        /// <value>The properties.</value>
        [XmlArray("Properties"), XmlArrayItem("Property")]
        public PropertyConfiguration[] Properties
        {
            get
            {
                return properties.ToArray();
            }
            set
            {
                properties = new List<PropertyConfiguration>();
                if (value != null)
                {
                    foreach (PropertyConfiguration item in value)
                    {
                        properties.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the property configuration.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The property configuration</returns>
        public PropertyConfiguration GetPropertyConfiguration(string propertyName)
        {
            foreach (PropertyConfiguration item in properties)
            {
                if (item.Name == propertyName)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(PropertyConfiguration item)
        {
            lock (properties)
            {
                properties.Add(item);
            }
        }

        /// <summary>
        /// Gets the mapping column names.
        /// </summary>
        /// <param name="propertyNames">The property names.</param>
        /// <returns>The column names.</returns>
        public string[] GetMappingColumnNames(string[] propertyNames)
        {
            if (propertyNames == null)
            {
                return null;
            }

            string[] mappingColumnNames = new string[propertyNames.Length];

            for (int i = 0; i < propertyNames.Length; i++)
            {
                mappingColumnNames[i] = GetPropertyConfiguration(propertyNames[i]).MappingName;
            }

            return mappingColumnNames;
        }

        /// <summary>
        /// Gets the name of the mapping column.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The column names.</returns>
        public string GetMappingColumnName(string propertyName)
        {
            if (propertyName == null)
            {
                return null;
            }

            PropertyConfiguration pc = GetPropertyConfiguration(propertyName);
            if (pc != null)
            {
                return pc.MappingName;
            }
            else
            {
                return propertyName;
            }
        }
    }

    /// <summary>
    /// A property configuration.
    /// </summary>
    [Serializable]
    public class PropertyConfiguration
    {
        private string mappingName;
        private string sqlType;
        private bool isPrimaryKey;
        private bool isNotNull;
        private string propertyMappingColumnType;

        private string GetDefaultSqlType(Type type)
        {
            if (type.IsEnum)
            {
                return "int";
            }
            else if (type == typeof(long) || type == typeof(long?))
            {
                return "bigint";
            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                return "int";
            }
            else if (type == typeof(short) || type == typeof(short?))
            {
                return "smallint";
            }
            else if (type == typeof(byte) || type == typeof(byte?))
            {
                return "tinyint";
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                return "bit";
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return "decimal";
            }
            else if (type == typeof(float) || type == typeof(float?))
            {
                return "real";
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                return "float";
            }
            else if (type == typeof(string))
            {
                return "nvarchar(127)";
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return "datetime";
            }
            else if (type == typeof(char) || type == typeof(char?))
            {
                return "nchar";
            }
            else if (type == typeof(string))
            {
                return "nvarchar(127)";
            }
            else if (type == typeof(byte[]))
            {
                return "image";
            }
            else if (type == typeof(Guid) || type == typeof(Guid?))
            {
                return "uniqueidentifier";
            }

            return "ntext";
        }

        /// <summary>
        /// Name of the property.
        /// </summary>
        [XmlAttribute("name")]
        public string Name;

        /// <summary>
        /// Gets or sets the name of the mapping.
        /// </summary>
        /// <value>The name of the mapping.</value>
        [XmlAttribute("mappingName")]
        public string MappingName
        {
            get
            {
                if (IsQueryProperty && QueryType == "FkReverseQuery" && RelatedForeignKey != null)
                {
                    return mappingName ?? Name + "_" + RelatedForeignKey;
                }
                else
                {
                    return mappingName ?? Name;
                }
            }
            set
            {
                mappingName = value;
            }
        }

        /// <summary>
        /// Commnet of entity
        /// </summary>
        [XmlAttribute("comment")]
        public string Commnet;

        /// <summary>
        /// Type of the property.
        /// </summary>
        [XmlAttribute("type")]
        public string PropertyType;

        /// <summary>
        /// Gets or sets the type of the property mapping column.
        /// </summary>
        /// <value>The type of the property mapping column.</value>
        [XmlAttribute("mappingColumnType")]
        public string PropertyMappingColumnType
        {
            get
            {
                return propertyMappingColumnType ?? PropertyType;
            }
            set
            {
                propertyMappingColumnType = value;
            }
        }

        /// <summary>
        /// Whether the property is inherited from a base entity.
        /// </summary>
        [XmlAttribute("isInherited")]
        public bool IsInherited;

        /// <summary>
        /// Gets or sets the mapping sql type.
        /// </summary>
        /// <value>The type of the SQL.</value>
        [XmlAttribute("sqlType")]
        public string SqlType
        {
            get
            {
                if (string.IsNullOrEmpty(sqlType))
                {
                    sqlType = GetDefaultSqlType(Util.GetType(PropertyMappingColumnType) ?? typeof(string));
                }
                return sqlType;
            }
            set
            {
                sqlType = value;
            }
        }

        /// <summary>
        /// The sql default value
        /// </summary>
        [XmlAttribute("sqlDefaultValue")]
        public string SqlDefaultValue;

        /// <summary>
        /// Whether the property is readonly.
        /// </summary>
        [XmlAttribute("isReadOnly")]
        public bool IsReadOnly;

        /// <summary>
        /// Whether the property is a CompoundUnit.
        /// </summary>
        [XmlAttribute("isCompoundUnit")]
        public bool IsCompoundUnit;

        /// <summary>
        /// Whether the property is a contained property, which means if the entity saved or deleted, all contained property will be included in update object list.
        /// </summary>
        [XmlAttribute("isContained")]
        public bool IsContained;

        /// <summary>
        /// Whether this property is a query property.
        /// </summary>
        [XmlAttribute("isQuery")]
        public bool IsQueryProperty;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is primary DEFAULT_KEY.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is primary DEFAULT_KEY; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute("isPrimaryKey")]
        public bool IsPrimaryKey
        {
            get
            {
                return isPrimaryKey || ((!IsQueryProperty) && IsRelationKey);
            }
            set
            {
                isPrimaryKey = value;
            }
        }

        /// <summary>
        /// Whether a property is a friend DEFAULT_KEY.
        /// </summary>
        [XmlAttribute("isFriendKey")]
        public bool IsFriendKey;

        /// <summary>
        /// Whether the property is a lazyload query property. It is only used by query entity.
        /// </summary>
        [XmlAttribute("isLazyLoad")]
        public bool IsLazyLoad;

        /// <summary>
        /// The where condition used by query property.
        /// </summary>
        [XmlAttribute("queryWhere")]
        public string QueryWhere;

        /// <summary>
        /// The type of the query property.
        /// </summary>
        [XmlAttribute("queryType")]
        public string QueryType;

        /// <summary>
        /// The order by condition used by query property.
        /// </summary>
        [XmlAttribute("queryOrderBy")]
        public string QueryOrderBy;

        /// <summary>
        /// Whether the property is a relationkey. It is only used by relation entity.
        /// </summary>
        [XmlAttribute("isRelationKey")]
        public bool IsRelationKey;

        /// <summary>
        /// The related entity type of this relationkey. It is only used by relation entity.
        /// </summary>
        [XmlAttribute("relatedType")]
        public string RelatedType;

        /// <summary>
        /// The relation type of the query property.
        /// </summary>
        [XmlAttribute("relationType")]
        public string RelationType;

        /// <summary>
        /// The related entity type's foreignkey relating to this relationkey. It is only used by relation entity.
        /// </summary>
        [XmlAttribute("relatedForeignKey")]
        public string RelatedForeignKey;

        /// <summary>
        /// Whether need to add index for the property when creating the table in database.
        /// </summary>
        [XmlAttribute("isIndexProperty")]
        public bool IsIndexProperty;

        /// <summary>
        /// Whether the index property is desc.
        /// </summary>
        [XmlAttribute("isIndexPropertyDesc")]
        public bool IsIndexPropertyDesc;

        /// <summary>
        /// whether the property could not be NULL.
        /// </summary>
        [XmlAttribute("isNotNull")]
        public bool IsNotNull
        {
            get
            {
                return isNotNull || isPrimaryKey || IsIndexProperty;
            }
            set
            {
                isNotNull = value;
            }
        }

        /// <summary>
        /// Whether this property should not included in default XML serialization.
        /// </summary>
        [XmlAttribute("isSerializationIgnore")]
        public bool IsSerializationIgnore;

        /// <summary>
        /// Custom data.
        /// </summary>
        [XmlAttribute("customData")]
        public string CustomData;
    }

    #endregion
}
