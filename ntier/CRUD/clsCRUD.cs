using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using NTier.sqlbuilder;
using NTier.adapter;
using System.Data;
namespace NTier.CRUD
{
    public class clsCRUD : sqlCRUD
    {
        clsDataAdapterBase _adapter = null;
        public clsCRUD(clsDataAdapterBase adapter
            , string sTableName
            , string sViewName
            , string sPrimaryKey
            , bool blnIsIdentity)
            : base(sTableName, sViewName, sPrimaryKey, blnIsIdentity)
        {
            _adapter = adapter;
        }


        private clsCmd getCmdFromRow(DataRow r)
        {
            var cmd = new clsCmd();

            foreach (DataColumn col in r.Table.Columns)
            {

                cmd.setValue(col.ColumnName, r[col.ColumnName]);
            }

            return cmd;
        }


        public void updateTable(DataTable t)
        {
            var tTable = _adapter.getData("select top 0 * from " + TableName);
            
            foreach (DataRow r in t.Rows)
            {
                var cmd2 = getCmdFromRow(r);
                cmd2 = getSaveCommand(tTable, cmd2);
                _adapter.exec(cmd2);
            }
        }

        public clsMsg save(clsCmd cmd)
        {
            var t = _adapter.getData("select top 0 * from " + TableName);
            var cmd2 = getSaveCommand(t, cmd);

            try
            {
                var obj = _adapter.execScalar(cmd2);
                return g.msg("", obj);
            }
            catch (Exception ex)
            {
                return g.msg_exception(ex);
            }


        }

        public void delete(clsCmd cmd)
        {
            var iID = cmd.getIntValue("id");
            string q = "delete from " + TableName + " where " + " " + PrimaryKeyField + " = " + iID;
            _adapter.exec(q);
        }
    }
}
