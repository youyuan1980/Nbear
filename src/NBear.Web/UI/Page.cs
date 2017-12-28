using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Resources;
using System.Configuration;
using System.Web.UI;

namespace NBear.Web.UI
{
    /// <summary>
    /// Page base class.
    /// </summary>
    public class Page : System.Web.UI.Page, ICallbackEventHandler
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
        /// PageClientScriptFactory
        /// </summary>
        protected sealed class PageClientScriptFactory : WebHelper.ClientScriptFactoryHelper
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="T:PageClientScriptFactory"/> class.
            /// </summary>
            public PageClientScriptFactory()
                : base()
            {
            }

            /// <summary>
            /// Calls the client validator.
            /// </summary>
            /// <param name="contentPlaceHolder">The content place holder.</param>
            /// <param name="validators">The validators.</param>
            /// <returns>The formated script.</returns>
            public new string CallClientValidator(string contentPlaceHolder, params System.Web.UI.WebControls.BaseValidator[] validators)
            {
                return base.CallClientValidator("_ctl0_" + contentPlaceHolder + "_", validators);
            }

            /// <summary>
            /// Calls the client validator.
            /// </summary>
            /// <param name="validators">The validators.</param>
            /// <returns>The formated script.</returns>
            public string CallClientValidator(params System.Web.UI.WebControls.BaseValidator[] validators)
            {
                return base.CallClientValidator(null, validators);
            }
        }

        /// <summary>
        /// A ClientScriptFactory instance.
        /// </summary>
        protected PageClientScriptFactory ClientScriptFactory = new PageClientScriptFactory();

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

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

        #region ICallbackEventHandler Members

        private Dictionary<string, string> callbackParams = new Dictionary<string, string>();

        /// <summary>
        /// Returns the results of a callback event that targets a control.
        /// </summary>
        /// <returns>The result of the callback.</returns>
        public string GetCallbackResult()
        {
            StringBuilder sb = new StringBuilder();
            Control ctl = LoadControl(callbackParams["AjaxTemplate"].ToLower().EndsWith(".ascx") ? 
                callbackParams["AjaxTemplate"] : callbackParams["AjaxTemplate"] + ".ascx");
            if (ctl != null && (!(ctl.GetType().IsSubclassOf(typeof(NBear.Web.UI.AjaxTemplate)))))
            {
                ctl = null;
            }
            if (ctl != null)
            {
                ((NBear.Web.UI.AjaxTemplate)ctl).OnInit(null);
                ((NBear.Web.UI.AjaxTemplate)ctl).OnAjaxTemplatePreRender(callbackParams);
                ctl.RenderControl(new HtmlTextWriter(new System.IO.StringWriter(sb)));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Processes a callback event that targets a control.
        /// </summary>
        /// <param name="eventArgument">A string that represents an event argument to pass to the event handler.</param>
        public void RaiseCallbackEvent(string eventArgument)
        {
            if (string.IsNullOrEmpty(eventArgument))
            {
                return;
            }
            string[] keyValues = Server.UrlDecode(eventArgument).Split('&');
            if (keyValues != null && keyValues.Length > 0)
            {
                foreach (string keyValueString in keyValues)
                {
                    string[] keyValue = keyValueString.Split('=');
                    if (callbackParams.ContainsKey(keyValue[0]))
                    {
                        callbackParams[keyValue[0]] += (keyValue.Length > 1 ? keyValue[1] : keyValue[0]);
                    }
                    else
                    {
                        callbackParams.Add(keyValue[0], (keyValue.Length > 1 ? keyValue[1] : keyValue[0]));
                    }
                }
            }
        }

        #endregion

        #region Ajax

        /// <summary>
        /// Gets a value indicating whether [enable ajax callback].
        /// </summary>
        /// <value><c>true</c> if [enable ajax callback]; otherwise, <c>false</c>.</value>
        protected virtual bool EnableAjaxCallback
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Page.PreLoad"></see> event after postback data is loaded into the page server controls but before the <see cref="M:System.Web.UI.Control.OnLoad(System.EventArgs)"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);

            if (EnableAjaxCallback)
            {
                string clientAjaxScriptBlock = string.Concat(
                    "<script language=\"javascript\" type=\"text/javascript\">\r\n",
                    "    var Ajax = new Object;\r\n",
                    "    Ajax.DoUpdate = function(content, outputAndOnComplete)\r\n",
                    "    {\r\n",
                    "       var output = outputAndOnComplete.split('|')[0] == 'null' ? null : outputAndOnComplete.split('|')[0]; \r\n",
                    "       var onComplete = outputAndOnComplete.split('|')[1] == 'null' ? null : outputAndOnComplete.split('|')[1]; \r\n",
                    "       if (document.getElementById(output).innerHTML != undefined) \r\n",
                    "       { \r\n",
                    "           document.getElementById(output).innerHTML = content; \r\n",
                    "           for (var i = 0; i < document.getElementById(output).childNodes.length; i++)\r\n",
                    "           {\r\n",
                    "               if (document.getElementById(output).childNodes.item(i).tagName && document.getElementById(output).childNodes.item(i).tagName == 'SCRIPT')\r\n",
                    "               {\r\n",
                    "                   var oScriptNode = document.createElement('script');\r\n",
                    "                   oScriptNode.text = document.getElementById(output).childNodes.item(i).text;\r\n",
                    "                   oScriptNode.language = document.getElementById(output).childNodes.item(i).language;\r\n",
                    "                   oScriptNode.type = document.getElementById(output).childNodes.item(i).type;\r\n",
                    "                   oScriptNode.src = document.getElementById(output).childNodes.item(i).src;\r\n",
                    "                   document.body.appendChild(oScriptNode);\r\n",
                    "               }\r\n",
                    "           }\r\n",
                    "        }\r\n",
                    "        if (onComplete != null) eval(onComplete)(content);\r\n",
                    "    };\r\n",
                    "    Ajax.Callback = function(ajaxTemplate, params, onComplete)\r\n",
                    "    {\r\n",
                    "        " + ClientScript.GetCallbackEventReference(this, "'AjaxTemplate=' + ajaxTemplate + '&' + params", "onComplete != null && onComplete != '' ? eval(onComplete) : null", null) + "\r\n",
                    "    };\r\n",
                    "    Ajax.Update = function(ajaxTemplate, output, params, onComplete)\r\n",
                    "    {\r\n",
                    "        " + ClientScript.GetCallbackEventReference(this, "'AjaxTemplate=' + ajaxTemplate + '&' + params", "Ajax.DoUpdate", "(output == null || output == '' ? 'null' : output) + '|' + (onComplete == null || onComplete == '' ? 'null' : onComplete)") + "\r\n",
                    "    };\r\n",
                    "    Ajax.Params = new Array();\r\n",
                    "    //parse ajax params after '#' in the url to Ajax.Params\r\n",
                    "    var _paramStr = new String(window.location.href);\r\n",
                    "    var _sharpPos = _paramStr.indexOf('#');\r\n",
                    "    if (_sharpPos >= 0 && _sharpPos < _paramStr.length - 1)\r\n",
                    "    {\r\n",
                    "        _paramStr = _paramStr.substring(_sharpPos + 1, _paramStr.length);\r\n",
                    "    }\r\n",
                    "    else\r\n",
                    "    {\r\n",
                    "        _paramStr = '';\r\n",
                    "    }\r\n",
                    "    if (_paramStr.length > 0)\r\n",
                    "    {\r\n",
                    "        var _paramArr = _paramStr.split('&');\r\n",
                    "        for (var i = 0; i < _paramArr.length; i++)\r\n",
                    "        {\r\n",
                    "            if (_paramArr[i].indexOf('=') >= 0)\r\n",
                    "            {\r\n",
                    "                var _paramKeyVal = _paramArr[i].split('=');\r\n",
                    "                Ajax.Params[_paramKeyVal[0]] = _paramKeyVal[1];\r\n",
                    "            }\r\n",
                    "            else\r\n",
                    "            {\r\n",
                    "                Ajax.Params[_paramArr[i]] = _paramArr[i];\r\n",
                    "            }\r\n",
                    "        }\r\n",
                    "    }\r\n",
                    "    Ajax.Params.toString = function()\r\n",
                    "    {\r\n",
                    "	    var _retStr = _paramStr;\r\n",
                    "	    var _andPos = _retStr.indexOf('&');\r\n",
                    "	    if (_andPos > 0  && _andPos != _retStr.length)\r\n",
                    "	    {\r\n",
                    "		    _retStr = _retStr.substring(_andPos + 1, _retStr.length);\r\n",
                    "	    }\r\n",
                    "	    _andPos = _retStr.indexOf('&');\r\n",
                    "	    if (_andPos > 0  && _andPos != _retStr.length)\r\n",
                    "	    {\r\n",
                    "		    _retStr = _retStr.substring(_andPos + 1, _retStr.length);\r\n",
                    "	    }\r\n",
                    "	    else\r\n",
                    "	    {\r\n",
                    "		    _retStr = null;\r\n",
                    "	    }\r\n",
                    "	    return _retStr;\r\n",
                    "    };\r\n",
                    "    Ajax.SetLocation = function(ajaxTemplate, output, params, onComplete)\r\n",
                    "    {\r\n",
                    "        document.location = '#ajaxTemplate=' + ajaxTemplate + (output != null && output != '' ? '&output=' + output : '') + (onComplete != null && onComplete != '' ? '&onComplete=' + onComplete : '') + (params != null && params != '' ? '&' + params : '');\r\n",
                    "    };\r\n",
                    "    Ajax.Callback2 = function(ajaxTemplate, params, onComplete)\r\n",
                    "    {\r\n",
                    "        Ajax.SetLocation(ajaxTemplate, null, params, onComplete);\r\n",
                    "        Ajax.Callback(ajaxTemplate, params, eval(onComplete));\r\n",
                    "    };\r\n",
                    "    Ajax.Update2 = function(ajaxTemplate, output, params, onComplete)\r\n",
                    "    {\r\n",
                    "        Ajax.SetLocation(ajaxTemplate, output, params, onComplete);\r\n",
                    "        Ajax.Update(ajaxTemplate, output, params, onComplete);\r\n",
                    "    };\r\n",
                    "    Ajax.OnPageLoad = function ()\r\n",
                    "    {\r\n",
                    "	    if (Ajax.Params['ajaxTemplate'] != null && Ajax.Params['ajaxTemplate'] != '')\r\n",
                    "	    {\r\n",
                    "	        if (Ajax.Params['output'] != null && Ajax.Params['output'] != '')\r\n",
                    "	        {\r\n",
                    "		        Ajax.Update(Ajax.Params['ajaxTemplate'], Ajax.Params['output'], Ajax.Params.toString(), Ajax.Params['onComplete']);\r\n",
                    "	        }\r\n",
                    "	        else if (Ajax.Params['onComplete'] != null && Ajax.Params['onComplete'] != '')\r\n",
                    "	        {\r\n",
                    "		        Ajax.Callback(Ajax.Params['ajaxTemplate'], Ajax.Params.toString(), eval(Ajax.Params['onComplete']));\r\n",
                    "	        }\r\n",
                    "	    }\r\n",
                    "    };    \r\n", 
                    "</script>\r\n",
                    ""); 
                if (!ClientScript.IsClientScriptBlockRegistered("clientAjaxScriptBlock"))
                {
                    ClientScript.RegisterClientScriptBlock(typeof(string), "clientAjaxScriptBlock", clientAjaxScriptBlock);
                }
            }
        }

        /// <summary>
        /// AjaxManager provides AjaxTemplate based Ajax support.
        /// </summary>
        public class AjaxManager
        {
            private string JsQuoteEncode(string s)
            {
                return string.IsNullOrEmpty(s) ? string.Empty : s.Replace("'", "\\'").Replace("\"", "\\\"");
            }

            /// <summary>
            /// Callbacks the specified ajax template.
            /// </summary>
            /// <param name="ajaxTemplate">The ajax template.</param>
            /// <param name="parms">The parms.</param>
            /// <param name="onComplete">The on complete.</param>
            /// <returns>The client side script.</returns>
            public string Callback(string ajaxTemplate, string parms, string onComplete)
            {
                string script = string.Format("Ajax.Callback('{0}', '{1}', '{2}')",
                    JsQuoteEncode(ajaxTemplate), JsQuoteEncode(parms), JsQuoteEncode(onComplete));
                return script;
            }

            /// <summary>
            /// Callback2s the specified ajax template.
            /// </summary>
            /// <param name="ajaxTemplate">The ajax template.</param>
            /// <param name="parms">The parms.</param>
            /// <param name="onComplete">The on complete.</param>
            /// <returns>The client side script.</returns>
            public string Callback2(string ajaxTemplate, string parms, string onComplete)
            {
                string script = string.Format("Ajax.Callback2('{0}', '{1}', '{2}')",
                    JsQuoteEncode(ajaxTemplate), JsQuoteEncode(parms), JsQuoteEncode(onComplete));
                return script;
            }

            /// <summary>
            /// Updates the specified ajax template.
            /// </summary>
            /// <param name="ajaxTemplate">The ajax template.</param>
            /// <param name="output">The output.</param>
            /// <param name="parms">The parms.</param>
            /// <param name="onComplete">The on complete.</param>
            /// <returns>The client side script.</returns>
            public string Update(string ajaxTemplate, string output, string parms, string onComplete)
            {
                string script = string.Format("Ajax.Update('{0}', '{1}', '{2}', '{3}')",
                    ajaxTemplate, output, JsQuoteEncode(parms), JsQuoteEncode(onComplete));
                return script;
            }

            /// <summary>
            /// Update2s the specified ajax template.
            /// </summary>
            /// <param name="ajaxTemplate">The ajax template.</param>
            /// <param name="output">The output.</param>
            /// <param name="parms">The parms.</param>
            /// <param name="onComplete">The on complete.</param>
            /// <returns>The client side script.</returns>
            public string Update2(string ajaxTemplate, string output, string parms, string onComplete)
            {
                string script = string.Format("Ajax.Update2('{0}', '{1}', '{2}', '{3}')",
                    ajaxTemplate, output, JsQuoteEncode(parms), JsQuoteEncode(onComplete));
                return script;
            }

            /// <summary>
            /// Called when [page load].
            /// </summary>
            /// <returns>The client side script.</returns>
            public string OnPageLoad()
            {
                return "Ajax.OnPageLoad()";
            }
        }

        /// <summary>
        /// An AjaxManager instance.
        /// </summary>
        protected AjaxManager Ajax = new AjaxManager();

        #endregion
    }
}
