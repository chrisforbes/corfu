using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace XmlIde.Editor
{
	delegate T Instantiator<T>();

	public class FileType
	{
		readonly string name;
		readonly string suffix;
		readonly Instantiator<ILanguageService> creator;
		ILanguageService languageService;

		public ILanguageService LanguageService
		{
			get { return languageService ?? (languageService = creator()); }
		}

		public string Name { get { return name; } }
		public string Suffix { get { return suffix; } }

		public FileType(XmlElement langService, XmlElement fileType)
		{
			name = fileType.GetAttribute("name");
			suffix = fileType.GetAttribute("suffix").ToLowerInvariant();

			creator = InitializeCreator(langService, fileType);

			if (creator == null)
				throw new InvalidOperationException("no appropriate ctor");
		}

		Instantiator<ILanguageService> InitializeCreator(XmlElement langService, XmlElement fileType)
		{
			string assembly = langService.GetAttribute("assembly");
			string typeName = langService.GetAttribute("languageservice");
			string ctorParam = fileType.GetAttribute("param");

			if (!Path.IsPathRooted(assembly))
				assembly = Config.GetAbsolutePath(assembly);
			Assembly a = Util.GetAssembly(assembly);

			Type type = a.GetType(typeName, true);

			if (!typeof(ILanguageService).IsAssignableFrom(type))
				throw new InvalidOperationException("not a ILanguageService");

			if (string.IsNullOrEmpty(ctorParam))
			{
				ConstructorInfo ci = type.GetConstructor(Type.EmptyTypes);
				if (ci != null)
					return delegate { return (ILanguageService)ci.Invoke(null); };
			}
			else
			{
				ConstructorInfo ci = type.GetConstructor(new Type[] { typeof(string) });
				if (ci != null)
					return delegate { return (ILanguageService)ci.Invoke(new object[] { ctorParam }); };
			}

			return null;
		}
	}
}
