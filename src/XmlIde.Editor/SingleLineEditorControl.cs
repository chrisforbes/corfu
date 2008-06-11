using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using XmlIde.Editor.Stylers;

namespace XmlIde.Editor
{
	public class SingleLineEditorControl : TextBox
	{
		public SingleLineEditorControl()
			: base()
		{
			Multiline = false;
			Font = new Font("Lucida Console", 8.5f);

			TextChanged += delegate { Invalidate(); };
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			UpdateStyles();
		}

		public override bool Multiline
		{
			get { return base.Multiline; }
			set
			{
				if (value)
					throw new InvalidOperationException("you suck. (a)bort, (r)etry, (i)gnore?");
				base.Multiline = value;
			}
		}

		ILanguageService languageService;
		AbstractStyler styler;

		public ILanguageService LanguageService
		{
			get { return languageService; }
			set
			{
				languageService = value;
				if (languageService != null)
					styler = languageService.Styler;
				if (styler == null)
					styler = new PlainTextStyler();

				Invalidate();
			}
		}

		Span GetSelectionSpan()
		{
			if (SelectionLength == 0 || SelectionStart < 0)
				return null;

			return new Span(SelectionStart, SelectionLength, SystemBrushes.HighlightText, SystemBrushes.Highlight);
		}

		static StringFormat sf = new StringFormat(StringFormat.GenericTypographic);
		
		static SingleLineEditorControl()
		{
			sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if (m.Msg == 15)
			{
				OnPaint(new PaintEventArgs(CreateGraphics(), ClientRectangle));
				m.Result = IntPtr.Zero;
				return;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.FillRectangle(SystemBrushes.Window, ClientRectangle);
			Graphics g = e.Graphics;
			Line line = new Line(Text, LineModification.Clean, null);
			line.RegenerateLineSpans( 0, styler );

			Span selection = GetSelectionSpan();

			IEnumerable<Span> spans = line.spans;
			if (selection != null)
				spans = EditorControl.ApplySelectionSpanInternal( spans, selection );

			int x = 0, y = 2;
			float offset = 2.5f;
			foreach (Span span in spans)
			{
				if (span.RenderedText == null)
					span.RenderedWidth = MeasureString(line.Text.Substring(span.Start, span.Length),
						g, x, out span.RenderedText);

				if (span.BackColor != null)
					g.FillRectangle(span.BackColor, offset, y, span.RenderedWidth, Font.Height);

				g.DrawString(span.RenderedText, Font, span.ForeColor, offset, y, sf);

				offset += span.RenderedWidth;
				x += span.RenderedText.Length;

				if (offset > ClientSize.Width)
					break;
			}
		}

		float MeasureString(string s, Graphics g)
		{
			string ignoreMe;
			return MeasureString(s, g, 0, out ignoreMe);
		}

		float MeasureString(string s, Graphics g, int startColumn, out string convertedText)
		{
			convertedText = Util.ConvertTabs(s, startColumn);
			return g.MeasureString(convertedText, Font, int.MaxValue, sf).Width;
		}

		public event EventHandler Dismissed = delegate { };
		public event EventHandler Accepted = delegate { };

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Escape)
				Dismissed(this, EventArgs.Empty);
			if (keyData == Keys.Enter)
				Accepted(this, EventArgs.Empty);
				
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
