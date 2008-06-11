using System;
using System.Collections.Generic;
using System.Text;

namespace XmlIde.Editor
{
	class ReplaceTextContext
	{
		IEnumerable<Action<Region>> adjusters;
		RegionManager manager;

		public ReplaceTextContext(RegionManager manager)
		{
			this.manager = manager;
		}

		public void SubscribeTo(Document document)
		{
			document.AfterReplace += AfterReplace;
			document.BeforeReplace += BeforeReplace;
		}

		public void Unsubscribe(Document document)
		{
			document.AfterReplace -= AfterReplace;
			document.BeforeReplace -= BeforeReplace;
		}

		void BeforeReplace(Region before)
		{
			adjusters = manager.GetAdjustments(before);
		}

		void AfterReplace(Region after)
		{
			manager.ApplyAdjustments(after, adjusters);
		}
	}
}
