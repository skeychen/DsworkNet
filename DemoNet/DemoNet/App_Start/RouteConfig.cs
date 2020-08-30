using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DemoNet
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
			routes.MapRoute("NoAction", "{controller}.html", new { controller = "Home", action = "Index", id = "" });//无Action的匹配 
			routes.MapRoute("NoID", "{controller}/{action}.html", new { controller = "Home", action = "Index", id = "" });//无ID的匹配 
			routes.MapRoute("Root", "", new { controller = "Home", action = "Index", id = "" });//根目录匹配
		}
	}
}