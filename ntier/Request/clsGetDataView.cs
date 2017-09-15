using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DAL;
namespace NTier.Request
{
    public class clsGetDataView
    {
        public string viewName { get; set; }
        public string OrderBy { get; set; }
        NTier.adapter.clsDataAdapterBase _adapter;
        
        public clsGetDataView(NTier.adapter.clsDataAdapterBase adapter)
        {
            _adapter = adapter;
        }


        public DataTable getData(clsCmd cmd)
        {

            if (cmd.ContainFields("_filter"))
            {
                string sFilterJson = cmd.getStringValue("_filter");

                var tFilter = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(sFilterJson);
                foreach(DataRow rFilter in tFilter.Rows)
                {
                    if (rFilter["val"].ToString().isEmpty() == false)
                    {
                        var prm = cmd.setValue(rFilter["name"].ToString(), rFilter["val"].ToString());
                        prm.Operator = rFilter["operator"].ToString();
                    }
                }
                
                cmd.Remove(cmd["_filter"]);
            }

            string q = "select * from " + viewName + " where 1=1 ";
            cmd.SQL = NTier.sqlbuilder.sqlUtility.joinWhereCondition(q, cmd);
            

            

            if (!OrderBy.isEmpty()) cmd.SQL += " order by " + OrderBy;

            var t = _adapter.getData(cmd);
            return t;
        }
    }
}
