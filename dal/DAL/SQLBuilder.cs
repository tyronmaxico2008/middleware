using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL
{


	public class SQLBuilder
	{
		public string SQL { get; set; }
		public string TableName { get; set; }
		private string _ViewName = "";
		public string ViewName
		{
			get
			{
				return _ViewName == "" ? TableName : _ViewName;
			}

			set
			{
				_ViewName = value;
			}
		}
		public string PrimaryKey { get; set; }
		public string Where { get; set; }
		public string OrderBy { get; set; }

		public List<string> DisplayColumns = new List<string>();


		//public string getCreateScript(string sTableName)
		//{
		//    StringBuilder sb1 = new StringBuilder();

		//    sb1.AppendFormat("create table {0}", sTableName);
		//    sb1.AppendLine("(");

		//    int i = 0;
		//    foreach (var f in Fields)
		//    {
		//        sb1.AppendFormat("{0} {1}", f.DataField, f.DataType);

		//    }

		//    sb1.AppendLine(")");


		//    return sb1.ToString();
		//}


		public string getInsertScript(bool isIdentity, string[] Columns)
		{

			var cols = new List<string>();

			//if (!isIdentity) cols.Add(PrimaryKey);

			cols.AddRange(Columns);

			StringBuilder sb1 = new StringBuilder();

			sb1.AppendFormat("INSERT INTO {0} \n", TableName);
			sb1.AppendLine("(");

			int i = 0;

			foreach (string s in cols)
			{


                if (isIdentity && s.ToLower() == PrimaryKey.ToLower()) continue;

				if (i > 0) sb1.Append(",");
				sb1.Append(s);
				sb1.AppendLine();


				i++;
			}

			sb1.AppendLine(")");
			sb1.AppendLine(" VALUES( ");
			i = 0;
			foreach (var s in cols)
			{
                if (isIdentity && s.ToLower() == PrimaryKey.ToLower()) continue;

				if (i > 0) sb1.Append(",");
				sb1.AppendFormat("@{0}", s);
				sb1.AppendLine();
				i++;
			}
			sb1.AppendLine(" )");

			if (isIdentity) sb1.AppendLine("select Scope_identity()");

			return sb1.ToString();
		}


		public string getUpdateScript(params string[] columns)
		{
			StringBuilder sb1 = new StringBuilder();

			sb1.AppendFormat("update {0}  SET\n", TableName);

			int i = 0;
			foreach (var s in columns)
			{
				if (s.ToLower() != PrimaryKey.ToLower())
				{
					if (i > 0) sb1.Append(",");
					sb1.AppendFormat("{0}       =       @{0}", s);
					sb1.AppendLine();
					i++;
				}
			}
			sb1.AppendFormat("\n WHERE {0} = @{0}", PrimaryKey);
			return sb1.ToString();
		}

		public string getSelectScript(string[] FilterColumns)
		{
			StringBuilder sb1 = new StringBuilder();

			sb1.AppendFormat("SELECT * FROM {0} ");

			return sb1.ToString();
		}


		public string getSelectAllScript(string sWhere)
		{
			return string.Format("SELECT * FROM {0}  WHERE {1}", ViewName, sWhere);
		}

		public string getSelectAllScript()
		{
			return string.Format("SELECT * FROM {0} ", ViewName);

		}


		private void addWhereCondition(StringBuilder sb1, clsCmd prms)
		{
			
			if(prms.Count == 0 ) return;
			
			List<string> lstFilters = new List<string>();

			foreach (var f in prms)
			{
				lstFilters.Add(string.Format("{0} = @{0}", f.Name));
			}

			sb1.Append(" WHERE ");
			sb1.Append(string.Join(" AND ", lstFilters.ToArray()));

		}

		public string getSelectAllScript(DAL.clsCmd prm)
		{
			StringBuilder sb1 = new StringBuilder();
			sb1.Append(getSelectAllScript());
			addWhereCondition(sb1, prm);
			return sb1.ToString();
		}


		public string getSelectScript()
		{
			return string.Format("SELECT * FROM {0} WHERE {1} = @{1}", TableName, PrimaryKey);
		}

		public string getSelectScript(int iID)
		{
			return string.Format("SELECT * FROM {0} WHERE {1} = {2}", TableName, PrimaryKey, iID);
		}

		public string getDeleteScript(int iid)
		{
			return string.Format("DELETE FROM {0} WHERE {1} = {2}", TableName, PrimaryKey, iid);
		}

        public static string joinWhereCondition(string sSelectSQL, clsCmd cmd, params string[] sIgnoreFields)
        {
            string sWhere = getWhereCondition(cmd, sIgnoreFields);

            if (string.IsNullOrWhiteSpace(sWhere))
                return sSelectSQL;
            else
                if (sSelectSQL.Contains("where", "WHERE"))
                    return sSelectSQL + "  AND " + sWhere;
                else
                    return sSelectSQL + " WHERE " + sWhere;
        }



        public static string getWhereCondition(clsCmd prms, params string[] sIgnoreFields)
        {
            if (prms == null) return "";
            List<string> lst = new List<string>();

            foreach (var f in prms)
            {
                if (f.Name.Contains(sIgnoreFields) == false  && f.Name.isEmpty() == false && !prms.getStringValue(f.Name).isEmpty())
                {
                    string sOperator = string.IsNullOrWhiteSpace(f.Operator) ? "=" : f.Operator;
                    string sVal = f.Name;
                    sOperator = sOperator.Replace("_", " ");

                    switch (sOperator.ToUpper())
                    {
                        case "NOT LIKE":
                        case "LIKE":
                            sVal = "'%' + @" + f.Name + " + '%'";
                            break;
                        case "BETWEEN":
                            sVal = string.Format(" @{0} AND @{0}2 ", f.Name);
                            break;
                        default:
                            sVal = "@" + f.Name;
                            break;
                    }

                    string sField = string.IsNullOrWhiteSpace(f.TableName) ? f.Name : f.TableName + "." + f.Name;
                    lst.Add(string.Format(" ( {0} {1} {2})", sField, sOperator, sVal));
                }
            }

            return string.Join(" AND ", lst.ToArray());
        }



        public static string getWhereCondition(string sFieldName, string sOperator)
        {

            string sVal = "";

            switch (sOperator.ToUpper())
            {
                case "NOT LIKE":
                case "LIKE":
                    sVal = "'%' + @" + sFieldName + " + '%'";
                    break;
                case "BETWEEN":
                    sVal = string.Format(" @{0} AND @{0}2 ");
                    break;
                default:
                    sVal = "@" + sFieldName;
                    break;
            }

            return string.Format(" ( {0} {1} {2})", sFieldName, sOperator, sVal);
        }


		public string getDeleteScript(clsCmd prms)
		{
			StringBuilder sb1 = new StringBuilder();
			sb1.AppendFormat("DELETE FROM {0} ", this.TableName);

			var sWhere = getWhereCondition(prms);
			if (sWhere != "") sb1.AppendFormat(" WHERE {0}", sWhere);

			return sb1.ToString();
		}

		public string getDeleteScript(string FieldName)
		{
			return string.Format("SELECT * FROM {0} WHERE {1} = @{1}", TableName, FieldName);
		}


		public string getDeleteScript()
		{
			return string.Format("SELECT * FROM {0} WHERE {1} = @{1}", TableName, PrimaryKey);
		}

		public string MaxIDScript()
		{
			return string.Format("SELECT MAX({1}) FROM [{0}] ", TableName, PrimaryKey);
		}




		public string getSearchSQL()
		{
			if (DisplayColumns.Count > 0)
				return string.Format("SELECT {0},{1} FROM {2} ", string.Join(",", DisplayColumns.ToArray()), PrimaryKey, TableName);
			else
				return "";
		}

		public string FilterEqualManyValues(string sField, string[] Values)
		{
			StringBuilder sb1 = new StringBuilder();
			List<string> lst = new List<string>();

			foreach (var s in Values) lst.Add(string.Format("{0} = '{1}'", sField, s));

			return string.Join(" Or ", lst.ToArray());

		}
    }
}
