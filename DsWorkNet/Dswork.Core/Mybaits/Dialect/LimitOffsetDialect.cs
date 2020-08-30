using System;
using System.Collections.Generic;
using System.Text;

namespace Dswork.Core.Mybaits.Dialect
{
	/// <summary>
	/// 可用于支持(limit m offset n)语法的数据库，如：MySQL、PostgreSQL、SQLite、GBase、H2
	/// </summary>
	public class LimitOffsetDialect : Dialect
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
			StringBuilder sb = new StringBuilder(sql.Length + 21).Append(sql).Append(" limit ");
			sb.Append(limit);
			//if(offset > 0)
			//{
				sb.Append(" offset ").Append(offset);
			//}
			return sb.ToString();
		}
	}
}
