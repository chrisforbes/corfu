using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace XmlIde.Editor
{
	class TextGeometry
	{
		Control control;
		Graphics g;
		StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic);
		float charWidth;
		float lineHeight;

		public TextGeometry(Control control)
		{
			this.control = control;
			g = control.CreateGraphics();
			stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

			charWidth = MeasureString("_");
			lineHeight = Font.Height;
		}

		public float LineHeight { get { return lineHeight; } }
		public float CharWidth { get { return charWidth; } }
		public Font Font { get { return control.Font; } }
		public StringFormat Format { get { return stringFormat; } }

		public float MeasureString(string s)
		{
			string ignoreMe;
			return MeasureString(s, 0, out ignoreMe);
		}

		public float MeasureString(string s, int startColumn, out string convertedText)
		{
			convertedText = s.ExpandTabs(startColumn);
			return g.MeasureString(convertedText, control.Font, int.MaxValue, stringFormat).Width;
		}

		public int GetVirtualColumn(float x, string text)
		{
			int i = 0, j = 0;
			float charWidth = CharWidth;
			foreach (char c in text)
			{
				if (c == '\t')
					while ((++i % Util.tabSize) != 0) { }
				else
					i++;

				if (x < charWidth * i)
					break;

				j = i;
			}

			return (x > (i + j) / 2 * charWidth) ? i : j;
		}
	}
}
