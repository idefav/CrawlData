using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebend.DataSplider
{
    public class SpliderType
    {
        public struct TypeStore
        {
            public string sUrl;
            public string sName;
            public string sProvince;
            public string sCity;
            public string sMainProduct;
        }

        public struct TypeProduct
        {
            /// <summary>
            /// 项目编号
            /// </summary>
            public string sItemID;

            /// <summary>
            /// 标题
            /// </summary>
            public string sTitle;

            /// <summary>
            /// 价格
            /// </summary>
            public string sPrice;

            /// <summary>
            /// 单位价格
            /// </summary>
            public string sPriceAVG;

            /// <summary>
            /// 销量
            /// </summary>
            public string sSellCount;

            /// <summary>
            /// 货币单位
            /// </summary>
            public string sPriceUnit;

            /// <summary>
            /// 计量单位
            /// </summary>
            public string sCountUnit;

            public string sCountAVG;

            /// <summary>
            /// 统计日期
            /// </summary>
            public DateTime PDate { get; set; }

        }
    }
}
