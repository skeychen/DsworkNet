using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Text;

namespace Builder
{
	public class OracleModel:BaseModel, IDbModel
	{
		private string connectUrl = "";
		public string ConnectUrl
		{
			get
			{
				return connectUrl;
			}
			set
			{
				connectUrl = value;
			}
		}

		private string sqlPk = "select COLUMN_NAME from USER_CONSTRAINTS c, USER_CONS_COLUMNS col where c.CONSTRAINT_NAME=col.CONSTRAINT_NAME and c.CONSTRAINT_TYPE='P'and c.TABLE_NAME='{0}'";
		private string sqlTableComment = "select * from USER_TAB_COMMENTS  where TABLE_TYPE='TABLE' and TABLE_NAME ='{0}'";
		private string sqlColumn = "select A.COLUMN_NAME NAME, A.DATA_TYPE TYPENAME, A.DATA_LENGTH LENGTH, A.DATA_PRECISION PRECISION, A.DATA_SCALE SCALE, A.DATA_DEFAULT, B.COMMENTS DESCRIPTION from USER_TAB_COLUMNS A, USER_COL_COMMENTS B where A.COLUMN_NAME=B.COLUMN_NAME and A.TABLE_NAME=B.TABLE_NAME and A.TABLE_NAME='{0}'";
		private string sqlAllTables = "select TABLE_NAME from USER_TABLES where STATUS='VALID'";

		private List<string> GetPk(string tableName)
		{
			List<string> list = new List<string>();
			string sql = string.Format(sqlPk, tableName);
			DataSet ds = this.GetData(sql);
			DataTable dt = ds.Tables[0];

			foreach(DataRow dr in dt.Rows)
			{
				list.Add(dr["COLUMN_NAME"].ToString());
			}
			return list;
		}

		private List<ColumnModel> GetColumn(string tableName, List<string> pkList)
		{
			List<ColumnModel> list = new List<ColumnModel>();
			string sql = string.Format(sqlColumn, tableName);
			DataSet ds = this.GetData(sql);
			DataTable dt = ds.Tables[0];
			foreach(DataRow dr in dt.Rows)
			{
				string name = dr["NAME"].ToString();
				string typename = dr["TYPENAME"].ToString();
				int length = int.Parse(dr["LENGTH"].ToString());

				int precision = 0;
				object objPrecision = dr["PRECISION"];
				if(objPrecision != null && objPrecision != DBNull.Value)
					precision = int.Parse(objPrecision.ToString());
				int scale = 0;
				object objScale = dr["SCALE"];
				if(objScale != null && objScale != DBNull.Value)
					scale = int.Parse(objScale.ToString());
				object objDesc = dr["DESCRIPTION"];
				string description = name;
				if(objDesc != null && objDesc != DBNull.Value)
					description = objDesc.ToString();

				ColumnModel colModel = GetColumnModel(name, description, typename,
					precision, scale, length, 0, pkList);
				list.Add(colModel);
			}
			return list;

		}
		private string GetTableComment(string tableName)
		{
			string tableComment = tableName;
			string sql = string.Format(sqlTableComment, tableName);
			DataSet ds = this.GetData(sql);
			DataTable dtComment = ds.Tables[0];
			if(dtComment.Rows.Count > 0)
			{
				DataRow drComment = dtComment.Rows[0];
				tableComment = drComment["COMMENTS"].ToString();
			}
			return tableComment;
		}
		private ColumnModel GetColumnModel(String columnName, String comment, string typeName, int precision, int digit, int length, int autoGen, List<string> pkList)
		{
			ColumnModel model = new ColumnModel();

			model.ColumnName = columnName.ToUpper();
			model.Comment = comment;
			//设定主键
			if(pkList.Contains(columnName))
			{
				model.IsPK = true;
			}
			model.Length = length;
			model.Precision = precision;
			model.Scale = digit;
			model.AutoGen = autoGen;
			model.ColType = this.STRING;
			//ORACLE START

			//数字类型处理
			if(typeName.Equals("NUMBER"))
			{
				if(digit > 0)
				{
					model.ColType = this.FLOAT;
                }
                else if (precision > 10)
                {
                    model.ColType = this.LONG;
                }
				else
				{
					model.ColType = this.INT;
				}
			}
			//浮点类型
			if(typeName.Equals("FLOAT"))
			{
				model.ColType = this.FLOAT;
				//model.ColDbType = "FLOAT";
			}
			//char类型
			if(typeName.Equals("VARCHAR2") || typeName.Equals("NCHAR") || typeName.Equals("CHAR") || typeName.Equals("NVARCHAR2"))
			{
				model.ColType = this.STRING;
				//model.ColDbType = typeName;
			}

			if(typeName.Equals("CLOB") || typeName.Equals("NCLOB"))
			{
				model.ColType = this.STRING;
				//model.ColDbType = typeName;

			}
			//日期类型处理
			if(typeName.Equals("DATE"))
			{
				model.ColType = this.DATE;
				//model.ColDbType = typeName;
			}

			//ORACLE END
			model.ColDbType = typeName;
			return model;
		}

		public TableModel GetByTable(string tableName)
		{
			DbHelper.ConnectionURL = connectUrl;
			TableModel tableModel = new TableModel();
			tableModel.TableName = tableName.ToUpper();
			string strComment = GetTableComment(tableName);
			tableModel.TableComment = strComment;
			List<string> pkList = GetPk(tableName);
			List<ColumnModel> colList = GetColumn(tableName, pkList);
			tableModel.ColumnList = colList;
			return tableModel;

		}

		//public ilist<string> getalltable()
		//{
		//    dbhelper.connectionurl = connecturl;
		//    ilist<string> list = new list<string>();
		//    dataset ds = dbhelper.getdsbysql(sqlalltables);
		//    datatable dt = ds.tables[0];

		//    foreach(datarow dr in dt.rows)
		//    {
		//        list.add(dr["table_name"].tostring());
		//    }
		//    return list;
		//}

		private DataSet GetData(string sql)
		{
			DataSet dataSet = new DataSet();
			try
			{
				OracleDataAdapter adapter = new OracleDataAdapter(sql, this.ConnectUrl);
				adapter.Fill(dataSet);
				return dataSet;
			}
			catch
			{
				return DbHelper.GetDsBySql(sql);
			}
		}
	}
}
