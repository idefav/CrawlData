using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CrawlServices.Interface;
using CrawlServices.Model;
using Ebend.DataSplider;
using Idefav.DbFactory;
using Idefav.IDAL;
using Idefav.Utility;

namespace CrawlServices.BusinessTask
{
    public class TmallTask: TaskBase
    {
        public TmallTaskModel TmallTaskModel { get { return base.Model as TmallTaskModel; } }
        public CancellationToken CancellationToken { get; set; }
        public IDbObject Db { get; set; }

        private const string sCookies= "x=__ll%3D-1%26_ato%3D0; cna=97ykDz7G2GYCAXVZmbuJZpdH; otherx=e%3D1%26p%3D*%26s%3D0%26c%3D0%26f%3D0%26g%3D0%26t%3D0; uc3=nk2=AnWUh%2Bs%3D&id2=UNk1%2BrxWtNk%3D&vt3=F8dASmgq691MKE0n%2BJg%3D&lg2=UIHiLt3xD8xYTw%3D%3D; uss=UtJSnXzx31ekK4NBPk9D5ChUqQKCyM0Iw%2BMNDs7w1BQltILkn%2BmmiykAQw%3D%3D; lgc=adobo; tracknick=adobo; cookie2=3c8fda0838e9f0f989dc5c427f4239e6; t=18fbd460625667ad4fc53c42619f98bf; _tb_token_=e5e15b5e9e591; pnm_cku822=157UW5TcyMNYQwiAiwQRHhBfEF8QXtHcklnMWc%3D%7CUm5OcktwT3tFe0Z7Q3xBeiw%3D%7CU2xMHDJ7G2AHYg8hAS8RJQsrBVk4XjJVK1F%2FKX8%3D%7CVGhXd1llXGdYbFJsUWxUa1ZtWmdFeEZ9RX5AdEF8R3hAdE1yTHRaDA%3D%3D%7CVWldfS0QMAs0CioWKwslTjIXIVU4XCZLZ0kfSQ%3D%3D%7CVmhIGC0ZOQA0FCgXKxAwDDEMOQYmGiUQLQ0xDDMKKhYpHCEBNQA5bzk%3D%7CV25Tbk5zU2xMcEl1VWtTaUlwJg%3D%3D; res=scroll%3A1216*10082-client%3A1216*866-offset%3A1216*10082-screen%3A1280*1024; cq=ccp%3D1; l=AgsLWcRfWvyTxboIhs4ASAfcm6H1oh8i";

        public TmallTask(ITaskModel taskModel) : base(taskModel)
        {
            Db= DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
        }

        public override void Run(TaskScheduler taskScheduler, CancellationToken token)
        {

            this.CancellationToken = token;
            
            Task = new Task<object>(() =>
            {
                Business();
                NextStartTime = DateTime.Now.AddSeconds(TmallTaskModel.Interval);
                return null;
            }, token);
            Task.Start(taskScheduler);

            //System.Threading.Tasks.Task.WaitAll(Task);
        }

        private void Business()
        {
            Common.CommonLog.LogInfo(string.Format("{0}_{1}",TmallTaskModel.TaskName,"开始"));
            foreach (string keyWord in TmallTaskModel.KeyWords)
            {
                //TMallSplider tMallSplider = new TMallSplider();
                //tMallSplider.sCookies = sCookies;
                    

                //var productlist = tMallSplider.SearchProduct2(keyWord, iPage++, out totalPage);
                Common.CommonLog.LogInfo(string.Format("{0}-正在解析关键字:{1}",TmallTaskModel.TaskName,keyWord));
                ResolveKeyWord(keyWord);
                Common.CommonLog.LogInfo(string.Format("{0}-解析关键字:{1} 完成", TmallTaskModel.TaskName, keyWord));
            }
            Common.CommonLog.LogInfo(string.Format("{0}_{1}", TmallTaskModel.TaskName, "结束"));
        }

        private void ResolveKeyWord(string keyword)
        {
            int totalPage = 0;
            int iPage = 0;
            TMallSplider tMallSplider = new TMallSplider();
            tMallSplider.sCookies = sCookies;


            SaveDataToDb( tMallSplider.SearchProduct2(keyword, iPage++, out totalPage));
            while (iPage<totalPage)
            {
                SaveDataToDb(tMallSplider.SearchProduct2(keyword, iPage++, out totalPage));
                Thread.Sleep(10000);
            }
        }

        private void SaveDataToDb(SpliderType.TypeProduct[] typeProducts)
        {
            //var db = DBOMaker.CreateDbObj(DBType.SQLServer, AppSettings.COMMONSETTINGS.DbConn);
            foreach (SpliderType.TypeProduct typeProduct in typeProducts)
            {
                try
                {
                    string sql = @"INSERT INTO [DB_Tmall].[dbo].[td_data]
                                       ([Guid]
                                       ,[ItemId]
                                       ,[Title]
                                       ,[Price]
                                       ,[PriceAVG]
                                       ,[SellCount]
                                       ,[PriceUnit]
                                       ,[CountUnit]
                                       ,[CountAVG]
                                       ,[PDate])
                                 VALUES
                                       ({0} ,{1},{2} ,{3},{4},{5},{6},{7},{8},{9})";
                    sql = string.Format(sql
                        , Db.GetParameterName("Guid")
                        , Db.GetParameterName("ItemId")
                        , Db.GetParameterName("Title")
                        , Db.GetParameterName("Price")
                        , Db.GetParameterName("PriceAVG")
                        , Db.GetParameterName("SellCount")
                        , Db.GetParameterName("PriceUnit")
                        , Db.GetParameterName("CountUnit")
                        , Db.GetParameterName("CountAVG")
                        , Db.GetParameterName("PDate"));
                    
                    Db.ExecuteSql(sql, parameters: new
                    {
                        Guid= Guid.NewGuid().ToString(),
                        ItemId= typeProduct.sItemID,
                        Title= typeProduct.sTitle,
                        Price=string.IsNullOrEmpty(typeProduct.sPrice)?0: decimal.Parse(typeProduct.sPrice),
                        PriceAVG=string.IsNullOrEmpty(typeProduct.sPriceAVG)?0: decimal.Parse(typeProduct.sPriceAVG),
                        SellCount= string.IsNullOrEmpty(typeProduct.sSellCount)?0:int.Parse(typeProduct.sSellCount),
                        PriceUnit= typeProduct.sPriceUnit,
                        CountUnit= typeProduct.sCountUnit,
                        CountAVG= typeProduct.sCountAVG,
                        PDate= typeProduct.PDate
                    });
                }
                catch (Exception e)
                {
                    Common.CommonLog.LogError(e.ToString());
                }
            }
            
        }

    }

    public class TmallTaskModel : ITaskModel
    {
        public bool Enable { get; set; }
        public string TaskName { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string[] KeyWords { get; set; }

        /// <summary>
        /// 并发数
        /// </summary>
        private int _taskcount = 1;
        public int TaskCount { get { return _taskcount; } set { _taskcount = value; } }

        /// <summary>
        /// 时间间隔
        /// </summary>
        private int _interval = 24*60*60;
        public int Interval { get { return _interval; } set { _interval = value; } }
    }
}
