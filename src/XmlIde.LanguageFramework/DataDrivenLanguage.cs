using System;
using System.Collections.Generic;
using System.Text;
using XmlIde.Editor;
using XmlIde.Editor.Stylers;

namespace XmlIde.LanguageFramework
{
	public class DataDrivenLanguage : ILanguageService
	{
		readonly string languageDefinition;
		Styler styler;

		public Styler Styler { get { return styler ?? (styler = new DataDrivenStyler(languageDefinition)); } }

		public DataDrivenLanguage(string languageDefinition)
		{
			this.languageDefinition = languageDefinition;
		}
	}
}
