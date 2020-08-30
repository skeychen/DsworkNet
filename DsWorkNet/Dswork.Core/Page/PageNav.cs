using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace Dswork.Core.Page
{
	/// <summary>
	/// 页面显示翻页的对象
	/// </summary>
	/// <typeparam name="T">详见Tecamo.Core.Page.Page类的T</typeparam>
	public class PageNav<T>
	{
		private static long i = 0L;
		private Page<T> page;
		private Boolean isOutForm = false;
		private static String PAGEFORMID = "jskeyPageForm";
		private String formString = "";
		private String Path = "";
		private static int[] sizeArray = {5, 10, 15, 20, 25, 30, 50};

		/// <summary>
		/// 初始化formString
		/// </summary>
		/// <param name="req"></param>
		private void PageNavInit(NameValueCollection req)
        {
            String pageName = page.PageName;
            String pageSizeName = page.PageSizeName;
            String formId = PAGEFORMID + pageName;
			try
			{
                StringBuilder sb = new StringBuilder("<script language=\"javascript\">if(typeof($jskey)!=\"object\"){$jskey={};}$jskey.page={go:function(pn,page,pagesize){pn=\"" + PAGEFORMID + "\"+pn;page=parseInt(page)||1;page=(page<1)?1:page;document.getElementById(pn+\"_page\").value=page;pagesize=parseInt(pagesize||" + page.PageSize + ")||10;pagesize=(pagesize<1)?10:pagesize;document.getElementById(pn+\"_pagesize\").value=pagesize;document.getElementById(pn).submit();}};</script>\n");
				sb.Append("<form id=\"").Append(formId).Append("\" method=\"post\" style=\"display:none;\" action=\"").Append(Path).Append("\">");
				sb.Append("<input id=\"").Append(formId).Append("_page\" name=\"").Append(pageName).Append("\" type=\"hidden\" value=\"1\"/>\n");
                sb.Append("<input id=\"").Append(formId).Append("_pagesize\" name=\"").Append(pageSizeName).Append("\" type=\"hidden\" value=\"").Append(page.PageSize).Append("\"/>\n");

				String key;
				String[] values;
				for(int i = 0; i < req.Count; i++)
				{
					key = req.Keys[i];
                    if (key != pageName && key != pageSizeName)
					{
						values = req.GetValues(key);
						if(values == null || values.Length == 0)
						{
							sb.Append("<input name=\"").Append(key).Append("\" type=\"hidden\" value=\"\"/>\n");
						}
						else
						{
							for(int j = 0; j < values.Length; j++)
							{
								if(values[j] == null)
								{
									values[j] = "";
								}
								sb.Append("<input name=\"").Append(key).Append("\" type=\"hidden\" value=\"").Append(values[j].Replace("\"", "&quot;")).Append("\"/>\n");
							}
						}
					}
				}
				sb.Append("</form>");
				formString = sb.ToString();
			}
			catch
			{
			}
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="request">HttpRequest</param>
		/// <param name="page">Page&lt;T&gt;</param>
		public PageNav(HttpRequest request, Page<T> page)
		{
			NameValueCollection req = new NameValueCollection();// 用于合并get和post
			req.Add(request.QueryString);
			req.Add(request.Form);
			this.page = page;
			this.Path = request.Path;
			PageNavInit(req);
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="request">HttpRequestBase</param>
		/// <param name="page">Page&lt;T&gt;</param>
		public PageNav(HttpRequestBase request, Page<T> page)
		{
			NameValueCollection req = new NameValueCollection();// 用于合并get和post
			req.Add(request.QueryString);
			req.Add(request.Form);
			this.page = page;
			this.Path = request.Path;
			PageNavInit(req);
		}

		/// <summary>
		/// 构造函数
		/// </summary>
        /// <param name="request">HttpRequestWrapper</param>
		/// <param name="page">Page&lt;T&gt;</param>
        public PageNav(HttpRequestWrapper request, Page<T> page)
		{
			NameValueCollection req = new NameValueCollection();// 用于合并get和post
			req.Add(request.QueryString);
			req.Add(request.Form);
			this.page = page;
			this.Path = request.Path;
			PageNavInit(req);
		}

		/// <summary>
		/// 输出表单
		/// </summary>
		/// <returns>String</returns>
		public String GetForm()
		{
			if(!isOutForm)
			{
				isOutForm = true;
				return formString;
			}
			return "";
		}

		/// <summary>
		/// 显示默认的翻页效果
		/// </summary>
		/// <returns>String</returns>
		public String GetPage()
		{
			return GetPage(true, true, true, true, true);
		}

		/// <summary>
		/// 显示分页控件
		/// </summary>
		/// <param name="isViewTotal">是否显示所有记录数</param>
		/// <param name="isViewPageInfo">是否显示页面信息</param>
		/// <param name="isShowLink">是否翻页</param>
		/// <param name="isShowJump">是否支持跳转</param>
		/// <param name="isShowJumpSize">是否支持定制记录数</param>
		/// <returns>String</returns>
		public String GetPage(Boolean isViewTotal, Boolean isViewPageInfo, Boolean isShowLink, Boolean isShowJump, Boolean isShowJumpSize)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(GetForm());
            sb.Append("<div class=\"pageview\">");
			if(isViewTotal)
			{
				sb.Append(" 共").Append(page.TotalCount).Append("条记录 ");
			}
			if(isViewPageInfo)
			{
				sb.Append(" 第").Append(page.CurrentPage).Append("/").Append(page.TotalPage).Append("页 ");
			}
			if(isShowLink)
			{
				sb.Append("<a class=\"first\"" + ((page.TotalPage > 1 && page.CurrentPage > 1) ? " onclick=\"$jskey.page.go('" + page.PageName + "','1');return false;\" href=\"#\"" : "") + ">首页</a>&nbsp;");
                sb.Append("<a class=\"prev\"" + ((page.IsHasPreviousPage) ? " onclick=\"$jskey.page.go('" + page.PageName + "','" + page.PreviousPage + "');return false;\" href=\"#\"" : "") + ">上页</a>&nbsp;");
                sb.Append("<a class=\"next\"" + ((page.IsHasNextPage) ? " onclick=\"$jskey.page.go('" + page.PageName + "','" + page.NextPage + "');return false;\" href=\"#\"" : "") + ">下页</a>&nbsp;");
                sb.Append("<a class=\"last\"" + ((page.TotalPage > 1 && page.CurrentPage < page.TotalPage) ? " onclick=\"$jskey.page.go('" + page.PageName + "','" + page.TotalPage + "');return false;\" href=\"#\"" : "") + ">尾页</a>&nbsp;");
			}
            if (isShowJump || isShowJumpSize)
			{
				i = (i > 888L) ? (0L) : (i + 1);
                String pid = PAGEFORMID + page.PageName + "_go" + i;
				if(isShowJump)
				{
                    sb.Append("转到第 <input type=\"text\" class=\"input\" id=\"").Append(pid).Append("\" value=\"").Append(page.CurrentPage).Append("\"/> 页 ").Append("<input type=\"button\" class=\"go\" value=\"GO\" onclick=\"$jskey.page.go('").Append(page.PageName).Append("', document.getElementById('").Append(pid).Append("').value);\" />");
				}
				if(isShowJumpSize)
				{
					sb.Append(" <select onchange=\"$jskey.page.go('").Append(page.PageName).Append("',1,this.value);\">");
					foreach(int j in sizeArray)
					{
						sb.Append("<option value=\"").Append(j).Append((page.PageSize == j)?"\" selected=\"selected\">":"\">").Append(j).Append("</option>");
					}
					sb.Append("</select>");
				}
			}
			sb.Append("</div>");
			return sb.ToString();
		}
	}
}
