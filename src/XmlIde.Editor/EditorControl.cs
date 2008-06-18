using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using IjwFramework.Ui;

namespace XmlIde.Editor
{
	public partial class EditorControl : ScrollableView
	{
		Document document;
		public virtual Document Document
		{
			get { return document; }
			set
			{
				if (document == value) return;

				if (document != null)
				{
					document.UndoCapabilityChanged -= OnUndoCapabilityChanged;
					document.BeforeReplace -= InvalidateEditedRegion;
					document.AfterReplace -= InvalidateEditedRegion;
				}

				document = value;

				if( document != null )
				{
					VerticalScroll.Position = FirstVisibleLine;
					HorizontalScroll.Position = FirstVisibleColumn;
					HorizontalScroll.Maximum = document.GetMaxLineLength();
					document.UndoCapabilityChanged += OnUndoCapabilityChanged;

					document.BeforeReplace += InvalidateEditedRegion;
					document.AfterReplace += InvalidateEditedRegion;
				}

				OnUndoCapabilityChanged();
				Invalidate();
			}
		}

		void InvalidateEditedRegion(Region r)
		{
			int firstY = GetLineOffset(r.Start.Line);
			Invalidate(new Rectangle(0, firstY, ClientSize.Width, ClientSize.Height - firstY));
		}

		void InvalidateSelectedRegion(Region r)
		{
			int firstY = GetLineOffset(r.Start.Line);
			int lastY = GetLineOffset(r.End.Line + 1);	//include line's height!
			Invalidate(new Rectangle(0, firstY, ClientSize.Width, lastY - firstY));
		}

		int GetLineOffset(int line) { return (line - FirstVisibleLine) * Font.Height; }

		StyleProvider styleProvider = new StyleProvider();
		protected ViewCaret caret;

		public event Action UndoCapabilityChanged;

		void OnUndoCapabilityChanged() { if (UndoCapabilityChanged != null) UndoCapabilityChanged(); }

		int FirstVisibleLine
		{
			get { return document.FirstVisibleLine; }
			set
			{
				int dy = FirstVisibleLine - value;
				document.FirstVisibleLine = value;

				if (VerticalScroll.Position != value)
					VerticalScroll.Position = value;

				ScrollContent(0, dy * Font.Height);
			}
		}

		int FirstVisibleColumn
		{
			get { return document.FirstVisibleColumn; }
			set
			{
				int dx = FirstVisibleColumn - value;
				document.FirstVisibleColumn = value;
				Invalidate();
				HorizontalScroll.Maximum = document.GetMaxLineLength();

				if (HorizontalScroll.Position != value)
					HorizontalScroll.Position = value;
			}
		}

		TextGeometry geometry;
		Gutter.Gutter gutter;

		public event Action OnCaretMoved = () => { };

		public EditorControl()
		{
			BackColor = SystemColors.Window;
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.Selectable, true);
			UpdateStyles();

			Font = new Font("Lucida Console", 9.0f);

			caret = new ViewCaret(this);
			caret.LocationChanged += () => OnCaretMoved();
			gutter = new Gutter.Gutter();

			geometry = new TextGeometry(this);

			Cursor = Cursors.IBeam;

			SetKeyBindings();

			HorizontalScroll.Scroll += (_, e) => FirstVisibleColumn = e.NewValue;
			VerticalScroll.Scroll += (_, e) => FirstVisibleLine = e.NewValue;

			HorizontalScroll.PageSize = 10;
		}

		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);

			Form f = FindForm();
			f.Activated += (_,_2) => caret.Visible = true;
			f.Deactivate += (_,_2) => caret.Visible = false;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (document == null || document.Lines.Count == 0) return;

			VerticalScroll.Maximum = document.Lines.Count + VerticalScroll.PageSize - 2;

			document.StyleUpTo( LastVisibleLine );

			int y = 0;
			int lineHeight = Font.Height;

			foreach (var l in document.Lines.Skip(FirstVisibleLine).Take(VisibleLines))
			{
				if (y > e.ClipRectangle.Top - lineHeight)
				{
					var painter = new LinePainter(e.Graphics, styleProvider, geometry, y);

					if (document.Point.CurrentLine == l)
						painter.PaintLineBackground();

					painter.PaintLine(l, gutter, FirstVisibleColumn, document);
					gutter.Paint(e.Graphics, y, lineHeight, l);
				}

				y += lineHeight;

				if (y > this.Height || y > e.ClipRectangle.Bottom) break;
			}

			UpdateCaret();
		}

		void UpdateCaret()
		{
			if (!caret.Visible) return;

			float lineLength = document.Point.TextBefore.MeasureWith(geometry);
			caret.Location = new Point((int)lineLength + gutter.Width - (int)(geometry.CharWidth * FirstVisibleColumn),
				(document.Point.Line - FirstVisibleLine) * Font.Height);
		}

		public void EnsureVisible()
		{
			int lineNumber = document.Point.Line.Clamp(0,document.Lines.Count);

			if (lineNumber < FirstVisibleLine)
				FirstVisibleLine = lineNumber;
			else if (lineNumber > FirstVisibleLine + VisibleLines)
				FirstVisibleLine = lineNumber - VisibleLines;

			int columnNumber = document.Point.TextBefore.ExpandTabs().Length;
			if (columnNumber < FirstVisibleColumn)
				FirstVisibleColumn = columnNumber;
			else if (columnNumber > FirstVisibleColumn + VisibleColumns)
				FirstVisibleColumn = columnNumber - (int)(0.7 * VisibleColumns);
		}

		int VisibleLines { get { return ClientSize.Height / Font.Height - 1; } }
		int VisibleColumns { get { return (int)(ClientSize.Width  / geometry.CharWidth - 3); } }
		
		int LastVisibleLine
		{
			get { return (FirstVisibleLine + VisibleLines + 2).Clamp(0, document.Lines.Count - 1); }
		}
	}
}
