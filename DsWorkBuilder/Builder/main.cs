using System;
using System.Collections;
using System.Text;
using System.Data;
using System.IO;

namespace Builder
{
	class main
	{
		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.GetEncoding(65001);
			XmlUtil helper = new XmlUtil();
			try
			{
				//配置文件
				string configFile = "";
				if (args.Length == 1)
				{
					configFile = args[0];
				}
				else
				{
					configFile = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Builder.xml";
				}
				//string schema = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "codegen.xsd";
				//验证文件// bool rtn = helper.ValidXml(configFile, schema);if (rtn){}
				helper.GenCodeByConfig(configFile);
			}
			catch (Exception ex)
			{
				//记录错误日志
				//string logFile = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
				//string content = "{0}:{1}\r\n";
				//content = string.Format(content, DateTime.Now.ToString(), ex.Message);
				string msg = "执行错误，原因：" + ex.Message;
				Console.WriteLine(msg);
				//CodeGenHelper.WriteFile(logFile, content, true, "UTF-8");
			}
			Console.Read();
		}

	}
}
