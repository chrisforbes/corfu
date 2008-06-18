using System;
using System.Collections.Generic;
using IjwFramework.Types;

namespace XmlIde.Editor.Commands
{
	abstract class CompositeCommand : ICommand
	{
		protected List<ICommand> inner = new List<ICommand>();
		protected Pair<Caret, Caret> before, after;
		bool done;

		protected CompositeCommand() {}

		public void Do(ITextBuffer target)
		{
			if (done)
				throw new InvalidOperationException("already done");

			foreach (ICommand r in inner)
				r.Do(target);

			CommandHelper.Apply(target, after);
			done = true;
		}

		public void Undo(ITextBuffer target)
		{
			if (!done)
				throw new InvalidOperationException("not done");

			for (int i = inner.Count - 1; i >= 0; i--)
				inner[i].Undo(target);

			CommandHelper.Apply(target, before);

			done = false;
		}
	}
}
