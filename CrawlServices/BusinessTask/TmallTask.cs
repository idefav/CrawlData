﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crawl.Common;
using Crawl.Common.Interface;
using Crawl.Common.Model;
using Ebend.DataSplider;
using Idefav.DbFactory;
using Idefav.IDAL;
using Idefav.Utility;

namespace CrawlServices.BusinessTask
{
    public class TmallTask : TaskBase
    {
        public TmallTaskModel TmallTaskModel { get { return base.Model as TmallTaskModel; } }
        public CancellationToken CancellationToken { get; set; }
        public const string Shop = "天猫";

        public IDbObject Db { get; set; }
        public IDbObject DbData { get; set; }

        private const string sCookies = "x=__ll%3D-1%26_ato%3D0; cna=97ykDz7G2GYCAXVZmbuJZpdH; otherx=e%3D1%26p%3D*%26s%3D0%26c%3D0%26f%3D0%26g%3D0%26t%3D0; uc3=nk2=AnWUh%2Bs%3D&id2=UNk1%2BrxWtNk%3D&vt3=F8dASmgq691MKE0n%2BJg%3D&lg2=UIHiLt3xD8xYTw%3D%3D; uss=UtJSnXzx31ekK4NBPk9D5ChUqQKCyM0Iw%2BMNDs7w1BQltILkn%2BmmiykAQw%3D%3D; lgc=adobo; tracknick=adobo; cookie2=3c8fda0838e9f0f989dc5c427f4239e6; t=18fbd460625667ad4fc53c42619f98bf; _tb_token_=e5e15b5e9e591; pnm_cku822=157UW5TcyMNYQwiAiwQRHhBfEF8QXtHcklnMWc%3D%7CUm5OcktwT3tFe0Z7Q3xBeiw%3D%7CU2xMHDJ7G2AHYg8hAS8RJQsrBVk4XjJVK1F%2FKX8%3D%7CVGhXd1llXGdYbFJsUWxUa1ZtWmdFeEZ9RX5AdEF8R3hAdE1yTHRaDA%3D%3D%7CVWldfS0QMAs0CioWKwslTjIXIVU4XCZLZ0kfSQ%3D%3D%7CVmhIGC0ZOQA0FCgXKxAwDDEMOQYmGiUQLQ0xDDMKKhYpHCEBNQA5bzk%3D%7CV25Tbk5zU2xMcEl1VWtTaUlwJg%3D%3D; res=scroll%3A1216*10082-client%3A1216*866-offset%3A1216*10082-screen%3A1280*1024; cq=ccp%3D1; l=AgsLWcRfWvyTxboIhs4ASAfcm6H1oh8i";

        public TmallTask(ITaskModel taskModel) : base(taskModel)
        {
            Db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            DbData = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbAnalyze);
        }

        public override void Run(TaskScheduler taskScheduler, CancellationToken token)
        {

            this.CancellationToken = token;

            Task = new Task<object>(() =>
            {
                try
                {
                    Business();
                    NextStartTime = DateTime.Now.AddSeconds(TmallTaskModel.Interval);
                    //Common.UpdateCrawlStatus(TmallTaskModel.TaskName, true);
                    CrawlServices.Business.UpdateCrawlComplete(TmallTaskModel.TaskName);
                }
                catch (Exception e)
                {
                    Crawl.Common.Common.Log.LogError(e.ToString());
                }
                return null;
            }, token);
            Task.Start(taskScheduler);

            //System.Threading.Tasks.Task.WaitAll(Task);
        }

        private void Business()
        {
            try
            {
                Crawl.Common.Common.Log.LogInfo(string.Format("{0}_{1}", TmallTaskModel.TaskName, "开始"));

                var breakpoint = CrawlServices.Business.GetCurrBreakpoint(TmallTaskModel.TaskName);
                int index = 0;
                int ipage = 0;
                if (!string.IsNullOrEmpty(breakpoint.Key))
                {
                    index = TmallTaskModel.KeyWords.ToList().IndexOf(breakpoint.Key);
                    if (breakpoint.Value.HasValue)
                    {
                        ipage = breakpoint.Value.Value;
                    }

                }
                for (int i = index; i < TmallTaskModel.KeyWords.Length; i++)
                {
                    try
                    {
                        string keyWord = TmallTaskModel.KeyWords[i];
                        Crawl.Common.Common.Log.LogInfo(string.Format("{0}-正在解析关键字:{1}", TmallTaskModel.TaskName, keyWord));
                        ResolveKeyWord(keyWord, ipage);
                        ipage = 0;
                        Crawl.Common.Common.Log.LogInfo(string.Format("{0}-解析关键字:{1} 完成", TmallTaskModel.TaskName, keyWord));
                    }
                    catch (Exception e)
                    {
                        Crawl.Common.Common.Log.LogError(e.ToString());
                    }
                }
                //foreach (string keyWord in TmallTaskModel.KeyWords)
                //{
                //    //TMallSplider tMallSplider = new TMallSplider();
                //    //tMallSplider.sCookies = sCookies;


                //    //var productlist = tMallSplider.SearchProduct2(keyWord, iPage++, out totalPage);
                //    try
                //    {
                //        Common.CommonLog.LogInfo(string.Format("{0}-正在解析关键字:{1}", TmallTaskModel.TaskName, keyWord));
                //        ResolveKeyWord(keyWord);
                //        Common.CommonLog.LogInfo(string.Format("{0}-解析关键字:{1} 完成", TmallTaskModel.TaskName, keyWord));
                //    }
                //    catch (Exception e)
                //    {
                //        Common.CommonLog.LogError(e.ToString());
                //    }
                //}
                Crawl.Common.Common.Log.LogInfo(string.Format("{0}_{1}", TmallTaskModel.TaskName, "结束"));
            }
            catch (Exception e)
            {
                Crawl.Common.Common.Log.LogError(e.ToString());
            }
        }

        private void ResolveKeyWord(string keyword, int page)
        {
            int totalPage = 0;
            int iPage = page;
            TMallSplider tMallSplider = new TMallSplider();
            tMallSplider.sCookies = sCookies;


            SaveDataToDb(tMallSplider.SearchProduct2(keyword, iPage, out totalPage));
            CrawlServices.Business.UpdateCrawlBreakpoint(TmallTaskModel.TaskName, keyword, iPage);
            iPage++;
            while (iPage < totalPage && iPage < TmallTaskModel.CrawlPages)
            {

                SaveDataToDb(tMallSplider.SearchProduct2(keyword, iPage, out totalPage));
                CrawlServices.Business.UpdateCrawlBreakpoint(TmallTaskModel.TaskName, keyword, iPage);
                iPage++;
                Thread.Sleep(AppSettings.COMMONSETTINGS.Interval * 1000);
            }
        }

        private void SaveDataToDb(SpliderType.TypeProduct[] typeProducts)
        {
            //var db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            foreach (SpliderType.TypeProduct typeProduct in typeProducts)
            {
                try
                {
                    decimal? oldprice = null;
                    var product = Product.Create(typeProduct);
                    if (CrawlServices.Business.IsCheapProduct(Shop, product.ItemId, product.Price, out oldprice))
                    {
                        DbData.Upsert(CheapProduct.Create(Shop, product.ItemId, product.Title, product.Price, oldprice, typeProduct.PicUrl));
                    }
                    var result = SavetoDb(product);
                    Crawl.Common.Common.Log.LogInfo(result
                        ? string.Format("更新[{0}]{1} 成功", typeProduct.sItemID, typeProduct.sTitle)
                        : string.Format("更新[{0}]{1} 失败", typeProduct.sItemID, typeProduct.sTitle));
                }
                catch (Exception e)
                {
                    Crawl.Common.Common.Log.LogError(e.ToString());
                }
            }

        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="tablename">表名称</param>
        private void CreateDataTable(string tablename)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(
                @"CREATE TABLE [DB_Tmall].[dbo].[" + tablename + @"](

                [Guid][nvarchar](50) NOT NULL,

                [ItemId][nvarchar](50) NOT NULL,

                [Title][nvarchar](max) NULL,

                [Price][decimal](18, 2) NULL,

                [PriceAVG][decimal](18, 2) NULL,

                [SellCount][int] NULL,

                [PriceUnit][nvarchar](50) NULL,

                [CountUnit][nvarchar](50) NULL,

                [CountAVG][int] NULL,

                [PDate][datetime] NOT NULL,

                [UpdateTime][datetime] NOT NULL,

                [CommentCount][int] NULL,

                [PicUrl][nvarchar](max) NULL,
                CONSTRAINT[PK_" + tablename + @"] PRIMARY KEY CLUSTERED
            (

                [ItemId] ASC,

                [PDate] ASC
            )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
            ) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]");
            Db.ExecuteSql(stringBuilder.ToString());
        }

        private bool SavetoDb(Product product)
        {
            string tablename = "td_data_" + DateTime.Now.ToString("yyyyMMdd");
            if (!CrawlServices.Business.TableIsExist(tablename, AppSettings.COMMONSETTINGS.DbTmall))
            {
                // 创建表
                CreateDataTable(tablename);
            }
            return Db.Upsert(product, "db_tmall.dbo." + tablename);
        }

    }

    [TableName("db_tmall.dbo.td_data")]
    public class Product
    {
        public string Guid { get; set; }
        [PrimaryKey]
        public string ItemId { get; set; }
        public string Title { get; set; }
        public decimal? Price { get; set; }
        public int? SellCount { get; set; }
        public string PriceUnit { get; set; }
        public string CountUnit { get; set; }
        public string CountAVG { get; set; }
        [PrimaryKey]
        public DateTime PDate { get; set; }

        public DateTime UpdateTime { get; set; }

        public void Init(SpliderType.TypeProduct typeProduct)
        {
            Guid = System.Guid.NewGuid().ToString();

            ItemId = typeProduct.sItemID;
            Title = typeProduct.sTitle;
            decimal price = 0;
            if (decimal.TryParse(typeProduct.sPrice, out price))
            {
                Price = price;
            }
            int sellcount = 0;
            if (int.TryParse(typeProduct.sSellCount, out sellcount))
            {
                SellCount = sellcount;
            }
            PriceUnit = typeProduct.sPriceUnit;
            CountUnit = typeProduct.sCountUnit;
            CountAVG = typeProduct.sCountAVG;
            PDate = DateTime.Now.Date;
            UpdateTime = DateTime.Now;
        }

        public static Product Create(SpliderType.TypeProduct typeProduct)
        {
            Product product = new Product();
            product.Init(typeProduct);
            return product;
        }

    }

    /// <summary>
    /// 降价商品
    /// </summary>
    [TableName("DB_CrawlConfig.dbo.td_cheapproduct")]
    public class CheapProduct
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

        /// <summary>
        ///  图片链接
        /// </summary>
        public string PicUrl { get; set; }

        /// <summary>
        /// 原价
        /// </summary>
        public decimal? OldPrice { get; set; }
        /// <summary>
        /// 初始化实例
        /// </summary>
        /// <param name="shop">商城</param>
        /// <param name="productid">商品编号</param>
        /// <param name="productname">商品名称</param>
        /// <param name="price">商品价格</param>
        public void Init(string shop, string productid, string productname, decimal? price, decimal? oldprice, string picurl = null)
        {
            Guid = System.Guid.NewGuid().ToString();
            Shop = shop;
            ProductId = productid;
            ProductName = productname;
            Price = price;
            UpdateTime = DateTime.Now;
            PicUrl = picurl;
            OldPrice = oldprice;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="shop"></param>
        /// <param name="productid"></param>
        /// <param name="productname"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public static CheapProduct Create(string shop, string productid, string productname, decimal? price, decimal? oldprice, string prcurl = null)
        {
            CheapProduct cheapProduct = new CheapProduct();
            cheapProduct.Init(shop, productid, productname, price, oldprice, prcurl);
            return cheapProduct;
        }
    }


}
