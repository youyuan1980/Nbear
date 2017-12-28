using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace NBear.Web.UI
{
    /// <summary>
    /// The UserCOntrol base class.
    /// </summary>
    public class AjaxTemplate : UserControl
    {
        /// <summary>
        /// Called when [ajax template pre render].
        /// </summary>
        /// <param name="callbackParams">The callback params.</param>
        public virtual void OnAjaxTemplatePreRender(Dictionary<string, string> callbackParams)
        {
        }

        /// <summary>
        /// Loads the JST content from user control.
        /// </summary>
        /// <param name="jstControlPath">The JST control path.</param>
        /// <returns>The jst content.</returns>
        protected string LoadJstContentFromUserControl(string jstControlPath)
        {
            StringBuilder sb = new StringBuilder();
            string path = (jstControlPath.ToLower().EndsWith(".ascx") ? jstControlPath : jstControlPath + ".ascx");
            try
            {
                Control ctl = LoadControl(path);
                ctl.RenderControl(new HtmlTextWriter(new System.IO.StringWriter(sb)));
            }
            catch
            {
            }
            //return ToJsSingleQuoteSafeString(sb.ToString());
            return sb.ToString();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        public new void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
    }
}
