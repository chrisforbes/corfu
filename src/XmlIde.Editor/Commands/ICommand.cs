
namespace XmlIde.Editor.Commands
{
	public interface ICommand
	{
		void Do( ITextBuffer target );
		void Undo( ITextBuffer target );
	}
}
