using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace System
{
   

    
    public class clsFiles : List<FileData>
    {

        public FileData this[string sField]
        {

            get
            {
                return this.Find(p => p.FieldName.Equals(sField, StringComparison.CurrentCultureIgnoreCase));
            }

        }

        public bool ContainFields(params string[] sFields)
        {
            foreach (string sField in sFields)
            {
                var f = this.Find(p => p.FieldName.ToLower() == sField.ToLower());
                if (f == null) return false;
            }

            return true;
        }

        public FileData Add(string fileName
            , byte[] data)
        {

            var oFile = new FileData();


            oFile.ContentType = "application/unknown";

            oFile.FileName = fileName;
            oFile.FileExtension = g.getFileExtension(fileName);
            oFile.Data = data;

            this.Add(oFile);



            return oFile;

        }
        public FileData Add(string sPath)
        {

            FileData oFile = null;

            if (System.IO.File.Exists(sPath))
            {


                oFile = new FileData();



                oFile.ContentType = "application/unknown";

                oFile.FileName = System.IO.Path.GetFileName(sPath);
                oFile.FileExtension = System.IO.Path.GetExtension(sPath);
                oFile.Data = System.IO.File.ReadAllBytes(sPath);


                this.Add(oFile);
            }


            return oFile;

        }


    }

    public class clsCmd : List<Param>
    {
        public string SQL { get; set; }
        public clsFiles Files = new clsFiles();

        public clsCmd()
        {
            this.CommandType = System.Data.CommandType.Text;
        }

        public System.Data.CommandType CommandType { get; set; }

        public void Add(DataRow r, params string[] fields)
        {
            foreach (var sField in fields) this.Add(sField, r[sField]);
        }

        public void AddValues(DataRow r)
        {
            foreach (DataColumn col in r.Table.Columns)
            {
                this.Add(col.ColumnName, r[col.ColumnName]);
            }
        }
        public Param Add(string sField, object val, string sTableName = "")
        {
            var f = new Param() { Name = sField, Value = val, TableName = sTableName };
            this.Add(f);

            return f;
        }

        public void setTableNameForAllFields(string sTableName)
        {

            foreach (var f in this)
                if (!f.Name.Contains("."))
                    f.TableName = sTableName;
        }


        public Param setValue(string sField, object value)
        {
            var f = this[sField];
            if (f != null)
            {
                f.Value = value;

                return f;
            }
            else
            {
                return Add(sField, value);
            }

        }




        /*
        public clsMsg checkMaxLength(int iMaxLength,params string[] sFields )
        {

            foreach (var sField in sFields)
            {
                var fld = getFieldInfo(sField);
                if (this.getStringValue(fld.FieldName).Length > iMaxLength)
                   return g.msg("The max length of field [" ++ fld.FieldTitle "] should not exced to  "

            }

        }

        */


        public clsMsg isExists(params string[] sFields)
        {
            var msg = new clsMsg();

            foreach (var exp in sFields)
            {

                string sField = exp;
                string sFieldTitle = exp;

                if (exp.Contains(":"))
                {
                    var arr = exp.Split(':');
                    if (arr.Length == 2)
                    {
                        sField = arr[0].Trim();
                        sFieldTitle = arr[1].Trim();
                    }
                }




                if (!this.ContainFields(sField))
                {
                    msg.Message = "Error : value of Field [" + sFieldTitle + "], can't be blank or empty !";
                    return msg;
                }
            }

            return msg;
        }

        public bool ContainFields(params string[] sFields)
        {
            foreach (string sField in sFields)
            {
                var f = this.Find(p => p.Name.ToLower() == sField.ToLower());
                if (f == null) return false;
            }

            return true;
        }

        public struct clsFieldInfo
        {
            public string FieldName { get; set; }
            public string FieldTitle { get; set; }
        }

        public static clsFieldInfo getFieldInfo(string sField)
        {
            var f = new clsFieldInfo();
            if (sField.Contains(":"))
            {
                string[] sVals = sField.Split(':');

                f.FieldName = sVals[0].Trim();
                f.FieldTitle = sVals.Length > 0 ? sVals[1].Trim() : f.FieldName;
            }

            else
            {
                f.FieldName = sField;
                f.FieldTitle = sField;
            }


            return f;
        }

        public clsMsg checkForNumeric(params string[] sFields)
        {
            var msg = new clsMsg();
            foreach (var sField in sFields)
            {
                var f = getFieldInfo(sField);

                if (!string.IsNullOrWhiteSpace(getStringValue(f.FieldName)) &&
                    !g.isNumeric(getStringValue(f.FieldName)))
                {
                    msg.Message = "Please specify proper Numeric Value for [" + f.FieldTitle + "] !";
                    return msg;
                }
            }

            return msg;
        }


        public bool getBoolValue(string sField)
        {
            return (this.ContainFields(sField) && this[sField].Value.ToString() == "true") ? true : false;
        }

        public string getStringValue(string sField)
        {
            return (this.ContainFields(sField) && this[sField].Value != null) ? this[sField].Value.ToString() : "";
        }

        public List<int> getIDs(string sField)
        {


            var lst1 = new List<int>();

            string sUser_Ids = this.getStringValue(sField);
            string[] sIDs = sUser_Ids.Split(',');

            foreach (var sID in sIDs)
            {
                int iId = g.parseInt(sID);
                if (iId != 0) lst1.Add(iId);
            }

            return lst1;

        }


        public string getDateString(string sField, string sFormat = "dd/MMM/yyyy")
        {
            string sVal = Convert.ToDateTime(getStringValue(sField)).ToString(sFormat);
            return sVal;
        }



        public DateTime getDate(string sField, string sFormat)
        {
            return DateTime.ParseExact(getStringValue(sField), sFormat, System.Globalization.CultureInfo.InvariantCulture);
        }





        public int getIntValue(string sField)
        {
            return g.parseInt(getStringValue(sField));
        }

        public clsMsg isEmpty(params string[] sFields)
        {



            var msg = isExists(sFields);
            if (msg.Validated == false) return msg;

            foreach (var exp in sFields)
            {

                string sField = exp;
                string sFieldTitle = exp;

                if (exp.Contains(":"))
                {
                    var arr = exp.Split(':');
                    if (arr.Length == 2)
                    {
                        sField = arr[0].Trim();
                        sFieldTitle = arr[1].Trim();
                    }
                }



                if (this[sField].Value is string)
                {
                    if (string.IsNullOrWhiteSpace(this[sField].Value.ToString()))
                    {
                        msg.Message = "Error : value of Field [" + sFieldTitle + "], can't be blank or empty !";
                        return msg;
                    }
                }
            }

            return msg;

        }

        public Param this[string sField]
        {

            get
            {
                return this.Find(p => p.Name.Equals(sField, StringComparison.CurrentCultureIgnoreCase));
            }
        }
    }

}
