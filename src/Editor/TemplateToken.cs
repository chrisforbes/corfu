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
			this.template = (string.IsNullOrEmpty(template) ? DefaultTemplate : template).AsAbsolute();
		}

		public Document CreateInstance(string filename)
		{
			return new Document(template, true) { FilePath = filename };
		}
	}
}
