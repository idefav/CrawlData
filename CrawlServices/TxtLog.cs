//-----------------------------------------------------------------------
// <copyright file="TxtLog.cs" company="Fund , Ltd .">
//     Copyright (c) 2015 , All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CrawlServices
{
    /// <summary>
    ///     TxtLog
    ///     日志(写文本文件 支持多线程同时写)
    ///     修改纪录
    ///     2015-8-17版本：1.0 WuZiShu 创建文件。
    ///     <author>
    ///         <name>WuZiShu</name>
    ///         <date>2015-8-17</date>
    ///     </author>
    /// </summary>
    public class TxtLog : ILog
    {

        public enum LogType
        {
            Info,
            Warn,
            Error
        }

        public void LogInfo(string message)
        {
            LogHelper.LogInfo(message);
        }

        public void LogInfo(string message,string filename)
        {
            LogHelper.LogInfo(message,filename);
        }

        public void LogError(string message)
        {
            LogHelper.LogError(message);
        }

        public void LogError(string message,string filename)
        {
            LogHelper.LogError(message,filename);
        }

        public void LogWarn(string message)
        {
            LogHelper.LogWarn(message);
        }

        public void LogWarn(string message,string filename)
        {
            LogHelper.LogWarn(message,filename);
        }

        public class LogHelper
        {
            private static readonly Thread WriteThread;
            private static readonly Queue<LogModel> MsgQueue;
            private static readonly object FileLock;
            private static readonly string FilePath;

            static LogHelper()
            {
                FileLock = new object();
                if (Directory.Exists(AppSettings.COMMONSETTINGS.LogPath))
                {
                    FilePath = AppSettings.COMMONSETTINGS.LogPath;
                }
                else
                {
                    FilePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log\\";
                }
                WriteThread = new Thread(WriteMsg);
                MsgQueue = new Queue<LogModel>();
                WriteThread.Start();
            }

            public static void LogInfo(string msg)
            {
                Monitor.Enter(MsgQueue);
                MsgQueue.Enqueue(new LogModel(LogType.Info,
                    string.Format("{0} {1} {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss"), LogType.Info, msg)));
                Monitor.Exit(MsgQueue);
            }

            public static void LogInfo(string msg,string filename)
            {
                Monitor.Enter(MsgQueue);
                MsgQueue.Enqueue(new LogModel(LogType.Info,
                    string.Format("{0} {1} {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss"), LogType.Info, msg),filename));
                Monitor.Exit(MsgQueue);
            }

            public static void LogError(string msg)
            {
                Monitor.Enter(MsgQueue);
                MsgQueue.Enqueue(new LogModel(LogType.Error,
                    string.Format("{0} {1} {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss"), LogType.Error, msg)));
                Monitor.Exit(MsgQueue);
            }

            public static void LogError(string msg,string filename)
            {
                Monitor.Enter(MsgQueue);
                MsgQueue.Enqueue(new LogModel(LogType.Error,
                    string.Format("{0} {1} {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss"), LogType.Error, msg),filename));
                Monitor.Exit(MsgQueue);
            }

            public static void LogWarn(string msg)
            {
                Monitor.Enter(MsgQueue);
                MsgQueue.Enqueue(new LogModel(LogType.Warn,
                    string.Format("{0} {1} {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss"), LogType.Warn, msg)));
                Monitor.Exit(MsgQueue);
            }

            public static void LogWarn(string msg,string filename)
            {
                Monitor.Enter(MsgQueue);
                MsgQueue.Enqueue(new LogModel(LogType.Warn,
                    string.Format("{0} {1} {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss"), LogType.Warn, msg),filename));
                Monitor.Exit(MsgQueue);
            }

            private static void WriteMsg()
            {
                int sleeptime = 5000;
                while (true)
                {
                    if (MsgQueue.Count > 0)
                    {
                        Monitor.Enter(MsgQueue);
                        var msg = MsgQueue.Dequeue();
                        Monitor.Exit(MsgQueue);

                        Monitor.Enter(FileLock);
                        if (!Directory.Exists(Path.Combine(FilePath)))
                        {
                            Directory.CreateDirectory(Path.Combine(FilePath));
                        }
                        string _name = DateTime.Now.ToString("yyyy-MM-dd") + "_" + msg.LogType + ".log";
                        if (!string.IsNullOrEmpty(msg.FileName))
                        {
                            _name = msg.FileName;
                        }
                        var fileName = Path.Combine(FilePath,
                            _name);
                        var logStreamWriter = new StreamWriter(fileName, true);

                        logStreamWriter.WriteLine(msg.Message);
                        logStreamWriter.Close();
                        Monitor.Exit(FileLock);

                        //if (GetFileSize(fileName) > 1024*5)
                        //{
                        //    CopyToBak(fileName);
                        //}
                        sleeptime = 0;
                    }
                    else
                    {
                        sleeptime = 5000;
                    }
                    Thread.Sleep(sleeptime);
                }
            }

            private static long GetFileSize(string fileName)
            {
                long strRe = 0;
                if (File.Exists(fileName))
                {
                    Monitor.Enter(FileLock);
                    var myFs = new FileStream(fileName, FileMode.Open);
                    strRe = myFs.Length/1024;
                    myFs.Close();
                    myFs.Dispose();
                    Monitor.Exit(FileLock);
                }
                return strRe;
            }

            private static void CopyToBak(string sFileName)
            {
                var fileCount = 0;
                var sBakName = "";
                Monitor.Enter(FileLock);
                do
                {
                    fileCount++;
                    sBakName = sFileName + "." + fileCount + ".BAK";
                } while (File.Exists(sBakName));

                File.Copy(sFileName, sBakName);
                File.Delete(sFileName);
                Monitor.Exit(FileLock);
            }


        }

        public class LogModel
        {
            public LogModel(LogType logtype, string message)
            {
                this.LogType = logtype;
                this.Message = message;
            }

            public LogModel(LogType logtype, string message,string filename)
            {
                this.LogType = logtype;
                this.Message = message;
                this.FileName = filename;
            }

            public LogType LogType { get; set; }

            public string Message { get; set; }
            public string FileName { get; set; } 
        }
    }
}