using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NTier.adapter;

namespace NTier.Request
{
    internal interface iConfig
    {
        string getConnectionString();
        clsDataAdapterBase getAdapter();

    }
}
