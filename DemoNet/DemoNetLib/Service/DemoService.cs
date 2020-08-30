using System;

using Dswork.Core.Db;

using DemoNet.Dao;
using DemoNet.Model;

namespace DemoNet.Service
{
    public class DemoService : BaseService<Demo, long>
    {
        public virtual DemoDao demoDao { set; protected get; }

        protected override EntityDao<Demo, long> GetEntityDao()
        {
            return demoDao;
        }
    }
}
