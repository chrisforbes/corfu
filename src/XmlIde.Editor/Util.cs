using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using IjwFramework.Types;
using System.Drawing;

namespace XmlIde.Editor
{
	public static class Util
	{
		public const int tabSize = 4;

		public static bool Contains(this string s, char c) { return s.IndexOf(c) != -1; }

		public static string ExpandTabs(this string input) { return ExpandTabs(input, 0); }

		public static string ExpandTabs(this string s, int startColumn )
		{
			if( !s.Contains( '\t' ) ) return s;

			int nonTabStart = 0;
			int nonTabLength = 0;
			var b = new StringBuilder( s.Length * 2 );
			foreach( char c in s )
			{
				if( c == '\t' )
				{
					if( nonTabLength != 0 )
						b.Append( s, nonTabStart, nonTabLength );
					b.Append( ' ', tabSize - ( ( startColumn + b.Length ) % tabSize ) );
					nonTabStart += nonTabLength + 1;
					nonTabLength = 0;
				}
				else
					++nonTabLength;
			}
			if( nonTabLength != 0 )
				b.Append( s, nonTabStart, nonTabLength );

			return b.ToString();
		}

		public static string CanonicalizePath(this string s)
		{
			return (s == null) ? null : s.Replace('/', '\\').ToLowerInvariant();
		}

		public static bool PathsEqual(string a, string b)
		{
			return CanonicalizePath(a) == CanonicalizePath(b);
		}

		public static Keys ToShortcutKey(this string s)
		{
			var k = Keys.None;

			foreach (var p in s.Split(new char[] { '+', ',', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries))
				k |= Enum<Keys>.Parse(p == "Ctrl" ? "Control" : p);

			return k;
		}

		public static string SafeSubstring(string s, int start, int len)
		{
			if (string.IsNullOrEmpty(s)) return string.Empty;

			if (start < 0) start = 0;
			if (len < 0) len = 0;
			if (start + len > s.Length) len = s.Length - start;

			return (len == 0) ? string.Empty : s.Substring(start, len);
		}

		public static string F(this string s, params object[] args)
		{
			return string.Format(s, args);
		}

		internal static float MeasureWith(this string s, TextGeometry geom)
		{
			return geom.MeasureString(s);
		}

		public static int Clamp(this int x, int low, int high)
		{
			if (x < low) return low;
			if (x > high) return high;
			return x;
		}

		public static void Do<T>(this IEnumerable<T> seq, Action<T> a)
		{
			foreach (var t in seq) a(t);
		}

		public static U ValueOrDefault<T, U>(this Dictionary<T, U> dict, T key, U value)
		{
			dict.TryGetValue(key, out value);
			return value;
		}

		public static IEnumerable<T> JustThis<T>(this T t) { yield return t; }
		
		public static string Reverse(this string s)
		{
			var ch = s.ToCharArray();
			Array.Reverse(ch);
			return new string(ch);
		}
	}
}
