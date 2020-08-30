using System;
using System.Collections;
using System.Collections.Generic;

namespace Dswork.Core.Page
{
	/// <summary>
	/// 分页信息, 第一页从1开始
	/// </summary>
	/// <typeparam name="T">IList的每条记录使用HashTable存储</typeparam>
	public class Page<T>
	{
		private IList resultList = (IList)new List<T>();
		private IList<T> result = new List<T>();
		private int pageSize;
		private int currentPage;
		private int lastPage;
		private int totalCount = 0;
		private String pageName = "page";
		private String pageSizeName = "pageSize";

		private void PageSetting(int currentPage, int pageSize, int totalCount)
		{
			if(pageSize <= 0)
			{
				throw new Exception("[pageSize] must great than zero");
			}
			this.lastPage = totalCount % pageSize == 0 ? totalCount / pageSize : totalCount / pageSize + 1;
			if (this.lastPage < 1)
			{
				this.lastPage = 1;
			}
			this.pageSize = pageSize;
			if(currentPage <= 1)
			{
				currentPage = 1;
			}
			else if(int.MaxValue == currentPage || currentPage > this.lastPage)
			{
				currentPage = this.lastPage;
			}
			//		// 下面这种的话，如果pageSize=0的话，就相当于不分页
			//		if(pageSize == 0)
			//		{
			//			pageNumber = 1;
			//			pageSize = (totalCount <= 0) ? 1 : totalCount;
			//		}
			this.currentPage = currentPage;
			this.totalCount = totalCount;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="currentPage">当前页码</param>
		/// <param name="pageSize">一页显示的条数</param>
		/// <param name="totalCount">数据总条数</param>
		public Page(int currentPage, int pageSize, int totalCount)
		{
			PageSetting(currentPage, pageSize, totalCount);
		}

		/// <summary>
		/// 构造函数，使用ResultList变量，尽量使用泛型
		/// </summary>
		/// <param name="currentPage">当前页码</param>
		/// <param name="pageSize">一页显示的条数</param>
		/// <param name="totalCount">数据总条数</param>
		/// <param name="result">结果集IList</param>
		[Obsolete("Recommended to use Page(int currentPage, int pageSize, int totalCount, IList<T> result)", false)]
		public Page(int currentPage, int pageSize, int totalCount, IList result)
		{
			PageSetting(currentPage, pageSize, totalCount);
			this.SetResult(result);
		}

		/// <summary>
		/// 构造函数，使用Result变量&lt;T&gt;
		/// </summary>
		/// <param name="currentPage">当前页码</param>
		/// <param name="pageSize">一页显示的条数</param>
		/// <param name="totalCount">数据总条数</param>
        /// <param name="result">结果集IList&lt;T&gt;</param>
		public Page(int currentPage, int pageSize, int totalCount, IList<T> result)
		{
			PageSetting(currentPage, pageSize, totalCount);
			this.SetResult(result);
			this.SetResult((IList)result);
		}

		/// <summary>
		/// 取得当前页页码
		/// </summary>
		public int CurrentPage
		{
			get
			{
				return currentPage;
			}
		}

		/// <summary>
		/// 得到结果集第一条数据的行编码
		/// </summary>
		public int FirstResultIndex
		{
			get
			{
				if(pageSize <= 0)
				{
					throw new Exception("[pageSize] must great than zero");
				}
				return (currentPage - 1) * pageSize;
			}
		}

		/// <summary>
		/// 取得数据总页数，也就是最后一页页码
		/// </summary>
		public int LastPage
		{
			get
			{
				return lastPage;
			}
		}

		/// <summary>
		/// 取得下一页页码
		/// </summary>
		public int NextPage
		{
			get
			{
				return currentPage + 1;
			}
		}

		/// <summary>
		/// 取得当前页码的参数名，默认值是page
		/// </summary>
		public String PageName
		{
			get
			{
				return pageName;
			}
			set
			{
				if(value == null || value.Trim().Length == 0)
				{
					throw new Exception("[pageName] must be not null");
				}
				pageName = value.Trim();
			}
		}

		/// <summary>
		/// 取得一页显示的条数
		/// </summary>
		public int PageSize
		{
			get
			{
				return pageSize;
			}
		}

		/// <summary>
		/// 取得一页显示的条数的参数名，默认值是page
		/// </summary>
		public String PageSizeName
		{
			get
			{
				return pageSizeName;
			}
			set
			{
				if(value == null || value.Trim().Length == 0)
				{
					throw new Exception("[pageSizeName] must be not null");
				}
				pageSizeName = value.Trim();
			}
		}

		/// <summary>
		/// 取得上一页页码
		/// </summary>
		public int PreviousPage
		{
			get
			{
				return currentPage - 1;
			}
		}

		/// <summary>
		/// 设置结果集
		/// </summary>
		/// <param name="value">IList</param>
		public void SetResult(IList value)
		{
			if(value == null)
			{
				throw new Exception("'result' must be not null");
			}
			resultList = value;
		}

		/// <summary>
		/// 设置结果集
		/// </summary>
		/// <param name="value">IList&lt;T&gt;</param>
		public void SetResult(IList<T> value)
		{
			if(value == null)
			{
				throw new Exception("'result<T>' must be not null");
			}
			result = value;
		}

		/// <summary>
		/// 取得结果集，结果集IList
		/// </summary>
		public IList GetResult()
		{
			return resultList;
		}

		/// <summary>
		/// 取得结果集，结果集IList&lt;T&gt;
		/// </summary>
		public IList<T> GetResult<M>()
		{
			return result;
		}

		/// <summary>
		/// 取得数据总条数，0表示没有数据
		/// </summary>
		public int TotalCount
		{
			get
			{
				return totalCount;
			}
		}

		/// <summary>
		/// 取得数据总页数
		/// </summary>
		public int TotalPage
		{
			get
			{
				return lastPage;
			}
		}

		/// <summary>
		/// 是否是首页（第一页），第一页页码为1
		/// </summary>
		public Boolean IsFirstPage
		{
			get
			{
				return currentPage == 1;
			}
		}

		/// <summary>
		/// 是否有下一页
		/// </summary>
		public Boolean IsHasNextPage
		{
			get
			{
				return lastPage > currentPage;
			}
		}

		/// <summary>
		/// 是否有上一页
		/// </summary>
		public Boolean IsHasPreviousPage
		{
			get
			{
				return currentPage > 1;
			}
		}

		/// <summary>
		/// 是否是最后一页
		/// </summary>
		public Boolean IsLastPage
		{
			get
			{
				return currentPage >= lastPage;
			}
		}

	}
}
