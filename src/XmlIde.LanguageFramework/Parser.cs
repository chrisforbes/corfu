using System;
using System.Collections.Generic;
using System.Text;
using XmlIde.Editor;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;
using IjwFramework.Types;
using System.Linq;

namespace XmlIde.LanguageFramework
{
	class Parser
	{
		List<Rule> rules = new List<Rule>();
		readonly string defaultStyle = null;

		public Parser(string defaultTextStyle) { this.defaultStyle = defaultTextStyle; }
		public void AddRule(Rule rule) { rules.Add(rule); }

		Checkpoint state;

		public Checkpoint NewCheckpoint() { return new Checkpoint(defaultStyle); }

		public Pair<IEnumerable<Span>, Checkpoint> Parse(string sourceText, Checkpoint start)
		{
			state = new Checkpoint(start);
			IEnumerable<Span> spans = ParseInner(sourceText);
			return new Pair<IEnumerable<Span>, Checkpoint>(spans, state);
		}

		IEnumerable<Span> ParseInner(string sourceText)
		{
			int offset = 0;

			while (offset < sourceText.Length)
			{
				Pair<IEnumerable<Span>, Action> current = GetEarliestMatch(sourceText, offset);

				if (current.First == null)
				{
					yield return new Span(offset, sourceText.Length - offset, state.ContentStyle);
					yield break;
				}

				var f = current.First.First();

				if (f.Start > offset)
					yield return new Span(offset, f.Start - offset, state.ContentStyle);

				foreach (Span span in current.First)
				{
					yield return span;
					offset = span.End;
				}

				if (current.Second != null)
					current.Second();
			}
		}

		Pair<IEnumerable<Span>, Action> GetEarliestMatch(string sourceText, int offset)
		{
			var current = new Pair<IEnumerable<Span>, Action>(null, null);
			int firstIndex = int.MaxValue;

			foreach (Pair<Rule, Action> rule in Rules())
			{
				IEnumerable<Span> spanList = rule.First.Match(sourceText, offset);
				if (spanList == null)
					continue;

				Span span = spanList.First();

				if (current.First == null || span.Start < firstIndex)
				{
					current = new Pair<IEnumerable<Span>, Action>(spanList, rule.Second);
					firstIndex = span.Start;
					if (span.Start == offset)
						break;
				}
			}

			return current;
		}

		IEnumerable<Pair<Rule, Action>> Rules()
		{
			Rule popRule = state.EndContextRule;
			if (popRule != null)
				yield return new Pair<Rule, Action>(popRule, state.LambdaPop());

			IEnumerable<Rule> applicableRules = state.ApplicableRules ?? rules;

			foreach (Rule rule in applicableRules)
				yield return new Pair<Rule, Action>(rule, rule.GetContextAction(state));
		}
	}
}
