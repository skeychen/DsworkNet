using System;

namespace Dswork.Core.Util
{
	/// <summary>
	/// 时间操作类
	/// </summary>
	public class TimeUtil
	{
		/// <summary>
		/// 返回当前的时间，格式为：yyyy-MM-dd HH:mm:ss
		/// </summary>
		/// <returns>string</returns>
		public static String GetCurrentTime()
		{
			return GetCurrentTime("yyyy-MM-dd HH:mm:ss");
		}

		/// <summary>
		/// 返回当前的时间，格式为：yyyy-MM-dd
		/// </summary>
		/// <returns>string</returns>
		public static String GetCurrentDate()
		{
			return GetCurrentTime("yyyy-MM-dd");
		}

		/// <summary>
		/// 返回当前的时间
		/// </summary>
		/// <param name="format">需要显示的格式化参数，如：yyyy-MM-dd HH:mm:ss</param>
		/// <returns>string</returns>
		public static String GetCurrentTime(String format)
		{
			DateTime today = DateTime.Now;
			return today.ToString(format);
		}

		/// <summary>
		/// 格式化时间
		/// </summary>
		/// <param name="date">需要格式化的时间</param>
		/// <param name="format">需要显示的格式化参数，如：yyyy-MM-dd HH:mm:ss</param>
		/// <returns>string</returns>
		public static String FormatDate(DateTime date, String format)
		{
			return date.ToString(format);
		}

		/// <summary>
		/// 格式化时间
		/// </summary>
		/// <param name="value">需要格式化的时间</param>
		/// <param name="format">对应时间字符串的格式化参数，如：yyyy-MM-dd HH:mm:ss</param>
		/// <returns>DateTime，失败则返回DateTime.MinValue</returns>
		public static DateTime ConvertString(String value, String format)
		{
			try
			{
				return DateTime.ParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture);
			}
			catch
			{
				return DateTime.MinValue;
			}
		}

		/// <summary>
		/// 格式化时间，格式为：yyyy-MM-dd HH:mm:ss
		/// </summary>
		/// <param name="value">需要格式化的时间字符串，格式为：yyyy-MM-dd HH:mm:ss</param>
		/// <returns>DateTime</returns>
		public static DateTime ConvertString(String value)
		{
			return ConvertString(value, "yyyy-MM-dd HH:mm:ss");
		}

		///// <summary>
		///// 将长整形时间转日期类型
		///// </summary>
		///// <param name="value">long值的时间戳</param>
		///// <returns>DateTime</returns>
		//public static DateTime ConvertTimeInMillis(long value)
		//{
		//    return new DateTime(value);
		//}

		///// <summary>
		///// 将长整形时间按照格式化要求转时间字符串
		///// </summary>
		///// <param name="value">long值的时间戳</param>
		///// <param name="format">需要显示的格式化参数，如：yyyy-MM-dd HH:mm:ss</param>
		///// <returns>string</returns>
		//public static String ConvertTimeInMillis(long value, String format)
		//{
		//    DateTime dateTime = new DateTime(value);
		//    return FormatDate(dateTime, format);
		//}

		///// <summary>
		///// 将长整形时间转时间字符串，格式为：yyyy-MM-dd HH:mm:ss
		///// </summary>
		///// <param name="value">long值的时间戳</param>
		///// <returns>string</returns>
		//public static String ConvertDefaultTimeInMillis(long value)
		//{
		//    DateTime dateTime = new DateTime(value);
		//    return FormatDate(dateTime, "yyyy-MM-dd HH:mm:ss");
		//}
	}
}
