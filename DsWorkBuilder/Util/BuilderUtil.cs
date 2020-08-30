using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using NVelocity.Runtime;

using Commons.Collections;

namespace Builder
{
	public class BuilderUtil
	{
		/// <summary>
		/// 向指定的文件名写入内容
		/// </summary>
		public static void WriteFile(String fileName, String content, string charset, bool hasBom)
		{
			WriteFile(fileName, content, false, charset, hasBom);
		}

		/// <summary>
		/// 写入文件
		/// </summary>
		public static void WriteFile(String fileName, String content, bool isAppend, string charset, bool hasBom)
		{
			charset = charset.ToLower();
			Encoding enc;
			if(charset.Equals("utf-8"))
				enc = new UTF8Encoding(hasBom);
			else
				enc = Encoding.GetEncoding(charset);
			CreateFolder(fileName, true);
			StreamWriter sw = new StreamWriter(fileName, isAppend, enc);
			sw.Write(content);
			sw.Close();
		}

		/// <summary>
		/// 读取文件
		/// </summary>
		public static string ReadFile(string fileName)
		{
			using(StreamReader sr = new StreamReader(fileName, Encoding.Default))
			{
				return sr.ReadToEnd();
			}
		}
		
		/// <summary>
		/// 创建目录
		/// </summary>
		public static void CreateFolder(String path, bool isFile)
		{
			if(isFile)
			{
				try
				{
					path = path.Substring(0, path.LastIndexOf("/"));
				}
				catch
				{
					try
					{
						path = path.Substring(0, path.LastIndexOf("\\"));
					}
					catch
					{
					}
				}
			}
			DirectoryInfo dir = new DirectoryInfo(path);
			if(!dir.Exists)
			{
				Directory.CreateDirectory(path);
			}
		}

		/// <summary>
		/// 根据模板解析内容.
		/// </summary>
		public static string GenByTemplate(string templatePath, string templateFile, Hashtable ht, string charset)
		{
			ExtendedProperties props = new ExtendedProperties();//使用设置初始化VelocityEngine
			props.AddProperty(RuntimeConstants.RESOURCE_LOADER, "file");
			props.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, templatePath);
			Velocity.Init(props);
			IContext context = new VelocityContext();
			if(ht != null && ht.Count > 0)
			{
				IDictionaryEnumerator dictenum = ht.GetEnumerator();
				while(dictenum.MoveNext())
				{
					String key = dictenum.Key.ToString();
					context.Put(key, dictenum.Value);
				}
			}
			Template template = Velocity.GetTemplate(templateFile, charset);
			StringWriter writer = new StringWriter();
			template.Merge(context, writer);
			Velocity.ClearProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH);// 清除掉该地址
			return writer.ToString();
		}

		/// <summary>
		/// 转为/*的模式，以/开头，不以/结尾，把\和.转为/
		/// </summary>
		/// <returns></returns>
		public static string GetToPath(string path)
		{
			path = path.Replace(".", "/").Replace("\\", "/").Replace("//", "/");
			if(path.EndsWith("/"))
			{
				path = path.Substring(0, path.Length - 1);
			}
			if (path.Length > 0 && !path.StartsWith("/"))
			{
				path = "/" + path;
			}
			return path;
		}

		public static string GetPath(string path1, string path2)
		{
			path1 = path1.Replace("\\", "/");
			path2 = path2.Replace("\\", "/");
			if(path1.EndsWith("/"))
			{
				if(path2.StartsWith("/"))
				{
					return path1 + path2.Substring(1, path2.Length - 1);
				}
				else
				{
					return path1 + path2;
				}
			}
			else
			{
				if(path2.StartsWith("/"))
				{
					return path1 + path2;
				}
				else
				{
					return path1 + "/" + path2;
				}
			}
		}

		/// <summary>
		/// 加载程序集
		/// </summary>
		public static object LoadAssembly(string dllFile, string typeName)
		{
			try
			{
				Assembly assembly = Assembly.LoadFile(dllFile);
				object obj = assembly.CreateInstance(typeName);
				return obj;
			}
			catch(Exception ex)
			{
				System.Console.WriteLine("Error:" + ex.Message);
				return null;
			}
		}

		/// <summary>
		/// 根据表名取得TableModel
		/// </summary>
		public static TableModel GetTableModel(string tableName, string dllFile, string dbTypeName, string dbUrl)
		{
			IDbModel dbModel = GetDbModel(dllFile, dbTypeName, dbUrl);
			TableModel tableModel = dbModel.GetByTable(tableName);
			return tableModel;
		}

		/// <summary>
		/// 根据model取得IDbModel
		/// </summary>
		public static IDbModel GetDbModel(string dllFile, string dbTypeName, string dbUrl)
		{
			IDbModel dbModel = null;
			string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + dllFile;
			dbModel = LoadAssembly(path, dbTypeName) as IDbModel;
			//dbModel = new SQL2005Model();
			dbModel.ConnectUrl = dbUrl;

			return dbModel;
		}
	}
}
