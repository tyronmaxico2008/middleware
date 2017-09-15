using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using DAL;

namespace NTier.Request
{




    internal abstract class clsBussinessTierFromXmlBase : iBussinessTier
    {

        clsAppServerBase _appServerInfo = null;

        adapter.clsDataAdapterBase _adapter = null;
        XmlDocument xDoc;

        private myAssembly oAssembly = new myAssembly();
        public abstract void setCookie(string sKey, string sValue);
        public abstract string getCookie(string sKey);

        
        
        public string getAppSetting(string sKey)
        {
            
            XmlNode node1 = xDoc.SelectSingleNode("//appConfig/appSettings/appSetting[@key='" + sKey + "']");
                
            return node1.getXmlText();
        }

        public string getPath(string sPath)
        {
            return _appServerInfo.getAppResourcePath(sPath);
        }

        public XmlDocument getXmlDoc(string sPath)
        {
            var xDoc = new XmlDocument();
            xDoc.Load(sPath);
            return xDoc;
        }

        public clsBussinessTierFromXmlBase(clsAppServerBase appServerInfo)
        {

            _appServerInfo = appServerInfo;
            //_AppConfigFolderPath = appServerInfo.getAppConfigFolder();


            xDoc = getXmlDoc(appServerInfo.getAppConfigFilePath());

            var xNodeConnection = xDoc.SelectSingleNode("//appConfig/defaultConnectionString");

            string sConnectionString = xNodeConnection.InnerText;
            string sConnectionType = xNodeConnection.Attributes["type"].Value;

            _adapter = adapter.utility.createAdapter(sConnectionType, sConnectionString);

            oAssembly.appServerRootPath = appServerInfo.appServerRootPath;
            oAssembly.appName = appServerInfo.appName;
            oAssembly.loadDll(xDoc);
        }




        /*
        private clsMsg getRequest_fork(string sRequestPath, clsCmd cmd)
        {
            var oRequestPath = new requestPath(sRequestPath);


            var sPath = getPath(oRequestPath.getPath());

            if (!System.IO.File.Exists(sPath))
                return g.msg("Request path not found !");

            ///////////////////////////////////////////////
            string sRequestType = "";
            iRequest request = null;

            var xDoc = getXmlDoc(oRequestPath.getPath());

            var xNodeType = xDoc.SelectSingleNode("//request");

            var attr = xNodeType.Attributes["type"];
            if (attr != null)
                sRequestType = attr.Value;

            switch (sRequestType)
            {
                case "getData":
                    var node = xDoc.SelectSingleNode("//request/dt[@name='" + oRequestPath.getQueryString("name") + "']/view");
                    //request = getRequest(node);
                    break;
            }

            return request.getRequest(cmd);
        }
        */

        
        private iGetData getRequest(XmlNode node)
        {

            var myRequest = new NTier.CRUD.clsReadSimple();
            myRequest.viewName = node.InnerText;
            return myRequest;
        }

        public clsMsg getData(string sPath, clsCmd cmd)
        {
            XmlNode xNode = xDoc.SelectSingleNode("//appConfig/requestData[@type='getData']/dt[@name='" + sPath + "']");

            if (xNode.getXmlAttributeValue("type") == "view")
            {
                string sViewName = xNode.getXmlText("view");

                var obj = new NTier.Request.clsGetDataView(_adapter);
                obj.viewName = sViewName;
                obj.OrderBy = xNode.getXmlText("orderby");

                return g.msg("", obj.getData(cmd));
            }
            else if (xNode.getXmlAttributeValue("type") == "other")
            {
                string sAssemblyName = xNode.getXmlAttributeValue("assemblyName");
                string sClassPath = xNode.getXmlAttributeValue("classPath");
                string sFunc = xNode.getXmlAttributeValue("func");

                NTier.Request.iGetData iCmd = AppDomain.CurrentDomain.CreateInstance(sAssemblyName, sClassPath).Unwrap() as iGetData;
                iCmd.setTier(this);

                if (!sFunc.isEmpty())
                {
                    object[] objParams = new object[] { cmd };
                    var m = iCmd.GetType().GetMethod(sFunc);
                    clsMsg msg = (clsMsg)m.Invoke(iCmd, objParams);
                    return msg;
                }
                else
                    return iCmd.getData(cmd);
            }

            return g.msg("");
        }

        public clsMsg getDropDownData(string sPath
            , clsCmd cmd)
        {
            XmlNode xNode = xDoc.SelectSingleNode("//appConfig/requestData[@type='DropDown']/dt[@name='" + sPath + "']");

            if (xNode.getXmlAttributeValue("type") == "sql")
            {

                string sSQL = xNode.getXmlText("sql");
                cmd.SQL = sqlbuilder.sqlUtility.joinWhereCondition(sSQL, cmd);
                var t = _adapter.getData(cmd);
                //var t = _adapter.getData(sSQL);
                return g.msg("", t);
            }


            return g.msg("");
        }



        public NTier.CRUD.clsCRUD getCRUD(string sCRUDName)
        {

            XmlNode xNode = xDoc.SelectSingleNode("//appConfig/cruds/crud[@name='" + sCRUDName + "']");
            string sTableName = xNode.getXmlAttributeValue("tableName");
            string sViewName = xNode.getXmlAttributeValue("viewName");
            string sPrimaryKeyField = xNode.getXmlAttributeValue("primaryKey");
            bool isIdentity = xNode.getXmlAttributeValue("isIdentity") == "true" ? true : false;
            //
            var oCRUD = new NTier.CRUD.clsCRUD(_adapter, sTableName, sViewName, sPrimaryKeyField, isIdentity);
            return oCRUD;
        }

        private clsMsg validate(XmlNode xNode
            , clsCmd cmd)
        {


            var oValidation = new NTier.Validations.clsValidation();
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


            return oValidation.validate(cmd);



        }

        public NTier.adapter.clsDataAdapterBase getAdapter(string sKey = "")
        {
            return _adapter;
        }

        private class xmlAssembly
        {

            XmlDocument _xDoc = null;
            myAssembly _oAssembly;
            public xmlAssembly(XmlDocument xDoc
                , myAssembly oAssembly
                , string sType
                , string sPath)
            {

                _xDoc = xDoc;
                _oAssembly = oAssembly;

                string xNodePath = string.Format("//appConfig/requestData[@type='{0}']/file[@name='{1}']", sType, sPath);

                //
                XmlNode xNode = xDoc.SelectSingleNode(xNodePath);

                this.AssemblyName = xNode.getXmlAttributeValue("assemblyName");
                this.classPath = xNode.getXmlAttributeValue("classPath");
                this.functionName = xNode.getXmlAttributeValue("func");
            }

            public string AssemblyName { get; set; }
            public string classPath { get; set; }
            public string functionName { get; set; }

            public FileData getFileData(clsCmd cmd
                ,iBussinessTier oTier)
            {
                NTier.Request.iRequest oReq = _oAssembly.createInstance(this.AssemblyName, this.classPath) as iRequest;

                oReq.setTier(oTier);


                object[] objParams = new object[] { cmd };
                var m = oReq.GetType().GetMethod(this.functionName);
                FileData msg = (FileData)m.Invoke(oReq, objParams);
                return msg;
            }

        }

        

        public FileData getFileContent(string sPath, clsCmd cmd)
        {
            var oXmlAssembly = new xmlAssembly(xDoc, oAssembly, "file", sPath);

            return oXmlAssembly.getFileData(cmd, this);
        }

        public clsMsg exec(string sPath, clsCmd cmd)
        {


            XmlNode xNode = xDoc.SelectSingleNode("//appConfig/requestData[@type='cmd']/cmd[@name='" + sPath + "']");
            if (xNode == null)
                throw new Exception("Command path " + sPath + " not found !");

            string sCmdType = xNode.getXmlAttributeValue("type");

            if (sCmdType != "json")
            {
                var msg = validate(xNode, cmd);
                if (msg.Validated == false) return msg;
            }

            //Validation
            if (xNode != null)
            {
                //Declaration
                //string sCmdType = xNode.getXmlAttributeValue("type");
                string sCRUDName = xNode.getXmlAttributeValue("crudName");

                var oCRUD = getCRUD(sCRUDName);

                if (sCmdType == "save")
                {
                    try
                    {
                        return oCRUD.save(cmd);
                    }
                    catch (Exception ex)
                    {
                        return g.msg_exception(ex);
                    }
                }
                else if (sCmdType == "delete")
                {
                    try
                    {
                        oCRUD.delete(cmd);
                        return g.msg("");
                    }
                    catch (Exception ex)
                    {
                        return g.msg_exception(ex);
                    }

                }
                else if (sCmdType == "other")
                {
                    string sAssemblyName = xNode.getXmlAttributeValue("assemblyName");
                    string sClassPath = xNode.getXmlAttributeValue("classPath");
                    string sFunc = xNode.getXmlAttributeValue("func");
                    //NTier.Request.iCommand iCmd = AppDomain.CurrentDomain.CreateInstance(sAssemblyName, sClassPath).Unwrap() as iCommand;
                    NTier.Request.iCommand iCmd = oAssembly.createInstance(sAssemblyName, sClassPath) as iCommand;

                    iCmd.setTier(this);

                    if (!sFunc.isEmpty())
                    {
                        object[] objParams = new object[] { cmd };
                        var m = iCmd.GetType().GetMethod(sFunc);
                        clsMsg msg = (clsMsg)m.Invoke(iCmd, objParams);
                        return msg;
                    }
                    else
                        return iCmd.exec(cmd);
                }
                else if (sCmdType == "cmd")
                {
                    string q = xNode.getXmlText("sql");
                    cmd.SQL = q;
                    try
                    {
                        _adapter.exec(cmd);
                        return g.msg("");
                    }
                    catch (Exception ex)
                    {
                        return g.msg_exception(ex);
                    }
                }

                else if (sCmdType == "json")
                {

                    DataTable t = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(cmd.getStringValue("data"));

                    //validation start
                    int i = 0;
                    foreach (DataRow r in t.Rows)
                    {
                        i++;
                        var cmd2 = new clsCmd();
                        cmd2.AddValues(r);

                        var msg = validate(xNode, cmd2);
                        if (msg.Validated == false)
                        {
                            msg.Message = string.Format(" Row Number : {0} : {1} ", i, msg.Message);
                            return msg;
                        }
                    }

                    //updation start

                    i = 0;
                    foreach (DataRow r in t.Rows)
                    {
                        i++;
                        var cmd2 = new clsCmd();
                        cmd2.AddValues(r);

                        string q = xNode.getXmlText("sql");
                        cmd2.SQL = q;

                        try
                        {
                            _adapter.exec(cmd2);
                            return g.msg("");
                        }
                        catch (Exception ex)
                        {
                            var msg = g.msg_exception(ex);
                            msg.Message = string.Format(" Row Number : {0} : {1} ", i, msg.Message);
                        }

                    }



                }
            }

            return g.msg("Path not found !");
            
        }


        public sqlReport.SQLReportBase getSQLReport(string sPath, clsCmd cmd)
        {
            XmlNode xNode = xDoc.SelectSingleNode("//appConfig/requestData[@type='sqlreport']/sqlreport[@name='" + sPath + "']");
            if (xNode == null)
                throw new Exception("Report Command path " + sPath + " not found !");

            string sRdlPath = xNode.getXmlAttributeValue("rdlPath");

            if (sRdlPath.isEmpty() == false)
            {
                string sRdlFullPath = getPath(sRdlPath);
                var rpt = sqlReportFactory();
                rpt.output = g.ConvertFileToByteArray(sRdlFullPath);
                rpt.ds = getDsForReport(xNode, cmd);
                rpt.downloadName = xNode.getXmlText("downloadName");
                return rpt;
            }
            return null;
        }


        private sqlReport.SQLReportBase sqlReportFactory()
        {
            sqlReport.SQLReportBase rpt;

            string sApplicationType = xDoc.SelectSingleNode("//appConfig").getXmlText("applicationType");

            if (sApplicationType == "win")
            {
                rpt = AppDomain.CurrentDomain.CreateInstance("NTier", "NTier.sqlReport.SQLReportWin").Unwrap() as NTier.sqlReport.SQLReportBase;
            }
            else
            {
                rpt = AppDomain.CurrentDomain.CreateInstance("NTier_Web", "NTier.sqlReport.SQLReportWeb").Unwrap() as NTier.sqlReport.SQLReportBase;
            }
            return rpt;
        }

        private DataSet getDsForReport(XmlNode xNodeParent, clsCmd cmd)
        {
            var ds = new DataSet();
            var dsNodes = xNodeParent.SelectNodes("ds");
            foreach (XmlNode xNode in dsNodes)
            {

                var sType = xNode.getXmlAttributeValue("type");
                var sName = xNode.getXmlAttributeValue("name");
                var sPath = xNode.getXmlAttributeValue("path");
                DataTable t = null;
                switch (sType)
                {
                    case "sql":
                        cmd.SQL = xNode.InnerText;
                        t = _adapter.getData(cmd);
                        break;
                    case "bll":
                        t = this.getData(sPath, cmd).Obj as DataTable;
                        t.TableName = sName;
                        break;
                }
                if (t != null)
                {
                    t.TableName = sName;
                    ds.Tables.Add(t);
                }
            }
            return ds;
        }
    }
}