using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XmlIde.Editor;
using IjwFramework.Types;

namespace Editor
{
	public partial class ClipboardHistory : Form
	{
		public ClipboardHistory( IEnumerable<Pair<string,string>> history )
		{
			InitializeComponent();
			listBox1.Font = new Font("Lucida Console", 8.5f);

			foreach (Pair<string, string> s in history)
				listBox1.Items.Add(s);

			if (listBox1.Items.Count > 0)
				listBox1.SelectedIndex = 0;

			Text += " ({0} items)".F(listBox1.Items.Count);
		}

		void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			Pair<string, string> s = (Pair<string, string>)listBox1.Items[e.Index];
			string[] lines = s.First.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			
			int count = Math.Min(lines.Length, 5) + 1;

			e.ItemWidth = Width;
			e.ItemHeight = count * listBox1.Font.Height;
		}

		void listBox1_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index == -1)
				return;

			e.DrawBackground();

			var s = (Pair<string, string>)listBox1.Items[e.Index];
			var lines = s.First.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			var y = e.Bounds.Top;

			e.Graphics.DrawString("From {0} ({1} lines)".F(s.Second, lines.Length),
				listBox1.Font, Brushes.Gray, 0, y);
			y += listBox1.Font.Height;

			using (var b = new SolidBrush(e.ForeColor))
				for( int i = 0; i < Math.Min( lines.Length, 5 ); i++)
				{
					e.Graphics.DrawString(lines[i].ExpandTabs(), listBox1.Font, b, 32, y);
					y += listBox1.Font.Height;
				}
		}

		public Pair<string, string> SelectedItem
		{
			get { return (Pair<string, string>)listBox1.SelectedItem; }
		}

		void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			button2.Enabled = listBox1.SelectedItem != null;
		}
	}
}