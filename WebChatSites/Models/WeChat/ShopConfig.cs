using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebChatSites.Models.WeChat
{
    public class ShopConfig
    {
        public string Guid { get; set; }
        public string Shop { get; set; }

        public string Regex { get; set; }

        public string Cookies { get; set; }

        public int Order { get; set; }

        public DateTime UpdateTime { get; set; }
        public bool IsDel { get; set; }
        public string DbTable { get; set; }
    }

    public class ShopFilter
    {
        public ProductLinkMode name { get; set; }
        public string value { get; set; }
        public bool download { get; set; }
        public string regex { get; set; }
        
    }

    public enum ProductLinkMode
    {
        /// <summary>
        /// PC端浏览器链接
        /// </summary>
        PcLink,

        /// <summary>
        /// APP分享的口令
        /// </summary>
        AppShareToken,

        /// <summary>
        /// APP分享的链接
        /// </summary>
        AppShareLink,

        /// <summary>
        /// 产品编号
        /// </summary>
        ItemId
    }

    public enum ShopEnum
    {
        天猫,
        淘宝
    }
}