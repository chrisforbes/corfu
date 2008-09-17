using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IjwFramework.Types;
using XmlIde.Editor;

namespace Corfu.Language
{
	public enum RuleAction
	{
		JustEmit,
		Push,
		PopBefore,
		PopAfter,
	}

	public class Rule
	{
		public readonly Regex Pattern;
		public readonly string ScopeName;
		RuleAction action;
		string actionScopeName;
		List<Pair<int, string>> captureScopes;

		public Rule(string pattern, string scopeName)
		{
			Pattern = new Regex(pattern);
			ScopeName = scopeName;
			action = RuleAction.JustEmit;
		}

		public void SetCaptureScope(int n, string scopeName)
		{
			if (captureScopes == null)
				captureScopes = new List<Pair<int, string>>();

			captureScopes.Add(n.PairedWith(scopeName));
		}

		public void SetAction(RuleAction action, string actionScopeName)
		{
			this.action = action;
			this.actionScopeName = actionScopeName;
		}

		public void SetAction(RuleAction action)
		{
			SetAction(action, ScopeName);
		}

		public string PreEmit(string scope)
		{
			if (action == RuleAction.PopBefore)
				return scope.Substring(0, scope.LastIndexOf(' '));

			return scope;
		}

		public string PostEmit(string scope)
		{
			if (action == RuleAction.PopAfter)
				return scope.Substring(0, scope.LastIndexOf(' '));

			if (action == RuleAction.Push)
				return scope + " " + actionScopeName;

			return scope;
		}

		public IEnumerable<Span> GetSpans(string scope, Match m)
		{
			if (captureScopes != null && captureScopes.Count > 0)
				throw new NotImplementedException();

			yield return new Span(m.Index, m.Length, 
				string.IsNullOrEmpty(ScopeName) 
					? scope 
					: (scope + " " + ScopeName));
		}
	}
}
