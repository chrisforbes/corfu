using System.Drawing;

namespace XmlIde.Editor.Gutter
{
	class DirtyMark : IGutterElement
	{
		public void Draw(Graphics g, Point location, int height, Line line)
		{
			var dirtyBrush = GetDirtyBrush(line);
			if (dirtyBrush != null)
				g.FillRectangle(dirtyBrush, location.X, location.Y, Width, height);
		}

		Brush GetDirtyBrush(Line line)
		{
			switch (line.Dirty)
			{
				case LineModification.Unsaved: return Brushes.Yellow;
				case LineModification.Saved: return Brushes.GreenYellow;
				default: return null;
			}
		}

		public int Width { get { return 4; } }
	}
}
