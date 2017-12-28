using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;
using System.Collections;

namespace NBear.Common
{
    /// <summary>
    /// The base entity class.
    /// </summary>
    [Serializable]
    public class Entity : MarshalByRefObject
    {
        #region Nested types

        /// <summary>
        /// The event arg used by PropertyChanged event.
        /// </summary>
        public class PropertyChangedEventArgs : EventArgs
        {
            private string propertyName;
            private object oldValue;
            private object newValue;

            /// <summary>
            /// Gets or sets the name of the property.
            /// </summary>
            /// <value>The name of the property.</value>
            public string PropertyName
            {
                get
                {
                    return propertyName;
                }
                set
                {
                    propertyName = value;
                }
            }

            /// <summary>
            /// Gets or sets the old value.
            /// </summary>
            /// <value>The old value.</value>
            public object OldValue
            {
                get
                {
                    return oldValue;
                }
                set
                {
                    oldValue = value;
                }
            }

            /// <summary>
            /// Gets or sets the new value.
            /// </summary>
            /// <value>The new value.</value>
            public object NewValue
            {
                get
                {
                    return newValue;
                }
                set
                {
                    newValue = value;
                }
            }
        }

        /// <summary>
        /// Delegate stands for a property changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void PropertyChangedHandler(Entity sender, PropertyChangedEventArgs args);

        #endregion

        #region Attach & Detach

        protected bool isAttached;

        /// <summary>
        /// Determines whether this instance is attached. Not attached means an entity is newly created, or the entity is already persisted.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is attached; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAttached()
        {
            return isAttached;
        }

        /// <summary>
        /// Not attached means an entity is newly created, or the entity is already persisted. 
        /// Set entity's is-attached status as true also means entity's PropertyChanged event SHOULD be raised on property changing.
        /// </summary>
        public void Attach()
        {
            ResetModifiedPropertyStates();
            isAttached = true;
        }

        /// <summary>
        /// Not attached means an entity is newly created, or the entity is already persisted. 
        /// Set entity's is-attached status as false also means entity's PropertyChanged event SHOULD NOT be raised on property changing.
        /// </summary>
        public void Detach()
        {
            isAttached = false;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        public Entity()
        {
            this.PropertyChanged += new PropertyChangedHandler(Entity_PropertyChanged);
            isAttached = false;
        }

        #endregion

        #region Property Changes Monitor

        private Dictionary<string, object> changedProperties = new Dictionary<string, object>();

        /// <summary>
        /// Determines whether this entity is modified.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is modified; otherwise, <c>false</c>.
        /// </returns>
        public bool IsModified()
        {
            return (changedProperties.Count > 0 || toDeleteRelatedPropertyObjects.Count > 0);
        }

        /// <summary>
        /// Gets the modified properties of this entity, not including query properties and properties of base entity's.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The modified properties</returns>
        public Dictionary<string, object> GetModifiedProperties(Type type)
        {
            EntityConfiguration ec = MetaDataManager.GetEntityConfiguration(type.ToString());

            if (ec.BaseEntity == null && type == this.GetType())
            {
                return changedProperties;
            }

            Check.Require(typeof(Entity).IsAssignableFrom(type), "type must be an Entity");

            List<string> toRemoveItems = new List<string>();
            foreach (string item in changedProperties.Keys)
            {
                PropertyConfiguration pc = ec.GetPropertyConfiguration(item);
                if (pc == null || pc.IsInherited || pc.IsPrimaryKey)
                {
                    toRemoveItems.Add(item);
                }
            }
            Dictionary<string, object> retProperties = new Dictionary<string, object>();
            foreach (string item in changedProperties.Keys)
            {
                if (!toRemoveItems.Contains(item))
                {
                    retProperties.Add(item, changedProperties[item]);
                }
            }
            return retProperties;
        }

        /// <summary>
        /// Gets all the modified properties in the entire entity hierachy.
        /// </summary>
        /// <returns>All the modified properties</returns>
        public Dictionary<string, object> GetModifiedProperties()
        {
            return changedProperties;
        }

        /// <summary>
        /// Sets the modified properties.
        /// </summary>
        /// <param name="changedProperties">The changed properties.</param>
        public void SetModifiedProperties(Dictionary<string, object> changedProperties)
        {
            if (changedProperties != null)
            {
                this.changedProperties = changedProperties;
            }
            else
            {
                changedProperties = new Dictionary<string, object>();
            }
        }

        private void Entity_PropertyChanged(Entity sender, Entity.PropertyChangedEventArgs args)
        {
            if (sender == this && sender.isAttached)
            {
                //if ((args.OldValue == null && args.NewValue != null) || (args.OldValue != null && (!args.OldValue.Equals(args.NewValue))))
                //{
                    lock (changedProperties)
                    {
                        if (changedProperties.ContainsKey(args.PropertyName))
                        {
                            changedProperties[args.PropertyName] = args.NewValue;
                        }
                        else
                        {
                            changedProperties.Add(args.PropertyName, args.NewValue);
                        }
                    }
                //}
            }
        }

        /// <summary>
        /// Resets the modified property states.
        /// </summary>
        public void ResetModifiedPropertyStates()
        {
            changedProperties.Clear();
            toDeleteRelatedPropertyObjects.Clear();
            toSaveRelatedPropertyObjects.Clear();
        }

        /// <summary>
        /// The property changed event.
        /// </summary>
        public event PropertyChangedHandler PropertyChanged;

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldVal">The old val.</param>
        /// <param name="newVal">The new val.</param>
        protected void OnPropertyChanged(string propertyName, object oldVal, object newVal)
        {
            if (oldVal == newVal)
            {
                return;
            }

            if (PropertyChanged != null)
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs();
                args.PropertyName = propertyName;
                args.OldValue = oldVal;
                args.NewValue = newVal;

                PropertyChanged(this, args);
            }
        }

        /// <summary>
        /// Sets all property as modified.
        /// </summary>
        public void SetAllPropertiesAsModified()
        {
            string[] columnNames = GetPropertyMappingColumnNames();
            object[] columnValues = GetPropertyValues();
            PropertyConfiguration[] pcs = GetEntityConfiguration().Properties;

            lock (changedProperties)
            {
                changedProperties.Clear();

                int j = 0;
                for (int i = 0; i < columnNames.Length; i++)
                {
                    while (pcs[j].MappingName != columnNames[i])
                    {
                        j++;
                    }

                    if (!pcs[j].IsPrimaryKey)
                    {
                        changedProperties.Add(pcs[j].Name, columnValues[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Reloads all queries properties's value.
        /// </summary>
        /// <param name="includeLazyLoadQueries">if set to <c>true</c> [include lazy load queries].</param>
        public virtual void ReloadQueries(bool includeLazyLoadQueries) { }

        #endregion

        #region GetPropertyMappingColumnNames

        private static void GuessPrimaryKey(EntityConfiguration ec, List<string> primaryKeys)
        {
            //check name = ID or GUID column first
            foreach (PropertyConfiguration pc in ec.Properties)
            {
                if (pc.MappingName.ToUpper() == "ID" || pc.MappingName.ToUpper() == "GUID")
                {
                    primaryKeys.Add(pc.MappingName);
                    return;
                }
            }

            //check the first ends with ID or Guid column
            foreach (PropertyConfiguration pc in ec.Properties)
            {
                if (pc.MappingName.ToUpper().EndsWith("ID") || pc.MappingName.ToUpper().EndsWith("GUID"))
                {
                    primaryKeys.Add(pc.MappingName);
                    return;
                }
            }

            //or threat the first column as DEFAULT_KEY column
            primaryKeys.Add(ec.Properties[0].MappingName);
        }

        /// <summary>
        /// Gets the primary key mapping column names.
        /// </summary>
        /// <param name="ec">The ec.</param>
        /// <returns></returns>
        public static string[] GetPrimaryKeyMappingColumnNames(EntityConfiguration ec)
        {
            List<string> primaryKeys = new List<string>();
            foreach (PropertyConfiguration pc in ec.Properties)
            {
                if (pc.IsPrimaryKey)
                {
                    primaryKeys.Add(pc.MappingName);
                }
            }

            if (primaryKeys.Count == 0)
            {
                //take the most possible column as a single primary DEFAULT_KEY
                GuessPrimaryKey(ec, primaryKeys);
            }

            return primaryKeys.ToArray();
        }

        /// <summary>
        /// Gets the property mapping column names.
        /// </summary>
        /// <param name="ec">The ec.</param>
        /// <returns>Column names</returns>
        public static string[] GetPropertyMappingColumnNames(EntityConfiguration ec)
        {
            List<string> columnNames = new List<string>();
            foreach (PropertyConfiguration pc in ec.Properties)
            {
                if ((!pc.IsQueryProperty) || (pc.QueryType == "FkReverseQuery"))
                {
                    columnNames.Add(pc.MappingName);
                }
            }

            return columnNames.ToArray();
        }

        /// <summary>
        /// Gets the create property mapping column names.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string[] GetCreatePropertyMappingColumnNames(Type type)
        {
            return GetCreatePropertyMappingColumnNames(MetaDataManager.GetEntityConfiguration(type.ToString()));
        }

        /// <summary>
        /// Gets the create property mapping column names.
        /// </summary>
        /// <param name="ec">The ec.</param>
        /// <returns>Column names</returns>
        public static string[] GetCreatePropertyMappingColumnNames(EntityConfiguration ec)
        {
            List<string> insertColumnNames = new List<string>();
            foreach (PropertyConfiguration pc in ec.Properties)
            {
                if ((!(pc.IsReadOnly && ec.BaseEntity == null)) && ((!pc.IsQueryProperty) || (pc.QueryType == "FkReverseQuery")) && ((!pc.IsInherited) || pc.IsPrimaryKey))
                {
                    insertColumnNames.Add(pc.MappingName);
                }
            }

            return insertColumnNames.ToArray();
        }

        /// <summary>
        /// Gets the create property mapping column names.
        /// </summary>
        /// <returns>Column names</returns>
        public string[] GetPropertyMappingColumnNames()
        {
            return GetPropertyMappingColumnNames(this.GetEntityConfiguration());
        }

        #endregion

        #region Get & Set PropertyValues

        /// <summary>
        /// Gets the property values.
        /// </summary>
        /// <returns>The values.</returns>
        public virtual object[] GetPropertyValues() { return null; }

        /// <summary>
        /// Sets the property values.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public virtual void SetPropertyValues(IDataReader reader) { }

        /// <summary>
        /// Sets the property values.
        /// </summary>
        /// <param name="row">The row.</param>
        public virtual void SetPropertyValues(DataRow row) { }

        /// <summary>
        /// Gets the property values.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="columnNames">The column names.</param>
        /// <returns>The values.</returns>
        public static object[] GetPropertyValues(object obj, params string[] columnNames)
        {
            Check.Require(obj != null, "obj could not be null.");
            Check.Require(obj is Entity, "obj must be an Entity.");
            Check.Require(columnNames != null, "columnNames could not be null.");
            Check.Require(columnNames.Length > 0, "columnNames's length could not be 0.");

            Entity entityObj = obj as Entity;

            string[] names = entityObj.GetPropertyMappingColumnNames();
            object[] values = entityObj.GetPropertyValues();
            List<object> lsValues = new List<object>();
            foreach (string item in columnNames)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    if (names[i] == item)
                    {
                        lsValues.Add(values[i]);
                        break;
                    }
                }
            }

            return lsValues.ToArray();
        }

        /// <summary>
        /// Gets the primary DEFAULT_KEY values.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="obj">The obj.</param>
        /// <returns>The values.</returns>
        public static object[] GetPrimaryKeyValues(object obj)
        {
            Check.Require(obj != null, "obj could not be null.");
            Check.Require(obj is Entity, "obj must be an Entity.");

            return GetPropertyValues(obj, GetPrimaryKeyMappingColumnNames(((Entity)obj).GetEntityConfiguration()));
        }

        /// <summary>
        /// Gets the create property values.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="obj">The obj.</param>
        /// <returns>The values.</returns>
        public static object[] GetCreatePropertyValues(Type type, object obj)
        {
            Check.Require(obj != null, "obj could not be null.");
            Check.Require(obj is Entity, "obj must be an Entity.");

            return GetPropertyValues(obj, GetCreatePropertyMappingColumnNames(type));
        }

        #endregion

        #region MarshalByRefObject

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"></see> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"></see> property.
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure"/></PermissionSet>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        #endregion
        
        #region QueryProxy

        private List<string> queryLoadedProperties = new List<string>();

        /// <summary>
        /// Sets the property as loaded.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void SetPropertyLoaded(string propertyName)
        {
            lock (queryLoadedProperties)
            {
                if (!queryLoadedProperties.Contains(propertyName))
                {
                    queryLoadedProperties.Add(propertyName);
                }
            }
        }

        /// <summary>
        /// Determines whether query property is loaded.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// 	<c>true</c> if query property is loaded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQueryPropertyLoaded(string propertyName)
        {
            return (!isAttached) || queryLoadedProperties.Contains(propertyName);
        }

        /// <summary>
        /// The proxy to do actual query property loading.
        /// </summary>
        /// <param name="returnEntityType">return type of the query.</param>
        /// <param name="propertyName">related property name</param>
        /// <param name="where">where sql clip.</param>
        /// <param name="orderBy">order by  sql clip.</param>
        /// <param name="baseEntity">instance of the owner entity.</param>
        /// <returns>The query result.</returns>
        public delegate object QueryProxyHandler(Type returnEntityType, string propertyName, string where, string orderBy, Entity baseEntity);

        internal QueryProxyHandler onQuery;

        /// <summary>
        /// Set the actual query proxy handler binded to this entity.
        /// </summary>
        /// <param name="onQuery">The on query.</param>
        public void SetQueryProxy(QueryProxyHandler onQuery)
        {
            this.onQuery = onQuery;
        }

        /// <summary>
        /// Queries array of the specified return entity type.
        /// </summary>
        /// <param name="returnEntityType">Type of the return entity.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="baseEntity">The base entity.</param>
        /// <returns>The query result.</returns>
        protected object Query(Type returnEntityType, string propertyName, Entity baseEntity)
        {
            if (onQuery != null)
            {
                EntityConfiguration ec = baseEntity.GetEntityConfiguration();
                PropertyConfiguration pc = ec.GetPropertyConfiguration(propertyName);

                try
                {
                    return onQuery(returnEntityType, propertyName, pc.QueryWhere, pc.QueryOrderBy, baseEntity);
                }
                catch
                {
                    onQuery = null;
                }
            }
            return null;
        }

        /// <summary>
        /// Queries a single entity instance.
        /// </summary>
        /// <param name="returnEntityType">Type of the return entity.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="baseEntity">The base entity.</param>
        /// <returns>The query result.</returns>
        protected object QueryOne(Type returnEntityType, string propertyName, Entity baseEntity)
        {
            Array objs = (Array)Query(returnEntityType, propertyName, baseEntity);
            if (objs != null && objs.Length > 0)
            {
                return objs.GetValue(0);
            }
            return null;
        }

        #endregion

        #region Add & Remove Array Property

        private Dictionary<string, List<object>> toDeleteRelatedPropertyObjects = new Dictionary<string, List<object>>();
        private Dictionary<string, List<object>> toSaveRelatedPropertyObjects = new Dictionary<string, List<object>>();

        /// <summary>
        /// Return the dictionary contains all the objects need to be cascade deleted when this object is deleted or saved.
        /// </summary>
        /// <returns>The dictionary contains all the objects need to be cascade deleted</returns>
        public Dictionary<string, List<object>> GetToDeleteRelatedPropertyObjects()
        {
            return toDeleteRelatedPropertyObjects;
        }

        /// <summary>
        /// Gets to save related property objects.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<object>> GetToSaveRelatedPropertyObjects()
        {
            return toSaveRelatedPropertyObjects;
        }

        /// <summary>
        /// Clears to delete related property objects.
        /// </summary>
        public void ClearToDeleteRelatedPropertyObjects()
        {
            toDeleteRelatedPropertyObjects.Clear();
        }

        /// <summary>
        /// Clears to save related property objects.
        /// </summary>
        public void ClearToSaveRelatedPropertyObjects()
        {
            toSaveRelatedPropertyObjects.Clear();
        }

        /// <summary>
        /// Called when query one property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected void OnQueryOnePropertyChanged(string propertyName, object oldValue, object newValue)
        {
            bool isLoadedBefore = IsQueryPropertyLoaded(propertyName);
            SetPropertyLoaded(propertyName);

            if (!isLoadedBefore)
            {
                return;
            }

            if (oldValue == newValue)
            {
                return;
            }

            EntityConfiguration ec = GetEntityConfiguration();
            PropertyConfiguration pc = ec.GetPropertyConfiguration(propertyName);

            if ((!(pc.IsContained || pc.QueryType == "ManyToManyQuery")))
            {
                return;
            }

            if (oldValue != null)
            {
                //lock (toSaveRelatedPropertyObjects)
                //{
                //    if (toSaveRelatedPropertyObjects.ContainsKey(propertyName) && toSaveRelatedPropertyObjects[propertyName].Contains(oldValue))
                //    {
                //        toSaveRelatedPropertyObjects[propertyName].Remove(oldValue);
                //    }
                //}
                //lock (toDeleteRelatedPropertyObjects)
                //{
                //    if (!toDeleteRelatedPropertyObjects.ContainsKey(propertyName))
                //    {
                //        toDeleteRelatedPropertyObjects.Add(propertyName, new List<object>());
                //    }
                //    if (!toDeleteRelatedPropertyObjects[propertyName].Contains(oldValue))
                //    {
                //        toDeleteRelatedPropertyObjects[propertyName].Add(oldValue);
                //    }
                //}

                OnQueryPropertyItemRemove(propertyName, oldValue);
            }

            if (newValue != null)
            {
                //lock (toDeleteRelatedPropertyObjects)
                //{
                //    if (toDeleteRelatedPropertyObjects.ContainsKey(propertyName) && toDeleteRelatedPropertyObjects[propertyName].Contains(newValue))
                //    {
                //        toDeleteRelatedPropertyObjects[propertyName].Remove(newValue);
                //    }
                //}
                //lock (toSaveRelatedPropertyObjects)
                //{
                //    if (!toSaveRelatedPropertyObjects.ContainsKey(propertyName))
                //    {
                //        toSaveRelatedPropertyObjects.Add(propertyName, new List<object>());
                //    }
                //    if (!toSaveRelatedPropertyObjects[propertyName].Contains(newValue))
                //    {
                //        toSaveRelatedPropertyObjects[propertyName].Add(newValue);
                //    }
                //}

                OnQueryPropertyItemAdd(propertyName, newValue);
            }
        }

        /// <summary>
        /// Called when added item query property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="item">The item.</param>
        protected void OnQueryPropertyItemAdd(string propertyName, object item)
        {
            Check.Require(item != null, "item to add could not be null.");

            bool isLoadedBefore = IsQueryPropertyLoaded(propertyName);
            SetPropertyLoaded(propertyName);

            if (!isLoadedBefore)
            {
                return;
            }

            EntityConfiguration ec = GetEntityConfiguration();
            PropertyConfiguration pc = ec.GetPropertyConfiguration(propertyName);
            if ((!(pc.IsContained || pc.QueryType == "ManyToManyQuery")))
            {
                return;
            }

            bool isInDeleteList = false;

            lock (toDeleteRelatedPropertyObjects)
            {
                if (toDeleteRelatedPropertyObjects.ContainsKey(propertyName) && toDeleteRelatedPropertyObjects[propertyName].Contains(item))
                {
                    toDeleteRelatedPropertyObjects[propertyName].Remove(item);
                    isInDeleteList = true;
                }
            }

            if (!isInDeleteList)
            {
                lock (toSaveRelatedPropertyObjects)
                {
                    if (!toSaveRelatedPropertyObjects.ContainsKey(propertyName))
                    {
                        toSaveRelatedPropertyObjects.Add(propertyName, new List<object>());
                    }
                    if (!toSaveRelatedPropertyObjects[propertyName].Contains(item))
                    {
                        toSaveRelatedPropertyObjects[propertyName].Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Called when removed item from query property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="item">The item.</param>
        protected void OnQueryPropertyItemRemove(string propertyName, object item)
        {
            Check.Require(item != null, "item to remove could not be null.");

            bool isLoadedBefore = IsQueryPropertyLoaded(propertyName);
            SetPropertyLoaded(propertyName);

            if (!isLoadedBefore)
            {
                return;
            }

            EntityConfiguration ec = GetEntityConfiguration();
            PropertyConfiguration pc = ec.GetPropertyConfiguration(propertyName);
            if ((!(pc.IsContained || pc.QueryType == "ManyToManyQuery")))
            {
                return;
            }

            bool isInSaveList = false;

            lock (toSaveRelatedPropertyObjects)
            {
                if (toSaveRelatedPropertyObjects.ContainsKey(propertyName) && toSaveRelatedPropertyObjects[propertyName].Contains(item))
                {
                    toSaveRelatedPropertyObjects[propertyName].Remove(item);
                    isInSaveList = true;
                }
            }

            if (!isInSaveList)
            {
                lock (toDeleteRelatedPropertyObjects)
                {
                    if (!toDeleteRelatedPropertyObjects.ContainsKey(propertyName))
                    {
                        toDeleteRelatedPropertyObjects.Add(propertyName, new List<object>());
                    }

                    if (!toDeleteRelatedPropertyObjects[propertyName].Contains(item))
                    {
                        toDeleteRelatedPropertyObjects[propertyName].Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Called when query property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValues">The old values.</param>
        /// <param name="newValues">The new values.</param>
        protected void OnQueryPropertyChanged(string propertyName, object oldValues, object newValues)
        {
            bool isLoadedBefore = IsQueryPropertyLoaded(propertyName);
            SetPropertyLoaded(propertyName);

            if (newValues != null)
            {
                BindArrayListEventHandlers(propertyName, newValues);
            }

            if (!isLoadedBefore)
            {
                return;
            }

            EntityConfiguration ec = GetEntityConfiguration();
            PropertyConfiguration pc = ec.GetPropertyConfiguration(propertyName);

            if ((!(pc.IsContained || pc.QueryType == "ManyToManyQuery")) || oldValues == newValues)
            {
                return;
            } 

            if (oldValues != null)
            {
                //lock (toSaveRelatedPropertyObjects)
                //{
                //    if (toSaveRelatedPropertyObjects.ContainsKey(propertyName))
                //    {
                //        foreach (object oldValue in (IEnumerable)oldValues)
                //        {
                //            if (toSaveRelatedPropertyObjects[propertyName].Contains(oldValue))
                //            {
                //                toSaveRelatedPropertyObjects[propertyName].Remove(oldValue);
                //            }
                //        }
                //    }
                //}
                //lock (toDeleteRelatedPropertyObjects)
                //{
                //    if (!toDeleteRelatedPropertyObjects.ContainsKey(propertyName))
                //    {
                //        toDeleteRelatedPropertyObjects.Add(propertyName, new List<object>());
                //    }

                //    foreach (object oldValue in (IEnumerable)oldValues)
                //    {
                //        if (!toDeleteRelatedPropertyObjects[propertyName].Contains(oldValue))
                //        {
                //            toDeleteRelatedPropertyObjects[propertyName].Add(oldValue);
                //        }
                //    }
                //}

                foreach (object oldValue in (IEnumerable)oldValues)
                {
                    OnQueryPropertyItemRemove(propertyName, oldValue);
                }
            }

            if (newValues != null)
            {
                //lock (toDeleteRelatedPropertyObjects)
                //{
                //    if (toDeleteRelatedPropertyObjects.ContainsKey(propertyName))
                //    {
                //        foreach (object newValue in (IEnumerable)newValues)
                //        {
                //            if (toDeleteRelatedPropertyObjects[propertyName].Contains(newValue))
                //            {
                //                toDeleteRelatedPropertyObjects[propertyName].Remove(newValue);
                //            }
                //        }
                //    }
                //}
                //lock (toSaveRelatedPropertyObjects)
                //{
                //    if (!toSaveRelatedPropertyObjects.ContainsKey(propertyName))
                //    {
                //        toSaveRelatedPropertyObjects.Add(propertyName, new List<object>());
                //    }

                //    foreach (object newValue in (IEnumerable)newValues)
                //    {
                //        if (!toSaveRelatedPropertyObjects[propertyName].Contains(newValue))
                //        {
                //            toSaveRelatedPropertyObjects[propertyName].Add(newValue);
                //        }
                //    }
                //}

                foreach (object newValue in (IEnumerable)newValues)
                {
                    OnQueryPropertyItemAdd(propertyName, newValue);
                }
            }
        }

        /// <summary>
        /// Binds the array list event handlers.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="newValues">The new values.</param>
        protected void BindArrayListEventHandlers(string propertyName, object newValues)
        {
            IEntityArrayList entityArrayList = (IEntityArrayList)newValues;
            entityArrayList.OnAddCallbackHandler = new EntityArrayItemChangeHandler(OnQueryPropertyItemAdd);
            entityArrayList.OnRemoveCallbackHandler = new EntityArrayItemChangeHandler(OnQueryPropertyItemRemove);
            entityArrayList.PropertyName = propertyName;
        }

        #endregion

        #region Read Meta Data Performance Enhancement

        /// <summary>
        /// Gets the entity configuration.
        /// </summary>
        /// <returns>The entity configuration</returns>
        public virtual EntityConfiguration GetEntityConfiguration() { return null; }

        #endregion

        #region Entity to DataTable

        /// <summary>
        /// Convert an entity array to a data table.
        /// </summary>
        /// <param name="objs">The entity array.</param>
        /// <returns>The data table.</returns>
        public static DataTable EntityArrayToDataTable<EntityType>(EntityType[] objs)
            where EntityType : Entity, new()
        {
            EntityConfiguration ec;

            if (objs != null && objs.Length > 0)
            {
                ec = objs[0].GetEntityConfiguration();
            }
            else
            {
                ec = new EntityType().GetEntityConfiguration();
            }

            DataTable table = new DataTable(ec.Name);

            foreach (PropertyConfiguration pc in ec.Properties)
            {
                if ((!pc.IsQueryProperty) || (pc.QueryType == "FkReverseQuery"))
                {
                    DataColumn column = new DataColumn(pc.QueryType == "FkReverseQuery" ? pc.MappingName : pc.Name, Util.GetType(pc.PropertyMappingColumnType.Replace("System.Nullable`1[", "").Replace("]", "")));
                    table.Columns.Add(column);
                }
            }

            if (objs != null && objs.Length > 0)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    object[] values = Entity.GetPropertyValues(objs[i], Entity.GetPropertyMappingColumnNames(objs[0].GetEntityConfiguration()));
                    DataRow row = table.NewRow();

                    int j = 0;
                    foreach (PropertyConfiguration pc in ec.Properties)
                    {
                        if ((!pc.IsQueryProperty) || (pc.QueryType == "FkReverseQuery"))
                        {
                            object value = (values[j] == null ? DBNull.Value : values[j]);
                            if (pc.IsCompoundUnit)
                            {
                                value = SerializationManager.Serialize(value);
                            }
                            row[j] = value;
                        }

                        j++;
                    }

                    table.Rows.Add(row);
                }
            }
            table.AcceptChanges();
            return table;
        }

        /// <summary>
        /// Data table to entity array.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="setEntitiesAsModified">if set to <c>true</c> set entities as modified.</param>
        /// <returns></returns>
        public static EntityType[] DataTableToEntityArray<EntityType>(DataTable dt, bool setEntitiesAsModified)
            where EntityType : Entity, new()
        {
            if (dt == null)
            {
                return null;
            }

            EntityType[] objs = new EntityType[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                objs[i] = new EntityType();
                objs[i].SetPropertyValues(dt.Rows[i]);
                if (setEntitiesAsModified)
                {
                    objs[i].Attach();
                    objs[i].SetAllPropertiesAsModified();
                }
            }

            return objs;
        }

        #endregion

        #region Get Value Helper

        /// <summary>
        /// Gets the GUID.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        protected Guid GetGuid(IDataReader reader, int index)
        {
            if (reader.GetFieldType(index) == typeof(Guid))
            {
                return reader.GetGuid(index);
            }
            else
            {
                return new Guid(reader.GetValue(index).ToString());
            }
        }

        /// <summary>
        /// Gets the GUID.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        protected Guid GetGuid(DataRow row, int index)
        {
            if (row.Table.Columns[index].DataType == typeof(Guid))
            {
                return (Guid)row[index];
            }
            else
            {
                return new Guid(row[index].ToString());
            }
        }

        #endregion
    }
}