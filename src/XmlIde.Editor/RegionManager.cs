using System;
using System.Collections.Generic;
using System.Text;

namespace XmlIde.Editor
{
	public class RegionManager : List<Region>
	{
		EditorControl host;

		public RegionManager(EditorControl host) { this.host = host; }

		public IEnumerable<Span> ApplyRegions(Line line)
		{
			return host.Document.ApplySelectionSpan(line.Spans, line);
		}
	}
}
