using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebend.DataSplider
{
  public  class SpliderType
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
            public string sItemID;
            public string sTitle;
            public string sPrice;
            public string sPriceAVG;
            public string sSellCount;

        }
    }
}
