using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTier.Request
{
    internal class clsErrorLog
    {

        clsBussinessTierFromXmlBase _Tier;

        public clsErrorLog(clsBussinessTierFromXmlBase oTier)
        {
            _Tier = oTier;
        }

        

        public void log(string slogType, string sPath, clsCmd cmd)
        {

            var sPathErrorLogPath = _Tier.getPath("errorlog");

            if (!System.IO.Directory.Exists(sPathErrorLogPath))
                System.IO.Directory.CreateDirectory(sPathErrorLogPath);
            //

            string sDateFolder = sPathErrorLogPath + "\\" + System.DateTime.Today.ToString("dd-MM-yyyy");

            if (!System.IO.Directory.Exists(sDateFolder))
                System.IO.Directory.CreateDirectory(sDateFolder);

            //folder preparation and creation done.
            //
            checkAndCreateErrorTable();





            return;
        }

        private void checkAndCreateErrorTable()
        {
            var t = _Tier.getAdapter().getData("select * from  sysobjects where name ='tmp_error_log' ");
            if (t.Rows.Count == 0)
            {

                string q = @"create table tmp_error_log
                    (
	                    id int primary key identity(1,1)
	                    ,errMsg varchar(max)
	                    ,requestType varchar(100)
	                    ,requestPath varchar(100)
                    ) ";

                _Tier.getAdapter().exec(q);
            }


        }




    }
}
