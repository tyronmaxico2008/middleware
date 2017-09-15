using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DAL
{


    public class ExportToExcel
    {



        public Action<excelWriter, DataRow> row_created;

        public class clsCell
        {
            public string Text { get; set; }
            public int Index { get; set; }
            public bool bold { get; set; }
            public bool WrapText { get; set; }
            public int width { get; set; }
        }


        public class clsRow
        {
            public List<clsCell> Cells { get; set; }

            public clsRow()
            {
                Cells = new List<clsCell>();
            }

            public void addCell(string sText, int iIndex, bool bBold = false)
            {
                Cells.Add(new clsCell() { Index = iIndex, Text = sText, bold = bBold });
            }


        }

        public class clsHeader : List<clsRow>
        {
            public clsRow addRow()
            {
                var row = new clsRow();
                this.Add(row);
                return row;
            }
        }

        private List<ColMap> cols = new List<ColMap>();
        public clsHeader header = new clsHeader();

        public int getColCount()
        {
            return cols.Count;
        }

        public ColMap addColumn(string sField, string sTitle, string sFormat = "")
        {
            var col = new ColMap() { title = sTitle, datafield = sField,Format = sFormat };
            cols.Add(col);
            return col;
        }

        private void exportToExcel_old(DataSet source, string fileName)
        {

            System.IO.StreamWriter excelDoc;

            excelDoc = new System.IO.StreamWriter(fileName);
            const string startExcelXML = "<xml version>\r\n<Workbook " +
                  "xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" +
                  " xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n " +
                  "xmlns:x=\"urn:schemas-    microsoft-com:office:" +
                  "excel\"\r\n xmlns:ss=\"urn:schemas-microsoft-com:" +
                  "office:spreadsheet\">\r\n <Styles>\r\n " +
                  "<Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n " +
                  "<Alignment ss:Vertical=\"Bottom\"/>\r\n <Borders/>" +
                  "\r\n <Font/>\r\n <Interior/>\r\n <NumberFormat/>" +
                  "\r\n <Protection/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"BoldColumn\">\r\n <Font " +
                  "x:Family=\"Swiss\" ss:Bold=\"1\"/>\r\n </Style>\r\n " +
                  "<Style     ss:ID=\"StringLiteral\">\r\n <NumberFormat" +
                  " ss:Format=\"@\"/>\r\n </Style>\r\n <Style " +
                  "ss:ID=\"Decimal\">\r\n <NumberFormat " +
                  "ss:Format=\"0.0000\"/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"Integer\">\r\n <NumberFormat " +
                  "ss:Format=\"0\"/>\r\n </Style>\r\n <Style " +
                  "ss:ID=\"DateLiteral\">\r\n <NumberFormat " +
                  "ss:Format=\"mm/dd/yyyy;@\"/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"s64\"> \r\n " +
                  "<Alignment ss:Vertical=\"Bottom\" ss:WrapText=\"1\"/>  \r\n" +
                  "<NumberFormat ss:Format=\"@\"/> \r\n " +
                  "</Style> \r\n" +
                  "</Styles>\r\n ";


            const string endExcelXML = "</Workbook>";

            int rowCount = 0;
            int sheetCount = 1;
            /*
           <xml version>
           <Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"
           xmlns:o="urn:schemas-microsoft-com:office:office"
           xmlns:x="urn:schemas-microsoft-com:office:excel"
           xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">
           <Styles>
           <Style ss:ID="Default" ss:Name="Normal">
             <Alignment ss:Vertical="Bottom"/>
             <Borders/>
             <Font/>
             <Interior/>
             <NumberFormat/>
             <Protection/>
           </Style>
           <Style ss:ID="BoldColumn">
             <Font x:Family="Swiss" ss:Bold="1"/>
           </Style>
           <Style ss:ID="StringLiteral">
             <NumberFormat ss:Format="@"/>
           </Style>
           <Style ss:ID="Decimal">
             <NumberFormat ss:Format="0.0000"/>
           </Style>
           <Style ss:ID="Integer">
             <NumberFormat ss:Format="0"/>
           </Style>
           <Style ss:ID="DateLiteral">
             <NumberFormat ss:Format="mm/dd/yyyy;@"/>
           </Style>
           </Styles>
           <Worksheet ss:Name="Sheet1">
           </Worksheet>
           </Workbook>
           */
            excelDoc.Write(startExcelXML);
            excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
            excelDoc.Write("<Table>");
            excelDoc.Write("<Row>");
            for (int x = 0; x < source.Tables[0].Columns.Count; x++)
            {
                excelDoc.Write("<Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");
                excelDoc.Write(source.Tables[0].Columns[x].ColumnName);
                excelDoc.Write("</Data></Cell>");
            }
            excelDoc.Write("</Row>");
            foreach (DataRow x in source.Tables[0].Rows)
            {
                rowCount++;
                //if the number of rows is > 64000 create a new page to continue output
                if (rowCount == 64000)
                {
                    rowCount = 0;
                    sheetCount++;
                    excelDoc.Write("</Table>");
                    excelDoc.Write(" </Worksheet>");
                    excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
                    excelDoc.Write("<Table>");
                }
                excelDoc.Write("<Row>"); //ID=" + rowCount + "
                for (int y = 0; y < source.Tables[0].Columns.Count; y++)
                {
                    System.Type rowType;
                    rowType = x[y].GetType();
                    switch (rowType.ToString())
                    {
                        case "System.String":
                            string XMLstring = x[y].ToString();
                            XMLstring = XMLstring.Trim();
                            XMLstring = XMLstring.Replace("&", "&");
                            XMLstring = XMLstring.Replace(">", ">");
                            XMLstring = XMLstring.Replace("<", "<");
                            excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                           "<Data ss:Type=\"String\">");
                            excelDoc.Write(XMLstring);
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.DateTime":
                            //Excel has a specific Date Format of YYYY-MM-DD followed by  
                            //the letter 'T' then hh:mm:sss.lll Example 2005-01-31T24:01:21.000
                            //The Following Code puts the date stored in XMLDate 
                            //to the format above
                            DateTime XMLDate = (DateTime)x[y];
                            string XMLDatetoString = ""; //Excel Converted Date
                            XMLDatetoString = XMLDate.Year.ToString() +
                                 "-" +
                                 (XMLDate.Month < 10 ? "0" +
                                 XMLDate.Month.ToString() : XMLDate.Month.ToString()) +
                                 "-" +
                                 (XMLDate.Day < 10 ? "0" +
                                 XMLDate.Day.ToString() : XMLDate.Day.ToString()) +
                                 "T" +
                                 (XMLDate.Hour < 10 ? "0" +
                                 XMLDate.Hour.ToString() : XMLDate.Hour.ToString()) +
                                 ":" +
                                 (XMLDate.Minute < 10 ? "0" +
                                 XMLDate.Minute.ToString() : XMLDate.Minute.ToString()) +
                                 ":" +
                                 (XMLDate.Second < 10 ? "0" +
                                 XMLDate.Second.ToString() : XMLDate.Second.ToString()) +
                                 ".000";
                            excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\">" +
                                         "<Data ss:Type=\"DateTime\">");
                            excelDoc.Write(XMLDatetoString);
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.Boolean":
                            excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                        "<Data ss:Type=\"String\">");
                            excelDoc.Write(x[y].ToString());
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            excelDoc.Write("<Cell ss:StyleID=\"Integer\">" +
                                    "<Data ss:Type=\"Number\">");
                            excelDoc.Write(x[y].ToString());
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.Decimal":
                        case "System.Double":
                            excelDoc.Write("<Cell ss:StyleID=\"Decimal\">" +
                                  "<Data ss:Type=\"Number\">");
                            excelDoc.Write(x[y].ToString());
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.DBNull":
                            excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                  "<Data ss:Type=\"String\">");
                            excelDoc.Write("");
                            excelDoc.Write("</Data></Cell>");
                            break;
                        default:
                            throw (new Exception(rowType.ToString() + " not handled."));
                    }
                }
                excelDoc.Write("</Row>");
            }
            excelDoc.Write("</Table>");
            excelDoc.Write(" </Worksheet>");
            excelDoc.Write(endExcelXML);
            excelDoc.Close();
        }


        public byte[] exportToExcel(DataTable source)
        {
            var ds = new DataSet();
            ds.Tables.Add(source);
            return exportToExcel(ds);
        }
        public void exportToExcel(DataTable source, string fileName)
        {
            var ds = new DataSet();
            ds.Tables.Add(source);
            System.IO.File.WriteAllBytes(fileName, exportToExcel(ds));
        }

        public class excelWriter
        {
            System.IO.StreamWriter excelDoc;

            public excelWriter(System.IO.StreamWriter sw)
            {
                excelDoc = sw;

            }


            public void write_emptyrow(int iRowCount = 1)
            {
                for (int i = 0; i < iRowCount; i++)
                {
                    excelDoc.Write("<Row>"); //ID=" + rowCount + "
                    excelDoc.Write("</Row>");
                }
            }

            public void write(List<clsCell> cols)
            {

                excelDoc.Write("<Row>"); //ID=" + rowCount + "

                for (int i = 0; i < cols.Count; i++)
                {
                    var col = cols[i];

                    string XMLstring = col.Text;
                    XMLstring = XMLstring.Trim();
                    XMLstring = XMLstring.Replace("&", "&");
                    XMLstring = XMLstring.Replace(">", ">");
                    XMLstring = XMLstring.Replace("<", "<");



                    string sAttr = string.Format(" ss:Index=\"{0}\" ", col.Index);
                    if (col.WrapText)
                        sAttr += "  ss:StyleID=\"s64\" ";
                    else
                        sAttr += "  ss:StyleID=\"StringLiteral\" ";





                    excelDoc.Write("<Cell " + sAttr + " >" +
                                   "<Data ss:Type=\"String\">");
                    //excelDoc.Write("<Alignment ss:Vertical=\"Bottom\" ss:WrapText=\"" + 1 + "\" > ");
                    excelDoc.Write(XMLstring);
                    //excelDoc.Write("</Alignment>");

                    excelDoc.Write("</Data></Cell>");
                }
                excelDoc.Write("</Row>");
            }
        }





        public byte[] exportToExcel(DataSet source)
        {


            var workbook = new NPOI.HSSF.UserModel.HSSFWorkbook();

            var sheet = workbook.CreateSheet("data");

            var rowIndex = 0;
            var row = sheet.CreateRow(rowIndex);
            var font1 = workbook.CreateFont();
            font1.IsBold = true;



            for (int i = 0; i < cols.Count; i++)
            {
                var col = cols[i];

                var cell = row.CreateCell(i);
                cell.SetCellValue(col.title);
                cell.CellStyle = workbook.CreateCellStyle();
                cell.CellStyle.SetFont(font1);

                sheet.SetColumnWidth(i, 5500);

            }



            foreach (DataRow r in source.Tables[0].Rows)
            {
                rowIndex++;

                row = sheet.CreateRow(rowIndex);
                for (int iCol = 0; iCol < cols.Count; iCol++)
                {
                    var col = cols[iCol];

                    var cell = row.CreateCell(iCol);


                    string sVal = "";

                    if (r[col.datafield] is DateTime && col.Format.isEmpty() == false)
                        sVal = r.getDateFormat(col.datafield, col.Format);
                    else
                        sVal = r[col.datafield].ToString();


                    cell.SetCellValue(sVal);



                }
            }


            using (var fileData = new System.IO.MemoryStream())
            {
                workbook.Write(fileData);

                return g.ConvertStreamToByteArray(fileData);
            }


        }

        public byte[] exportToExcel_old(DataSet source)
        {


            System.IO.StreamWriter excelDoc;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            excelDoc = new System.IO.StreamWriter(ms);

            const string startExcelXML = "<xml version>\r\n<Workbook " +
                  "xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" +
                  " xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n " +
                  "xmlns:x=\"urn:schemas-    microsoft-com:office:" +
                  "excel\"\r\n xmlns:ss=\"urn:schemas-microsoft-com:" +
                  "office:spreadsheet\">\r\n <Styles>\r\n " +
                  "<Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n " +
                  "<Alignment ss:Vertical=\"Bottom\"/>\r\n <Borders/>" +
                  "\r\n <Font/>\r\n <Interior/>\r\n <NumberFormat/>" +
                  "\r\n <Protection/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"BoldColumn\">\r\n <Font " +
                  "x:Family=\"Swiss\" ss:Bold=\"1\"/>\r\n </Style>\r\n " +
                  "<Style     ss:ID=\"StringLiteral\">\r\n <NumberFormat" +
                  " ss:Format=\"@\"/>\r\n </Style>\r\n <Style " +
                  "ss:ID=\"Decimal\">\r\n <NumberFormat " +
                  "ss:Format=\"0.0000\"/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"Integer\">\r\n <NumberFormat " +
                  "ss:Format=\"0\"/>\r\n </Style>\r\n <Style " +
                  "ss:ID=\"DateLiteral\">\r\n <NumberFormat " +
                  "ss:Format=\"mm/dd/yyyy;@\"/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"s64\"> \r\n " +
                  "<Alignment ss:Vertical=\"Bottom\" ss:WrapText=\"1\"/>  \r\n" +
                  "<NumberFormat ss:Format=\"@\"/> \r\n " +
                  "</Style> \r\n" +
                  "</Styles>\r\n ";
            const string endExcelXML = "</Workbook>";

            int rowCount = 0;
            int sheetCount = 1;
            /*
           <xml version>
           <Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"
           xmlns:o="urn:schemas-microsoft-com:office:office"
           xmlns:x="urn:schemas-microsoft-com:office:excel"
           xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">
           <Styles>
           <Style ss:ID="Default" ss:Name="Normal">
             <Alignment ss:Vertical="Bottom"/>
             <Borders/>
             <Font/>
             <Interior/>
             <NumberFormat/>
             <Protection/>
           </Style>
           <Style ss:ID="BoldColumn">
             <Font x:Family="Swiss" ss:Bold="1"/>
           </Style>
           <Style ss:ID="StringLiteral">
             <NumberFormat ss:Format="@"/>
           </Style>
           <Style ss:ID="Decimal">
             <NumberFormat ss:Format="0.0000"/>
           </Style>
           <Style ss:ID="Integer">
             <NumberFormat ss:Format="0"/>
           </Style>
           <Style ss:ID="DateLiteral">
             <NumberFormat ss:Format="mm/dd/yyyy;@"/>
           </Style>
           </Styles>
           <Worksheet ss:Name="Sheet1">
           </Worksheet>
           </Workbook>
           */
            excelDoc.Write(startExcelXML);
            excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
            excelDoc.Write("<Table>");

            for (int x1 = 0; x1 < cols.Count; x1++)
            {
                int iColWidth = cols[x1].width == 0 ? 100 : cols[x1].width;
                excelDoc.Write("<Column ss:AutoFitWidth=\"0\" ss:Width=\"" + iColWidth + "\"/>");
            }

            addHeader(excelDoc);

            excelDoc.Write("<Row>");

            for (int x1 = 0; x1 < cols.Count; x1++)
            {
                var col = cols[x1];
                excelDoc.Write("<Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");
                excelDoc.Write(col.title);
                excelDoc.Write("</Data></Cell>");
            }
            excelDoc.Write("</Row>");

            foreach (DataRow r in source.Tables[0].Rows)
            {
                rowCount++;
                //if the number of rows is > 64000 create a new page to continue output
                if (rowCount == 64000)
                {
                    rowCount = 0;
                    sheetCount++;
                    excelDoc.Write("</Table>");
                    excelDoc.Write(" </Worksheet>");
                    excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
                    excelDoc.Write("<Table>");
                }
                excelDoc.Write("<Row>"); //ID=" + rowCount + "

                for (int y1 = 0; y1 < cols.Count; y1++)
                {
                    var col = cols[y1];
                    if (!string.IsNullOrWhiteSpace(col.datafield))
                        addValue(excelDoc, r[col.datafield]);
                }

                excelDoc.Write("</Row>");

                if (row_created != null)
                {
                    var arg = new excelWriter(excelDoc);
                    row_created(arg, r);
                }
            }
            excelDoc.Write("</Table>");
            excelDoc.Write(" </Worksheet>");
            excelDoc.Write(endExcelXML);
            excelDoc.Flush();

            byte[] ret = g.ConvertStreamToByteArray(ms);

            //excelDoc.Close();

            return ret;


        }
        private void addHeader(System.IO.StreamWriter excelDoc)
        {
            /*
            for (int i = 0; i < cols.Count; i++)
            {
                var col = cols[i];
                if (col.width > 0)
                {
                    excelDoc.Write("<Column ss:Index=\"" + col.width + "\" ss:AutoFitWidth=\"0\" ss:Width=\"" + (i + 1) + "\" />");
                }
            }
            */

            foreach (var row in header)
            {
                excelDoc.Write("<Row>");
                foreach (var cell in row.Cells)
                {
                    string sAttr = string.Format(" ss:Index=\"{0}\" ", cell.Index);
                    if (cell.bold) sAttr += "  ss:StyleID=\"BoldColumn\" ";

                    excelDoc.Write("<Cell " + sAttr + "><Data ss:Type=\"String\">");
                    excelDoc.Write(cell.Text);
                    excelDoc.Write("</Data></Cell>");
                }

                excelDoc.Write("</Row>");
            }


        }

        public static void addValue(System.IO.StreamWriter excelDoc, object val)
        {

            System.Type rowType;
            rowType = val.GetType();
            switch (rowType.ToString())
            {
                case "System.String":
                    string XMLstring = val.ToString();
                    XMLstring = XMLstring.Trim();
                    XMLstring = XMLstring.Replace("&", "&");
                    XMLstring = XMLstring.Replace(">", ">");
                    XMLstring = XMLstring.Replace("<", "<");
                    excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                   "<Data ss:Type=\"String\">");
                    excelDoc.Write(XMLstring);
                    excelDoc.Write("</Data></Cell>");
                    break;
                case "System.DateTime":
                    //Excel has a specific Date Format of YYYY-MM-DD followed by  
                    //the letter 'T' then hh:mm:sss.lll Example 2005-01-31T24:01:21.000
                    //The Following Code puts the date stored in XMLDate 
                    //to the format above
                    DateTime XMLDate = (DateTime)val;
                    string XMLDatetoString = ""; //Excel Converted Date
                    XMLDatetoString = XMLDate.Year.ToString() +
                         "-" +
                         (XMLDate.Month < 10 ? "0" +
                         XMLDate.Month.ToString() : XMLDate.Month.ToString()) +
                         "-" +
                         (XMLDate.Day < 10 ? "0" +
                         XMLDate.Day.ToString() : XMLDate.Day.ToString()) +
                         "T" +
                         (XMLDate.Hour < 10 ? "0" +
                         XMLDate.Hour.ToString() : XMLDate.Hour.ToString()) +
                         ":" +
                         (XMLDate.Minute < 10 ? "0" +
                         XMLDate.Minute.ToString() : XMLDate.Minute.ToString()) +
                         ":" +
                         (XMLDate.Second < 10 ? "0" +
                         XMLDate.Second.ToString() : XMLDate.Second.ToString()) +
                         ".000";
                    excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\">" +
                                 "<Data ss:Type=\"DateTime\">");
                    excelDoc.Write(XMLDatetoString);
                    excelDoc.Write("</Data></Cell>");
                    break;

                case "System.Boolean":
                    excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                "<Data ss:Type=\"String\">");
                    excelDoc.Write(val.ToString());
                    excelDoc.Write("</Data></Cell>");
                    break;
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                    excelDoc.Write("<Cell ss:StyleID=\"Integer\">" +
                            "<Data ss:Type=\"Number\">");
                    excelDoc.Write(val.ToString());
                    excelDoc.Write("</Data></Cell>");
                    break;
                case "System.Decimal":
                case "System.Double":
                    excelDoc.Write("<Cell ss:StyleID=\"Decimal\">" +
                          "<Data ss:Type=\"Number\">");
                    excelDoc.Write(val.ToString());
                    excelDoc.Write("</Data></Cell>");
                    break;
                case "System.DBNull":
                    excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                          "<Data ss:Type=\"String\">");
                    excelDoc.Write("");
                    excelDoc.Write("</Data></Cell>");
                    break;
                default:
                    throw (new Exception(rowType.ToString() + " not handled."));
            }


        }



        public static clsFiles exportToCsv(DataSet ds)
        {

            clsFiles oFiles = new clsFiles();

            foreach (DataTable t in ds.Tables)
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    /*
                    var sw = new System.IO.StreamWriter(ms);
                    for (int i = 0; i < t.Columns.Count; i++)
                    {
                        sw.Write("{0},", t.Columns[i].ColumnName);
                    }
                    sw.Write("\r\n");
                    foreach (DataRow r in t.Rows)
                    {
                        for (int iCol = 0; iCol < t.Columns.Count; iCol++)
                        {
                            sw.Write("{0},", r[iCol].ToString());
                        }

                        sw.Write("\r\n");
                    }

                    */

                    var sw = new System.IO.StreamWriter(ms);
                    sw.WriteLine("<?xml version=\"1.0\" standalone=\"yes\"?>");
                    t.WriteXml(sw, true);


                    var oFileData = new FileData();

                    oFileData.ContentType = "application/unknown";
                    oFileData.FileName = t.TableName + ".xls";
                    oFileData.Data = g.ConvertStreamToByteArray(ms);

                    oFiles.Add(oFileData);



                }
            }

            return oFiles;
        }

    }
}
