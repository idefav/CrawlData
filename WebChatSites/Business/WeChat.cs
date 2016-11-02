﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using Crawl.Common;
using Idefav.DbFactory;
using Idefav.IDAL;
using Idefav.Utility;
using WebChatSites.Models.WeChat;

namespace WebChatSites.Business
{
    public class WeChat
    {
        private const string sCookies = "x=__ll%3D-1%26_ato%3D0; cna=97ykDz7G2GYCAXVZmbuJZpdH; otherx=e%3D1%26p%3D*%26s%3D0%26c%3D0%26f%3D0%26g%3D0%26t%3D0; uc3=nk2=AnWUh%2Bs%3D&id2=UNk1%2BrxWtNk%3D&vt3=F8dASmgq691MKE0n%2BJg%3D&lg2=UIHiLt3xD8xYTw%3D%3D; uss=UtJSnXzx31ekK4NBPk9D5ChUqQKCyM0Iw%2BMNDs7w1BQltILkn%2BmmiykAQw%3D%3D; lgc=adobo; tracknick=adobo; cookie2=3c8fda0838e9f0f989dc5c427f4239e6; t=18fbd460625667ad4fc53c42619f98bf; _tb_token_=e5e15b5e9e591; pnm_cku822=157UW5TcyMNYQwiAiwQRHhBfEF8QXtHcklnMWc%3D%7CUm5OcktwT3tFe0Z7Q3xBeiw%3D%7CU2xMHDJ7G2AHYg8hAS8RJQsrBVk4XjJVK1F%2FKX8%3D%7CVGhXd1llXGdYbFJsUWxUa1ZtWmdFeEZ9RX5AdEF8R3hAdE1yTHRaDA%3D%3D%7CVWldfS0QMAs0CioWKwslTjIXIVU4XCZLZ0kfSQ%3D%3D%7CVmhIGC0ZOQA0FCgXKxAwDDEMOQYmGiUQLQ0xDDMKKhYpHCEBNQA5bzk%3D%7CV25Tbk5zU2xMcEl1VWtTaUlwJg%3D%3D; res=scroll%3A1216*10082-client%3A1216*866-offset%3A1216*10082-screen%3A1280*1024; cq=ccp%3D1; l=AgsLWcRfWvyTxboIhs4ASAfcm6H1oh8i";
        public static Random rd = new Random();
        private static string GetUserAgent()
        {
            string[] ualist = new[]
            {
                "Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_6_8; en-us) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50",
                "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-us) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50",
                "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0",
                "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)",
                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.6; rv:2.0.1) Gecko/20100101 Firefox/4.0.1",
                "Mozilla/5.0 (Windows NT 6.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1",
                "Opera/9.80 (Macintosh; Intel Mac OS X 10.6.8; U; en) Presto/2.8.131 Version/11.11",
                "Opera/9.80 (Windows NT 6.1; U; en) Presto/2.8.131 Version/11.11",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_7_0) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.56 Safari/535.11",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Maxthon 2.0)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; TencentTraveler 4.0)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; The World)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Trident/4.0; SE 2.X MetaSr 1.0; SE 2.X MetaSr 1.0; .NET CLR 2.0.50727; SE 2.X MetaSr 1.0)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; 360SE)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Avant Browser)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)"
            };
            int d = rd.Next(ualist.Length);
            return ualist[d];
        }

        private IDbObject Db = DBOMaker.CreateDbObj(DBType.SQLServer, CommSettings.DbConn);

        private static string DownloadProduct(string url, string sTCookies)
        {
            HtmlHttpHelper HHH = new HtmlHttpHelper();
            HHH.sCookies = sTCookies;
            HHH.UserAgent = GetUserAgent();
            HHH.Referer = "https://www.tmall.com/";
            string sUrl = url;

            string sHtmlCode = HHH.Get(sUrl, "").Replace("\n", "");
            Regex regex = new Regex("\"itemid\":([^,]*)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var match = regex.Match(sHtmlCode);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return "";
        }

        public static string GetProductId(string input)
        {
            string productId = "";
            Regex regex = new Regex("//detail.tmall.com/item.htm\\?.*?id=([^&]*)&*");
            var match = regex.Match(input);
            if (match.Success)
            {
                productId = match.Groups[1].Value;
            }
            else
            {
                regex = new Regex("复制整段信息，打开.*】\\(未安装App点这里：(.*?)\\)");
                match = regex.Match(input);
                if (match.Success)
                {
                    string url = match.Groups[1].Value;
                    productId = DownloadProduct(url, sCookies);
                }
                else
                {
                    regex = new Regex("http://sjtm.me/s/.*?\\?tm=.*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    match = regex.Match(input);
                    if (match.Success)
                    {
                        productId = DownloadProduct(input, sCookies);
                    }
                }
            }

            return productId;
        }


        /// <summary>
        /// 获取图表数据
        /// </summary>
        /// <param name="itemid">商品编号</param>
        /// <returns></returns>
        public List<ProductChartData> GetData(string itemid)
        {
            StringBuilder stringBuilder = new StringBuilder("SELECT [ItemId],[Price],[Title],[PDate]FROM [DB_Tmall].[dbo].[td_data] where  itemid=@itemid order by updatetime");
            var data = Db.QueryModels<ProductChartData>(stringBuilder.ToString(), new { itemid = itemid });
            return data;
        }


        /// <summary>
        /// 获取最低价格
        /// </summary>
        /// <param name="itemid">商品编号</param>
        /// <returns></returns>
        public decimal? GetMinPrice(string itemid)
        {

            StringBuilder stringBuilder = new StringBuilder(" SELECT MIN(Price) minprice FROM [DB_Tmall].[dbo].[td_data] where ItemId=@itemid");
            var dataset = Db.Query(stringBuilder.ToString(), new { itemid = itemid });
            if (dataset != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
            {
                decimal v = 0;
                decimal.TryParse(dataset.Tables[0].Rows[0][0].ToString(), out v);
                return v;
            }
            return null;
        }

        /// <summary>
        /// 获取降价排行版
        /// </summary>
        /// <param name="updatetime">更新时间</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public List<CheapProductData> GetCheapProductDatas(DateTime updatetime ,int count=20)
        {
            StringBuilder stringBuilder = new StringBuilder(@"SELECT TOP 20 "); 
            stringBuilder.Append(
                                  @" [Guid]
                                    ,[Shop]
                                    ,[ProductId]
                                    ,[ProductName]
                                    ,[Price]
                                    ,[UpdateTime]
                                    FROM[DB_CrawlConfig].[dbo].[td_cheapproduct] a
                                    where a.updatetime > @updatetime
                                    order by updatetime desc");

            var datas = Db.QueryModels<CheapProductData>(stringBuilder.ToString(), new { updatetime = updatetime });
            return datas;
        }

    }
}