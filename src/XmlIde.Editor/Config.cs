using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace XmlIde.Editor
{
	public static class Config
	{
		public static string GetAbsolutePath(string resourceName)
		{
			return Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])
				+ (resourceName.StartsWith("/") || resourceName.StartsWith("\\") ? "" : @"/") + resourceName;
		}

		public static IEnumerable<string> FilesPassedOnCommandLine()
		{
			string[] args = Environment.GetCommandLineArgs();
			for (int i = 1; i < args.Length; i++)
				yield return args[i];
		}

		static Dictionary<string, FileType> fileTypes;

		static Dictionary<string, FileType> FileTypes
		{
			get
			{
				if (fileTypes == null)
				{
					fileTypes = new Dictionary<string, FileType>();
					XmlDocument doc = new XmlDocument();
					doc.Load(Config.GetAbsolutePath("languages/filetypes.xml"));
					foreach (XmlElement e in doc.SelectNodes("/filetypes/languageservice"))
						foreach (XmlElement f in e.SelectNodes("filetype"))
						{
							FileType fileType = new FileType(e, f);
							fileTypes.Add(fileType.Suffix, fileType);
						}
				}

				return fileTypes;
			}
		}

		public static FileType ChooseFileType(string filename)
		{
			FileType f;
			return FileTypes.TryGetValue(Path.GetExtension(filename).ToLowerInvariant(), out f) ? f : null;
		}

		public static string DefaultSaveLocation = 
			Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
	}
}
