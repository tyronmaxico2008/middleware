using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DAL;

namespace NTier.adapter
{
    public abstract class clsDataAdapterBase : iAdapter
    {
        public abstract void exec(clsCmd cmd);
        public abstract DataTable getData(clsCmd cmd);
        public abstract object execScalar(clsCmd cmd);

        public void exec(string sCommand)
        {
            var cmd = new clsCmd();
            cmd.SQL = sCommand;
            exec(cmd);
        }

        public object execScalar(string sCommand)
        {
            var cmd = new clsCmd();
            cmd.SQL = sCommand;
            return execScalar(cmd);
        }

        public DataTable getData(string sql)
        {

            var cmd = new clsCmd();
            cmd.SQL = sql;
            return getData(cmd);
        }
    }
}
