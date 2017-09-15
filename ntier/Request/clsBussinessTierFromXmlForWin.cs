using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTier.Request
{
    internal class clsBussinessTierFromXmlForWin : clsBussinessTierFromXmlBase
    {
        System.Collections.Specialized.NameValueCollection clnCookie = new System.Collections.Specialized.NameValueCollection();

        public override void setCookie(string sKey, string sValue)
        {
            clnCookie.Set(sKey, sValue);
        }

        public  override string getCookie(string sKey)
        {
            return clnCookie[sKey];
        }


        public clsBussinessTierFromXmlForWin(clsAppServerBase oAppServerInfo)
            : base(oAppServerInfo)
        {

        }
    }
}
