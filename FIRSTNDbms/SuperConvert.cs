using System;
using System.Diagnostics;

namespace CodePlusTimeSavers_CE
{
    public class SuperConvert
    {
        public static DateTime StringToDateTime(string p1)
        {
            DateTime dtmReturn=new DateTime(2000,1,1);
            bool bolParseWorked = false;
            try
            {
                dtmReturn = Convert.ToDateTime(p1);
                bolParseWorked = true;
            }
            catch 
            {
                bolParseWorked = false;
            }
            if (!bolParseWorked)
            {
                try
                {
                    // HTML5 date  yyyy-mm-dd
                    dtmReturn = new DateTime(Convert.ToInt32(p1.Substring(0, 4)),
                        Convert.ToInt32(p1.Substring(6, 2)),
                        Convert.ToInt32(p1.Substring(9, 2)));
                }
                catch (Exception)
                {
                    bolParseWorked = false;
                }
            }
            if (!bolParseWorked)
            {
                Debug.WriteLine("String Extension Method StringToDateTime could not parse " + p1);
            }
           return dtmReturn;    

        } // end StringToDateTime
    } // end class
} // end namespace