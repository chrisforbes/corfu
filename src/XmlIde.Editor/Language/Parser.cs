using System.Collections.Generic;
using System.Linq;
using XmlIde.Editor;

namespace Corfu.Language
{
	class Parser
	{
		string scope;
		Grammar g;

		public string Scope { get { return scope; } }

		public Parser(Grammar g, string scope)
		{
			this.g = g;
			this.scope = scope;
		}

		public IEnumerable<Span> ParseSpans(string text)
		{
			int offset = 0;
			while (offset < text.Length)
			{
				var rules = g.GetRules(scope);
				var firstMatch = rules
					.Select(x => new { p = x, m = x.Pattern.Match(text, offset) })
					.Where(x => x.m.Success)
					.OrderBy(x => x.m.Index)
					.FirstOrDefault();

				if (firstMatch == null)
				{
					yield return new Span(offset, text.Length - offset, scope);
					yield break;
				}

				if (firstMatch.m.Index > offset)
					yield return new Span(offset, firstMatch.m.Index - offset, scope);

				scope = firstMatch.p.PreEmit(scope);
				foreach (var e in firstMatch.p.GetSpans(scope, firstMatch.m))
					yield return e;
				scope = firstMatch.p.PostEmit(scope);
				offset = firstMatch.m.Index + firstMatch.m.Length;
			}
		}
	}
}
