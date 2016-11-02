//-----------------------------------------------------------------------
// <copyright file="ITask.cs" company="Eastmoney , Ltd .">
//     Copyright (c) 2016 , All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Crawl.Common.Interface
{
    /// <summary>
    /// ITask
    /// ITask功能描述
    /// 
    /// 修改纪录
    /// 
    /// 2016/9/29 10:14:38版本：1.0 Administrator 创建文件。     
    /// 
    /// <author>
    ///     <name>Administrator</name>
    ///     <date>2016/9/29 10:14:38</date>
    /// </author>
    /// </summary>
    public interface ITask<T> where T : ITaskModel
    {
        T Model { get; set; }

        Task<object> Task { get; set; }
        DateTime NextStartTime { get; set; }
        void Run();

        void Run(TaskScheduler taskScheduler);

        void Run(TaskScheduler taskScheduler, CancellationToken token);
    }
}
