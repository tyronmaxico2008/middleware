using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using NTier.Request;
namespace NTier.CRUD
{

    public class clsReadSimple : iGetData
    {
        public string viewName { get; set; }
        internal adapter.clsDataAdapterBase _adapter = null;


        public virtual clsMsg getData(clsCmd cmd)
        {
            string sQuery = "select * from " + viewName;
            var t = _adapter.getData(sQuery);
            return g.msg("", t);
        }

        public void setTier(iBussinessTier oTier)
        {
            throw new NotImplementedException();
        }

        public clsMsg validate(clsCmd cmd)
        {
            return g.msg("");
        }

    }
}
