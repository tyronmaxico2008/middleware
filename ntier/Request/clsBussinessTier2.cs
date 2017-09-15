using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Xml;
using System.Data;
namespace NTier.Request
{



    public class clsBussinessTier2 : iBussinessTier
    {



        private myAssembly oAssembly = new myAssembly();

        clsAppServerBase _appServerInfo = null;
        NameValueCollection clnAppSettings = new NameValueCollection();
        adapter.clsDataAdapterBase _adapter = null;

        Dictionary<string, NTier.CRUD.clsCRUD> clnCRUDs = new Dictionary<string, CRUD.clsCRUD>();
        Dictionary<string, NTier.Request.clsRequestGetDataBase> clnGetData = new Dictionary<string, clsRequestGetDataBase>();
        Dictionary<string, NTier.Request.clsRequestGetDataBase> clnDropDown = new Dictionary<string, clsRequestGetDataBase>();
        Dictionary<string, NTier.Request.clsRequestCommandBase> clnCmd = new Dictionary<string, clsRequestCommandBase>();
        Dictionary<string, NTier.Request.clsRequestSQLReportBase> clnSQLReport = new Dictionary<string, clsRequestSQLReportBase>();
        Dictionary<string, NTier.Request.clsRequestFileData_Base> clnFileData = new Dictionary<string, clsRequestFileData_Base>();

        public clsBussinessTier2(clsAppServerBase appServerInfo
            , string sMainApp)
        {
            _appServerInfo = appServerInfo;

            var xDocParent = new XmlDocument();
            var xDoc = new XmlDocument();
            string sPath = _appServerInfo.getAppConfigFilePath();

            if (System.IO.File.Exists(sPath))
                xDoc.Load(_appServerInfo.getAppConfigFilePath());

            sPath = _appServerInfo.appServerRootPath + "\\" + sMainApp + ".xml";
            if (System.IO.File.Exists(sPath))
                xDocParent.Load(sPath);

            fillAppSettings(xDoc);
            fillAppSettings(xDocParent);

            setAdapter(xDoc);
            setAdapter(xDocParent);


            setCRUD(xDoc);
            setCRUD(xDocParent);

            setGetData(xDoc);
            setGetData(xDocParent);

            setCMD(xDoc);
            setCMD(xDocParent);

            setSQLReport(xDoc);
            setSQLReport(xDocParent);

            setFileData(xDoc);
            setFileData(xDocParent);



            //testing
            /*
            foreach(string skey in clnAppSettings.AllKeys)
            {
                Console.Write("{0} : {1} \n", skey, clnAppSettings[skey]);
            }
             */

        }

        private void setAdapter(XmlDocument xDoc)
        {

            if (xDoc == null) return;

            if (_adapter != null) return;


            var xNodeConnection = xDoc.SelectSingleNode("//appConfig/defaultConnectionString");
            if (xNodeConnection == null) return;

            string sConnectionString = xNodeConnection.InnerText;
            string sConnectionType = xNodeConnection.getXmlAttributeValue("type");

            _adapter = adapter.utility.createAdapter(sConnectionType, sConnectionString);

            /*
            oAssembly.appServerRootPath = appServerInfo.appServerRootPath;
            oAssembly.appName = appServerInfo.appName;
            oAssembly.loadDll(xDoc);
             */


        }

        private void fillAppSettings(XmlDocument xDoc)
        {

            //checking if xdoc is not available then return
            if (xDoc == null) return;

            XmlNodeList _nodeList = xDoc.SelectNodes("//appConfig/appSettings/appSetting");

            //if appsettings node not found, return;
            if (_nodeList == null) return;


            foreach (XmlNode node in _nodeList)
            {
                string sKey = node.getXmlAttributeValue("key");
                if (clnAppSettings.AllKeys.Contains(sKey) == false)
                {
                    clnAppSettings.Set(sKey, node.InnerText);
                }
            }

        }

        public void setCRUD(XmlDocument xDoc)
        {

            if (xDoc == null) return;

            XmlNodeList xNodeList = xDoc.SelectNodes("//appConfig/cruds/crud");
            if (xNodeList == null) return;

            foreach (XmlNode xNode in xNodeList)
            {

                string sKey = xNode.getXmlAttributeValue("name");
                string sTableName = xNode.getXmlAttributeValue("tableName");
                string sViewName = xNode.getXmlAttributeValue("viewName");
                string sPrimaryKeyField = xNode.getXmlAttributeValue("primaryKey");
                bool isIdentity = xNode.getXmlAttributeValue("isIdentity") == "true" ? true : false;
                //
                if (!clnCRUDs.ContainsKey(sKey))
                {
                    var oCRUD = new NTier.CRUD.clsCRUD(_adapter, sTableName, sViewName, sPrimaryKeyField, isIdentity);

                    clnCRUDs.Add(sKey, oCRUD);
                }
                //return oCRUD;
            }
        }

        private void setGetData(XmlDocument xDoc)
        {
            if (xDoc == null) return;
            XmlNodeList xNodeList = xDoc.SelectNodes("//appConfig/requestData[@type='getData']/dt");
            if (xNodeList == null) return;

            foreach (XmlNode xNode in xNodeList)
            {
                tmp(xNode, clnGetData);
            }
        }

        private void setDropDown(XmlDocument xDoc)
        {
            if (xDoc == null) return;
            XmlNodeList xNodeList = xDoc.SelectNodes("//appConfig/requestData[@type='DropDown']/dt");
            if (xNodeList == null) return;

            foreach (XmlNode xNode in xNodeList)
            {
                tmp(xNode, clnDropDown);
            }
        }



        private void tmp(XmlNode xNode,Dictionary<string, NTier.Request.clsRequestGetDataBase> cln )
        {
            string sType = xNode.getXmlAttributeValue("type");
            string sKey = xNode.getXmlAttributeValue("name");

            if (cln.ContainsKey(sKey)) return;

            switch (sType)
            {
                case "view":
                    string sViewName = xNode.getXmlText("view");

                    var obj = new NTier.Request.clsRequestGetData_SimpleView(_adapter);
                    obj.viewName = sViewName;
                    obj.orderBy = xNode.getXmlText("orderby");
                    obj.setTier(this);

                    clnGetData.Add(sKey, obj);

                    break;

                case "other":

                    string sAssemblyName = xNode.getXmlAttributeValue("assemblyName");
                    string sClassPath = xNode.getXmlAttributeValue("classPath");
                    string sFunc = xNode.getXmlAttributeValue("func");

                    var objOther = new NTier.Request.clsRequestGetData_FromAssembly(_adapter);
                    objOther.assemblyName = sAssemblyName;
                    objOther.classPath = sClassPath;
                    objOther.func = sFunc;
                    objOther.setTier(this);

                    clnGetData.Add(sKey, objOther);
                    break;
                case "sql":
                    string sSQL = xNode.getXmlText("sql");

                    var objSQL = new NTier.Request.clsRequestGetData_sql(_adapter);
                    objSQL.sql = sSQL;
                    objSQL.setTier(this);
                    clnGetData.Add(sKey, objSQL);
                    break;
            }
        }

        private void setCMD(XmlDocument xDoc)
        {
            if (xDoc == null) return;

            XmlNodeList xNodeList = xDoc.SelectNodes("//appConfig/requestData[@type='cmd']/cmd");
            foreach (XmlNode xNode in xNodeList)
            {

                string skey = xNode.getXmlAttributeValue("name");
                if (clnCmd.ContainsKey(skey)) continue;

                string sType = xNode.getXmlAttributeValue("type");
                string sCRUDName = xNode.getXmlAttributeValue("crudName");



                switch (sType)
                {
                    case "save":

                        var objSave = new clsRequestCommand_save();
                        objSave.crudName = sCRUDName;
                        setValidationFrom_Node(xNode, objSave.oValidation);
                        objSave.setTier(this);

                        clnCmd.Add(skey, objSave);
                        break;

                    case "delete":


                        var objDelete = new clsRequestCommand_save();
                        objDelete.crudName = sCRUDName;
                        setValidationFrom_Node(xNode, objDelete.oValidation);
                        objDelete.setTier(this);

                        clnCmd.Add(skey, objDelete);
                        break;
                    case "other":
                        string sAssemblyName = xNode.getXmlAttributeValue("assemblyName");
                        string sClassPath = xNode.getXmlAttributeValue("classPath");
                        string sFunc = xNode.getXmlAttributeValue("func");


                        var objother = new clsRequestCommand_other(oAssembly);
                        objother.AssemblyName = sAssemblyName;
                        objother.classPath = sClassPath;
                        objother.func = sFunc;

                        setValidationFrom_Node(xNode, objother.oValidation);

                        objother.setTier(this);

                        clnCmd.Add(skey, objother);
                        break;

                    case "cmd":
                        string sSQL = xNode.getXmlAttributeValue("assemblyName");
                        var objSQL = new clsRequestCommand_sql();
                        objSQL.sql = sSQL;
                        setValidationFrom_Node(xNode, objSQL.oValidation);
                        objSQL.setTier(this);

                        clnCmd.Add(skey, objSQL);
                        break;
                }
            }
        }

        
        private void setSQLReport(XmlDocument xDoc)
        {
            if (xDoc == null) return;

            XmlNodeList xNodeList = xDoc.SelectNodes("//appConfig/requestData[@type='sqlreport']/sqlreport");

            foreach (XmlNode xNode in xNodeList)
            {

                string sName = xNode.getXmlAttributeValue("name");
                string sRdlPath = xNode.getXmlAttributeValue("rdlPath");
                string downloadName = xNode.getXmlText("downloadName");

                var osqlReport = new clsRequest_sqlReport();
                osqlReport.setTier(this);
                setValidationFrom_Node(xNode, osqlReport.oValidation);
                setSQLReportDs(osqlReport, xNode);
                osqlReport.rdlPath = sRdlPath;
                osqlReport.downloadName = downloadName;


                clnSQLReport.Add(sName, osqlReport);


            }


        }


        private void setFileData(XmlDocument xDoc)
        {
            if (xDoc == null) return;

            XmlNodeList xNodeList = xDoc.SelectNodes("//appConfig/requestData[@type='file']/file");
            if (xNodeList == null) return;
            foreach (XmlNode xNode in xNodeList)
            {

                string sName = xNode.getXmlAttributeValue("name");
                string sAssemblyName = xNode.getXmlAttributeValue("assemblyName");
                string sClassPath = xNode.getXmlAttributeValue("classPath");
                string sFunc = xNode.getXmlAttributeValue("func");

                var obj = new clsRequestFileData_Assembly(oAssembly);
                obj.setTier(this);

                obj.assemblyName = sAssemblyName;
                obj.classPath = sClassPath;
                obj.func = sFunc;
                clnFileData.Add(sName, obj);

            }


        }


        private void setSQLReportDs(clsRequest_sqlReport oSQLReport
            , XmlNode xNodeParent)
        {

            var dsNodes = xNodeParent.SelectNodes("ds");

            foreach (XmlNode xNode in dsNodes)
            {

                var sType = xNode.getXmlAttributeValue("type");
                var sName = xNode.getXmlAttributeValue("name");
                var sPath = xNode.getXmlAttributeValue("path");

                DataTable t = null;
                sqlReportTableBase oSQLReportTbl = null;
                switch (sType)
                {
                    case "sql":
                        var objTbl_sql = new sqlReportTableSQL();
                        objTbl_sql.setTier(this);
                        objTbl_sql.name = sName;
                        objTbl_sql.sql = xNode.InnerText;

                        oSQLReport.ds.Add(objTbl_sql);
                        break;
                    case "bll":
                        var objTbl_bll = new sqlReportTableBll();
                        objTbl_bll.setTier(this);
                        objTbl_bll.name = sName;
                        objTbl_bll.path = sPath;
                        oSQLReport.ds.Add(objTbl_bll);
                        break;
                }


            }
        }




        private void setValidationFrom_Node(XmlNode xNode, NTier.Validations.clsValidation oValidation)
        {
            //var oValidation = new NTier.Validations.clsValidation();
            XmlNodeList xNodeValidations = xNode.SelectNodes("validations/validate");

            foreach (XmlNode xnodeValidate in xNodeValidations)
            {
                string sType = xnodeValidate.getXmlAttributeValue("type");

                string sFieldName = xnodeValidate.getXmlAttributeValue("fieldName");
                string sFieldTitle = xnodeValidate.getXmlAttributeValue("fieldTitle");
                bool required = xnodeValidate.getXmlAttributeValue("required") == "true" ? true : false;

                switch (sType)
                {
                    case "basic":
                        int iMaxLength = g.parseInt(xnodeValidate.getXmlAttributeValue("maxLength"));
                        oValidation.addTextField(sFieldName, sFieldTitle, iMaxLength);
                        break;
                    case "unique":
                        string sTableName = xnodeValidate.getXmlAttributeValue("tableName");
                        string sPrimaryKey = xnodeValidate.getXmlAttributeValue("primaryKey");
                        //Validation delete 
                        oValidation.addDuplicate(_adapter, sTableName, sPrimaryKey, sFieldName, sFieldTitle, required);
                        break;
                    case "drp":
                        oValidation.addDropDownField(sFieldName, sFieldTitle);
                        break;
                    case "email":
                        oValidation.addEmailField(sFieldName, sFieldTitle, required);
                        break;
                    case "numeric":
                        oValidation.addNumberField(sFieldName, sFieldTitle, required);
                        break;
                    case "check":
                        string checkContraintValues = xnodeValidate.getXmlAttributeValue("values");
                        oValidation.addCheckConstraint(sFieldName, sFieldTitle, checkContraintValues);
                        break;
                }

            }
            //return oValidation.validate(cmd);
        }

        public string getAppSetting(string sKey)
        {
            return clnAppSettings[sKey];
        }

        public void setCookie(string sKey, string sVal)
        {
            throw new NotImplementedException();
        }

        public string getCookie(string sKey)
        {
            throw new NotImplementedException();
        }

        public CRUD.clsCRUD getCRUD(string sCRUDName)
        {
            return clnCRUDs[sCRUDName];
        }

        public clsMsg getData(string sPath, clsCmd cmd)
        {
            if (!clnGetData.ContainsKey(sPath))
            {
                throw new Exception(string.Format("Path key [{0}]not found !", sPath));
            }

            var obj = clnGetData[sPath];
            return obj.getData(cmd);


        }

        public clsMsg getDropDownData(string sPath, clsCmd cmd)
        {
            if (!clnGetData.ContainsKey(sPath))
            {
                throw new Exception(string.Format("Path key [{0}]not found !", sPath));
            }

            var obj = clnGetData[sPath];
            return obj.getData(cmd);
        }

        public clsMsg exec(string sPath, clsCmd cmd)
        {
            if (!clnCmd.ContainsKey(sPath))
            {
                throw new Exception(string.Format("Path key [{0}]not found !", sPath));
            }

            return clnCmd[sPath].exec(cmd);

        }

        public adapter.clsDataAdapterBase getAdapter(string sKey = "")
        {
            return _adapter;
        }

        public NTier.sqlReport.SQLReportBase getSQLReport(string sPath, clsCmd cmd)
        {
            if (!clnSQLReport.ContainsKey(sPath))
            {
                throw new Exception(string.Format("Path key [{0}] not found !", sPath));
            }

            return clnSQLReport[sPath].getReport(cmd);

        }

        public string getPath(string sPath)
        {
            return _appServerInfo.getAppResourcePath(sPath);
        }

        public FileData getFileContent(string sPath, clsCmd cmd)
        {
            if (!clnFileData.ContainsKey(sPath))
            {
                throw new Exception(string.Format("Path key [{0}] not found !", sPath));
            }

            return clnFileData[sPath].getFileData(cmd);
        }
    }
}
