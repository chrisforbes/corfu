using System;
using System.Collections.Generic;
using System.Text;

namespace XmlIde.Editor
{
	interface ILineNavigator
	{
		bool AtEnd { get; }
		char NextChar { get; }
		void Move();
	}

	class ForwardLineNavigator : ILineNavigator
	{
		Document document;
		public ForwardLineNavigator(Document document) { this.document = document; }

		public bool AtEnd { get { return document.Point.EndOfLine; } }
		public char NextChar { get { return document.Point.CharOnRight; } }
		public void Move() { document.MovePoint(Direction.Right); }
	}

	class ReverseLineNavigator : ILineNavigator
	{
		Document document;
		public ReverseLineNavigator(Document document) { this.document = document; }

		public bool AtEnd { get { return document.Point.StartOfLine; } }
		public char NextChar { get { return document.Point.CharOnLeft; } }
		public void Move() { document.MovePoint(Direction.Left); }
	}
}
