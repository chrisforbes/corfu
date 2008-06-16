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
	public class FileType
	{
		readonly string name;
		readonly string suffix;
		readonly string rootScope;

		public string Name { get { return name; } }
		public string Suffix { get { return suffix; } }
		public string RootScope { get { return rootScope; } }

		public FileType(XmlElement fileType)
		{
			name = fileType.GetAttribute("name");
			suffix = fileType.GetAttribute("suffix").ToLowerInvariant();
			rootScope = fileType.GetAttribute("rootscope");
		}
	}
}
