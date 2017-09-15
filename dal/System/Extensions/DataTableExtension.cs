using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace System
{
    public static class  DAL_DataTableExtension
    {

        public static string getJoinedValue(this  DataTable t
            , string sField
            , string sFilter = ""
            , string sSeprator = ",")
        {

            List<string> lstString = new List<string>();
            DataRow[] rows = t.Select(sFilter);

            foreach (DataRow r in rows)
            {
                lstString.Add(r[sField].ToString());
            }

            return string.Join(sSeprator, lstString.ToArray());
        }


        public static string getDateFormat(this DataRow r, string sField, string sFormat = "dd/MMM/yyyy")
        {
            if (r.IsNull(sField)) return "";
            return ((DateTime)r[sField]).ToString(sFormat);
            

        }

    }
}
