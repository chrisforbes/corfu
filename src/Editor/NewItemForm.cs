using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using XmlIde.Editor;

namespace Editor
{
	partial class NewItemForm : Form
	{
		string fileName;
		public string FileName { get { return fileName; } }
		Document newDocument;
		public Document NewDocument { get { return newDocument; } }

		public NewItemForm()
		{
			InitializeComponent();

			var templates = new XmlDocument();
			templates.Load( "languages/NewItemTemplates/templates.xml".AsAbsolute() );
			foreach (XmlElement e in templates.SelectNodes("/templates/template"))
			{
				var name = e.GetAttribute("name");
				var file = e.GetAttribute("file");
				var icon = e.GetAttribute("icon");
				var suffix = e.GetAttribute("suffix");
				var i = Image.FromFile(icon.AsAbsolute());
				imageList1.Images.Add(name, i);
				listView1.Items.Add(name, name).Tag = new TemplateToken(file, suffix);
			}

			if( listView1.Items.Count <= 0 )
				throw new InvalidOperationException( "No templates in `templates.xml'" );

			TemplateSelected( null, null );
		}

		TemplateToken SelectedTemplate
		{
			get
			{
				if (listView1.SelectedItems.Count == 0)
					return null;

				return (TemplateToken)listView1.SelectedItems[0].Tag;
			}
		}

		void TemplateSelected( object sender, EventArgs e )
		{
			if (SelectedTemplate != null)
			{
				var s = string.IsNullOrEmpty(fileNameTextBox.Text) ? "Untitled" : fileNameTextBox.Text;
				fileNameTextBox.Text = Path.ChangeExtension(s, SelectedTemplate.Suffix);
			} 

			UpdateOk();
		}

		void UpdateOk()
		{
			okButton.Enabled = SelectedTemplate != null && !string.IsNullOrEmpty( fileNameTextBox.Text );
		}

		void FilenameChanged( object sender, EventArgs e ) { UpdateOk(); }

		void OkClicked( object sender, EventArgs e )
		{
			fileName = fileNameTextBox.Text;
			if( string.IsNullOrEmpty( Path.GetExtension( fileName ) ) )
				fileName = Path.ChangeExtension( fileName, SelectedTemplate.Suffix );

			newDocument = SelectedTemplate.CreateInstance(fileName);
		}

		void listView1_DoubleClick( object sender, EventArgs e )
		{
			if( !okButton.Enabled )
				return;

			OkClicked( sender, e );
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
