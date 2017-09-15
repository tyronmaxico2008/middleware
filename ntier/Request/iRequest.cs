using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using System.Xml;
using NTier.adapter;
using System.Data;
namespace NTier.Request
{

    public interface iRequest
    {
        void setTier(iBussinessTier oTier);
        clsMsg validate(clsCmd cmd);
    }

    public interface iGetData : iRequest
    {
        clsMsg getData(clsCmd cmd);
    }


    public interface iCommand : iRequest
    {
        clsMsg exec(clsCmd cmd);
    }


    public interface iGetSQLReport : iRequest
    {


        sqlReport.iSQLReport getReport(clsCmd cmd);
    }


    public abstract class clsRequest : iRequest
    {

        internal NTier.Validations.clsValidation oValidation = new Validations.clsValidation();

        public abstract clsMsg validate(clsCmd cmd);
        public iBussinessTier _tier;

        public clsRequest(iBussinessTier oTier)
        {
            _tier = oTier;
        }

        public clsRequest()
        {

        }


        public void setTier(iBussinessTier oTier)
        {
            _tier = oTier;
        }



    }

    internal abstract class clsRequestGetDataBase : clsRequest, iGetData
    {
        public abstract clsMsg getData(clsCmd cmd);
    }

    internal abstract class clsRequestCommandBase : clsRequest, iCommand
    {
        public abstract clsMsg exec(clsCmd cmd);
    }

    internal abstract class clsRequestSQLReportBase : clsRequest, iGetSQLReport
    {

        public abstract override clsMsg validate(clsCmd cmd);
        public abstract NTier.sqlReport.SQLReportBase getReport(clsCmd cmd);



        sqlReport.iSQLReport iGetSQLReport.getReport(clsCmd cmd)
        {
            return (sqlReport.iSQLReport)this.getReport(cmd);
        }


    }

    internal class clsRequestGetData_SimpleView : clsRequestGetDataBase
    {

        public string viewName { get; set; }
        public string orderBy { get; set; }
        NTier.adapter.clsDataAdapterBase _adapter;

        public override clsMsg validate(clsCmd cmd)
        {
            return g.msg("");
        }
        public clsRequestGetData_SimpleView(NTier.adapter.clsDataAdapterBase adapter)
        {
            _adapter = adapter;
        }

        public override clsMsg getData(clsCmd cmd)
        {
            var obj = new NTier.Request.clsGetDataView(_adapter);
            obj.viewName = this.viewName;
            obj.OrderBy = this.orderBy;
            return g.msg("", obj.getData(cmd));
        }

    }

    internal class clsRequestGetData_sql : clsRequestGetDataBase
    {
        public string sql { get; set; }
        //public string orderBy { get; set; }
        NTier.adapter.clsDataAdapterBase _adapter;

        public override clsMsg validate(clsCmd cmd)
        {
            return g.msg("");
        }
        public clsRequestGetData_sql(NTier.adapter.clsDataAdapterBase adapter)
        {
            _adapter = adapter;
        }

        public override clsMsg getData(clsCmd cmd)
        {
            cmd.SQL = sqlbuilder.sqlUtility.joinWhereCondition(this.sql, cmd);
            var t = _adapter.getData(cmd);
            //var t = _adapter.getData(sSQL);
            return g.msg("", t);
        }
    }


    internal class clsRequestGetData_FromAssembly : clsRequestGetDataBase
    {


        public string assemblyName { get; set; }
        public string classPath { get; set; }
        public string func { get; set; }

        public string viewName { get; set; }
        public string orderBy { get; set; }
        NTier.adapter.clsDataAdapterBase _adapter;

        public clsRequestGetData_FromAssembly(NTier.adapter.clsDataAdapterBase adapter)
        {
            _adapter = adapter;
        }

        public override clsMsg validate(clsCmd cmd)
        {
            return g.msg();
        }

        public override clsMsg getData(clsCmd cmd)
        {
            var obj = new NTier.Request.clsGetDataView(_adapter);

            NTier.Request.iGetData iCmd = AppDomain.CurrentDomain.CreateInstance(this.assemblyName, this.classPath).Unwrap() as iGetData;
            iCmd.setTier(this._tier);

            if (!this.func.isEmpty())
            {
                object[] objParams = new object[] { cmd };
                var m = iCmd.GetType().GetMethod(this.func);
                clsMsg msg = (clsMsg)m.Invoke(iCmd, objParams);
                return msg;
            }
            else
                return iCmd.getData(cmd);

            return g.msg("", obj.getData(cmd));
        }
    }



    
    internal abstract class clsRequestFileData_Base : clsRequest
    {
        public abstract override clsMsg validate(clsCmd cmd);
        public abstract FileData getFileData(clsCmd cmd);
    }


    internal class clsRequestFileData_Assembly : clsRequestFileData_Base
    {

        public string assemblyName { get; set; }
        public string classPath { get; set; }
        public string func { get; set; }
        myAssembly _Assembly = null;

        public clsRequestFileData_Assembly(myAssembly oAssembly)
        {
            _Assembly = oAssembly;
        }
        public override clsMsg validate(clsCmd cmd)
        {
            return g.msg("");
        }
        public override FileData getFileData(clsCmd cmd)
        {
            NTier.Request.iRequest oReq = _Assembly.createInstance(this.assemblyName, this.classPath) as iRequest;
            oReq.setTier(this._tier);

            object[] objParams = new object[] { cmd };
            var m = oReq.GetType().GetMethod(this.func);
            FileData msg = (FileData)m.Invoke(oReq, objParams);
            return msg;
        }
    }


    internal class clsRequestCommand_save : clsRequestCommandBase
    {

        public string crudName { get; set; }


        public override clsMsg validate(clsCmd cmd)
        {
            return oValidation.validate(cmd);
            //throw new NotImplementedException();
        }

        public override clsMsg exec(clsCmd cmd)
        {

            var msg = validate(cmd);
            if (msg.Validated == false) return msg;


            var oCRUD = _tier.getCRUD(this.crudName);

            try
            {
                return oCRUD.save(cmd);
            }
            catch (Exception ex)
            {
                return g.msg_exception(ex);
            }

        }


    }

    internal class clsRequestCommand_delete : clsRequestCommandBase
    {

        public string crudName { get; set; }


        public override clsMsg validate(clsCmd cmd)
        {
            return oValidation.validate(cmd);
            //throw new NotImplementedException();
        }

        public override clsMsg exec(clsCmd cmd)
        {

            var msg = validate(cmd);
            if (msg.Validated == false) return msg;


            var oCRUD = _tier.getCRUD(this.crudName);

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
    }


    internal class clsRequestCommand_other : clsRequestCommandBase
    {

        public string crudName { get; set; }
        public string AssemblyName { get; set; }
        public string classPath { get; set; }
        public string func { get; set; }

        private myAssembly _assembly;
        public clsRequestCommand_other(myAssembly oAssembly)
        {
            _assembly = oAssembly;
        }
        public override clsMsg validate(clsCmd cmd)
        {
            return oValidation.validate(cmd);
            //throw new NotImplementedException();
        }

        public override clsMsg exec(clsCmd cmd)
        {

            var msg = validate(cmd);
            if (msg.Validated == false) return msg;


            NTier.Request.iCommand iCmd = _assembly.createInstance(this.AssemblyName, this.classPath) as iCommand;

            iCmd.setTier(_tier);

            if (!func.isEmpty())
            {
                object[] objParams = new object[] { cmd };
                var m = iCmd.GetType().GetMethod(func);
                msg = (clsMsg)m.Invoke(iCmd, objParams);
                return msg;
            }
            else
                return iCmd.exec(cmd);
        }


    }

    internal class clsRequestCommand_sql : clsRequestCommandBase
    {

        public string sql { get; set; }


        public override clsMsg validate(clsCmd cmd)
        {
            return oValidation.validate(cmd);
            //throw new NotImplementedException();
        }

        public override clsMsg exec(clsCmd cmd)
        {
            var msg = validate(cmd);
            if (msg.Validated == false) return msg;

            cmd.SQL = sql;
            try
            {

                _tier.getAdapter().exec(cmd);
                return g.msg("");
            }
            catch (Exception ex)
            {
                return g.msg_exception(ex);
            }

        }
    }


    internal abstract class sqlReportTableBase : clsRequestGetDataBase
    {
        public string name { get; set; }

    }


    internal class sqlReportTableSQL : sqlReportTableBase
    {


        public override clsMsg validate(clsCmd cmd)
        {
            return oValidation.validate(cmd);
        }

        public string sql { get; set; }

        public override clsMsg getData(clsCmd cmd)
        {
            cmd.SQL = sql;
            DataTable t = _tier.getAdapter().getData(cmd);
            t.TableName = this.name;
            return g.msg("", t);
        }
    }

    internal class sqlReportTableBll : sqlReportTableBase
    {

        public override clsMsg validate(clsCmd cmd)
        {
            throw new NotImplementedException();
        }
        public string path { get; set; }

        public override clsMsg getData(clsCmd cmd)
        {
            DataTable t = _tier.getData(path, cmd).Obj as DataTable;
            t.TableName = this.name;
            return g.msg("", t);
        }
    }

    internal class sqlReportDs : List<sqlReportTableBase>
    {

        public DataSet getDs(clsCmd cmd)
        {

            var ds = new DataSet();

            foreach (var f in this)
            {
                var result = f.getData(cmd);
                ds.Tables.Add(result.Obj as DataTable);
            }
            return ds;
        }

    }

    internal class clsRequest_sqlReport : clsRequestSQLReportBase
    {

        public override clsMsg validate(clsCmd cmd)
        {
            return oValidation.validate(cmd);
        }

        public sqlReportDs ds = new sqlReportDs();
        public string rdlPath { get; set; }
        public string downloadName { get; set; }


        public override NTier.sqlReport.SQLReportBase getReport(clsCmd cmd)
        {
            if (rdlPath.isEmpty() == false)
            {
                string sRdlFullPath = _tier.getPath(rdlPath);
                var rpt = AppDomain.CurrentDomain.CreateInstance("NTier_Web", "NTier.sqlReport.SQLReportWeb").Unwrap() as NTier.sqlReport.SQLReportBase;
                rpt.output = g.ConvertFileToByteArray(sRdlFullPath);
                rpt.ds = ds.getDs(cmd);
                rpt.downloadName = this.downloadName;
                return rpt;
            }
            
            return null;

        }
    }

}
