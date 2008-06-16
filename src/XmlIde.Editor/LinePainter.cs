using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace XmlIde.Editor
{
	class LinePainter
	{
		readonly Graphics g;
		readonly StyleProvider styleProvider;
		readonly TextGeometry geometry;
		readonly float lineOffset;

		public LinePainter( Graphics g, StyleProvider styleProvider, TextGeometry geometry, float lineOffset )
		{
			this.g = g;
			this.styleProvider = styleProvider;
			this.geometry = geometry;
			this.lineOffset = lineOffset;
		}

		public void PaintLineBackground()
		{
			Brush b = styleProvider.GetStyle("special.current-line").background;
			if (b != null)
				g.FillRectangle(b, g.ClipBounds.Left, lineOffset, g.ClipBounds.Width, geometry.LineHeight);
		}

		void PaintSpan(Span span, float offset)
		{
			Style style = styleProvider.GetStyle(span.Style);
			if (style.background != null)
				g.FillRectangle(style.background, offset, lineOffset, span.renderCache.Width, geometry.LineHeight);

			g.DrawString(span.renderCache.Text, geometry.Font, style.foreground, offset, lineOffset, geometry.Format);

			switch (style.decoration)
			{
				case Decoration.Outline:
					PaintBoxAroundSpan(span, offset);
					break;

				case Decoration.RedSquiggle:
					PaintSquigglyUnderline(span, offset, Pens.Red);
					break;

				case Decoration.GreenSquiggle:
					PaintSquigglyUnderline(span, offset, Pens.Green);
					break;

				case Decoration.BlueSquiggle:
					PaintSquigglyUnderline(span, offset, Pens.Blue);
					break;
			}
		}

		void PaintBoxAroundSpan(Span span, float offset)
		{
			using (Pen p = new Pen(Color.CadetBlue))
			{
				p.DashStyle = DashStyle.Dot;
				p.Alignment = PenAlignment.Outset;
				g.DrawRectangle(p, offset, lineOffset, span.renderCache.Width, geometry.LineHeight);
			}
		}

		void PaintSquigglyUnderline(Span span, float offset, Pen pen)
		{
			int dy = 1;
			float bl = lineOffset + geometry.LineHeight - 2.0f;

			for (var x = offset; x < offset + span.renderCache.Width; x += 2)
			{
				g.DrawLine(pen, x, bl + dy, x + 2, bl - dy);
				dy = -dy;
			}
		}

		public void PaintLine(Line line, Gutter.Gutter gutter, int firstColumn, Document doc)
		{
			var x = 0;
			var offset = gutter.Width - (firstColumn * geometry.CharWidth);
			var spans = doc.ApplySelectionSpan(line.Spans, line);

			foreach (var span in SplitSpansForRendering(spans, MaxSpanLength))
			{
				if (span.renderCache == null)
					span.renderCache = new RenderCache(span, geometry, line, x);

				if (offset + span.renderCache.Width >= 0)
					PaintSpan(span, offset);

				offset += span.renderCache.Width;
				x += span.renderCache.Text.Length;

				if (offset > g.ClipBounds.Right) break;
			}
		}

		const int MaxSpanLength = 80;

		static IEnumerable<Span> SplitSpansForRendering(IEnumerable<Span> src, int maxSpanLength)
		{
			foreach (Span s in src)
			{
				if (s.Length < maxSpanLength)
					yield return s;
				else foreach (Span t in s.ChopForRendering(maxSpanLength))
						yield return t;
			}
		}
	}
}
