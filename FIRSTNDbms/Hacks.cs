using System;
using System.Globalization;

namespace CodePlusTimeSavers_CE
{
    public class Hacks
    {
        public decimal someDec;
        public static bool IsNumber(string p1)
        {
            decimal result = 0;
            bool bolConverted = true;
            if (p1 == "000000") return false;
            try
            {
                result = Decimal.Parse(p1, NumberStyles.Any);
                bolConverted = false;
            }
            catch (FormatException)
            {
                
                bolConverted = false;
            }
            
            
            if (bolConverted) return true;
            if (p1 == "0.00") return true;
            if (p1 == "0.00") return true;
            if (p1 == "000") return true;
            return bolConverted;
        }

        public static bool IsSql(string p1)
        {
            string strDDL = p1;
            strDDL = strDDL.ToLower().TrimStart(' ');
            bool boolRtrn = strDDL.StartsWith("select") || strDDL.StartsWith("insert into") ||
                            strDDL.StartsWith("delete") || strDDL.StartsWith("update");
            return boolRtrn;
        }

        public static string FixHalfsAndTimes(string p1)
        {
            string strReturn = p1;
            strReturn = strReturn.Replace(".5", ":30");
            strReturn = strReturn.Replace(" 0pm", " 12noon");
            strReturn = strReturn.Replace("0pm to", "12noon to");
            strReturn = strReturn.Replace(" 0:", " 12:");
            return strReturn;
        }
    }
}