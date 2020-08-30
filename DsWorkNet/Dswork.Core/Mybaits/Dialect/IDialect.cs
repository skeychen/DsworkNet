using System;
using System.Collections.Generic;

namespace Dswork.Core.Mybaits.Dialect
{
	/// <summary>
	/// IDialect
	/// </summary>
	public interface IDialect
	{
		/// <summary>
		/// 默认不支持分页
		/// </summary>
		/// <returns>Boolean</returns>
		Boolean SupportsLimitOffset();

		/// <summary>
		/// 获取count语句
		/// </summary>
		/// <param name="sql">原始sql语句</param>
		/// <returns>String</returns>
		String GetCountSql(String sql);

		/// <summary>
		/// 将sql变成分页sql
		/// </summary>
		/// <param name="sql">原始sql语句</param>
		/// <param name="offset">跳过行数</param>
		/// <param name="limit">返回行数</param>
		/// <returns>String</returns>
		String GetLimitString(String sql, int offset, int limit);
	}
}
