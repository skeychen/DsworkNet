using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Dswork.Core.Util
{
	/// <summary>
	/// 读取系统properties配置文件，默认路径为："/Config/properties.config"
	/// </summary>
	public static class EnvironmentUtil
	{
		private static Hashtable ht = GetHashtable();
		private static Hashtable GetHashtable()
		{
			Hashtable ht = new Hashtable();
			XmlDocument propertiesConfig = new XmlDocument();
			XmlTextReader reader = new XmlTextReader(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Config/properties.config");
			propertiesConfig.Load(reader);
			reader.Close();
			foreach (XmlNode node in propertiesConfig.SelectNodes("*/add"))
			{
				ht.Add(node.Attributes["key"].Value, node.Attributes["value"].Value.Trim());
			}
			return ht;
		}

		/// <summary>
		/// 获得系统属性配置信息，如果没有则返回null
		/// </summary>
		/// <param name="name">属性名</param>
		/// <returns>String</returns>
		public static String GetStringProperty(String name)
		{
			try
			{
				return ht[name].ToString();
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// 获得系统属性配置信息,如果没有则返回默认值(长整型)
		/// </summary>
		/// <param name="name">属性名</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns>long</returns>
		public static long GetToLong(String name, long defaultValue)
		{
			try
			{
				return Convert.ToInt64(GetStringProperty(name));
			}
			catch
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// 获得系统属性配置信息,如果没有则返回默认值(字符串类型)
		/// </summary>
		/// <param name="name">属性名</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns>String</returns>
		public static String GetToString(String name, String defaultValue)
		{
			try
			{
				String str = GetStringProperty(name);
				if (str != null)
				{
					return str;
				}
			}
			catch
			{
			}
			return defaultValue;
		}

		/// <summary>
		/// 获得系统属性配置信息,如果没有则返回默认值("true"、"false")
		/// </summary>
		/// <param name="name">属性名</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns>Boolean</returns>
		public static Boolean GetToBoolean(String name, Boolean defaultValue)
		{
			try
			{
				String str = GetStringProperty(name);
				if (str == "true")
				{
					return true;
				}
				else if (str == "false")
				{
					return false;
				}
			}
			catch
			{
			}
			return defaultValue;
		}
	}
}
