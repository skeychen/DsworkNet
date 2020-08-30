using System;

namespace Dswork.Http
{
	/// <summary>
	/// �Զ���NameValue
	/// </summary>
	public class NameValue
	{
		private String name;
		private String value;

		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="name">String</param>
		/// <param name="value">String</param>
		public NameValue(String name, String value)
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
			set { this.name = Convert.ToString(value); }
		}

		/// <summary>
		/// Value
		/// </summary>
		public String Value
		{
			get { return value; }
			set { this.value = value; }
		}
	}
}