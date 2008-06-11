using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using XmlIde.Editor;
using System.Xml;
using System.Windows.Forms;
using System.Linq;

namespace XmlIde.LanguageFramework
{
	class StyledCapturesRule : Rule
	{
		readonly Regex regex;
		readonly string[] styles;

		public override IEnumerable<Span> Match(string sourceText, int offset)
		{
			Match m = regex.Match(sourceText, offset);
			if (m == null || !m.Success)
				return null;

			List<Span> spans = new List<Span>();
			int groupCount = Math.Min(m.Groups.Count, styles.Length);

			for (int i = 0; i < groupCount; i++)
				if (styles[i] != null)
					spans.Add(new Span(m.Groups[i].Index, m.Groups[i].Length, styles[i]));

			List<Span> resultSpans = new List<Span>();
			foreach (Span s in spans)
			{
				if (resultSpans.Count == 0)
					resultSpans.Add(s);
				else
				{
					List<Span> newResultSpans = new List<Span>();

					foreach (Span t in resultSpans)
						newResultSpans.AddRange(t.SplitWith(s));

					resultSpans = newResultSpans;
				}
			}

			resultSpans.Sort(delegate(Span a, Span b) { return a.Start - b.Start; });
			return resultSpans;
		}

		StyledCapturesRule(string pattern, string[] captureStyles, string name)
			: base( name )
		{
			regex = new Regex(pattern);
			this.styles = captureStyles;
		}

		public static Rule Create(XmlElement e, IDictionary<string, Rule> namedRules)
		{
			int[] groups = new Regex(e.GetAttribute("regex")).GetGroupNumbers();
			string[] styles = new string[1 + groups.Max()];

			foreach (int j in groups)
			{
				string style = e.GetAttribute("group" + j);
				styles[j] = string.IsNullOrEmpty(style) ? null : style;
			}

			return new StyledCapturesRule(e.GetAttribute("regex"), styles, e.GetAttribute("name"));
		}
	}
}
