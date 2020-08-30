using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;

namespace Dswork.Web
{
	/// <summary>
	/// 扩展Request的功能
	/// </summary>
	public class MyRequest
	{
		private NameValueCollection req = new NameValueCollection();// 用于合并get和post
		private String _Path, _RequestURI;
		private Uri _RefererURL;

		/// <summary>
		/// 初始化request
		/// </summary>
		/// <param name="request">HttpRequest</param>
		public MyRequest(HttpRequest request)
		{
			_Path = request.Path;
			_RequestURI = request.Path.Substring(request.ApplicationPath.Length);
			_RefererURL = request.UrlReferrer;
			req.Add(request.QueryString);
			req.Add(request.Form);
		}

		/// <summary>
		/// 初始化request
		/// </summary>
		/// <param name="request">HttpRequestBase</param>
		public MyRequest(HttpRequestBase request)
		{
			_Path = request.Path;
			_RequestURI = request.Path.Substring(request.ApplicationPath.Length);
			_RefererURL = request.UrlReferrer;
			req.Add(request.QueryString);
			req.Add(request.Form);
		}

		/// <summary>
		/// 初始化request
		/// </summary>
		/// <param name="request">HttpRequestWrapper</param>
		public MyRequest(HttpRequestWrapper request)
		{
			_Path = request.Path;
			_RequestURI = request.Path.Substring(request.ApplicationPath.Length);
			_RefererURL = request.UrlReferrer;
			req.Add(request.QueryString);
			req.Add(request.Form);
		}

		/// <summary>
		/// 取得当前页的URL，如有参数则带参数
		/// </summary>
		/// <returns>String</returns>
		public String GetCurrentUrl()
		{
			StringBuilder urlThisPage = new StringBuilder();
			urlThisPage.Append(_Path);
			String key;
			String[] values;
			urlThisPage.Append("?");
			for(int i = 0; i < req.Count; i++)
			{
				key = req.Keys[i];
				values = req.GetValues(key);
				for(int j = 0; i < values.Length; j++)
				{
					urlThisPage.Append(key).Append("=").Append(values[j]).Append("&");
				}
			}
			//return urlThisPage.ToString().TrimEnd(new Char[] {'&', '?'});
			return urlThisPage.ToString().Substring(0, urlThisPage.ToString().Length - 1);
		}

		/// <summary>
		/// 取得当前页的URL，如有参数则带参数，但多个同名参数以","合并为一个参数
		/// </summary>
		/// <returns>String</returns>
		public String GetCurrentUrlUniteParameter()
		{
			return GetCurrentUrlUniteParameter(",");
		}

		/// <summary>
		/// 取得当前页的URL，如有参数则带参数，但多个同名参数以separator合并为一个参数
		/// </summary>
		/// <param name="separator">分隔符</param>
		/// <returns>String</returns>
		public String GetCurrentUrlUniteParameter(String separator)
		{
			StringBuilder urlThisPage = new StringBuilder();
			urlThisPage.Append(_Path);
			String key, values;
			urlThisPage.Append("?");
			for(int i = 0; i < req.Count; i++)
			{
				key = req.Keys[i];
				values = GetStringValues(key, separator);
				urlThisPage.Append(key).Append("=").Append(values).Append("&");
			}
			return urlThisPage.ToString().Substring(0, urlThisPage.ToString().Length - 1);
		}

		/// <summary>
		/// 从Request中取得double值，如果取得的值为null，则返回0D
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <returns>double</returns>
		public double GetDouble(String key)
		{
			return GetDouble(key, 0);
		}

		/// <summary>
		/// 从Request中取得double值，如果取得的值为null，则使用默认值
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns>double</returns>
		public double GetDouble(String key, double defaultValue)
		{
			try
			{
				String str = GetParameter(key);
				return (str == null || str.Trim() == "") ? defaultValue : double.Parse(str.Trim());
			}
			catch
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// 返回数组取得多个同名参数值，如果取得的值为null，则使用默认值
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns>double[]</returns>
		public double[] GetDoubleArray(String key, double defaultValue)
		{
			try
			{
				String[] _v = req.GetValues(key);
				if(_v != null && _v.Length > 0)
				{
					double[] _numArr = new double[_v.Length];
					for(int i = 0; i < _v.Length; i++)
					{
						try
						{
							_numArr[i] = double.Parse(_v[i]);
						}
						catch
						{
							_numArr[i] = defaultValue;
						}
					}
					return _numArr;
				}
			}
			catch
			{
			}
			return new double[0];
		}

		/// <summary>
		/// 从Request中获取值并自动填充到Object
		/// </summary>
		/// <param name="o">Object</param>
		public void GetFillObject(Object o)
		{
			try
			{
				foreach (PropertyInfo properInfo in o.GetType().GetProperties())
				{
					try
					{
						properInfo.SetValue(o, Convert.ChangeType(GetParameter(properInfo.Name), properInfo.PropertyType), null);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
		/*
		/// <summary>
		/// 从request中获取文件流信息并自动填充到MyFile对象
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <returns>MyFile</returns>
		public MyFile GetFile(String key)
		{
			MyFile file = new MyFile();
			if(_isBase)
			{
				if(filesBase != null)
				{
					HttpPostedFileBase _f = filesBase["key"];
				}
			}

		}
		*/
		/// <summary>
		/// 从Request中获取值并自动填充到Object
		/// </summary>
		/// <typeparam name="T">Object</typeparam>
		/// <param name="o">T</param>
		public void GetFillObject<T>(T o)
		{
			try
			{
				foreach (PropertyInfo properInfo in o.GetType().GetProperties())
				{
					try
					{
						properInfo.SetValue(o, Convert.ChangeType(GetParameter(properInfo.Name), properInfo.PropertyType), null);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		/// <summary>
		/// 从Request中获取值并自动填充到Object
		/// </summary>
		/// <param name="o">Object</param>
		/// <param name="clazzName">request中获取类的属性key为clazzName加上属性名，即clazzName为key的前缀</param>
		public void GetFillObject(Object o, String clazzName)
		{
			try
			{
				foreach (PropertyInfo properInfo in o.GetType().GetProperties())
				{
					try
					{
						properInfo.SetValue(o, Convert.ChangeType(GetParameter(clazzName + properInfo.Name), properInfo.PropertyType), null);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		/// <summary>
		/// 从Request中获取值并自动填充到Object
		/// </summary>
		/// <typeparam name="T">Object</typeparam>
		/// <param name="o">T</param>
		/// <param name="clazzName">request中获取类的属性key为clazzName加上属性名，即clazzName为key的前缀</param>
		public void GetFillObject<T>(T o, String clazzName)
		{
			try
			{
				foreach (PropertyInfo properInfo in o.GetType().GetProperties())
				{
					try
					{
						properInfo.SetValue(o, Convert.ChangeType(GetParameter(clazzName + properInfo.Name), properInfo.PropertyType), null);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		/// <summary>
		/// 从Request中取得float值，如果取得的值为null，则返回0F
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <returns>float</returns>
		public float GetFloat(String key)
		{
			return GetFloat(key, 0);
		}

		/// <summary>
		/// 从Request中取得float值，如果取得的值为null，则使用默认值
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns>float</returns>
		public float GetFloat(String key, float defaultValue)
		{
			try
			{
				String str = GetParameter(key);
				return (str == null || str.Trim() == "") ? defaultValue : float.Parse(str.Trim());
			}
			catch
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// 返回数组取得多个同名参数值，如果取得的值为null，则使用默认值
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns>float[]</returns>
		public float[] GetFloatArray(String key, float defaultValue)
		{
			try
			{
				String[] _v = req.GetValues(key);
				if(_v != null && _v.Length > 0)
				{
					float[] _numArr = new float[_v.Length];
					for(int i = 0; i < _v.Length; i++)
					{
						try
						{
							_numArr[i] = float.Parse(_v[i]);
						}
						catch
						{
							_numArr[i] = defaultValue;
						}
					}
					return _numArr;
				}
			}
			catch
			{
			}
			return new float[0];
		}

		/// <summary>
		/// 将取得的值中的"'替换为&amp;quot; &amp;#039;，如果取得的值为null，则返回空字符串
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <returns>String</returns>
		public String GetInputValue(String key)
		{
			return GetString(key, "").Replace("\"", "&quot;").Replace("'", "&#039;");
		}

		/// <summary>
		/// 从request中取得int值，如果取得的值为null，则返回0
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <returns>int</returns>
		public int GetInt(String key)
		{
			return GetInt(key, 0);
		}

		/// <summary>
		/// 从request中取得int值，如果取得的值为null，则使用默认值
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns>int</returns>
		public int GetInt(String key, int defaultValue)
		{
			try
			{
				String str = GetParameter(key);
				return (str == null || str.Trim() == "") ? defaultValue : int.Parse(str.Trim());
			}
			catch
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// 返回数组取得多个同名参数值，如果取得的值为null，则使用默认值
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns>int[]</returns>
		public int[] GetIntArray(String key, int defaultValue)
		{
			try
			{
				String[] _v = req.GetValues(key);
				if(_v != null && _v.Length > 0)
				{
					int[] _numArr = new int[_v.Length];
					for(int i = 0; i < _v.Length; i++)
					{
						try
						{
							_numArr[i] = int.Parse(_v[i].Trim());
						}
						catch
						{
							_numArr[i] = defaultValue;
						}
					}
					return _numArr;
				}
			}
			catch
			{
			}
			return new int[0];
		}

		/// <summary>
		/// 从Request中取得long值，如果取得的值为null，则返回0L
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <returns>long</returns>
		public long GetLong(String key)
		{
			return GetLong(key, 0);
		}

		/// <summary>
		/// 从request中取得long值，如果取得的值为null，则使用默认值
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns>long</returns>
		public long GetLong(String key, long defaultValue)
		{
			try
			{
				String str = GetParameter(key);
				return (str == null || str.Trim() == "") ? defaultValue : long.Parse(str.Trim());
			}
			catch
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// 返回数组取得多个同名参数值，如果取得的值为null，则使用默认值
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns>long[]</returns>
		public long[] GetLongArray(String key, long defaultValue)
		{
			try
			{
				String[] _v = req.GetValues(key);
				if(_v != null && _v.Length > 0)
				{
					long[] _numArr = new long[_v.Length];
					for(int i = 0; i < _v.Length; i++)
					{
						try
						{
							_numArr[i] = long.Parse(_v[i].Trim());
						}
						catch
						{
							_numArr[i] = defaultValue;
						}
					}
					return _numArr;
				}
			}
			catch
			{
			}
			return new long[0];
		}

		/// <summary>
		/// 取得请求中所有的参数集合形成一个map，根据remainArray参数决定返回字符串数组或字符串
		/// </summary>
		/// <param name="remainArray">是否保留为数组，否则以","分隔成一个字符串</param>
		/// <param name="isSecure">是否过滤为安全字符</param>
		/// <returns>Hashtable </returns>
		public Hashtable GetParameterValueMap(Boolean remainArray, Boolean isSecure)
		{
			return GetParameterValueMap(remainArray, ",", isSecure);
		}

		/// <summary>
		/// 取得请求中所有的参数集合形成一个map，根据remainArray参数决定返回字符串数组或字符串
		/// </summary>
		/// <param name="remainArray">是否保留为数组，否则以separator分隔成一个字符串</param>
		/// <param name="separator">分隔符</param>
		/// <param name="isSecure">是否过滤为安全字符</param>
		/// <returns>Hashtable </returns>
		public Hashtable GetParameterValueMap(Boolean remainArray, String separator, Boolean isSecure)
		{
			Hashtable map = new Hashtable();
			String key;
			// 是否保留为数组
			if(remainArray)
			{
				for(int i = 0; i < req.Count; i++)
				{
					key = req.Keys[i];
					map.Add(key, GetStringArray(key, isSecure));
				}
			}
			else
			{
				for(int i = 0; i < req.Count; i++)
				{
					key = req.Keys[i];
					map.Add(key, GetStringValues(key, separator, isSecure));
				}
			}
			return map;
		}

		// */
		/// <summary>
		/// 取的前面页面的地址
		/// </summary>
		/// <returns>String</returns>
		public String GetRefererURL()
		{
			return _RefererURL != null ? _RefererURL.ToString() : "";
		}

		/// <summary>
		/// 取得申请的URL，不包含上下文路径
		/// </summary>
		/// <returns>String</returns>
		public String GetRequestURI()
		{
			return _RequestURI;
		}

		/// <summary>
		/// 取得安全字符串，如果取得的值为null，则返回空字符串
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <returns>String</returns>
		public String GetSecureString(String key)
		{
			return GetSecureString(key, "");
		}

		/// <summary>
		/// 取得安全字符串，如果取得的值为null，则使用默认值
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns>String</returns>
		public String GetSecureString(String key, String defaultValue)
		{
			String value = GetParameter(key);
			return (value == null) ? defaultValue : FilterInject(value);
		}

		/// <summary>
		/// 取字符串类型的参数，如果取得的值为null，则返回空字符串
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <returns>String</returns>
		public String GetString(String key)
		{
			return GetString(key, "");
		}

		/// <summary>
		/// 取字符串类型的参数，如果取得的值为null，则使用默认值
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns>String</returns>
		public String GetString(String key, String defaultValue)
		{
			String value = GetParameter(key);
			return (value == null || value == "") ? defaultValue : value;
		}

		/// <summary>
		/// 返回数组取得多个同名参数值
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <returns>String[]</returns>
		public String[] GetStringArray(String key)
		{
			try
			{
				String[] _v = req.GetValues(key);
				if(_v != null && _v.Length > 0)
				{
					return _v;
				}
			}
			catch
			{
			}
			return new String[0];
		}

		/// <summary>
		/// 返回数组取得多个同名参数值
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <param name="hasEmpty">是否去掉空值</param>
		/// <returns>String[]</returns>
		public String[] GetStringArray(String key, Boolean hasEmpty)
		{
			return GetStringArray(key, hasEmpty, false);
		}

		/// <summary>
		/// 返回数组取得多个同名参数值
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <param name="hasEmpty">是否去掉空值</param>
		/// <param name="isSecure">是否过滤为安全字符</param>
		/// <returns>String[]</returns>
		public String[] GetStringArray(String key, Boolean hasEmpty, Boolean isSecure)
		{
			if(!hasEmpty)
			{
				return GetStringArray(key);
			}
			try
			{
				String[] _v = req.GetValues(key);
				if(_v != null && _v.Length > 0)
				{
					List<String> list = new List<String>();
					for(int i = 0; i < _v.Length; i++)
					{
						if(_v[i] == null || _v[i].Trim().Length == 0)
						{
							continue;
						}
						list.Add(isSecure ? FilterInject(_v[i].Trim()) : _v[i].Trim());
					}
					_v = new String[list.Count];
					for(int i = 0; i < list.Count; i++)
					{
						_v[i] = list[i];
					}
					return _v;
				}
			}
			catch
			{
			}
			return new String[0];
		}

		/// <summary>
		/// 以","分隔符取得多个同名参数值
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <returns>String</returns>
		public String GetStringValues(String key)
		{
			return GetStringValues(key, ",", false);
		}

		/// <summary>
		/// 以分隔符取得多个同名参数值
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <param name="separator">分隔符</param>
		/// <returns>String</returns>
		public String GetStringValues(String key, String separator)
		{
			return GetStringValues(key, separator, false);
		}

		/// <summary>
		/// 以分隔符取得多个同名参数值
		/// </summary>
		/// <param name="key">request参数名</param>
		/// <param name="separator">分隔符</param>
		/// <param name="isSecure">是否过滤为安全字符</param>
		/// <returns>String</returns>
		public String GetStringValues(String key, String separator, Boolean isSecure)
		{
			StringBuilder value = new StringBuilder();
			String[] _v = req.GetValues(key);
			if(_v != null && _v.Length > 0)
			{
				if(isSecure)
				{
					value.Append(FilterInject(_v[0].Replace(separator, "")));
					for(int i = 1; i < _v.Length; i++)
					{
						value.Append(separator);
						value.Append(FilterInject(_v[i].Replace(separator, "")));
					}
				}
				else
				{
					value.Append(_v[0].Replace(separator, ""));
					for(int i = 1; i < _v.Length; i++)
					{
						value.Append(separator);
						value.Append(_v[i].Replace(separator, ""));
					}
				}
			}
			return value.ToString();
		}

		/// <summary>
		/// 去掉字符，替换;&lt;&gt;()%为双字节字符，替换'为''
		/// </summary>
		/// <param name="str">需要过滤的String</param>
		/// <returns>String</returns>
		private static String FilterInject(String str)
		{
			str = Regex.Replace(str, "\\||\\&|\\*|\\?|exec\\s|drop\\s|insert\\s|select\\s|delete\\s|update\\s|truncate\\s", "", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			str = str.Replace("'", "''");// 过滤sql字符串
			str = str.Replace(";", "；");// 过滤中断sql
			str = str.Replace("<", "＜");// 过滤脚本,iframe等html标签
			str = str.Replace(">", "＞");
			str = str.Replace("(", "（");// 处理调用数据库函数，如mid,substring,substr,chr等
			str = str.Replace(")", "）");
			str = str.Replace("%", "％");
			return str;
		}

		private String GetParameter(String key)
		{
			String[] _v = req.GetValues(key);
			if(_v != null && _v.Length > 0)
			{
				return _v[0];
			}
			else
			{
				return null;
			}
		}
	}
}
