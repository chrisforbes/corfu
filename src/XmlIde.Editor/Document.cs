using System;
using System.Collections.Generic;
using IjwFramework.Types;

namespace XmlIde.Editor
{
	public partial class Document : ITextBuffer
	{
		readonly List<Line> lines = new List<Line>();
		internal IList<Line> Lines { get { return lines; } }

		public int FirstVisibleLine, FirstVisibleColumn;
		Caret point, mark;

		public Caret Point { get { return new Caret(point); } set { point = new Caret(value); } }
		public Caret Mark { get { return new Caret(mark); } set { mark = new Caret(value); } }

		public Region Selection { get { return new Region(Point, Mark); } }

		public int GetMaxLineLength()
		{
			int l = 0;
			foreach (Line line in lines)
			{
				if (line.Text.Length < l)
					continue;

				string s = line.Text.ExpandTabs();
				if (l < s.Length)
					l = s.Length;
			}

			return l;
		}

		internal int ClampLineNumber(int value)
		{
			if (value < 0)	return 0;

			if (value >= lines.Count)
				return lines.Count - 1;

			return value;
		}

		internal Line GetLine( int lineNumber )
		{
			if (lineNumber < 0 || lineNumber >= lines.Count)
				return null;

			return lines[lineNumber];
		}

		public event Action<Region> BeforeReplace = delegate { };
		public event Action<Region> AfterReplace = delegate { };

		public void ReplaceText(string newText, bool indent)
		{
			Pair<Caret, Caret> before = Caret.Order(point, mark);
			BeforeReplace(Selection);

			if (before.Second != before.First)
			{
				if (before.Second.Line > before.First.Line + 1)
				{
					lines.RemoveRange(before.First.Line + 1, before.Second.Line - before.First.Line - 1);
					before.Second.Line = before.First.Line + 1;
				}

				while (before.Second > before.First)
					before.Second.DeleteLeft();

				Mark = Point = new Caret(before.First);
			}

			point.Insert(newText, indent);

			AfterReplace(Selection);
		}

		public void MovePoint(Direction direction)
		{
			switch (direction)
			{
				case Direction.Left: point.MoveLeft(); break;
				case Direction.Right: point.MoveRight(); break;
				case Direction.Up: point.MoveVertically( -1 ); break;
				case Direction.Down: point.MoveVertically( 1 ); break;
				case Direction.DownLineStart: point.MoveDownStart( 1 ); break;
				case Direction.LineStart: point.MoveHome(); break;
				case Direction.LineEnd: point.MoveEnd(); break;
				case Direction.AbsoluteLineStart:
					point.MoveHome();
					if (point.Column > 0)
						point.MoveHome();
					break;

				case Direction.DocumentStart: point = Caret.AtStartOfDocument(this); break;
				case Direction.DocumentEnd: point = Caret.AtEndOfLine(this, lines.Count - 1);
					break;
			}
		}

		public void MovePoint(Direction direction, int count)
		{
			while (count-- > 0)
				MovePoint(direction);
		}

		public void SwapMarkAndPoint()
		{
			Caret temp = point;
			point = mark;
			mark = temp;
		}

		public IEnumerable<Span> ApplySelectionSpan(IEnumerable<Span> spans, Line line)
		{
			Region r = new Region(Point, Mark);
			Span selection = r.GetSpan(line, "special.selection");
			return (selection != null) ? selection.ApplyTo(spans) : spans;
		}

		public void ReloadStyler()
		{
			GrammarLoader.ReloadGrammar();
			foreach (Line l in lines) l.ReStyle(l.Number);
		}
	}
}
