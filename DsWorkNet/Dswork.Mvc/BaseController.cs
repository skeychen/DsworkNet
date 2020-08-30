using System;
using System.Web;
using System.Web.Mvc;
using Dswork.Core.Page;
using Dswork.Web;

namespace Dswork.Mvc
{
	public class BaseController : Controller
	{
		protected static String PageSize_SessionName = "dswork_session_pagesize";
		private MyRequest _req;
		protected MyRequest req
		{
			get
			{
				if (_req == null) { _req = new MyRequest(Request); }
				return _req;
			}
		}
	
		protected PageRequest GetPageRequest()
		{
			PageRequest pr = new PageRequest();
			pr.Filters = req.GetParameterValueMap(false, false);
			pr.CurrentPage = req.GetInt("page", 1);
			int pagesize = 10;
			try
			{
				pagesize = int.Parse(Session[PageSize_SessionName].ToString().Trim());
			}
			catch
			{
				pagesize = 10;
			}
			pagesize = req.GetInt("pageSize", pagesize);
			Session[PageSize_SessionName] = pagesize;
			pr.PageSize = pagesize;
			return pr;
		}
	}
}
