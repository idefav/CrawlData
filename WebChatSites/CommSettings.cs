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
        public static string Token { get; set; }

        static CommSettings()
        {
            DbConn = ConfigurationManager.AppSettings["DbConn"];
            DbAnalyze = ConfigurationManager.AppSettings["DbAnalyze"];
            DbConfig = ConfigurationManager.AppSettings["DbConfig"];
            Token = ConfigurationManager.AppSettings["Token"];
        }

        
    }

    public static class CommonEx
    {
        public static string GetDateString(this DateTime date)
        {
            Dictionary<string, Func<bool>> dict = new Dictionary<string, Func<bool>>
            {
                { "刚刚",()=>DateTime.Now - date<TimeSpan.FromSeconds(30)},
                { $"{(DateTime.Now - date).Seconds}秒前",()=>DateTime.Parse(date.ToString("yyyy-MM-dd HH:mm")+":00")==DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm")+":00")},
                { $"{date.ToString("mm分ss秒")}",()=>DateTime.Parse(date.ToString("yyyy-MM-dd HH")+":00:00")==DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH")+":00:00")},
                { $"{date.ToString("HH时mm分")}",()=>DateTime.Parse(date.ToString("yyyy-MM-dd"))==DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"))},
                { $"昨天{date.ToString("HH时mm分")}",()=>(DateTime.Parse(date.ToString("yyyy-MM-dd"))==DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")))},
                { $"前天{date.ToString("HH时mm分")}",()=>(DateTime.Parse(date.ToString("yyyy-MM-dd"))==DateTime.Parse(DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd")))},
                { $"今年{date.ToString("MM月dd日")}",()=>date.Year==DateTime.Now.Year},
                { $"很久以前",()=>true},
            };

            foreach (KeyValuePair<string, Func<bool>> keyValuePair in dict)
            {
                if (keyValuePair.Value())
                {
                    return keyValuePair.Key;
                }
            }
            return date.ToString("yyyy-MM-dd HH:mm:ss");


        }
    }
}