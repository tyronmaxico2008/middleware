using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTier.adapter;




namespace NTier.Request
{
    internal class sqlAppConfig : iConfig
    {
        private string _connectionString = "";
        private clsDataAdapterBase adapter = null;
        public sqlAppConfig(string sConnectionString)
        {
            _connectionString = sConnectionString;
            adapter = new clsSQLAdapter(_connectionString);
        }
        public clsDataAdapterBase getAdapter()
        {
            return adapter;
        }
        public string getConnectionString()
        {
            return _connectionString;
        }
    }
}
