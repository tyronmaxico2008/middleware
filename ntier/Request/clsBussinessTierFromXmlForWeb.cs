using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NTier.Request
{
    internal class clsBussinessTierFromXmlForWeb : clsBussinessTierFromXmlBase
    {
        string _AppName = "";

        public override void setCookie(string sKey, string sValue)
        {
            HttpCookie myUserCookie = new HttpCookie(sKey);

            myUserCookie.Value = sValue;

            if(_AppName.isEmpty()==false)
                myUserCookie.Path = "~/apps/" + _AppName;
            
            HttpContext.Current.Response.Cookies.Add(myUserCookie);
        }

        public override string getCookie(string sKey)
        {
            if (HttpContext.Current.Request.Cookies.AllKeys.Contains(sKey))
                return HttpContext.Current.Request.Cookies[sKey].Value;
            else
                return "";
        }


        public clsBussinessTierFromXmlForWeb(clsAppServerBase oAppServerInfo)
            : base(oAppServerInfo)
        {
            
        }
    }
}
