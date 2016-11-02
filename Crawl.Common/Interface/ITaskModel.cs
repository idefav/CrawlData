//-----------------------------------------------------------------------
// <copyright file="ITaskModel.cs" company="Eastmoney , Ltd .">
//     Copyright (c) 2016 , All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Crawl.Common.Interface
{
    /// <summary>
    /// ITaskModel
    /// ITaskModel功能描述
    /// 
    /// 修改纪录
    /// 
    /// 2016/9/29 10:18:40版本：1.0 Administrator 创建文件。     
    /// 
    /// <author>
    ///     <name>Administrator</name>
    ///     <date>2016/9/29 10:18:40</date>
    /// </author>
    /// </summary>
    public interface ITaskModel
    {
        

        bool Enable { get; set; }

        string TaskName { get; set; }

    }
}
