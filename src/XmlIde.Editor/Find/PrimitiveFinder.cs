using System;
using IjwFramework.Types;

namespace XmlIde.Editor.Find
{
	public class PrimitiveFinder : IFinder
	{
		readonly StringComparison comparison;

		public PrimitiveFinder(bool caseSensitive)
		{
			comparison = caseSensitive ?
				StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
		}

		public Pair<int, int> FindNext(string source, string match)
		{
			var index = source.IndexOf(match, comparison);
            return (index < 0) ? Finder.NotFound : Pair.New(index, match.Length);
		}
	}
}
