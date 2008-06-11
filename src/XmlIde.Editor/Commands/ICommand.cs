using System;
using System.Collections.Generic;
using System.Text;

namespace XmlIde.Editor.Commands
{
	public interface ICommand
	{
		void Do( ITextBuffer target );
		void Undo( ITextBuffer target );
	}
}
