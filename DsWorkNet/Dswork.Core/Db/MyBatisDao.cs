using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using IBatisNet.DataMapper;
using IBatisNet.DataMapper.MappedStatements;
using IBatisNet.DataMapper.Scope;

using Dswork.Core.Page;
using Dswork.Core.Mybaits;
using Dswork.Core.Mybaits.Dialect;

using log4net;

namespace Dswork.Core.Db
{
	/// <summary>
	/// IBatisNet的抽象类，不是MyBatis的
	/// </summary>
	public abstract class MyBatisDao
	{
		/// <summary>
		/// 用于写入log4net日志
		/// </summary>
		protected readonly ILog log = LogManager.GetLogger(typeof(MyBatisDao));

		private static Object _lock = new Object();//互斥锁
		private String _statement = null;
		private ISqlMapper session;
		private IDialect dialect;

		/// <summary>
		/// 注入ISqlMapper对象
		/// </summary>
		public ISqlMapper SqlSessionTemplate
		{
			set
			{
				session = value;
			}
		}

		/// <summary>
		/// 注入IDialect对象
		/// </summary>
		public IDialect SqlSessionTemplateDialect
		{
			set
			{
				dialect = value;
			}
		}

		#region 公共方法

		/// <summary>
		/// 用于返回命名空间的全路径typeof(T).FullName
		/// </summary>
		/// <returns>Type</returns>
		protected abstract Type GetEntityClass();

		/// <summary>
		/// 当无法使用GetEntityClass()时，可以重载此方法，用于返回命名空间的值
		/// </summary>
		/// <returns>String</returns>
		protected virtual String GetSqlNamespace()
		{
			return GetEntityClass().FullName;
		}

		/// <summary>
		/// 获取需要操作sql的id
		/// </summary>
		/// <param name="statementName">SQL的ID(不包含namespace)</param>
		/// <returns>String</returns>
		protected virtual String GetStatementName(String statementName)
		{
			if(_statement == null)
			{
				_statement = GetSqlNamespace() + ".";
			}
			return _statement + statementName;
		}

		/// <summary>
		/// 封装ISqlMapper.Insert方法
		/// </summary>
		/// <param name="statementName">SQL的ID(不包含namespace)</param>
		/// <param name="parameter">参数</param>
		/// <returns>Object</returns>
		protected Object ExecuteInsert(String statementName, Object parameter)
		{
			lock(_lock)
			{
				return session.Insert(GetStatementName(statementName), parameter);
			}
		}

		/// <summary>
		/// 封装ISqlMapper.Delete方法
		/// </summary>
		/// <param name="statementName">SQL的ID(不包含namespace)</param>
		/// <param name="parameter">参数</param>
		/// <returns>int</returns>
		protected int ExecuteDelete(String statementName, Object parameter)
		{
			lock(_lock)
			{
				return session.Delete(GetStatementName(statementName), parameter);
			}
		}

		/// <summary>
		/// 封装ISqlMapper.Update方法
		/// </summary>
		/// <param name="statementName">SQL的ID(不包含namespace)</param>
		/// <param name="parameter">参数</param>
		/// <returns>int</returns>
		protected int ExecuteUpdate(String statementName, Object parameter)
		{
			lock(_lock)
			{
				return session.Update(GetStatementName(statementName), parameter);
			}
		}

		/// <summary>
		/// 封装ISqlMapper.QueryForObject方法
		/// </summary>
		/// <param name="statementName">SQL的ID(不包含namespace)</param>
		/// <param name="parameter">参数</param>
		/// <returns>Object</returns>
		protected Object ExecuteSelect(String statementName, Object parameter)
		{
			lock(_lock)
			{
				return session.QueryForObject(GetStatementName(statementName), parameter);
			}
		}

		/// <summary>
		/// 封装ISqlMapper.QueryForObject&lt;T&gt;方法
		/// </summary>
		/// <param name="statementName">SQL的ID(不包含namespace)</param>
		/// <param name="parameter">参数</param>
		/// <returns>T</returns>
		protected T ExecuteSelect<T>(String statementName, Object parameter)
		{
			lock(_lock)
			{
				return session.QueryForObject<T>(GetStatementName(statementName), parameter);
			}
		}

		/// <summary>
		/// 封装ISqlMapper.QueryForList方法
		/// </summary>
		/// <param name="statementName">SQL的ID(不包含namespace)</param>
		/// <param name="parameter">参数</param>
		/// <returns>IList</returns>
		protected IList ExecuteSelectList(String statementName, Object parameter)
		{
			lock(_lock)
			{
				return session.QueryForList(GetStatementName(statementName), parameter);
			}
		}

		/// <summary>
		/// 封装ISqlMapper.QueryForList&lt;T&gt;方法
		/// </summary>
		/// <param name="statementName">SQL的ID(不包含namespace)</param>
		/// <param name="parameter">参数</param>
        /// <returns>IList&lt;T&gt;</returns>
		protected IList<T> ExecuteSelectList<T>(String statementName, Object parameter)
		{
			lock(_lock)
			{
				return session.QueryForList<T>(GetStatementName(statementName), parameter);
			}
		}

		/// <summary>
		/// 不分页查询数据，封装ISqlMapper.QueryForList方法
		/// </summary>
		/// <param name="statementName">SQL的ID(不包含namespace)</param>
		/// <param name="pageRequest">PageRequest.getFilters()查询参数和条件数据</param>
		/// <returns>IList</returns>
		protected IList QueryList(String statementName, PageRequest pageRequest)
		{
			lock(_lock)
			{
				return session.QueryForList(GetStatementName(statementName), pageRequest.Filters);
			}
		}

		/// <summary>
		/// 不分页查询数据，封装ISqlMapper.QueryForList&lt;T&gt;方法
		/// </summary>
		/// <param name="statementName">SQL的ID(不包含namespace)</param>
		/// <param name="pageRequest">PageRequest.getFilters()查询参数和条件数据</param>
        /// <returns>IList&lt;T&gt;</returns>
		protected IList<T> QueryList<T>(String statementName, PageRequest pageRequest)
		{
			lock(_lock)
			{
				return session.QueryForList<T>(GetStatementName(statementName), pageRequest.Filters);
			}
		}

		/// <summary>
		/// 分页查询数据
		/// </summary>
		/// <param name="statementName">查询SQL的ID(不包含namespace)</param>
		/// <param name="pageRequest">PageRequest.getFilters()查询参数和条件数据</param>
		/// <param name="statementNameCount">查询总数SQL的ID(不包含namespace)</param>
		/// <param name="pageRequestCount">PageRequest.getFilters()查询参数和条件数据</param>
		/// <returns>Page&lt;Object&gt;</returns>
		protected Page<Object> QueryPage(String statementName, PageRequest pageRequest, String statementNameCount, PageRequest pageRequestCount)
		{
			int totalCount = QueryCount(statementNameCount, pageRequestCount);
			Page<Object> page = new Page<Object>(pageRequest.CurrentPage, pageRequest.PageSize, totalCount);
			IList list;
			if(dialect != null && dialect.SupportsLimitOffset())
			{
				list = OffsetLimitInterceptor.QueryPageList(dialect, session, GetStatementName(statementName), pageRequest.Filters, page.FirstResultIndex, page.PageSize);
			}
			else
			{
				Console.WriteLine("请注意，你正在使用逻辑分页！");
				list = session.QueryForList(GetStatementName(statementName), pageRequest.Filters, page.FirstResultIndex, page.PageSize);
			}
			page.SetResult(list);
			return page;
		}

		/// <summary>
		/// 分页查询数据
		/// </summary>
		/// <param name="statementName">查询SQL的ID(不包含namespace)</param>
		/// <param name="pageRequest">PageRequest.getFilters()查询参数和条件数据</param>
		/// <param name="statementNameCount">查询总数SQL的ID(不包含namespace)</param>
		/// <param name="pageRequestCount">PageRequest.getFilters()查询参数和条件数据</param>
		/// <returns>Page&lt;T&gt;</returns>
		protected Page<T> QueryPage<T>(String statementName, PageRequest pageRequest, String statementNameCount, PageRequest pageRequestCount)
		{
			int totalCount = QueryCount(statementNameCount, pageRequestCount);
			Page<T> page = new Page<T>(pageRequest.CurrentPage, pageRequest.PageSize, totalCount);
			IList<T> list;
			if(dialect != null && dialect.SupportsLimitOffset())
			{
				list = OffsetLimitInterceptor.QueryPageList<T>(dialect, session, GetStatementName(statementName), pageRequest.Filters, page.FirstResultIndex, page.PageSize);
			}
			else
			{
				Console.WriteLine("请注意，你正在使用逻辑分页！");
				list = session.QueryForList<T>(GetStatementName(statementName), pageRequest.Filters, page.FirstResultIndex, page.PageSize);
			}
			page.SetResult(list);
			return page;
		}

		/// <summary>
		/// 执行查询操作取得数据条数
		/// </summary>
		/// <param name="statementNameCount">SQL的ID(不包含namespace)</param>
		/// <param name="pageRequestCount">PageRequest.getFilters()查询参数和条件数据</param>
		/// <returns>int</returns>
		protected int QueryCount(String statementNameCount, PageRequest pageRequestCount)
		{
			Object o = session.QueryForObject(GetStatementName(statementNameCount), pageRequestCount.Filters);
			return QueryCountProcess(o);
		}

		private static int QueryCountProcess(Object o)
		{
			int totalCount = 0;
			try
			{
				totalCount = (int)o;
			}
			catch
			{
				totalCount = 0;
			}
			return totalCount;
		}

		/// <summary>
		/// 执行查询操作返回数据集
		/// </summary>
		/// <param name="statementName">SQL的ID(不包含namespace)</param>
		/// <param name="parameter">参数</param>
		/// <returns></returns>
		protected DataSet QueryDataSet(string statementName, object parameter)
		{
			IMappedStatement statement = session.GetMappedStatement(statementName);
			if(!session.IsSessionStarted)
			{
				session.OpenConnection();
			}
			RequestScope scope;
			lock(_lock)
			{
				scope = statement.Statement.Sql.GetRequestScope(statement, parameter, session.LocalSession);
				statement.PreparedCommand.Create(scope, session.LocalSession, statement.Statement, parameter);
			}
			IDbCommand dc = session.LocalSession.CreateCommand(scope.IDbCommand.CommandType);
			dc.CommandText = scope.IDbCommand.CommandText;
			foreach(IDbDataParameter para in scope.IDbCommand.Parameters)
			{
				//dc.Parameters.Add(para);
				IDbDataParameter pa = session.LocalSession.CreateDataParameter();
				pa.ParameterName = para.ParameterName;
				pa.Value = para.Value;
				dc.Parameters.Add(pa);
			}
			//dc.Parameters = scope.IDbCommand.Parameters;
			IDbDataAdapter adapter = session.LocalSession.CreateDataAdapter(dc);
			DataSet ds = new DataSet();
			adapter.Fill(ds);
			return ds;
		}

		#endregion 公共方法

		#region 事务处理
		/// <summary>
		/// 开始事务
		/// </summary>
		protected virtual void BeginTransaction()
		{
			session.BeginTransaction();
		}
		/// <summary>
		/// 提交事务
		/// </summary>
		protected virtual void CommitTransaction()
		{
			session.CommitTransaction();
		}
		/// <summary>
		/// 回滚事务
		/// </summary>
		protected virtual void RollBackTransaction()
		{
			session.RollBackTransaction();
		}
		#endregion 事务处理
	}
}
