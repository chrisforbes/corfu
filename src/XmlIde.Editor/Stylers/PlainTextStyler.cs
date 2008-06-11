using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace XmlIde.Editor.Stylers
{
	public class PlainTextStyler : Styler, ILanguageService
	{
		public override IEnumerable<Span> GetStyles( Line line, int length )
		{
			yield return new Span(0, length);
		}

		public Styler Styler { get { return this; } }

		static ILanguageService instance;
		public static ILanguageService Instance
		{
			get { return instance ?? (instance = new PlainTextStyler()); }
		}
	}
}
