using System;

namespace Dswork.Core.Util
{
	/// <summary>
	/// ID生成类
	/// </summary>
	public class UniqueId
	{
		private static long thisId = 0;
		private static readonly object o = new object();

		/// <summary>
		/// 根据时间戳产生一个唯一ID，具有防止重复机制，18位
		/// </summary>
		/// <returns>long</returns>
		public static long GenId()
		{
			lock(o)
			{
				long id = 0;
				do
				{
					id = DateTime.Now.Ticks;
				}
				while(id == thisId);
				thisId = id;
				return id;
			}
		}

		/// <summary>
		/// 返回GUID，格式00000000-0000-0000-0000-000000000000
		/// </summary>
		/// <returns>GUID</returns>
		public static String GenGuid()
		{
			return System.Guid.NewGuid().ToString();
		}
	}
}
