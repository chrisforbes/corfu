using System;
using System.Collections.Generic;
using System.Text;
using XmlIde.Editor.Commands;
using System.Windows.Forms;

namespace XmlIde.Editor
{
	public partial class Document
	{
		Stack<ICommand> undoStack = new Stack<ICommand>();
		Stack<ICommand> redoStack = new Stack<ICommand>();

		void Redo(ICommand c)
		{
			c.Do(this);
			undoStack.Push(c);
			UndoCapabilityChanged();
		}

		void Undo(ICommand c)
		{
			c.Undo(this);
			redoStack.Push(c);
			UndoCapabilityChanged();
		}

		public void Apply(ICommand c)
		{
			redoStack.Clear();
			Redo(c);
		}

		public event MethodInvoker UndoCapabilityChanged = delegate { };

		public void Undo()
		{
			if (CanUndo)
				Undo(undoStack.Pop());
		}

		public void Redo()
		{
			if (CanRedo)
				Redo(redoStack.Pop());
		}

		public bool CanUndo
		{
			get { return undoStack.Count > 0; }
		}

		public bool CanRedo
		{
			get { return redoStack.Count > 0; }
		}
	}
}
