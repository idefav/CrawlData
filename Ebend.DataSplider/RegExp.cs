using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace Ebend.DataSplider
{
   public class ClassRegExp
    {
        public static ArrayList GetExpStringArray(string sString, string sExpStr)
        {
            MatchCollection m;
            Regex r;
            r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            m = r.Matches(sString);
            ArrayList ALM = new ArrayList();
            for (int i = 0; i < m.Count; i++)
            {
                if (m[i].Groups.Count > 1)
                {
                    ALM.Add( m[i].Groups[1].Value);
                }
                else
                {
                    ALM.Add(m[i].Value);
                }
            }
            return ALM;
        }
        public static string GetExpStringArray(string sString, string sExpStr,string sSplitStr)
        {
            MatchCollection m;
            Regex r;
            r = new Regex(sExpStr, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            m = r.Matches(sString);
            string sResult = "";
            for (int i = 0; i < m.Count; i++)
            {
                sResult += m[i].Value;
                if(i<m.Count-1)
                {
                    sResult += sSplitStr;
               
                }
            }
            return sResult;
        }

        public static string GetExpString(string sString, string sExpStr)
        {
            Match m;
            Regex r;
            r = new Regex(sExpStr,  RegexOptions.Singleline | RegexOptions.IgnoreCase);
            m = r.Match(sString);
            if (m.Success)
            {
                if (m.Groups.Count > 1)
                {
                    return m.Groups[1].Value;
                }
                else
                {
                    return m.Value;
                }
            }
            else
            {
                return "";
            }
        }
        public static string GetExpStringRTL(string sString, string sExpStr)
        {
            Match m;
            Regex r;
            r = new Regex(sExpStr,  RegexOptions.Singleline | RegexOptions.IgnoreCase|RegexOptions.RightToLeft);
            m = r.Match(sString);
            if (m.Success)
            {
                if (m.Groups.Count > 1)
                {
                    return m.Groups[1].Value;
                }
                else
                {
                    return m.Value;
                }
            }
            else
            {
                return "";
            }
        }
    }

}
