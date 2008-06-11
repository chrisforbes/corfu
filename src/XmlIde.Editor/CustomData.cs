using System;
using System.Collections.Generic;
using System.Text;

namespace XmlIde.Editor
{
	public class CustomData<T>
	{
		public T start, end;

		public CustomData() { }
		public CustomData(CustomData<T> other)
			: this(other.start)
		{
		}

		public CustomData(T startState)
		{
			start = startState;
		}

		static IEqualityComparer<T> tc = EqualityComparer<T>.Default;

		public static bool IsValidTransition(CustomData<T> a, CustomData<T> b)
		{
			return a != null && b != null && tc.Equals(a.end, b.start);
		}
	}
}
