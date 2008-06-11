using System;
using System.Collections.Generic;
using System.Text;
using XmlIde.Editor;
using System.Windows.Forms;

namespace XmlIde.LanguageFramework
{
	abstract class Rule
	{
		readonly string name;

		public string Name { get { return name; } }

		public abstract IEnumerable<Span> Match(string sourceText, int offset);
		public virtual Action GetContextAction(Checkpoint p) { return null; }

		protected Rule(string name)
		{
			this.name = name;
		}
	}
}
