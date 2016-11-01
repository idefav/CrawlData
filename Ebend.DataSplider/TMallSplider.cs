using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Crawl.Common;

namespace Ebend.DataSplider
{
    public class TMallSplider
    {
        public string sTCookies = "";
        public string sCookies
        {
            get
            {
                return sTCookies;
            }
            set
            {
                sTCookies = value;
            }
        }

        public static Random rd = new Random();

        /// <summary>
        /// 搜索天猫店铺
        /// </summary>
        /// <param name="sKeyWord">关键字</param>
        /// <param name="iPage">页码</param>
        /// <param name="iTotalPage">返回总页码</param>
        /// <returns>店铺结果数组</returns>
        public SpliderType.TypeStore[] SearchStore(string sKeyWord, int iPage, out int iTotalPage)
        {
            HtmlHttpHelper HHH = new HtmlHttpHelper();
            HHH.sCookies = sTCookies;
            HHH.Referer = "https://www.tmall.com/";
            string sUrl = "https://list.tmall.com/search_product.htm?spm=" + DateTime.Now.ToFileTime().ToString() + "&s=" + Convert.ToString(20 * iPage) + "&q=" + sKeyWord + "&sort=s&style=w&from=mallfp..pc_1_searchbutton&tmhkmain=0&type=pc#J_Filter";
            string sHtmlCode = HHH.Get(sUrl, "");
            iTotalPage = Convert.ToInt32(ClassRegExp.GetExpString(sHtmlCode, "<b class=\"ui-page-s-len\">\\d+/(\\d+)</b>"));//获取页码
            ArrayList ALTypeStore = DecodeStore(sHtmlCode);
            return (DataSplider.SpliderType.TypeStore[])ALTypeStore.ToArray(typeof(DataSplider.SpliderType.TypeStore));
        }
        public SpliderType.TypeProduct[] SearchProduct(string sUid, int iPage, out int iTotalPage)
        {
            string sUrl = "https://list.tmall.com/search_shopitem.htm?spm=" + DateTime.Now.ToFileTime().ToString() + "&s=" + Convert.ToString(60 * iPage) + "&style=sg&sort=s&user_id=" + sUid + "&from=_1_&stype=search#grid-column";
            HtmlHttpHelper HHH = new HtmlHttpHelper();
            HHH.sCookies = sTCookies;
            HHH.Referer = sUrl;
            string sHtmlCode = HHH.Get(sUrl, "");
            iTotalPage = Convert.ToInt32(ClassRegExp.GetExpString(sHtmlCode, "共(\\d+)页"));//获取页码
            ArrayList ALTypeProduct = DecodeProduct(sHtmlCode);
            return (DataSplider.SpliderType.TypeProduct[])ALTypeProduct.ToArray(typeof(DataSplider.SpliderType.TypeProduct));
        }

        private string GetUserAgent()
        {
            string[] ualist = new[]
            {
                "Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_6_8; en-us) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50",
                "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-us) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50",
                "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0",
                "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)",
                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.6; rv:2.0.1) Gecko/20100101 Firefox/4.0.1",
                "Mozilla/5.0 (Windows NT 6.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1",
                "Opera/9.80 (Macintosh; Intel Mac OS X 10.6.8; U; en) Presto/2.8.131 Version/11.11",
                "Opera/9.80 (Windows NT 6.1; U; en) Presto/2.8.131 Version/11.11",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_7_0) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.56 Safari/535.11",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Maxthon 2.0)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; TencentTraveler 4.0)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; The World)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Trident/4.0; SE 2.X MetaSr 1.0; SE 2.X MetaSr 1.0; .NET CLR 2.0.50727; SE 2.X MetaSr 1.0)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; 360SE)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Avant Browser)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)"
            };
            int d = rd.Next(ualist.Length);
            return ualist[d];
        }

        public SpliderType.TypeProduct[] SearchProduct2(string sKeyWord, int iPage, out int iTotalPage)
        {

            HtmlHttpHelper HHH = new HtmlHttpHelper();
            HHH.sCookies = sTCookies;
            HHH.UserAgent = GetUserAgent();
            HHH.Referer = "https://www.tmall.com/";
            string sUrl = "https://list.tmall.com/search_product.htm?spm=" + DateTime.Now.ToFileTime().ToString() + "&s=" + Convert.ToString(60 * iPage) + "&q=" + sKeyWord + "&sort=s&style=w&from=mallfp..pc_1_searchbutton&tmhkmain=0&type=pc#J_Filter";
            sUrl =
                "https://list.tmall.com/search_product.htm?spm=" + DateTime.Now.ToFileTime().ToString() + "&s=" + Convert.ToString(60 * iPage) + "&q=" + sKeyWord + "&sort=s&style=g&from=.list.pc_1_searchbutton&smAreaId=310100&type=pc#J_Filter";
            string sHtmlCode = HHH.Get(sUrl, "");
            sHtmlCode = sHtmlCode.Replace("\n", "");
            iTotalPage = Convert.ToInt32(ClassRegExp.GetExpString(sHtmlCode, "<b class=\"ui-page-s-len\">\\d+/(\\d+)</b>"));//获取页码
            ArrayList ALTypeProduct = DecodeProduct(sHtmlCode);
            return (SpliderType.TypeProduct[])ALTypeProduct.ToArray(typeof(SpliderType.TypeProduct));
            //}
            //catch (Exception e)
            //{
            //    iTotalPage = 0;
            //    return (SpliderType.TypeProduct[])new ArrayList().ToArray(typeof(SpliderType.TypeProduct));
            //}
        }

        private ArrayList DecodeStore(string sHtmlCode)
        {
            ArrayList ALTypeStore = new ArrayList();
            string sExpStr = "class=\"shopHeader-info\">(.+?)</div>";
            DataSplider.SpliderType.TypeStore TS;
            MatchCollection mc;
            Match m;
            Regex r;
            r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            mc = r.Matches(sHtmlCode);
            string sResult = "";
            for (int i = 0; i < mc.Count; i++)
            {
                TS = new DataSplider.SpliderType.TypeStore();
                sResult = mc[i].Groups[1].Value;
                sExpStr = "<a href=\"(search_shopitem.+?)\"";
                r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                m = r.Match(sResult);
                if (m.Success)
                {
                    TS.sUrl = m.Groups[1].Value;
                }
                sExpStr = "class=\"sHi-title\"\\s*data-p=\"\\d+\\-\\d+\">(.+?)</a>";
                r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                m = r.Match(sResult);
                if (m.Success)
                {
                    TS.sName = m.Groups[1].Value;
                }
                sExpStr = "主营品牌：\\s*<span title=\"(.*?)\">";
                r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                m = r.Match(sResult);
                if (m.Success)
                {
                    TS.sMainProduct = m.Groups[1].Value;
                }
                sExpStr = "<p>所在地：(\\w+)\\s+(\\w+)\\s*</p>";
                r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                m = r.Match(sResult);
                if (m.Success)
                {
                    TS.sProvince = m.Groups[1].Value;
                    TS.sCity = m.Groups[2].Value;
                }
                ALTypeStore.Add(TS);

            }
            return ALTypeStore;
        }
        private ArrayList DecodeProduct(string sHtmlCode)
        {
            ArrayList ALTypeProduct = new ArrayList();


            string sExpStr = "<div class=\"product [^\"]* \" data-id=\"\\d+\"data-atp=\".+?\">(.+?)</span>\\s*</p>\\s*</div>\\s*</div>";
            SpliderType.TypeProduct TS;
            MatchCollection mc;
            Match m;
            Regex r;
            r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(30));
            mc = r.Matches(sHtmlCode);
            string sResult = "";
            for (int i = 0; i < mc.Count; i++)
            {
                try
                {
                    TS = new SpliderType.TypeProduct();
                    sResult = mc[i].Groups[1].Value;
                    ////detail.tmall.com/item.htm?id=35464629066&amp;is_b=1&amp;cat_id=2&amp;q=&amp;rn=4d5659c1ab3f48f75871f0d0f2a998f8
                    sExpStr = "<a href=\"//detail.tmall.com/item.htm\\?id=(\\d+)";
                    r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    m = r.Match(sResult);
                    if (m.Success)
                    {
                        TS.sItemID = m.Groups[1].Value;
                    }
                    sExpStr = "<em title=\"[\\d\\.]+\"><b>&yen;</b>([\\d\\.]+)</em>";
                    r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    m = r.Match(sResult);
                    if (m.Success)
                    {
                        TS.sPrice = m.Groups[1].Value;
                    }
                    sExpStr = "<span class=\"productPrice-ave\">([\\d\\.]+)(.+?)/([\\d\\.]+)(.+?)</span>";
                    r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    m = r.Match(sResult);
                    if (m.Success)
                    {
                        TS.sPriceAVG = m.Groups[1].Value;
                        TS.sPriceUnit = m.Groups[2].Value;
                        TS.sCountAVG = m.Groups[3].Value;
                        TS.sCountUnit = m.Groups[4].Value;
                    }
                    sExpStr = "<span>月成交 <em>(\\d+?)笔</em></span>";
                    r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    m = r.Match(sResult);
                    if (m.Success)
                    {
                        TS.sSellCount = m.Groups[1].Value;
                    }
                    sExpStr = "target=\"_blank\" title=\"(.+?)\" data-p=\".*?\"\\s*>";
                    r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    m = r.Match(sResult);
                    if (m.Success)
                    {
                        TS.sTitle = m.Groups[1].Value;
                    }
                    TS.PDate = DateTime.Now.Date;
                    ALTypeProduct.Add(TS);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }

            return ALTypeProduct;
        }
    }
}
