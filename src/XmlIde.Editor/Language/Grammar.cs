using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IjwFramework.Collections;
using IjwFramework.Types;
using XmlIde.Editor;

namespace Corfu.Language
{
	enum GrammarToken
	{
		None,		Keyword,
		ScopeName,	Arrow,
		Pattern,	PatternEscape,
	}

	public class Grammar
	{
		Cache<string, Scope> scopes = new Cache<string, Scope>(x => new Scope(x));

		public Grammar() { }

		public Grammar AddLanguage( string filename )
		{
			var currentScopes = new List<Scope>();
			Rule rule = null;

			foreach (var line in File.ReadAllLines(filename))
			{
				var tokens = GetTokens(line).ToArray();
				if (tokens.Length == 0) continue;

				if (tokens[0].First == GrammarToken.Pattern)
				{
					if (tokens.Length != 3 || tokens[1].First != GrammarToken.Arrow || tokens[2].First != GrammarToken.ScopeName)
						throw new InvalidDataException("Expected `pattern -> scopename`");

					if (currentScopes.Count == 0) throw new InvalidDataException("Rule without context");
					rule = new Rule(tokens[0].Second, tokens[2].Second);
					foreach (var scope in currentScopes)
						scope.AddRule(rule);
				}

				if (tokens[0].First == GrammarToken.Keyword)
				{
					if (tokens[0].Second == "in")
					{
						if (tokens.Length != 2 || tokens[1].First != GrammarToken.ScopeName)
							throw new InvalidDataException("Expected `scopename` to follow `in`");

						currentScopes = tokens[1].Second.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
							.Select(x => scopes[x.Trim()]).ToList();
					}
					else if (tokens[0].Second == "enter:")
					{
						if (rule == null)
							throw new InvalidDataException("`enter` action without rule");

						rule.SetAction(RuleAction.Push, tokens[1].Second);
					}
					else if (tokens[0].Second == "leave:")
					{
						if (rule == null)
							throw new InvalidDataException("`leave` action without rule");
						rule.SetAction(tokens[1].Second == "after" ? RuleAction.PopAfter : RuleAction.PopBefore);
					}
					else
					{
						var capnum = tokens[0].Second.Substring(0, tokens[0].Second.Length - 1);
						int n;
						if (!int.TryParse(capnum, out n))
							throw new InvalidDataException("Unrecognized option: {0}".F(tokens[0].Second));

						rule.SetCaptureScope(n, tokens[1].Second);
					}
				}
			}

			return this;
		}

		public IEnumerable<Scope> Scopes { get { return scopes.Values; } }

		IEnumerable<Pair<GrammarToken, string>> GetTokens(string line)
		{
			string token = "";
			var type = GrammarToken.None;
			int col = 1;

			foreach (var c in (line+"\n"))
			{
				switch (type)
				{
					case GrammarToken.None:
						if (c == '-') { token = "-"; type = GrammarToken.Arrow; }
						else if (c == '[') { token = ""; type = GrammarToken.ScopeName; }
						else if (c == '/') { token = ""; type = GrammarToken.Pattern; }
						else if (c == ' ' || c == '\t' || c == '\n' || c == '\r') {}
						else if (char.IsLetterOrDigit(c)) { token = new string(c, 1); type = GrammarToken.Keyword; }
						else throw new InvalidDataException( "While parsing {0}; unexpected character at col {1}".F(line,col) );
						break;

					case GrammarToken.ScopeName:
						if (c == '\n') throw new InvalidDataException("While parsing {0}; end of line found while looking for `]`".F(line));
						if (c == ']') { yield return type.PairedWith(token); type = GrammarToken.None; }
						token += c;
						break;

					case GrammarToken.Arrow:
						if (c == '-') { yield break; }
						if (c == '>') { token += c; yield return type.PairedWith(token); type = GrammarToken.None; }
						else throw new InvalidDataException("While parsing {0}; Expected `-` or `>` but found {1}".F(line, c)); 
						break;

					case GrammarToken.Pattern:
						if (c == '\\') { type = GrammarToken.PatternEscape; }
						else if (c == '/') { yield return type.PairedWith(token); type = GrammarToken.None; }
						else token += c;
						break;

					case GrammarToken.PatternEscape:
						if (c != '/')
							token += '\\';
						token += c;
						type = GrammarToken.Pattern;
						break;

					case GrammarToken.Keyword:
						if (c == ' ' || c == '\t' || c == '\n') { yield return type.PairedWith(token); type = GrammarToken.None; }
						else token += c;
						break;
				}
				++col;
			}
		}

		public IEnumerable<Rule> GetRules(string scope)
		{
			return Scopes
				.Where(x => (x.Name == scope) || x.Name.EndsWith(' ' + scope))
				.SelectMany(x => x.Rules);
		}
	}
}
