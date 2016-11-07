using System;
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
    public class TaoBaoTask : TaskBase
    {
        public TaoBaoTaskModel TaoBaoTaskModel { get { return base.Model as TaoBaoTaskModel; } }
        public CancellationToken CancellationToken { get; set; }
        public const string Shop = "淘宝";

        public IDbObject Db { get; set; }

        private const string sCookies = " thw=cn; ali_ab=114.221.152.238.1456796750675.0; lzstat_uv=26429925573013598357|3555462@2144678; uc3=sg2=B0T4xvKU3WRD5Hk2%2Fv%2B4Q9kggge1Dr3ExU5qweNLeH4%3D&nk2=AnWUh%2Bs%3D&id2=UNk1%2BrxWtNk%3D&vt3=F8dASmgq691MKE0n%2BJg%3D&lg2=UIHiLt3xD8xYTw%3D%3D; uss=UtJSnXzx31ekK4NBPk9D5ChUqQKCyM0Iw%2BMNDs7w1BQltILkn%2BmmiykAQw%3D%3D; lgc=adobo; tracknick=adobo; _cc_=VT5L2FSpdA%3D%3D; tg=0; cna=97ykDz7G2GYCAXVZmbuJZpdH; x=e%3D1%26p%3D*%26s%3D0%26c%3D0%26f%3D0%26g%3D0%26t%3D0%26__ll%3D-1; mt=ci=-1_0; v=0; cookie2=3c8fda0838e9f0f989dc5c427f4239e6; t=18fbd460625667ad4fc53c42619f98bf; _tb_token_=e5e15b5e9e591; l=ApWVxAvyiABLxfSmbBwWjTf4pZ9P0Uml; uc1=cookie14=UoWxM%2FY4orO16g%3D%3D; pnm_cku822=251UW5TcyMNYQwiAiwQRHhBfEF8QXtHcklnMWc%3D%7CUm5OcktwT3pOcEp%2BSnJKdiA%3D%7CU2xMHDJ7G2AHYg8hAS8RJQsrBVk4XjJVK1F%2FKX8%3D%7CVGhXd1llXGdYbVlnXWldZV1hVmtJdk50SnROck57QX9Ddkp2TXRaDA%3D%3D%7CVWldfS0QMA43Di4QMB4iHjsVQxU%3D%7CVmhIGC0ZOQA0FCgXKRc3CD0EJBgnEi8PMw4xCCgUKx4jAzcCO207%7CV25Tbk5zU2xMcEl1VWtTaUlwJg%3D%3D";

        public TaoBaoTask(ITaskModel taskModel) : base(taskModel)
        {
            Db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
        }

        private void Business()
        {
            try
            {
                Crawl.Common.Common.Log.LogInfo(string.Format("{0}_{1}", TaoBaoTaskModel.TaskName, "开始"));

                var breakpoint = CrawlServices.Business.GetCurrBreakpoint(TaoBaoTaskModel.TaskName);
                int index = 0;
                int ipage = 0;
                if (!string.IsNullOrEmpty(breakpoint.Key))
                {
                    index =
                        TaoBaoTaskModel.KeyWords.ToList()
                            .Select(c => c.Key + "_" + c.Value)
                            .ToList()
                            .IndexOf(breakpoint.Key);
                    if (breakpoint.Value.HasValue)
                    {
                        ipage = breakpoint.Value.Value;
                    }

                }
                index = index <=0 ? 0 : index;
                for (int i = index; i < TaoBaoTaskModel.KeyWords.Length; i++)
                {
                    try
                    {
                        string keyWord = TaoBaoTaskModel.KeyWords.ToList()[i].Key;
                        string cat = TaoBaoTaskModel.KeyWords.ToList()[i].Value;
                        Crawl.Common.Common.Log.LogInfo(string.Format("{0}-正在解析关键字:{1}", TaoBaoTaskModel.TaskName, keyWord+"_"+cat));
                        ResolveKeyWord(keyWord,cat, ipage);
                        ipage = 0;
                        Crawl.Common.Common.Log.LogInfo(string.Format("{0}-解析关键字:{1} 完成", TaoBaoTaskModel.TaskName, keyWord + "_" + cat));
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
                Crawl.Common.Common.Log.LogInfo(string.Format("{0}_{1}", TaoBaoTaskModel.TaskName, "结束"));
            }
            catch (Exception e)
            {
                Crawl.Common.Common.Log.LogError(e.ToString());
            }
        }

        private void ResolveKeyWord(string keyword,string cat, int page)
        {
            int totalPage = 0;
            int iPage = page;
            TaobaoSplider taoBaoSplider = new TaobaoSplider();
            taoBaoSplider.sCookies = sCookies;


            SaveDataToDb(taoBaoSplider.SearchProducts2(keyword, iPage,cat, out totalPage));
            CrawlServices.Business.UpdateCrawlBreakpoint(TaoBaoTaskModel.TaskName, keyword + "_" + cat, iPage);
            iPage++;
            while (totalPage>=0 && iPage < TaoBaoTaskModel.CrawlPages)
            {

                SaveDataToDb(taoBaoSplider.SearchProducts2(keyword, iPage,cat, out totalPage));
                CrawlServices.Business.UpdateCrawlBreakpoint(TaoBaoTaskModel.TaskName, keyword+"_"+cat, iPage);
                iPage++;
                Thread.Sleep(AppSettings.COMMONSETTINGS.Interval);
            }
        }

        public override void Run(TaskScheduler taskScheduler, CancellationToken token)
        {

            this.CancellationToken = token;

            Task = new Task<object>(() =>
            {
                try
                {
                    Business();
                    NextStartTime = DateTime.Now.AddSeconds(TaoBaoTaskModel.Interval);
                    //Common.UpdateCrawlStatus(TmallTaskModel.TaskName, true);
                    CrawlServices.Business.UpdateCrawlComplete(TaoBaoTaskModel.TaskName);
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

        private void SaveDataToDb(SpliderType.TypeProduct[] typeProducts)
        {
            //var db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            foreach (SpliderType.TypeProduct typeProduct in typeProducts)
            {
                try
                {
                    var product = TaoBaoProduct.Create(typeProduct);
                    decimal? oldprice;
                    if (CrawlServices.Business.IsCheapProduct(Shop, product.ItemId, product.Price,out oldprice))
                    {
                        Db.Upsert(CheapProduct.Create(Shop, product.ItemId, product.Title, product.Price,oldprice, typeProduct.PicUrl));
                    }
                    var result = Db.Upsert(product);
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
    }

    [TableName("db_taobao.dbo.td_data")]
    public class TaoBaoProduct
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

        public int? CommentCount { get; set; }

        public string PicUrl { get; set; }

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
            int _commentCount = 0;
            if (int.TryParse(typeProduct.CommentCount, out _commentCount))
            {
                CommentCount = _commentCount;
            }
            PicUrl = typeProduct.PicUrl;
        }

        public static TaoBaoProduct Create(SpliderType.TypeProduct typeProduct)
        {
            TaoBaoProduct product = new TaoBaoProduct();
            product.Init(typeProduct);
            return product;
        }

    }
}
