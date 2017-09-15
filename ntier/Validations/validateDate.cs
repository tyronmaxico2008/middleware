using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;

namespace NTier.Validations
{
    internal class validateDate : validateText
    {

        public string DateFormat { get; set; }

        public override clsMsg validate(clsCmd cmd)
        {

            var msg = base.validate(cmd);
            if (msg.Validated == false) return msg;

            string sVal = cmd.getStringValue(this.FieldName);

            if (sVal.isEmpty() == false && g.isDate(sVal, this.DateFormat) == false)
                return g.msg(string.Format("Wrong Date value [{1}] \nYou have not specified proper Date value for field [{0}]", this.FieldTitle, sVal));
            return g.msg("");
        }

    }
}
