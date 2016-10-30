using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Idefav.Utility;

namespace WebChatSites.Models.WeChat
{
    [TableName("db_tmall.dbo.td_data")]
    public class ProductChartData
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        [PrimaryKey]
        public string ItemId { get; set; }

        /// <summary>
        /// 价格更新日期
        /// </summary>
        [PrimaryKey]
        public DateTime PDate { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        public Decimal? Price { get; set; }
    }
}