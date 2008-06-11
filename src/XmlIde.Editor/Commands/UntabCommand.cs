using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace XmlIde.Editor.Commands
{
	class UntabCommand : ICommand
	{
		readonly string removed;
		ReplaceText replaceTextCommand;

		public UntabCommand( ITextBuffer document )
		{
			Line line = document.Point.CurrentLine;
			string text = line.Text.Substring( 0, document.Point.Column );

			int lastTabStop = 0;
			int effectiveIndex = 0;
			for( int i = 0 ; i < text.Length ; i++ )
			{
				if( !char.IsWhiteSpace( text[ i ] ))
					lastTabStop = i + 1;
				else if( ( effectiveIndex % Util.tabSize ) == 0 )
					lastTabStop = i;

				if( text[ i ] == '\t' )
					effectiveIndex += Util.tabSize - effectiveIndex % Util.tabSize;
				else
					++effectiveIndex;
			}

			removed = text.Substring( lastTabStop );
		}

		public void Do( ITextBuffer target )
		{
			if( replaceTextCommand == null )
			{
				target.MovePoint( Direction.Left, removed.Length );
				replaceTextCommand = new ReplaceText( target, "" );
			}

			replaceTextCommand.Do( target );
		}

		public void Undo(ITextBuffer target)
		{
			replaceTextCommand.Undo( target );
			target.MovePoint( Direction.Right, removed.Length );
			target.Mark = target.Point;
		}
	}
}
