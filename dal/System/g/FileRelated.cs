using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace System
{
    partial class g
    {
        public static string getFileExtension(string sFileName)
        {
            string[] sExtensions = sFileName.Split('.');

            return sExtensions[sExtensions.Length - 1];
        }

        public static bool isFileExtension(string sFile, params string[] extensions)
        {
            foreach (var sExtension in extensions)
            {
                if (getFileExtension(sFile).ToLower() == sExtension.ToLower())
                    return true;
            }

            return false;
        }

        public static byte[] ConvertFileToByteArray(string sFilePath)
        {

            byte[] data;

            using (var st = new System.IO.FileStream(sFilePath, FileMode.Open))
            {
                data = ConvertStreamToByteArray(st);
            }
            return data;
        }

        public static MemoryStream ConvertBytesToMemoryStream(byte[] data)
        {
            var ms = new MemoryStream(data);
            return ms;
        }

        public static System.IO.MemoryStream getMemoryStreamFrom(string sPath)
        {
            return getMemoryStreamFrom(ConvertFileToByteArray(sPath));
        }
        public static System.IO.MemoryStream getMemoryStreamFrom(byte[] data)
        {
            System.IO.MemoryStream ms = new MemoryStream(data, true);

            return ms;
        }

        public static byte[] ConvertStreamToByteArray(Stream inputStream)
        {
            if (!inputStream.CanRead)
            {
                throw new ArgumentException();
            }

            // This is optional
            if (inputStream.CanSeek)
            {
                inputStream.Seek(0, SeekOrigin.Begin);
            }

            byte[] output = new byte[inputStream.Length];
            int bytesRead = inputStream.Read(output, 0, output.Length);
            //Debug.Assert(bytesRead == output.Length, "Bytes read from stream matches stream length");


            return output;
        }
    }
}
