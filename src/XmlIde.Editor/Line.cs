using System;
using System.Collections.Generic;
using System.Linq;
using Corfu.Language;
using IjwFramework.Types;

namespace XmlIde.Editor
{
	public enum LineModification { Clean, Saved, Unsaved, }

	public class Line
	{
		readonly Document document;
		string text;
		Pair<string, string> scopeInfo;

		public LineModification Dirty { get; set; }
		public IEnumerable<Span> Spans { get; private set; }

		public Document Document { get { return document; } }
		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				Spans = null;
				Dirty = LineModification.Unsaved;
				document.Dirty = true;
				document.ResetStylerPosition(Number);
			}
		}

		int cachedLineNumber;

		public int Number
		{
			get
			{
				IList<Line> lines = document.Lines;

				if (lines.Count > cachedLineNumber && lines[cachedLineNumber] == this)
					return cachedLineNumber;

				return cachedLineNumber = lines.IndexOf(this);
			}
		}

		public Line(string text, LineModification changed, Document document)
		{
			if (document == null)
				throw new ArgumentNullException("document");

			this.text = text;
			this.Dirty = changed;
			this.document = document;

			if (changed == LineModification.Unsaved)
				document.Dirty = true;
		}

		public void Insert(char c, int index, int lineNumber) { Text = Text.Insert(index, "" + c); }
		public void Delete(int index, int lineNumber) { Text = Text.Remove(index, 1); }

		public int Indent
		{
			get
			{
				int i = 0;
				foreach (char c in Text)
				{
					if (!char.IsWhiteSpace(c))
						break;
					++i;
				}

				return i;
			}
		}

		void FixTransitionFrom(Line from)
		{
			var initialScope = (from == null || from.scopeInfo == null)
				? document.FileType.RootScope : from.scopeInfo.Second;

			scopeInfo = new Pair<string, string>(initialScope, initialScope);
			Spans = null;
		}

		public void ReStyle(int lineNumber)
		{
			cachedLineNumber = lineNumber;

			FixTransitionFrom(document.GetLine(lineNumber - 1));

			var parser = new Parser(GrammarLoader.Grammar, scopeInfo.First);
			Spans = parser.ParseSpans(text).ToList();
			scopeInfo.Second = parser.Scope;

			var nextLine = document.GetLine(lineNumber + 1);
			if (nextLine != null)
				nextLine.FixTransitionFrom(this);
		}
	}
}
