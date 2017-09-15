using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;

namespace NTier.Validations
{
    internal class validateText : validateBase
    {
        public int MaxLength { get; set; }
        public bool Required { get; set; }

        public override clsMsg validate(clsCmd cmd)
        {
            string sVal = cmd.getStringValue(this.FieldName);

            if (Required)
                if (sVal.isEmpty())
                    return g.msg(string.Format("Field value [{0}] is empty !", this.FieldTitle));


            if (MaxLength > 0 && sVal.isEmpty() == false && sVal.Length > MaxLength)
                return g.msg(string.Format("Max Legnth of field [{0}] is [{1}], you have entered more than that length !", this.FieldTitle, this.MaxLength));

            return g.msg("");
        }

    }
}
