using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using IjwFramework.Types;

namespace XmlIde.Editor
{
	public static class Config
	{
		public static string AsAbsolute(this string resourceName)
		{
			return GetAbsolutePath(resourceName);
		}

		public static string GetAbsolutePath(string resourceName)
		{
			return Path.GetDirectoryName(Environment.GetCommandLineArgs().First())
				+ (resourceName.StartsWith("/") || resourceName.StartsWith("\\") ? "" : "/") + resourceName;
		}

		static Lazy<Dictionary<string, FileType>> fileTypes = Lazy.New(() =>
			{
				var doc = new XmlDocument();
				doc.Load(Config.GetAbsolutePath("languages/filetypes.xml"));

				return doc.SelectNodes("//filetype").Cast<XmlElement>()
					.Select(e => new FileType(e))
					.ToDictionary(x => x.Suffix);
			});

		public static FileType ChooseFileType(string filename)
		{
			return fileTypes.Value.ValueOrDefault( Path.GetExtension(filename).ToLowerInvariant(), null ) 
				?? ChooseFileType( "bogus.txt" );
		}

		public static string DefaultSaveLocation = 
			Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
	}
}
