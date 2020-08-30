using System;
using System.Collections;
using System.Collections.Generic;
using Dswork.Core.Page;

namespace Dswork.Core.Db
{
	/// <summary>
	/// 服务基础类
	/// </summary>
	/// <typeparam name="T">对象模型</typeparam>
	/// <typeparam name="PK">主键类</typeparam>
	public abstract class BaseService<T, PK>
	{
		/// <summary>
		/// 需要被子类覆盖
		/// </summary>
		/// <returns>EntityDao&lt;T, PK&gt;</returns>
		protected abstract EntityDao<T, PK> GetEntityDao();

		/// <summary>
		/// 新增对象
		/// </summary>
		/// <param name="entity">需要新增的对象模型</param>
		/// <returns>int返回执行结果</returns>
		public virtual int Save(T entity)
		{
			return GetEntityDao().Save(entity);
		}

		/// <summary>
		/// 根据主键删除对象
		/// </summary>
		/// <param name="primaryKey">如果是单主键的，传入主键数据类型，如果为多主键的，可以用主键类或map</param>
		/// <returns>int返回执行结果</returns>
		public virtual int Delete(PK primaryKey)
		{
			return GetEntityDao().Delete(primaryKey);
		}

		/// <summary>
		/// 根据主键批量删除对象
		/// </summary>
		/// <param name="primaryKeys">主键数组(如果是单主键的，传入主键数据类型，如果为多主键的，可以用主键类或map)</param>
		public virtual void DeleteBatch(PK[] primaryKeys)
		{
			if(primaryKeys != null && primaryKeys.Length > 0)
			{
				foreach(PK p in primaryKeys)
				{
					Delete(p);
				}
			}
		}

		/// <summary>
		/// 更新对象
		/// </summary>
		/// <param name="entity">需要更新的对象模型</param>
		/// <returns>int返回执行结果</returns>
		public virtual int Update(T entity)
		{
			return GetEntityDao().Update(entity);
		}

		/// <summary>
		/// 根据主键primaryKey获取对象
		/// </summary>
		/// <param name="primaryKey">如果是单主键的，传入主键数据类型，如果为多主键的，可以用主键类或map</param>
		/// <returns>T对象模型</returns>
		public virtual T Get(PK primaryKey)
		{
			return (T)GetEntityDao().Get(primaryKey);
		}

		/// <summary>
		/// 取得全部数据
		/// </summary>
		/// <param name="map">查询参数和条件数据</param>
		/// <returns>IList&lt;T&gt;</returns>
		public virtual IList<T> QueryList(Hashtable map)
		{
			PageRequest request = new PageRequest();
			request.Filters = map;
			IList<T> list = GetEntityDao().QueryList(request);
			return list;
		}

		/// <summary>
		/// 默认列表方法
		/// </summary>
		/// <param name="pageRequest">PageRequest.getFilters()查询参数和条件数据</param>
		/// <returns>IList&lt;T&gt;</returns>
		public virtual IList<T> QueryList(PageRequest pageRequest)
		{
			return GetEntityDao().QueryList(pageRequest);
		}

		/// <summary>
		/// 得到分页模型
		/// </summary>
		/// <param name="currentPage">当前页码</param>
		/// <param name="pageSize">一页显示的条数</param>
		/// <param name="map">查询参数和条件数据</param>
		/// <returns>Page&lt;T&gt;</returns>
		public virtual Page<T> QueryPage(int currentPage, int pageSize, Hashtable map)
		{
			PageRequest request = new PageRequest();
			request.Filters = map;
			request.CurrentPage = currentPage;
			request.PageSize = pageSize;
			Page<T> page = GetEntityDao().QueryPage(request);
			return page;
		}

		/// <summary>
		/// 默认分页方法
		/// </summary>
		/// <param name="pageRequest">PageRequest.getFilters()查询参数和条件数据</param>
		/// <returns>Page&lt;T&gt;</returns>
		public virtual Page<T> QueryPage(PageRequest pageRequest)
		{
			return GetEntityDao().QueryPage(pageRequest);
		}
	}
}
