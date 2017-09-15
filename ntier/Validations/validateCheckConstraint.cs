using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;

namespace NTier.Validations
{
    internal class validateCheckConstraint : validateBase
    {
        string[] values;

        public validateCheckConstraint(string[] Values)
        {
            values = Values;
        }
        public override clsMsg validate(clsCmd cmd)
        {
            string sVal = cmd.getStringValue(FieldName);
            if (!sVal.Contains(values))
                return g.msg("Wrong value entered in [" + this.FieldTitle + "] ! ");

            return g.msg("");
        }

    }
}
