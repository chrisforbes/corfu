using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using XmlIde.Editor;
using XmlIde.Editor.Find;

namespace Editor
{
	class FindBar : ToolStrip
	{
		FindBarTextBox ec;
		EditorControl editor;
		CheckBox RegExCheckBox = new CheckBox();
		CheckBox CaseSensitivityCheckBox = new CheckBox();

		public FindBar(ToolStripContainer workspace, EditorControl editor)
		{
			this.editor = editor;

			Image cancelImage = Image.FromFile(Config.GetAbsolutePath("/res/critical.png"));
			Image goImage = Image.FromFile(Config.GetAbsolutePath("/res/findnexths.png"));

			workspace.BottomToolStripPanel.Controls.Add(this);
			Visible = false;
			Left = workspace.LeftToolStripPanel.Width;

			Items.Add("", cancelImage, delegate { Visible = false; });
			
			Items.Add(new ToolStripLabel("Find What:"));

			ec = new FindBarTextBox();
			ec.Width = 300;
			ec.Dismissed += delegate
			{
				Hide();
				editor.Focus();
			};

			ec.Accepted += delegate { DoFind(); };

			ToolStripControlHost i = new ToolStripControlHost(ec);
			i.AutoSize = false;
			i.Width = ec.Width;
			Items.Add(i);

			Items.Add(new ToolStripButton("", goImage, delegate { DoFind(); }));

			RegExCheckBox.Text = "Regular Expression";
			RegExCheckBox.AutoSize = false;
			RegExCheckBox.Width = 130;
			RegExCheckBox.Checked = false;
			RegExCheckBox.BackColor = Color.Transparent;

			CaseSensitivityCheckBox.Text = "Case Sensitive";
			CaseSensitivityCheckBox.Checked = false;
			CaseSensitivityCheckBox.BackColor = Color.Transparent;

			ToolStripControlHost RegExHost = new ToolStripControlHost(RegExCheckBox);
			RegExHost.AutoSize = false;
			RegExHost.Width = RegExCheckBox.Width;
			ToolStripControlHost CaseHost = new ToolStripControlHost(CaseSensitivityCheckBox);
			CaseHost.AutoSize = false;
			CaseHost.Width = CaseSensitivityCheckBox.Width;

			Items.Add(RegExHost);
			Items.Add(CaseHost);
		}

		public new void Show()
		{
			Visible = true;
			ec.Focus();
			ec.SelectAll();
		}

		void DoFind()
		{
			if (editor.Document == null)
				throw new InvalidOperationException("find without a document");

			IFinder f;
			if (RegExCheckBox.Checked)
				f = new RegexFinder(CaseSensitivityCheckBox.Checked);
			else
				f = new PrimitiveFinder(CaseSensitivityCheckBox.Checked);

			Finder finder = new Finder(editor.Document, f);
			if (!finder.FindNext(ec.Text))
				MessageBox.Show("The current document does not contain `{0}`".F(ec.Text));
			else
				editor.EnsureVisible();

			editor.Invalidate();
		}
	}
}
