
using CsharpHttpHelper;
using CsharpHttpHelper.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ebend.DataSplider
{
    class HtmlHttpHelper
    {
        public string sCookies = "";
        public string sPostString = "";
        public string Referer = "";
        public string UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 Safari/537.36";
        public string sAcceptEncoding = "";
        public int iTimeOut =10000;//超时时间
        public bool bError = false;
        public  bool bProxy = false;
        public  string sProxyIP = "";
        public  string sProxyPort = "80";
        public  string sProxyUserName = "";
        public  string sProxyPassWord = "";
        HttpHelper http = new HttpHelper();
        public string GetHtmlCode(string sUrl, string sCode)//根据不同方式
        {
            if (iTimeOut == 0)
            {
                iTimeOut = 10000;
            }
            try
            {
                if (sPostString == "")
                {
                    return Get(sUrl, sCode);
                }
                else
                {
                    return Post(sUrl, sPostString, sCode);
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
      
        public string Get(string sUrl, string sCode)//根据不同方式
        {
            Encoding e =null;
            try
            {
                e = Encoding.GetEncoding(sCode);
            }
            catch
            {

            }
            HttpItem item = new HttpItem()
            {
                URL = sUrl,//URL     必需项
                Encoding = e,//编码格式（utf-8,gb2312,gbk）     可选项 默认类会自动识别
                //Encoding = Encoding.Default,
                Method = "get",//URL     可选项 默认为Get
                Timeout = iTimeOut,//连接超时时间     可选项默认为100000
                ReadWriteTimeout = iTimeOut,//写入Post数据超时时间     可选项默认为30000
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写
                Cookie = sCookies,//字符串Cookie     可选项
                UserAgent = UserAgent,//用户的浏览器类型，版本，操作系统     可选项有默认值
                Accept = "text/html, application/xhtml+xml, */*",//    可选项有默认值
                ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值
                Referer = Referer,//来源URL     可选项
                Allowautoredirect = true,//是否根据３０１跳转     可选项
                //CerPath = "d:\\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数
                Connectionlimit = 1024,//最大连接数     可选项 默认为1024
                //Postdata = "C:\\PERKYSU_20121129150608_ScrubLog.txt",//Post数据     可选项GET时不需要写
                //PostDataType = PostDataType.FilePath,//默认为传入String类型，也可以设置PostDataType.Byte传入Byte类型数据
                //ProxyIp = "192.168.1.105：8015",//代理服务器ID 端口可以直接加到后面以：分开就行了    可选项 不需要代理 时可以不设置这三个参数
                //ProxyPwd = "123456",//代理服务器密码     可选项
                //ProxyUserName = "administrator",//代理服务器账户名     可选项
                //ResultType = ResultType.Byte,//返回数据类型，是Byte还是String
                //PostdataByte = System.Text.Encoding.Default.GetBytes("测试一下"),//如果PostDataType为Byte时要设置本属性的值
                //CookieCollection = new System.Net.CookieCollection(),//可以直接传一个Cookie集合进来
            };
             if (bProxy)
            {
                item.ProxyIp = sProxyIP+":"+sProxyPort;
                item.ProxyUserName=sProxyUserName;
                item.ProxyPwd=sProxyPassWord;
            }
            //得到HTML代码
            HttpResult result = http.GetHtml(item);
            //取出返回的Cookie
             SetCookies(result.Cookie);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //表示访问成功，具体的大家就参考HttpStatusCode类
                bError = false;
            }
           
            else
            {
                bError = true;
            }
            //返回的Html内容
            return  result.Html;

        }
        public string Post(string sUrl, string sPostString,string sCode)//根据不同方式
        {
            Encoding e = null;
            try
            {
                e = Encoding.GetEncoding(sCode);
            }
            catch
            {

            }
            HttpItem item = new HttpItem()
            {
                URL = sUrl,//URL     必需项
                Encoding = e,//编码格式（utf-8,gb2312,gbk）     可选项 默认类会自动识别
                //Encoding = Encoding.Default,
                Method = "post",//URL     可选项 默认为Get
                Timeout = iTimeOut,//连接超时时间     可选项默认为100000
                ReadWriteTimeout = iTimeOut,//写入Post数据超时时间     可选项默认为30000
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写
                Cookie = sCookies,//字符串Cookie     可选项
                UserAgent = UserAgent,//用户的浏览器类型，版本，操作系统     可选项有默认值
                Accept = "text/html, application/xhtml+xml, */*",//    可选项有默认值
                ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值
                Referer = Referer,//来源URL     可选项
                Allowautoredirect = true,//是否根据３０１跳转     可选项
                //CerPath = "d:\\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数
                Connectionlimit = 1024,//最大连接数     可选项 默认为1024
                Postdata = sPostString,//Post数据     可选项GET时不需要写
                PostDataType = PostDataType.String,//默认为传入String类型，也可以设置PostDataType.Byte传入Byte类型数据
                //ProxyIp = "192.168.1.105：8015",//代理服务器ID 端口可以直接加到后面以：分开就行了    可选项 不需要代理 时可以不设置这三个参数
                //ProxyPwd = "123456",//代理服务器密码     可选项
                //ProxyUserName = "administrator",//代理服务器账户名     可选项
                //ResultType = ResultType.Byte,//返回数据类型，是Byte还是String
                //PostdataByte = System.Text.Encoding.Default.GetBytes("测试一下"),//如果PostDataType为Byte时要设置本属性的值
                //CookieCollection = new System.Net.CookieCollection(),//可以直接传一个Cookie集合进来
            };
             if (bProxy)
            {
                item.ProxyIp = sProxyIP+":"+sProxyPort;
                item.ProxyUserName=sProxyUserName;
                item.ProxyPwd=sProxyPassWord;
            }
            //得到HTML代码
            HttpResult result = http.GetHtml(item);
            //取出返回的Cookie
             SetCookies(result.Cookie);
            //返回的Html内容
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //表示访问成功，具体的大家就参考HttpStatusCode类
                bError = false;
            }
            else
            {
                bError = true;
            }
            return result.Html;

        }
        private void SetCookies(string sHtml)
        {
            if (sHtml == null||sHtml=="")
            {
                return;
            }
           //Set-Cookie: b_110128=0; domain=.qidian.com; expires=Fri, 15-Sep-2023 15:48:41 GMT; path=/
            string sName = "";
            string sValue = "";
            MatchCollection mc;
            Match m;
            Regex r;
            if (!sCookies.EndsWith(";") && sCookies != "")
            {
                sCookies += ";";
            }
            r = new Regex("(?<sName>.*?)=(?<sValue>.*?);", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            mc = r.Matches(sHtml);
            for (int i = 0; i < mc.Count; i++)
            {

                sName = mc[i].Groups["sName"].Value.Trim();
                sValue = mc[i].Groups["sValue"].Value.Trim();
                if (sName.ToLower().Trim() == "expires" || sName.ToLower().Trim() == "path")
                {
                    continue;
                }
                r = new Regex(sName + "\\s*=\\s*.*?;", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                m = r.Match(sCookies);
                if (m.Success)
                {
                    sCookies = sCookies.Replace(m.Value, sName + "=" + sValue + ";");
                }
                else
                {
                    sCookies += sName + "=" + sValue + ";";
                }
               

            }
        }
       

      
    }
}
