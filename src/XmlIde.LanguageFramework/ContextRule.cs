using System;
using System.Collections.Generic;
using System.Text;
using XmlIde.Editor;
using System.Xml;
using System.Windows.Forms;

namespace XmlIde.LanguageFramework
{
	class ContextRule : Rule
	{
		public readonly Rule Begin, End;
		public readonly string ContentStyle;
		public readonly IEnumerable<Rule> ApplicableRules;

		static Rule[] NoRules = { };

		ContextRule(Rule begin, Rule end, IEnumerable<Rule> applicableRules, string contentStyle, string name)
			: base(name)
		{
			Begin = begin;
			End = end;
			ContentStyle = contentStyle;
			ApplicableRules = applicableRules ?? NoRules;
		}

		public override IEnumerable<Span> Match(string sourceText, int offset)
		{
			return Begin.Match(sourceText, offset);
		}

		public static Rule Create(XmlElement e, IDictionary<string, Rule> namedRules)
		{
			XmlElement beginElement = e.SelectSingleNode("./begin/*") as XmlElement;
			XmlElement endElement = e.SelectSingleNode("./end/*") as XmlElement;

			if (beginElement == null || endElement == null)
				throw new InvalidOperationException("ContextRule must have begin and end components");

			Rule beginRule = ParserFactory.CreateRule(beginElement, namedRules);
			Rule endRule = ParserFactory.CreateRule(endElement, namedRules);

			List<Rule> innerRules = new List<Rule>();
			foreach (XmlElement f in e.SelectNodes("./*"))
				if (f.Name != "begin" && f.Name != "end")
					innerRules.Add(ParserFactory.CreateRule(f, namedRules));

			return new ContextRule(beginRule, endRule, innerRules, e.GetAttribute("style"), e.GetAttribute("name"));
		}

		public override Action GetContextAction(Checkpoint p)
		{
			return p.LambdaPush(this);
		}
	}
}
