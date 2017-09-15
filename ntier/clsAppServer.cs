using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTier
{


    public abstract class clsAppServerBase
    {

        public abstract string appServerRootPath { get; }
        public abstract string appName { get; }
        

        public string getAppConfigFilePath()
        {
            string sPath = appServerRootPath + "\\apps\\" + appName + "\\appConfig.xml";
            return sPath;
        }


        public string getAppConfigFolder()
        {
            string sPath = appServerRootPath + "\\apps\\" + appName;
            return sPath;
        }

        public string getAppResourcePath(string sPath)
        {
            return getAppConfigFolder() + "\\" + sPath;
        }


    }

    public class clsAppServerInfo : clsAppServerBase
    {

        private string _AppServerRootPath;
        private string _AppName;

        public clsAppServerInfo(string sAppServerRootPath
            , string sAppName)
        {

            _AppServerRootPath = sAppServerRootPath;
            _AppName = sAppName;
        }


        public override string appName
        {
            get { return _AppName; }
        }
        public override string appServerRootPath
        {
            get { return _AppServerRootPath; }
        }
    }

}
