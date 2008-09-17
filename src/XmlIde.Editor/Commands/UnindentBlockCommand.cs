using IjwFramework.Types;

namespace XmlIde.Editor.Commands
{
	class UnindentBlockCommand : CompositeCommand
	{
		public UnindentBlockCommand(Document document)
		{
			before = document.GetCarets();
			after = document.GetCarets();
			var sortedBefore = Caret.Order(before);

			for (int i = sortedBefore.First.Line; i <= sortedBefore.Second.Line; i++)
			{
				if (sortedBefore.Second.Column == 0 && sortedBefore.Second.Line == i && before.First != before.Second)
					continue;

				document.Mark = document.Point = Caret.AtStartOfLine(document, i);

				if (document.Point.EndOfLine)
					continue;

				if (char.IsWhiteSpace(document.Point.CharOnRight))
				{
					if (i == before.First.Line)
						after.First.MoveLeft();
					if (i == before.Second.Line)
						after.Second.MoveLeft();

					document.MovePoint(Direction.Right);
					inner.Add(new ReplaceText(document, ""));
				}
			}

			document.SetCarets(before);
		}
	}
}
