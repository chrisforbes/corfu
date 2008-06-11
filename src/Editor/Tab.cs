using System;
using System.Collections.Generic;
using System.Text;
using XmlIde.Editor;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Editor
{
	class Tab : IDisposable
	{
		readonly Document document;
		Rectangle bounds;
		DocumentTabStrip outer;

		public Rectangle Bounds { get { return bounds; } }
		public Document Document { get { return document; } } 

		const string fontName = "MS Sans Serif";
		internal static Font unselectedFont = new Font(fontName, 8.5f, FontStyle.Regular);
		internal static Font selectedFont = new Font(fontName, 8.5f, FontStyle.Bold);

		public Tab(Document document, DocumentTabStrip outer)
		{
			this.document = document;
			this.outer = outer;
			document.DirtyChanged += OnDocumentChanged;
			document.FilenameChanged += OnDocumentChanged;
		}

		void OnDocumentChanged()
		{
			outer.Invalidate();
			outer.DocumentFilenameChanged();
		}

		public void Paint(Graphics g, ref int x, bool selected, Rectangle clientRect)
		{
			Font f = selected ? selectedFont : unselectedFont;
			Brush tabBrush = selected ?
				SystemBrushes.ButtonHighlight : SystemBrushes.ButtonFace;

			int width = (int)g.MeasureString(document.DisplayName, f, int.MaxValue, StringFormat.GenericTypographic).Width;

			g.FillRectangle(tabBrush, x + 1, 2, width + 18, selected ? 50 : clientRect.Height - 3);
			g.DrawRectangle(SystemPens.ButtonShadow, x + 1, 2, width + 18, 50);

			g.DrawString(document.DisplayName, f, Brushes.Black, x + 10, 4, StringFormat.GenericTypographic);

			bounds = new Rectangle(x, 0, width + 20, clientRect.Height);

			x += 20 + width;
		}

		~Tab()
		{
			Dispose();
		}

		bool disposed;

		public void Dispose()
		{
			if (disposed)
				return;

			document.DirtyChanged -= OnDocumentChanged;
			document.FilenameChanged -= OnDocumentChanged;

			disposed = true;
			GC.SuppressFinalize(this);
		}
	}
}
