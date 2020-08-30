using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using IBatisNet.Common;
using IBatisNet.Common.Logging;
using IBatisNet.Common.Logging.Impl;
using log4net;
using log4net.Config;
using System.IO;
using Spring.Context.Support;
//using Spring.Core.IO;
//using Spring.Objects.Factory;
//using Spring.Objects.Factory.Xml;

using Dswork.Core.Page;
using TestWork.Dao;
using TestWork.Model;
using TestWork.Service;

namespace Test
{
	public class Program
	{
		public static void executeDictionary(ISqlMapper mapper)
		{
			Console.WriteLine("ok");
			IDictionary str = mapper.QueryForDictionary("Model.Demo.FindPageId", "呵呵", "ID", "TITLE");
			foreach(Object o in str.Keys)
			{
				Console.WriteLine("executeDictionary  : {ID:'" + o + "',TITLE'" + str[o].ToString() + "'}");
			}
		}
		public static void executeList(ISqlMapper mapper)
		{
			Console.WriteLine("ok2");
			IList<Hashtable> str = mapper.QueryForList<Hashtable>("Model.Demo.FindPageId", null);
			foreach(Hashtable o in str)
			{
				Console.WriteLine("executeList : {ID:'" + o["ID"] + "',TITLE:'" + o["TITLE"] + "'} : " + o.ToString());
			}
		}

		public static void doMain(string[] args)
		{
			try
			{
				//XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Config/log4net.config"));
				Spring.Context.IApplicationContext ctx = ContextRegistry.GetContext();
				//ISqlMapper mapper = new DomSqlMapBuilder().Configure(AppDomain.CurrentDomain.BaseDirectory + @"\Config\SqlMap.config");
				//ISqlMapper mapper = ctx.GetObject<ISqlMapper>("SqlSessionTemplate");

				//executeDictionary(mapper);
				//executeList(mapper);

				DemoService ser = ctx.GetObject("demoService") as DemoService;
				Demo d = ser.GetShow(2);
				Console.WriteLine(d.title);
				/*
				try
				{
					Demo d = new Demo();
					d.id = 4;
					d.title = "heihei";
					d.content = "test";
					d.foundtime = DateTime.Now.ToString();
					ser.Save(d);
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
				}
				*/

				Hashtable ht = new Hashtable();
				ht.Add("title", "heihei");
				PageRequest pageRequest = new PageRequest();
				pageRequest.PageSize = 2;
				pageRequest.CurrentPage = 2;
				pageRequest.Filters = ht;
				Page<Demo> page = ser.QueryPage(pageRequest);
				foreach(Demo o in page.GetResult<Demo>())
				{
					Console.WriteLine("Service.QueryPage : {ID:'" + o.id + "',TITLE:'" + o.title + "',CONTENT:'" + o.content + "',FOUNDTIME:'" + o.foundtime + "'} : " + o.ToString());
				}
				Console.WriteLine("总数是：" + page.TotalCount);

				IList<Demo> list = ser.QueryList(pageRequest);
				foreach(Demo o in list)
				{
					Console.WriteLine("Service.QueryList : {ID:'" + o.id + "',TITLE:'" + o.title + "',CONTENT:'" + o.content + "',FOUNDTIME:'" + o.foundtime + "'} : " + o.ToString());
				}
				Console.Read();
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}
	}
}
