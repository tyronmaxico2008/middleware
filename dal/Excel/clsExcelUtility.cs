using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace DAL
{
    public class clsExcelUtility
    {


        public static byte[] getExcelData(DataTable t)
        {
            var cols = new GridColumns();
            foreach (DataColumn col in t.Columns)
            {
                cols.Add(col.ColumnName, col.ColumnName, 100);
            }

            return getExcelData(t, cols);
        }


        public static byte[] getExcelData(DataTable t
            , GridColumns cols)
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



            foreach (DataRow r in t.Rows)
            {
                rowIndex++;

                row = sheet.CreateRow(rowIndex);
                for (int iCol = 0; iCol < cols.Count; iCol++)
                {
                    var col = cols[iCol];

                    var cell = row.CreateCell(iCol);
                    cell.SetCellValue(r[col.datafield].ToString());
                    //cell.CellStyle.GetFont(workbook).IsBold = false;

                }
            }


            using (var fileData = new System.IO.MemoryStream())
            {
                workbook.Write(fileData);

                return g.ConvertStreamToByteArray(fileData);
            }


        }

    }
}
