using System;
using System.Collections.Generic;
using System.Text;

namespace Dswork.Core.Mybaits.Dialect
{
	/// <summary>
    /// Dialect for DB2
	/// </summary>
    public class DB2Dialect : Dialect
	{
		/// <summary>
		/// 是否支持分页，limit和offset
		/// </summary>
		/// <returns>Boolean</returns>
		public override Boolean SupportsLimitOffset()
		{
			return true;
		}

		/// <summary>
		/// 将sql变成分页sql
		/// </summary>
		/// <param name="sql">原始sql语句</param>
		/// <param name="offset">跳过行数</param>
		/// <param name="limit">返回行数</param>
		/// <returns>String</returns>
		public override String GetLimitString(String sql, int offset, int limit)
		{

            StringBuilder sb = new StringBuilder(sql.Length + 110);
            sb.Append("select * from ( select ROW_NUMBER() over() rn, t_.* from (")
            .Append(sql)
            .Append(") t_ ")
            .Append(") n_ where n_.rn>").Append(offset).Append(" and n_.rn<=").Append(offset + limit);
            return sb.ToString();
		}
	}
}
