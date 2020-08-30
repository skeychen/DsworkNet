using System;

using Spring.Context.Support;

namespace Dswork.Spring
{
	/// <summary>
	/// 从ApplicationContext取得spring管理的类
	/// </summary>
	public class BeanFactory
	{
		/// <summary>
		/// 取得Spring托管的类
		/// </summary>
		/// <param name="name">托管类的名称</param>
		/// <returns>Object</returns>
		public static Object GetObject(String name)
		{
			return ContextRegistry.GetContext().GetObject(name);
		}

		/// <summary>
		/// 取得Spring托管的类
		/// </summary>
		/// <typeparam name="T">Object</typeparam>
		/// <param name="name">托管类的名称</param>
		/// <returns>T</returns>
		public static T GetObject<T>(String name)
		{
			return ContextRegistry.GetContext().GetObject<T>(name);
		}
    }
}
