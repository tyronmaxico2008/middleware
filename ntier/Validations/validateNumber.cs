using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;

namespace NTier.Validations
{
    internal class validateNumber : validateText
    {


        public bool AllowZero { get; set; }
        public override clsMsg validate(clsCmd cmd)
        {

            string sVal = cmd.getStringValue(this.FieldName);

            if (cmd.ContainFields(this.FieldName) && sVal.isEmpty())
            {
                cmd.setValue(this.FieldName, 0);
                return g.msg("");
            }

            if (sVal.isEmpty() == false && g.isNumeric(sVal) == false)
            {
                return g.msg(string.Format("Wrong numeric value [{1}] \nYou have not specified proper numeric value for field [{0}]", this.FieldTitle, sVal));

            }

            if (AllowZero == false
                && g.parseDouble(sVal) == 0.0)
                return g.msg(string.Format("The field [{0}] doesn't allow zero value, please specify some value for this field !", this.FieldTitle));


            return g.msg();
        }
    }
}
