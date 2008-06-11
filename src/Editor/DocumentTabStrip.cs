using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using XmlIde.Editor;
using IjwFramework.Ui;
using System.Drawing.Drawing2D;

namespace Editor
{
	using DRegion = System.Drawing.Region;

	class DocumentTabStrip : Control
	{
		readonly List<Tab> tabs = new List<Tab>();
		readonly TabIterator iterator;

		public TabIterator Iterator { get { return iterator; } }
		public int Count { get { return tabs.Count; } }

		CloseBox closeBox;

		public event EventHandler Changed = delegate { };

		public DocumentTabStrip()
		{
			BackColor = SystemColors.ButtonFace;
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			UpdateStyles();

			closeBox = new CloseBox(this);
			closeBox.Clicked += delegate { CloseCurrent(); };
			iterator = new TabIterator(this);

			Changed += delegate { closeBox.Visible = (Current != null); };
		}

		public IEnumerable<Document> Documents
		{
			get
			{
				foreach (Tab tab in tabs)
					yield return tab.Document;
			}
		}

		Tab GetTab(Document document)
		{
			if (document == null)
				throw new ArgumentNullException("document");

			foreach (Tab tab in tabs)
				if (tab.Document == document)
					return tab;

			return null;
		}

		internal Tab GetTab(int index) { return (index < 0) ? null : tabs[index]; }

		public Document Current
		{
			get
			{
				if (iterator.Current == null)
					return null;
				return iterator.Current.Document;
			}
		}

		public void Add(Document document)
		{
			Tab tab = GetTab(document);
			if (tab == null)
				tabs.Add(tab = new Tab(document, this));

			Changed(this, EventArgs.Empty);
			Select( document );
		}

		public void Select( Document document )
		{
			Tab tab = GetTab( document );
			if( tab != null )
				iterator.Current = tab;
		}

		public Document GetDocument(string path)
		{
			Tab hax = tabs.Find(delegate(Tab t)
			{
				return ( t.Document.FilePath != null ) && 
					Util.PathsEqual( path, t.Document.FilePath );
			});

			return hax == null ? 
				null : hax.Document;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Graphics g = e.Graphics;

			ControlPaint.DrawBorder(g,
				ClientRectangle,
				SystemColors.ButtonShadow,
				ButtonBorderStyle.Solid);

			DRegion oldClip = e.Graphics.Clip;
			e.Graphics.IntersectClip(new Rectangle(1, 1, ClientSize.Width - ClientSize.Height, ClientSize.Height + 1));

			int x = 1;
			foreach (Tab d in tabs)
				d.Paint(g, ref x, iterator.Current == d, ClientRectangle);

			e.Graphics.Clip = oldClip;

			closeBox.Paint(g);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			Tab tab = GetTab(e.Location);
			if (tab == null)
				return;

			switch (e.Button)
			{
				case MouseButtons.Left:
					iterator.Current = tab;
					break;
				case MouseButtons.Middle:
					Close(tab.Document, false);
					break;

				default:
					break;
			}
		}

		bool ConfirmCloseAction(IEnumerable<Document> items)
		{
			List<Document> dirtyDocuments = new List<Document>();
			foreach (Document d in items)
				if (d.Dirty)
					dirtyDocuments.Add(d);

			if (dirtyDocuments.Count == 0)
				return true;

			CloseConfirmationDialog dialog = new CloseConfirmationDialog(dirtyDocuments);
			switch (dialog.ShowDialog())
			{
				case DialogResult.Cancel:
					return false;

				case DialogResult.Yes:
					foreach( Document document in dialog.GetSelectedDocuments() )
						EditorForm.Save( document );
					return true;

				case DialogResult.No:
					return true;

				default:
					return true;
			}
		}

		public bool CloseAll()
		{
			if (!ConfirmCloseAction(Documents))
				return false;

			tabs.Clear();
			iterator.Current = null;
			Changed(this, EventArgs.Empty);
			return true;
		}

		public void CloseCurrent() { Close(Current, false); }

		public void Close(Document d, bool suppressSave)
		{
			if (d == null)
				return;

			if (!suppressSave)
				if (!ConfirmCloseAction(new Document[] { d }))
					return;

			Tab tab = GetTab(d);
			tabs.Remove(tab);
			tab.Dispose();
			Changed(this, EventArgs.Empty);
			Invalidate();
		}

		Tab GetTab(Point p)
		{
			foreach (Tab t in tabs)
				if (t.Bounds.Contains(p))
					return t;

			return null;
		}

		internal int IndexOf(Tab t) { return tabs.IndexOf(t); }

		internal void DocumentFilenameChanged()
		{
			Changed(this, EventArgs.Empty);
		}
	}
}
