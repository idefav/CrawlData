using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using Newtonsoft.Json;
using WebChatSites.Models.WeChat;

namespace WebChatSites.Business
{


    public static class DateTimeUtil
    {
        /// <summary> 
               /// 微信的CreateTime是当前与1970-01-01 00:00:00之间的秒数        /// </summary> 
               /// <param name=“dt”></param>        /// <returns></returns> 
        public static long DateTimeToInt(this DateTime dt)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            //intResult = (time- startTime).TotalMilliseconds; 
            long t = (dt.Ticks - startTime.Ticks) / 10000000;            //现在是10位，除10000调整为13位 
            return t;
        }

        /// <summary>
        /// datetime转换为unixtime
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

            return (int)(time - startTime).TotalSeconds;

        }

        /// <summary>

        /// unix时间转换为datetime

        /// </summary>

        /// <param name="timeStamp"></param>

        /// <returns></returns>

        public static DateTime UnixTimeToTime(string timeStamp)

        {

            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

            long lTime = long.Parse(timeStamp + "0000000");

            TimeSpan toNow = new TimeSpan(lTime);

            return dtStart.Add(toNow);

        }
    }
    /// <summary>
    /// 基础消息内容
    /// </summary>
    [XmlRoot(ElementName = "xml")]
    public class BaseMessage
    {
        /// <summary>
        /// 初始化一些内容，如创建时间为整形，
        /// </summary>
        public BaseMessage()
        {
            this.CreateTime = (int)DateTime.Now.DateTimeToInt();
        }

        /// <summary>
        /// 开发者微信号
        /// </summary>
        public string ToUserName { get; set; }

        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        public string FromUserName { get; set; }

        /// <summary>
        /// 消息创建时间 （整型）
        /// </summary>
        public int CreateTime { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string MsgType { get; set; }

        public virtual string ToXml()
        {
            this.CreateTime = (int)DateTime.Now.DateTimeToInt();//重新更新
            return MyXmlHelper.ObjectToXml(this);
        }

    }

    public class MyXmlHelper
    {
        public static string ObjectToXml(BaseMessage message)
        {
            //XmlSerializer xsSubmit = new XmlSerializer(typeof(BaseMessage));

            //XmlDocument doc = new XmlDocument();

            //System.IO.StringWriter sww = new System.IO.StringWriter();
            //XmlWriter writer = XmlWriter.Create(sww);
            //xsSubmit.Serialize(writer, message);
            //var xml = sww.ToString(); // Your xml
            //return xml;
            StringBuilder stringBuilder = new StringBuilder("<xml>");
            foreach (PropertyInfo propertyInfo in message.GetType().GetProperties())
            {
                string name = propertyInfo.Name;
                object value = propertyInfo.GetValue(message, null);
                if (value != null && value is string)
                {
                    stringBuilder.Append(string.Format("<{0}><![CDATA[{1}]]></{0}>", name, value));
                }
                else
                {
                    stringBuilder.Append(string.Format("<{0}>{1}</{0}>", name, value));
                }
            }
            stringBuilder.Append("</xml>");
            return stringBuilder.ToString();
        }


    }

    public enum ResponseMsgType
    {
        TEXT
    }
    /// <summary>
    /// 回复文本消息
    /// </summary>
    [System.Xml.Serialization.XmlRoot(ElementName = "xml")]
    public class ResponseText : BaseMessage
    {
        public ResponseText()
        {
            this.MsgType = ResponseMsgType.TEXT.ToString().ToLower();
        }

        public ResponseText(BaseMessage info) : this()
        {
            this.FromUserName = info.ToUserName;
            this.ToUserName = info.FromUserName;
        }

        /// <summary>
        /// 内容
        /// </summary>        
        public string Content { get; set; }
    }

    public interface IWeixinAction
    {
        /// <summary>
        /// 对文本请求信息进行处理
        /// </summary>
        /// <param name="info">文本信息实体</param>
        /// <returns></returns>
        string HandleText(RequestText info);

        /// <summary>
        /// 对图片请求信息进行处理
        /// </summary>
        /// <param name="info">图片信息实体</param>
        /// <returns></returns>
        //string HandleImage(RequestImage info);
    }

    public class WeixinAction : IWeixinAction
    {
        public string HandleText(RequestText info)
        {

            string resxml = "<xml><ToUserName><![CDATA[" + info.FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + info.ToUserName + "]]></FromUserName><CreateTime>" + DateTimeUtil.ConvertDateTimeInt(DateTime.Now) + "</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[" + "{0}" + "]]></Content><FuncFlag>1</FuncFlag></xml>";

            var data = WeChat.GetProductId(info.Content);
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
            string text = "";
            if (!string.IsNullOrEmpty(itemid))
            {
                text = string.Format("您所查询的是: 【{2}】{0} 当前价:{5} 最低价:{1} 平均价:{3}  最高价:{4}  {6}", model.Title,
                    model.MinPrice, model.Shop, model.AvgPrice, model.MaxPrice, model.Price,
                    string.Format("http://idefav.com/wechat/ProductDetail?shop={0}&id={1}", HttpUtility.UrlEncode(model.Shop), model.ItemId));
            }
            else
            {
                text = "未查到相关商品";
            }
            return string.Format(resxml, text);
        }
    }

    public class RequestText : BaseMessage
    {
        //public string ToUserName { get; set; }
        //public string FromUserName { get; set; }
        //public int CreateTime { get; set; }
        //public string MsgType { get; set; }
        public string URL { get; set; }
        public string Content { get; set; }
        public string MsgId { get; set; }
    }


}