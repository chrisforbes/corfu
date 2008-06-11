using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XmlIde.Editor;

namespace Editor
{
	partial class CloseConfirmationDialog : Form
	{
		public CloseConfirmationDialog( IEnumerable<Document> documents )
		{
			InitializeComponent();

			itemList.Format += delegate(object sender, ListControlConvertEventArgs e)
			{
				e.Value = ( (Document)e.ListItem ).Filename;
			};

			foreach (Document d in documents)
				itemList.Items.Add(d, true);
		}

		public IEnumerable<Document> GetSelectedDocuments()
		{
			foreach (Document d in itemList.CheckedItems)
				yield return d;
		}
	}
}