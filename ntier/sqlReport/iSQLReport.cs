using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace NTier.sqlReport
{

    public interface iSQLReport
    {
         //byte[] render(string sType);
         void render(string sType, Stream st);
         string downloadName { get; set; }

    }
}
