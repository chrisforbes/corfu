using System.Text;

namespace XmlIde.Editor
{
	public class Region
	{
		public readonly Caret Start, End;

		public Region(Caret start, Caret end)
		{
			Start = new Caret(Caret.Min(start, end));
			End = new Caret(Caret.Max(start, end));
		}

		public bool Contains(Caret c) {	return Start <= c && End >= c; }

		public string Content		// WTF???
		{
			get
			{
				if (Start.Line < End.Line)
				{
					var sb = new StringBuilder();
					sb.AppendLine(Start.TextAfter);

					for (int i = Start.Line + 1; i < End.Line; i++)
						sb.AppendLine(Start.Document.Lines[i].Text);

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

		public Span GetSpan(Line line, string style)
		{
			var lineNumber = line.Number;

			if (Start == End) return null;
			if (lineNumber < Start.Line) return null;
			if (lineNumber > End.Line) return null;

			var columnStart = (lineNumber == Start.Line) ? Start.Column : 0;
			var columnEnd = (lineNumber == End.Line) ? End.Column : line.Text.Length;

			var selection = new Span(columnStart, columnEnd - columnStart,
				style);

			return selection;
		}
	}
}
