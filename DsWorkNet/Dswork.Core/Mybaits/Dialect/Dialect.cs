using System;
using System.Collections.Generic;

namespace Dswork.Core.Mybaits.Dialect
{
	/// <summary>
	/// Dialect
	/// </summary>
	public class Dialect : IDialect
	{
		/// <summary>
		/// 默认不支持分页
		/// </summary>
		/// <returns>Boolean</returns>
		public virtual Boolean SupportsLimitOffset()
		{
			return false;
		}

		/// <summary>
		/// 获取count语句
		/// </summary>
		/// <param name="sql">原始sql语句</param>
		/// <returns>String</returns>
		public virtual String GetCountSql(String sql)
		{
			return "select count(1) amount from (" + sql + ") as a";
		}

		/// <summary>
		/// 将sql变成分页sql
		/// </summary>
		/// <param name="sql">原始sql语句</param>
		/// <param name="offset">跳过行数</param>
		/// <param name="limit">返回行数</param>
		/// <returns>String</returns>
		public virtual String GetLimitString(String sql, int offset, int limit)
		{
			throw new Exception("不支持分页！" + sql);
		}
	}
}
