using System;
using System.Collections.Generic;
using System.Text;
using IjwFramework.Types;

namespace XmlIde.Editor.Find
{
	public interface IFinder
	{
		Pair<int, int> FindNext(string source, string match);
	}
}
