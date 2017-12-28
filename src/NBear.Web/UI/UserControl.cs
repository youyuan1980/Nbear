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
    /// User Control base class.
    /// </summary>
    public class UserControl : System.Web.UI.UserControl
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
        /// UserControlClientScriptFactory
        /// </summary>
        protected sealed class UserControlClientScriptFactory : WebHelper.ClientScriptFactoryHelper
        {
            /// <summary>
            /// MasterPage control id Prefix
            /// </summary>
            public static string MasterPagePrefix = "ctl00_";

            /// <summary>
            /// The user control.
            /// </summary>
            UserControl ctl;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:UserControlClientScriptFactory"/> class.
            /// </summary>
            /// <param name="ctl">The CTL.</param>
            public UserControlClientScriptFactory(UserControl ctl)
                : base()
            {
                this.ctl = ctl;
            }

            /// <summary>
            /// Calls the client validator.
            /// </summary>
            /// <param name="validators">The validators.</param>
            /// <returns>The client side script.</returns>
            public string CallClientValidator(params System.Web.UI.WebControls.BaseValidator[] validators)
            {
                return base.CallClientValidator(ctl.ID + "_", validators);
            }

            /// <summary>
            /// Calls the client validator.
            /// </summary>
            /// <param name="contentPlaceHolder">The content place holder.</param>
            /// <param name="validators">The validators.</param>
            /// <returns>The client side script.</returns>
            public new string CallClientValidator(string contentPlaceHolder, params System.Web.UI.WebControls.BaseValidator[] validators)
            {
                return base.CallClientValidator(MasterPagePrefix + contentPlaceHolder + "_" + ctl.ID + "_", validators);
            }
        }

        /// <summary>
        /// A ClientScriptFactory instance.
        /// </summary>
        protected UserControlClientScriptFactory ClientScriptFactory;

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ClientScriptFactory = new UserControlClientScriptFactory(this);
        }

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
        /// A AjaxManager instance.
        /// </summary>
        Page.AjaxManager Ajax = new Page.AjaxManager();

        #endregion
    }
}
