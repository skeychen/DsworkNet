using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace Builder
{
	public class MySqlModel:BaseModel, IDbModel
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

		private string sqlPk = "select COLUMN_NAME from information_schema.COLUMNS where table_schema='{0}' AND table_name='{1}' and column_key='PRI';";
		private string sqlTableComment = "select table_name,table_comment  from information_schema.TABLES T where table_schema='{0}' AND table_name='{1}' ";
		private string sqlColumn = "select COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION,NUMERIC_SCALE,COLUMN_COMMENT,EXTRA from information_schema.COLUMNS where table_schema='{0}' AND table_name='{1}'";
		private string sqlAllTables = "select table_name from information_schema.TABLES T where table_schema='{0}'";

		private List<string> GetPk(string dbName, string tableName)
		{
			List<string> list = new List<string>();
			string sql = string.Format(sqlPk, dbName, tableName);
			DataTable dt = this.GetData(sql);
			foreach(DataRow dr in dt.Rows)
			{
				list.Add(dr["COLUMN_NAME"].ToString());
			}
			return list;
		}

		private List<ColumnModel> GetColumn(string dbName, string tableName, List<string> pkList)
		{
			List<ColumnModel> list = new List<ColumnModel>();
			string sql = string.Format(sqlColumn, dbName, tableName);
			DataTable dt = this.GetData(sql);
			foreach(DataRow dr in dt.Rows)
			{
				string name = dr["COLUMN_NAME"].ToString();
				string typeName = dr["DATA_TYPE"].ToString();
				int length = 0;
				object obj2 = dr["CHARACTER_MAXIMUM_LENGTH"];
				if(!(((obj2 == null) || (obj2 == DBNull.Value)) || typeName.Equals("longtext")))
				{
					length = int.Parse(obj2.ToString());
				}
				int precision = 0;
				object obj3 = dr["NUMERIC_PRECISION"];
				if((obj3 != null) && (obj3 != DBNull.Value))
				{
					precision = int.Parse(obj3.ToString());
				}
				int digit = 0;
				object obj4 = dr["NUMERIC_SCALE"];
				if((obj4 != null) && (obj4 != DBNull.Value))
				{
					digit = int.Parse(obj4.ToString());
				}
				object obj5 = dr["COLUMN_COMMENT"];
				string comment = name;
				if((obj5 != null) && (obj5 != DBNull.Value))
				{
					comment = obj5.ToString();
				}
				int autoGen = 0;
				object obj6 = dr["EXTRA"];
				if(((obj6 != null) && (obj6 != DBNull.Value)) && obj6.ToString().Equals("auto_increment"))
				{
					autoGen = 1;
				}
				ColumnModel item = this.GetColumnModel(name, comment, typeName, precision, digit, length, autoGen, pkList);
				list.Add(item);
			}
			return list;
		}

		private string GetTableComment(string dbName, string tableName)
		{
			string tableComment = tableName;
			string sql = string.Format(sqlTableComment, dbName, tableName);
			DataTable dtComment = this.GetData(sql);
			if(dtComment.Rows.Count > 0)
			{
				DataRow drComment = dtComment.Rows[0];
				tableComment = drComment["table_comment"].ToString();
			}
			return tableComment;


		}

		private ColumnModel GetColumnModel(String columnName, String comment,
			string typeName, int precision, int digit, int length, int autoGen, List<string> pkList)
		{
			ColumnModel model = new ColumnModel();

			model.ColumnName = columnName.ToUpper();
			model.Comment = comment;
			//设定主键
			if(pkList.Contains(columnName))
				model.IsPK = true;

			model.Length = length;
			model.Precision = precision;
			model.Scale = digit;
			model.AutoGen = autoGen;
			model.ColType = this.STRING;

			if((typeName.Equals("int") || typeName.Equals("tinyint")) || typeName.Equals("mediumint"))
			{
				model.ColType = this.INT;
			}
			if(typeName.Equals("bigint"))
			{
				model.ColType = this.LONG;
			}
			if((typeName.Equals("float") || typeName.Equals("double")) || typeName.Equals("decimal"))
			{
				model.ColType = this.FLOAT;
			}
			if((((typeName.Equals("char") || typeName.Equals("varchar")) || (typeName.Equals("tinytext") || typeName.Equals("text"))) || typeName.Equals("mediumtext")) || typeName.Equals("longtext"))
			{
				model.ColType = this.STRING;
			}
			if(((typeName.Equals("datetime") || typeName.Equals("timestamp")) || typeName.Equals("datetime")) || typeName.Equals("time"))
			{
				model.ColType = this.DATE;
			}
			model.ColDbType = typeName;
			return model;
		}

		public TableModel GetByTable(string tableName)
		{
			DbHelper.ConnectionURL = connectUrl;
			string databaseName = this.GetDatabaseName();
			TableModel tableModel = new TableModel();
			tableModel.TableName = tableName.ToUpper();
			string strComment = GetTableComment(databaseName, tableName);
			tableModel.TableComment = strComment;
			List<string> pkList = GetPk(databaseName, tableName);
			List<ColumnModel> colList = GetColumn(databaseName, tableName, pkList);
			tableModel.ColumnList = colList;
			return tableModel;

		}

		//public ilist<string> getalltable()
		//{
		//    dbhelper.connectionurl = connecturl;
		//    ilist<string> list = new list<string>();
		//    //dataset ds = dbhelper.getdsbysql(sqlalltables);
		//    datatable dt = this.GetData(sql);//ds.tables[0];

		//    foreach(datarow dr in dt.rows)
		//    {
		//        list.add(dr["table_name"].tostring());
		//    }
		//    return list;
		//}

		private DataTable GetData(string sql)
		{
			//DataSet ds = DbHelper.GetDsBySql(sql);
			//return ds.Tables[0];
			MySqlDataAdapter adapter = new MySqlDataAdapter(sql, this.ConnectUrl);
			DataSet dataSet = new DataSet();
			adapter.Fill(dataSet);
			return dataSet.Tables[0];
		}
		private string GetDatabaseName()
		{
			string database = "";
			MySqlConnection connection = new MySqlConnection();
			connection.ConnectionString = this.ConnectUrl;
			connection.Open();
			database = connection.Database;
			connection.Close();
			return database;
		}
	}
}
