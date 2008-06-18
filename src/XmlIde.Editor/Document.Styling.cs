
namespace XmlIde.Editor
{
	partial class Document
	{
		int lastStyledLine;

		internal void StyleUpTo(int endLineNumber)
		{
			for (int line = lastStyledLine; line <= endLineNumber; line++)
				lines[line].ReStyle(line);

			if (lastStyledLine < endLineNumber)
				lastStyledLine = endLineNumber;
		}

		internal void ResetStylerPosition(int lineNumber)
		{
			if (lastStyledLine > lineNumber)
				lastStyledLine = lineNumber;
		}
	}
}
