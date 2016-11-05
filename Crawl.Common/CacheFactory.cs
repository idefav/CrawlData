//-----------------------------------------------------------------------
// <copyright file="CacheFactory.cs" company="Idefav , Ltd .">
//     Copyright (c) 2015 , All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Crawl.Common
{
    /// <summary>
    /// CacheFactory
    /// 缓存工厂类
    /// 
    /// 修改纪录
    /// 
    /// 2015-6-08版本：1.0 WuZiShu 创建文件。     
    /// 
    /// <author>
    ///     <name>WuZiShu</name>
    ///     <date>2015-6-08</date>
    /// </author>
    /// </summary>
    public class CacheFactory
    {
        /// <summary>
        /// 缓存处理
        /// </summary>
        /// <param name="cacheKey">缓存的Key</param>
        /// <param name="proc">处理函数</param>
        /// <param name="isCache">是否缓存</param>
        /// <param name="refreshCache">是否强制刷新</param>
        /// <param name="cacheTime"></param>
        /// <param name="cacheOnException">出现异常是否缓存</param>
        /// <returns></returns>
        public static T Cache<T>(string cacheKey, Func<T> proc, bool isCache = false, bool refreshCache = false, TimeSpan cacheTime = default(TimeSpan), Func<T, bool> cacheOnException = null)
        {
            // 测试
            //isCache = false;
            //-----
            if (cacheTime == default(TimeSpan))
            {
                cacheTime = TimeSpan.FromMilliseconds(30 * 1000);//设置默认缓存30秒
            }
            //
            DateTime absoluteTime = DateTime.Now + cacheTime;
            cacheTime = TimeSpan.Zero;
            T result;
            if (!isCache)//判断是否缓存
            {//不缓存
                result = proc();
            }
            else
            {
                result = CacheHelper.GetCache<T>(cacheKey);
                
                //缓存
                if (result != null) //判断是否有缓存
                {
                    //已经缓存
                    if (refreshCache)//是否强制刷新缓存
                    {
                        //强制刷新
                        result = ExecuteResult(cacheKey, proc, cacheTime, absoluteTime, cacheOnException);
                    }
                }
                else
                {
                    result = ExecuteResult(cacheKey, proc, cacheTime, absoluteTime, cacheOnException);
                }
            }

            return result;
        }



        /// <summary>
        /// 执行方法并缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="proc">产生数据的方法</param>
        /// <param name="cacheTime">缓存时间</param>
        /// <param name="absoluteTime"></param>
        /// <param name="cacheOnException"></param>
        /// <returns></returns>
        private static T ExecuteResult<T>(string cacheKey, Func<T> proc, TimeSpan cacheTime, DateTime absoluteTime, Func<T, bool> cacheOnException = null)
        {
            // 未缓存
            var result = proc();

            if (result != null)
            {
                // 默认缓存异常错误数据
                if (cacheOnException == null)
                {
                    
                    CacheHelper.SetCache(cacheKey, result, absoluteTime, cacheTime);
                }

                // 异常处理不为空的时候,根据处理返回值判断是否缓存异常数据
                if (cacheOnException != null && cacheOnException(result))
                {
                    // 缓存数据
                    CacheHelper.SetCache(cacheKey, result, absoluteTime, cacheTime);
                }
            }

            return result;
        }
       
        
    }

    

   

}