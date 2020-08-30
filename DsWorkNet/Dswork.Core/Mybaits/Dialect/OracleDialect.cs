using System;
using System.Collections.Generic;
using System.Text;

namespace Dswork.Core.Mybaits.Dialect
{
	/// <summary>
	/// Dialect for Oracle
	/// </summary>
	public class OracleDialect : Dialect
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
			StringBuilder sb = new StringBuilder(sql.Length + 90);
            sb.Append("select * from ( select rownum rn, t_.* from (")
			.Append(sql)
            .Append(") t_ where rownum<=").Append(offset + limit)
            .Append(") n_ where n_.rn>").Append(offset);
			return sb.ToString();
		}
	}
}
