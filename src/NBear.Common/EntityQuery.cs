using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace NBear.Common
{
    /// <summary>
    /// A property item stands for a property in strong typed query.
    /// </summary>
    public class PropertyItem
    {
        /// <summary>
        /// All stands for *, which is only used in Gateway.Count query.
        /// </summary>
        public static readonly PropertyItem All = new PropertyItem("*");

        static PropertyItem()
        {
            All.leftToken = string.Empty;
            All.rightToken = string.Empty;
            All.paramPrefix = string.Empty;
        }

        private string columnName;
        private string leftToken = "{";
        private string rightToken = "}";
        private string paramPrefix = "@";

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName
        {
            get
            {
                return columnName;
            }
            set
            {
                columnName = value;
            }
        }

        /// <summary>
        /// Gets the name of the intermedia column name of the property.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName
        {
            get
            {
                return leftToken + columnName + rightToken;
            }
        }

        /// <summary>
        /// Gets the name of the param.
        /// </summary>
        /// <value>The name of the param.</value>
        internal string ParamName
        {
            get
            {
                return paramPrefix + columnName + "_" + Guid.NewGuid().ToString("N") + " ";
            }
        }

        /// <summary>
        /// Gets the ascendent order by clip of this property.
        /// </summary>
        /// <value>The asc.</value>
        public OrderByClip Asc
        {
            get
            {
                return new OrderByClip(this, false);
            }
        }

        /// <summary>
        /// Gets the descendent order by clip of this property.
        /// </summary>
        /// <value>The desc.</value>
        public OrderByClip Desc
        {
            get
            {
                return new OrderByClip(this, true);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyItem"/> class.
        /// </summary>
        public PropertyItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyItem"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public PropertyItem(string propertyName)
        {
            this.columnName = propertyName;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        #region Equals and Not Equals

        /// <summary>
        /// Operator ==s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator ==(PropertyItem left, PropertyItem right)
        {
            if (((object)right) == null)
            {
                return new WhereClip(left.ColumnName + " IS NULL");
            }
            else if (((object)left) == null)
            {
                return new WhereClip(right.ColumnName + " IS NULL");
            }

            return new WhereClip(left.ColumnName + " = " + right.ColumnName);
        }

        /// <summary>
        /// Operator !=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator !=(PropertyItem left, PropertyItem right)
        {
            if (((object)right) == null)
            {
                return new WhereClip("( NOT " + left.ColumnName + " IS NULL)");
            }
            else if (((object)left) == null)
            {
                return new WhereClip("( NOT " + right.ColumnName + " IS NULL)");
            }

            return new WhereClip(left.ColumnName + " <> " + right.ColumnName);
        }

        /// <summary>
        /// Operator ==s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator ==(PropertyItem left, object right)
        {
            if (right == null)
            {
                return new WhereClip(left.ColumnName + " IS NULL");
            }

            return new WhereClip(left.ColumnName + " = " + left.ParamName, right);
        }

        /// <summary>
        /// Operator !=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator !=(PropertyItem left, object right)
        {
            if (right == null)
            {
                return new WhereClip("( NOT " + left.ColumnName + " IS NULL)");
            }

            return new WhereClip(left.ColumnName + " <> " + left.ParamName, right);
        }

        /// <summary>
        /// Operator ==s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator ==(object left, PropertyItem right)
        {
            if (left == null)
            {
                return new WhereClip(right.ColumnName + " IS NULL");
            }

            return new WhereClip(right.ParamName + " = " + right.ColumnName, left);
        }

        /// <summary>
        /// Operator !=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator !=(object left, PropertyItem right)
        {
            if (left == null)
            {
                return new WhereClip("( NOT " + right.ColumnName + " IS NULL)");
            }

            return new WhereClip(right.ParamName + " <> " + right.ColumnName, left);
        }

        #endregion

        #region Greater and Less

        /// <summary>
        /// Operator &gt;s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator >(PropertyItem left, PropertyItem right)
        {
            if (((object)right) == null)
            {
                return new WhereClip(left.ColumnName + " > " + left.ParamName, right);
            }
            else if (((object)left) == null)
            {
                return new WhereClip(right.ColumnName + " < " + right.ParamName, left);
            }
            return new WhereClip(left.ColumnName + " > " + right.ColumnName);
        }

        /// <summary>
        /// Operator &lt;s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator <(PropertyItem left, PropertyItem right)
        {
            if (((object)right) == null)
            {
                return new WhereClip(left.ColumnName + " < " + left.ParamName, right);
            }
            else if (((object)left) == null)
            {
                return new WhereClip(right.ColumnName + " > " + right.ParamName, left);
            }
            return new WhereClip(left.ColumnName + " < " + right.ColumnName);
        }

        /// <summary>
        /// Operator &gt;s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator >(PropertyItem left, object right)
        {
            return new WhereClip(left.ColumnName + " > " + left.ParamName, right);
        }

        /// <summary>
        /// Operator &lt;s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator <(PropertyItem left, object right)
        {
            return new WhereClip(left.ColumnName + " < " + left.ParamName, right);
        }

        /// <summary>
        /// Operator &gt;s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator >(object left, PropertyItem right)
        {
            return new WhereClip(right.ParamName + " > " + right.ColumnName, left);
        }

        /// <summary>
        /// Operator &lt;s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator <(object left, PropertyItem right)
        {
            return new WhereClip(right.ParamName + " < " + right.ColumnName, left);
        }

        #endregion

        #region Greater or Equals and Less and Equals

        /// <summary>
        /// Operator &gt;=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator >=(PropertyItem left, PropertyItem right)
        {
            if (((object)right) == null)
            {
                return new WhereClip(left.ColumnName + " >= " + left.ParamName, right);
            }
            else if (((object)left) == null)
            {
                return new WhereClip(right.ColumnName + " <= " + right.ParamName, left);
            }
            return new WhereClip(left.ColumnName + " >= " + right.ColumnName);
        }

        /// <summary>
        /// Operator &lt;=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator <=(PropertyItem left, PropertyItem right)
        {
            if (((object)right) == null)
            {
                return new WhereClip(left.ColumnName + " <= " + left.ParamName, right);
            }
            else if (((object)left) == null)
            {
                return new WhereClip(right.ColumnName + " >= " + right.ParamName, left);
            }
            return new WhereClip(left.ColumnName + " <= " + right.ColumnName);
        }

        /// <summary>
        /// Operator &gt;=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator >=(PropertyItem left, object right)
        {
            return new WhereClip(left.ColumnName + " >= " + left.ParamName, right);
        }

        /// <summary>
        /// Operator &lt;=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator <=(PropertyItem left, object right)
        {
            return new WhereClip(left.ColumnName + " <= " + left.ParamName, right);
        }

        /// <summary>
        /// Operator &gt;=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator >=(object left, PropertyItem right)
        {
            return new WhereClip(right.ParamName + " >= " + right.ColumnName, left);
        }

        /// <summary>
        /// Operator &lt;=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator <=(object left, PropertyItem right)
        {
            return new WhereClip(right.ParamName + " <= " + right.ColumnName, left);
        }

        #endregion

        #region + - * / %

        /// <summary>
        /// Operator +s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator +(PropertyItem left, PropertyItem right)
        {
            return new PropertyItemParam(string.Format("{0} + {1}", left.ColumnName, right.ColumnName));
        }

        /// <summary>
        /// Operator +s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator +(object left, PropertyItem right)
        {
            Check.Require(left != null, "left could not be null.");

            return new PropertyItemParam(string.Format("{0} + {1}", Util.FormatParamVal(left), right.ColumnName));
        }

        /// <summary>
        /// Operator +s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator +(PropertyItem left, object right)
        {
            Check.Require(right != null, "right could not be null.");

            return new PropertyItemParam(string.Format("{0} + {1}", left.ColumnName, Util.FormatParamVal(right)));
        }

        /// <summary>
        /// Operator -s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator -(PropertyItem left, PropertyItem right)
        {
            return new PropertyItemParam(string.Format("{0} - {1}", left.ColumnName, right.ColumnName));
        }

        /// <summary>
        /// Operator -s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator -(object left, PropertyItem right)
        {
            Check.Require(left != null, "left could not be null.");

            return new PropertyItemParam(string.Format("{0} - {1}", Util.FormatParamVal(left), right.ColumnName));
        }

        /// <summary>
        /// Operator -s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator -(PropertyItem left, object right)
        {
            Check.Require(right != null, "right could not be null.");

            return new PropertyItemParam(string.Format("{0} - {1}", left.ColumnName, Util.FormatParamVal(right)));
        }

        /// <summary>
        /// Operator *s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator *(PropertyItem left, PropertyItem right)
        {
            return new PropertyItemParam(string.Format("{0} * {1}", left.ColumnName, right.ColumnName));
        }

        /// <summary>
        /// Operator *s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator *(object left, PropertyItem right)
        {
            Check.Require(left != null, "left could not be null.");

            return new PropertyItemParam(string.Format("{0} * {1}", Util.FormatParamVal(left), right.ColumnName));
        }

        /// <summary>
        /// Operator *s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator *(PropertyItem left, object right)
        {
            Check.Require(right != null, "right could not be null.");

            return new PropertyItemParam(string.Format("{0} * {1}", left.ColumnName, Util.FormatParamVal(right)));
        }

        /// <summary>
        /// Operator /s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator /(PropertyItem left, PropertyItem right)
        {
            return new PropertyItemParam(string.Format("{0} / {1}", left.ColumnName, right.ColumnName));
        }

        /// <summary>
        /// Operator /s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator /(object left, PropertyItem right)
        {
            Check.Require(left != null, "left could not be null.");

            return new PropertyItemParam(string.Format("{0} / {1}", Util.FormatParamVal(left), right.ColumnName));
        }

        /// <summary>
        /// Operator /s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator /(PropertyItem left, object right)
        {
            Check.Require(right != null, "right could not be null.");

            return new PropertyItemParam(string.Format("{0} / {1}", left.ColumnName, Util.FormatParamVal(right)));
        }

        /// <summary>
        /// Operator %s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator %(PropertyItem left, PropertyItem right)
        {
            return new PropertyItemParam(string.Format("{0} % {1}", left.ColumnName, right.ColumnName));
        }

        /// <summary>
        /// Operator %s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator %(object left, PropertyItem right)
        {
            Check.Require(left != null, "left could not be null.");

            return new PropertyItemParam(string.Format("{0} % {1}", Util.FormatParamVal(left), right.ColumnName));
        }

        /// <summary>
        /// Operator %s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator %(PropertyItem left, object right)
        {
            Check.Require(right != null, "right could not be null.");

            return new PropertyItemParam(string.Format("{0} % {1}", left.ColumnName, Util.FormatParamVal(right)));
        }

        #endregion

        #region Additional Operations

        /// <summary>
        /// Operator !s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <returns></returns>
        public static PropertyItemParam operator!(PropertyItem left)
        {
            return new PropertyItemParam(string.Format("(~ {0})", left.ColumnName));
        }

        /// <summary>
        /// Like operator the specified right.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public WhereClip Like(string right)
        {
            Check.Require(right != null, "right could not be null.");

            return new WhereClip(this.ColumnName + " LIKE " + this.ParamName, right);
        }

        /// <summary>
        /// Betweens the specified left and right value.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public WhereClip Between(object left, object right)
        {
            Check.Require(left != null, "left could not be null.");
            Check.Require(right != null, "right could not be null.");

            return this >= left & this <= right;
        }

        /// <summary>
        /// Whether property value in the specified values.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <returns></returns>
        public WhereClip In(params object[] objs)
        {
            Check.Require(objs != null && objs.Length > 0, "objs could not be null or empty.");

            WhereClip retWhere = WhereClip.All;
            foreach (object obj in objs)
            {
                retWhere = retWhere | this == obj;
            }

            return retWhere;
        }

        /// <summary>
        /// Bitwises the and.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseAnd(object right)
        {
            return new PropertyItemParam(string.Format("{0} & {1}", this.ColumnName, Util.FormatParamVal(right)));
        }

        /// <summary>
        /// Bitwises the and.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseAnd(PropertyItem right)
        {
            return new PropertyItemParam(string.Format("{0} & {1}", this.ColumnName, right.ColumnName));
        }

        /// <summary>
        /// Bitwises the and.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseAnd(PropertyItemParam right)
        {
            return new PropertyItemParam(string.Format("{0} & ({1})", this.ColumnName, right.CustomValue));
        }

        /// <summary>
        /// Bitwises the or.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseOr(object right)
        {
            return new PropertyItemParam(string.Format("{0} | {1}", this.ColumnName, Util.FormatParamVal(right)));
        }

        /// <summary>
        /// Bitwises the or.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseOr(PropertyItem right)
        {
            return new PropertyItemParam(string.Format("{0} | {1}", this.ColumnName, right.ColumnName));
        }

        /// <summary>
        /// Bitwises the or.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseOr(PropertyItemParam right)
        {
            return new PropertyItemParam(string.Format("{0} | ({1})", this.ColumnName, right.CustomValue));
        }

        /// <summary>
        /// Bitwises the exclusive or.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseExclusiveOr(object right)
        {
            return new PropertyItemParam(string.Format("{0} ^ {1}", this.ColumnName, Util.FormatParamVal(right)));
        }

        /// <summary>
        /// Bitwises the exclusive or.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseExclusiveOr(PropertyItem right)
        {
            return new PropertyItemParam(string.Format("{0} ^ {1}", this.ColumnName, right.ColumnName));
        }

        /// <summary>
        /// Bitwises the exclusive or.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseExclusiveOr(PropertyItemParam right)
        {
            return new PropertyItemParam(string.Format("{0} ^ ({1})", this.ColumnName, right.CustomValue));
        }

        #endregion

        #region Static Members

        /// <summary>
        /// Parses the expression by entity meta data to actual sql.
        /// </summary>
        /// <param name="inStr">The in STR.</param>
        /// <param name="propertyToColumnMapHandler">The property to column map handler.</param>
        /// <param name="leftToken">The left token.</param>
        /// <param name="rightToken">The right token.</param>
        /// <param name="paramPrefix">The param prefix.</param>
        /// <returns>The actual sql.</returns>
        public static string ParseExpressionByMetaData(string inStr, PropertyToColumnMapHandler propertyToColumnMapHandler, string leftToken, string rightToken, string paramPrefix)
        {
            if (inStr == null)
            {
                return null;
            }

            string retStr = inStr;

            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\{[\w\d_]+\}", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
            System.Text.RegularExpressions.MatchCollection ms = r.Matches(retStr);
            foreach (System.Text.RegularExpressions.Match m in ms)
            {
                retStr = retStr.Replace(m.Value, string.Format("{0}{1}{2}", leftToken, propertyToColumnMapHandler(m.Value.Trim('{', '}')), rightToken));
            }

            r = new System.Text.RegularExpressions.Regex(@"@[\w\d_]+\s+", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
            ms = r.Matches(retStr);
            foreach (System.Text.RegularExpressions.Match m in ms)
            {
                retStr = retStr.Replace(m.Value, string.Format("{0}{1} ", paramPrefix, m.Value.Replace("@", "")));
            }

            return retStr;
        }

        #endregion
    }

    /// <summary>
    /// A StoredProcedureParamItem stands for a stored procedure param item in strong typed query.
    /// </summary>
    [Serializable]
    public class StoredProcedureParamItem
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
            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedureParamItem"/> class.
        /// </summary>
        public StoredProcedureParamItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedureParamItem"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public StoredProcedureParamItem(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        #region Equals and Not Equals

        /// <summary>
        /// Operator ==s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The where clip.</returns>
        public static WhereClip operator ==(StoredProcedureParamItem left, object right)
        {
            return new WhereClip(left.Name + " = @" + left.Name, right);
        }

        /// <summary>
        /// Operator !=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The where clip.</returns>
        public static WhereClip operator !=(StoredProcedureParamItem left, object right)
        {
            return new WhereClip(left.Name + " <> @" + left.Name, right);
        }

        #endregion
    }

    /// <summary>
    /// Strong typed where clip.
    /// </summary>
    [Serializable]
    public class WhereClip
    {
        private static readonly WhereClip _All = new WhereClip(null);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <value>All.</value>
        public static WhereClip All
        {
            get
            {
                return _All;
            }
        }

        private bool isNot = false;
        private string whereStr;
        private object[] paramValues;

        /// <summary>
        /// Gets the param values.
        /// </summary>
        /// <value>The param values.</value>
        public object[] ParamValues
        {
            get
            {
                if (paramValues == null)
                {
                    return new object[0];
                }
                else
                {
                    return paramValues;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WhereClip"/> class.
        /// </summary>
        public WhereClip()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WhereClip"/> class.
        /// </summary>
        /// <param name="whereStr">The where STR.</param>
        /// <param name="paramValues">The param values.</param>
        public WhereClip(string whereStr, params object[] paramValues)
        {
            this.whereStr = whereStr;
            this.paramValues = paramValues;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            if (whereStr == null)
            {
                return null;
            }

            if (isNot)
            {
                return "Not " + "(" + whereStr + ")";
            }
            else
            {
                return whereStr;
            }
        }

        #region Operators

        /// <summary>
        /// And two where clips.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>The where clip.</returns>
        public static WhereClip operator &(WhereClip left, WhereClip right)
        {
            Check.Require(left != null, "left could not be null.");
            Check.Require(right != null, "right could not be null.");

            if (left.ToString() != null && right.ToString() != null)
            {
                string where = "(" + left.ToString() + ")" + " AND " + "(" + right.ToString() + ")";
                object[] values = new object[left.ParamValues.Length + right.ParamValues.Length];
                left.ParamValues.CopyTo(values, 0);
                right.ParamValues.CopyTo(values, left.ParamValues.Length);
                return new WhereClip(where, values);
            }
            else if (left.ToString() != null && right.ToString() == null)
            {
                return left;
            }
            else if (left.ToString() == null && right.ToString() != null)
            {
                return right;
            }
            else
            {
                return All;
            }
        }

        /// <summary>
        /// Operator trues the specified right.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator true(WhereClip right)
        {
            return false;
        }

        /// <summary>
        /// Operator falses the specified right.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator false(WhereClip right)
        {
            return false;
        }

        /// <summary>
        /// Or two where clips.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The where clip.</returns>
        public static WhereClip operator |(WhereClip left, WhereClip right)
        {
            Check.Require(left != null, "left could not be null.");
            Check.Require(right != null, "right could not be null.");

            if (left.ToString() != null && right.ToString() != null)
            {
                string where = "(" + left.ToString() + ")" + " OR " + "(" + right.ToString() + ")";
                object[] values = new object[left.ParamValues.Length + right.ParamValues.Length];
                left.ParamValues.CopyTo(values, 0);
                right.ParamValues.CopyTo(values, left.ParamValues.Length);
                return new WhereClip(where, values);
            }
            else if (left.ToString() != null && right.ToString() == null)
            {
                return left;
            }
            else if (left.ToString() == null && right.ToString() != null)
            {
                return right;
            }
            else
            {
                return All;
            }
        }

        /// <summary>
        /// Not the specified where.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns>The where clip.</returns>
        public static WhereClip operator !(WhereClip right)
        {
            right.isNot = (!right.isNot);
            return right;
        }

        #endregion

        /// <summary>
        /// Gets or sets the serialized data. This property is used for serialization only
        /// </summary>
        /// <value>The serialized data.</value>
        public string SerializedData
        {
            get
            {
                JSON.JSONObject json = new JSON.JSONObject();
                json.put("isNot", SerializationManager.Serialize(isNot));
                json.put("whereStr", SerializationManager.Serialize(whereStr));
                if (paramValues != null && paramValues.Length > 0)
                {
                    JSON.JSONArray paramArrTypes = new JSON.JSONArray();
                    JSON.JSONArray paramArrValues = new JSON.JSONArray();
                    foreach (object obj in paramValues)
                    {
                        if (obj == null)
                        {
                            paramArrTypes.put(typeof(object).ToString());
                            paramArrValues.put(null);
                        }
                        else
                        {
                            paramArrTypes.put(obj.GetType().ToString());
                            paramArrValues.put(SerializationManager.Serialize(obj));
                        }
                    }
                    json.put("paramTypes", paramArrTypes);
                    json.put("paramValues", paramArrValues);
                }
                return json.ToString();
            }
            set
            {
                if (value != null)
                {
                    try
                    {
                        JSON.JSONObject json = new JSON.JSONObject(value);
                        isNot = (bool)SerializationManager.Deserialize(typeof(bool), json.getString("isNot"));
                        whereStr = (string)SerializationManager.Deserialize(typeof(string), json.getString("whereStr"));
                        JSON.JSONArray paramArrTypes = json.getJSONArray("paramTypes");
                        JSON.JSONArray paramArrValues = json.getJSONArray("paramValues");
                        if (paramArrTypes != null && paramArrTypes.Length() > 0)
                        {
                            paramValues = new object[paramArrTypes.Length()];
                            for (int i = 0; i < paramValues.Length; i++)
                            {
                                paramValues[i] = SerializationManager.Deserialize(Util.GetType(paramArrTypes.getString(i)), paramArrValues.getString(i));
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
    }

    /// <summary>
    /// Strong typed orderby clip.
    /// </summary>
    [Serializable]
    public class OrderByClip
    {
        private static readonly OrderByClip _Default = new OrderByClip(null);

        /// <summary>
        /// Gets the default order by condition.
        /// </summary>
        /// <value>The default.</value>
        public static OrderByClip Default
        {
            get
            {
                return _Default;
            }
        }

        private string orderByStr;

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return orderByStr;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderByClip"/> class.
        /// </summary>
        public OrderByClip()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderByClip"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="descend">if set to <c>true</c> [descend].</param>
        public OrderByClip(PropertyItem item, bool descend)
        {
            if (descend)
            {
                this.orderByStr = item.ColumnName + " DESC";
            }
            else
            {
                this.orderByStr = item.ColumnName;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderByClip"/> class.
        /// </summary>
        /// <param name="orderByStr">The order by STR.</param>
        public OrderByClip(string orderByStr)
        {
            this.orderByStr = orderByStr;
        }

        /// <summary>
        /// And two orderby clips.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>The combined order by clip.</returns>
        public static OrderByClip operator &(OrderByClip left, OrderByClip right)
        {
            Check.Require(left != null, "left could not be null.");
            Check.Require(right != null, "right could not be null.");

            if (left.ToString() != null && right.ToString() != null)
            {
                return new OrderByClip(left.ToString() + ", " + right.ToString());
            }
            else if (left.ToString() != null && right.ToString() == null)
            {
                return left;
            }
            else if (left.ToString() == null && right.ToString() != null)
            {
                return right;
            }
            else
            {
                return Default;
            }
        }

        /// <summary>
        /// Operator trues the specified right.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator true(OrderByClip right)
        {
            return false;
        }

        /// <summary>
        /// Operator falses the specified right.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator false(OrderByClip right)
        {
            return false;
        }

        /// <summary>
        /// Gets or sets the order by str.
        /// </summary>
        /// <value>The order by.</value>
        public string OrderBy
        {
            get
            {
                return orderByStr;
            }
            set
            {
                orderByStr = value;
            }
        }
    }

    /// <summary>
    /// Delegate used to map a property name to a column name.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns>The mapping column name.</returns>
    public delegate string PropertyToColumnMapHandler(string propertyName);

    /// <summary>
    /// special class used when updating a specified column with value of some other column or combination of several other columns' values.
    /// </summary>
    [Serializable]
    public class PropertyItemParam
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyItemParam"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PropertyItemParam(string value)
        {
            this.CustomValue = value;
        }

        /// <summary>
        /// custom value
        /// </summary>
        public string CustomValue;

        #region + - * / %

        /// <summary>
        /// Operator +s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator +(PropertyItemParam left, PropertyItemParam right)
        {
            return new PropertyItemParam(string.Format("({0}) + ({1})", left.CustomValue, right.CustomValue));
        }

        /// <summary>
        /// Operator +s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator +(object left, PropertyItemParam right)
        {
            Check.Require(left != null, "left could not be null.");

            return new PropertyItemParam(string.Format("{0} + ({1})", Util.FormatParamVal(left), right.CustomValue));
        }

        /// <summary>
        /// Operator +s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator +(PropertyItemParam left, object right)
        {
            Check.Require(right != null, "right could not be null.");

            return new PropertyItemParam(string.Format("({0}) + {1}", left.CustomValue, Util.FormatParamVal(right)));
        }

        /// <summary>
        /// Operator +s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator +(PropertyItem left, PropertyItemParam right)
        {
            return new PropertyItemParam(string.Format("{0} + ({1})", left.ColumnName, right.CustomValue));
        }

        /// <summary>
        /// Operator +s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator +(PropertyItemParam left, PropertyItem right)
        {
            return new PropertyItemParam(string.Format("({0}) + {1}", left.CustomValue, right.ColumnName));
        }

        /// <summary>
        /// Operator -s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator -(PropertyItemParam left, PropertyItemParam right)
        {
            return new PropertyItemParam(string.Format("({0}) - ({1})", left.CustomValue, right.CustomValue));
        }

        /// <summary>
        /// Operator -s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator -(object left, PropertyItemParam right)
        {
            Check.Require(left != null, "left could not be null.");

            return new PropertyItemParam(string.Format("{0} - ({1})", Util.FormatParamVal(left), right.CustomValue));
        }

        /// <summary>
        /// Operator -s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator -(PropertyItemParam left, object right)
        {
            Check.Require(right != null, "right could not be null.");

            return new PropertyItemParam(string.Format("({0}) - {1}", left.CustomValue, Util.FormatParamVal(right)));
        }

        /// <summary>
        /// Operator -s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator -(PropertyItem left, PropertyItemParam right)
        {
            return new PropertyItemParam(string.Format("{0} - ({1})", left.ColumnName, right.CustomValue));
        }

        /// <summary>
        /// Operator -s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator -(PropertyItemParam left, PropertyItem right)
        {
            return new PropertyItemParam(string.Format("({0}) - {1}", left.CustomValue, right.ColumnName));
        }

        /// <summary>
        /// Operator *s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator *(PropertyItemParam left, PropertyItemParam right)
        {
            return new PropertyItemParam(string.Format("({0}) * ({1})", left.CustomValue, right.CustomValue));
        }

        /// <summary>
        /// Operator *s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator *(object left, PropertyItemParam right)
        {
            Check.Require(left != null, "left could not be null.");

            return new PropertyItemParam(string.Format("{0} * ({1})", Util.FormatParamVal(left), right.CustomValue));
        }

        /// <summary>
        /// Operator *s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator *(PropertyItemParam left, object right)
        {
            Check.Require(right != null, "right could not be null.");

            return new PropertyItemParam(string.Format("({0}) * {1}", left.CustomValue, Util.FormatParamVal(right)));
        }

        /// <summary>
        /// Operator *s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator *(PropertyItem left, PropertyItemParam right)
        {
            return new PropertyItemParam(string.Format("{0} * ({1})", left.ColumnName, right.CustomValue));
        }

        /// <summary>
        /// Operator *s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator *(PropertyItemParam left, PropertyItem right)
        {
            return new PropertyItemParam(string.Format("({0}) * {1}", left.CustomValue, right.ColumnName));
        }

        /// <summary>
        /// Operator /s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator /(PropertyItemParam left, PropertyItemParam right)
        {
            return new PropertyItemParam(string.Format("({0}) / ({1})", left.CustomValue, right.CustomValue));
        }

        /// <summary>
        /// Operator /s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator /(object left, PropertyItemParam right)
        {
            Check.Require(left != null, "left could not be null.");

            return new PropertyItemParam(string.Format("{0} / ({1})", Util.FormatParamVal(left), right.CustomValue));
        }

        /// <summary>
        /// Operator /s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator /(PropertyItemParam left, object right)
        {
            Check.Require(right != null, "right could not be null.");

            return new PropertyItemParam(string.Format("({0}) / {1}", left.CustomValue, Util.FormatParamVal(right)));
        }

        /// <summary>
        /// Operator /s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator /(PropertyItem left, PropertyItemParam right)
        {
            return new PropertyItemParam(string.Format("{0} / ({1})", left.ColumnName, right.CustomValue));
        }

        /// <summary>
        /// Operator /s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator /(PropertyItemParam left, PropertyItem right)
        {
            return new PropertyItemParam(string.Format("({0}) / {1}", left.CustomValue, right.ColumnName));
        }

        /// <summary>
        /// Operator %s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator %(PropertyItemParam left, PropertyItemParam right)
        {
            return new PropertyItemParam(string.Format("({0}) % ({1})", left.CustomValue, right.CustomValue));
        }

        /// <summary>
        /// Operator %s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator %(object left, PropertyItemParam right)
        {
            Check.Require(left != null, "left could not be null.");

            return new PropertyItemParam(string.Format("{0} % ({1})", Util.FormatParamVal(left), right.CustomValue));
        }

        /// <summary>
        /// Operator %s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator %(PropertyItemParam left, object right)
        {
            Check.Require(right != null, "right could not be null.");

            return new PropertyItemParam(string.Format("({0}) % {1}", left.CustomValue, Util.FormatParamVal(right)));
        }

        /// <summary>
        /// Operator %s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator %(PropertyItem left, PropertyItemParam right)
        {
            return new PropertyItemParam(string.Format("{0} % ({1})", left.ColumnName, right.CustomValue));
        }

        /// <summary>
        /// Operator %s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static PropertyItemParam operator %(PropertyItemParam left, PropertyItem right)
        {
            return new PropertyItemParam(string.Format("({0}) % {1}", left.CustomValue, right.ColumnName));
        }

        #endregion

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        #region Equals and Not Equals

        /// <summary>
        /// Operator ==s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator ==(PropertyItemParam left, PropertyItemParam right)
        {
            if (((object)right) == null)
            {
                return new WhereClip(left.CustomValue + " IS NULL");
            }
            else if (((object)left) == null)
            {
                return new WhereClip(right.CustomValue + " IS NULL");
            }

            return new WhereClip(left.CustomValue + " = " + right.CustomValue);
        }

        /// <summary>
        /// Operator !=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator !=(PropertyItemParam left, PropertyItemParam right)
        {
            if (((object)right) == null)
            {
                return new WhereClip("( NOT " + left.CustomValue + " IS NULL)");
            }
            else if (((object)left) == null)
            {
                return new WhereClip("( NOT " + right.CustomValue + " IS NULL)");
            }

            return new WhereClip(left.CustomValue + " <> " + right.CustomValue);
        }

        /// <summary>
        /// Operator ==s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator ==(PropertyItemParam left, object right)
        {
            if (right == null)
            {
                return new WhereClip(left.CustomValue + " IS NULL");
            }

            return new WhereClip(left.CustomValue + " = " + Util.FormatParamVal(right));
        }

        /// <summary>
        /// Operator !=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator !=(PropertyItemParam left, object right)
        {
            if (right == null)
            {
                return new WhereClip("( NOT " + left.CustomValue + " IS NULL)");
            }

            return new WhereClip(left.CustomValue + " <> " + Util.FormatParamVal(right));
        }

        /// <summary>
        /// Operator ==s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator ==(object left, PropertyItemParam right)
        {
            if (left == null)
            {
                return new WhereClip(right.CustomValue + " IS NULL");
            }

            return new WhereClip(Util.FormatParamVal(left) + " = " + right.CustomValue);
        }

        /// <summary>
        /// Operator !=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator !=(object left, PropertyItemParam right)
        {
            if (left == null)
            {
                return new WhereClip("( NOT " + right.CustomValue + " IS NULL)");
            }

            return new WhereClip(Util.FormatParamVal(left) + " <> " + right.CustomValue);
        }

        /// <summary>
        /// Operator ==s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator ==(PropertyItemParam left, PropertyItem right)
        {
            if ((object)right == null)
            {
                return new WhereClip(left.CustomValue + " IS NULL");
            }

            return new WhereClip(left.CustomValue + " = " + right.ColumnName);
        }

        /// <summary>
        /// Operator !=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator !=(PropertyItemParam left, PropertyItem right)
        {
            if ((object)right == null)
            {
                return new WhereClip("( NOT " + left.CustomValue + " IS NULL)");
            }

            return new WhereClip(left.CustomValue + " <> " + right.ColumnName);
        }

        /// <summary>
        /// Operator ==s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator ==(PropertyItem left, PropertyItemParam right)
        {
            if ((object)left == null)
            {
                return new WhereClip(right.CustomValue + " IS NULL");
            }

            return new WhereClip(left.ColumnName + " = " + right.CustomValue);
        }

        /// <summary>
        /// Operator !=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator !=(PropertyItem left, PropertyItemParam right)
        {
            if ((object)left == null)
            {
                return new WhereClip("( NOT " + right.CustomValue + " IS NULL)");
            }

            return new WhereClip(left.ColumnName + " <> " + right.CustomValue);
        }

        #endregion

        #region Greater and Less

        /// <summary>
        /// Operator &gt;s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator >(PropertyItemParam left, PropertyItemParam right)
        {
            if (((object)right) == null)
            {
                return new WhereClip(left.CustomValue + " > NULL");
            }
            else if (((object)left) == null)
            {
                return new WhereClip(right.CustomValue + " < NULL");
            }
            return new WhereClip(left.CustomValue + " > " + right.CustomValue);
        }

        /// <summary>
        /// Operator &lt;s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator <(PropertyItemParam left, PropertyItemParam right)
        {
            if (((object)right) == null)
            {
                return new WhereClip(left.CustomValue + " < NULL");
            }
            else if (((object)left) == null)
            {
                return new WhereClip(right.CustomValue + " > NULL");
            }
            return new WhereClip(left.CustomValue + " < " + right.CustomValue);
        }

        /// <summary>
        /// Operator &gt;s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator >(PropertyItemParam left, object right)
        {
            return new WhereClip(left.CustomValue + " > " + Util.FormatParamVal(right));
        }

        /// <summary>
        /// Operator &lt;s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator <(PropertyItemParam left, object right)
        {
            return new WhereClip(left.CustomValue + " < " + Util.FormatParamVal(right));
        }

        /// <summary>
        /// Operator &gt;s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator >(object left, PropertyItemParam right)
        {
            return new WhereClip(Util.FormatParamVal(left) + " > " + right.CustomValue);
        }

        /// <summary>
        /// Operator &lt;s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator <(object left, PropertyItemParam right)
        {
            return new WhereClip(Util.FormatParamVal(left) + " < " + right.CustomValue);
        }

        /// <summary>
        /// Operator &gt;s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator >(PropertyItemParam left, PropertyItem right)
        {
            return new WhereClip(left.CustomValue + " > " + right.ColumnName);
        }

        /// <summary>
        /// Operator &lt;s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator <(PropertyItemParam left, PropertyItem right)
        {
            return new WhereClip(left.CustomValue + " < " + right.ColumnName);
        }

        /// <summary>
        /// Operator &gt;s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator >(PropertyItem left, PropertyItemParam right)
        {
            return new WhereClip(left.ColumnName + " > " + right.CustomValue);
        }

        /// <summary>
        /// Operator &lt;s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator <(PropertyItem left, PropertyItemParam right)
        {
            return new WhereClip(left.ColumnName + " < " + right.CustomValue);
        }

        #endregion

        #region Greater or Equals and Less and Equals

        /// <summary>
        /// Operator &gt;=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator >=(PropertyItemParam left, PropertyItemParam right)
        {
            if (((object)right) == null)
            {
                return new WhereClip(left.CustomValue + " >= NULL");
            }
            else if (((object)left) == null)
            {
                return new WhereClip(right.CustomValue + " <= NULL");
            }
            return new WhereClip(left.CustomValue + " >= " + right.CustomValue);
        }

        /// <summary>
        /// Operator &lt;=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator <=(PropertyItemParam left, PropertyItemParam right)
        {
            if (((object)right) == null)
            {
                return new WhereClip(left.CustomValue + " <= NULL");
            }
            else if (((object)left) == null)
            {
                return new WhereClip(right.CustomValue + " >= NULL");
            }
            return new WhereClip(left.CustomValue + " <= " + right.CustomValue);
        }

        /// <summary>
        /// Operator &gt;=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator >=(PropertyItemParam left, object right)
        {
            return new WhereClip(left.CustomValue + " >= " + Util.FormatParamVal(right));
        }

        /// <summary>
        /// Operator &lt;=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator <=(PropertyItemParam left, object right)
        {
            return new WhereClip(left.CustomValue + " <= " + Util.FormatParamVal(right));
        }

        /// <summary>
        /// Operator &gt;=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator >=(object left, PropertyItemParam right)
        {
            return new WhereClip(Util.FormatParamVal(left) + " >= " + right.CustomValue);
        }

        /// <summary>
        /// Operator &lt;=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator <=(object left, PropertyItemParam right)
        {
            return new WhereClip(Util.FormatParamVal(left) + " <= " + right.CustomValue);
        }

        /// <summary>
        /// Operator &gt;=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator >=(PropertyItemParam left, PropertyItem right)
        {
            return new WhereClip(left.CustomValue + " >= " + right.ColumnName);
        }

        /// <summary>
        /// Operator &lt;=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator <=(PropertyItemParam left, PropertyItem right)
        {
            return new WhereClip(left.CustomValue + " <= " + right.ColumnName);
        }

        /// <summary>
        /// Operator &gt;=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator >=(PropertyItem left, PropertyItemParam right)
        {
            return new WhereClip(left.ColumnName + " >= " + right.CustomValue);
        }

        /// <summary>
        /// Operator &lt;=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public static WhereClip operator <=(PropertyItem left, PropertyItemParam right)
        {
            return new WhereClip(left.ColumnName + " <= " + right.CustomValue);
        }

        #endregion

        #region Additional Operations

        /// <summary>
        /// Operator !s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <returns></returns>
        public static PropertyItemParam operator!(PropertyItemParam left)
        {
            return new PropertyItemParam(string.Format("(~({0}))", left.CustomValue));
        }

        /// <summary>
        /// Like operator the specified right.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns>A where clip.</returns>
        public WhereClip Like(string right)
        {
            Check.Require(right != null, "right could not be null.");

            return new WhereClip(this.CustomValue + " LIKE " + Util.FormatParamVal(right));
        }

        /// <summary>
        /// Betweens the specified left and right value.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public WhereClip Between(object left, object right)
        {
            Check.Require(left != null, "left could not be null.");
            Check.Require(right != null, "right could not be null.");

            return this >= left & this <= right;
        }

        /// <summary>
        /// Whether property value in the specified values.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <returns></returns>
        public WhereClip In(params object[] objs)
        {
            Check.Require(objs != null && objs.Length > 0, "objs could not be null or empty.");

            WhereClip retWhere = WhereClip.All;
            foreach (object obj in objs)
            {
                retWhere = retWhere | this == obj;
            }

            return retWhere;
        }

        /// <summary>
        /// Bitwises the and.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseAnd(object right)
        {
            return new PropertyItemParam(string.Format("({0}) & {1}", this.CustomValue, Util.FormatParamVal(right)));
        }

        /// <summary>
        /// Bitwises the and.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseAnd(PropertyItem right)
        {
            return new PropertyItemParam(string.Format("({0}) & {1}", this.CustomValue, right.ColumnName));
        }

        /// <summary>
        /// Bitwises the and.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseAnd(PropertyItemParam right)
        {
            return new PropertyItemParam(string.Format("({0}) & ({1})", this.CustomValue, right.CustomValue));
        }

        /// <summary>
        /// Bitwises the or.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseOr(object right)
        {
            return new PropertyItemParam(string.Format("({0}) | {1}", this.CustomValue, Util.FormatParamVal(right)));
        }

        /// <summary>
        /// Bitwises the or.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseOr(PropertyItem right)
        {
            return new PropertyItemParam(string.Format("({0}) | {1}", this.CustomValue, right.ColumnName));
        }

        /// <summary>
        /// Bitwises the or.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseOr(PropertyItemParam right)
        {
            return new PropertyItemParam(string.Format("({0}) | ({1})", this.CustomValue, right.CustomValue));
        }

        /// <summary>
        /// Bitwises the exclusive or.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseExclusiveOr(object right)
        {
            return new PropertyItemParam(string.Format("({0}) ^ {1}", this.CustomValue, Util.FormatParamVal(right)));
        }

        /// <summary>
        /// Bitwises the exclusive or.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseExclusiveOr(PropertyItem right)
        {
            return new PropertyItemParam(string.Format("({0}) ^ {1}", this.CustomValue, right.ColumnName));
        }

        /// <summary>
        /// Bitwises the exclusive or.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public PropertyItemParam BitwiseExclusiveOr(PropertyItemParam right)
        {
            return new PropertyItemParam(string.Format("({0}) ^ ({1})", this.CustomValue, right.CustomValue));
        }

        #endregion
    }

    /// <summary>
    /// Strong type query class for entity array
    /// </summary>
    /// <typeparam name="EntityType"></typeparam>
    public class EntityArrayQuery<EntityType>
        where EntityType : Entity, new()
    {
        #region Private Members

        private DataTable dt;
        private EntityConfiguration ec = new EntityType().GetEntityConfiguration();
        private Entity.QueryProxyHandler handler;

        private string ParseExpressionByMetaData(string sql)
        {
            return PropertyItem.ParseExpressionByMetaData(sql, new PropertyToColumnMapHandler(ec.GetMappingColumnName), "[", "]", "@");
        }

        private string[] DiscoverParams(string sql)
        {
            if (sql == null)
            {
                return null;
            }

            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"@([\w\d_]+)");
            System.Text.RegularExpressions.MatchCollection ms = r.Matches(sql);

            if (ms.Count == 0)
            {
                return null;
            }

            string[] paramNames = new string[ms.Count];
            for (int i = 0; i < ms.Count; i++)
            {
                paramNames[i] = ms[i].Value;
            }
            return paramNames;
        }

        private string ParseWhereClip(WhereClip where)
        {
            if (where == WhereClip.All)
            {
                return null;
            }

            object[] values = where.ParamValues;

            string sql = where.ToString();

            if (sql != null)
            {
                string[] whereNames = DiscoverParams(sql);

                if (whereNames != null && whereNames.Length > 0)
                {
                    for (int i = 0; i < whereNames.Length; i++)
                    {
                        if (values[i] != null && values[i] is PropertyItemParam)
                        {
                            PropertyItemParam pip = (PropertyItemParam)values[i];
                            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(whereNames[i] + @"(\s|,)");
                            sql = r.Replace(sql, pip.CustomValue + "$1");
                        }
                        else if (values[i] != null && values[i] is PropertyItem)
                        {
                            PropertyItem pi = (PropertyItem)values[i];
                            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(whereNames[i] + @"(\s|,)");
                            sql = r.Replace(sql, pi.ColumnName + "$1");
                        }
                        else
                        {
                            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(whereNames[i] + @"(\s|,)");
                            sql = r.Replace(sql, Util.FormatParamVal(values[i]) + "$1");
                        }
                    }
                }

                sql = ParseExpressionByMetaData(sql);
                sql = sql.Replace(" N'", " '");
            }

            return sql;
        }

        private EntityType CreateEntity()
        {
            EntityType obj = new EntityType();
            obj.SetQueryProxy(handler);
            obj.Attach();
            return obj;
        }
        
        private EntityType[] ToEntityArray(DataRow[] rows)
        {
            List<EntityType> list = new List<EntityType>();
            if (rows != null)
            {
                foreach (DataRow row in rows)
                {
                    EntityType retObj = CreateEntity();
                    retObj.SetPropertyValues(row);
                    list.Add(retObj);
                }
            }
            return list.ToArray();
        }

        private EntityType ToEntity(DataRow[] rows)
        {
            EntityType[] list = ToEntityArray(rows);
            return list.Length > 0 ? list[0] : null;
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

        private object FindScalar(string column, WhereClip where)
        {
            if (column.Contains("("))
            {
                //aggregate query
                if (column.StartsWith("COUNT(DISTINCT "))
                {
                    string columName = ParseExpressionByMetaData(column).Substring(15).Trim(' ', ')').Replace("[", string.Empty).Replace("]", string.Empty);
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
                    return dt.Compute(ParseExpressionByMetaData(column), ParseWhereClip(where));
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
                    rows = dt.Select(ParseWhereClip(where));
                }
                if (rows != null && rows.Length > 0)
                {
                    return rows[0][ParseExpressionByMetaData(column).TrimStart('[').TrimEnd(']')];
                }
            }

            return 0;
        }

        #endregion

        #region Public Members

        public EntityArrayQuery(EntityType[] arr)
        {
            Check.Require(arr != null && arr.Length > 0, "arr could not be null or empty.");

            dt = Entity.EntityArrayToDataTable<EntityType>(arr);
            handler = arr[0].onQuery;
        }

        /// <summary>
        /// Finds the specified entity.
        /// </summary>
        /// <param name="pkValues">The pk values.</param>
        /// <returns></returns>
        public EntityType Find(params object[] pkValues)
        {
            Check.Require(pkValues != null && pkValues.Length > 0, "pkValues could not be null or empty.");

            string[] pks = Entity.GetPrimaryKeyMappingColumnNames(ec);
            WhereClip where = BuildEqualWhereClip(pkValues, pks);

            return Find(where);
        }

        /// <summary>
        /// Finds the specified entity.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public EntityType Find(WhereClip where)
        {
            Check.Require(where != null, "where could not be null.");

            return ToEntity(dt.Select(ParseWhereClip(where)));
        }

        /// <summary>
        /// Existses the specified entity.
        /// </summary>
        /// <param name="pkValues">The pk values.</param>
        /// <returns></returns>
        public bool Exists(params object[] pkValues)
        {
            Check.Require(pkValues != null && pkValues.Length > 0, "pkValues could not be null or empty.");

            return Find(pkValues) != null;
        }

        /// <summary>
        /// Existses the specified entity.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public bool Exists(WhereClip where)
        {
            Check.Require(where != null, "where could not be null.");

            return Find(where) != null;
        }

        /// <summary>
        /// Finds all entities.
        /// </summary>
        /// <returns></returns>
        public EntityType[] FindArray()
        {
            return ToEntityArray(dt.Select());
        }

        /// <summary>
        /// Finds the array.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public EntityType[] FindArray(WhereClip where)
        {
            return ToEntityArray(dt.Select(ParseWhereClip(where)));
        }

        /// <summary>
        /// Finds the array.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns></returns>
        public EntityType[] FindArray(WhereClip where, OrderByClip orderBy)
        {
            return ToEntityArray(dt.Select(ParseWhereClip(where), ParseExpressionByMetaData(orderBy.ToString())));
        }

        /// <summary>
        /// Finds the array.
        /// </summary>
        /// <param name="orderBy">The order by.</param>
        /// <returns></returns>
        public EntityType[] FindArray(OrderByClip orderBy)
        {
            return ToEntityArray(dt.Select(null, ParseExpressionByMetaData(orderBy.ToString())));
        }

        /// <summary>
        /// Avgs the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public object Avg(PropertyItem property, WhereClip where)
        {
            return FindScalar(string.Format("AVG({0})", property.ColumnName), where);
        }

        /// <summary>
        /// Sums the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public object Sum(PropertyItem property, WhereClip where)
        {
            return FindScalar(string.Format("SUM({0})", property.ColumnName), where);
        }

        /// <summary>
        /// MAXs the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public object Max(PropertyItem property, WhereClip where)
        {
            return FindScalar(string.Format("MAX({0})", property.ColumnName), where);
        }

        /// <summary>
        /// MINs the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public object Min(PropertyItem property, WhereClip where)
        {
            return FindScalar(string.Format("MIN({0})", property.ColumnName), where);
        }

        /// <summary>
        /// Counts the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="where">The where.</param>
        /// <param name="isDistinct">if set to <c>true</c> [is distinct].</param>
        /// <returns></returns>
        public object Count(PropertyItem property, WhereClip where, bool isDistinct)
        {
            return FindScalar(string.Format("COUNT({1}{0})", property.ColumnName, isDistinct ? "DISTINCT " : string.Empty), where);
        }

        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return dt.Rows.Count;
        }

        #endregion
    }
}
