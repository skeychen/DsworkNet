using System;
using System.Collections.Generic;
using Dswork.Core.Page;

namespace Dswork.Core.Db
{
	/// <summary>
	/// IBatisNet的基础Dao类
	/// </summary>
	/// <typeparam name="T">对象模型</typeparam>
	/// <typeparam name="PK">主键类</typeparam>
	public abstract class BaseDao<T, PK>:MyBatisDao,EntityDao<T, PK>
	{
		private static String statement = "query";

		/// <summary>
		/// 新增对象
		/// </summary>
		/// <param name="entity">需要新增的对象模型</param>
		/// <returns>int返回执行结果</returns>
		public virtual int Save(T entity)
		{
			ExecuteInsert("insert", entity);
			return 1;
		}

		/// <summary>
		/// 根据主键删除对象
		/// </summary>
		/// <param name="primaryKey">如果是单主键的，传入主键数据类型，如果为多主键的，可以用主键类或map</param>
		/// <returns>int返回执行结果</returns>
		public virtual int Delete(PK primaryKey)
		{
			return ExecuteDelete("delete", primaryKey);
		}

		/// <summary>
		/// 更新对象
		/// </summary>
		/// <param name="entity">需要更新的对象模型</param>
		/// <returns>int返回执行结果</returns>
		public virtual int Update(T entity)
		{
			return ExecuteUpdate("update", entity);
		}

		/// <summary>
		/// 根据主键primaryKey获取对象
		/// </summary>
		/// <param name="primaryKey">如果是单主键的，传入主键数据类型，如果为多主键的，可以用主键类或map</param>
		/// <returns>T对象模型</returns>
		public virtual Object Get(PK primaryKey)
		{
			return ExecuteSelect("select", primaryKey);
		}

		/// <summary>
		/// 取得数据条数
		/// </summary>
		/// <param name="pageRequest">PageRequest.getFilters()查询参数和条件数据</param>
		/// <returns>int</returns>
		public virtual int QueryCount(PageRequest pageRequest)
		{
			return QueryCount(statement + "Count", pageRequest);
		}

		/// <summary>
		/// 不分页查询数据
		/// </summary>
		/// <param name="pageRequest">PageRequest.getFilters()查询参数和条件数据</param>
		/// <returns>IList&lt;T&gt;</returns>
		public virtual IList<T> QueryList(PageRequest pageRequest)
		{
			return QueryList<T>(statement, pageRequest);
		}

		/// <summary>
		/// 分页查询数据，statement默认为query
		/// </summary>
		/// <param name="pageRequest">PageRequest.getFilters()查询参数和条件数据</param>
		/// <returns>Page&lt;T&gt;</returns>
		public virtual Page<T> QueryPage(PageRequest pageRequest)
		{
			return QueryPage<T>(statement, pageRequest, statement + "Count", pageRequest);
		}

		/// <summary>
		/// 分页查询数据
		/// </summary>
		/// <param name="statementName">查询数据的statement，统计总数的是statement + "Count"</param>
		/// <param name="pageRequest">PageRequest.getFilters()查询参数和条件数据</param>
		/// <returns>Page&lt;T&gt;</returns>
		protected virtual Page<T> QueryPage(String statementName, PageRequest pageRequest)
		{
			return QueryPage<T>(statementName, pageRequest, statementName + "Count", pageRequest);
		}
	}
}
