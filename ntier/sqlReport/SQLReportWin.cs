using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using Microsoft.Reporting.WinForms;

namespace NTier.sqlReport
{
    public class SQLReportWin : SQLReportBase
    {



        public SQLReportWin()
        {
            
        }

        public override byte[] export(string fileType)
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
        

        public override void render(string sType
            ,Stream st)
        {

            /*
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






            byte[] bytes = localReport.Render(sType);

                st.Write(bytes, 0, bytes.Length);
            */
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
