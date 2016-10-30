﻿//-----------------------------------------------------------------------
// <copyright file="Common.cs" company="Eastmoney , Ltd .">
//     Copyright (c) 2016 , All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
    public class Common
    {
        public static ILog CommonLog { get; set; }

        static Common()
        {
            CommonLog = new TxtLog();
        }

        public static bool IsCrawlComplete(string taskname)
        {
            IDbObject db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            var model = db.QueryModel<CrawlConfig>(
                 " select * from DB_CrawlConfig.dbo.td_crawlconfig where taskname=" + db.GetParameterName("taskname"),
                 new { taskname = taskname });
            if (model != null)
            {
                return model.UpdatedDay >= DateTime.Now.Date&&model.Status;
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
                return model.UpdatedDay < DateTime.Now.Date ;
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
                return new KeyValuePair<string, int?>(model.CurrentKeyWord,model.CurrentPage);
            }
            return new KeyValuePair<string, int?>(null,null);
        }

        public static void UpdateCrawlStart(string taskname)
        {
            IDbObject db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            CrawlConfig config=new CrawlConfig();
            config.Guid = Guid.NewGuid().ToString();
            
            config.TaskName = taskname;
            config.TimeUsed = 0;
            config.Status = false;
            if (NewDayOfCrawl(taskname))
            {
                config.UpdatedDay = DateTime.Now.Date;
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
            if (model!=null)
            {
                return (model.UpdatedDay<DateTime.Now.Date&&model.Status)||!model.UpdatedDay.HasValue;
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
            config.CurrentKeyWord = null;
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
