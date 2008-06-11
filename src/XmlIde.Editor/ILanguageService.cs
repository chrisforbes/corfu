using System;
using System.Collections.Generic;
using System.Text;
using XmlIde.Editor.Stylers;

namespace XmlIde.Editor
{
	public interface ILanguageService
	{
		Styler Styler { get; }
	}
}
