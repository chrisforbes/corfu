using System;
using System.Collections.Generic;
using IjwFramework.Types;
using System.Drawing;

namespace XmlIde.Editor
{
	public class Caret
	{
		readonly Document document;
		int line, column;

		public Document Document { get { return document; } }

		public int Column { get { return column; } }
		public int Line
		{
			get { return line; }
			internal set { line = value; }
		}

		internal Caret(Document document)
		{
			this.document = document;
		}

		internal Caret(Document document, int line, int column)
			: this(document)
		{
			this.line = line;
			this.column = column;
		}

		public static Caret AtVirtualPosition(Document d, Point location)
		{
			Caret c = Caret.AtStartOfLine(d, location.X);
			c.column = c.GetRealColumn(location.Y);
			return c;
		}

		public static Caret AtStartOfLine(Document d, int line) { return new Caret(d, line, 0); }
		public static Caret AtEndOfLine(Document d, int line) { return new Caret(d, line, d.Lines[line].Text.Length); }
		public static Caret AtStartOfDocument(Document document) { return new Caret(document, 0, 0); }
		public static Caret AtEndOfDocument(Document Document) { return AtEndOfLine(Document, Document.Lines.Count - 1); }

		internal Caret(Caret other)
			: this(other.document, other.line, other.column) {}

		public Line CurrentLine { get { return document.Lines[line]; } }

		public char CharOnRight { get { return document.Lines[line].Text[column]; } }
		public char CharOnLeft { get { return document.Lines[line].Text[column - 1]; } }

		public bool StartOfFile { get { return line <= 0 && StartOfLine; } }
		public bool EndOfFile { get { return line >= document.Lines.Count - 1 && EndOfLine; } }

		public bool StartOfLine { get { return column <= 0; } }
		public bool EndOfLine { get { return column >= document.Lines[line].Text.Length; } }

		public string TextBefore { get { return CurrentLine.Text.Substring(0, column); } }
		public string TextAfter { get { return (column == 0) ? CurrentLine.Text : CurrentLine.Text.Substring(column); } }

		internal void MoveLeft()
		{
			if (column > 0)
				--column;
			else if (line > 0)
			{
				--line;
				column = CurrentLine.Text.Length;
			}
		}

		internal void MoveRight()
		{
			if (column < CurrentLine.Text.Length)
				++column;
			else if (line < document.Lines.Count - 1)
			{
				column = 0;
				++line;
			}
		}

		internal void MoveHome()
		{
			int firstNonWhiteIndex = CurrentLine.Indent;
			column = (column == 0) ? firstNonWhiteIndex : 0;
		}

		internal void MoveEnd() { column = CurrentLine.Text.Length; }

		internal void Insert(char c, bool insertIndent)
		{
			if (c == '\r')
				return;

			if (c == '\n')
				SplitLine(insertIndent);
			else
			{
				CurrentLine.Insert(c, column, line);
				MoveRight();
			}
		}

		internal void Insert(IEnumerable<char> chars, bool insertIndent)
		{
			if (chars == null)
				return;

			foreach (char c in chars)
				Insert(c, insertIndent);
		}

		void SplitLine(bool insertIndent)
		{
			string indent = CurrentLine.Text.Substring(0, CurrentLine.Indent);
			string before = TextBefore;

			CurrentLine.Text = insertIndent ? indent + TextAfter : TextAfter;
			document.Lines.Insert(line, new Line(before, LineModification.Unsaved, document));

			++line;
			column = insertIndent ? indent.Length : 0;
		}

		int GetRealColumn(int virtualColumn)
		{
			for (int i = 0; i < CurrentLine.Text.Length; i++)
				if (virtualColumn < CurrentLine.Text.Substring(0, i + 1).ExpandTabs().Length)
					return i;

			return CurrentLine.Text.Length;
		}

		internal void MoveVertically(int delta)
		{
			int newLineIndex = line + delta;
			if (newLineIndex < 0 || newLineIndex >= document.Lines.Count)
				return;

			int effectiveIndex = Math.Min(
							TextBefore.ExpandTabs().Length,
							document.Lines[line + delta].Text.ExpandTabs().Length);
			line += delta;

			column = GetRealColumn(effectiveIndex);
		}

		internal void MoveDownStart(int delta)
		{
			int newLineIndex = line + delta;
			if (newLineIndex < 0 || newLineIndex >= document.Lines.Count)
				return;

			line += delta;
			column = 0;
		}

		internal void DeleteLeft()
		{
			if (column > 0)
				CurrentLine.Delete(--column, line);
			else if (line > 0)
			{
				string oldLine = CurrentLine.Text;
				document.Lines.RemoveAt(line--);
				column = CurrentLine.Text.Length;
				CurrentLine.Text = CurrentLine.Text + oldLine;
			}
		}

		public static bool operator ==(Caret l, Caret r)
		{
			if (object.ReferenceEquals(l, r))
				return true;

			if (object.ReferenceEquals(l, null) || object.ReferenceEquals(r, null))
				return false;

			return l.Line == r.Line && l.Column == r.Column;
		}

		public static bool operator <(Caret l, Caret r)
		{
			if (l.document != r.document)
				throw new InvalidOperationException("Comparing caret positions between two documents");

			return (l.line == r.line) ? l.column < r.column : l.line < r.line;
		}

		public static bool operator >(Caret l, Caret r) { return r < l; }

		public static Caret Max(Caret l, Caret r) { return l > r ? l : r; }
		public static Caret Min(Caret l, Caret r) { return l > r ? r : l; }

		public static Caret Max(Pair<Caret, Caret> p) { return Max(p.First, p.Second); }
		public static Caret Min(Pair<Caret, Caret> p) { return Min(p.First, p.Second); }

		public static Pair<Caret, Caret> Order(Pair<Caret, Caret> p)
		{
			return new Pair<Caret, Caret>(Min(p), Max(p));
		}

		public static Pair<Caret, Caret> Order(Caret a, Caret b)
		{
			return new Pair<Caret, Caret>(Min(a, b), Max(a, b));
		}

		public static bool operator >=(Caret l, Caret r) { return !(l < r); }
		public static bool operator <=(Caret l, Caret r) { return !(l > r); }
		public static bool operator !=(Caret l, Caret r) { return !(l == r); }

		public override int GetHashCode()
		{
			return line.GetHashCode() ^ column.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var c = obj as Caret;
			return c != null && this == c;
		}

		public string Style { get { return CurrentLine.StyleAt(Column); } }
	}
}
