using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using Dswork.Core.Page;

using IBatisNet.Common.Utilities.Objects;

using IBatisNet.DataMapper;
using IBatisNet.DataMapper.MappedStatements;
using IBatisNet.DataMapper.MappedStatements.PostSelectStrategy;
using IBatisNet.DataMapper.MappedStatements.ResultStrategy;
using IBatisNet.DataMapper.Scope;
using IBatisNet.DataMapper.Configuration.Statements;

using Dswork.Core.Mybaits.Dialect;

namespace Dswork.Core.Mybaits
{
	internal static class OffsetLimitInterceptor
	{
		public static IList QueryPageList(IDialect dialect, ISqlMapper sqlMap, String statementName, Object parameter, int offset, int limit)
		{
			IMappedStatement statement = sqlMap.GetMappedStatement(statementName);
			if(!sqlMap.IsSessionStarted)
			{
				sqlMap.OpenConnection();
			}
			RequestScope request = statement.Statement.Sql.GetRequestScope(statement, parameter, sqlMap.LocalSession);
			request.PreparedStatement.PreparedSql = dialect.GetLimitString(request.PreparedStatement.PreparedSql, offset, limit);
			//Console.WriteLine(dialect.GetType().FullName + "------" + request.PreparedStatement.PreparedSql);
			statement.PreparedCommand.Create(request, sqlMap.LocalSession, statement.Statement, parameter);
			return RunQueryForList(request, sqlMap.LocalSession, parameter, statement.Statement);
		}

		public static IList<T> QueryPageList<T>(IDialect dialect, ISqlMapper sqlMap, String statementName, Object parameter, int offset, int limit)
		{
			IMappedStatement statement = sqlMap.GetMappedStatement(statementName);
			if(!sqlMap.IsSessionStarted)
			{
				sqlMap.OpenConnection();
			}
			RequestScope request = statement.Statement.Sql.GetRequestScope(statement, parameter, sqlMap.LocalSession);
			request.PreparedStatement.PreparedSql = dialect.GetLimitString(request.PreparedStatement.PreparedSql, offset, limit);
			//Console.WriteLine(dialect.GetType().FullName + "------" + request.PreparedStatement.PreparedSql);
			statement.PreparedCommand.Create(request, sqlMap.LocalSession, statement.Statement, parameter);
			return RunQueryForList<T>(request, sqlMap.LocalSession, parameter, statement.Statement);
		}

		private static IList RunQueryForList(RequestScope request, ISqlMapSession session, object parameterObject, IStatement _statement)
		{
			IList list = null;
			using(IDbCommand command = request.IDbCommand)
			{
				list = (_statement.ListClass == null)? (new ArrayList()):(_statement.CreateInstanceOfListClass());
				IDataReader reader = command.ExecuteReader();
				try
				{
					while(reader.Read())
					{
						object obj = ResultStrategyFactory.Get(_statement).Process(request, ref reader, null);
						if(obj != BaseStrategy.SKIP)
						{
							list.Add(obj);
						}
					}
				}
				catch
				{
					throw;
				}
				finally
				{
					reader.Close();
					reader.Dispose();
				}
				ExecutePostSelect(request);
				RetrieveOutputParameters(request, session, command, parameterObject);
			}
			return list;
		}

		private static IList<T> RunQueryForList<T>(RequestScope request, ISqlMapSession session, object parameterObject, IStatement _statement)
		{
			IList<T> list = new List<T>();
			using(IDbCommand command = request.IDbCommand)
			{
				list = (_statement.ListClass == null) ? (new List<T>()) : (_statement.CreateInstanceOfGenericListClass<T>());
				IDataReader reader = command.ExecuteReader();
				try
				{
					while(reader.Read())
					{
						object obj = ResultStrategyFactory.Get(_statement).Process(request, ref reader, null);
						if(obj != BaseStrategy.SKIP)
						{
							list.Add((T)obj);
						}
					}
				}
				catch
				{
					throw;
				}
				finally
				{
					reader.Close();
					reader.Dispose();
				}
				ExecutePostSelect(request);
				RetrieveOutputParameters(request, session, command, parameterObject);
			}
			return list;
		}

		private static void ExecutePostSelect(RequestScope request)
		{
			while(request.QueueSelect.Count > 0)
			{
				PostBindind postSelect = request.QueueSelect.Dequeue() as PostBindind;
				PostSelectStrategyFactory.Get(postSelect.Method).Execute(postSelect, request);
			}
		}

		private static void RetrieveOutputParameters(RequestScope request, ISqlMapSession session, IDbCommand command, object result)
		{
			if(request.ParameterMap != null)
			{
				int count = request.ParameterMap.PropertiesList.Count;
				for(int i = 0; i < count; i++)
				{
					IBatisNet.DataMapper.Configuration.ParameterMapping.ParameterProperty mapping = request.ParameterMap.GetProperty(i);
					if(mapping.Direction == ParameterDirection.Output ||
						mapping.Direction == ParameterDirection.InputOutput)
					{
						string parameterName = string.Empty;
						if(session.DataSource.DbProvider.UseParameterPrefixInParameter == false)
						{
							parameterName = mapping.ColumnName;
						}
						else
						{
							parameterName = session.DataSource.DbProvider.ParameterPrefix +
								mapping.ColumnName;
						}

						if(mapping.TypeHandler == null) // Find the TypeHandler
						{
							lock(mapping)
							{
								if(mapping.TypeHandler == null)
								{
									Type propertyType = ObjectProbe.GetMemberTypeForGetter(result, mapping.PropertyName);

									mapping.TypeHandler = request.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(propertyType);
								}
							}
						}

						// Fix IBATISNET-239
						//"Normalize" System.DBNull parameters
						IDataParameter dataParameter = (IDataParameter)command.Parameters[parameterName];
						object dbValue = dataParameter.Value;

						object value = null;

						bool wasNull = (dbValue == DBNull.Value);
						if(wasNull)
						{
							if(mapping.HasNullValue)
							{
								value = mapping.TypeHandler.ValueOf(mapping.GetAccessor.MemberType, mapping.NullValue);
							}
							else
							{
								value = mapping.TypeHandler.NullValue;
							}
						}
						else
						{
							value = mapping.TypeHandler.GetDataBaseValue(dataParameter.Value, result.GetType());
						}

						request.IsRowDataFound = request.IsRowDataFound || (value != null);

						request.ParameterMap.SetOutputParameter(ref result, mapping, value);
					}
				}
			}
		}
	}
}
