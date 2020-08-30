using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Builder
{
	public class MSSQLModel:BaseModel, IDbModel
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

		private string sqlPk = "sp_pkeys N'{0}'";
		private string sqlTableComment = "select b.value from sys.tables a, sys.extended_properties b where a.type='U' and a.object_id=b.major_id and b.minor_id=0 and a.name='{0}'";
		private string sqlColumn = "select a.name, c.name typename, a.max_length length, a.is_nullable,a.precision,a.scale," +
			"(select count(*) from sys.identity_columns where sys.identity_columns.object_id = a.object_id and a.column_id = sys.identity_columns.column_id) as autoGen," +
			"(select value from sys.extended_properties where sys.extended_properties.major_id = a.object_id and sys.extended_properties.minor_id = a.column_id) as description" +
			" from sys.columns a, sys.tables b, sys.types c where a.object_id = b.object_id and a.system_type_id=c.system_type_id and b.name='{0}' and c.name<>'sysname' order by a.column_id";
		private string sqlAllTables = "select name from sys.tables where type='U' and name<>'sysdiagrams'";

		private List<string> GetPk(string tableName)
		{
			List<string> list = new List<string>();
			string sql = string.Format(sqlPk, tableName);
			DataSet ds = DbHelper.GetDsBySql(sql);
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
			DataSet ds = DbHelper.GetDsBySql(sql);
			DataTable dt = ds.Tables[0];
			foreach(DataRow dr in dt.Rows)
			{
				string name = dr["name"].ToString();
				string typename = dr["typename"].ToString();
				int length = int.Parse(dr["length"].ToString());
				int precision = int.Parse(dr["precision"].ToString());
				int scale = int.Parse(dr["scale"].ToString());
				int autoGen = int.Parse(dr["autoGen"].ToString());
				object objDesc = dr["description"];
				string description = name;
				if(objDesc != null && objDesc != DBNull.Value)
				{
					description = objDesc.ToString();
				}
				ColumnModel colModel = GetColumnModel(name, description, typename, precision, scale, length, autoGen, pkList);
				list.Add(colModel);
			}
			return list;
		}

		public TableModel GetByTable(string tableName)
		{
			DbHelper.ConnectionURL = connectUrl;
			TableModel tableModel = new TableModel();
			tableModel.TableName = tableName.ToUpper();
			//取得注释
			string strComment = GetTableComment(tableName);
			tableModel.TableComment = strComment;
			//主键
			List<string> pkList = GetPk(tableName);

			List<ColumnModel> colList = GetColumn(tableName, pkList);

			tableModel.ColumnList = colList;

			return tableModel;
		}

		private string GetTableComment(string tableName)
		{
			string tableComment = tableName;
			string sql = string.Format(sqlTableComment, tableName);
			DataSet ds = DbHelper.GetDsBySql(sql);
			DataTable dtComment = ds.Tables[0];
			if (dtComment.Rows.Count > 0)
			{
				DataRow drComment = dtComment.Rows[0];
				tableComment = drComment["value"].ToString();
			}
			return tableComment;
		}

		private ColumnModel GetColumnModel(String columnName, String comment, string typeName, int precision, int digit, int length, int autoGen, List<string> pkList)
		{
			ColumnModel model = new ColumnModel();

			model.ColumnName = columnName.ToUpper();
			model.Comment = comment;
			//设定主键
			if (pkList.Contains(columnName))
			{
				model.IsPK = true;
			}
			model.Length = length;
			model.Precision = precision;
			model.Scale = digit;
			model.AutoGen = autoGen;
			model.ColType = this.STRING;

			if (typeName.Equals("bigint"))
			{
				model.ColType = this.LONG;
			}
			if (typeName.Equals("bit"))
			{
				model.ColType = this.BOOLEAN;
			}
			if (typeName.Equals("char") || typeName.Equals("nchar"))
			{
				model.ColType = this.STRING;
			}
			if ((typeName.Equals("nvarchar") && length == -1) || (typeName.Equals("varchar") && length == -1))
			{
				model.ColType = this.STRING;
			}
			if ((typeName.Equals("nvarchar") && length > 0) || (typeName.Equals("varchar") && length > 0))
			{
				model.ColType = this.STRING;
			}
			if (typeName.Equals("float"))
			{
				model.ColType = this.FLOAT;
			}
			if (typeName.Equals("real"))
			{
				model.ColType = this.FLOAT;
			}
			if (typeName.Equals("smalldatetime") || typeName.Equals("datetime"))
			{
				model.ColType = this.DATE;
			}
			if (typeName.Equals("smallint"))
			{
				model.ColType = this.INT;
			}
			if (typeName.Equals("int"))
			{
				model.ColType = this.INT;
			}
			if (typeName.Equals("numeric") && precision < 11 && digit == 0)
			{
				model.ColType = this.INT;
			}
			if (typeName.Equals("numeric") && precision > 10 && digit == 0)
			{
				model.ColType = this.LONG;
			}
			if (typeName.Equals("numeric") && digit > 0)
			{
				model.ColType = this.FLOAT;
			}
			if (typeName.Equals("decimal") && precision < 11 && digit == 0)
			{
				model.ColType = this.INT;
			}
			if (typeName.Equals("decimal") && precision > 10 && digit == 0)
			{
				model.ColType = this.LONG;
			}
			if (typeName.Equals("decimal") && digit > 0)
			{
				model.ColType = this.FLOAT;
			}
			if (typeName.Equals("uniqueidentifier"))
			{
				model.ColType = this.STRING;
			}
			if (typeName.Equals("xml"))
			{
				model.ColType = this.STRING;
			}
			if (typeName.Equals("tinyint"))
			{
				model.ColType = this.INT;
			}
			if (typeName.Equals("smallmoney") || typeName.Equals("money"))
			{
				model.ColType = this.FLOAT;
			}
			if (typeName.Equals("timestamp"))
			{
				model.ColType = this.STRING;
			}
			if (typeName.Equals("text") || typeName.Equals("ntext"))
			{
				model.ColType = this.STRING;
			}
			model.ColDbType = typeName;
			return model;
		}

		//public IList<string> GetAllTable()
		//{
		//    DbHelper.ConnectionURL = connectUrl;
		//    IList<string> list = new List<string>();
		//    DataSet ds = DbHelper.GetDsBySql(sqlAllTables);
		//    DataTable dt = ds.Tables[0];

		//    foreach (DataRow dr in dt.Rows)
		//    {
		//        list.Add(dr["name"].ToString());
		//    }
		//    return list;
		//}
	}
}
