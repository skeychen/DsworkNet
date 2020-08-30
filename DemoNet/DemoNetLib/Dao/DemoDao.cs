using System;

using Dswork.Core.Db;

using DemoNet.Model;

namespace DemoNet.Dao
{
	public class DemoDao:BaseDao<Demo, long>
	{
		protected override Type GetEntityClass()
		{
			return typeof(Demo);
		}
	}
}
