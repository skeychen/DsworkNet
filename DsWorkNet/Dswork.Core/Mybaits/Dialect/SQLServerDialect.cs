using System;
using System.Collections.Generic;
using System.Text;

namespace Dswork.Core.Mybaits.Dialect
{
	/// <summary>
    /// 可用于支持ROW_NUMBER() over(order by *)语法的数据库，如：SQLServer2005、SQLServer2008
	/// </summary>
    public class SQLServerDialect : Dialect
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
            StringBuilder sb = new StringBuilder(sql.Length + 170);
            sb.Append("select * from ( select ROW_NUMBER() over(order by m_.temp__) rn, m_.* from (")
            .Append("select 0 as temp__, t_.* from (")
            .Append(sql)
            .Append(") t_ ")
            .Append(") m_ ")
            .Append(") n_ where n_.rn>").Append(offset).Append(" and n_.rn<=").Append(offset + limit);
            return sb.ToString();
		}
	}
}
