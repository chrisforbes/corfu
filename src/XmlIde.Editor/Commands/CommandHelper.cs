using IjwFramework.Types;

namespace XmlIde.Editor.Commands
{
	public static class CommandHelper
	{
		public static void SetCarets(this ITextBuffer target, Pair<Caret, Caret> p)
		{
			target.Point = p.First;
			target.Mark = p.Second;
		}

		public static Pair<Caret, Caret> GetCarets(this ITextBuffer target)
		{
            return Pair.New(target.Point, target.Mark);
		}
	}
}
