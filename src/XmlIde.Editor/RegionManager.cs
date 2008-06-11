using System;
using System.Collections.Generic;
using System.Text;

namespace XmlIde.Editor
{
	public class RegionManager : List<Region>
	{
		EditorControl host;

		public RegionManager(EditorControl host) { this.host = host; }

		public IEnumerable<Span> GetStickyRegionSpans(Line line)
		{
			foreach (Region r in this)
			{
				Span s = r.GetSpan(line);
				if (s != null)
					yield return s;
			}
		}

		public IEnumerable<Span> ApplyRegions(Line line)
		{
			IEnumerable<Span> spans = host.Document.ApplySelectionSpan(line.Spans, line);
			foreach (Span regionSpan in GetStickyRegionSpans(line))
				spans = regionSpan.ApplyTo(spans);

			return spans;
		}

		public IEnumerable<Action<Region>> GetAdjustments(Region before)
		{
			ICollection<Action<Region>> adjustments = new List<Action<Region>>();
			ICollection<Region> toDelete = new List<Region>();

			foreach (Region r in this)
			{
				Action<Region> adj = r.GetAdjustment(before);
				if (adj != null)
					adjustments.Add(adj);
				else
					toDelete.Add(r);
			}

			foreach (Region r in toDelete)
				Remove(r);

			return adjustments;
		}

		public void ApplyAdjustments(Region after, IEnumerable<Action<Region>> adjustments)
		{
			foreach (Action<Region> adj in adjustments)
				adj(after);
		}
	}
}
