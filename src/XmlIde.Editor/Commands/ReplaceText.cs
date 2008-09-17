using System;
using IjwFramework.Types;

namespace XmlIde.Editor.Commands
{
	using CaretPair = Pair<Caret, Caret>;

	public class ReplaceText : ICommand
	{
		CaretPair before, after;
		string textBefore, textAfter;

		bool done, autoIndent;

		public ReplaceText(ITextBuffer document, string replacement, bool autoIndent)
		{
			before = CommandHelper.GetCarets(document);
			textBefore = document.Selection.Content;
			textAfter = replacement;
			this.autoIndent = autoIndent;

			if (replacement == "\n")
			{
				Caret x = Caret.Max( before );
				char[] ws = { ' ', '\t' };

				if (x.TextAfter.IndexOfAny( ws ) == 0)
					this.autoIndent = false;
			}
		}

		public ReplaceText(ITextBuffer document, string replacement)
			: this( document, replacement, true )
		{
		}

		public void Do(ITextBuffer target)
		{
			if (done)
				throw new InvalidOperationException("Command is already applied.");

			target.SetCarets(before);
			target.ReplaceText(textAfter, autoIndent);
			after = target.GetCarets();
			target.Mark = target.Point;
			done = true;
		}

		public void Undo(ITextBuffer target)
		{
			if (!done)
				throw new InvalidOperationException("Command is already unapplied.");

			target.SetCarets(after);
			target.ReplaceText(textBefore, false);
			target.SetCarets(before);
			done = false;
		}
	}
}
