using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DAL;
namespace NTier.sqlbuilder
{
    public class sqlCRUD
    {

        public string TableName { get; set; }
        public string ViewName { get; set; }
        public string PrimaryKeyField { get; set; }
        public bool isIdentity { get; set; }

        public sqlCRUD(string sTableName
            , string sViewName
            , string sPrimaryKeyField
            , bool blnIsIdenity
            )
        {

            TableName = sTableName;
            ViewName = sViewName;
            PrimaryKeyField = sPrimaryKeyField;
            isIdentity = blnIsIdenity;
        }


        public string MaxIDScript()
        {
            return string.Format("SELECT MAX({1}) FROM [{0}] ", TableName, PrimaryKeyField);
        }

        private void correctCommand(clsCmd cmd, DataTable t)
        {

            foreach (DataColumn col in t.Columns)
            {

                if (cmd.ContainFields(col.ColumnName) && col.DataType.Name == "Decimal")
                {
                    switch (col.DataType.Name)
                    {
                        case "Decimal":
                        case "Double":
                            cmd.setValue(col.ColumnName, g.parseDecimal(cmd.getStringValue(col.ColumnName)));
                            break;
                        case "Int32":
                        case "Int16":
                            cmd.setValue(col.ColumnName, g.parseInt( cmd.getStringValue(col.ColumnName)));
                            break;
                    }

                }
            }
        }


        public clsCmd getInsertCommand(DataTable t
            , clsCmd cmd)
        {

            clsCmd cmd2 = new clsCmd();

            int i = 0;
            StringBuilder sb1 = new StringBuilder();

            if (isIdentity == false)
            {
                sb1.AppendLine("declare @id int ");
                sb1.AppendFormat("set @id  = (select isnull(max({0}),0) + 1 from {1}) \n\r ", PrimaryKeyField, TableName);
            }

            sb1.AppendFormat("insert into {0} \n\r (", TableName);


            if (isIdentity == false)
            {
                sb1.AppendFormat(" {0} ", PrimaryKeyField);
                i++;
            }

            //simple column selection
            foreach (DataColumn col in t.Columns)
            {
                if (cmd.ContainFields(col.ColumnName) && col.ColumnName.ToLower() != PrimaryKeyField.ToLower())
                {

                    if (i > 0) sb1.Append(",");

                    sb1.AppendFormat(" {0} ", col.ColumnName);
                    i++;
                    cmd2.setValue(col.ColumnName, cmd[col.ColumnName].Value);
                }
            }

            //simeple column selection for filedata
            foreach (FileData file in cmd.Files)
            {
                if (t.Columns.Contains(file.FieldName) && file.FieldName.ToLower() != PrimaryKeyField.ToLower())
                {

                    if (i > 0) sb1.Append(",");
                    sb1.AppendFormat(" {0} ", file.FieldName);
                    i++;
                    cmd2.setValue(file.FieldName, file.Data);

                    if (t.Columns.Contains(file.FieldName + "_contenttype"))
                    {
                        sb1.AppendFormat(", {0} ", file.FieldName + "_ContentType");
                        cmd2.setValue(file.FieldName + "_ContentType", file.ContentType);
                    }
                }

            }

            i = 0;

            sb1.AppendLine(") \n\r values(");

            if (isIdentity == false)
            {
                sb1.AppendFormat(" @id ", PrimaryKeyField);
                i++;
            }

            //simple value selection
            foreach (DataColumn col in t.Columns)
            {
                if (cmd.ContainFields(col.ColumnName) && col.ColumnName.ToLower() != PrimaryKeyField.ToLower())
                {

                    if (i > 0) sb1.Append(",");
                    sb1.AppendFormat(" @{0} ", col.ColumnName);
                    i++;
                }
            }

            //simple value selection for file.
            foreach (FileData file in cmd.Files)
            {
                if (t.Columns.Contains(file.FieldName) && file.FieldName.ToLower() != PrimaryKeyField.ToLower())
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

            if (isIdentity)
                sb1.AppendLine("select SCOPE_IDENTITY()");
            else
                sb1.AppendLine("select @id");


            cmd2.SQL = sb1.ToString();
            return cmd2;
        }



        public clsCmd getUpdateCommand(DataTable t
            , clsCmd cmd)
        {

            int iID = cmd.getIntValue(PrimaryKeyField);
            if (iID == 0) throw new Exception("Id value is 0, you can't update record !");

            StringBuilder sb1 = new StringBuilder();
            clsCmd cmd2 = new clsCmd();
            int i = 0;
            sb1.AppendFormat("update {0} set \r\n", TableName);


            foreach (DataColumn col in t.Columns)
            {
                if (cmd.ContainFields(col.ColumnName) && col.ColumnName.ToLower() != PrimaryKeyField.ToLower())
                {
                    if (i > 0) sb1.Append(",");
                    sb1.AppendFormat(" {0} = @{0} ", col.ColumnName);
                    i++;
                    cmd2.setValue(col.ColumnName, cmd[col.ColumnName].Value);
                }
            }

            foreach (FileData file in cmd.Files)
            {
                if (t.Columns.Contains(file.FieldName) && file.FieldName.ToLower() != PrimaryKeyField.ToLower())
                {

                    if (i > 0) sb1.Append(",");
                    sb1.AppendFormat(" {0} = @{0} ", file.FieldName);
                    i++;
                    cmd2.setValue(file.FieldName, file.Data);

                    if (t.Columns.Contains(file.FieldName + "_contenttype"))
                    {
                        sb1.AppendFormat(", {0} = @{0} ", file.FieldName + "_ContentType");
                        cmd2.setValue(file.FieldName + "_ContentType", file.ContentType);
                    }
                }
            }

            sb1.AppendFormat(" where {0} = {1} \n\r", PrimaryKeyField, iID);

            sb1.AppendFormat("select {0}", iID);

            cmd2.SQL = sb1.ToString();


            return cmd2;


        }



        public clsCmd getSaveCommand(DataTable t, clsCmd cmd)
        {
            int iID = cmd.getIntValue(PrimaryKeyField);
            correctCommand(cmd, t);
            if (iID == 0)
                return getInsertCommand(t, cmd);
            else
                return getUpdateCommand(t, cmd);
        }

        public string getInsertScript(string[] Columns)
        {

            var cols = new List<string>();

            //if (!isIdentity) cols.Add(PrimaryKey);

            cols.AddRange(Columns);

            StringBuilder sb1 = new StringBuilder();

            sb1.AppendFormat("INSERT INTO {0} \n", TableName);
            sb1.AppendLine("(");

            int i = 0;

            foreach (string s in cols)
            {


                if (isIdentity && s.ToLower() == PrimaryKeyField.ToLower()) continue;

                if (i > 0) sb1.Append(",");
                sb1.Append(s);
                sb1.AppendLine();


                i++;
            }

            sb1.AppendLine(")");
            sb1.AppendLine(" VALUES( ");
            i = 0;
            foreach (var s in cols)
            {
                if (isIdentity && s.ToLower() == PrimaryKeyField.ToLower()) continue;

                if (i > 0) sb1.Append(",");
                sb1.AppendFormat("@{0}", s);
                sb1.AppendLine();
                i++;
            }
            sb1.AppendLine(" )");

            if (isIdentity) sb1.AppendLine("select Scope_identity()");

            return sb1.ToString();
        }


        public string getUpdateScript(params string[] columns)
        {
            StringBuilder sb1 = new StringBuilder();

            sb1.AppendFormat("update {0}  SET\n", TableName);

            int i = 0;
            foreach (var s in columns)
            {
                if (s.ToLower() != PrimaryKeyField.ToLower())
                {
                    if (i > 0) sb1.Append(",");
                    sb1.AppendFormat("{0}       =       @{0}", s);
                    sb1.AppendLine();
                    i++;
                }
            }
            sb1.AppendFormat("\n WHERE {0} = @{0}", PrimaryKeyField);
            return sb1.ToString();
        }

        public string getDeleteScript()
        {
            return string.Format("SELECT * FROM {0} WHERE {1} = @{1}", TableName, PrimaryKeyField);
        }


        public string getDeleteScript(int iID)
        {
            return string.Format("SELECT * FROM {0} WHERE {1} = {2}", TableName, PrimaryKeyField, iID);
        }


        public string getSelectScript()
        {
            return string.Format("SELECT * FROM {0} WHERE {1} = @{1}", TableName, PrimaryKeyField);
        }

        public string getSelectScript(int iID)
        {
            return string.Format("SELECT * FROM {0} WHERE {1} = {2}", TableName, PrimaryKeyField, iID);
        }


    }
}
