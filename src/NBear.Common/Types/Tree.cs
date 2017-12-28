using System;
using System.Collections;
using System.Collections.Generic;

namespace NBear.Common.Types
{
    /// <summary>
    /// Node Collection of a Tree.
    /// </summary>
    /// <typeparam name="Element"></typeparam>
    public class NodeCollection<Element> : IEnumerable<Node<Element>> where Element : class
    {
        #region Implementation Detail:
        List<Node<Element>> mList = new List<Node<Element>>();
        Node<Element> mOwner = null;

        #endregion
        #region Internal Interface:
        internal NodeCollection(Node<Element> owner)
        {
            if (null == owner) throw new ArgumentNullException("owner");
            mOwner = owner;
        }
        #endregion
        #region Public Interface:
        /// <summary>
        /// Adds the specified node to collection.
        /// </summary>
        /// <param name="rhs">The RHS.</param>
        public void Add(Node<Element> rhs)
        {
            if (mOwner.DoesShareHierarchyWith(rhs))
                throw new InvalidOperationException("Cannot add an ancestor or descendant.");
            mList.Add(rhs);
            rhs.Parent = mOwner;
        }
        /// <summary>
        /// Removes the specified node from collection.
        /// </summary>
        /// <param name="rhs">The RHS.</param>
        public void Remove(Node<Element> rhs)
        {
            mList.Remove(rhs);
            rhs.Parent = null;
        }
        /// <summary>
        /// Determines whether [contains] [the specified node].
        /// </summary>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified RHS]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Node<Element> rhs)
        {
            return mList.Contains(rhs);
        }
        /// <summary>
        /// Clears this collection.
        /// </summary>
        public void Clear()
        {
            foreach (Node<Element> n in this)
                n.Parent = null;
            mList.Clear();
        }
        /// <summary>
        /// Gets the node count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return mList.Count;
            }
        }
        /// <summary>
        /// Gets the owner of this node.
        /// </summary>
        /// <value>The owner.</value>
        public Node<Element> Owner
        {
            get
            {
                return mOwner;
            }
        }
        /// <summary>
        /// Gets the <see cref="NBear.Common.Types.Node&lt;Element&gt;"/> at the specified index.
        /// </summary>
        /// <value></value>
        public Node<Element> this[int index]
        {
            get
            {
                return mList[index];
            }
        }
        #endregion
        #region IEnumerable<Element> Members
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Node<Element>> GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() 
        { 
            return this.GetEnumerator(); 
        }
        #endregion
    } // class NodeCollection
    /// <summary>
    /// A node of a Tree.
    /// </summary>
    /// <typeparam name="Element">The related data type of the tree node.</typeparam>
    public class Node<Element> where Element : class
    {
        #region Implementation Detail:
        NodeCollection<Element> mChildren = null;
        Node<Element> mParent = null;
        Element mData = null;
        #endregion
        #region Public Interface:
        /// <summary>
        /// Initializes a new instance of the <see cref="Node&lt;Element&gt;"/> class.
        /// </summary>
        /// <param name="nodedata">The nodedata.</param>
        public Node(Element nodedata)
        {
            mChildren = new NodeCollection<Element>(this);
            mData = nodedata;
        }
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public Node<Element> Parent
        {
            get
            {
                return mParent;
            }
            internal set
            {
                mParent = value;
            }
        }
        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public NodeCollection<Element> Children
        {
            get
            {
                return mChildren;
            }
        }
        /// <summary>
        /// Gets the root.
        /// </summary>
        /// <value>The root.</value>
        public Node<Element> Root
        {
            get
            {
                if (null == mParent) return this;
                return mParent.Root;
            }
        }
        /// <summary>
        /// Determines whether [is ancestor of] [the specified node].
        /// </summary>
        /// <param name="rhs">The node.</param>
        /// <returns>
        /// 	<c>true</c> if [is ancestor of] [the specified node]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAncestorOf(Node<Element> rhs)
        {
            if (mChildren.Contains(rhs)) return true;
            foreach (Node<Element> kid in mChildren)
                if (kid.IsAncestorOf(rhs)) return true;
            return false;
        }
        /// <summary>
        /// Determines whether [is descendant of] [the specified node].
        /// </summary>
        /// <param name="rhs">The node.</param>
        /// <returns>
        /// 	<c>true</c> if [is descendant of] [the specified node]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDescendantOf(Node<Element> rhs)
        {
            if (null == mParent) return false;
            if (rhs == mParent) return true;
            return mParent.IsDescendantOf(rhs);
        }
        /// <summary>
        /// Doeses the share hierarchy with.
        /// </summary>
        /// <param name="rhs">The node.</param>
        /// <returns></returns>
        public bool DoesShareHierarchyWith(Node<Element> rhs)
        {
            if (rhs == this) return true;
            if (this.IsAncestorOf(rhs)) return true;
            if (this.IsDescendantOf(rhs)) return true;
            return false;
        }
        /// <summary>
        /// Gets the data of the node.
        /// </summary>
        /// <value>The data.</value>
        public Element Data
        {
            get
            {
                return mData;
            }
        }
        /// <summary>
        /// Gets the depth first enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerable GetDepthFirstEnumerator()
        {
            yield return mData;
            foreach (Node<Element> kid in mChildren)
            {
                IEnumerator kidenumerator = kid.GetDepthFirstEnumerator().GetEnumerator();
                while (kidenumerator.MoveNext())
                    yield return kidenumerator.Current;
            }
        }
        /// <summary>
        /// Gets the breadth first enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerable GetBreadthFirstEnumerator()
        {
            Queue<Node<Element>> todo = new Queue<Node<Element>>();
            todo.Enqueue(this);
            while (0 < todo.Count)
            {
                Node<Element> n = todo.Dequeue();
                foreach (Node<Element> kid in n.mChildren)
                    todo.Enqueue(kid);
                yield return n.mData;
            }
        }
        #endregion
    }       // class Node
} // namespace AzazelDev.Collections.Trees
