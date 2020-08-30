using System;
using System.Collections;

namespace Dswork.Core.Page
{
	/// <summary>
	/// 分页请求信息，filters的类型Object，默认初始化是System.Collections.Hashtable
	/// </summary>
	public class PageRequest
	{
		private Object filters = new Hashtable();
		private int currentPage;
		private int pageSize;
		private String pageName = "page";
		private String pageSizeName = "pageSize";

		/// <summary>
		/// 构造函数
		/// </summary>
		public PageRequest() : this(0, 0)
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="filters">条件，使用HashTable</param>
		public PageRequest(Object filters) : this(0, 0, filters)
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="currentPage">当前页码</param>
		/// <param name="pageSize">一页显示的条数</param>
		public PageRequest(int currentPage, int pageSize)
			: this(currentPage, pageSize, new Hashtable())
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="currentPage">当前页码</param>
		/// <param name="pageSize">一页显示的条数</param>
		/// <param name="filters">条件，使用HashTable</param>
		public PageRequest(int currentPage, int pageSize, Object filters)
		{
			this.CurrentPage = currentPage;
			this.PageSize = pageSize;
			this.Filters = filters;
		}

		/// <summary>
		/// 取得当前页
		/// </summary>
		public int CurrentPage
		{
			get
			{
				return currentPage;
			}
			set
			{
				currentPage = value < 0 ? 1 : value;
			}
		}

		/// <summary>
		/// 设置参数，使用HashTable
		/// </summary>
		public Object Filters
		{
			get
			{
				return filters;
			}
			set
			{
				this.filters = value;
			}
		}

		/// <summary>
		/// 设置当前页码的参数名，默认值是page
		/// </summary>
		public String PageName
		{
			get
			{
				return pageName;
			}
			set
			{
				pageName = value;
			}
		}

		/// <summary>
		/// 设置一页显示的条数
		/// </summary>
		public int PageSize
		{
			get
			{
				return pageSize;
			}
			set
			{
				pageSize = value < 0 ? 10 : value;
			}
		}

		/// <summary>
		/// 设置一页显示的条数的参数名，默认值是pageSize
		/// </summary>
		public String PageSizeName
		{
			get
			{
				return pageSizeName;
			}
			set
			{
				pageSizeName = value;
			}
		}
	}
}
