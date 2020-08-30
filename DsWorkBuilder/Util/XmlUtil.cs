using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;

namespace Builder
{
	public class XmlUtil
	{
		static String dbFileName = "";
		static String dbTypeName = "";
		static String dbUrlName = "";
		/// <summary>
		/// 根据配置文件生成代码
		/// </summary>
		public void GenCodeByConfig(string configFilename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(configFilename);

			XmlElement root = doc.DocumentElement;//取得根
			string charset = root.SelectSingleNode("/config/charset").InnerText;//取得字符集
			string outputEncoding = root.SelectSingleNode("/config/charset").Attributes["outputEncoding"].Value;
			Console.OutputEncoding = Encoding.GetEncoding(int.Parse(outputEncoding));// 重置输出字符
			System.Console.WriteLine("OutputEncoding=" + outputEncoding);

			dbFileName = root.SelectSingleNode("/config/connect").Attributes["name"].Value;//dll连接模型
			dbTypeName = root.SelectSingleNode("/config/connect").Attributes["type"].Value;//获取数据库类型
			dbUrlName = root.SelectSingleNode("/config/connect").Attributes["url"].Value;//获取数据库连接字符

			string templatesPath = root.SelectSingleNode("/config/templates").Attributes["path"].Value;//取得模板路径
			Hashtable paramsHt = GetParams(root);//取得自定义的变量
			string type = root.SelectSingleNode("/config/builder").Attributes["templatename"].Value;//引用的模板类型
			Hashtable templateHt = GetTemplates(root, type);//取得模板
				
			string codeProjectPath = root.SelectSingleNode("/config/builder").Attributes["project"].Value;//根目录。项目目录根路径(Java) | 解决方案目录根路径(C#)
			string codeSrc = root.SelectSingleNode("/config/builder").Attributes["src"].Value;//源代码目录。src目录(Java) | 类库Lib目录(C#)
			string codeWeb = root.SelectSingleNode("/config/builder").Attributes["web"].Value;//网页程序目录。web目录(Java) | MVC项目目录(C#，首字母大写)
			string developer = root.SelectSingleNode("/config/builder").Attributes["developer"].Value;//生成者

			if (developer == "无名氏" || developer.Length < 2 || !IsChina(developer))
			{
				System.Console.WriteLine("==============================请正确填写builder中的developer=============================");
				return;
			}
			try
			{
				paramsHt.Add("developer", developer);
			}
			catch
			{
			}
			XmlNodeList codeList = root.SelectNodes("/config/builder/code");//读取配置文件的表
			if(codeList != null && codeList.Count > 0)
			{
				System.Console.WriteLine("==============================开始执行代码生成操作=============================");
				foreach(XmlNode codeNode in codeList)
				{
					string _frame = codeNode.Attributes["frame"].Value;//使用的框架。dswork或tecamo(Java) | Dswork或Tecamo(C#)
					string _namespace = codeNode.Attributes["namespace"].Value;//package或namespace前缀，用“.”分隔，有“.”则会创建出多级目录
					string _module = codeNode.Attributes["module"].Value;//模块名，用于放置代码或网页文件的上级目录名(Java) | MVC域的名称(C#)录
					string _webmodule = codeNode.Attributes["webmodule"].Value;//模块名，用于放置代码或网页文件的上级目录名(Java) | MVC域的名称(C#)
					string _table = codeNode.Attributes["table"].Value;//数据库表名，大写
					string _model = codeNode.Attributes["model"].Value;//映射数据库的模型类名，首字母大写
					string _comment = codeNode.Attributes["comment"].Value;//mybatis | hibernate | ibatisnet，当template存在comment时，如果和它不一致，则不生成该template

					try
					{
						System.Console.WriteLine("生成Code模块：表格：" + _table + "，模型：" + _model);
						GenCoding(charset, codeProjectPath, codeSrc, codeWeb,
							templatesPath, templateHt, paramsHt,
							_frame, _namespace, _module, _webmodule, _table, _model, _comment
						);
					}
					catch (Exception ex)
					{
						System.Console.WriteLine("生成Code模块错误：表格：" + _table + "，模型：" + _model + "，原因：" + ex.Message);
					}
				}
				System.Console.WriteLine("==============================结束执行代码生成操作=============================");
			}
		}
		
		private void GenCoding(string charset, string codeProjectPath, string codeSrc, string codeWeb,
			string templatesPath, Hashtable templateHt, Hashtable paramsHt,
			string _frame, string _namespace, string _module, string _webmodule, string _table, string _model, string _comment
		)
		{
			_namespace = BuilderUtil.GetToPath(_namespace);
			_module = BuilderUtil.GetToPath(_module);
			_webmodule = BuilderUtil.GetToPath(_webmodule);
			string tmp_namespace = _namespace;
			if (_namespace.StartsWith("/"))
			{
				tmp_namespace = _namespace.Substring(1, _namespace.Length - 1);
			}
			TableModel tableModel = BuilderUtil.GetTableModel(_table, dbFileName, dbTypeName, dbUrlName);
			tableModel.AddParam("Frame", _frame);
			tableModel.AddParam("Namespace", tmp_namespace.Replace("/", "."));
			tableModel.AddParam("Module", _module.Replace("/", ""));// csharp里不支持多级
			tableModel.AddParam("Javamodule", _module.Replace("/", "."));
			tableModel.AddParam("Webmodule", _webmodule);
			tableModel.AddParam("Model", _model);
			tableModel.AddParam("ModelName", _model.Substring(0, 1).ToLower() + _model.Substring(1, _model.Length - 1));

			Hashtable ht = new Hashtable();
			ht.Add("vo", tableModel);// 替换或增加vo属性
			if (paramsHt != null && paramsHt.Count > 0)
			{
				IDictionaryEnumerator dictenum = paramsHt.GetEnumerator();
				while (dictenum.MoveNext())
				{
					try
					{
						if (dictenum.Key.ToString() != "vo")
						{
							ht.Add(dictenum.Key.ToString(), dictenum.Value);
						}
					}
					catch
					{
					}
				}
			}

			if (templateHt != null && templateHt.Count > 0)
			{
				IDictionaryEnumerator dictenum = templateHt.GetEnumerator();
				while (dictenum.MoveNext())
				{
					try
					{
						String key = dictenum.Key.ToString();
						TempModel value = (TempModel)dictenum.Value;
						if (value.comment == "" || value.comment == _comment)
						{
							String _path = value.path.Replace("{src}", codeSrc).Replace("{web}", codeWeb).Replace("{module}", _module).Replace("{webmodule}", _webmodule).Replace("{model}", _model).Replace("{namespace}", _namespace.Replace(".", "/")).Replace("\\", "/").Replace("//", "/");
							System.Console.Write("    模板ID[" + value.id + "]文件：" + _path);
							string parsedStr = BuilderUtil.GenByTemplate(templatesPath, value.viewpath, ht, charset);//解析后的代码
							BuilderUtil.WriteFile(BuilderUtil.GetPath(codeProjectPath, _path), parsedStr, charset, false);//写入文件
							System.Console.WriteLine("，生成成功！");
						}
						else
						{
							System.Console.WriteLine("    模板ID[" + value.id + "]被跳过！");
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("，生成错误，原因：" + ex.Message);
					}
				}
			}
		}

		/// <summary>
		/// 取得模板
		/// </summary>
		private Hashtable GetTemplates(XmlElement root, String name)
		{
			Hashtable ht = new Hashtable();
			XmlNodeList templateList = root.SelectNodes("/config/templates/template");
			foreach(XmlNode node in templateList)
			{
				if(node.Attributes["name"].Value == name)
				{
					string id = node.Attributes["id"].Value;
					TempModel m = new TempModel();
					m.name = name;
					m.id = id;
					m.viewpath = node.Attributes["viewpath"].Value;
					m.path = node.Attributes["path"].Value;
					try
					{
						m.comment = node.Attributes["comment"].Value;
					}
					catch
					{
						m.comment = "";
					}
					ht.Add(id, m);
				}
			}
			return ht;
		}

		/// <summary>
		/// 读取用户自定义变量
		/// </summary>
		private Hashtable GetParams(XmlElement root)
		{
			Hashtable ht = new Hashtable();
			ht.Add("currentDate", DateTime.Now.ToString());//加入当前时间
			XmlNodeList varList = root.SelectNodes("/config/params/param");
			foreach(XmlNode node in varList)
			{
				XmlAttributeCollection varAttributes = node.Attributes;
				string name = varAttributes["name"].Value;
				string value = varAttributes["value"].Value;
				ht.Add(name, value);
			}
			return ht;
		}

		/// <summary>
		/// 判断是否中文
		/// </summary>
		/// <param name="CString"></param>
		/// <returns></returns>
		public bool IsChina(string s)
		{
			if(s.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < s.Length; i++)
			{
				Regex rx = new Regex("^[\u4e00-\u9fa5]$");
				if (rx.IsMatch(s.Substring(i, 1)))
				{
				}
				else
				{
					return false;
				}
			}
			return true;
		}
	}
}
