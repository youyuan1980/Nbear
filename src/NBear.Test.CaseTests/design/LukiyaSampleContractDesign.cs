using System;
using NBear.Common.Design;

namespace LukiyA.GameSite.EntityDesigns
{
    public interface base_Id_Int_Writeble
    {
        [PrimaryKey]
        int Id { get; }
    }

    public interface base_Title
    {
        [SqlType("nvarchar(256)")]
        string Title { get; set; }
    }

    public interface base_SubTitle
    {
        [SqlType("nvarchar(256)")]
        string SubTitle { get; set; }
    }

    public interface base_OrderNum
    {
        int OrderNum { get; set; }
    }

    public interface base_IsDisplay
    {
        bool IsDisplay { get; set; }
    }

    public interface base_Node
    {
        [FkReverseQuery(LazyLoad = true)]
        game_SiteNodes Parent { get; set; }

        [FkQuery("Parent", Contained = true, LazyLoad = true)]
        game_SiteNodes[] Childs { get; set; }
    }

    [AutoPreLoad]
    public interface game_SiteNodes : Entity,
    base_Id_Int_Writeble,
    base_OrderNum,
    base_Node,
    base_IsDisplay,
    base_Title,
    base_SubTitle
    { }
}
