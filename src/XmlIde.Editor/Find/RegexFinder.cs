using System;
using System.Collections.Generic;
using System.Text;
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
			Regex regex = new Regex(match, options);

			Match m = regex.Match(source);
			if (m == null || !m.Success)
				return Finder.NotFound;

			return new Pair<int, int>(m.Index, m.Length);
		}
	}
}
