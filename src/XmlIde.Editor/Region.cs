using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace XmlIde.Editor
{
	public class Region
	{
		public Caret Start, End;

		public Region(Caret start, Caret end)
		{
			Start = new Caret(Caret.Min(start, end));
			End = new Caret(Caret.Max(start, end));
		}

		public event MethodInvoker ContentChanged = delegate { };

		public bool Contains(Caret c)
		{
			return Start <= c && End >= c;
		}

		public string Content
		{
			get
			{
				if (Start.Line < End.Line)
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(Start.TextAfter);
					sb.Append('\n');

					for (int i = Start.Line + 1; i < End.Line; i++)
					{
						sb.Append(Start.Document.Lines[i].Text);
						sb.Append('\n');
					}

					sb.Append(End.TextBefore);

					return sb.ToString();
				}
				else
				{
					return Start.CurrentLine.Text.Substring(
						Start.Column, End.Column - Start.Column);
				}
			}
		}

		public override bool Equals(object obj)
		{
			Region r = obj as Region;
			return r != null && r.Start == Start && r.End == End;
		}

		public override int GetHashCode()
		{
			return Start.GetHashCode() ^ End.GetHashCode();
		}

		public Span GetSpan(Line line)
		{
			return GetSpan(line, "special.debug-region");
		}

		public Span GetSpan(Line line, string style)
		{
			int lineNumber = line.Number;

			if (Start == End)
				return null;
			if (lineNumber < Start.Line)
				return null;
			if (lineNumber > End.Line)
				return null;

			int columnStart = (lineNumber == Start.Line) ? Start.Column : 0;
			int columnEnd = (lineNumber == End.Line) ? End.Column : line.Text.Length;

			Span selection = new Span(columnStart, columnEnd - columnStart,
				style);

			return selection;
		}

		Action<Region> LambdaDoNothing() { return delegate { }; }
		Action<Region> LambdaKillRegion() { return null; }

		Action<Region> LambdaPushRegion(Region before)
		{
			int offset = Start - before.End;
			int size = End - Start;

			return delegate(Region after)
			{
				Start = new Caret(after.End);
				while (offset-- > 0)
					Start.MoveRight();

				End = new Caret(Start);
				while (size-- > 0)
					End.MoveRight();

				ContentChanged();
			};
		}

		Action<Region> LambdaClipEnd()
		{
			return delegate(Region after)
			{
				End = new Caret(after.Start);

				ContentChanged();
			};
		}

		Action<Region> LambdaClipStart(Region before)
		{
			int offset = End - before.End;

			return delegate(Region after)
			{
				Start = new Caret(after.End);
				End = new Caret(Start);

				while (offset-- > 0)
					End.MoveRight();

				ContentChanged();
			};
		}

		Action<Region> LambdaExpand(Region before)
		{
			int offset = End - before.End;

			return delegate(Region after)
			{
				End = new Caret(after.End);

				while (offset-- > 0)
					End.MoveRight();

				ContentChanged();
			};
		}

		public Action<Region> GetAdjustment(Region before)
		{
			if (before.Start >= Start && before.End <= End)
				return LambdaExpand(before);

			if (before.Start < Start && before.End >= End)
				return LambdaKillRegion();

			if (before.Start <= Start && before.End > End)
				return LambdaKillRegion();

			if (before.End <= Start)
				return LambdaPushRegion(before);

			if (before.Start < Start && before.End < End)
				return LambdaClipStart(before);

			if (before.Start < End)
				return LambdaClipEnd();

			return LambdaDoNothing();
		}
	}
}
