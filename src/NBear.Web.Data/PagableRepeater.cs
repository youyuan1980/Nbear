using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;

namespace NBear.Web.Data
{
    public class PagableRepeater : Repeater
    {
        [Category("Paging"), DefaultValue(0), Description("Page size.")]
        public int PageSize
        {
            get
            {
                return SelectArguments.MaximumRows > 0 ? (SelectArguments.StartRowIndex / SelectArguments.MaximumRows) + 1 : 1;
            }
            set
            {
                if (value > 0)
                {
                    SelectArguments.MaximumRows = value;
                    int pageIndex = (ViewState["pageIndex"] == null ? 1 : (int)ViewState["pageIndex"]);
                    if (pageIndex > 1)
                    {
                        SelectArguments.StartRowIndex = (pageIndex - 1) * value;
                    }
                }
                else
                {
                    SelectArguments.MaximumRows = 0;
                }
            }
        }

        [Category("Paging"), DefaultValue(1), Description("Current page No.")]
        public int PageIndex
        {
            get
            {
                return SelectArguments.MaximumRows > 0 ? (SelectArguments.StartRowIndex / SelectArguments.MaximumRows) + 1 : 1;
            }
            set
            {
                if (value > 0)
                {
                    ViewState["pageIndex"] = value;
                    if (PageSize > 0)
                    {
                        SelectArguments.StartRowIndex = (value - 1) * PageSize;
                    }
                }
                else
                {
                    ViewState["pageIndex"] = 1;
                    SelectArguments.StartRowIndex = 0;
                }
            }
        }
    }
}
