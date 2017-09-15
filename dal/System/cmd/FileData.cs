using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public class FileData
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
        public string FieldName { get; set; }

        public string FileExtension { get; set; }



        public FileData()
        {
            List<byte> b = new List<byte>();
            b.Add(0);

            Data = b.ToArray();

        }
    }

}
