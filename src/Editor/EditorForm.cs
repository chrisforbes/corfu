using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Xml;

using XmlIde.Editor;
using IjwFramework.Ui;
using IjwFramework;

namespace Editor
{
	partial class EditorForm : Form
	{
		EditorControl editor = new EditorControl();
		DocumentTabStrip tabStrip = new DocumentTabStrip();
		FindBar findBar;
		StatusStrip statusStrip = new StatusStrip();
		ToolStripStatusLabel styleLabel = new ToolStripStatusLabel();

		static readonly string baseTitle = "Corfu " + Product.ShortVersion;

		const AnchorStyles TopEdge = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		const AnchorStyles ClientArea = TopEdge | AnchorStyles.Bottom;

		public EditorForm()
		{
			InitializeComponent();

			Text = baseTitle;

			workspace.BottomToolStripPanel.Controls.Add(statusStrip);
			statusStrip.Dock = DockStyle.None;
			statusStrip.Items.Add(documentTypeLabel);
			statusStrip.Items.Add(new ToolStripGap());
			statusStrip.Items.Add(styleLabel);
			statusStrip.Location = new Point(0, 0);
			statusStrip.Size = new Size(822, 22);
			statusStrip.TabIndex = 0;

			BuildUI();

			workspace.ContentPanel.Controls.Add(tabStrip);
			workspace.ContentPanel.Controls.Add(editor);
			workspace.ContentPanel.BackColor = SystemColors.AppWorkspace;
			
			tabStrip.Anchor = TopEdge;
			tabStrip.Location = new Point(0, 0);
			tabStrip.Size = new Size(workspace.ContentPanel.ClientSize.Width, 20);

			editor.Anchor = ClientArea;
			editor.Location = new Point(1, 20);
			editor.Size = new Size(workspace.ContentPanel.ClientSize.Width - 1,
				workspace.ContentPanel.ClientSize.Height - 21);

			editor.Visible = false;

			tabStrip.Iterator.Changed += OnDocumentChanged;
			tabStrip.Changed += delegate { DocumentCountChanged(); OnDocumentChanged(this, EventArgs.Empty); };
			OnDocumentChanged(null, EventArgs.Empty);

			editor.UndoCapabilityChanged += delegate { UndoCapabilityChanged(); };
			UndoCapabilityChanged();
			DocumentCountChanged();

			LoadFilesFromCommandLine();

			Activated += delegate { ProtectedCheckForModifiedDocuments(); };
			editor.OnCaretMoved += delegate { SetStyleStatus(); };

			Visible = true;

			if (editor.Visible)
				editor.Focus();
		}

		Document Document
		{
			get { return editor.Document; }
			set { editor.Document = value; }
		}

		void DocumentCountChanged()
		{
			foreach (ToolStripItem i in enableIfMultipleDocuments)
				i.Enabled = tabStrip.Count > 1;
		}

		void SetStyleStatus()
		{
			styleLabel.Text = Document.Point.Style;
		}

		void UndoCapabilityChanged()
		{
			foreach (ToolStripItem i in enableIfUndo)
				i.Enabled = Document != null && Document.CanUndo;

			foreach (ToolStripItem i in enableIfRedo)
				i.Enabled = Document != null && Document.CanRedo;
		}

		void LoadDocument(string filename)
		{
			Document d = tabStrip.GetDocument( filename );
			if( d == null )
				tabStrip.Add( new Document( filename, false ) );
			else
				tabStrip.Select( d );
		}

		void LoadDocumentEx(string filename)
		{
			try
			{
				LoadDocument(filename);
			}
			catch (FileNotFoundException fnf)
			{
				MessageBox.Show(fnf.Message);
			}
		}

		void LoadFilesFromCommandLine()
		{
			foreach (string filename in Config.FilesPassedOnCommandLine())
				LoadDocumentEx(filename);
		}

		List<ToolStripItem> enableIfDocument = new List<ToolStripItem>();
		List<ToolStripItem> enableIfUndo = new List<ToolStripItem>();
		List<ToolStripItem> enableIfRedo = new List<ToolStripItem>();
		List<ToolStripItem> enableIfMultipleDocuments = new List<ToolStripItem>();

		void HookEnabled(ToolStripItem i, XmlElement e)
		{
			if (i == null)
				return;

			string ehook = e.GetAttribute("ehook");
			if (ehook == "doc")
				enableIfDocument.Add(i);
			else if (ehook == "undo")
				enableIfUndo.Add(i);
			else if (ehook == "redo")
				enableIfRedo.Add(i);
			else if (ehook == "mdoc")
				enableIfMultipleDocuments.Add(i);
		}

		void BuildUI()
		{
			BindFunctions();

			Dictionary<string, Image> images = new Dictionary<string, Image>();

			XmlDocument doc = new XmlDocument();
			doc.Load(Config.GetAbsolutePath("/res/ui.xml"));

			foreach (XmlElement e in doc.SelectNodes("/ui/images/image"))
				images.Add(e.GetAttribute("name"), Image.FromFile(
					Config.GetAbsolutePath(e.GetAttribute("path"))));

			MenuBuilder menuBuilder = new MenuBuilder(workspace, images);
			ToolbarBuilder toolbarBuilder = new ToolbarBuilder(workspace, images);

			foreach (XmlElement e in doc.SelectNodes("/ui/menu/menu-item"))
				HookEnabled(menuBuilder.CreateMenu(e.GetAttribute("path"), GetNamedHandler(e.GetAttribute("handler")),
					e.GetAttribute("image"), Util.ParseShortcutKey(e.GetAttribute("shortcut"))), e);

			foreach (XmlElement e in doc.SelectNodes("/ui/toolbar/toolbar-button"))
				HookEnabled(toolbarBuilder.CreateButton(e.GetAttribute("image"), e.GetAttribute("text"),
					GetNamedHandler(e.GetAttribute("handler"))), e);

			findBar = new FindBar(workspace, editor);
		}

		void OpenFiles( IEnumerable<string> files )
		{
			foreach (string filename in files)
				LoadDocumentEx(filename);
		}

		static void SaveAs( Document document )
		{
			if (document == null)
				return;

			using (SaveFileDialog fd = new SaveFileDialog())
			{
				fd.RestoreDirectory = true;
				if( document.FilePath != null )
				{
					fd.FileName = Util.CanonicalizePath(document.FilePath);
					if( fd.ShowDialog() != DialogResult.OK )
						return;
				}
				else
				{
					fd.FileName = Path.Combine( Config.DefaultSaveLocation, document.Filename );
					if( fd.ShowDialog() != DialogResult.OK )
						return;

					Config.DefaultSaveLocation = Path.GetDirectoryName( fd.FileName );
				}

				document.SaveAs(fd.FileName);
			}
		}

		public static void Save( Document document )
		{
			if (document.FilePath != null)
				document.Save();
			else
				SaveAs(document);
		}

		void SetWindowTitle(Document d)
		{
			if (d != null)
				Text = d.Filename + " - " + baseTitle;
			else
				Text = baseTitle;
		}

		void OnDocumentChanged(object sender, EventArgs e)
		{
			Document document = tabStrip.Current;
			editor.Visible = document != null;
			Document = document;

			if (document != null)
			{
				documentTypeLabel.Text = "Document Type: " +
					((document.FileType != null) ? document.FileType.Name : "(unrecognised)");
				editor.Focus();
			}
			else
				documentTypeLabel.Text = "";

			SetWindowTitle(document);

			foreach (ToolStripItem i in enableIfDocument)
				i.Enabled = document != null;
		}

		void OnCloseWindow(object sender, FormClosingEventArgs e)
		{
			e.Cancel = !tabStrip.CloseAll();
		}

		protected override void OnDragEnter( DragEventArgs dragEvent )
		{
			foreach( string s in dragEvent.Data.GetFormats() )
				if( s == DataFormats.FileDrop )
					dragEvent.Effect = DragDropEffects.Copy;

			base.OnDragEnter( dragEvent );
		}

		protected override void OnDragDrop( DragEventArgs dragEvent )
		{
			string[] files = dragEvent.Data.GetData( DataFormats.FileDrop ) as string[];
			if( files == null || files.Length == 0 )
				return;

			OpenFiles( files );
			base.OnDragDrop( dragEvent );
		}

		bool inModCheck = false;

		void ProtectedCheckForModifiedDocuments()
		{
			if (inModCheck)
				return;
			inModCheck = true;
			CheckForModifiedDocuments();
			inModCheck = false;
		}

		void CheckForModifiedDocuments()
		{
			List<Document> modified = new List<Document>();
			foreach (Document d in tabStrip.Documents)
				if (d.Stale)
					modified.Add(d);

			if (modified.Count == 0)
				return;

			ExternalChangesDialog ecd = new ExternalChangesDialog(modified);
			if (DialogResult.Cancel == ecd.ShowDialog())
			{
				foreach (Document d in modified)
					d.Stale = false;
				return;
			}

			foreach (Document d in ecd.GetSelectedDocuments())
			{
				string fn = d.FilePath;
				tabStrip.Close(d, true);
				LoadDocument(fn);
			}
		}
	}
}
