using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace NBear.Common
{
    /// <summary>
    /// EntityArrayList item change handler, used as callback handlers in EntityArrayLists.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="item">The changed item.</param>
    public delegate void EntityArrayItemChangeHandler(string propertyName, object item);

    /// <summary>
    /// Interface for entity array lists
    /// </summary>
    public interface IEntityArrayList
    {
        /// <summary>
        /// Gets or sets the on add callback handler.
        /// </summary>
        /// <value>The on add callback handler.</value>
        EntityArrayItemChangeHandler OnAddCallbackHandler { get; set; }
        /// <summary>
        /// Gets or sets the on remove callback handler.
        /// </summary>
        /// <value>The on remove callback handler.</value>
        EntityArrayItemChangeHandler OnRemoveCallbackHandler { get; set; }
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        string PropertyName { get; set; }
        /// <summary>
        /// Gets the type of the array item.
        /// </summary>
        /// <returns></returns>
        Type GetArrayItemType();
    }

    /// <summary>
    /// The base generic entity array type.
    /// </summary>
    /// <typeparam name="EntityType">entity type</typeparam>
    public abstract class EntityArrayList<EntityType> : MarshalByRefObject, ICollection<EntityType>, IEntityArrayList
        where EntityType : Entity, new()
    {
        private List<EntityType> list = new List<EntityType>();
        private EntityArrayItemChangeHandler onAddCallbackHandler;
        private EntityArrayItemChangeHandler onRemoveCallbackHandler;
        private string propertyName;

        /// <summary>
        /// Toes the array.
        /// </summary>
        /// <returns></returns>
        public EntityType[] ToArray()
        {
            return list.ToArray();
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <value></value>
        public EntityType this[int index]
        {
            get
            {
                return list[index];
            }
        }

        /// <summary>
        /// Adds a range of entities.
        /// </summary>
        /// <param name="items">The items.</param>
        public void AddRange(EntityType[] items)
        {
            if (items != null)
            {
                foreach (EntityType item in items)
                {
                    Add(item);
                }
            }
        }

        #region ICollection<EntityType> Members

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public void Add(EntityType item)
        {
            Check.Require(item != null, "item could not be null.");

            if (!list.Contains(item))
            {
                if (OnAddCallbackHandler != null)
                {
                    OnAddCallbackHandler(propertyName, item);
                }
                list.Add(item);
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only. </exception>
        public void Clear()
        {
            if (OnRemoveCallbackHandler != null)
            {
                foreach (EntityType item in list)
                {
                    OnRemoveCallbackHandler(propertyName, item);
                }
            }

            list.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if item is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
        /// </returns>
        public bool Contains(EntityType item)
        {
            return list.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        /// <exception cref="T:System.ArgumentNullException">array is null.</exception>
        /// <exception cref="T:System.ArgumentException">array is multidimensional.-or-arrayIndex is equal to or greater than the length of array.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"></see> is greater than the available space from arrayIndex to the end of the destination array.-or-Type T cannot be cast automatically to the type of the destination array.</exception>
        public void CopyTo(EntityType[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</returns>
        public int Count
        {
            get { return list.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if item was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if item is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public bool Remove(EntityType item)
        {
            if (list.Contains(item))
            {
                if (OnRemoveCallbackHandler != null)
                {
                    OnRemoveCallbackHandler(propertyName, item);
                }

                return list.Remove(item);
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region IEnumerable<EntityType> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<EntityType> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
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

        #region IEntityArrayList Members

        /// <summary>
        /// Gets or sets the on add callback handler.
        /// </summary>
        /// <value>The on add callback handler.</value>
        [XmlIgnore]
        public EntityArrayItemChangeHandler OnAddCallbackHandler
        {
            get
            {
                return onAddCallbackHandler;
            }
            set
            {
                onAddCallbackHandler = value;
            }
        }

        /// <summary>
        /// Gets or sets the on remove callback handler.
        /// </summary>
        /// <value>The on remove callback handler.</value>
        [XmlIgnore]
        public EntityArrayItemChangeHandler OnRemoveCallbackHandler
        {
            get
            {
                return onRemoveCallbackHandler;
            }
            set
            {
                onRemoveCallbackHandler = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        [XmlIgnore]
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
        /// Gets the type of the array item.
        /// </summary>
        /// <returns></returns>
        public Type GetArrayItemType()
        {
            return typeof(EntityType);
        }

        #endregion
    }
}
