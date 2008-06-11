using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using IjwFramework.Delegates;

namespace XmlIde.LanguageFramework
{
	using RuleCreator = Provider<Rule, XmlElement, IDictionary<string, Rule>>;

	class ParserFactory
	{
		public static Parser Load(string filename)
		{
			IDictionary<string, Rule> namedRules = new Dictionary<string, Rule>();

			XmlDocument doc = new XmlDocument();
			doc.Load(filename);

			string defaultStyle = ((XmlElement)doc.SelectSingleNode("/language")).GetAttribute("defaultStyle");
			Parser p = new Parser(defaultStyle);

			foreach (XmlElement e in doc.SelectNodes("/language/*"))
				if (e.Name != "repository")
					p.AddRule(CreateRule(e, namedRules));

			foreach (XmlElement e in doc.SelectNodes("/language/repository/*"))
				CreateRule(e, namedRules);

			return p;
		}

		internal static Rule CreateRule(XmlElement e, IDictionary<string, Rule> namedRules)
		{
			Dictionary<string, RuleCreator> loaders = new Dictionary<string, RuleCreator>();

			loaders.Add("pattern", SimpleRule.Create);
			loaders.Add("capturePattern", StyledCapturesRule.Create);
			loaders.Add("ref", RefRule.Create);
			loaders.Add("contextPattern", ContextRule.Create);

			RuleCreator creator;
			if (!loaders.TryGetValue(e.Name, out creator))
				throw new ArgumentException("unrecognised element: " + e.Name);

			Rule rule = creator(e, namedRules);

			if (!string.IsNullOrEmpty(rule.Name))
				namedRules.Add(rule.Name, rule);

			return rule;
		}
	}
}
