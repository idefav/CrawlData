using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using WebChatSites.Business;

namespace WebChatSites.Controllers
{
    public class WeChatApiController : ApiController
    {
        [HttpPost]
        [HttpGet]
        public string CommonMsg()
        {
            string postString = string.Empty;
            if (Request.Method.Method.ToUpper() == "POST")
            {
                using (Stream stream = HttpContext.Current.Request.InputStream)
                {
                    Byte[] postBytes = new Byte[stream.Length];
                    stream.Read(postBytes, 0, (Int32)stream.Length);
                    postString = Encoding.UTF8.GetString(postBytes);
                }

                if (!string.IsNullOrEmpty(postString))
                {
                    postString = Execute(postString);
                }
            }
            else
            {
                //Auth();
            }
            //ResponseWrite(postString);
            return postString;

            
        }

        [HttpGet]
        public void Business(string signature, string timestamp, string nonce, string echostr)
        {
            string[] tmpArr = new string[] { CommSettings.Token, timestamp, nonce };
            Array.Sort(tmpArr);
            string tmpstr = string.Join("", tmpArr);
            string sha1 = WeChat.EncryptToSHA1(tmpstr);
            if (sha1.Equals(signature, StringComparison.CurrentCultureIgnoreCase))
            {
                ResponseWrite(echostr);
            }
            ResponseWrite("");
        }

        [HttpPost]
        public void Business()
        {
            string postString = string.Empty;
            if (Request.Method.Method.ToUpper() == "POST")
            {
                using (Stream stream = HttpContext.Current.Request.InputStream)
                {
                    Byte[] postBytes = new Byte[stream.Length];
                    stream.Read(postBytes, 0, (Int32)stream.Length);
                    postString = Encoding.UTF8.GetString(postBytes);
                }

                if (!string.IsNullOrEmpty(postString))
                {
                    postString = Execute(postString);
                }
            }
            else
            {
                //Auth();
            }
            ResponseWrite(postString);
            //return postString;
        }

        private void ResponseWrite(string msg)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write(msg);
            HttpContext.Current.Response.End();
        }

        [HttpGet]
        public void checkSignature(string signature, string timestamp, string nonce, string echostr)
        {
            //string signature = HttpContext.Current.Request["signature"];
            //string timestamp = HttpContext.Current.Request["timestamp"];
            //string nonce = HttpContext.Current.Request["nonce"];
            //string echostr = HttpContext.Current.Request["echostr"];

            string[] tmpArr = new string[] { CommSettings.Token, timestamp, nonce };
            Array.Sort(tmpArr);
            string tmpstr = string.Join("", tmpArr);
            string sha1 = WeChat.EncryptToSHA1(tmpstr);
            if (sha1.Equals(signature, StringComparison.CurrentCultureIgnoreCase))
            {
                ResponseWrite(echostr);
            }
            ResponseWrite("");
        }

        /// <summary>
        /// 处理各种请求信息并应答（通过POST的请求）
        /// </summary>
        /// <param name="postStr">POST方式提交的数据</param>
        private string Execute(string postStr)
        {
            WeixinApiDispatch dispatch = new WeixinApiDispatch();
            string responseContent = dispatch.Execute(postStr);

            return responseContent;
        }
    }
}
