using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace XmlIde.Editor
{
	public class Span
	{
		public readonly int Start, Length;
		public readonly string Style;

		public int End { get { return Start + Length; } }

		internal RenderCache renderCache;

		public Span(int start, int length, string style)
			: this(start, length)
		{
			Style = style;
		}

		public Span(int start, int length)
		{
			Start = start;
			Length = length;
		}

		bool Intersects(Span other)
		{
			return End > other.Start && Start < other.End;
		}

		bool Engulfs(Span other)
		{
			return Start <= other.Start && End >= other.End;
		}

		Span ChopEnd(int p)
		{
			return new Span(Start, p - Start, Style);
		}

		Span ChopStart(int p)
		{
			return new Span(p, Length - (p - Start), Style);
		}

		IEnumerable<Span> ChopWith(Span other)
		{
			if (!Intersects(other))
				yield return this;
			else if (other.Engulfs(this))
				yield return other;
			else
			{
				if (Start < other.Start)
					yield return ChopEnd(other.Start);

				yield return other;

				if (End > other.End)
					yield return ChopStart(other.End);
			}
		}

		public IEnumerable<Span> SplitWith(Span splitter)
		{
			if (!Intersects(splitter) || splitter.Length == 0)
			{
				yield return this;
				yield break;
			}

			if (Start < splitter.Start)
				yield return new Span(Start, splitter.Start - Start, Style);

			yield return splitter;

			if (End > splitter.End)
				yield return new Span(splitter.End, End - splitter.End, Style);
		}

		public IEnumerable<Span> ChopForRendering(int maxSpanLength)
		{
			if (Length < maxSpanLength)
				yield return this;
			else
			{
				int currentStart = Start;
				int currentLength = Length;

				while (currentLength >= maxSpanLength)
				{
					yield return new Span(currentStart, maxSpanLength, Style);
					currentLength -= maxSpanLength;
					currentStart += maxSpanLength;
				}

				if (currentLength > 0)
					yield return new Span(currentStart, currentLength, Style);
			}
		}

		public IEnumerable<Span> ApplyTo(IEnumerable<Span> source)
		{
			Span previous = null;

			foreach (Span s in source)
				foreach (Span subSpan in s.ChopWith(this))
					if (previous != subSpan)
						yield return previous = subSpan;
		}
	}
}
