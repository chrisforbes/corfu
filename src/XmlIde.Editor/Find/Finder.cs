using System;
using System.Collections.Generic;
using System.Text;
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
			Pair<Caret, Caret> oldCarets = Caret.Order(document.Point, document.Mark);

			bool wasMatch = impl.FindNext(oldCarets.First.TextAfter, s).First == 0;

			document.Point = oldCarets.Second;
			document.Mark = document.Point;

			bool passedStart = false;

			while (true)
			{
				string w = document.Point.TextAfter ?? "";
				Pair<int,int> result = impl.FindNext(w, s);

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
