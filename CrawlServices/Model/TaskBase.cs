//-----------------------------------------------------------------------
// <copyright file="TaskBase.cs" company="Eastmoney , Ltd .">
//     Copyright (c) 2016 , All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using CrawlServices.Interface;

namespace CrawlServices.Model
{
    /// <summary>
    /// TaskBase
    /// TaskBase功能描述
    /// 
    /// 修改纪录
    /// 
    /// 2016/9/29 10:28:33版本：1.0 Administrator 创建文件。     
    /// 
    /// <author>
    ///     <name>Administrator</name>
    ///     <date>2016/9/29 10:28:33</date>
    /// </author>
    /// </summary>
    public class TaskBase:ITask<ITaskModel>
    {
        public delegate void TaskStartEventHandler(object sender,TaskEventHandlerArgs e);
        public delegate void TaskStopEventHandler(object sender, TaskEventHandlerArgs e);

        public delegate void TaskProcessEventHandler(object sender, TaskProcessEventHandlerArgs e);
        
        public event TaskStartEventHandler TaskStartEvent;
        public event TaskStopEventHandler TaskStopEvent;
        public event TaskProcessEventHandler TaskProcessEvent;

        public void OnTaskStartEvent(TaskEventHandlerArgs args)
        {
            if (TaskStartEvent!=null)
            {
                TaskStartEvent(this,args);
            }
        }

        public void OnTaskStopEvent(TaskEventHandlerArgs args)
        {
            if (TaskStartEvent != null)
            {
                TaskStopEvent(this, args);
            }
        }

        public void OnTaskProcessEvent(TaskProcessEventHandlerArgs args)
        {
            if (TaskProcessEvent != null)
            {
                TaskProcessEvent(this, args);
            }
        }

        public ITaskModel Model { get; set; }

        public Task<object> Task { get; set; }

        public TaskBase(ITaskModel taskModel)
        {
            this.Model = taskModel;
        }
        

        public virtual void Run()
        {
            
        }

        public virtual void Run(TaskScheduler taskScheduler) { }

        public virtual void Run(TaskScheduler taskScheduler,CancellationToken token) { }
    }

    public class TaskProcessEventHandlerArgs:EventArgs
    {
        public string Message { get; set; }

        public string PercentType { get; set; }

        public double Percent { get; set; }


        public TaskProcessEventHandlerArgs(string message, double percent)
        {
            this.Message = message;
            this.Percent = percent;
        }

        public TaskProcessEventHandlerArgs(string message, double percent,string percenttype)
        {
            this.Message = message;
            this.Percent = percent;
            this.PercentType = percenttype;
        }
    }

    public class TaskEventHandlerArgs : EventArgs
    {
        public string Message { get; set; }

        public TaskEventHandlerArgs(string message)
        {
            this.Message = message;
        }
    }
}
