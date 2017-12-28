using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Resources;

namespace NBear.Web.UI
{
    /// <summary>
    /// The MasterPage base class.
    /// </summary>
    public class MasterPage : System.Web.UI.MasterPage
    {
        #region Helper Methods

        /// <summary>
        /// Gets the string param.
        /// </summary>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="errorReturn">The error return.</param>
        /// <returns>The param value.</returns>
        protected string GetStringParam(string paramName, string errorReturn)
        {
            return WebHelper.GetStringParam(Request, paramName, errorReturn);
        }

        /// <summary>
        /// Gets the int param.
        /// </summary>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="errorReturn">The error return.</param>
        /// <returns>The param value.</returns>
        protected int GetIntParam(string paramName, int errorReturn)
        {
            return WebHelper.GetIntParam(Request, paramName, errorReturn);
        }

        /// <summary>
        /// Gets the date time param.
        /// </summary>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="errorReturn">The error return.</param>
        /// <returns>The param value.</returns>
        protected DateTime GetDateTimeParam(string paramName, DateTime errorReturn)
        {
            return WebHelper.GetDateTimeParam(Request, paramName, errorReturn);
        }

        /// <summary>
        /// Strongs the typed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The strong typed instance.</returns>
        protected static ObjectType StrongTyped<ObjectType>(object obj)
        {
            return WebHelper.StrongTyped<ObjectType>(obj);
        }

        /// <summary>
        /// Toes the js single quote safe string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The formated str.</returns>
        protected static string ToJsSingleQuoteSafeString(string str)
        {
            return WebHelper.ToJsSingleQuoteSafeString(str);
        }

        /// <summary>
        /// Toes the js double quote safe string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The formated str.</returns>
        protected static string ToJsDoubleQuoteSafeString(string str)
        {
            return WebHelper.ToJsDoubleQuoteSafeString(str);
        }

        /// <summary>
        /// Toes the VBS quote safe string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The formated str.</returns>
        protected static string ToVbsQuoteSafeString(string str)
        {
            return WebHelper.ToVbsQuoteSafeString(str);
        }

        /// <summary>
        /// Toes the SQL quote safe string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The formated str.</returns>
        protected static string ToSqlQuoteSafeString(string str)
        {
            return WebHelper.ToSqlQuoteSafeString(str);
        }

        /// <summary>
        /// Texts to HTML.
        /// </summary>
        /// <param name="txtStr">The TXT STR.</param>
        /// <returns>The formated str.</returns>
        protected static string TextToHtml(string txtStr)
        {
            return WebHelper.TextToHtml(txtStr);
        }

        #endregion

        #region ClientScriptFactoryHelper

        /// <summary>
        /// MasterPageClientScriptFactory
        /// </summary>
        protected sealed class MasterPageClientScriptFactory : WebHelper.ClientScriptFactoryHelper
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="T:MasterPageClientScriptFactory"/> class.
            /// </summary>
            public MasterPageClientScriptFactory()
                : base()
            {
            }

            /// <summary>
            /// Calls the client validator.
            /// </summary>
            /// <param name="validators">The validators.</param>
            /// <returns>The client side script.</returns>
            public string CallClientValidator(params System.Web.UI.WebControls.BaseValidator[] validators)
            {
                return CallClientValidator("ctl00_", validators);
            }
        }

        /// <summary>
        /// A ClientScriptFactory instance.
        /// </summary>
        protected MasterPageClientScriptFactory ClientScriptFactory = new MasterPageClientScriptFactory();

        #endregion

        #region Resource

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The resource value.</returns>
        protected static string GetString(string key)
        {
            return WebHelper.GetString(key);
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="ci">The ci.</param>
        /// <returns>The resource value.</returns>
        protected static string GetString(string key, System.Globalization.CultureInfo ci)
        {
            return WebHelper.GetString(key, ci);
        }

        #endregion

        #region Ajax

        /// <summary>
        /// An AjaxManager Instance.
        /// </summary>
        Page.AjaxManager Ajax = new Page.AjaxManager();

        #endregion
    }
}
