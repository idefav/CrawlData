using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Crawl.Common;

namespace Ebend.DataSplider
{
    public class TaobaoSplider
    {
        private string sWID = "";
        private string sMID = "";
        public string sLoc = "南京";
        public string sTCookies = "";
        public static Random rd = new Random();
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
        /// 搜索淘宝店铺
        /// </summary>
        /// <param name="sKeyWord">关键字</param>
        /// <param name="iPage">页码</param>
        /// <param name="iTotalPage">返回总页码</param>
        /// <returns>店铺结果数组</returns>
        public DataSplider.SpliderType.TypeStore[] SearchStore(string sKeyWord, int iPage, out int iTotalPage)
        {
            HtmlHttpHelper HHH = new HtmlHttpHelper();
            HHH.Referer = "https://www.taobao.com/";
            string sHtmlCode = HHH.Get("https://shopsearch.taobao.com/search?data-key=s&data-value=" + Convert.ToString(20 * iPage) + "&ajax=true&_ksTS=1462867509248_488&callback=jsonp489&app=shopsearch&q=" + sKeyWord + "&js=1&initiative_id=staobaoz_" + DateTime.Now.ToString("yyyyMMdd") + "&ie=utf8&loc=" + sLoc + "&s=" + Convert.ToString(20 * iPage), "");
            iTotalPage = Convert.ToInt32(ClassRegExp.GetExpString(sHtmlCode, "\"totalPage\":(\\d+)"));//获取页码
            ArrayList ALTypeStore = DecodeStore(sHtmlCode);
            return (DataSplider.SpliderType.TypeStore[])ALTypeStore.ToArray(typeof(DataSplider.SpliderType.TypeStore));
        }
        private ArrayList DecodeStore(string sHtmlCode)
        {
            ArrayList ALTypeStore = new ArrayList();
            DataSplider.SpliderType.TypeStore TS = new SpliderType.TypeStore();
            string jsonText = ClassRegExp.GetExpString(sHtmlCode, "jsonp\\d+\\((.+?)\\);");
            JObject jo = (JObject)JsonConvert.DeserializeObject(jsonText);
            JArray ja = (JArray)jo["mods"]["shoplist"]["data"]["shopItems"];
            string sProvcity = "";
            foreach (JToken token in ja)
            {
                jo = (JObject)token;
                if (jo["shopUrl"] != null)
                {
                    TS.sUrl = "https:" + jo["shopUrl"].ToString();
                }

                if (jo["mainAuction"] != null)
                {
                    TS.sMainProduct = jo["mainAuction"].ToString();
                }
                if (jo["provcity"] != null)
                {
                    sProvcity = jo["provcity"].ToString();
                }
                TS.sProvince = ClassRegExp.GetExpString(sProvcity, "(\\w+)\\s+");
                if (TS.sProvince == "")
                {
                    TS.sProvince = sProvcity;
                }
                TS.sCity = ClassRegExp.GetExpString(sProvcity, "\\w+\\s+(\\w+)");
                if (TS.sCity == "")
                {
                    TS.sCity = sProvcity;
                }
                if (jo["title"] != null)
                {
                    TS.sName = jo["title"].ToString();
                }
                ALTypeStore.Add(TS);
            }
            return ALTypeStore;
        }
        public void InitShop(string sShopUrl)
        {
            string sUrl = sShopUrl + "/search.htm?spm=" + DateTime.Now.ToString() + "&search=y";
            HtmlHttpHelper HHH = new HtmlHttpHelper();
            HHH.sCookies = sTCookies;
            HHH.Referer = sUrl;
            string sHtmlCode = HHH.Get(sUrl, "");
            sMID = ClassRegExp.GetExpString(sHtmlCode, "\\?mid=(.+?)&");
            sWID = ClassRegExp.GetExpString(sHtmlCode, "&wid=(.+?)&");
        }
        public SpliderType.TypeProduct[] SearchProduct(string sShopUrl, int iPage, out int iTotalPage)
        {
            string sUrl = sShopUrl + "/i/asynSearch.htm?_ksTS=1462893454477_130&callback=jsonp131&mid=" + sMID + "&wid=" + sWID + "&path=/search.htm&search=y&spm=" + DateTime.Now.ToFileTime().ToString() + "&viewType=grid&pageNo=" + iPage;
            //https://shop106956264.taobao.com/i/asynSearch.htm?_ksTS=1462893454477_130&callback=jsonp131&mid=w-4253175477-0&wid=4253175477&path=/search.htm&search=y&spm=a1z10.3-c.w4002-4253175477.82.HMjruY&viewType=grid&pageNo=2

            HtmlHttpHelper HHH = new HtmlHttpHelper();
            HHH.sCookies = sTCookies;
            HHH.Referer = sShopUrl;
            string sHtmlCode = HHH.Get(sUrl, "");
            iTotalPage = Convert.ToInt32(ClassRegExp.GetExpStringRTL(sHtmlCode, "\">(\\d+)</a>"));
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

        public SpliderType.TypeProduct[] SearchProducts2(string sKeyWord, int iPage, out int iTotalPage)
        {
            ArrayList ALTypeProduct = new ArrayList();
            HtmlHttpHelper HHH = new HtmlHttpHelper();
            HHH.sCookies = sTCookies;
            HHH.UserAgent = GetUserAgent();
            HHH.Referer = "https://www.taobao.com/";
            string sUrl = "https://s.taobao.com/list?bcoffset=12&spm=" + DateTime.Now.ToFileTime().ToString() + "&s=" + Convert.ToString(60 * iPage) + "&q=" + sKeyWord + "&style=grid&seller_type=taobao";
            //sUrl =
            //    "https://list.tmall.com/search_product.htm?spm=" + DateTime.Now.ToFileTime().ToString() + "&s=" + Convert.ToString(60 * iPage) + "&q=" + sKeyWord + "&sort=s&style=g&from=.list.pc_1_searchbutton&smAreaId=310100&type=pc#J_Filter";
            string sHtmlCode = HHH.Get(sUrl, "");
            sHtmlCode = sHtmlCode.Replace("\n", "");
            Regex regex=new Regex("g_page_config\\s*=\\s*({.*?});",RegexOptions.Compiled|RegexOptions.IgnoreCase);
            var match = regex.Match(sHtmlCode);
            JToken pager=null;
            if (match.Success)
            {
                try
                {
                    var taobaoResolve = JsonConvert.DeserializeObject<HybridDictionary>(match.Groups[1].Value) ;
                    if (taobaoResolve != null)
                    {
                        JObject mods = taobaoResolve["mods"] as JObject;
                        if (mods != null)
                        {
                            pager = mods["pager"]["data"];
                        
                            var itemlist = mods["itemlist"]["data"]["auctions"].Value<JArray>();
                        
                            foreach (JToken jToken in itemlist)
                            {
                                try
                                {
                                    SpliderType.TypeProduct TS;
                                    TS = new SpliderType.TypeProduct();
                                    TS.sTitle = jToken["raw_title"].Value<string>();
                                    TS.sPrice = jToken["view_price"].Value<string>();
                                    TS.CommentCount = jToken["comment_count"].Value<string>();
                                    TS.PDate = DateTime.Now.Date;
                                    TS.sItemID = jToken["nid"].Value<string>();
                                    TS.sSellCount = string.IsNullOrEmpty(jToken["view_sales"].Value<string>())?"":jToken["view_sales"].Value<string>().Replace("人付款", "");
                                    TS.PicUrl = jToken["pic_url"].Value<string>();
                                    ALTypeProduct.Add(TS);
                                }
                                catch (Exception e)
                                {
                                    Common.Log.LogError(e.ToString());
                                }
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    Common.Log.LogError(e.ToString());
                }
            }
            iTotalPage = pager?["totalPage"].Value<int>() ?? 0;
            //iTotalPage = Convert.ToInt32(ClassRegExp.GetExpString(sHtmlCode, "<b class=\"ui-page-s-len\">\\d+/(\\d+)</b>"));//获取页码
            //ArrayList ALTypeProduct = DecodeProduct(sHtmlCode);
            return (SpliderType.TypeProduct[])ALTypeProduct.ToArray(typeof(SpliderType.TypeProduct));
        }

        private ArrayList DecodeProduct(string sHtmlCode)
        {
            ArrayList ALTypeProduct = new ArrayList();
            sHtmlCode = sHtmlCode.Replace("\\", "");
            string sExpStr = "<dl class=\"item\\s*\"(.+?)</dl>";
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
                sExpStr = "item.htm\\?id=(\\d+)";
                r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                m = r.Match(sResult);
                if (m.Success)
                {
                    TS.sItemID = m.Groups[1].Value;
                }
                sExpStr = "\"c-price\">(.+?)</span>";
                r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                m = r.Match(sResult);
                if (m.Success)
                {
                    TS.sPrice = m.Groups[1].Value;
                }

                TS.sPriceAVG = "无";

                //<div class="sale-area">
                sExpStr = "\"sale-num\">(\\d+)</span>件</div>";
                r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
                m = r.Match(sResult);
                if (m.Success)
                {
                    TS.sSellCount = m.Groups[1].Value;
                }
                sExpStr = "<img alt=\\s*\"(.+?)\"";
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

        //{
        //                  "uid": "109738980",
        //                  "title": "天台山土特产店",
        //                  "nick": "皮慧芳pipee",
        //                  "provcity": "浙江 台州",
        //                  "totalsold": 978,
        //                  "procnt": 253,
        //                  "encodeNick": "%E7%9A%AE%E6%85%A7%E8%8A%B3pipee",
        //                  "goodratePercent": "99.93%",
        //                  "dynamic": "",
        //                  "shopUrl": "//shop63812317.taobao.com",
        //                  "similarUrl": "/search?user_id=109738980",
        //                  "picUrl": "//g-search1.alicdn.com/img/bao/uploaded/i4//99/e9/T1sLLhFlNfXXb1upjX.jpg",
        //                  "rawTitle": "天台山土特产店",
        //                  "hasMoreAuctions": true,
        //                  "mainAuction": "农家 有机 新鲜 绿色 货 无公害 断 g 自 现 《 野生 暂 克 天台山 种 磨 新 上市 熟 生 纯 小 》 高山 原生态 本地 段 做 炒 蔬菜 传统 山区 2 ; & 新货 手工 纯天然 天台 天然 干货 味道 云雾茶 种子 面粉 自制 可 特产 的 粗粮 （ 斤 发 无污染 无 酵素 绿茶 茶叶 ） 艾米粒 菜 红薯 青 非转基因 儿时 黄豆 肉 红豆 5 gX 心 包 优质 冰糖 不 番薯 营养 年糕 包邮 绿 粉 杂粮 红豆粉 卖 油 果冻 健康 黑芝麻 味 买 作 新茶 干 碎 500 采 香 1 口感 赤豆 大蒜 特色 自然 红枣 小吃 莲子 仁 山茶 粉丝 香椿芽 袋泡茶 浓郁 纯手工 食用油 勾 糯米 芯 原味 特级 L 豆浆 麻糍 月子 土豆 富含 豆 米 瘦 老 山野菜 瘦身 青皮 打 件 糕点 白皮 葱 叶 宝宝 腌制 100% 泡 花生 200 大豆 荞麦 gt 好 糯 红米 软 煮 牛肉 苦 黄米 薏仁粉 爽 125 地瓜 修 地 苦菜 小狗 缘 核桃仁 饼 芝麻 莲蓬 礼盒装 摘 婆婆 时令 粗 薏米 山芋 苦瓜 自留 地方 青梅 青菜 素食 醇 炒菜 零食 辅食 柴火 白 棉花 养生 玉米 大小 清香 美容 白糯米 清明 美食 养 大 安全",
        //                  "userRateUrl": "//rate.taobao.com/user-rate-UvFNSMGv4OFgW.htm",
        //                  "isManjian": false,
        //                  "isSongli": false,
        //                  "isTmall": false,
        //                  "hasSimilar": true,
        //                  "auctionsInshop": [
        //                      {
        //                          "nid": "8895765374",
        //                          "picUrl": "//g-search1.alicdn.com/img/bao/uploaded/i4/i4/TB1DRdTJVXXXXagXpXXXXXXXXXX_!!0-item_pic.jpg",
        //                          "title": "深山农家原生态新鲜土生姜 原始点小黄姜 有机月子土姜老姜非嫩姜",
        //                          "price": "12.00",
        //                          "url": "//item.taobao.com/item.htm?id=8895765374&abbucket=4",
        //                          "uid": "109738980"
        //                      },
        //                      {
        //                          "nid": "10494431636",
        //                          "picUrl": "//g-search1.alicdn.com/img/bao/uploaded/i4/i2/TB1WllZJVXXXXXlXFXXXXXXXXXX_!!0-item_pic.jpg",
        //                          "title": "农家自种绿色无公害新鲜小土豆 烧有机椒盐土豆 土豆泥《暂断货》",
        //                          "price": "5.20",
        //                          "url": "//item.taobao.com/item.htm?id=10494431636&abbucket=4",
        //                          "uid": "109738980"
        //                      },
        //                      {
        //                          "nid": "9856926046",
        //                          "picUrl": "//g-search1.alicdn.com/img/bao/uploaded/i4/i3/109738980/TB2r02ompXXXXb9XXXXXXXXXXXX_!!109738980.jpg",
        //                          "title": "高山新鲜有机 野生香椿芽 香椿头 原生态 现摘速发 江浙沪2件包邮",
        //                          "price": "32.00",
        //                          "url": "//item.taobao.com/item.htm?id=9856926046&abbucket=4",
        //                          "uid": "109738980"
        //                      },
        //                      {
        //                          "nid": "8884797728",
        //                          "picUrl": "//g-search1.alicdn.com/img/bao/uploaded/i4/i3/TB1C8s7JFXXXXc2XpXXXXXXXXXX_!!0-item_pic.jpg",
        //                          "title": "农家纯天然有机胡萝卜 红萝卜 宝宝营养辅食 原生态富硒《断货》",
        //                          "price": "8.90",
        //                          "url": "//item.taobao.com/item.htm?id=8884797728&abbucket=4",
        //                          "uid": "109738980"
        //                      }
        //                  ],
        //                  "shopIcon": {
        //                      "trace": "shop",
        //                      "iconClass": "rank seller-rank-12",
        //                      "url": "//rate.taobao.com/user-rate-UvFNSMGv4OFgW.htm"
        //                  },
        //                  "nid": "63812317",
        //                  "isMianyou": false,
        //                  "startFee": "0",
        //                  "discountCash": "0",
        //                  "giftName": "",
        //                  "isHideSale": false,
        //                  "icons": [
        //                      {
        //                          "title": "卖家承诺消费者保障服务",
        //                          "domClass": "icon-service-xiaobao",
        //                          "url": "//www.taobao.com/go/act/315/xfzbz_rsms.php",
        //                          "text": "消费者保障"
        //                      },
        //                      {
        //                          "title": "金牌卖家",
        //                          "domClass": "icon-service-jinpaimaijia",
        //                          "url": "//www.taobao.com/go/act/jpmj.php",
        //                          "text": "金牌卖家"
        //                      }
        //                  ],
        //                  "dsrInfo": {
        //                      "mgDomClass": "descr-icon-morethan",
        //                      "dsrStr": "{\"srn\":\"47202\",\"sgr\":\"99.93%\",\"ind\":\"食品/保健\",\"mas\":\"4.90\",\"mg\":\"42.74%\",\"sas\":\"4.93\",\"sg\":\"51.42%\",\"cas\":\"4.91\",\"cg\":\"51.81%\",\"encryptedUserId\":\"UvFNSMGv4OFgW\"}",
        //                      "cgDomClass": "descr-icon-morethan",
        //                      "sgDomClass": "descr-icon-morethan"
        //                  }
        //              }
    }
}
