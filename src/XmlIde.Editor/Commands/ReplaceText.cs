using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using IjwFramework.Types;

namespace XmlIde.Editor.Commands
{
	using CaretPair = Pair<Caret, Caret>;

	public class ReplaceText : ICommand
	{
		CaretPair before, after;
		string textBefore, textAfter;

		bool done;
		bool autoIndent;

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

			CommandHelper.Apply(target, before);
			target.ReplaceText(textAfter, autoIndent);
			after = CommandHelper.GetCarets(target);
			target.Mark = target.Point;
			done = true;
		}

		public void Undo(ITextBuffer target)
		{
			if (!done)
				throw new InvalidOperationException("Command is already unapplied.");

			CommandHelper.Apply(target, after);
			target.ReplaceText(textBefore, false);
			CommandHelper.Apply(target, before);
			done = false;
		}
	}
}
