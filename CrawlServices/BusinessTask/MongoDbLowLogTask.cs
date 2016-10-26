//-----------------------------------------------------------------------
// <copyright file="MongoDbLowLogTask.cs" company="Eastmoney , Ltd .">
//     Copyright (c) 2016 , All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CrawlServices.Interface;
using CrawlServices.Model;

namespace CrawlServices.BusinessTask
{
    /// <summary>
    /// MongoDbLowLogTask
    /// Mongodb慢查询日志分析
    /// 
    /// 修改纪录
    /// 
    /// 2016/10/8 16:23:53版本：1.0 Administrator 创建文件。     
    /// 
    /// <author>
    ///     <name>Administrator</name>
    ///     <date>2016/10/8 16:23:53</date>
    /// </author>
    /// </summary>
    public class MongoDbLowLogTask : TaskBase
    {
        public MongodbLowLogTaskModel MongodbLowLogTaskModel { get { return base.Model as MongodbLowLogTaskModel; } }
        private static object lockObj = new object();
        public Dictionary<string, MongdbQueryLogModel> Matches { get; set; }
        public CancellationToken CancellationToken { get; set; }



        public MongoDbLowLogTask(ITaskModel taskModel) : base(taskModel)
        {

        }


        public override void Run(TaskScheduler taskScheduler, CancellationToken token)
        {
            this.CancellationToken = token;
            Matches = new Dictionary<string, MongdbQueryLogModel>();
            Task = new Task<object>(() =>
            {
                Business();
                return Matches;
            }, token);
            Task.Start(taskScheduler);
        }

        private void Business()
        {
            // 任务开始
            OnTaskStartEvent(new TaskEventHandlerArgs("任务开始"));

            ProcessTask();

            // 任务结束
            OnTaskStopEvent(new TaskEventHandlerArgs("任务结束"));
        }

        private void SortMatchedList()
        {
            var matches_ordered = Matches.ToList().OrderByDescending(c => c.Value.Delay).ToList();
            long fs = DateTime.UtcNow.Ticks;
            for (int i = 0; i < matches_ordered.Count; i++)
            {
                var kv = matches_ordered[i];
                OnTaskProcessEvent(new TaskProcessEventHandlerArgs("[" + kv.Value.Delay + "]" + kv.Key, fs, "MongoDbLowLogTask_sorted"));
            }

            Matches.Clear();
        }

        private void ReadFileSync(string file, Encoding encoding, Action<string[]> action)
        {
            StreamReader sr = new StreamReader(file, encoding);
            List<string> liststr = new List<string>();
            string line = sr.ReadLine();
            liststr.Add(line);
            //long i = 1;

            while (!string.IsNullOrEmpty(line))
            {
                line = sr.ReadLine();
                liststr.Add(line);
                //i++;

                if (liststr.Count >= MongodbLowLogTaskModel.ProcLineCount)
                {
                    action(liststr.ToArray());
                    SortMatchedList();
                    liststr.Clear();
                    //i = 1;
                }
            }
            if(liststr.Count>0)
            action(liststr.ToArray());
            SortMatchedList();
            liststr.Clear();

        }

        /// <summary>
        /// 任务处理
        /// </summary>
        private void ProcessTask()
        {
            foreach (string path in MongodbLowLogTaskModel.LogDir)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string[] files = Directory.GetFiles(path, MongodbLowLogTaskModel.FileExt);
                for (int i = 0; i < files.Length; i++)
                {
                    string file = files[i];
                    ReadFileSync(file, Encoding.Default, lines =>
                     {
                       //var lines = File.ReadAllLines(file, Encoding.Default);
                       ParallelOptions options = new ParallelOptions();
                         options.CancellationToken = CancellationToken;
                         options.MaxDegreeOfParallelism = MongodbLowLogTaskModel.TaskCount;
                         long totallen = lines.Length;
                         
                         var result= Parallel.For(0, totallen-1, options, k =>
                         {
                             var l = lines[k];
                             if (l.Contains("I QUERY"))
                             {
                                 var model = GetMonogdbQuery(l);
                                 if (model != null)
                                 {
                                     string key = model.ToString();
                                     lock (lockObj)
                                     {
                                         if (!Matches.ContainsKey(key))
                                         {

                                             Matches.Add(key, model);
                                         }

                                     }

                                     OnTaskProcessEvent(new TaskProcessEventHandlerArgs(string.Format("[{0}]{1}", file, l),
                                         totallen, "MongoDbLowLogTask_proc"));
                                 }
                                 
                             }
                             else if(l.Contains("I COMMAND"))
                             {
                                 var model = GetMonogdbCommand(l);
                                 if (model != null)
                                 {
                                     string key = model.ToString();
                                     lock (lockObj)
                                     {
                                         if (!Matches.ContainsKey(key))
                                         {

                                             Matches.Add(key, model);
                                         }

                                     }
                                     OnTaskProcessEvent(new TaskProcessEventHandlerArgs(string.Format("[{0}]{1}", file, l),
                                         totallen, "MongoDbLowLogTask_proc"));
                                 }
                                 
                             }
                         });
                         while (!result.IsCompleted)
                         {
                             Thread.Sleep(100);
                         }
                     });


                    //for (int j = 0; j < lines.Length; j++)
                    //{
                    //    var l = lines[j];
                    //    if (l.Contains("I QUERY"))
                    //    {
                    //        var model = GetMonogdbQuery(l);
                    //        Matches.Add(l, model);
                    //        OnTaskProcessEvent(new TaskProcessEventHandlerArgs(string.Format("[{0}]{1}", file, l), 0));
                    //    }
                    //}
                }
            }
        }

        private MongdbQueryLogModel GetMonogdbQuery(string input)
        {
            Regex regex = new Regex(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}\+\d{4} I QUERY    \[conn\d{1,}\] (.+?) (.+?\..+?) query:(.*?) planSummary:(.*?) (\d+?)ms");
            var match = regex.Match(input);
            MongdbQueryLogModel mongdbQueryLogModel = new MongdbQueryLogModel();
            if (match.Success)
            {
                mongdbQueryLogModel.Query = match.Groups[1].Value.ToString();
                mongdbQueryLogModel.DataBaseAndCollection = match.Groups[2].Value.ToString();
                mongdbQueryLogModel.QueryWhere = match.Groups[3].Value;
                mongdbQueryLogModel.PlanSummary = match.Groups[4].Value;
                mongdbQueryLogModel.Delay = int.Parse(match.Groups[5].Value.ToString());
                mongdbQueryLogModel.AllText = input;
            }
            else
            {
                return null;
            }
            return mongdbQueryLogModel;
        }

        private MongdbQueryLogModel GetMonogdbCommand(string input)
        {
            Regex regex = new Regex(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}\+\d{4} I COMMAND  \[conn\d{1,}\] (.+?) (.+?\..+?) count:(.*?) planSummary:(.*?) (\d+?)ms");
            var match = regex.Match(input);
            MongdbQueryLogModel mongdbQueryLogModel = new MongdbQueryLogModel();
            if (match.Success)
            {
                mongdbQueryLogModel.Query = match.Groups[1].Value.ToString();
                mongdbQueryLogModel.DataBaseAndCollection = match.Groups[2].Value.ToString();
                mongdbQueryLogModel.QueryWhere = match.Groups[3].Value;
                mongdbQueryLogModel.PlanSummary = match.Groups[4].Value;
                mongdbQueryLogModel.Delay = int.Parse(match.Groups[5].Value.ToString());
                mongdbQueryLogModel.AllText = input;
            }
            else
            {
                return null;
            }
            return mongdbQueryLogModel;
        }
    }

    public class MongdbQueryLogModel
    {
        public string Query { get; set; }

        public string DataBaseAndCollection { get; set; }

        public string QueryWhere { get; set; }
        public string PlanSummary { get; set; }
        public string AllText { get; set; }
        public int Delay { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", Query, DataBaseAndCollection, PlanSummary, QueryWhere);
        }
    }

    public class MongodbLowLogTaskModel : ITaskModel
    {
        public bool Enable { get; set; }
        public string TaskName { get; set; }

        /// <summary>
        /// 文件筛选
        /// </summary>
        public string FileExt { get; set; }

        private int _taskcount = 5;
        /// <summary>
        /// 并行任务数量
        /// </summary>
        public int TaskCount
        {
            get { return _taskcount; }
            set { _taskcount = value; }
        }

        private long _procLineCount = 10000;

        /// <summary>
        /// 一次处理的行数
        /// </summary>
        public long ProcLineCount
        {
            get { return _procLineCount; }
            set { _procLineCount = value; }
        }

        /// <summary>
        /// 日志文件目录
        /// </summary>
        public List<string> LogDir { get; set; }
    }
}
