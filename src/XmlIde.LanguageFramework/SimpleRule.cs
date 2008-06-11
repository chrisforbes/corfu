using System;
using System.Collections.Generic;
using System.Text;
using XmlIde.Editor;
using System.Text.RegularExpressions;
using System.Xml;
using System.Windows.Forms;
using IjwFramework.Delegates;

namespace XmlIde.LanguageFramework
{
	class SimpleRule : Rule
	{
		readonly Regex regex;
		readonly string style;

		public override IEnumerable<Span> Match(string sourceText, int offset)
		{
			Match m = regex.Match(sourceText, offset);
			if (m == null || !m.Success)
				return null;

			return Iterators.Yield(new Span(m.Index, m.Length, style));
		}

		SimpleRule(string pattern, string style, string name)
			: base(name)
		{
			regex = new Regex(pattern);
			this.style = style;
		}

		public static Rule Create(XmlElement e, IDictionary<string, Rule> namedRules)
		{
			return new SimpleRule(e.GetAttribute("regex"), e.GetAttribute("style"),
				e.GetAttribute("name"));
		}
	}
}
