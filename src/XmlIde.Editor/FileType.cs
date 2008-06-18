using System.Xml;

namespace XmlIde.Editor
{
	public class FileType
	{
		public readonly string Name;
		public readonly string Suffix;
		public readonly string RootScope;

		public FileType(XmlElement fileType)
		{
			Name = fileType.GetAttribute("name");
			Suffix = fileType.GetAttribute("suffix").ToLowerInvariant();
			RootScope = fileType.GetAttribute("rootscope");
		}
	}
}
