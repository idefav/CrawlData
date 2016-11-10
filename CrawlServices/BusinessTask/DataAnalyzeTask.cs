using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crawl.Common;
using Crawl.Common.Interface;
using Crawl.Common.Model;
using Idefav.DbFactory;
using Idefav.IDAL;
using Idefav.Utility;
using Newtonsoft.Json;

namespace CrawlServices.BusinessTask
{
    public class DataAnalyzeTask : TaskBase,IAnalyzeTask
    {
        public DataAnalyzeModel DataAnalyzeModel { get { return base.Model as DataAnalyzeModel; } }
        public IDbObject Db { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public DataAnalyzeTask(ITaskModel taskModel) : base(taskModel)
        {
            Db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbAnalyze);
        }

        public override void Run(TaskScheduler taskScheduler, CancellationToken token)
        {
            this.CancellationToken = token;

            Task = new Task<object>(() =>
            {
                try
                {
                    Business();
                    NextStartTime = DateTime.Now.AddSeconds(DataAnalyzeModel.Interval);
                    //Common.UpdateCrawlStatus(TmallTaskModel.TaskName, true);
                    CrawlServices.Business.UpdateAnalyzeComplete(DataAnalyzeModel.TaskName);
                }
                catch (Exception e)
                {
                    Crawl.Common.Common.Log.LogError(e.ToString());
                }
                return null;
            }, token);
            Task.Start(taskScheduler);
        }



        private void Business()
        {
            foreach (string shop in DataAnalyzeModel.Shops)
            {
                ShopEnum shopEnum = ShopEnum.淘宝;
                Enum.TryParse(shop, out shopEnum);
                var dbname = CrawlServices.Business.GetShopConfigByName(shopEnum);
                string currdbname = string.Format("{0}.dbo.td_data_{1}", dbname, DateTime.Now.ToString("yyyyMMdd"));

                DateTime breaktime = CrawlServices.Business.GetBreakTimeByTaskName(DataAnalyzeModel.TaskName);

                string sql = "select * from " + currdbname + " where updatetime>=@updatetime order by updatetime desc ";
                var models = Db.QueryModels<TaoBaoProduct>(sql, new { updatetime = breaktime });
                if (models != null)
                {
                    foreach (TaoBaoProduct taoBaoProduct in models)
                    {
                        // 月数据
                        UpdateProductInfo(taoBaoProduct, shopEnum);
                        Analyze(taoBaoProduct,shopEnum,"M");
                        Analyze(taoBaoProduct,shopEnum,"Q");
                        Analyze(taoBaoProduct,shopEnum,"HY");
                        Analyze(taoBaoProduct,shopEnum,"Y");
                    }
                }
            }
        }

        private void UpdateProductInfo(TaoBaoProduct product, ShopEnum shopEnum)
        {
            if (!product.Price.HasValue)
            {
                return;
            }
            var productinfo =
               Db.QueryModel<ProductInfoModel>(
                   "select * from db_analyze.dbo.td_productinfo where productid=@productid and shop=@shop ",
                   new { productid = product.ItemId, shop = shopEnum.ToString() });
            if (string.IsNullOrEmpty(productinfo?.ProductId) || string.IsNullOrEmpty(productinfo.Shop))
            {
                productinfo = ProductInfoModel.Create(product.ItemId, shopEnum.ToString(), product.Title, product.Price,
                    product.PicUrl, product.SellCount, product.CommentCount);
            }
            else
            {
                productinfo.PicUrl = product.PicUrl;
                productinfo.NowPrice = product.Price;
                productinfo.SellCount = product.SellCount;
                productinfo.CommentCount = product.CommentCount;
            }
            Db.Upsert(productinfo, "db_analyze.dbo.td_productinfo");
        }

        private void Analyze(TaoBaoProduct product, ShopEnum shopEnum,string type)
        {
            if (!product.Price.HasValue)
            {
                return;
            }

            // 查询出现有的数据
            var analyzemodel =
                Db.QueryModel<AnalyzeModel>(
                    "select * from db_analyze.dbo.td_data_"+type+" where productid=@productid and shop=@shop ",
                    new { productid = product.ItemId, shop = shopEnum.ToString() });

            if (string.IsNullOrEmpty(analyzemodel?.ProductId) || string.IsNullOrEmpty(analyzemodel.Shop))
            {
                Dictionary<string, decimal> dictionary = new Dictionary<string, decimal> { { product.PDate.ToString("yyyy-MM-dd"), product.Price.Value } };
                analyzemodel = AnalyzeModel.Create(product.ItemId, shopEnum.ToString(), dictionary,
                    product.Price.Value, product.Price.Value, product.Price.Value);
            }
            else
            {
                var prices = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(analyzemodel.Prices);
                string updateday = product.PDate.ToString("yyyy-MM-dd");
                if (prices.ContainsKey(updateday))
                {
                    prices[updateday] = product.Price.Value;
                }
                else
                {
                    prices.Add(updateday, product.Price.Value);
                }
            }


            Db.Upsert(analyzemodel, "db_analyze.dbo.td_data_" + type);

        }
    }

    [TableName("db_analyze.dbo.td_data_m")]
    public class AnalyzeModel
    {
        public string Guid { get; set; }
        public string ProductId { get; set; }
        public string Shop { get; set; }
        public string Prices { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? AvgPrice { get; set; }
        public DateTime UpdateTime { get; set; }

        public void Init(string productid, string shop, Dictionary<string, decimal> prices, decimal? minprice,
            decimal? maxprice, decimal? avgprice)
        {
            Guid = System.Guid.NewGuid().ToString();
            ProductId = productid;
            Shop = Shop;
            MinPrice = minprice;
            MaxPrice = maxprice;
            AvgPrice = avgprice;
            Prices = JsonConvert.SerializeObject(prices);
            UpdateTime = DateTime.Now;
        }

        public static AnalyzeModel Create(string productid, string shop, Dictionary<string, decimal> prices,
            decimal? minprice,
            decimal? maxprice, decimal? avgprice)
        {
            AnalyzeModel model = new AnalyzeModel();
            model.Init(productid, shop, prices, minprice, maxprice, avgprice);
            return model;
        }
    }

    [TableName("db_analyze.dbo.td_productinfo")]
    public class ProductInfoModel
    {
        public string Guid { get; set; }
        public string ProductId { get; set; }

        public string Shop { get; set; }
        public string ProductName { get; set; }
        public decimal? NowPrice { get; set; }
        public string PicUrl { get; set; }
        public int? SellCount { get; set; }
        public int? CommentCount { get; set; }
        public DateTime UpdateTime { get; set; }

        public void Init(string productid, string shop, string productname, decimal? nowprice, string picurl,
            int? sellcount, int? commentcount)
        {
            Guid = System.Guid.NewGuid().ToString();
            ProductId = productid;
            ProductName = productname;
            Shop = shop;
            NowPrice = nowprice;
            PicUrl = picurl;
            SellCount = sellcount;
            CommentCount = commentcount;
            UpdateTime = DateTime.Now;
        }

        public static ProductInfoModel Create(string productid, string shop, string productname, decimal? nowprice,
            string picurl,
            int? sellcount, int? commentcount)
        {
            ProductInfoModel model = new ProductInfoModel();
            model.Init(productid, shop, productname, nowprice, picurl, sellcount, commentcount);
            return model;
        }
    }
}
