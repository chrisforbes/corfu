using System;
using System.Collections.Generic;
using System.Text;
using XmlIde.Editor;

namespace XmlIde.LanguageFramework
{
	class DummyRule : Rule
	{
		public override IEnumerable<Span> Match(string sourceText, int offset)
		{
			return null;
		}

		public DummyRule()
			: base(null)
		{
		}
	}
}
