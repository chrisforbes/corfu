using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Linq;

namespace XmlIde.Editor
{
	public class KeyBindings
	{
		Dictionary<string, Action<Keys>> commands = new Dictionary<string, Action<Keys>>();
		Dictionary<Keys, string> bindings;

		public KeyBindings() { Reload(); }

		public void Reload()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load("/res/keybindings.xml".AsAbsolute());

			bindings = doc.SelectNodes("/keybindings/binding")
				.Cast<XmlElement>().ToDictionary(
					e => e.GetAttribute("key").ToShortcutKey(),
					e => e.GetAttribute("action"));
		}

		public void Offer(string name, Action<Keys> command)
		{
			commands.Add(name, command);
		}

		public bool OnKeyPress( Keys k )
		{
			if (!bindings.ContainsKey(k)) return false;
			string commandName = bindings[k];

			if (!commands.ContainsKey(commandName)) return false;
			commands[commandName](k);
			return true;
		}
	}
}
