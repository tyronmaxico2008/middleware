using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using NTier.adapter;
using NTier.sqlbuilder;

namespace NTier.Validations
{
    internal class validateDuplicate : validateText
    {


        iAdapter _adapter = null;
        List<string> sFields = new List<string>();
        private string _TableName = "";
        private string _PrimaryKeyField = "";

        public validateDuplicate(iAdapter adapter
            , string sTableName
            , string sPrimaryKeyField
            , params string[] sFields1)
        {

            _adapter = adapter;
            _TableName = sTableName;
            _PrimaryKeyField = sPrimaryKeyField;
            sFields.AddRange(sFields1);
        }

        public override clsMsg validate(clsCmd cmd)
        {


            if (this.Required == false
                && sFields.Count > 0
                && cmd.getStringValue(sFields[0]).isEmpty())
                return g.msg("");


            List<string> sValues = new List<string>();

            var cmd2 = new clsCmd();
            foreach (var sField in sFields)
            {
                string sVal = cmd.getStringValue(sField);
                cmd2.setValue(sField, sVal);
                sValues.Add(sVal);
            }

            string q = "select count(*) from " + _TableName;
            q += " where 1=1 and " + _PrimaryKeyField + " <> " + cmd.getIntValue(_PrimaryKeyField);
            cmd2.SQL = NTier.sqlbuilder.sqlUtility.joinWhereCondition(q, cmd2);

            var ret = g.parseInt(_adapter.execScalar(cmd2).ToString());

            if (ret > 0)
            {

                var label_fields = this.FieldTitle;
                var label_values = string.Join(",", sValues.ToArray());
                return g.msg(string.Format("Values [{0}] already exists for fields [{1}],duplication not allowed !", label_values, label_fields));
            }

            return g.msg("");
        }

        public void addFields(params string[] s)
        {
            sFields.AddRange(s);
        }

    }
}
