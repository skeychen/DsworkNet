using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Timers;

using Dswork.Core.Util;

using log4net;

namespace Dswork.Core.Upload
{
	/// <summary>
	/// 临时文件上传服务类
	/// </summary>
	public class JskeyUpload
	{
		/// <summary>
		/// 用于写入log4net日志
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(typeof(JskeyUpload));

		// ################################################################################################
		// 基本配置参数
		// ################################################################################################
		private static string ToPathString(string key)
		{
			String s = EnvironmentUtil.GetToString(key, "");
			String c = s.Substring(s.Length - 1);
			if (c != "\\" && c != "/")
			{
				return s + "/";
			}
			return s;
		}

		/// <summary>
		/// 是否定时执行临时目录清理工作，建议仅web项目清理
		/// </summary>
		public static readonly Boolean UPLOAD_CLEAR = EnvironmentUtil.GetToBoolean("jskey.upload.clear", false);
		/// <summary>
		/// 临时上传总目录
		/// </summary>
		public static readonly String UPLOAD_SAVEPATH = ToPathString("jskey.upload.savePath");
		/// <summary>
		/// 文件占用最大空间（bit）
		/// </summary>
		public static readonly long UPLOAD_MAXSIZE = EnvironmentUtil.GetToLong("jskey.upload.maxSize", 10485760);
		/// <summary>
		/// 文件保存时长（毫秒）
		/// </summary>
		public static readonly long UPLOAD_TIMEOUT = EnvironmentUtil.GetToLong("jskey.upload.timeout", 600000L);
		private static readonly long UPLOAD_TIMEOUT_TICKS = JskeyUpload.UPLOAD_TIMEOUT * 10000;
		/// <summary>
		/// 默认允许上传的图片后缀
		/// </summary>
		public static readonly String UPLOAD_IMAGE = EnvironmentUtil.GetToString("jskey.upload.image", "jpg,jpeg,gif,png").ToLower();
		/// <summary>
		/// 默认允许上传的文件后缀
		/// </summary>
		public static readonly String UPLOAD_FILE = EnvironmentUtil.GetToString("jskey.upload.file", "jpg,jpeg,gif,png,bmp,doc,rtf,xls,txt,ppt,pdf,rar,zip,7z,docx,xlsx,pptx").ToLower();
		private static readonly String UPLOAD_CHECK = "," + JskeyUpload.UPLOAD_FILE + ",";

		// ################################################################################################
		// 定时任务相关
		// ################################################################################################
		private static Timer _timer = GetTimer();
		private static int count = 0;// count仅用于标记是否启动任务，1为启动，0为不启动
		private static Hashtable jskeyUploadMap = new Hashtable();
		private static Object _lock = new Object();//互斥锁
		private static int Count
		{
			get { lock (_lock) { return JskeyUpload.count; } }
			set { lock (_lock) { JskeyUpload.count = value; } }
		}
		private static Timer GetTimer()//临时目录初始化，在服务器启动时执行
		{
			if (JskeyUpload.UPLOAD_CLEAR)
			{
				JskeyUpload.Delete(JskeyUpload.UPLOAD_SAVEPATH);//删除整个目录
			}
			Timer timer = new Timer(Convert.ToDouble(JskeyUpload.UPLOAD_TIMEOUT));
			timer.Elapsed += new System.Timers.ElapsedEventHandler(delegate (Object source, ElapsedEventArgs e) { new JskeyUpload().Run(); });
			timer.AutoReset = true;
			timer.Enabled = true;
			return timer;
		}

		//判断Map是否有值存在
		private static Boolean ExistMapValue()
		{
			lock (_lock) { return JskeyUpload.jskeyUploadMap.Count > 0; }
		}

		//移除sessionKey
		private static void RemoveSessionKey(long sessionKey)
		{
			try
			{
				JskeyUpload.jskeyUploadMap.Remove(sessionKey);//移除标识
			}
			catch
			{
			}
		}

		//删除整个临时上传目录
		private static Boolean DelSessionKey(long sessionKey)
		{
			try
			{
				if (sessionKey <= 0)
				{
					return false;
				}
				JskeyUpload.Delete(JskeyUpload.UPLOAD_SAVEPATH + sessionKey + "/");
				JskeyUpload.RemoveSessionKey(sessionKey);//移除标识
				return true;
			}
			catch
			{
				JskeyUpload.RemoveSessionKey(sessionKey);//移除标识
			}
			return false;
		}

		/// <summary>
		/// 启动定时任务
		/// </summary>
		public void Run()
		{
			try
			{
				if (!JskeyUpload.UPLOAD_CLEAR)
				{
					return;
				}
				if (JskeyUpload.Count != 0)//启动中
				{
					return;
				}
				if (!JskeyUpload.ExistMapValue())
				{
					_timer.Stop();
					return;//同样不需要启动
				}
				JskeyUpload.Count = 1;//标记启动
				long currTime = DateTime.Now.Ticks;
				IDictionaryEnumerator dictenum = JskeyUpload.jskeyUploadMap.GetEnumerator();
				while (dictenum.MoveNext())
				{
					try
					{
						if ((currTime - Convert.ToInt64(dictenum.Value)) > JskeyUpload.UPLOAD_TIMEOUT_TICKS)
						{
							JskeyUpload.DelSessionKey(Convert.ToInt64(dictenum.Key));//超时,移除
						}
					}
					catch
					{
					}
				}
				if (!JskeyUpload.ExistMapValue())//让再判断一次
				{
					_timer.Stop();
					JskeyUpload.Count = 0;
					log.Debug("--临时上传目录清理程序结束，清理完毕。--");
					//Console.WriteLine("--临时上传目录清理程序结束，清理完毕。--");
				}
			}
			catch
			{
				try
				{
					_timer.Stop();
				}
				finally
				{
					JskeyUpload.Count = 0;
					log.Error("--临时上传目录清理程序异常结束。--");
					//Console.WriteLine("--临时上传目录清理程序异常结束。--");
				}
			}
		}

		// ################################################################################################
		// 文件操作相关
		// ################################################################################################
		/// <summary>
		/// 将一个文件读成byte(但需要注意文件太大时，内存是否足够)
		/// </summary>
		/// <param name="filePath">文件名称(全路径)</param>
		/// <returns>成功返回byte[]，失败返回null</returns>
		public static byte[] GetToByte(String filePath)
		{
			try
			{
				if (File.Exists(filePath))
				{
					FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

					byte[] bytes = new byte[(int)fileStream.Length];
					fileStream.Read(bytes, 0, (int)bytes.Length);
					if (fileStream != null)
					{
						fileStream.Close();
					}
					return bytes;
				}
			}
			catch
			{
			}
			return null;
		}
		//删除文件或清空文件夹
		private static Boolean Delete(String filePath)
		{
			try
			{
				Boolean success = true;
				if (Directory.Exists(filePath))
				{
					DirectoryInfo dir = new DirectoryInfo(filePath);
					dir.Attributes = FileAttributes.Normal & FileAttributes.Directory;// 去掉文件夹只读属性
					foreach (FileInfo file in dir.GetFiles())// 删除所有文件
					{
						File.SetAttributes(file.FullName, FileAttributes.Normal);// 去掉文件只读属性
						File.Delete(file.FullName);
					}
					foreach (DirectoryInfo subDir in dir.GetDirectories())// 删除所有子文件夹
					{
						success = JskeyUpload.Delete(subDir.FullName) && success; ;
					}
					Directory.Delete(filePath);//删除自己
				}
				else if (File.Exists(filePath))
				{
					File.SetAttributes(filePath, FileAttributes.Normal);// 去掉文件只读属性
					File.Delete(filePath);
				}
				return success;
			}
			catch
			{
				return false;
			}
		}

		// ################################################################################################
		// 用户上传目录下的文件操作相关
		// ################################################################################################
		/// <summary>
		/// 返回用于保存的临时目录，以"/"结尾
		/// </summary>
		/// <param name="sessionKey">用户临时主目录</param>
		/// <param name="fileKey">用户临时子目录</param>
		/// <returns>String</returns>
		public static String GetSavePath(long sessionKey, long fileKey)
		{
			return JskeyUpload.UPLOAD_SAVEPATH + sessionKey + "/" + fileKey + "/";
		}

		/// <summary>
		/// 根据参数取得上传的文件列表
		/// </summary>
		/// <param name="sessionKey">用户临时主目录</param>
		/// <param name="fileKey">用户临时子目录</param>
		/// <returns>FileInfo[]</returns>
		public static FileInfo[] GetFile(long sessionKey, long fileKey)
		{
			try
			{
				if (sessionKey <= 0 || fileKey <= 0)
				{
					return null;
				}
				String path = JskeyUpload.UPLOAD_SAVEPATH + sessionKey + "/" + fileKey;
				if (Directory.Exists(path))
				{
					return new DirectoryInfo(path).GetFiles();
				}
				else if (File.Exists(path))
				{
					File.SetAttributes(path, FileAttributes.Normal);//不正常文件，直接删除
					File.Delete(path);
				}
			}
			catch
			{
			}
			return null;
		}

		/// <summary>
		/// 删除指定的临时子目录
		/// </summary>
		/// <param name="sessionKey">用户临时主目录</param>
		/// <param name="fileKey">用户临时子目录</param>
		/// <returns>Boolean</returns>
		public static Boolean DelFile(long sessionKey, long fileKey)
		{
			try
			{
				if (sessionKey <= 0 || fileKey <= 0)
				{
					return false;
				}
				JskeyUpload.Delete(JskeyUpload.UPLOAD_SAVEPATH + sessionKey + "/" + fileKey);//删除整个目录
				return true;
			}
			catch
			{
			}
			return false;
		}

		/// <summary>
		/// 返回允许上传的后缀名字符串
		/// </summary>
		/// <param name="ext">需要过滤的上传文件后缀名，格式为"***,***"</param>
		/// <returns>String</returns>
		public static String GetUploadExt(String ext)
		{
			if (ext == null)
			{
				return JskeyUpload.UPLOAD_FILE;//默认的后缀名字符串//jpg,jpeg,gif,png,bmp,doc,rtf,xls,txt,ppt,pdf,rar,zip,7z
			}
			ext = ext.Trim().ToLower();
			if (ext.Length == 0 || ext == "file")
			{
				return JskeyUpload.UPLOAD_FILE;//默认的后缀名字符串//jpg,jpeg,gif,png,bmp,doc,rtf,xls,txt,ppt,pdf,rar,zip,7z
			}
			else if (ext == "image" || ext == "img")
			{
				return JskeyUpload.UPLOAD_IMAGE;//默认的图片后缀名字符串//jpg,jpeg,gif,png,bmp
			}
			else
			{
				String _tmp = "," + ext + ",";//前后加上","
				String[] extarr = ext.Split(',');
				foreach (String s in extarr)
				{
					String _s = "," + s + ",";//前后加上","
					if (JskeyUpload.UPLOAD_CHECK.IndexOf(_s) == -1)//用户设置的上传后缀，但需要过滤掉不在默认的后缀名字符串中的后缀名
					{
						_tmp.Replace(_s, ",");//移除非法后缀
					}
				}
				_tmp = _tmp.Substring(1, _tmp.Length - 1);//移除前后的","
				if (ext.Length == 0)
				{
					return JskeyUpload.UPLOAD_FILE;
				}
				return _tmp;
			}
		}

		/// <summary>
		/// 启动临时上传目录清理程序,在目录生成或文件上传后,才需要将sessionKey加入管理程序
		/// </summary>
		/// <param name="sessionKey">用户临时主目录</param>
		public static void ToStart(long sessionKey)
		{
			if (JskeyUpload.jskeyUploadMap.ContainsKey(sessionKey))
			{
				JskeyUpload.jskeyUploadMap.Remove(sessionKey);
			}
			JskeyUpload.jskeyUploadMap.Add(sessionKey, DateTime.Now.Ticks);//更新时间标识
			JskeyUpload._timer.Start();
			log.Debug("--临时上传目录清理程序启动，已经添加任务调度表，清理间隔" + JskeyUpload.UPLOAD_TIMEOUT / 1000 + "秒。--");
		}

		//################################################################################################
		/*
         * 简单说明：sessionKey是一次会话对应的值,根据它生成一个sessionKey目录,该目录下有多个fileKey目录,一个fileKey对应一次上传提交
         * 1 调用GetSessionKey()获取sessionKey
         * 2 页面传递一个当前session下唯一的long型fileKey
         * 3 上传前后调用ToStart(),启动定时清理程序将生成的sessionKey目录加入清理队列
         * 4 已处理的文件调用DelFile进行删除,节约服务器空间
         */
		//################################################################################################

		private static readonly string UPLOAD_KEY = "JSKEY_UPLOAD_KEY";
		private static long uploadSession = (new Random().Next(int.MaxValue));

		/// <summary>
		/// 获取一个新的上传文件会话信息
		/// </summary>
		/// <returns>long</returns>
		private static long GetNewSessionKey()
		{
			if (uploadSession >= long.MaxValue || uploadSession < 0)
			{
				uploadSession = 0L;//还原
			}
			uploadSession++;
			return uploadSession;
		}

		/// <summary>
		/// 获取sessionKey，没有则新创建一个
		/// </summary>
		/// <param name="request">HttpRequest</param>
		/// <returns>long</returns>
		public static long GetSessionKey(HttpRequest request)
		{
			return GetSessionKey(request.RequestContext.HttpContext.Session);
		}
		/// <summary>
		/// 获取sessionKey，没有则新创建一个
		/// </summary>
		/// <param name="request">HttpRequestBase</param>
		/// <returns>long</returns>
		public static long GetSessionKey(HttpRequestBase request)
		{
			return GetSessionKey(request.RequestContext.HttpContext.Session);
		}
		/// <summary>
		/// 获取sessionKey，没有则新创建一个
		/// </summary>
		/// <param name="request">HttpRequestWrapper</param>
		/// <returns>long</returns>
		public static long GetSessionKey(HttpRequestWrapper request)
		{
			return GetSessionKey(request.RequestContext.HttpContext.Session);
		}
		// 获取sessionKey，没有则新创建一个
		private static long GetSessionKey(HttpSessionStateBase session)
		{
			Object obj = session[JskeyUpload.UPLOAD_KEY];
			long key = 0L;
			if (obj != null)
			{
				try
				{
					key = Convert.ToInt64(obj);
					if (key > 0)
					{
						session[JskeyUpload.UPLOAD_KEY] = key;
						return key;
					}
				}
				catch
				{
				}
			}
			if (key > 0)
			{
				return key;
			}
			key = JskeyUpload.GetNewSessionKey();
			session[JskeyUpload.UPLOAD_KEY] = key;
			return key;
		}
	}
}
