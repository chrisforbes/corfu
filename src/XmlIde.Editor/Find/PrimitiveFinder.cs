using System;
using System.Collections.Generic;
using System.Text;
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
			int index = source.IndexOf(match, comparison);
			if (index < 0)
				return Finder.NotFound;
			else
				return new Pair<int, int>(index, match.Length);
		}
	}
}
