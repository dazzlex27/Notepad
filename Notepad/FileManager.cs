using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad
{
	class FileManager
	{
		public void SaveTextToFile(string fullpath, string text)
		{
			try
			{
				File.WriteAllText(fullpath, text);
			}
			catch
			{
				throw;
			}
		}
		public string OpenTextFromFile(string fullpath)
		{
			try
			{
				using (var sr = new StreamReader(fullpath, Encoding.Default))
				{
					return sr.ReadToEnd();
				}
			}
			catch
			{
				throw;
			}
		}

		public bool FileExists(string fullpath)
		{
			return File.Exists(fullpath);
		}
	}
}
