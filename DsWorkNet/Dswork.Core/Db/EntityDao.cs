using System;
using System.Collections.Generic;
using Dswork.Core.Page;

namespace Dswork.Core.Db
{
	/// <summary>
	/// 基类接口
	/// </summary>
	/// <typeparam name="T">对象模型</typeparam>
	/// <typeparam name="PK">主键类</typeparam>
	public interface EntityDao<T, PK>
	{
		/// <summary>
		/// 新增对象
		/// </summary>
		/// <param name="entity">需要新增的对象模型</param>
		/// <returns>返回执行结果</returns>
		int Save(T entity);

		/// <summary>
		/// 删除对象
		/// </summary>
		/// <param name="primaryKey">如果是单主键的，传入主键数据类型，如果为多主键的，可以用主键类或map</param>
		/// <returns>返回执行结果</returns>
		int Delete(PK primaryKey);

		/// <summary>
		/// 更新对象
		/// </summary>
		/// <param name="entity">需要新增的对象模型</param>
		/// <returns>返回执行结果</returns>
		int Update(T entity);

		/// <summary>
		/// 查询对象
		/// </summary>
		/// <param name="primaryKey">如果是单主键的，传入主键数据类型，如果为多主键的，可以用主键类或map</param>
		/// <returns>返回执行结果</returns>
		Object Get(PK primaryKey);
	
		/// <summary>
		/// 默认列表方法
		/// </summary>
		/// <param name="pageRequest">PageRequest.getFilters()查询参数和条件数据</param>
		/// <returns>IList</returns>
		IList<T> QueryList(PageRequest pageRequest);

		/// <summary>
		/// MyBatis默认分页方法
		/// </summary>
		/// <param name="pageRequest">PageRequest.getFilters()查询参数和条件数据</param>
		/// <returns>Page</returns>
		Page<T> QueryPage(PageRequest pageRequest);
	}
}
