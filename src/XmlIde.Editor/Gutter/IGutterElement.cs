using System.Drawing;

namespace XmlIde.Editor.Gutter
{
	interface IGutterElement
	{
		void Draw(Graphics g, Point location, int height, Line line);
		int Width { get; }
	}
}
