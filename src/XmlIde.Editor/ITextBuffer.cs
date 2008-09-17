using System;

namespace XmlIde.Editor
{
	public interface ITextBuffer
	{
		Caret Point { get; set; }
		Caret Mark { get; set; }
		Region Selection { get; }

		void MovePoint(Direction direction, int count);
		void MovePoint(Direction direction);
		
		void ReplaceText(string newText, bool indent);
	}
}
