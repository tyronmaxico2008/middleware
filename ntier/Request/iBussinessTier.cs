using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using DAL;
namespace NTier.Request
{

    public interface iBussinessTier
    {

        string getAppSetting(string sKey);
        void setCookie(string sKey, string sVal);
        string getCookie(string sKey);
        
        NTier.CRUD.clsCRUD getCRUD(string sCRUDName);
        clsMsg getData(string sPath, clsCmd cmd);
        clsMsg getDropDownData(string sPath,clsCmd cmd);
        clsMsg exec(string sPath, clsCmd cmd);
        NTier.adapter.clsDataAdapterBase getAdapter(string sKey = "");
        sqlReport.SQLReportBase getSQLReport(string sPath, clsCmd cmd);
        string getPath(string sPath);
        FileData getFileContent(string sPath, clsCmd cmd);
    }
}
