using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using XmlIde.Editor;
using System.Windows.Forms;

namespace XmlIde.LanguageFramework
{
	class RefRule : Rule
	{
		readonly string targetName;
		readonly IDictionary<string, Rule> namedRules;
		Rule target;

		static Rule dummyRule = new DummyRule();

		Rule FindTarget()
		{
			Rule rule;
			if (namedRules.TryGetValue(targetName, out rule))
				return rule;

			MessageBox.Show("Failed to find target for rule named " + targetName);
			return dummyRule;
		}

		Rule Target { get { return target ?? (target = FindTarget()); } }

		RefRule(string target, string name, IDictionary<string, Rule> namedRules)
			: base( name )
		{
			this.namedRules = namedRules;
			this.targetName = target;
		}

		public static Rule Create(XmlElement e, IDictionary<string, Rule> namedRules)
		{
			return new RefRule(e.GetAttribute("target"), e.GetAttribute("name"), namedRules);
		}

		public override IEnumerable<Span> Match(string sourceText, int offset)
		{
			return Target.Match(sourceText, offset);
		}

		public override Action GetContextAction(Checkpoint p)
		{
			return Target.GetContextAction(p);
		}
	}
}
