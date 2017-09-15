using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;

namespace NTier.Validations
{
    internal class validateUnknown : validateBase
    {

        Func<clsCmd, clsMsg> _fnValidate = null;

        public validateUnknown(Func<clsCmd, clsMsg> fnValidate)
        {
            _fnValidate = fnValidate;
        }

        public override clsMsg validate(clsCmd cmd)
        {
            if (_fnValidate != null)
                return _fnValidate(cmd);
            return g.msg("");
        }

    }
}
