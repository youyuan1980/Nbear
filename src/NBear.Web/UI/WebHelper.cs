using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Resources;

namespace NBear.Web.UI
{
    /// <summary>
    /// The web helper class contains shared functions used by page and user controls, as base class of Page and UserControl.
    /// </summary>
    public class WebHelper
    {
        #region Helper Methods

        /// <summary>
        /// Gets the string param.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="errorReturn">The error return.</param>
        /// <returns>The param value.</returns>
        public static string GetStringParam(System.Web.HttpRequest request, string paramName, string errorReturn)
        {
            string retStr = request.Form[paramName];
            if (retStr == null)
            {
                retStr = request.QueryString[paramName];
            }
            if (retStr == null)
            {
                return errorReturn;
            }
            return retStr;
        }

        /// <summary>
        /// Gets the int param.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="errorReturn">The error return.</param>
        /// <returns>The param value.</returns>
        public static int GetIntParam(System.Web.HttpRequest request, string paramName, int errorReturn)
        {
            string retStr = request.Form[paramName];
            if (retStr == null)
            {
                retStr = request.QueryString[paramName];
            }
            if (retStr == null || retStr.Trim() == string.Empty)
            {
                return errorReturn;
            }
            try
            {
                return Convert.ToInt32(retStr);
            }
            catch
            {
                return errorReturn;
            }
        }

        /// <summary>
        /// Gets the date time param.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="errorReturn">The error return.</param>
        /// <returns>The param value.</returns>
        public static DateTime GetDateTimeParam(System.Web.HttpRequest request, string paramName, DateTime errorReturn)
        {
            string retStr = request.Form[paramName];
            if (retStr == null)
            {
                retStr = request.QueryString[paramName];
            }
            if (retStr == null || retStr.Trim() == string.Empty)
            {
                return errorReturn;
            }
            try
            {
                return Convert.ToDateTime(retStr);
            }
            catch
            {
                return errorReturn;
            }
        }

        /// <summary>
        /// Strongs the typed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The strong typed instance.</returns>
        public static ObjectType StrongTyped<ObjectType>(object obj)
        {
            return (ObjectType)obj;
        }

        /// <summary>
        /// Toes the js single quote safe string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The formated str.</returns>
        public static string ToJsSingleQuoteSafeString(string str)
        {
            return str.Replace("'", "\\'");
        }

        /// <summary>
        /// Toes the js double quote safe string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The formated str.</returns>
        public static string ToJsDoubleQuoteSafeString(string str)
        {
            return str.Replace("\"", "\\\"");
        }

        /// <summary>
        /// Toes the VBS quote safe string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The formated str.</returns>
        public static string ToVbsQuoteSafeString(string str)
        {
            return str.Replace("\"", "\"\"");
        }

        /// <summary>
        /// Toes the SQL quote safe string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The formated str.</returns>
        public static string ToSqlQuoteSafeString(string str)
        {
            return str.Replace("'", "''");
        }

        /// <summary>
        /// Texts to HTML.
        /// </summary>
        /// <param name="txtStr">The TXT STR.</param>
        /// <returns>The formated str.</returns>
        public static string TextToHtml(string txtStr)
        {
            return txtStr.Replace(" ", "&nbsp;").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;").
                Replace("<", "&lt;").Replace(">", "&gt;").Replace("\r", "").Replace("\n", "<br />");
        }

        #endregion

        #region Resource

        private static Dictionary<string, Hashtable> stringResources = new Dictionary<string, Hashtable>();

        private static System.Globalization.CultureInfo defaultCulture = null;

        /// <summary>
        /// Gets or sets the default culture.
        /// </summary>
        /// <value>The default culture.</value>
        public static System.Globalization.CultureInfo DefaultCulture
        {
            get
            {
                return defaultCulture ?? System.Threading.Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                defaultCulture = value;
            }
        }

        /// <summary>
        /// Loads the resources.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="ci">The ci.</param>
        public static void LoadResources(string resourceName, System.Globalization.CultureInfo ci)
        {
            string resFileName = System.Web.HttpRuntime.BinDirectory + resourceName + "." + ci.ToString() + ".resources";
            if (System.IO.File.Exists(resFileName))
            {
                lock (stringResources)
                {
                    if (!stringResources.ContainsKey(ci.ToString()))
                    {
                        stringResources.Add(ci.ToString(), new Hashtable());

                        try
                        {
                            ResourceReader reader = new ResourceReader(resFileName);
                            IDictionaryEnumerator en = reader.GetEnumerator();
                            while (en.MoveNext())
                            {
                                stringResources[ci.ToString()].Add(en.Key, en.Value);
                            }
                            reader.Close();
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads the resources.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        public static void LoadResources(string resourceName)
        {
            LoadResources(resourceName, DefaultCulture);
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The resouce value.</returns>
        public static string GetString(string key)
        {
            return GetString(key, WebHelper.DefaultCulture);
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="ci">The ci.</param>
        /// <returns>The resouce value.</returns>
        public static string GetString(string key, System.Globalization.CultureInfo ci)
        {
            if (stringResources.ContainsKey(ci.ToString()))
            {
                if (stringResources[ci.ToString()].Contains(key))
                {
                    return stringResources[ci.ToString()][key].ToString();
                }
            }

            return string.Empty;
        }

        #endregion

        #region ClientScriptFactoryHelper

        /// <summary>
        /// Common Client Script
        /// </summary>
        public class ClientScriptFactoryHelper
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ClientScriptFactoryHelper"/> class.
            /// </summary>
            public ClientScriptFactoryHelper()
            {
            }

            #endregion

            /// <summary>
            /// Wraps the script tag.
            /// </summary>
            /// <param name="scripts">The scripts.</param>
            /// <returns>The script.</returns>
            public string WrapScriptTag(params string[] scripts)
            {
                if (scripts != null && scripts.Length > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("\r\n<script language=\"javascript\" type=\"text/javascript\">\r\n<!--\r\n");

                    foreach (string script in scripts)
                    {
                        sb.Append(script.EndsWith(";") || script.EndsWith("}") ? script : script + ";");
                    }

                    sb.Append("\r\n//-->\r\n</script>\r\n");
                    return sb.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            /// <summary>
            /// Pops the alert.
            /// </summary>
            /// <param name="msg">The MSG.</param>
            /// <returns>The script.</returns>
            public string PopAlert(string msg)
            {
                return string.Format(" window.alert('{0}'); ", ToJsSingleQuoteSafeString(msg));
            }

            /// <summary>
            /// Pops the confirm.
            /// </summary>
            /// <param name="msg">The MSG.</param>
            /// <returns>The script.</returns>
            public string PopConfirm(string msg)
            {
                return string.Format(" window.confirm('{0}') ", ToJsSingleQuoteSafeString(msg));
            }

            /// <summary>
            /// Pops the prompt.
            /// </summary>
            /// <param name="msg">The MSG.</param>
            /// <param name="defaultValue">The default value.</param>
            /// <returns>The script.</returns>
            public string PopPrompt(string msg, string defaultValue)
            {
                return string.Format(" window.prompt('{0}', '{1}') ", ToJsSingleQuoteSafeString(msg), ToJsSingleQuoteSafeString(defaultValue));
            }

            /// <summary>
            /// Closes the self.
            /// </summary>
            /// <returns>The script.</returns>
            public string CloseSelf()
            {
                return " window.close(); ";
            }

            /// <summary>
            /// Closes the parent.
            /// </summary>
            /// <returns>The script.</returns>
            public string CloseParent()
            {
                return " if (window.parent) { window.parent.close(); } ";
            }

            /// <summary>
            /// Closes the opener.
            /// </summary>
            /// <returns>The script.</returns>
            public string CloseOpener()
            {
                return " if (window.opener) { window.opener.close(); } ";
            }

            /// <summary>
            /// Refreshes the self.
            /// </summary>
            /// <returns>The script.</returns>
            public string RefreshSelf()
            {
                return " window.location += ' '; ";
            }

            /// <summary>
            /// Refreshes the opener.
            /// </summary>
            /// <returns>The script.</returns>
            public string RefreshOpener()
            {
                return " if (window.opener) { window.opener.location += ' '; } ";
            }

            /// <summary>
            /// Refreshes the parent.
            /// </summary>
            /// <returns>The script.</returns>
            public string RefreshParent()
            {
                return " if (window.parent) { window.parent.location += ' '; } ";
            }

            /// <summary>
            /// Shows the modal dialog.
            /// </summary>
            /// <param name="url">The URL.</param>
            /// <param name="status">if set to <c>true</c> [status].</param>
            /// <param name="resizable">if set to <c>true</c> [resizable].</param>
            /// <param name="height">The height.</param>
            /// <param name="width">The width.</param>
            /// <param name="top">The top.</param>
            /// <param name="left">The left.</param>
            /// <param name="scroll">if set to <c>true</c> [scroll].</param>
            /// <returns>The script.</returns>
            public string ShowModalDialog(string url, bool status, bool resizable, int height, int width, int top, int left, bool scroll)
            {
                return string.Format(" window.showModalDialog('{0}', window, 'status={1},resizable={2},dialogHeight={3}px,dialogWidth={4}px,dialogTop={5},dialogLeft={6},scroll={7},unadorne=yes'); ", 
                    ToJsSingleQuoteSafeString(url), (status ? 1 : 0), (resizable ? 1 : 0), height, width, top, left, (scroll ? 1 : 0));
            }

            /// <summary>
            /// Shows the modal dialog.
            /// </summary>
            /// <param name="url">The URL.</param>
            /// <param name="status">if set to <c>true</c> [status].</param>
            /// <param name="resizable">if set to <c>true</c> [resizable].</param>
            /// <param name="height">The height.</param>
            /// <param name="width">The width.</param>
            /// <param name="center">if set to <c>true</c> [center].</param>
            /// <param name="scroll">if set to <c>true</c> [scroll].</param>
            /// <returns>The script.</returns>
            public string ShowModalDialog(string url, bool status, bool resizable, int height, int width, bool center, bool scroll)
            {
                return string.Format(" window.showModalDialog('{0}', window, 'status={1},resizable={2},dialogHeight={3}px,dialogWidth={4}px,center={5},scroll={6},unadorne=yes'); ", 
                    ToJsSingleQuoteSafeString(url), (status ? 1 : 0), (resizable ? 1 : 0), height, width, (center ? 1 : 0), (scroll ? 1 : 0));
            }

            /// <summary>
            /// Shows the modeless dialog.
            /// </summary>
            /// <param name="url">The URL.</param>
            /// <param name="status">if set to <c>true</c> [status].</param>
            /// <param name="resizable">if set to <c>true</c> [resizable].</param>
            /// <param name="height">The height.</param>
            /// <param name="width">The width.</param>
            /// <param name="top">The top.</param>
            /// <param name="left">The left.</param>
            /// <param name="scroll">if set to <c>true</c> [scroll].</param>
            /// <returns>The script.</returns>
            public string ShowModelessDialog(string url, bool status, bool resizable, int height, int width, int top, int left, bool scroll)
            {
                return string.Format(" window.showModelessDialog('{0}', window, 'status={1},resizable={2},dialogHeight={3}px,dialogWidth={4}px,dialogTop={5},dialogLeft={6},scroll={7},unadorne=yes'); ", 
                    ToJsSingleQuoteSafeString(url), (status ? 1 : 0), (resizable ? 1 : 0), height, width, top, left, (scroll ? 1 : 0));
            }

            /// <summary>
            /// Shows the modeless dialog.
            /// </summary>
            /// <param name="url">The URL.</param>
            /// <param name="status">if set to <c>true</c> [status].</param>
            /// <param name="resizable">if set to <c>true</c> [resizable].</param>
            /// <param name="height">The height.</param>
            /// <param name="width">The width.</param>
            /// <param name="center">if set to <c>true</c> [center].</param>
            /// <param name="scroll">if set to <c>true</c> [scroll].</param>
            /// <returns>The script.</returns>
            public string ShowModelessDialog(string url, bool status, bool resizable, int height, int width, bool center, bool scroll)
            {
                return string.Format(" window.showModelessDialog('{0}', window, 'status={1},resizable={2},dialogHeight={3}px,dialogWidth={4}px,center={5},scroll={6},unadorne=yes'); ", 
                    ToJsSingleQuoteSafeString(url), (status ? 1 : 0), (resizable ? 1 : 0), height, width, (center ? 1 : 0), (scroll ? 1 : 0));
            }

            /// <summary>
            /// Selfs the go back.
            /// </summary>
            /// <returns>The script.</returns>
            public string SelfGoBack()
            {
                return " window.history.back(); ";
            }

            /// <summary>
            /// Parents the go back.
            /// </summary>
            /// <returns>The script.</returns>
            public string ParentGoBack()
            {
                return " if (window.parent) { window.parent.history.back(); } ";
            }

            /// <summary>
            /// Openers the go back.
            /// </summary>
            /// <returns>The script.</returns>
            public string OpenerGoBack()
            {
                return " if (window.opener) { window.opener.history.back(); } ";
            }

            /// <summary>
            /// Opens the specified URL.
            /// </summary>
            /// <param name="url">The URL.</param>
            /// <param name="frameName">Name of the frame.</param>
            /// <param name="status">if set to <c>true</c> [status].</param>
            /// <param name="location">if set to <c>true</c> [location].</param>
            /// <param name="menubar">if set to <c>true</c> [menubar].</param>
            /// <param name="resizable">if set to <c>true</c> [resizable].</param>
            /// <param name="height">The height.</param>
            /// <param name="width">The width.</param>
            /// <param name="top">The top.</param>
            /// <param name="left">The left.</param>
            /// <param name="scrollbars">if set to <c>true</c> [scrollbars].</param>
            /// <param name="toolbar">if set to <c>true</c> [toolbar].</param>
            /// <returns>The script.</returns>
            public string Open(string url, string frameName, bool status, bool location, bool menubar, 
                bool resizable, int height, int width, int top, int left, bool scrollbars, bool toolbar)
            {
                return string.Format(" window.open('{0}', '{1}', 'status={2},location={3},menubar={4},resizable={5},height={6}px,width={7}px,top={8},left={9},scrollbars={10},toolbar={11}'); ", 
                    ToJsSingleQuoteSafeString(url), ToJsSingleQuoteSafeString(frameName), (status ? 1 : 0), (location ? 1 : 0), (menubar ? 1 : 0), (resizable ? 1 : 0), height, width, top, left, (scrollbars ? 1 : 0), (toolbar ? 1 : 0));
            }

            /// <summary>
            /// Opens the specified URL.
            /// </summary>
            /// <param name="url">The URL.</param>
            /// <param name="frameName">Name of the frame.</param>
            /// <returns>The script.</returns>
            public string Open(string url, string frameName)
            {
                return string.Format(" window.open('{0}', '{1}'); ", ToJsSingleQuoteSafeString(url), ToJsSingleQuoteSafeString(frameName));
            }

            /// <summary>
            /// Calls the client validator.
            /// </summary>
            /// <param name="prefix">The prefix.</param>
            /// <param name="validators">The validators.</param>
            /// <returns>The script.</returns>
            protected string CallClientValidator(string prefix, params System.Web.UI.WebControls.BaseValidator[] validators)
            {
                if (validators != null && validators.Length > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (System.Web.UI.WebControls.BaseValidator validator in validators)
                    {
                        sb.Append(string.Format(" ValidatorValidate({1}{0}); ", validator.ID, prefix));
                    }
                    return sb.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            /// <summary>
            /// Toes the js string array.
            /// </summary>
            /// <param name="strs">The STRS.</param>
            /// <returns>The script.</returns>
            public string ToJsStringArray(params string[] strs)
            {
                if (strs != null && strs.Length > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(" new Array(");

                    foreach (string str in strs)
                    {
                        sb.Append(string.Format("'{0}', ", str.Replace("'", "\\'")));
                    }

                    return sb.ToString().TrimEnd(',', ' ') + ");";
               }
                else
                {
                    return " new Array;";
                }
            }
        }

        #endregion
    }
}
