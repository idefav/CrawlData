using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Idefav.Utility;

namespace WebChatSites.Models.WeChat
{
    /// <summary>
    /// 降价商品
    /// </summary>
    [TableName("db_crawlconfig.dbo.td_cheapproduct")]
    public class CheapProductData
    {
        /// <summary>
        /// Guid
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 所属商城
        /// </summary>
        [PrimaryKey]
        public string Shop { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        [PrimaryKey]
        public string ProductId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        public string PicUrl { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public decimal? Discount { get; set; }
    }
}