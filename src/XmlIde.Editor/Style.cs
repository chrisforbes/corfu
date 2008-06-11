using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace XmlIde.Editor
{
	class Style
	{
		public readonly Brush foreground, background;
		public Decoration decoration;

		public Style() { }
		public Style(Brush foreground, Brush background, Decoration decoration)
		{
			this.foreground = foreground;
			this.background = background;
			this.decoration = decoration;
		}
	}

	enum Decoration
	{
		None,
		Outline,
		RedSquiggle,
		GreenSquiggle,
		BlueSquiggle,
	}
}
