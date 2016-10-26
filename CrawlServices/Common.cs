//-----------------------------------------------------------------------
// <copyright file="Common.cs" company="Eastmoney , Ltd .">
//     Copyright (c) 2016 , All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

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
            CommonLog=new TxtLog();
        }
    }
}
