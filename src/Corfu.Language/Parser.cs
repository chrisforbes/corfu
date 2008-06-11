using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corfu.Language
{
	public class Span
	{
		public readonly string Scope;
		public readonly int Start;
		public readonly int Length;

		public Span( string scope, int start, int length )
		{
			Scope = scope;
			Start = start;
			Length = length;
		}

		public override string ToString()
		{
			return "({0}..{1} -> {2})".F(Start, Start + Length, Scope);
		}

		public override bool Equals(object obj)
		{
			var s = (Span)obj;
			return (s != null) && (s.Scope == Scope) && (s.Start == Start) && (s.Length == Length);
		}

		public override int GetHashCode()
		{
			return Scope.GetHashCode() ^ Start.GetHashCode() ^ Length.GetHashCode();
		}
	}

	public class Parser
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
					yield return new Span(scope, offset, text.Length - offset);
					yield break;
				}

				if (firstMatch.m.Index > offset)
					yield return new Span(scope, offset, firstMatch.m.Index - offset);

				scope = firstMatch.p.PreEmit(scope);
				foreach (var e in firstMatch.p.GetSpans(scope, firstMatch.m))
					yield return e;
				scope = firstMatch.p.PostEmit(scope);
				offset = firstMatch.m.Index + firstMatch.m.Length;
			}
		}
	}
}
