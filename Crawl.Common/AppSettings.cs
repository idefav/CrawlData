//-----------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Eastmoney , Ltd .">
//     Copyright (c) 2016 , All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Crawl.Common.Interface;
using Newtonsoft.Json;

namespace Crawl.Common
{
    /// <summary>
    /// AppSettings
    /// 全局应用程序设置类
    /// 
    /// 修改纪录
    /// 
    /// 2016/9/12 10:56:40版本：1.0 WuZiShu 创建文件。     
    /// 
    /// <author>
    ///     <name>WuZiShu</name>
    ///     <date>2016/9/12 10:56:40</date>
    /// </author>
    /// </summary>
    public class AppSettings
    {
        #region 私有配置变量

        /// <summary>
        /// 配置目录
        /// </summary>
        private const string SettingsPath = "Settings";

        private static string AppSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingsPath);

        #endregion

        #region 获取配置文件 + GetSettings<T>()

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <typeparam name="T">配置模型</typeparam>
        /// <returns></returns>
        private static T GetSettings<T>(string fileanme="")
        {
            try
            {
                string filename = fileanme;
                if (string.IsNullOrEmpty(filename))
                {
                    filename = Path.Combine(AppSettingsPath, typeof(T).Name + ".json");
                }
                if (File.Exists(filename))
                {
                    string settingscontent = File.ReadAllText(filename, Encoding.UTF8);
                    var model = JsonConvert.DeserializeObject<T>(settingscontent);
                    return model;
                }

            }
            catch (Exception e)
            {
                Crawl.Common.Common.Log.LogError(e.ToString());
            }
            return default(T);
        }

      
        /// <summary>
        /// 获取配置列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_filename"></param>
        /// <returns></returns>
        private static List<T> GetSettingList<T>(string _filename="")
        {
            try
            {
                string filename = _filename;
                if (string.IsNullOrEmpty(filename))
                {
                    filename = Path.Combine(AppSettingsPath, typeof(T).Name + ".json");
                }
                if (File.Exists(filename))
                {
                    string settingscontent = File.ReadAllText(filename, Encoding.UTF8);
                    var model = JsonConvert.DeserializeObject<List<T>>(settingscontent);
                    return model;
                }

            }
            catch (Exception e)
            {
                Crawl.Common.Common.Log.LogError(e.ToString());
            }
            return new List<T>();
        }

        #endregion

        #region 加载函数 + AppSettings()

        /// <summary>
        /// 加载函数
        /// </summary>
        static AppSettings()
        {
            if (!Directory.Exists(AppSettingsPath))
            {
                Directory.CreateDirectory(AppSettingsPath);
            }

            COMMONSETTINGS = GetSettings<CommonSettings>();
            COMMONTASKS = GetSettings<CommonTaskModel>();
        }

        #endregion
        public static void ReloadSettings()
        {
            if (!Directory.Exists(AppSettingsPath))
            {
                Directory.CreateDirectory(AppSettingsPath);
            }

            COMMONSETTINGS = GetSettings<CommonSettings>();
            COMMONTASKS = GetSettings<CommonTaskModel>();
        }

        #region 配置

        /// <summary>
        /// 通用配置
        /// </summary>
        public static CommonSettings COMMONSETTINGS { get; set; }

        public static CommonTaskModel COMMONTASKS { get; set; }

        #endregion

    }

    #region 配置模型

    /// <summary>
    /// 应用程序通用配置
    /// </summary>
    public class CommonSettings
    {
        public string DbConn { get; set; }
        public string DbTaobao { get; set; }

        public string DbTmall { get; set; }

        public string DbAnalyze { get; set; }
        public string LogPath { get; set; }

        private int _interval = 10;
        public int Interval { get { return _interval; } set { _interval = value; } }
        public int TaskCount { get; set; }

        public List<string> RegexList { get; set; }

        public List<string> FileExtension { get; set; }

        public List<string> FileRegex { get; set; }
        public int AnalyzeInterval { get; set; }
    }

    public class CommonTaskModel
    {
        public FileConfigTaskModel[] FileConfigTask { get; set; }

        public TmallTaskModel[] TmallTask { get; set; }

        public TaoBaoTaskModel[] TaoBaoTask { get; set; }

        public DataAnalyzeModel[] DataAnalyzeTask { get; set; }
    }

    public class TmallTaskModel : ITaskModel
    {
        public bool Enable { get; set; }
        public string TaskName { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string[] KeyWords { get; set; }

        /// <summary>
        /// 并发数
        /// </summary>
        private int _taskcount = 1;
        public int TaskCount { get { return _taskcount; } set { _taskcount = value; } }

        /// <summary>
        /// 时间间隔
        /// </summary>
        private int _interval = 24 * 60 * 60;
        public int Interval { get { return _interval; } set { _interval = value; } }

        /// <summary>
        /// 抓取的页数
        /// </summary>
        private int _crawlpages = 5;

        public int CrawlPages
        {
            get { return _crawlpages; }
            set { _crawlpages = value; }
        }


    }

    public class TaoBaoTaskModel : ITaskModel
    {
        public bool Enable { get; set; }
        public string TaskName { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public KeyValuePair<string,string>[] KeyWords { get; set; }

        /// <summary>
        /// 并发数
        /// </summary>
        private int _taskcount = 1;
        public int TaskCount { get { return _taskcount; } set { _taskcount = value; } }

        /// <summary>
        /// 时间间隔
        /// </summary>
        private int _interval = 24 * 60 * 60;
        public int Interval { get { return _interval; } set { _interval = value; } }

        /// <summary>
        /// 抓取的页数
        /// </summary>
        private int _crawlpages = 5;

        public int CrawlPages
        {
            get { return _crawlpages; }
            set { _crawlpages = value; }
        }


    }

    public class DataAnalyzeModel : ITaskModel
    {

        public bool Enable { get; set; }
        public string TaskName { get; set; }
        public string[] Shops { get; set; }

        public int Interval { get; set; }
    }

    public class FileConfigTaskModel : ITaskModel
    {
        public bool Enable { get; set; }

        public string TaskName { get; set; }

        public List<string> FileRegex { get; set; }

        public ContentFilterModel ContentFilter { get; set; }
        public List<string> Dirs { get; set; }
        public List<string> SVNDir { get; set; }

    }

    public class ContentFilterModel
    {
        public List<string> Memcached { get; set; }
        public List<string> Mongodb { get; set; }
    }


    public class LogFileCompareModel
    {
        public string name { get; set; }
        public List<LogFileCompareModel_HostIp> servers { get; set; }

        public string path { get; set; }

        public int taskcount { get; set; }

        public bool enable { get; set; }

        public List<string> regexlist { get; set; }
    }

    public class LogFileCompareModel_HostIp
    {
        public string host { get; set; }
        public List<string> ips { get; set; }
    }

    /// <summary>
    /// 发送手机短信配置模型
    /// </summary>
    public class SendMessagePhoneModel
    {
        /// <summary>
        /// 数据部门净值组
        /// </summary>
        public string datajz { get; set; }

        /// <summary>
        /// 数据部门其他组
        /// </summary>
        public string dataother { get; set; }

        /// <summary>
        /// 数据规划
        /// </summary>
        public string datagh { get; set; }

        /// <summary>
        /// 研发
        /// </summary>
        public string development { get; set; }

        /// <summary>
        /// 监控
        /// </summary>
        public string monitor { get; set; }

        /// <summary>
        /// 获取多个组合
        /// </summary>
        /// <param name="phone">电话号码组合</param>
        /// <param name="split">分隔符</param>
        /// <returns></returns>
        public static string GetMultiGroup(string[] phone, char split = ',')
        {
            return string.Join(split.ToString(), phone);
        }

        /// <summary>
        /// 获取多个组合
        /// </summary>
        /// <param name="phone">电话号码组合</param>
        /// <returns></returns>
        public static string GetMultiGroup(params string[] phone)
        {
            return GetMultiGroup(phone, ',');
        }

    }

    #endregion



}
