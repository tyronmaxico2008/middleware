using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
namespace NTier.Validations
{
    class validationDropDownID : validateBase

    {

        public override clsMsg validate(clsCmd cmd)
        {
            string sValue = cmd.getStringValue(this.FieldName);


            if (sValue.isEmpty() || g.parseInt(sValue) == 0)
            {
                return g.msg(string.Format("Please specify value for field [" + FieldTitle + "] !"));
 
            }

            return g.msg("");

        }
    }
}
