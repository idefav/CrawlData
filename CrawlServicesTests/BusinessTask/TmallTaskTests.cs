using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrawlServices.BusinessTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Crawl.Common;

namespace CrawlServices.BusinessTask.Tests
{
    [TestClass()]
    public class TmallTaskTests
    {
        CancellationTokenSource cancellationTokenSource=new CancellationTokenSource();
        [TestMethod()]
        public void RunTest()
        {
            AppSettings.ReloadSettings();
            TaskManager mgr=new TaskManager();
            mgr.Init();
            mgr.Run(cancellationTokenSource.Token);
            //Assert.Fail();
        }
    }
}