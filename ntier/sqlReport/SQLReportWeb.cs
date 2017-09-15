using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using Microsoft.Reporting.WinForms;

namespace NTier.sqlReport
{
    public class SQLReportWeb : NTier.sqlReport.SQLReportBase
    {


        public SQLReportWeb()
        {
            
        }
        

        public void addTable(string sTableName, DataTable t)
        {
            t.TableName = sTableName;
            if(ds == null ) ds  = new DataSet();
            ds.Tables.Add(t);
        }

        public SQLReportWeb(DataSet _ds, byte[] _output)
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


        public  override byte[] export(string fileType)
        {
            var rpt = new Microsoft.Reporting.WebForms.LocalReport();

            //rpt.ReportPath = "Rpt01.rdlc";
            rpt.EnableExternalImages = true;


            for (int i = 0; i < this.ds.Tables.Count; i++)
            {
                var o = new Microsoft.Reporting.WebForms.ReportDataSource(this.ds.Tables[i].TableName, this.ds.Tables[i]);
                rpt.DataSources.Add(o);
            }

            //string sReportName = Request.QueryString["ReportName"];
            System.IO.MemoryStream ms = new System.IO.MemoryStream(this.output);
            rpt.LoadReportDefinition(ms);


            Byte[] results = rpt.Render(fileType);

            return results;
        }
        
        public override void render(string sType,Stream st)
        {

            var localReport = new Microsoft.Reporting.WebForms.LocalReport();

            //rpt.ReportPath = "Rpt01.rdlc";
            localReport.EnableExternalImages = true;
            
            for (int i = 0; i < this.ds.Tables.Count; i++)
            {
                var o = new Microsoft.Reporting.WebForms.ReportDataSource(this.ds.Tables[i].TableName, this.ds.Tables[i]);
                localReport.DataSources.Add(o);
            }


            //string sReportName = Request.QueryString["ReportName"];
            System.IO.MemoryStream ms = new System.IO.MemoryStream(this.output);
            localReport.LoadReportDefinition(ms);
            //


            //MemoryStream ms = new MemoryStream();
            //file.PostedFile.InputStream.CopyTo(ms);
            //var byts = ms.ToArray();
            //ms.Dispose();
  
            byte[] bytes = localReport.Render(sType);

                st.Write(bytes, 0, bytes.Length);
            
        }
        /*

        public void render_old(string sType, Stream st)
        {

            var rpt = new Microsoft.Reporting.WinForms.ReportViewer();

            //rpt.ReportPath = "Rpt01.rdlc";
            rpt.LocalReport.EnableExternalImages = true;

            for (int i = 0; i < this.ds.Tables.Count; i++)
            {
                var o = new Microsoft.Reporting.WinForms.ReportDataSource(this.ds.Tables[i].TableName, this.ds.Tables[i]);
                rpt.LocalReport.DataSources.Add(o);
            }


            //string sReportName = Request.QueryString["ReportName"];
            System.IO.MemoryStream ms = new System.IO.MemoryStream(this.output);
            rpt.LocalReport.LoadReportDefinition(ms);
            //


            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;

            byte[] bytes = rpt.LocalReport.Render(
                sType, null, out mimeType, out encoding, out filenameExtension,
                out streamids, out warnings);

            st.Write(bytes, 0, bytes.Length);

        }
         */
    }
}
