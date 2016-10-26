using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ebend.DataSplider
{
    public class TMallSplider 
    {
        public string sTCookies="";
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

        /// <summary>
        /// 搜索天猫店铺
        /// </summary>
        /// <param name="sKeyWord">关键字</param>
        /// <param name="iPage">页码</param>
        /// <param name="iTotalPage">返回总页码</param>
        /// <returns>店铺结果数组</returns>
        public SpliderType.TypeStore [] SearchStore(string sKeyWord,int iPage,out int  iTotalPage )
        {
            HtmlHttpHelper HHH = new HtmlHttpHelper();
            HHH.sCookies = sTCookies;
            HHH.Referer = "https://www.tmall.com/";
            string sUrl = "https://list.tmall.com/search_product.htm?spm=" + DateTime.Now.ToFileTime().ToString() + "&s=" + Convert.ToString(20 * iPage) + "&q=" + sKeyWord + "&sort=s&style=w&from=mallfp..pc_1_searchbutton&tmhkmain=0&type=pc#J_Filter";
            string sHtmlCode = HHH.Get(sUrl, "");
            iTotalPage = Convert.ToInt32(ClassRegExp.GetExpString(sHtmlCode, "<b class=\"ui-page-s-len\">\\d+/(\\d+)</b>"));//获取页码
            ArrayList ALTypeStore=DecodeStore(sHtmlCode);
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

        public SpliderType.TypeProduct[] SearchProduct2(string sKeyWord, int iPage, out int iTotalPage)
        {
            HtmlHttpHelper HHH = new HtmlHttpHelper();
            HHH.sCookies = sTCookies;
            HHH.Referer = "https://www.tmall.com/";
            string sUrl = "https://list.tmall.com/search_product.htm?spm=" + DateTime.Now.ToFileTime().ToString() + "&s=" + Convert.ToString(60 * iPage) + "&q=" + sKeyWord + "&sort=s&style=w&from=mallfp..pc_1_searchbutton&tmhkmain=0&type=pc#J_Filter";
            sUrl =
                "https://list.tmall.com/search_product.htm?spm="+ DateTime.Now.ToFileTime().ToString() + "&s=" + Convert.ToString(60 * iPage) + "&q="+sKeyWord+"&sort=s&style=g&from=.list.pc_1_searchbutton&smAreaId=310100&type=pc#J_Filter";
            string sHtmlCode = HHH.Get(sUrl, "");
            iTotalPage = Convert.ToInt32(ClassRegExp.GetExpString(sHtmlCode, "<b class=\"ui-page-s-len\">\\d+/(\\d+)</b>"));//获取页码
            ArrayList ALTypeProduct = DecodeProduct(sHtmlCode);
            return (SpliderType.TypeProduct[]) ALTypeProduct.ToArray(typeof(SpliderType.TypeProduct));
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
            string sExpStr = "class=\"product\">(.+?)</div>\\s*</div>";
            DataSplider.SpliderType.TypeProduct TS;
            MatchCollection mc;
            Match m;
            Regex r;
            r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            mc = r.Matches(sHtmlCode);
            string sResult = "";
            for (int i = 0; i < mc.Count; i++)
            {
                TS = new DataSplider.SpliderType.TypeProduct();
                sResult = mc[i].Groups[1].Value;
                ////detail.tmall.com/item.htm?id=35464629066&amp;is_b=1&amp;cat_id=2&amp;q=&amp;rn=4d5659c1ab3f48f75871f0d0f2a998f8
                sExpStr = "<a href=\"//detail.tmall.com/item.htm\\?id=(\\d+)";
                r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                m = r.Match(sResult);
                if (m.Success)
                {
                    TS.sItemID = m.Groups[1].Value;
                }
                sExpStr = "<em><b>&yen;</b>(.+?)</em>";
                r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                m = r.Match(sResult);
                if (m.Success)
                {
                    TS.sPrice = m.Groups[1].Value;
                }
                sExpStr = "class=\"productPrice-ave\">(.*?)</span>";
                r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                m = r.Match(sResult);
                if (m.Success)
                {
                    TS.sPriceAVG = m.Groups[1].Value;
                }
                sExpStr = "<span>月成交<em>(\\d+)笔</em></span>";
                r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                m = r.Match(sResult);
                if (m.Success)
                {
                    TS.sSellCount = m.Groups[1].Value;
                }
                sExpStr = "title=\"(.+?)\">";
                r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                m = r.Match(sResult);
                if (m.Success)
                {
                    TS.sTitle = m.Groups[1].Value;
                }
                ALTypeProduct.Add(TS);

            }
            return ALTypeProduct;
        }
    }
}
