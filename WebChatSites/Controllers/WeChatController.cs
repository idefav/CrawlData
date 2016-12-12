using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Idefav.DbFactory;
using Idefav.IDAL;
using Idefav.Utility;
using Newtonsoft.Json;
using WebChatSites.Business;
using WebChatSites.Models.WeChat;

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

        public ContentResult checkSignature(string signature,string timestamp,string nonce,string echostr)
        {
            string[] tmpArr=new string[] {CommSettings.Token,timestamp,nonce};
             Array.Sort(tmpArr);
            string tmpstr = string.Join("", tmpArr);
            string sha1 = WeChat.EncryptToSHA1(tmpstr);
            if (sha1.Equals(signature,StringComparison.CurrentCultureIgnoreCase))
            {
                return Content(echostr);
            }
            return Content("");
        }

        public ContentResult ResponseCommonMsg(string msg)
        {
            Crawl.Common.Common.Log.LogInfo(msg);
            return base.Content("Success");
        }

        public JsonResult QueryProduct(string productlink)
        {
            // PC链接解析
            var data = WeChat.GetProductId(productlink);
            ShopConfig shopconfig = data["shopconfig"] != null ? data["shopconfig"] as ShopConfig : null;
            WeChat weChat = new WeChat();
            string itemid = data["itemid"].ToString();
            string shopname = data["shopname"].ToString();
            string dbtable = data["table"].ToString();
            bool match;
            var model = weChat.GetData(itemid, shopname, out match);
            var minModel = model.MinPrice;
            HybridDictionary hybrid = new HybridDictionary { { "ChartData", string.IsNullOrEmpty(model.Prices) ? new List<KeyValuePair<string, decimal?>>() : JsonConvert.DeserializeObject<Dictionary<string, decimal?>>(model.Prices).ToList() }, { "MinPrice", minModel }
                , { "Shop", shopname }, {"Title",model.Title}, {"ItemId",model.ItemId}, {"DetailLink",shopconfig!=null?string.Format(shopconfig.DetailLink,itemid):""} };
            return Json(JsonConvert.SerializeObject(hybrid), JsonRequestBehavior.AllowGet);

        }

        public ActionResult ProductDetail(string shop, string id)
        {
            ShopConfig shopconfig = WeChat.GetShopConfigByName(shop);
            WeChat weChat = new WeChat();
            bool match;
            var model = weChat.GetData(id, shop, out match);
            var m_cheap = weChat.GetCheapProductData(model.ItemId, model.Shop);
            List<string> labels = new List<string>();
            List<decimal?> datas = new List<decimal?>();
            
            if (!string.IsNullOrEmpty(model.Prices))
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, decimal?>>(model.Prices);
                labels = data.Keys.ToList();
                datas = data.Values.ToList();
            }
            labels.Add("Now");
            datas.Add(m_cheap.Price);
            HybridDictionary hybrid = new HybridDictionary { { "ChartLabels",JsonConvert.SerializeObject(labels)},{ "ChartDatas",JsonConvert.SerializeObject(datas)}, { "MinPrice", model.MinPrice }
                , { "Shop", shop }, {"Title",model.Title}, {"ItemId",model.ItemId}, {"DetailLink",shopconfig!=null?string.Format(shopconfig.DetailLink,id):""} };
            return View(hybrid);
        }

        public ActionResult GotoShop(string url,string title)
        {
            ViewBag.productname = title;
            ViewBag.url = url;
            return View();
        }

        public PartialViewResult GetCheapData(DateTime datetime)
        {
            WeChat weChat = new WeChat();
            var datas = weChat.GetCheapProductDatas2(datetime,10);
            return PartialView(datas);
        }

        
    }
}