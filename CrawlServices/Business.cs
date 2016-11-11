//-----------------------------------------------------------------------
// <copyright file="Common.cs" company="Eastmoney , Ltd .">
//     Copyright (c) 2016 , All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crawl.Common;
using Idefav.DbFactory;
using Idefav.IDAL;
using Idefav.Utility;

namespace CrawlServices
{
    /// <summary>
    /// Common
    /// Common功能描述
    /// 
    /// 修改纪录
    /// 
    /// 2016/9/22 9:47:45版本：1.0 Administrator 创建文件。     
    /// 
    /// <author>
    ///     <name>Administrator</name>
    ///     <date>2016/9/22 9:47:45</date>
    /// </author>
    /// </summary>
    public class Business
    {


        public static bool IsCrawlComplete(string taskname)
        {
            IDbObject db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            var model = db.QueryModel<CrawlConfig>(
                 " select * from DB_CrawlConfig.dbo.td_crawlconfig where taskname=" + db.GetParameterName("taskname"),
                 new { taskname = taskname });
            if (model != null)
            {
                return model.UpdatedDay >= DateTime.Now.Date && model.Status;
            }
            return false;
        }


        /// <summary>
        /// 获取正常表名
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public static string GetTableName(string tablename)
        {
            if (string.IsNullOrEmpty(tablename))
            {
                return tablename;
            }
            string[] tablenames = tablename.Split('.');
            if (tablenames.Length > 1)
            {
                return tablenames.Last();
            }
            return tablename;
        }

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public static bool TableIsExist(string tablename, string dbconn)
        {
            IDbObject db = DBOMaker.CreateDbObj(DBType.SQLServer, dbconn);

            var table =
                db.QueryDataTable(
                    "SELECT * FROM dbo.SysObjects WHERE ID = object_id(N'[" + GetTableName(tablename) + "]') AND OBJECTPROPERTY(ID, 'IsTable') = 1");
            if (table != null && table.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        public static bool NeedCrawl(string taskname)
        {
            IDbObject db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            var model = db.QueryModel<CrawlConfig>(
                 " select * from DB_CrawlConfig.dbo.td_crawlconfig where taskname=" + db.GetParameterName("taskname"),
                 new { taskname = taskname });
            if (model != null)
            {
                return model.UpdatedDay < DateTime.Now.Date || !model.Status;
            }
            return true;
        }

        public static bool NeedAnalyze(string taskname)
        {
            IDbObject db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            var model = db.QueryModel<CrawlConfig>(
                 " select * from DB_CrawlConfig.dbo.td_crawlconfig where taskname=" + db.GetParameterName("taskname"),
                 new { taskname = taskname });
            if (model != null)
            {
                return (model.UpdatedDay.HasValue && (DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH")+":00:00") - model.UpdatedDay.Value).TotalHours >= 2) || !model.Status;
            }
            return true;
        }

        public static KeyValuePair<string, int?> GetCurrBreakpoint(string taskname)
        {
            IDbObject db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            var model = db.QueryModel<CrawlConfig>(
                 " select * from DB_CrawlConfig.dbo.td_crawlconfig where taskname=" + db.GetParameterName("taskname"),
                 new { taskname = taskname });
            if (model != null)
            {
                return new KeyValuePair<string, int?>(model.CurrentKeyWord, model.CurrentPage);
            }
            return new KeyValuePair<string, int?>(null, null);
        }

        public static void UpdateCrawlStart(string taskname)
        {
            IDbObject db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            CrawlConfig config = new CrawlConfig();
            var config1 = db.QueryModel<CrawlConfig>("select * from db_crawlconfig.dbo.td_crawlconfig where taskname=@taskname",
                    new { taskname = taskname });
            if (config1 != null)
            {
                config = config1;
            }
            config.Guid = Guid.NewGuid().ToString();

            config.TaskName = taskname;
            config.TimeUsed = 0;
            if (NewDayOfCrawl(taskname))
            {
                config.Status = false;
                config.UpdatedDay = DateTime.Now.Date;
            }
            config.UpdateTime = DateTime.Now;
            db.Upsert(config);
        }

        public static void UpdateAnalyzeStart(string taskname)
        {
            IDbObject db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            CrawlConfig config = new CrawlConfig();
            var config1 = db.QueryModel<CrawlConfig>("select * from db_crawlconfig.dbo.td_crawlconfig where taskname=@taskname",
                    new { taskname = taskname });
            if (config1 != null)
            {
                config = config1;
            }
            config.Guid = Guid.NewGuid().ToString();

            config.TaskName = taskname;
            config.TimeUsed = 0;
            if (NewOfAnalyze(taskname))
            {
                config.Status = false;
                config.UpdatedDay = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH")+":00:00");
            }
            config.UpdateTime = DateTime.Now;
            db.Upsert(config);
        }

        public static bool NewDayOfCrawl(string taskname)
        {
            IDbObject db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            var model = db.QueryModel<CrawlConfig>(
                " select * from DB_CrawlConfig.dbo.td_crawlconfig where taskname=" + db.GetParameterName("taskname"),
                new { taskname = taskname });
            if (model != null)
            {
                return (model.UpdatedDay < DateTime.Now.Date && model.Status) || !model.UpdatedDay.HasValue;
            }
            return false;
        }

        public static bool NewOfAnalyze(string taskname)
        {
            IDbObject db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            var model = db.QueryModel<CrawlConfig>(
                " select * from DB_CrawlConfig.dbo.td_crawlconfig where taskname=" + db.GetParameterName("taskname"),
                new { taskname = taskname });
            if (model != null)
            {
                return (model.UpdatedDay.HasValue && (DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH")+":00:00") - model.UpdatedDay.Value).TotalHours >= 2 && model.Status) || !model.UpdatedDay.HasValue;
            }
            return false;
        }

        public static void UpdateCrawlComplete(string taskname)
        {
            IDbObject db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            CrawlConfig config = new CrawlConfig();
            config.Guid = Guid.NewGuid().ToString();

            config.TaskName = taskname;
            config.TimeUsed = 0;
            config.Status = true;
            config.CurrentKeyWord = "";
            config.CurrentPage = 0;
            config.UpdateTime = DateTime.Now;
            db.Upsert(config);
        }

        public static void UpdateAnalyzeComplete(string taskname)
        {
            IDbObject db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            CrawlConfig config = new CrawlConfig();
            config.Guid = Guid.NewGuid().ToString();

            config.TaskName = taskname;
            config.TimeUsed = 0;
            config.Status = true;
            config.CurrentKeyWord = "";
            config.CurrentPage = 0;
            config.UpdateTime = DateTime.Now;
            db.Upsert(config);
        }

        public static void UpdateCrawlBreakpoint(string taskname, string keywords, int page)
        {
            IDbObject db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            CrawlConfig config = new CrawlConfig();
            //config.UpdatedDay = DateTime.Now.Date;
            config.TaskName = taskname;
            config.CurrentKeyWord = keywords;
            config.CurrentPage = page;
            config.UpdateTime = DateTime.Now;
            db.Update(config);
        }

        /// <summary>
        /// 是否是降价商品
        /// </summary>
        /// <param name="shop">店铺名称</param>
        /// <param name="productid">商品编号</param>
        /// <param name="price">价格</param>
        /// <param name="oldprice">原价</param>
        /// <returns></returns>
        public static bool IsCheapProduct(string shop, string productid, decimal? price, out decimal? oldprice)
        {
            IDbObject db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            StringBuilder stringBuilder = new StringBuilder(@"select nowprice from DB_Analyze.dbo.td_productinfo where shop = @shop and productid = @productid; ");
            //ShopEnum shopEnum = ShopEnum.天猫;
            //Enum.TryParse(shop, out shopEnum);
            //stringBuilder.Append(GetShopConfigByName(shopEnum).DbTable);
            //stringBuilder.Append(" where itemid=@itemid ");
            //stringBuilder.Append(" order by updatetime desc ");
            var table = db.QueryDataTable(stringBuilder.ToString(), new { shop = shop, productid = productid });
            if (table != null && table.Rows.Count > 0)
            {
                oldprice = table.Rows[0][0] as decimal?;

                if (oldprice.HasValue && price < oldprice)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                oldprice = null;
            }

            return false;
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
        /// 获取断点时间
        /// </summary>
        /// <param name="taskname"></param>
        /// <returns></returns>
        public static DateTime GetBreakTimeByTaskName(string taskname,out string shopEnum)
        {
            DateTime result = DateTime.Parse("1990-01-01");
            string sql = "select * from db_crawlconfig.dbo.td_crawlconfig where taskname=@taskname";
            IDbObject Db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            var model = Db.QueryModel<CrawlConfig>(sql, new { taskname = taskname });
            if (string.IsNullOrEmpty(model.CurrentKeyWord))
            {
                shopEnum = "";
                return result;
            }
            //ShopEnum shopEnum;
            string[] currKeyWord = model.CurrentKeyWord.Split('|');
            DateTime.TryParse(currKeyWord[0], out result);
            shopEnum = currKeyWord[1];
            return result;
        }
    }

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
    public enum ShopEnum
    {
        天猫,
        淘宝,
        淘宝Test
    }
    [TableName("db_crawlconfig.dbo.td_crawlconfig")]
    public class CrawlConfig
    {
        public string Guid { get; set; }
        public DateTime? UpdatedDay { get; set; }
        [PrimaryKey]
        public string TaskName { get; set; }
        public int? TimeUsed { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool Status { get; set; }
        public string CurrentKeyWord { get; set; }
        public int? CurrentPage { get; set; }


    }
}
