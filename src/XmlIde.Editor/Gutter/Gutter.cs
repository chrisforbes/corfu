using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace XmlIde.Editor.Gutter
{
	class Gutter
	{
		ICollection<IGutterElement> elements;
		public int Width { get; private set; }

		public void Paint(Graphics g, int y, int lineHeight, Line line)
		{
			DrawBackground(g, y, lineHeight);

			var p = new Point(0, y);
			foreach (var e in elements)
			{
				e.Draw(g, p, lineHeight, line);
				p.X += e.Width;
			}
		}

		void DrawBackground(Graphics g, int y, int height)
		{
			g.FillRectangle(backgroundBrush, 0, y, Width, height);
			g.DrawLine(gutterEdgePen, Width - 1, y, Width - 1, y + height);
		}

		public void Reload()
		{
			var doc = new XmlDocument();
			doc.Load("/res/gutter.xml".AsAbsolute());

			elements = doc.SelectNodes("/gutter/element[@visible=\"true\"]").Cast<XmlElement>()
				.Select(x => (IGutterElement)Assembly.GetExecutingAssembly().CreateInstance(x.GetAttribute("class")))
				.ToList();

			Width = elements.Sum(x => x.Width);
		}

		readonly Pen gutterEdgePen = new Pen(Brushes.LightGray) { DashStyle = DashStyle.Dot };
		readonly Brush backgroundBrush = Brushes.White;

		public Gutter() { Reload(); }
	}
}
