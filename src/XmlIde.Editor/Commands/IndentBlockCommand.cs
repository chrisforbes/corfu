using IjwFramework.Types;

namespace XmlIde.Editor.Commands
{
	class IndentBlockCommand : CompositeCommand
	{
		public IndentBlockCommand(Document document)
		{
			before = CommandHelper.GetCarets(document);
			after = CommandHelper.GetCarets(document);

			after.First.MoveRight();
			after.Second.MoveRight();

			var sortedBefore = Caret.Order(before);

			int firstLine = sortedBefore.First.Line;
			int lastLine = sortedBefore.Second.Line;

			if (sortedBefore.Second.Column == 0)
			{
				--lastLine;
				Caret.Max(after).MoveLeft();
			}

			for (int i = firstLine; i <= lastLine; i++)
			{
				document.Point = document.Mark = Caret.AtStartOfLine(document, i);
				inner.Add(new ReplaceText(document, "\t"));
			}

			CommandHelper.Apply(document, before);
		}
	}
}
