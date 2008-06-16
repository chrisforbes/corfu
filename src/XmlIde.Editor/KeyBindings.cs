using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace XmlIde.Editor
{
	public class KeyBindings
	{
		Dictionary<string, Action<Keys>> commands = new Dictionary<string, Action<Keys>>();
		Dictionary<Keys, string> bindings = new Dictionary<Keys, string>();

		public KeyBindings() { Reload(); }

		public void Reload()
		{
			bindings.Clear();

			XmlDocument doc = new XmlDocument();
			doc.Load(Config.GetAbsolutePath("/res/keybindings.xml"));

			foreach (XmlElement e in doc.SelectNodes("/keybindings/binding"))
				bindings.Add(e.GetAttribute("key").ToShortcutKey(), e.GetAttribute("action"));
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
