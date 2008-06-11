using System;
using System.Collections.Generic;
using System.Text;

namespace XmlIde.Editor
{
	class RenderCache
	{
		public readonly float Width;
		public readonly string Text;

		public RenderCache(Span span, TextGeometry geometry, Line line, int charOffset)
		{
			Width = geometry.MeasureString(line.Text.Substring(span.Start, span.Length),
				charOffset, out Text);
		}
	}
}
