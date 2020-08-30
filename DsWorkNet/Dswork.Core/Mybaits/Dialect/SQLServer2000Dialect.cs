using System;
using System.Collections.Generic;
using System.Text;

namespace Dswork.Core.Mybaits.Dialect
{
	/// <summary>
	/// Dialect for SQLServer2000，查询的sql必须存在ID列，且翻页以ID顺序排序
	/// </summary>
	public class SQLServer2000Dialect:Dialect
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
			StringBuilder sb = new StringBuilder(sql.Length + 150);
			sb.Append("select top ").Append(limit).Append(" n_.* from (")
				.Append("select top ").Append(limit).Append(" m_.* from (")
					.Append("select top ").Append(offset + limit).Append(" t_.* from (")
					.Append(sql)
					.Append(") t_ order by t_.ID")
				.Append(") m_ order by m_.ID desc")
			.Append(") n_ order by n_.ID");
			return sb.ToString();
		}
	}
}
