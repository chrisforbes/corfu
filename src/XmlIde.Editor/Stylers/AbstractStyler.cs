using System;
using System.Collections.Generic;
using System.Text;

namespace XmlIde.Editor.Stylers
{
	public abstract class Styler
	{
		public IEnumerable<Span> GetStyles( Line line )
		{
			return GetStyles( line, line.Text.Length );
		}

		public abstract IEnumerable<Span> GetStyles( Line line, int length );

		public void FixTransition(Line first, Line second)
		{
			if (second == null)
				return;

			if (first == null || !IsValidTransition(first, second))
				second.Spans = null;
		}

		protected virtual bool IsValidTransition(Line first, Line second) { return true; }

		public virtual void Reload() { }

		public virtual string Definition { get { return null; } }
	}
}
