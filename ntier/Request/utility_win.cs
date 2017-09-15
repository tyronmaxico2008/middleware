using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTier.Request
{
    partial class utility
    {
        public static iBussinessTier createBussinessTierFromXmlForWin(clsAppServerBase oAppServerInfo)
        {
            clsBussinessTierFromXmlBase obj = new clsBussinessTierFromXmlForWin(oAppServerInfo);
            return obj;
        }

    }
}
