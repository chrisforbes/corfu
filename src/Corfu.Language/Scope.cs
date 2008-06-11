using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corfu.Language
{
	public class Scope
	{
		public readonly string Name;
		List<Rule> rules = new List<Rule>();

		public IEnumerable<Rule> Rules { get { return rules; } }
		public void AddRule(Rule r) { rules.Add(r); }

		public Scope(string name) { Name = name; }
	}
}
