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
		CheckBox RegExCheckBox;
		CheckBox CaseSensitivityCheckBox;

		public FindBar(ToolStripContainer workspace, EditorControl editor)
		{
			this.editor = editor;

			var cancelImage = Image.FromFile("/res/critical.png".AsAbsolute());
			var goImage = Image.FromFile("/res/findnexths.png".AsAbsolute());

			workspace.BottomToolStripPanel.Controls.Add(this);
			Visible = false;
			Left = workspace.LeftToolStripPanel.Width;

			Items.Add("", cancelImage, (_, e) => Visible = false);
			
			Items.Add(new ToolStripLabel("Find What:"));

			ec = new FindBarTextBox() { Width = 300 };
			ec.Dismissed += () => { Hide(); editor.Focus(); };
			ec.Accepted += () => { DoFind(); };

			var i = new ToolStripControlHost(ec) { AutoSize = false, Width = ec.Width };
			Items.Add(i);

			Items.Add(new ToolStripButton("", goImage, (_, e) => DoFind()));

			RegExCheckBox = new CheckBox()
			{
				Text = "Regular Expression",
				AutoSize = false,
				Width = 130,
				Checked = false,
				BackColor = Color.Transparent,
			};

			CaseSensitivityCheckBox = new CheckBox()
			{
				Text = "Case Sensitive",
				Checked = false,
				BackColor = Color.Transparent,
			};

			var RegExHost = new ToolStripControlHost(RegExCheckBox)
			{
				AutoSize = false,
				Width = RegExCheckBox.Width
			};

			var CaseHost = new ToolStripControlHost(CaseSensitivityCheckBox)
			{
				AutoSize = false,
				Width = CaseSensitivityCheckBox.Width
			};

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

			var f = RegExCheckBox.Checked 
				? (IFinder) new RegexFinder(CaseSensitivityCheckBox.Checked)
				: new PrimitiveFinder(CaseSensitivityCheckBox.Checked);

			var finder = new Finder(editor.Document, f);
			if (!finder.FindNext(ec.Text))
				MessageBox.Show("The current document does not contain `{0}`".F(ec.Text));
			else
				editor.EnsureVisible();

			editor.Invalidate();
		}
	}
}
