using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace CrawlServices
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskManager mgr = new TaskManager();
            mgr.Init();
            mgr.Run(new CancellationToken());
            Console.ReadLine();



            //string server = "抓取服务";
            //string description = "抓取服务";

            //if (args.Length >= 2)
            //{
            //    server = args[1];
            //    InstallService iS = new InstallService();
            //    if (args[0] == "add")
            //    {
            //        if (args.Length == 3)
            //            description = args[2];
            //        var result = iS.Install(System.Reflection.Assembly.GetExecutingAssembly().Location, server, server, description);
            //    }
            //    if (args[0] == "del")
            //        iS.UnInstall(server);

            //}
            //else
            //{
            //    ServiceBase[] ServicesToRun;
            //    ServicesToRun = new ServiceBase[]
            //{
            //        new CrawlService(),
            //};
            //    ServiceBase.Run(ServicesToRun);
            //}
        }
    }
}
