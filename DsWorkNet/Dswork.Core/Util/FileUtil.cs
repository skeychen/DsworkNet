using System;
using System.Text;
using System.IO;

namespace Dswork.Core.Util
{
	/// <summary>
	/// 文件对象操作类
	/// </summary>
	public class FileUtil
	{
		/// <summary>
		/// 复制，覆盖目标，复制文件或文件夹（包括子文件夹）到目标位置
		/// </summary>
		/// <param name="fromFilePath">源文件或文件夹路径(全路径)</param>
		/// <param name="toFilePath">目标文件或文件夹路径(全路径)</param>
		public static void Copy(String fromFilePath, String toFilePath)
		{
			Copy(fromFilePath, toFilePath, true, true);
		}

		/// <summary>
		/// 复制
		/// </summary>
		/// <param name="fromFilePath">源文件或文件夹路径(全路径)</param>
		/// <param name="toFilePath">目标文件或文件夹路径(全路径)</param>
		/// <param name="overwrite">复制过程中遇到已存在的文件夹或文件是否覆盖改写</param>
		/// <param name="copySubdir">复制文件夹时，是否复制子文件夹</param>
		public static void Copy(String fromFilePath, String toFilePath, Boolean overwrite, Boolean copySubdir)
		{
			if(!Directory.Exists(fromFilePath) || !File.Exists(fromFilePath))// 不存在来源（不是文件，也不是文件夹）
			{
				return;
			}
			if((Directory.Exists(toFilePath) || File.Exists(toFilePath)) && !overwrite)// 当目标已存在，并且不能覆盖时
			{
				return;// 不能覆盖时
			}
			if(Directory.Exists(fromFilePath))
			{
				if(File.Exists(toFilePath))// 当目标已存在，保留文件夹，删除文件
				{
					Delete(toFilePath);// 删除文件，让它变成文件夹
				}
				Directory.CreateDirectory(toFilePath);
				DirectoryInfo dir = new DirectoryInfo(fromFilePath);
				foreach(FileInfo file in dir.GetFiles())// 复制所有文件
				{
					Copy(file.FullName, toFilePath + @"\" + file.Name, overwrite, copySubdir);
				}
				if(copySubdir)//是否复制子文件夹
				{
					foreach(DirectoryInfo subDir in dir.GetDirectories())// 复制所有子文件夹
					{
						Copy(subDir.FullName, toFilePath + @"\" + subDir.FullName.Replace(fromFilePath, ""), overwrite, copySubdir);
					}
				}
			}
			else
			{
				String parentFilePath = (new FileInfo(toFilePath)).DirectoryName;
				if(!Directory.Exists(parentFilePath))
				{
					Directory.CreateDirectory(parentFilePath);
				}
				if(File.Exists(toFilePath) || Directory.Exists(toFilePath))// 当目标已存在，删除文件或文件夹
				{
					Delete(toFilePath);// 递归删除文件或子文件夹，让它变成文件
				}
				(new FileInfo(fromFilePath)).CopyTo(toFilePath, true);
			}
		}

		/// <summary>
		/// 根据文件夹路径创建文件夹
		/// </summary>
		/// <param name="filePath">文件夹全路径</param>
		public static void CreateFolder(string filePath)
		{
			if(!Directory.Exists(filePath))
			{
				Directory.CreateDirectory(filePath);
			}
		}

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="filePath">删除的文件夹或文件名称(全路径)</param>
		/// <returns>成功返回true，失败返回false</returns>
		public static Boolean Delete(string filePath)
		{
			return Delete(filePath, true, true, false);
		}

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="filePath">删除的文件夹或文件名称(全路径)</param>
		/// <param name="isDeleteDir">是否删除自己（删除对象为文件夹时有效），值为false时isDeleteSubDir和isKeepStructure参数有效</param>
		/// <param name="isDeleteSubDir">是否删除子文件夹，false时仅删除当前目录下的文件，值为true是isKeepStructure参数有效</param>
		/// <param name="isKeepStructure">是否保留子文件夹的目录结构</param>
		/// <returns>成功返回true，失败返回false</returns>
		public static Boolean Delete(String filePath, Boolean isDeleteDir, Boolean isDeleteSubDir, Boolean isKeepStructure)
		{
			try
			{
				Boolean success = true;
				if(Directory.Exists(filePath))
				{
					DirectoryInfo dir = new DirectoryInfo(filePath);
					dir.Attributes = FileAttributes.Normal & FileAttributes.Directory;// 去掉文件夹只读属性
					foreach(FileInfo file in dir.GetFiles())// 删除所有文件
					{
						File.SetAttributes(file.FullName, FileAttributes.Normal);// 去掉文件只读属性
						File.Delete(file.FullName);
					}
					if(isDeleteDir)// 全部删除
					{
						foreach(DirectoryInfo subDir in dir.GetDirectories())// 删除所有子文件夹
						{
							success = Delete(subDir.FullName, true, true, false) && success;
						}
						Directory.Delete(filePath);//删除自己
					}
					else if(isDeleteSubDir)// 是否删除子目录
					{
						foreach(DirectoryInfo subDir in dir.GetDirectories())// 删除所有子文件夹
						{
							success = Delete(subDir.FullName, !isKeepStructure, true, isKeepStructure) && success;
						}
					}
				}
				else if(File.Exists(filePath))
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

		/// <summary>
		/// 取得文件后缀名
		/// </summary>
		/// <param name="filePath">文件名称(全路径)</param>
		/// <returns>返回String，文件不存在则返回null，没后缀名则返回""</returns>
		public static String GetFileExt(String filePath)
		{
			try
			{
				if(File.Exists(filePath))
				{
					String name = (new FileInfo(filePath)).Name;
					int len = name.LastIndexOf(".");
					return (len != -1) ? name.Substring(len + 1) : "";
				}
			}
			catch
			{
			}
			return null;
		}

		/// <summary>
		/// 返回不重名的名称，失败返回空字符串
		/// </summary>
		/// <param name="fileName">不包括路径的文件夹或文件名称</param>
		/// <param name="realPath">需要存放的文件夹的路径(不包括文件夹或文件名称)</param>
		/// <returns>String，返回fileName或新名称："原名称(数字)"+".原扩展名"(有扩展名则加扩展名，无则不加)</returns>
		public static String GetRefrainFileName(String fileName, String realPath)
		{
			if(fileName == null || fileName.Trim().Length == 0)
			{
				return "";
			}
			fileName = fileName.Trim();
			realPath = realPath + "/";
			int len = fileName.LastIndexOf(".");
			String f_ext = (len != -1) ? ("." + fileName.Substring(len + 1)) : "";// .*或空
			String f_name = (len != -1) ? fileName.Substring(0, len) : "";
			int count = 1;// 记数器
			String path = realPath + fileName;
			StringBuilder sb = new StringBuilder();
			while(Directory.Exists(path) || File.Exists(path))// 判断是否存在
			{
				sb.Length = 0;
				sb.Append(f_name).Append("(").Append(count).Append(")").Append(f_ext);
				count++;
				fileName = sb.ToString();
				path = realPath + fileName;// 重新读入文件对象
			}
			sb = null;
			return fileName;
		}

		/// <summary>
		/// 返回指定文件夹或文件的大小，单位bit
		/// </summary>
		/// <param name="filePath">文件名称(全路径)</param>
		/// <returns>long</returns>
		public static long GetSize(String filePath)
		{
			long size = 0;
			if(File.Exists(filePath))
			{
				size = (new FileInfo(filePath)).Length;
			}
			else if(Directory.Exists(filePath))
			{
				DirectoryInfo dir = new DirectoryInfo(filePath);
				foreach(FileInfo file in dir.GetFiles())// 删除所有文件
				{
					size += file.Length;
				}
				foreach(DirectoryInfo subDir in dir.GetDirectories())// 删除所有子文件夹
				{
					size += GetSize(subDir.FullName);
				}
			}
			return size;
		}

		/// <summary>
		/// Stream转化为byte[]
		/// </summary>
		/// <param name="inStream">输入流</param>
		/// <returns>byte[]</returns>
		public static byte[] GetToByte(Stream inStream)
		{
			try
			{
				byte[] bytes = new byte[inStream.Length];
				inStream.Read(bytes, 0, bytes.Length);
				// 设置当前流的位置为流的开始
				inStream.Seek(0, SeekOrigin.Begin);// 设置当前流的位置为流的开始
				return bytes;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// File转化为byte[](注意内存是否足够)
		/// </summary>
		/// <param name="filePath">文件名称(全路径)</param>
		/// <returns>成功返回byte[]，失败返回null</returns>
		public static byte[] GetToByte(String filePath)
		{
			try
			{
				if(File.Exists(filePath))
				{
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        byte[] bytes = new byte[(int)fileStream.Length];
                        fileStream.Read(bytes, 0, (int)bytes.Length);
						
						//BinaryReader r = new BinaryReader(fileStream);
						//r.BaseStream.Seek(0, SeekOrigin.Begin);// 设置当前流的位置为流的开始
						//byte[] bytes = r.ReadBytes((int)r.BaseStream.Length);
						
						if(fileStream != null)
						{
							fileStream.Close();
						}
                        return bytes;
                    }
				}
			}
			catch
			{
			}
			return null;
		}

		/// <summary>
		/// byte[]转化为Stream
		/// </summary>
		/// <param name="bytes">byte[]</param>
		/// <returns>Stream</returns>
		public static Stream GetToStream(byte[] bytes)
		{
			return new MemoryStream(bytes);
		}

		/// <summary>
		/// 将Stream转换为字符串
		/// </summary>
		/// <param name="inStream">输入流</param>
		/// <param name="charsetName">字符集，如：utf-8，gbk</param>
		/// <returns>String</returns>
		public static String GetToString(Stream inStream, String charsetName)
		{
			StreamReader readStream = new StreamReader(inStream, System.Text.Encoding.GetEncoding(charsetName));
			StringBuilder buffer = new StringBuilder();
			String line = "";
			while(readStream.Peek() > -1)
			{
				line = readStream.ReadLine();
				buffer.Append(line + "\n");
			}
			inStream.Seek(0, SeekOrigin.Begin);// 设置当前流的位置为流的开始
			return buffer.ToString();
		}

		/// <summary>
		/// 读取文件
		/// </summary>
		/// <param name="filePath">文件名称(全路径)</param>
		/// <param name="charsetName">字符集，如：utf-8，gbk</param>
		/// <returns>成功返回String，失败返回null</returns>
		public static String ReadFile(String filePath, String charsetName)
		{
			try
			{
				if(File.Exists(filePath))
				{
					string tmp = "";
					using(StreamReader sr = new StreamReader(filePath, System.Text.Encoding.GetEncoding(charsetName)))
					{
						tmp = sr.ReadToEnd();
						sr.Close();
					}
					return tmp;
				}
			}
			catch
			{
			}
			return null;
		}

		///**
		// * 将指定文件流写入指定的file
		// * @param filePath 文件名称(全路径)
		// * @param inStream 输入流
		// * @param overwrite 是否覆盖，目标只能是文件
		// * @return boolean 成功返回true，失败返回false
		// */
		public static Boolean WriteFile(String filePath, Stream inStream, Boolean overwrite)
		{
			String folder = Path.GetDirectoryName(filePath);// 取得当前路径下全路径（不以/结尾的无法区分是否文件夹，则取得父路径）
			CreateFolder(folder);
			if(!Directory.Exists(filePath) || !File.Exists(filePath))
			{
				String parentFilePath = (new FileInfo(filePath)).DirectoryName;
				if(!Directory.Exists(parentFilePath))
				{
					Directory.CreateDirectory(parentFilePath);// 创建所属目录
				}
			}
			else if(!overwrite || Directory.Exists(filePath))// 不可覆盖或是目录时
			{
				return false;
			}
			byte[] bytes = new byte[inStream.Length];
			inStream.Read(bytes, 0, bytes.Length);
			inStream.Seek(0, SeekOrigin.Begin);// 设置当前流的位置为流的开始
			FileStream fs = new FileStream(filePath, FileMode.Create);
			BinaryWriter bw = new BinaryWriter(fs);
			bw.Write(bytes);// 把byte[]写入文件
			bw.Close();
			fs.Close();
			return true;
		}

		/// <summary>
		/// 写入文件，存在则覆盖（目标只能是文件）
		/// </summary>
		/// <param name="filePath">文件名称(全路径)</param>
		/// <param name="content">文件内容</param>
		/// <param name="charsetName">字符集，如：utf-8，gbk</param>
		/// <returns>Boolean 成功返回true，失败返回false</returns>
		public static Boolean WriteFile(string filePath, string content, String charsetName)
		{
			return WriteFile(filePath, content, charsetName, true);
		}

		/// <summary>
		/// 写入文件
		/// </summary>
		/// <param name="filePath">文件名称(全路径)</param>
		/// <param name="content">文件内容</param>
		/// <param name="charsetName">字符集，如：utf-8，gbk</param>
		/// <param name="overwrite">是否覆盖，目标只能是文件</param>
		/// <returns>Boolean 成功返回true，失败返回false</returns>
		public static Boolean WriteFile(String filePath, String content, String charsetName, Boolean overwrite)
		{
			String folder = Path.GetDirectoryName(filePath);// 取得当前路径下全路径（不以/结尾的无法区分是否文件夹，则取得父路径）
			CreateFolder(folder);
			if(!Directory.Exists(filePath) || !File.Exists(filePath))
			{
				String parentFilePath = (new FileInfo(filePath)).DirectoryName;
				if(!Directory.Exists(parentFilePath))
				{
					Directory.CreateDirectory(parentFilePath);// 创建所属目录
				}
			}
			else if(!overwrite || Directory.Exists(filePath))// 不可覆盖或是目录时
			{
				return false;
			}
			Encoding enc = charsetName.ToLower().Equals("utf-8") ? new UTF8Encoding(false) : Encoding.GetEncoding(charsetName);
			using (StreamWriter sw = new StreamWriter(filePath, false, enc))
			{
				sw.Write(content);
				sw.Close();
			}
			return true;
		}
	}
}
/*
1、System.Text.UnicodeEncoding converter = new System.Text.UnicodeEncoding();
　　byte[] inputBytes =converter.GetBytes(inputString);
　　string inputString = converter.GetString(inputBytes);

2、string inputString = System.Convert.ToBase64String(inputBytes);
　　byte[] inputBytes = System.Convert.FromBase64String(inputString);
*/