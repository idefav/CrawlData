using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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
    public class DataAnalyzeTask : TaskBase, IAnalyzeTask
    {
        public DataAnalyzeModel DataAnalyzeModel { get { return base.Model as DataAnalyzeModel; } }
        public IDbObject Db { get; set; }

        public IDbObject DbData { get; set; }

        public CancellationToken CancellationToken { get; set; }
        public DataAnalyzeTask(ITaskModel taskModel) : base(taskModel)
        {
            Db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbAnalyze);
            DbData = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
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
            string shopold = "";
            DateTime breaktime = CrawlServices.Business.GetBreakTimeByTaskName(DataAnalyzeModel.TaskName,out shopold);
            int index = 0;
            if (!string.IsNullOrEmpty(shopold))
            {
                index = DataAnalyzeModel.Shops.ToList().IndexOf(shopold.ToString());
                index = index < 0 ? 0 : index;
            }
            
            for (int j = index; j < DataAnalyzeModel.Shops.Length; j++)
            {
                string shop = DataAnalyzeModel.Shops[j];
                Crawl.Common.Common.Log.LogInfo($"正在解析 {shop} ");
                ShopEnum shopEnum = ShopEnum.淘宝;
                Enum.TryParse(shop, out shopEnum);
                var dbname = CrawlServices.Business.GetShopConfigByName(shopEnum);
                string tablename = "td_data" /*+"_"+DateTime.Now.ToString("yyyyMMdd")*/;
                string currdbname = string.Format("{0}.dbo.{1}", dbname.DbTable,tablename);
                string conn = shopEnum == ShopEnum.天猫
                    ? AppSettings.COMMONSETTINGS.DbTmall
                    : AppSettings.COMMONSETTINGS.DbTaobao;
                string indexname = "ix_" + tablename + "_updatetime";
                // 判断该表是否存在updatetime索引
                if (!CrawlServices.Business.IndexIsExist(indexname,
                    conn))
                {
                    IDbObject tmpdb = DBOMaker.CreateDbObj(DBType.SQLServer, conn);
                    tmpdb.ExecuteSql(string.Format(SQL.DB_Taobao_data_createindex_updatetime, indexname,tablename));
                }


                string sql = "select * from " + currdbname + " where updatetime>=@updatetime order by updatetime asc ";

                //var models = Db.QueryModels<TaoBaoProduct>(sql, new { updatetime = breaktime });
                //var data= Db.QueryDataReader(sql, new {updatetime = breaktime});
                //List<TaoBaoProduct> models = new List<TaoBaoProduct>();
                using (IDataReader dr = DbData.QueryDataReader(sql, new { updatetime = breaktime }))
                {
                   
                    while (dr.Read())
                    {
                        try
                        {
                            TaoBaoProduct taoBaoProduct = new TaoBaoProduct();
                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                                PropertyInfo pi = typeof(TaoBaoProduct).GetProperty(dr.GetName(i));
                                if (pi != null)
                                {
                                    var v = dr.GetValue(i);
                                    if (v == DBNull.Value)
                                    {
                                        pi.SetValue(taoBaoProduct, null, null);
                                    }
                                    else
                                    {
                                        pi.SetValue(taoBaoProduct, v, null);
                                    }
                                }
                            }
                            if (string.IsNullOrEmpty(taoBaoProduct.ItemId))
                            {
                                continue;
                            }
                            UpdateProductInfo(taoBaoProduct, shopEnum);
                            //Analyze(taoBaoProduct, shopEnum, "M",-1);
                            //Analyze(taoBaoProduct, shopEnum, "Q",-3);
                            //Analyze(taoBaoProduct, shopEnum, "HY",-6);
                            Analyze(taoBaoProduct, shopEnum, "ALL",0);
                            //string updatetimesql =
                            //    "update db_crawlconfig.dbo.td_crawlconfig set currentkeyword=@currentkeyword where taskname=@taskname ";
                            //Db.ExecuteSql(updatetimesql,
                            //    parameters:
                            //        new
                            //        {
                            //            taskname = DataAnalyzeModel.TaskName,
                            //            currentkeyword = taoBaoProduct.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "|" + shopEnum.ToString()
                            //        });
                            CrawlServices.Business.UpdateBreakTimeByTaskName(DataAnalyzeModel.TaskName,taoBaoProduct.UpdateTime,shopEnum.ToString());
                            //models.Add(model);
                        } catch (Exception exception) {
                           Crawl.Common.Common.Log.LogError(exception.ToString());
                        }
                    }
                }

                Crawl.Common.Common.Log.LogInfo($"解析 {shop} 完成 ");

            }

            //foreach (string shop in DataAnalyzeModel.Shops)
            //{
                

            //    //if (models != null && models.Count > 0)
            //    //{
            //    //    foreach (TaoBaoProduct taoBaoProduct in models)
            //    //    {
            //    //        // 月数据
            //    //        UpdateProductInfo(taoBaoProduct, shopEnum);
            //    //        Analyze(taoBaoProduct, shopEnum, "M");
            //    //        Analyze(taoBaoProduct, shopEnum, "Q");
            //    //        Analyze(taoBaoProduct, shopEnum, "HY");
            //    //        Analyze(taoBaoProduct, shopEnum, "Y");
            //    //        string updatetimesql =
            //    //            "update db_crawlconfig.dbo.td_crawlconfig set currentkeyword=@currentkeyword where taskname=@taskname ";
            //    //        Db.ExecuteSql(updatetimesql,
            //    //            parameters:
            //    //                new
            //    //                {
            //    //                    taskname = DataAnalyzeModel.TaskName,
            //    //                    currentkeyword = taoBaoProduct.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss fff")
            //    //                });
            //    //    }
            //    //}
            //    //else
            //    //{
            //    //    break;
            //    //}



            //}
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

        private void Analyze(TaoBaoProduct product, ShopEnum shopEnum, string type,int months)
        {
            if (!product.Price.HasValue)
            {
                return;
            }

            // 查询出现有的数据
            var analyzemodel =
                Db.QueryModel<AnalyzeModel>(
                    "select * from db_analyze.dbo.td_data_" + type + " where productid=@productid and shop=@shop ",
                    new { productid = product.ItemId, shop = shopEnum.ToString() });
            //string starttime = DateTime.Now.AddMonths(months).ToString("yyyy-MM-dd");
            
            if (string.IsNullOrEmpty(analyzemodel?.ProductId) || string.IsNullOrEmpty(analyzemodel.Shop))
            {
                Dictionary<string, decimal> dictionary = new Dictionary<string, decimal> { { product.PDate.ToString("yyyy-MM-dd"), product.Price.Value } };
                analyzemodel = AnalyzeModel.Create(product.ItemId, shopEnum.ToString(), dictionary,
                    product.Price.Value, product.Price.Value, product.Price.Value);
                analyzemodel.MaxPrice = dictionary.Values.Max();
                analyzemodel.AvgPrice = dictionary.Values.Average();
                analyzemodel.MinPrice = dictionary.Values.Min();
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
                //var pricesnew = prices.Where(c => DateTime.Parse(c.Key) >= DateTime.Parse(starttime));
                analyzemodel.Prices = JsonConvert.SerializeObject(prices);
                analyzemodel.MaxPrice = prices.Values.Max();
                analyzemodel.AvgPrice = prices.Values.Average();
                analyzemodel.MinPrice = prices.Values.Min();
            }


            Db.Upsert(analyzemodel, "db_analyze.dbo.td_data_" + type);

        }
    }

    [TableName("db_analyze.dbo.td_data_m")]
    public class AnalyzeModel
    {
        public string Guid { get; set; }
        [PrimaryKey]
        public string ProductId { get; set; }
        [PrimaryKey]
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
            Shop = shop;
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
        [PrimaryKey]
        public string ProductId { get; set; }
        [PrimaryKey]
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
