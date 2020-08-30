using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Dswork.Web
{
	/// <summary>
	/// Cookie的常用操作
	/// </summary>
	public class MyCookie
	{
		private Object request;
		private Object response;

		/// <summary>
		/// 初始化cookie
		/// </summary>
		/// <param name="context">HttpContext</param>
		public MyCookie(HttpContext context)
		{
			this.request = context.Request;
			this.response = context.Response;
		}

		/// <summary>
		/// 初始化cookie
		/// </summary>
		/// <param name="context">HttpContextBase</param>
		public MyCookie(HttpContextBase context)
		{
			this.request = context.Request;
			this.response = context.Response;
		}

		/// <summary>
		/// 初始化cookie
		/// </summary>
		/// <param name="context">HttpContextWrapper</param>
		public MyCookie(HttpContextWrapper context)
		{
			this.request = context.Request;
			this.response = context.Response;
		}

		/// <summary>
		/// 初始化cookie
		/// </summary>
		/// <param name="request">HttpRequest</param>
		/// <param name="response">HttpResponse</param>
		public MyCookie(HttpRequest request, HttpResponse response)
		{
			this.request = request;
			this.response = response;
		}

		/// <summary>
		/// 初始化cookie
		/// </summary>
		/// <param name="request">HttpRequestBase</param>
		/// <param name="response">HttpResponseBase</param>
		public MyCookie(HttpRequestBase request, HttpResponseBase response)
		{
			this.request = request;
			this.response = response;
		}

		/// <summary>
		/// 初始化cookie
		/// </summary>
		/// <param name="request">HttpRequestWrapper</param>
		/// <param name="response">HttpResponseWrapper</param>
		public MyCookie(HttpRequestWrapper request, HttpResponseWrapper response)
		{
			this.request = request;
			this.response = response;
		}

		/// <summary>
		/// 往客户端写入Cookie，当页面关闭时删除cookie，当前应用所有页面有效
		/// </summary>
		/// <param name="name">cookie参数名</param>
		/// <param name="value">cookie参数值</param>
		public void AddCookie(String name, String value)
		{
			AddCookie(name, value, -1, "/", null, false, false);
			//AddCookie(name, value, -1, HttpRuntime.AppDomainAppVirtualPath);
			//AddCookie(name, value, -1, request.ApplicationPath);
		}

		/// <summary>
		/// 往客户端写入Cookie，当前应用所有页面有效
		/// </summary>
		/// <param name="name">cookie参数名</param>
		/// <param name="value">cookie参数值</param>
		/// <param name="maxAge">有效时间，int(单位秒)，0:删除Cookie，-1:页面关闭时删除cookie</param>
		public void AddCookie(String name, String value, int maxAge)
		{
			AddCookie(name, value, maxAge, "/", null, false, false);
		}

		/// <summary>
		/// 往客户端写入Cookie，当前应用所有页面有效
		/// </summary>
		/// <param name="name">cookie参数名</param>
		/// <param name="value">cookie参数值</param>
		/// <param name="maxAge">有效时间，int(单位秒)，0:删除Cookie，-1:页面关闭时删除cookie</param>
		/// <param name="path">与cookie一起传输的虚拟路径</param>
		public void AddCookie(String name, String value, int maxAge, String path)
		{
			AddCookie(name, value, maxAge, path, null, false, false);
		}

		/// <summary>
		/// 往客户端写入Cookie，当前应用所有页面有效
		/// </summary>
		/// <param name="name">cookie参数名</param>
		/// <param name="value">cookie参数值</param>
		/// <param name="maxAge">有效时间，int(单位秒)，0:删除Cookie，-1:页面关闭时删除cookie</param>
		/// <param name="path">与cookie一起传输的虚拟路径</param>
		/// <param name="domain">与cookie关联的域</param>
		public void AddCookie(String name, String value, int maxAge, String path, String domain)
		{
			AddCookie(name, value, maxAge, path, domain, false, false);
		}

		/// <summary>
		/// 往客户端写入Cookie，当前应用所有页面有效
		/// </summary>
		/// <param name="name">cookie参数名</param>
		/// <param name="value">cookie参数值</param>
		/// <param name="maxAge">有效时间，int(单位秒)，0:删除Cookie，-1:页面关闭时删除cookie</param>
		/// <param name="path">与cookie一起传输的虚拟路径</param>
		/// <param name="domain">与cookie关联的域</param>
		/// <param name="isSecure">是否在https请求时才进行传输</param>
		/// <param name="isHttpOnly">是否只能通过http访问</param>
		public void AddCookie(String name, String value, int maxAge, String path, String domain, Boolean isSecure, Boolean isHttpOnly)
		{
			HttpCookie cookie = new HttpCookie(name, value);
			cookie.Expires = DateTime.Now.AddSeconds(maxAge);
			cookie.Path = path;
			if (maxAge > 0 && domain != null)
			{
				cookie.Domain = domain;
			}
			cookie.Secure = isSecure;
			cookie.HttpOnly = isHttpOnly;
			AddCookie(cookie);
		}

		/// <summary>
		/// 删除cookie
		/// </summary>
		/// <param name="name">cookie参数名</param>
		public void DelCookie(String name)
		{
			AddCookie(name, "", -1, "/", null);
		}

		/// <summary>
		/// 根据cookie名称取得参数值
		/// </summary>
		/// <param name="name">cookie参数名</param>
		/// <returns>存在返回String，不存在返回null</returns>
		public string GetValue(String name)
		{
			HttpCookie cookie;
			try
			{
				cookie = ((HttpRequest)request).Cookies[name];
			}
			catch
			{
				try
				{
					cookie = ((HttpRequestBase)request).Cookies[name];
				}
				catch
				{
					cookie = ((HttpResponseWrapper)request).Cookies[name];
				}
			}
			if(cookie != null)
			{
				return cookie.Value;
			}
			return null;
		}

		/// <summary>
		/// 根据Cookie参数名判断Cookie是否存在该值
		/// </summary>
		/// <param name="name">cookie参数名</param>
		/// <returns>存在返回true，不存在返回false</returns>
		public Boolean Exist(String name)
		{
			return GetValue(name) != null;
		}

		private void AddCookie(HttpCookie cookie)
		{
			try
			{
				((HttpResponse)response).Cookies.Add(cookie);
			}
			catch
			{
				try
				{
					((HttpResponseBase)response).Cookies.Add(cookie);
				}
				catch
				{
					((HttpResponseWrapper)response).Cookies.Add(cookie);
				}
			}
		}
	}
}
