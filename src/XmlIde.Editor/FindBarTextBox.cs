using System;
using System.Drawing;
using System.Windows.Forms;

namespace XmlIde.Editor
{
	public class FindBarTextBox : TextBox
	{
		public FindBarTextBox()
		{
			Font = new Font("Lucida Console", 8.5f);
		}

		public event Action Dismissed = () => { };
		public event Action Accepted = () => { };

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Escape)
			{
				Dismissed();
				return true;
			}

			if (keyData == Keys.Enter)
			{
				Accepted();
				return true;
			}
				
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
