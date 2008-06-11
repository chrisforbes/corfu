using System;
using System.Collections.Generic;
using System.Text;
using IjwFramework.Types;

namespace XmlIde.Editor.Commands
{
	static class CommandHelper
	{
		public static void Apply(ITextBuffer target, Pair<Caret, Caret> p)
		{
			target.Point = p.First;
			target.Mark = p.Second;
		}

		public static Pair<Caret, Caret> GetCarets(ITextBuffer target)
		{
			return new Pair<Caret, Caret>(target.Point, target.Mark);
		}
	}
}
