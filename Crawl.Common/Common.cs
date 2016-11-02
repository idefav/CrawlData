using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawl.Common
{
    public class Common
    {
        public static ILog Log { get; set; }

        static Common()
        {
            Log = new TxtLog();
        }
    }
}
