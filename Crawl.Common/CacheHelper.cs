//-----------------------------------------------------------------------
// <copyright file="CacheHelper.cs" company="EastMoney , Ltd .">
//     Copyright (c) 2015 , All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace Crawl.Common
{
    /// <summary>
    /// CacheHelper
    /// 
    /// 缓存帮助类
    /// 
    /// 修改纪录
    /// 
    /// 2015-4-13版本：1.0 WuZiShu 创建文件。     
    /// 
    /// <author>
    ///     <name>WuZiShu</name>
    ///     <date>2015-4-13</date>
    /// </author>
    /// </summary>
    public class CacheHelper
    {
        /// <summary>
        /// 获取数据缓存
        /// </summary>
        /// <param name="cacheKey">键</param>
        public static T GetCache<T>(string cacheKey)
        {
            Cache objCache = HttpRuntime.Cache;
            return (T)objCache[cacheKey];
        }

        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public static void SetCache<T>(string cacheKey, T objObject)
        {
            Cache objCache = HttpRuntime.Cache;
            objCache.Insert(cacheKey, objObject);
        }

        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public static void SetCache<T>(string cacheKey, T objObject, TimeSpan timeout)
        {
            Cache objCache = HttpRuntime.Cache;
            objCache.Insert(cacheKey, objObject, null,Cache.NoAbsoluteExpiration,timeout, CacheItemPriority.Normal, null);
        }

        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public static void SetCache<T>(string cacheKey, T objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            Cache objCache = HttpRuntime.Cache;
            objCache.Insert(cacheKey, objObject, null, absoluteExpiration, slidingExpiration);
        }

        /// <summary>
        /// 移除指定数据缓存
        /// </summary>
        public static void RemoveCache(string cacheKey)
        {
            Cache cache = HttpRuntime.Cache;
            cache.Remove(cacheKey);
        }

        /// <summary>
        /// 移除全部缓存
        /// </summary>
        public static void RemoveAllCache()
        {
            Cache cache = HttpRuntime.Cache;
            IDictionaryEnumerator cacheEnum = cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                cache.Remove(cacheEnum.Key.ToString());
            }
        }
    }
}
