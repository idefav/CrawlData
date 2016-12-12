using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace WebChatSites.Business
{
    public class WeixinApiDispatch
    {
        public string Execute(string poststr)
        {
            var request = ConvertObj<RequestText>(poststr);
            if (request != null)
            {
                return new WeixinAction().HandleText(request);
            }
            return "error";
        }

        public static T ConvertObj<T>(string xmlstr)
        {
            XElement xdoc = XElement.Parse(xmlstr);
            var type = typeof(T);
            var t = Activator.CreateInstance<T>();
            foreach (XElement element in xdoc.Elements())
            {
                var pr = type.GetProperty(element.Name.ToString());
                if (element.HasElements)
                {//这里主要是兼容微信新添加的菜单类型。nnd，竟然有子属性，所以这里就做了个子属性的处理
                    foreach (var ele in element.Elements())
                    {
                        pr = type.GetProperty(ele.Name.ToString());
                        pr.SetValue(t, Convert.ChangeType(ele.Value, pr.PropertyType), null);
                    }
                    continue;
                }
                if (pr.PropertyType.Name == "MsgType")//获取消息模型
                {
                    pr.SetValue(t, (ResponseMsgType)Enum.Parse(typeof(ResponseMsgType), element.Value.ToUpper()), null);
                    continue;
                }
                //if (pr.PropertyType.Name == "Event")//获取事件类型。
                //{
                //    pr.SetValue(t, (Event)Enum.Parse(typeof(Event), element.Value.ToUpper()), null);
                //    continue;
                //}
                pr.SetValue(t, Convert.ChangeType(element.Value, pr.PropertyType), null);
            }
            return t;
        }
    }

    internal static class ParseHelpers
    {

        

        private static JavaScriptSerializer json;
        private static JavaScriptSerializer JSON { get { return json ?? (json = new JavaScriptSerializer()); } }

        public static Stream ToStream(this string @this)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(@this);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }


        public static T ParseXML<T>(this string @this) where T : class
        {
            var reader = XmlReader.Create(@this.Trim().ToStream(), new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Document });
            return new XmlSerializer(typeof(T)).Deserialize(reader) as T;
        }

        public static T ParseJSON<T>(this string @this) where T : class
        {
            return JSON.Deserialize<T>(@this.Trim());
        }
    }
}