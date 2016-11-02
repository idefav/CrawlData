using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Idefav.DbFactory;
using Idefav.IDAL;
using Idefav.Utility;
using Newtonsoft.Json;
using WebChatSites.Business;

namespace WebChatSites.Controllers
{
    public class WeChatController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CheapProduct()
        {
            WeChat weChat = new WeChat();
            var datas = weChat.GetCheapProductDatas(DateTime.Parse("1990-01-01"));
            return View(datas);
        }

        public JsonResult QueryProduct(string productlink)
        {
            // PC链接解析
            var itemid = WeChat.GetProductId(productlink);
            WeChat weChat = new WeChat();
            var model = weChat.GetData(itemid);
            var minModel = weChat.GetMinPrice(itemid);
            HybridDictionary hybrid = new HybridDictionary { { "ChartData", model }, { "MinPrice", minModel } };
            return Json(JsonConvert.SerializeObject(hybrid), JsonRequestBehavior.AllowGet);

        }

    }
}