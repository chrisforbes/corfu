using System;
using System.Collections.Generic;
using System.Text;
using IjwFramework.Collections;

namespace XmlIde.LanguageFramework
{
	class Checkpoint
	{
		SharedStack<ContextRule> stack;
		readonly string defaultStyle;

		public Checkpoint(string defaultStyle)
		{
			this.defaultStyle = defaultStyle;
			stack = new SharedStack<ContextRule>();
		}

		public Checkpoint(Checkpoint other)
		{
			this.defaultStyle = other.defaultStyle;
			stack = new SharedStack<ContextRule>(other.stack);
		}

		public Action LambdaPush(ContextRule rule)
		{
			return delegate { stack.Push(rule); };
		}

		public Action LambdaPop()
		{
			return delegate { if (!stack.Empty) stack.Pop(); };
		}

		public Rule EndContextRule
		{
			get { return stack.Empty ? null : stack.Peek().End; }
		}
		
		public IEnumerable<Rule> ApplicableRules
		{
			get { return stack.Empty ? null : stack.Peek().ApplicableRules; }
		}
		
		public string ContentStyle
		{
			get { return stack.Empty ? defaultStyle : stack.Peek().ContentStyle; }
		}
	}
}
