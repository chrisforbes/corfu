using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using IjwFramework.Collections;
using IjwFramework.Types;

namespace XmlIde.Editor
{
	class StyleProvider
	{
		readonly static Style DefaultStyle = new Style(SystemBrushes.WindowText, null, Decoration.None);
		readonly static string XmlPath = Config.GetAbsolutePath("/res/styles.xml");

		XmlDocument doc = null;
		Cache<string, Style> styleCache;

		public StyleProvider() 
		{ 
			Reload(); 
		}

		public void Reload()
		{
			try
			{
				var newDocument = new XmlDocument();
				newDocument.Load(XmlPath);
				doc = newDocument;
				styleCache = new Cache<string, Style>(
					s => GetFromXml(s.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)));
			}
			catch (Exception e)
			{
				MessageBox.Show("The styles.xml file contains an error or does not exist. Styles not refreshed.\nException:"
					+ e.ToString(),
					"Corfu - Styles error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		public Style GetStyle(string name)
		{
			if (doc == null || name == null)
				return DefaultStyle;

			return styleCache[name.Split(' ').Last()];
		}

		Style GetFromXml(string[] frags)
		{
			XmlElement e = (XmlElement)doc.SelectSingleNode("/styles");
			foreach (string frag in frags)
			{
				XmlElement f = (XmlElement)e.SelectSingleNode("./scope[@name=\"{0}\"]".F(frag));
				if (f == null)
					break;
				e = f;
			}

			if (e.Name == "styles") return DefaultStyle;

			string forecolor = e.GetAttribute("forecolor");
			string backcolor = e.GetAttribute("backcolor");
			string decoration = e.GetAttribute("decoration");

			return new Style(GetColor(forecolor, DefaultStyle.foreground), 
				GetColor(backcolor, DefaultStyle.background), ParseDecoration(decoration));
		}

		Decoration ParseDecoration(string value)
		{
			return string.IsNullOrEmpty(value) ?
				Decoration.None : Enum<Decoration>.Parse(value, Decoration.None);
		}

		Brush GetColor(string name, Brush defaultBrush)
		{
			Color? c = ParseColor(name);
			return (c == null) ? defaultBrush : new SolidBrush(c.Value);
		}

		static Color? ParseColor(string color)
		{
			if (string.IsNullOrEmpty(color)) 
				return null;

			if (color.StartsWith("#"))
				return Color.FromArgb(255,
					Color.FromArgb(int.Parse(color.Substring(1,6), NumberStyles.HexNumber)));
			else
				return Color.FromName(color);
		}
	}
}