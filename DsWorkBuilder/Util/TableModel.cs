using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Builder
{
    /// <summary>
    /// 表对象
    /// </summary>
    public class TableModel
    {
        /// <summary>
        /// 表名
        /// </summary>
        private String tableName = "";

        /// <summary>
        /// 表注释
        /// </summary>
        private String tableComment = "";

        /// <summary>
        /// 表所有的列对象列表
        /// </summary>
        private List<ColumnModel> columnList = new List<ColumnModel>();

        /// <summary>
        /// 取得表相关变量
        /// </summary>
        private Hashtable paramHt = new Hashtable();



        #region 属性开始

        /// <summary>
        /// 获取或设置
        /// </summary>
        public string TableName
        {
            get
            {
                return this.tableName;
            }

            set
            {
                this.tableName = value;
            }
        }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public string TableComment
        {
            get
            {
                if (this.tableComment.Equals(""))
                    return this.tableName;
                return this.tableComment;
            }

            set
            {
                this.tableComment = value;
            }
        }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public List<ColumnModel> ColumnList
        {
            get
            {
                return this.columnList;
            }

            set
            {
                this.columnList = value;
            }
        }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public Hashtable Params
        {
            get
            {
				return this.paramHt;
            }

            set
            {
				this.paramHt = value;
            }
        }

        #endregion

        /// <summary>
        /// 添加表变量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddParam(string key, string value)
        {
			paramHt.Add(key, value);
        }

        /// <summary>
        /// 清除表变量
        /// </summary>
        public void ClearParam()
        {
			paramHt.Clear();
        }


        /// <summary>
        /// 取得非主键列
        /// </summary>
        /// <returns></returns>
        public List<ColumnModel> GetCommonColumn()
        {
            List<ColumnModel> tmpList = new List<ColumnModel>();
            for (int i = 0; i < columnList.Count; i++)
            {
                ColumnModel model = columnList[i];
                if (!model.IsPK)
                    tmpList.Add(model);
            }
            return tmpList;
        }

        

        /// <summary>
        /// 取得主键列,如果主键列不存在则取第一列为主键列.
        /// </summary>
        /// <returns></returns>
        public List<ColumnModel> GetPkColumn()
        {
            List<ColumnModel> tmpList = new List<ColumnModel>();
            for (int i = 0; i < columnList.Count; i++)
            {
                ColumnModel model = columnList[i];
                if (model.IsPK)
                    tmpList.Add(model);
            }
            if (tmpList.Count == 0)
                tmpList.Add(columnList[0]);


            return tmpList;
        }

        /// <summary>
        /// 取得是否有自动生成列
        /// </summary>
        /// <returns></returns>
        public bool GetHaveAutoGen()
        { 
            bool isHave=false;
            for (int i = 0; i < columnList.Count; i++)
            {
                ColumnModel model = columnList[i];
                if (model.AutoGen == 1){
                    isHave = true;
                    break;
                }
            }
            return isHave;
        }

        /// <summary>
        /// 取得自动生成列
        /// </summary>
        /// <returns></returns>
        public ColumnModel GetAutoGenColumn()
        {
            ColumnModel tmpModel = null;
            for (int i = 0; i < columnList.Count; i++)
            {
                ColumnModel model = columnList[i];
                if (model.AutoGen == 1)
                {
                    tmpModel = model;
                    break;
                }
            }
            return tmpModel;
        }

        /// <summary>
        /// 取得可以插入的列对象
        /// </summary>
        /// <returns></returns>
        public List<ColumnModel> GetInsertColumn()
        {
            List<ColumnModel> tmpList = new List<ColumnModel>();
            for (int i = 0; i < columnList.Count; i++)
            {
                ColumnModel model = columnList[i];
                if(model.AutoGen==0)
                    tmpList.Add(model);
            }
            return tmpList;
        }


        /// <summary>
        /// 添加列对象
        /// </summary>
        /// <param name="columnModel"></param>
        public void addColumnModel(ColumnModel columnModel)
        {
            columnList.Add(columnModel);
        }

        /// <summary>
        /// 读取变量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetParam(string key)
        {
			if (paramHt.ContainsKey(key))
				return paramHt[key].ToString();
            return "";
        }
    }

    /// <summary>
    /// 列对象类型
    /// </summary>
    public class ColumnModel
    {
        /// <summary>
        /// 列名
        /// </summary>
        private String columnName = "";
        /// <summary>
        /// 列注释
        /// </summary>
        private String comment = "";
        /// <summary>
        /// 列类型
        /// </summary>
        private String colType = "";
        /// <summary>
        /// 列数据库类型
        /// </summary>
        private String colDbType = "";
        /// <summary>
        /// 是否主键
        /// </summary>
        private bool isPK = false;
        /// <summary>
        /// 字段长度
        /// </summary>
        private int length = 0;
        /// <summary>
        /// 精度
        /// </summary>
        private int precision = 0;
        /// <summary>
        /// 小数位
        /// </summary>
        private int scale = 0;
        /// <summary>
        /// 自增长字段
        /// </summary>
        private int autoGen = 0;



        #region 属性开始

        /// <summary>
        /// 获取或设置
        /// </summary>
        public string ColumnName
        {
            get
            {
                return this.columnName;
            }

            set
            {
                this.columnName = value;
            }
        }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public string Comment
        {
            get
            {
				return this.comment.Trim();
            }

            set
            {
                this.comment = value;
            }
        }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public string ColType
        {
            get
            {
                return this.colType;
            }

            set
            {
                this.colType = value;
            }
        }








        /// <summary>
        /// 获取或设置
        /// </summary>
        public string ColDbType
        {
            get
            {
                return this.colDbType;
            }

            set
            {
                this.colDbType = value;
            }
        }
        #endregion

        /// <summary>
        /// 获取或设置
        /// </summary>
        public bool IsPK
        {
            get
            {
                return this.isPK;
            }

            set
            {
                this.isPK = value;
            }
        }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public int Length
        {
            get
            {
                return this.length;
            }

            set
            {
                this.length = value;
            }
        }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public int Precision
        {
            get
            {
                return this.precision;
            }

            set
            {
                this.precision = value;
            }
        }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public int Scale
        {
            get
            {
                return this.scale;
            }

            set
            {
                this.scale = value;
            }
        }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public int AutoGen
        {
            get
            {
                return this.autoGen;
            }

            set
            {
                this.autoGen = value;
            }
        }

        /// <summary>
        /// 将字段首字母大写
        /// </summary>
        /// <returns></returns>
        public string GetPropertyColumnName()
		{
			string str = this.columnName;
			return str.Substring(0, 1).ToUpper() + str.Substring(1);// 将字符串首字母大写其他不变
        }


        public string GetFirstUpper()
		{
			string str = this.columnName;
			return str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower();
        }

        

        /// <summary>
        /// 取得首字母大写
        /// </summary>
        /// <returns></returns>
        public string GetFirstUpColType()
        {
			string str = this.colType;
			return str.Substring(0, 1).ToUpper() + str.Substring(1);// 将字符串首字母大写其他不变
        }




    }
}
