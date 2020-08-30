using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace Builder
{

    public class DbHelper
    {
        public static string ConnectionURL = "";

        /// <summary>
        /// 取得DataSet数据集
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataSet GetDsBySql(string sql)
        {
            DataSet ds = new DataSet();
			OleDbDataAdapter od = new OleDbDataAdapter(sql, ConnectionURL);
			od.Fill(ds);
            return ds;
        }


    }
}
