using System;

using Dswork.Core.Db;

using TestWork.Model;

namespace TestWork.Dao
{
	public class DemoDao:BaseDao<Demo, long>
	{
		protected override Type GetEntityClass()
		{
			return typeof(Demo);
		}

		public Demo GetShow(long id)
		{
			return this.ExecuteSelect<Demo>("select", id);
		}
	}
}
