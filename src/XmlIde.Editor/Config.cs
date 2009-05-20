using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using IjwFramework.Types;

namespace XmlIde.Editor
{
	public static class Config
	{
		public static string AsAbsolute(this string resourceName)
		{
			return Path.GetDirectoryName(Environment.GetCommandLineArgs().First())
				+ (resourceName.StartsWith("/") || resourceName.StartsWith("\\") ? "" : "/") + resourceName;
		}

		static Lazy<Dictionary<string, FileType>> fileTypes = Lazy.New(() =>
			{
				var doc = new XmlDocument();
				doc.Load("languages/filetypes.xml".AsAbsolute());

				return doc.SelectNodes("//filetype").Cast<XmlElement>()
					.Select(e => new FileType(e))
					.ToDictionary(x => x.Suffix);
			});

		public static FileType ChooseFileType(string filename)
		{
			return fileTypes.Value.ValueOrDefault( Path.GetExtension(filename).ToLowerInvariant(), null ) 
				?? ChooseFileType( "bogus.txt" );
		}

		public static IEnumerable<FileType> FileTypes
		{
			get { return fileTypes.Value.Values; }
		}

		public static string DefaultSaveLocation = 
			Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
	}
}
