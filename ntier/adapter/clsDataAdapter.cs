using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using System.Data;
using System.Data.SqlClient;
namespace NTier.adapter
{
    internal class clsSQLAdapter : clsDataAdapterBase
    {
        string _connectionString = "";
        public clsSQLAdapter(string sConnectionString)
        {
            _connectionString = sConnectionString;
        }

        
        public override void exec(clsCmd cmd)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var sqlcmd = conn.CreateCommand();
                utility.setCommand(cmd, sqlcmd);
                sqlcmd.CommandText = cmd.SQL;
                sqlcmd.CommandType = cmd.CommandType;

                sqlcmd.ExecuteNonQuery();
            }
        }

        public override DataTable getData(clsCmd cmd)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                var sqlcmd = conn.CreateCommand();
                utility.setCommand(cmd, sqlcmd);
                sqlcmd.CommandText = cmd.SQL;
                sqlcmd.CommandType = cmd.CommandType;

                SqlDataAdapter ad = new SqlDataAdapter(sqlcmd);
                DataTable t = new DataTable();
                ad.Fill(t);
                return t;
            }
        }

        public override object execScalar(clsCmd cmd)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var sqlcmd = conn.CreateCommand();
               
                utility.setCommand(cmd, sqlcmd);
                sqlcmd.CommandText = cmd.SQL;
                sqlcmd.CommandType = cmd.CommandType;

                return sqlcmd.ExecuteScalar();
            }
        }



        //private void saveTable(string sTableName, string sPrimaryKeyField, clsCmd cmd)
        //{
        //    using (SqlConnection conn = new SqlConnection(_connectionString))
        //    {
        //        return save(conn, cmd, sTableName, sPrimaryKeyField);
        //    }
        //}


        private void save()
        {

 
        }

        private clsMsg save_old(SqlConnection cnn, clsCmd cmd,string _TableName,string _sPrimaryKeyField)
        {


            int iID = cmd.getIntValue(_sPrimaryKeyField);
            StringBuilder sb1 = new StringBuilder();

            DataTable t = getData(string.Format("select top 1 * from {0} where {1} = " + iID, _TableName, _sPrimaryKeyField));
            var sqlcmd = cnn.CreateCommand();
            sqlcmd.CommandText = "";

            int i = 0;
            if (iID == 0)
            {

                sb1.AppendFormat("insert into {0} \n\r (", _TableName);

                //simple column selection
                foreach (DataColumn col in t.Columns)
                {
                    if (cmd.ContainFields(col.ColumnName) && col.ColumnName.ToLower() != _sPrimaryKeyField.ToLower())
                    {

                        if (i > 0) sb1.Append(",");
                        sb1.AppendFormat(" {0} ", col.ColumnName);
                        i++;
                        sqlcmd.Parameters.AddWithValue(col.ColumnName, cmd[col.ColumnName].Value);
                    }
                }

                //simeple column selection for filedata
                foreach (FileData file in cmd.Files)
                {
                    if (t.Columns.Contains(file.FieldName) && file.FieldName.ToLower() != _sPrimaryKeyField.ToLower())
                    {

                        if (i > 0) sb1.Append(",");
                        sb1.AppendFormat(" {0} ", file.FieldName);
                        i++;
                        sqlcmd.Parameters.AddWithValue(file.FieldName, file.Data);

                        if (t.Columns.Contains(file.FieldName + "_contenttype"))
                        {
                            sb1.AppendFormat(", {0} ", file.FieldName + "_ContentType");
                            sqlcmd.Parameters.AddWithValue(file.FieldName + "_ContentType", file.ContentType);
                        }
                    }

                }

                i = 0;
                sb1.AppendLine(") \n\r values(");
                //simple value selection
                foreach (DataColumn col in t.Columns)
                {
                    if (cmd.ContainFields(col.ColumnName) && col.ColumnName.ToLower() != _sPrimaryKeyField.ToLower())
                    {

                        if (i > 0) sb1.Append(",");
                        sb1.AppendFormat(" @{0} ", col.ColumnName);
                        i++;
                    }
                }

                //simple value selection for file.
                foreach (FileData file in cmd.Files)
                {
                    if (t.Columns.Contains(file.FieldName) && file.FieldName.ToLower() != _sPrimaryKeyField.ToLower())
                    {

                        if (i > 0) sb1.Append(",");
                        sb1.AppendFormat(" @{0} ", file.FieldName);
                        i++;


                        if (t.Columns.Contains(file.FieldName + "_contenttype"))
                        {
                            sb1.AppendFormat(", @{0} ", file.FieldName + "_ContentType");

                        }
                    }

                }


                sb1.Append(" )");

                sb1.AppendLine("select SCOPE_IDENTITY()");
                sqlcmd.CommandText = sb1.ToString();
                try
                {
                    iID = g.parseInt(sqlcmd.ExecuteScalar());
                    return g.msg("", iID);
                }
                catch (Exception ex)
                {
                    return g.msg_exception(ex);

                }


            }
            else
            {
                if (t.Rows.Count > 0)
                {
                    i = 0;

                    sb1.AppendFormat("update {0} set \r\n", _TableName);

                    foreach (DataColumn col in t.Columns)
                    {
                        if (cmd.ContainFields(col.ColumnName) && col.ColumnName.ToLower() != _sPrimaryKeyField.ToLower())
                        {

                            if (i > 0) sb1.Append(",");
                            sb1.AppendFormat(" {0} = @{0} ", col.ColumnName);
                            i++;
                            sqlcmd.Parameters.AddWithValue(col.ColumnName, cmd[col.ColumnName].Value);
                        }
                    }

                    foreach (FileData file in cmd.Files)
                    {
                        if (t.Columns.Contains(file.FieldName) && file.FieldName.ToLower() != _sPrimaryKeyField.ToLower())
                        {

                            if (i > 0) sb1.Append(",");
                            sb1.AppendFormat(" {0} = @{0} ", file.FieldName);
                            i++;
                            sqlcmd.Parameters.AddWithValue(file.FieldName, file.Data);

                            if (t.Columns.Contains(file.FieldName + "_contenttype"))
                            {
                                sb1.AppendFormat(", {0} = @{0} ", file.FieldName + "_ContentType");
                                sqlcmd.Parameters.AddWithValue(file.FieldName + "_ContentType", file.ContentType);
                            }
                        }
                    }

                    sb1.AppendFormat(" where {0} = {1}", _sPrimaryKeyField, iID);
                }

            }


            //////////////////////////////////////////////

            sqlcmd.CommandText = sb1.ToString();
            try
            {
                sqlcmd.ExecuteNonQuery();
                return g.msg("", iID);
            }
            catch (Exception ex)
            {
                return g.msg_exception(ex);

            }

        }
    
    }

}
