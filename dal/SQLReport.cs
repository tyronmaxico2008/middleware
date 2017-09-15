using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DAL
{
    public class SQLReport
    {

        public clsMsg Result_msg;
        public DataSet ds;
        public byte[] output;

        public SQLReport()
        {
            
        }

        public void addTable(string sTableName, DataTable t)
        {
            t.TableName = sTableName;
            if(ds == null ) ds  = new DataSet();
            ds.Tables.Add(t);
        }

        public SQLReport(DataSet _ds, byte[] _output)
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

        public  byte[] export(string fileType)
        {
            var rpt = new Microsoft.Reporting.WinForms.LocalReport();

            //rpt.ReportPath = "Rpt01.rdlc";
            rpt.EnableExternalImages = true;


            for (int i = 0; i < this.ds.Tables.Count; i++)
            {
                var o = new Microsoft.Reporting.WinForms.ReportDataSource(this.ds.Tables[i].TableName, this.ds.Tables[i]);
                rpt.DataSources.Add(o);
            }

            //string sReportName = Request.QueryString["ReportName"];
            System.IO.MemoryStream ms = new System.IO.MemoryStream(this.output);
            rpt.LoadReportDefinition(ms);


            Byte[] results = rpt.Render(fileType);

            return results;
        }


    }
}
