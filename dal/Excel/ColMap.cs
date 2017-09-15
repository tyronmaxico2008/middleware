using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL
{
    public class ColMap
    {
        public string title { get; set; }
        public string datafield { get;set;}

        public int width { get; set; }

        public string Format { get; set; }
    }

    public class GridColumns : List<ColMap>
    {

        public string PrimaryKeyField { get; set; }
        public ColMap Add(string sTitle, string sField, int iWidth)
        {
            var col = new ColMap() { datafield = sField, title = sTitle, width = iWidth };
            this.Add(col);

            return col;
        }

    }


}
