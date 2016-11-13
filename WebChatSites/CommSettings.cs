using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebChatSites
{
    public class CommSettings
    {
        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        public static string DbConn { get; set; }
        public static string DbAnalyze { get; set; }
        public static string DbConfig { get; set; }

        static CommSettings()
        {
            DbConn = ConfigurationManager.AppSettings["DbConn"];
            DbAnalyze = ConfigurationManager.AppSettings["DbAnalyze"];
            DbConfig = ConfigurationManager.AppSettings["DbConfig"];
        }
    }
}