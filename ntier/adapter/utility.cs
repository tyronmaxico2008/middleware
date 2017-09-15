using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DAL;

namespace NTier.adapter
{

    public class utility
    {

        public static clsDataAdapterBase createAdapter(string sConnectionType, string sConnectionString)
        {

            clsDataAdapterBase _adapter = null;
            switch (sConnectionType.ToLower())
            {
                case "mssql":
                    _adapter = new clsSQLAdapter(sConnectionString);
                    return _adapter;
                case "mysql":
                case "access":
                    throw new Exception("Not implemented !");
                default:
                    throw new Exception("Unknown connection type !");
            }
        }

        public static void setCommand(clsCmd cmd
            , SqlCommand sqlcmd
            , CommandType iCommandType = CommandType.Text)
        {
            foreach (var f in cmd)
            {
                sqlcmd.Parameters.AddWithValue(f.Name, f.Value);
            }

            sqlcmd.CommandType = iCommandType;
            sqlcmd.CommandText = cmd.SQL;
        }
    }
}
