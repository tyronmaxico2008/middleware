using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace NTier.sqlReport
{
    public abstract class SQLReportBase : iSQLReport
    {

        public clsMsg Result_msg;
        public DataSet ds;
        public byte[] output;
        public string downloadName { get; set; }

        public abstract void render(string sType, Stream st);
        public abstract byte[] export(string fileType);
        
        

        public void addTable(string sTableName, DataTable t)
        {
            t.TableName = sTableName;
            if (ds == null) ds = new DataSet();
            ds.Tables.Add(t);
        }

        public SQLReportBase() 
        {
 
        }

        public SQLReportBase(DataSet _ds, byte[] _output)
        {
            ds = _ds;
            output = _output;
        }



        public byte[] exportToPdf()
        {
            return export("Pdf");
        }

        public byte[] exportToExcel()
        {
            return export("Excel");
        }



    }


}
