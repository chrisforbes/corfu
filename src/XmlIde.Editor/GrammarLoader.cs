using System.IO;
using System.Linq;
using Corfu.Language;
using IjwFramework.Types;

namespace XmlIde.Editor
{
	public static class GrammarLoader
	{
        static Cached<Grammar> grammar = Cached.New(() =>
            Directory.GetFiles("/languages".AsAbsolute(), "*.gx")
                    .Aggregate(new Grammar(), (g, f) => g.AddLanguage(f)));

		public static Grammar Grammar { get { return grammar.Value; } }
		public static void ReloadGrammar() { grammar.Invalidate(); }
	}
}
