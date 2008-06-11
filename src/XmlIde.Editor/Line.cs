using System;
using System.Collections.Generic;
using System.Text;
using XmlIde.Editor.Stylers;

namespace XmlIde.Editor
{
	public enum LineModification
	{
		Clean,
		Saved,
		Unsaved,
	}

	public class Line
	{
		readonly Document document;
		string text;
		IEnumerable<Span> spans; 
		LineModification dirty = LineModification.Clean;
		public object customData;

		public LineModification Dirty
		{
			get { return dirty; }
			set { dirty = value; }
		}

		public IEnumerable<Span> Spans
		{
			get { return spans; }
			set
			{
				if (value == null)
					spans = null;
				else
					spans = new List<Span>(value);
			}
		}

		public Document Document { get { return document; } }
		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				spans = null;
				dirty = LineModification.Unsaved;
				document.Dirty = true;
				document.ResetStylerPosition(Number);
			}
		}

		int cachedLineNumber;

		public int Number
		{
			get
			{
				IList<Line> lines = document.Lines;

				if (lines.Count > cachedLineNumber && lines[cachedLineNumber] == this)
					return cachedLineNumber;

				return cachedLineNumber = lines.IndexOf(this);
			}
		}

		public Line(string text, LineModification changed, Document document)
		{
			if (document == null)
				throw new ArgumentNullException("document");

			this.text = text;
			this.dirty = changed;
			this.document = document;

			if (changed == LineModification.Unsaved)
				document.Dirty = true;
		}

		public void Insert(char c, int index, int lineNumber) { Text = Text.Insert(index, "" + c); }
		public void Delete(int index, int lineNumber) { Text = Text.Remove(index, 1); }

		public int Indent
		{
			get
			{
				int i = 0;
				foreach (char c in Text)
				{
					if (!char.IsWhiteSpace(c))
						break;
					++i;
				}

				return i;
			}
		}

		public int ClampToWidth( int value )
		{
			if( value < 0 )
				return 0;
			if( value > Text.Length )
				return Text.Length;
			return value;
		}

		public void ReStyle( int lineNumber )
		{
			cachedLineNumber = lineNumber;

			document.Styler.FixTransition(document.GetLine(lineNumber - 1), this);
			spans = spans ?? new List<Span>(document.Styler.GetStyles(this));
			document.Styler.FixTransition(this, document.GetLine(lineNumber + 1));
		}
	}
}
