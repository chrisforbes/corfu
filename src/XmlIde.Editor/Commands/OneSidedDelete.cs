using System;

namespace XmlIde.Editor.Commands
{
	class OneSidedDelete : ICommand
	{
		readonly Caret point;
		bool done;
		readonly bool left;

		ReplaceText replaceCommand;

		public OneSidedDelete(ITextBuffer document, bool left)
		{
			point = document.Point;
			this.left = left;
		}

		public void Do(ITextBuffer target)
		{
			if (done)
				throw new InvalidOperationException("already done");

			if (replaceCommand == null)
			{
				target.Mark = target.Point;
				target.MovePoint( left ? Direction.Left : Direction.Right );
				replaceCommand = new ReplaceText(target, null);
			}

			replaceCommand.Do(target);

			done = true;
		}

		public void Undo(ITextBuffer target)
		{
			if (!done)
				throw new InvalidOperationException("not done");

			replaceCommand.Undo(target);
			target.Point = point;

			done = false;
		}
	}
}
