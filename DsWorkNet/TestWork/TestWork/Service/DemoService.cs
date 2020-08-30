using System;

using Dswork.Core.Db;

using TestWork.Dao;
using TestWork.Model;

//using Spring.Objects.Factory.Attributes;
//using Spring.Stereotype;

namespace TestWork.Service
{
	//[Service]
	public class DemoService : BaseService<Demo, long>
	{
		//[Autowired]
		// 这里的virtual和protected get非常重要，否则无法直接使用demoDao
		public virtual DemoDao demoDao { set; protected get; }
		//protected virtual DemoDao GetDemoDao() { return demoDao; }

		protected override EntityDao<Demo, long> GetEntityDao()
		{
			return demoDao;
		}
		/*
		public override int Save(Demo entity)
		{
			base.Save(entity);
			base.Save(entity);
			return 1;
		}
		*/
		public Demo GetShow(long id)
		{
			return demoDao.GetShow(id);
		}
	}
}
