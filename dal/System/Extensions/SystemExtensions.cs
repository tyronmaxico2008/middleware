using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class SystemExtensions
    {
        public static bool isEmpty(this String str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool Contains(this String str, params string[] sCompares)
        {

            foreach (var s in sCompares)
            {
                if (str.ToLower().Contains(s.ToLower()))
                    return true;
            }

            return false;
        }


        public static bool match(this String strSource, string strDestination)
        {
            if (strSource.ToLower() == strDestination.ToLower())
                return true;
            else
                return false;
        }
     
     
    }
}
