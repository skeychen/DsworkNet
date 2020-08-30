using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;

namespace Dswork.Http
{
	/// <summary>
	/// 自定义Cookie
	/// </summary>
	public class Cookie
	{
		private String name;
		private String value;
		private String domain;
		private DateTime expiryDate;
		private String path;
		private Boolean secure = false;//表示cookie通过https连接传送，假如是http，那么就不会被传送
		private Boolean httpOnly = false;//表示是否不允许javascript进行读取

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="name">String</param>
		/// <param name="value">String</param>
		public Cookie(String name, String value)
		{
			this.name = Convert.ToString(name);
			this.value = value;
		}

		/// <summary>
		/// Name
		/// </summary>
		public String Name
		{
			get { return name; }
			set { name = Convert.ToString(value); }
		}

		/// <summary>
		/// Value
		/// </summary>
		public String Value
		{
			get { return value; }
			set { this.value = Convert.ToString(value); }
		}

		/// <summary>
		/// Domain
		/// </summary>
		public String Domain
		{
			get { return domain; }
			set { if (value == null) { domain = null; } else { domain = Convert.ToString(value).ToLower(); } }
		}

		/// <summary>
		/// ExpiryDate
		/// </summary>
		public DateTime ExpiryDate
		{
			get { return expiryDate; }
			set { expiryDate = value; }
		}

		/// <summary>
		/// Path
		/// </summary>
		public String Path
		{
			get { return path; }
			set { path = value; }
		}

		/// <summary>
		/// Secure
		/// </summary>
		public Boolean Secure
		{
			get { return secure; }
			set { secure = value; }
		}

		/// <summary>
		/// HttpOnly
		/// </summary>
		public Boolean HttpOnly
		{
			get { return httpOnly; }
			set { httpOnly = value; }
		}

		/// <summary>
		/// 判断是否过期
		/// </summary>
		/// <param name="date">DateTime</param>
		/// <returns>Boolean</returns>
		public Boolean IsExpired(DateTime date)
		{
			return (this.expiryDate != null && this.expiryDate.Ticks != 0 && this.expiryDate.Ticks <= date.Ticks);
		}

		/// <summary>
		/// ToString
		/// </summary>
		public override String ToString()
		{
			StringBuilder buffer = new StringBuilder();
			//		buffer.Append("[version: ");
			//		buffer.Append(Integer.toString(this.cookieVersion));
			//		buffer.Append("]");
			buffer.Append("[name: ");
			buffer.Append(this.name);
			buffer.Append("]");
			buffer.Append("[value: ");
			buffer.Append(this.value);
			buffer.Append("]");
			buffer.Append("[domain: ");
			buffer.Append(this.domain);
			buffer.Append("]");
			buffer.Append("[path: ");
			buffer.Append(this.path);
			buffer.Append("]");
			buffer.Append("[expiry: ");
			buffer.Append(this.expiryDate);
			buffer.Append("]");
			return buffer.ToString();
		}

		/// <summary>
		/// 复制为新对象
		/// </summary>
		public Cookie Clone()
		{
			Cookie c = new Cookie(name, value);
			c.Path = Path;
			c.Secure = Secure;
			c.HttpOnly = HttpOnly;
			c.Domain = Domain;
			c.ExpiryDate = ExpiryDate;
			return c;
		}
	}
}
