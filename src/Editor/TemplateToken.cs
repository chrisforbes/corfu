using System;
using System.Collections.Generic;
using System.Text;
using XmlIde.Editor;

namespace Editor
{
	class TemplateToken
	{
		const string DefaultTemplate = "languages/NewItemTemplates/blank.txt";
		public string Suffix { get { return suffix; } }
		readonly string template, suffix;

		public TemplateToken(string template, string suffix)
		{
			this.suffix = suffix;
			this.template = Config.GetAbsolutePath(
				string.IsNullOrEmpty(template) ? DefaultTemplate : template);
		}

		public Document CreateInstance(string filename)
		{
			Document d = new Document(template, true);
			d.FilePath = filename;

			return d;
		}
	}
}
