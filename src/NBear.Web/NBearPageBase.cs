using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections;
namespace NBear.Web
{
    public class NBearPageBase:System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            List<string> keylist = new List<string>(ConfigurationManager.AppSettings.AllKeys);
            if (keylist.Contains("title"))
            {
                this.Title = Convert.ToString(ConfigurationManager.AppSettings["title"]);
            }

            base.OnLoad(e);
        }

        #region 显示对话框
        /// <summary>
        /// 显示对话框
        /// </summary>
        /// <param name="Text">显示文本</param>
        /// <param name="Url">跳转的URL</param>
        protected void MessageBox(string Msg, string Url, bool Top)
        {
            if (!Top)
            {
                Response.Write("<script language='javascript'>alert('" + Msg + "');window.location.href = '" + Url + "'</script>");
                Response.End();
            }
            else
            {
                Response.Write("<script language='javascript'>alert('" + Msg + "');top.location.href = '" + Url + "'</script>");
                Response.End();
            }
        }

        /// <summary>
        /// 显示对话框
        /// </summary>
        /// <param name="Text">显示文本</param>
        /// <param name="Url">跳转的URL</param>
        protected void MessageBox(string Msg, string Url)
        {
            Response.Write("<script language='javascript'>alert('" + Msg + "');window.location.href = '" + Url + "'</script>");
            Response.End();
        }

        /// <summary>
        /// 显示对话框
        /// </summary>
        /// <param name="Text">显示文本</param>
        /// <param name="Url">跳转的URL</param>
        protected void MessageBox(string Msg)
        {
            Response.Write("<script language='javascript'>alert('" + Msg + "');</script>");
            Response.End();
        }
        #endregion
    }
}
