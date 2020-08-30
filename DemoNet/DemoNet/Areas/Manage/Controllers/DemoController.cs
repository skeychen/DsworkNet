using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

using Dswork.Core.Page;
using Dswork.Web;
using Dswork.Mvc;

using DemoNet.Model;
using DemoNet.Service;

namespace Areas.Manage.Controllers
{
	public class DemoController:BaseController
    {
		public virtual DemoService demoService { set; protected get; }

        public ActionResult AddDemo1()
		{
            return View();
        }

		[HttpPost]
		public String AddDemo2(Demo po)
        {
			try
			{
                demoService.Save(po);
				return "1";
			}
			catch (Exception e)
			{
				return "0:" + e.Message;
			}
        }

		[AcceptVerbs(HttpVerbs.Post|HttpVerbs.Get)]
		public String DelDemo()
		{
			try
			{
			  demoService.DeleteBatch(req.GetLongArray("keyIndex", 0));
			  return "1";
			}
			catch (Exception e)
			{
				return "0:" + e.Message;
			}
		}

		public ActionResult UpdDemo1(long keyIndex)
		{
            ViewBag.po = demoService.Get(keyIndex);
			ViewBag.page = req.GetInt("page", 1);
			return View();
		}

		[HttpPost]
		public String UpdDemo2(Demo po)
		{
			try
			{
                demoService.Update(po);
				return "1";
			}
			catch(Exception e)
			{
				return "0:" + e.Message;
			}
		}

		[AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
		public ActionResult GetDemo()
		{
			PageRequest rq = GetPageRequest();
			rq.Filters = req.GetParameterValueMap(false, false);
            Page<Demo> pageModel = demoService.QueryPage(rq);
			ViewBag.pageModel = pageModel;
			ViewBag.result = pageModel.GetResult<Demo>();
			ViewBag.pageNav = (new PageNav<Demo>(Request, pageModel));
			return View();
		}

		public ActionResult GetDemoById(long keyIndex)
		{
            ViewBag.po = demoService.Get(keyIndex);
			return View();
        }
    }
}
