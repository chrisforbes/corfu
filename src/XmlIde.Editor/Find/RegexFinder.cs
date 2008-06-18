using System.Text.RegularExpressions;
using IjwFramework.Types;

namespace XmlIde.Editor.Find
{
	public class RegexFinder : IFinder
	{
		readonly RegexOptions options;

		public RegexFinder(bool caseSensitive)
		{
			options = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
		}

		public Pair<int, int> FindNext(string source, string match)
		{
			var m = new Regex(match, options).Match(source);
			return m.Success ? m.Index.PairedWith(m.Length) : Finder.NotFound;
		}
	}
}
