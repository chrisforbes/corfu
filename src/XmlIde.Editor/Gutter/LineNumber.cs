using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace XmlIde.Editor.Gutter
{
	class LineNumber : IGutterElement
	{
		public void Draw(Graphics g, Point location, int height, Line line)
		{
			g.DrawString((line.Number + 1).ToString().PadLeft(4), font, Brushes.Gray, location.X, location.Y + 1);
		}

		Font font = new Font("Lucida Console", 8.0f);

		public int Width { get { return 34; } }
	}
}
