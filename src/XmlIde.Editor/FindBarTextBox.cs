using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace XmlIde.Editor
{
	public class FindBarTextBox : TextBox
	{
		public FindBarTextBox()
			: base()
		{
			Font = new Font("Lucida Console", 8.5f);
		}

		public event EventHandler Dismissed = delegate { };
		public event EventHandler Accepted = delegate { };

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Escape)
			{
				Dismissed(this, EventArgs.Empty);
				return true;
			}

			if (keyData == Keys.Enter)
			{
				Accepted(this, EventArgs.Empty);
				return true;
			}
				
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
