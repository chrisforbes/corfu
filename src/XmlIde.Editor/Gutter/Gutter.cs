using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Reflection;

namespace XmlIde.Editor.Gutter
{
	class Gutter
	{
		ICollection<IGutterElement> elements = new List<IGutterElement>();
		int width;

		public int Width { get { return width; } }

		public void Paint(Graphics g, int y, int lineHeight, Line line)
		{
			DrawBackground(g, y, lineHeight);

			Point p = new Point(0, y);
			foreach (IGutterElement e in elements)
			{
				e.Draw(g, p, lineHeight, line);
				p.X += e.Width;
			}
		}

		void DrawBackground(Graphics g, int y, int height)
		{
			g.FillRectangle(backgroundBrush, 0, y, width, height);
			g.DrawLine(gutterEdgePen, width - 1, y, width - 1, y + height);
		}

		public void Add(IGutterElement element)
		{
			elements.Add(element);
			width += element.Width;
		}

		public void Reload()
		{
			elements.Clear();
			width = 0;

			XmlDocument doc = new XmlDocument();
			doc.Load(Config.GetAbsolutePath("/res/gutter.xml"));
			foreach (XmlElement e in doc.SelectNodes("/gutter/element[@visible=\"true\"]"))
			{
				string classPath = e.GetAttribute("class");
				Add((IGutterElement)Assembly.GetExecutingAssembly().CreateInstance(classPath));
			}
		}

		readonly Pen gutterEdgePen = new Pen(Brushes.LightGray);
		readonly Brush backgroundBrush = Brushes.White;

		public Gutter()
		{
			gutterEdgePen.DashStyle = DashStyle.Dot;
			Reload();
		}
	}
}
