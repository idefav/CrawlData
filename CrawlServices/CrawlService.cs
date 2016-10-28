using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrawlServices
{
    partial class CrawlService : ServiceBase
    {
        CancellationTokenSource cancellationTokenSource=new CancellationTokenSource();

        public CrawlService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            TaskManager mgr=new TaskManager();
            mgr.Init();

            

            mgr.Run(cancellationTokenSource.Token);
        }

        protected override void OnStop()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
