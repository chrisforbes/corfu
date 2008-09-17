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
			return target.Point.PairedWith(target.Mark);
		}

		public static Pair<T, U> Swap<T, U>(this Pair<U, T> p)
		{
			return p.Second.PairedWith(p.First);
		}
	}
}
