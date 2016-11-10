//-----------------------------------------------------------------------
// <copyright file="TaskManager.cs" company="Eastmoney , Ltd .">
//     Copyright (c) 2016 , All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Crawl.Common;
using Crawl.Common.Interface;

namespace CrawlServices
{
    /// <summary>
    /// TaskManager
    /// TaskManager功能描述
    /// 
    /// 修改纪录
    /// 
    /// 2016/9/29 10:33:01版本：1.0 Administrator 创建文件。     
    /// 
    /// <author>
    ///     <name>Administrator</name>
    ///     <date>2016/9/29 10:33:01</date>
    /// </author>
    /// </summary>
    public class TaskManager
    {
        private TaskScheduler taskScheduler = new WorkStealingTaskScheduler(AppSettings.COMMONSETTINGS.TaskCount);
        public Dictionary<string,ITask<ITaskModel>> Tasks { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }

        public void Init()
        {
            CancellationTokenSource = new CancellationTokenSource();
            if (Tasks == null)
            {
                Tasks=new Dictionary<string, ITask<ITaskModel>>();
            }
            if (AppSettings.COMMONTASKS!=null)
            {
                var propertes = AppSettings.COMMONTASKS.GetType().GetProperties();
                foreach (PropertyInfo propertyInfo in propertes)
                {
                    
                    var propertv = propertyInfo.GetValue(AppSettings.COMMONTASKS, null) as ITaskModel[];
                    if (propertv!=null )
                    {
                        foreach (ITaskModel taskModel in propertv)
                        {
                            if (taskModel.Enable)
                            {
                                string taskname = this.GetType().Namespace + ".BusinessTask." + propertyInfo.Name;
                                var task = Activator.CreateInstance(Type.GetType(this.GetType().Namespace + ".BusinessTask." + propertyInfo.Name, true), new object[] { taskModel });
                                Tasks.Add(taskname + "." + taskModel.TaskName, (ITask<ITaskModel>)task);
                            }
                           
                        }
                        //var itemsProp = propertv.GetType().GetProperty("Items") ;
                        //var itemsvalue = itemsProp.GetValue(propertv, null) as ITaskItemModel[];
                        //if (itemsvalue != null)
                        //{
                        //    foreach (ITaskItemModel taskItemModel in itemsvalue)
                        //    {
                        //        var task = Activator.CreateInstance(Type.GetType(propertv.TaskName, true), new object[] { taskItemModel });
                        //        Tasks.Add(propertv.TaskName, (ITask<ITaskItemModel>)task);
                        //    }
                        //}
                    }
                }
            }
        }

        public void Run(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            foreach (KeyValuePair<string, ITask<ITaskModel>> keyValuePair in Tasks)
                            {
                                var task = keyValuePair.Value;
                                
                                if (task.Task != null)
                                {
                                    if (task is IAnalyzeTask)
                                    {
                                        if (task.Task.IsCompleted &&
                                        Business.NeedAnalyze(task.Model.TaskName))
                                        {
                                            //task.NextStartTime = DateTime.MinValue;
                                            Business.UpdateAnalyzeStart(task.Model.TaskName);
                                            task.Run(taskScheduler, cancellationToken);
                                        }
                                    }
                                    else
                                    {
                                        if (task.Task.IsCompleted &&
                                        Business.NeedCrawl(task.Model.TaskName))
                                        {
                                            //task.NextStartTime = DateTime.MinValue;
                                            Business.UpdateCrawlStart(task.Model.TaskName);
                                            task.Run(taskScheduler, cancellationToken);
                                        }
                                    }
                                    
                                }
                                else
                                {
                                    if (task is IAnalyzeTask)
                                    {
                                        if (Business.NeedAnalyze(task.Model.TaskName))
                                        {
                                            Business.UpdateAnalyzeStart(task.Model.TaskName);
                                            //task.NextStartTime = DateTime.MinValue;
                                            task.Run(taskScheduler, cancellationToken);
                                        }
                                    }
                                    else
                                    {
                                        if (Business.NeedCrawl(task.Model.TaskName))
                                        {
                                            Business.UpdateCrawlStart(task.Model.TaskName);
                                            //task.NextStartTime = DateTime.MinValue;
                                            task.Run(taskScheduler, cancellationToken);
                                        }
                                    }
                                        
                                   
                                }
                            
                            

                            }
                        }
                        catch (Exception e)
                        {
                            Crawl.Common.Common.Log.LogError(e.ToString());
                        }
                        Thread.Sleep(10000);
                    }
                }
                catch (Exception e)
                {
                    Crawl.Common.Common.Log.LogError(e.ToString());
                }

            }, cancellationToken);

        }

        public void Run(string taskname,CancellationToken token)
        {
            Tasks[taskname].Run(taskScheduler,token);
        }

        public ITask<ITaskModel> GetTaskByName(string taskname)
        {
            return Tasks[taskname];
        } 
    }
}
