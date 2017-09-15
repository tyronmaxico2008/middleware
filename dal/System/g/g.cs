using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Data.OleDb;

namespace System
{
    public static partial class g
    {
        public static string getNextVoucherNo(string str1)
        {

            if (string.IsNullOrWhiteSpace(str1))
                return "001";

            //start
            string strNum = "";
            char[] cc = str1.ToCharArray();

            int iNumberCount = 0;

            for (int i = cc.Length; i > 0; i--)
            {
                if (char.IsNumber(cc[i - 1]))
                {
                    strNum = cc[i - 1] + strNum;
                    iNumberCount++;
                }
                else
                    break;
            }

            if (string.IsNullOrWhiteSpace(strNum))
                strNum = "0";

            int iNum = Convert.ToInt32(strNum) + 1;
            return str1.Substring(0, str1.Length - iNumberCount) + iNum.ToString(new String('0', iNumberCount));


        }

        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }








        public static DataTable readCSV(string sPath)
        {
            string CSVFilePathName = sPath;
            string[] Lines = File.ReadAllLines(CSVFilePathName);
            string[] Fields;
            Fields = Lines[0].Split(new char[] { ',' });
            int Cols = Fields.GetLength(0);
            DataTable dt = new DataTable();
            //1st row must be column names; force lower case to ensure matching later on.
            for (int i = 0; i < Cols; i++)
                dt.Columns.Add(Fields[i].ToLower(), typeof(string));
            DataRow Row;
            for (int i = 1; i < Lines.GetLength(0); i++)
            {
                Fields = Lines[i].Split(new char[] { ',' });
                Row = dt.NewRow();
                for (int f = 0; f < Cols; f++)
                    Row[f] = Fields[f];
                dt.Rows.Add(Row);
            }

            return dt;
        }


        public static DataTable createTableStringArray(params string[] sVals)
        {
            var t = new DataTable();
            t.Columns.Add("name", typeof(string));
            t.Columns.Add("value", typeof(string));


            foreach (var s in sVals)
            {
                var r = t.NewRow();
                r["name"] = s;
                r["value"] = s;
                t.Rows.Add(r);
            }
            return t;
        }
        public static string DateFormat(object dt, string sFormat)
        {
            if (dt == DBNull.Value) return "";
            return Convert.ToDateTime(dt).ToString(sFormat);
        }

        public static bool isNumeric(string sVal)
        {
            decimal iNum = 0;
            return decimal.TryParse(sVal, out iNum) ? true : false;
        }

        public static bool isNumeric(object sVal)
        {
            decimal iNum = 0;
            return decimal.TryParse(sVal.ToString(), out iNum) ? true : false;
        }


        public static object isNull(object val, object ReplaceVal)
        {
            if (val == DBNull.Value)
                return ReplaceVal;
            else
                return val;
        }

        public static DateTime parseDate(string sDate, string sFormat)
        {
            return DateTime.ParseExact(sDate, sFormat, System.Globalization.CultureInfo.InvariantCulture);
        }

        public static decimal parseDecimal(object sVal, int iRound = -1)
        {
            if (sVal == null) return 0;

            decimal iNum = 0;
            decimal.TryParse(sVal.ToString().Replace(",", ""), out iNum);

            if (iRound > -1)
                return Math.Round(iNum, iRound);
            return iNum;
        }
        public static double parseDouble(object sVal, int iRound = -1)
        {
            if (sVal == null) return 0;

            double iNum = 0;
            double.TryParse(sVal.ToString().Replace(",", ""), out iNum);

            if (iRound > -1)
                return Math.Round(iNum, iRound);
            return iNum;
        }


        public static int parseInt(object sVal)
        {
            if (sVal == null) return 0;

            int iNum = 0;
            int.TryParse(sVal.ToString().Replace(",", ""), out iNum);
            return iNum;
        }

        public static DataSet importExcel(string sPath)
        {
            DataSet ds = new DataSet();

            string fileExtension = System.IO.Path.GetExtension(sPath);

            if (fileExtension == ".xls" || fileExtension == ".xlsx")
            {

                string excelConnectionString = string.Empty;
                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                sPath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                //connection String for xls file format.
                if (fileExtension == ".xls")
                {
                    excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                    sPath + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                }
                //connection String for xlsx file format.
                else if (fileExtension == ".xlsx")
                {
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                    sPath + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                }
                //Create Connection to Excel work book and add oledb namespace
                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                excelConnection.Open();
                DataTable dt = new DataTable();

                dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt == null)
                {
                    return null;
                }

                String[] excelSheets = new String[dt.Rows.Count];
                int t = 0;
                //excel data saves in temp file here.
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[t] = row["TABLE_NAME"].ToString();
                    t++;
                }
                OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                string query = string.Format("Select * from [{0}]", excelSheets[0]);
                using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                {
                    dataAdapter.Fill(ds);
                }


            }
            return ds;
        }
        public static clsMsg msg(string sMsg = "", object data = null, string sInfo = "")
        {
            var _msg = new clsMsg(sMsg);
            _msg.Obj = data;
            _msg.Info = sInfo;
            return _msg;
        }

        public static clsMsg msg_exception(Exception ex)
        {

            if (ex.InnerException != null)
                return g.msg(ex.InnerException.Message);
            else
                return g.msg(ex.Message);


        }
        public static bool isDate(string sDate, string sFormat)
        {

            //return DateTime.ParseExact(sDate, sFormat, System.Globalization.CultureInfo.InvariantCulture);
            DateTime dt;


            return DateTime.TryParseExact(sDate, sFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out  dt);
        }

        public static bool isEmail(string sEmailID)
        {

            string MatchEmailPattern =
               @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
               + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
	                [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
               + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
	                [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
               + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";

            return System.Text.RegularExpressions.Regex.IsMatch(sEmailID, MatchEmailPattern);
        }

        public static string urlFormat(string sUrl)
        {

            return sUrl.ToString().Replace(" ", "-").Replace("&", "-");
        }

        public static object getJsonPaging(object eObj, string sSorting, int iStart, int ilength)
        {
            int iCount = 0;
            object eDataForJson;

            if (eObj is DataTable)
            {
                DataTable t = eObj as DataTable;

                if (!string.IsNullOrWhiteSpace(sSorting))
                {
                    var v = new DataView((t as DataTable));
                    v.Sort = sSorting;
                    t = v.ToTable();

                }

                iCount = t.Rows.Count;
                eDataForJson = GetTableRows(t, iStart, ilength);

            }
            else
            {
                var e = eObj as IEnumerable<object>;
                iCount = e.Count();
                eDataForJson = e.Skip(iStart).Take(ilength);
            }
                
            return new {  recordsTotal = iCount, recordsFiltered = iCount, data = eDataForJson, error = false, error_msg = "" };
        }

        //public static List<Dictionary<string, object>> GetTableRows(DataTable dtData)
        //{

        //    List<Dictionary<string, object>>
        //    lstRows = new List<Dictionary<string, object>>();
        //    Dictionary<string, object> dictRow = null;

        //    for (int i = 0; i < dtData.Rows.Count; i++)
        //    {
        //        if (i >= dtData.Rows.Count) break;
        //        dictRow = new Dictionary<string, object>();
        //        foreach (DataColumn col in dtData.Columns)
        //        {
        //            dictRow.Add(col.ColumnName, dtData.Rows[i][col]);
        //        }
        //        lstRows.Add(dictRow);
        //    }
        //    return lstRows;

        //}

        public static List<Dictionary<string, object>> GetTableRows(DataTable dtData, int iStart, int iLength)
        {
            List<Dictionary<string, object>>
            lstRows = new List<Dictionary<string, object>>();
            Dictionary<string, object> dictRow = null;

            for (int i = iStart; i < (iStart + iLength); i++)
            {
                if (i >= dtData.Rows.Count) break;
                dictRow = new Dictionary<string, object>();
                foreach (DataColumn col in dtData.Columns)
                {
                    dictRow.Add(col.ColumnName, dtData.Rows[i][col]);
                }
                lstRows.Add(dictRow);
            }
            return lstRows;
        }
    }


}
