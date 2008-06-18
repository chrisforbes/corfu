using IjwFramework.Types;

namespace XmlIde.Editor.Find
{
	public class Finder
	{
		readonly ITextBuffer document;
		readonly IFinder impl;
		public static readonly Pair<int, int> NotFound = new Pair<int, int>(-1, 0);

		public Finder(ITextBuffer document, IFinder impl)
		{
			this.document = document;
			this.impl = impl;
		}

		public bool FindNext(string s)
		{
			var oldCarets = Caret.Order(document.Point, document.Mark);
			var wasMatch = impl.FindNext(oldCarets.First.TextAfter, s).First == 0;
			var passedStart = false;

			document.Point = oldCarets.Second;
			document.Mark = document.Point;

			while (true)
			{
				var w = document.Point.TextAfter ?? "";
				var result = impl.FindNext(w, s);

				if (result.First >= 0)
				{
					document.MovePoint(Direction.Right, result.First);
					document.Mark = document.Point;
					document.MovePoint(Direction.Right, result.Second);
					return true;
				}
				else
				{
					document.MovePoint(Direction.DownLineStart);

					if (document.Point.Line == document.Mark.Line)
					{
						passedStart = true;
						document.MovePoint(Direction.DocumentStart);
					}

					document.Mark = document.Point;

					if (passedStart && document.Point >= oldCarets.First)
						return false;
				}
			}
		}
	}
}
