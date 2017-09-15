using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTier.Request
{
    internal class requestPath
    {
        public string seprator = "\\";
        string _Path = "";
        string _fullPath = "";
        public System.Collections.Specialized.NameValueCollection cln = new System.Collections.Specialized.NameValueCollection();

        public string getQueryString(string skey)
        {

            if (cln.Count > 0)
            {
                return cln[skey];
            }
            else
                return "";
        }
        public string getPath()
        {
            return _Path;
        }
        public string getFullPath()
        {
            return _fullPath;
        }
        public requestPath(string sPath)
        {

            _fullPath = sPath;
            string[] sTmp = sPath.Split('?');

            if (sTmp.Length > 0)
                _Path = sTmp[0];


            if (sTmp.Length > 1)
            {
                string[] queryStrings = sTmp[1].Split('&');

                foreach (string queryString in queryStrings)
                {
                    string[] queryInfo = queryString.Split('=');

                    if (queryInfo.Length > 1)
                    {
                        cln.Add(queryInfo[0], queryInfo[1]);
                    }
                }
            }

        }

    }
}
