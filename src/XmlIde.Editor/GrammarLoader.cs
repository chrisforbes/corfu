using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IjwFramework.Types;
using Corfu.Language;
using System.IO;

namespace XmlIde.Editor
{
	static class GrammarLoader
	{
		static Cached<Grammar> grammar = Cached.New(() =>
			{
				var g = new Grammar();
				var files = Directory.GetFiles(Config.GetAbsolutePath("/languages"), "*.gx");
				foreach (var f in files)
					g.AddLanguage(f);
				return g;
			});

		public static Grammar Grammar { get { return grammar.Value; } }
		public static void ReloadGrammar() { grammar.Invalidate(); }
	}
}
