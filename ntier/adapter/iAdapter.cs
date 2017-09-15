using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DAL;

namespace NTier.adapter
{

    public interface iAdapter
    {
        void exec(clsCmd cmd);
        DataTable getData(clsCmd cmd);
        object execScalar(clsCmd cmd);
    }

    

}
