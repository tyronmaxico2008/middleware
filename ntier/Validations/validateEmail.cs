using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
namespace NTier.Validations
{

    internal class validateEmail : validateText
    {
        public override clsMsg validate(clsCmd cmd)
        {

            string sVal = cmd.getStringValue(this.FieldName);

            if (sVal.isEmpty() == false && g.isEmail(sVal) == false)
                return g.msg(string.Format("Wrong Email value [{1}] \nYou have not specified proper email value for field [{0}]", this.FieldTitle, sVal));
            return g.msg("");
        }
    }
}
