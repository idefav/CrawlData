using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.DynamicData;
using System.Web.SessionState;
using Crawl.Common;
using Idefav.DbFactory;
using Idefav.IDAL;
using Idefav.Utility;
using Newtonsoft.Json;
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

        private IDbObject DbAnalyze = DBOMaker.CreateDbObj(DBType.SQLServer, CommSettings.DbAnalyze);
        private IDbObject DbConfig = DBOMaker.CreateDbObj(DBType.SQLServer, CommSettings.DbConfig);

        private static string DownloadProduct(string url, string sTCookies, string filter)
        {
            HtmlHttpHelper HHH = new HtmlHttpHelper();
            HHH.sCookies = sTCookies;
            HHH.UserAgent = GetUserAgent();
            HHH.Referer = "https://www.tmall.com/";
            string sUrl = url;
            // "\"itemid\":([^,]*)"
            string sHtmlCode = HHH.Get(sUrl, "").Replace("\n", "");
            Regex regex = new Regex(filter, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var match = regex.Match(sHtmlCode);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return "";
        }

        public static HybridDictionary GetProductId(string input)
        {


            string productId = "";


            string dbtable = "db_tmall.dbo.td_data";
            ShopEnum shop = ShopEnum.天猫;
            string productid = "";
            ShopConfig currShopConfig=null;
            var shopconfigs = GetShopConfigs();
            foreach (ShopConfig shopconfig in shopconfigs)
            {
                var shopFilter = JsonConvert.DeserializeObject<List<ShopFilter>>(shopconfig.Regex);
                if (shopFilter != null)
                {
                    foreach (ShopFilter filter in shopFilter)
                    {
                        switch (filter.name)
                        {
                            case ProductLinkMode.PcLink:
                                {
                                    Regex regex = new Regex(filter.value,
                                      RegexOptions.Compiled | RegexOptions.IgnoreCase);
                                    var match = regex.Match(input);
                                    if (match.Success)
                                    {
                                        productId = match.Groups[1].Value;

                                        break;
                                    }
                                    break;
                                }
                            case ProductLinkMode.AppShareToken:
                            case ProductLinkMode.AppShareLink:
                                {
                                    Regex regex = new Regex(filter.value,
                                      RegexOptions.Compiled | RegexOptions.IgnoreCase);
                                    var match = regex.Match(input);
                                    string url = match.Success ? match.Groups[1].Value : input;
                                    productId = DownloadProduct(url, shopconfig.Cookies, filter.regex);

                                    break;
                                }
                            case ProductLinkMode.ItemId:
                                {
                                    Regex regex = new Regex(filter.value,
                                      RegexOptions.Compiled | RegexOptions.IgnoreCase);
                                    var match = regex.Match(input);
                                    if (match.Success)
                                    {
                                        productId = match.Groups[1].Value;
                                    }
                                    break;
                                }

                        }
                        if (!string.IsNullOrEmpty(productId))
                        {
                            Enum.TryParse(shopconfig.Shop, true, out shop);
                            currShopConfig = shopconfig;
                            break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(productId))
                {
                    Enum.TryParse(shopconfig.Shop, true, out shop);
                    dbtable = shopconfig.DbTable;
                    break;
                }
            }

            //    string[] filters = shopconfig.Regex.Split('');
            //    foreach (string filter in filters)
            //    {
            //        if (filter.Contains("\n"))
            //        {
            //            var files2 = filter.Split('\n');
            //            string url = "";
            //            Regex regex = new Regex(files2[0]);
            //            Match match = regex.Match(input);
            //            url = match.Success ? match.Groups[1].Value : input;
            //            productId = DownloadProduct(url, shopconfig.Cookies, files2[1]);

            //        }
            //        else
            //        {
            //            Regex regex=new Regex(filter);
            //            var match = regex.Match(input);
            //            if (match.Success)
            //            {
            //                productId = match.Groups[1].Value;
            //            }
            //        }
            //    }

            //}
            //Regex regex = new Regex("//detail.tmall.com/item.htm\\?.*?id=([^&]*)&*");
            //var match = regex.Match(input);
            //if (match.Success)
            //{
            //    productId = match.Groups[1].Value;
            //}
            //else
            //{
            //    regex = new Regex("复制整段信息，打开.*】\\(未安装App点这里：(.*?)\\)");
            //    match = regex.Match(input);
            //    if (match.Success)
            //    {
            //        string url = match.Groups[1].Value;
            //        productId = DownloadProduct(url, sCookies);
            //    }
            //    else
            //    {
            //        regex = new Regex("http://sjtm.me/s/.*?\\?tm=.*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            //        match = regex.Match(input);
            //        if (match.Success)
            //        {
            //            productId = DownloadProduct(input, sCookies);
            //        }
            //    }
            //}

            return new HybridDictionary { { "itemid", productId }, { "shopname", shop.ToString() }, { "table", dbtable }, {"shopconfig",currShopConfig} };
        }

        /// <summary>
        /// 获取店铺配置
        /// </summary>
        /// <returns></returns>
        public static List<ShopConfig> GetShopConfigs()
        {
            string sql = "select * from db_crawlconfig.dbo.td_shopconfig order by [order] asc";
            return CacheFactory.Cache(sql, () =>
             {
                 StringBuilder stringBuilder = new StringBuilder(sql);
                 IDbObject Db = DBOMaker.CreateDbObj(DBType.SQLServer, CommSettings.DbConfig);
                 return Db.QueryModels<ShopConfig>(stringBuilder.ToString());
             }, true, cacheTime: TimeSpan.FromMinutes(1));

        }

        public static ShopConfig GetShopConfigByName(string shop)
        {
            string sql = "select * from db_crawlconfig.dbo.td_shopconfig where shop=@shop order by [order] asc";
            return CacheFactory.Cache(sql+shop, () =>
            {
                StringBuilder stringBuilder = new StringBuilder(sql);
                IDbObject Db = DBOMaker.CreateDbObj(DBType.SQLServer, CommSettings.DbConfig);
                return Db.QueryModel<ShopConfig>(stringBuilder.ToString(),new {shop=shop});
            }, true, cacheTime: TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// 获取图表数据
        /// </summary>
        /// <param name="itemid">商品编号</param>
        /// <param name="table">数据库表名</param>
        /// <returns></returns>
        public ProductChartData GetData(string itemid, string shop)
        {

            StringBuilder stringBuilder = new StringBuilder(@"SELECT  a.*,a.ProductId ItemId,b.ProductName Title,b.NowPrice Price
  FROM[DB_Analyze].[dbo].[td_data_ALL] a
  left join DB_Analyze.dbo.td_productinfo b on a.ProductId = b.ProductId and a.Shop = b.shop
  where a.ProductId = @productid and a.Shop = @shop");
            var data = DbAnalyze.QueryModel<ProductChartData>(stringBuilder.ToString(), new { ProductId = itemid, shop = shop });

            return data;
        }

        /// <summary>
        /// 获取店铺配置
        /// </summary>
        /// <param name="shop"></param>
        /// <returns></returns>
        public static ShopConfig GetShopConfigByName(ShopEnum shop)
        {
            StringBuilder stringBuilder = new StringBuilder("select * from db_crawlconfig.dbo.td_shopconfig where shop=@shop ");
            return CacheFactory.Cache(stringBuilder.ToString() + shop.ToString(), () =>
            {
                IDbObject Db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);

                var model = Db.QueryModel<ShopConfig>(stringBuilder.ToString(), new { shop = shop.ToString() });
                return model;
            }, true, cacheTime: TimeSpan.FromMinutes(1));

        }
        /// <summary>
        /// 获取最低价格
        /// </summary>
        /// <param name="itemid">商品编号</param>
        /// <returns></returns>
        public decimal? GetMinPrice(string itemid, string dbtable)
        {
            //var shopconfig=GetShopConfigByName()
            StringBuilder stringBuilder = new StringBuilder(" SELECT MIN(Price) minprice FROM " + dbtable + " where ItemId=@itemid");
            var dataset = DbAnalyze.Query(stringBuilder.ToString(), new { itemid = itemid });
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
        public List<CheapProductData> GetCheapProductDatas(DateTime updatetime, int count = 20)
        {
            StringBuilder stringBuilder = new StringBuilder(@"SELECT TOP 20 ");
            stringBuilder.Append(
                                  @" *
                                    FROM[DB_CrawlConfig].[dbo].[td_cheapproduct] a
                                    where a.updatetime > @updatetime
                                    order by updatetime desc");

            var datas = DbAnalyze.QueryModels<CheapProductData>(stringBuilder.ToString(), new { updatetime = updatetime });
            return datas;
        }

    }
}