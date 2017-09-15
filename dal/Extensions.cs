using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace System
{
    public static class Extensions
    {

        public static DataTable getTable(this DAL.clsCmd cmd, string sField)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(cmd.getStringValue(sField));
        }

        public static DataTable getTableFromJSON(this DAL.clsCmd cmd, string sField)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(cmd.getStringValue(sField));
        }

        public static DataTable getTableFromXml(this DAL.clsCmd cmd, string sField)
        {
            var ds = new DataSet();

            ds.ReadXml(new System.IO.StringReader(cmd.getStringValue(sField)));
            return ds.Tables[0];
        }

        public static List<T> getList<T>(this DAL.clsCmd cmd, string sField)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(cmd.getStringValue(sField));
        }

    }
}
