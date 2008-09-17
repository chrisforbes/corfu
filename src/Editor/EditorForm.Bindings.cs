using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Ijw.Updates;
using IjwFramework;
using IjwFramework.Types;
using XmlIde.Editor;
using XmlIde.Editor.Commands;

namespace Editor
{
	partial class EditorForm
	{
		Dictionary<string, MethodInvoker> bindableFunctions = new Dictionary<string, MethodInvoker>();

		void BindFunctions()
		{
			bindableFunctions.Add("New", New);
			bindableFunctions.Add("Open", Open);
			bindableFunctions.Add("Save", Save);
			bindableFunctions.Add("CloseFile", CloseFile);
			bindableFunctions.Add("Undo", Undo);
			bindableFunctions.Add("Redo", Redo);
			bindableFunctions.Add("Cut", Cut);
			bindableFunctions.Add("Copy", Copy);
			bindableFunctions.Add("Paste", Paste);
			bindableFunctions.Add("SelectAll", SelectAll);
			bindableFunctions.Add("Close", Close);
			bindableFunctions.Add("SaveAll", SaveAll);
			bindableFunctions.Add("SaveAs", SaveAs);
			bindableFunctions.Add("About", About);
			bindableFunctions.Add("ShowFindBar", ShowFindBar);
			bindableFunctions.Add("CloseAll", CloseAll);
			bindableFunctions.Add("NextDocument", NextDocument);
			bindableFunctions.Add("PreviousDocument", PreviousDocument);
			bindableFunctions.Add("PasteFromHistory", PasteFromHistory);
			bindableFunctions.Add("SwapMarkAndPoint", SwapMarkAndPoint);
			bindableFunctions.Add("ReloadStylers", ReloadStylers);
			bindableFunctions.Add("CheckForUpdates", CheckForUpdates);
		}

		List<Pair<string, string>> clipboardHistory = new List<Pair<string, string>>();

		EventHandler GetNamedHandler(string name)
		{
			if (string.IsNullOrEmpty(name))
				return null;

			MethodInvoker result;
			if (!bindableFunctions.TryGetValue(name, out result))
			{
				MessageBox.Show("Function binding for `{0}` does not exist".F(name));
				return null;
			}

			return delegate { result(); };
		}

		void SwapMarkAndPoint()
		{
			if (Document == null)
				return;

			var carets = Document.GetCarets();
			Document.SetCarets(carets.Swap());
			editor.Invalidate();
		}

		void NextDocument() { tabStrip.Iterator.MoveNext(); }
		void PreviousDocument() { tabStrip.Iterator.MovePrevious(); }
		void ShowFindBar() { findBar.Show(); }

		void PasteFromHistory()
		{
			var dialog = new ClipboardHistory(clipboardHistory);
			if (DialogResult.OK == dialog.ShowDialog())
			{
				Document.Apply(new ReplaceText(Document, dialog.SelectedItem.First.Replace("\r\n", "\n"), false));
				clipboardHistory.Remove(dialog.SelectedItem);
				clipboardHistory.Insert(0, dialog.SelectedItem);
				editor.Invalidate();
			}
		}

		void New()
		{
			var newItemForm = new NewItemForm();

			if( newItemForm.ShowDialog() != DialogResult.OK )
				return;

			tabStrip.Add( newItemForm.NewDocument );
		}

		void Open()
		{
			using (var fd = new OpenFileDialog())
			{
				fd.RestoreDirectory = true;
				fd.Multiselect = true;

				if (fd.ShowDialog() == DialogResult.OK)
					OpenFiles(fd.FileNames);
			}
		}

		void Cut()
		{
			if (Document == null)
				return;

			if (Document.Mark == Document.Point)
			{
				Document.MovePoint(Direction.AbsoluteLineStart);
				Document.Mark = Document.Point;
				Document.MovePoint(Direction.Down);
			}

			CopySelection();
			Document.Apply(new ReplaceText(Document, null));
			editor.Invalidate();
		}

		void Copy()
		{
			if( Document == null )
				return;

			if( Document.Mark == Document.Point )
			{
				Caret oldPoint = Document.Point;
				Document.MovePoint( Direction.AbsoluteLineStart );
				Document.Mark = Document.Point;
				Document.MovePoint( Direction.Down );
				CopySelection();
				Document.Point = oldPoint;
			}
			else
				CopySelection();
		}

		void CopySelection()
		{
			var s = Document.Selection.Content;
			if( string.IsNullOrEmpty( s ) )
				return;

			var historyItem = s.PairedWith(Document.Filename);

			clipboardHistory.Remove( historyItem );
			clipboardHistory.Insert( 0, historyItem );
			Clipboard.SetText( s );
		}

		void Paste()
		{
			if (Document == null)
				return;

			if (Clipboard.ContainsText())
			{
				Document.Apply(new ReplaceText(Document,
					Clipboard.GetText().Replace("\r\n", "\n"), false));
				editor.EnsureVisible();
				editor.Invalidate();
			}
		}

		void Undo()
		{
			if (Document == null)
				return;

			Document.Undo();
			editor.EnsureVisible();
			editor.Invalidate();
		}

		void Redo()
		{
			if (Document == null)
				return;

			Document.Redo();
			editor.EnsureVisible();
			editor.Invalidate();
		}

		void SelectAll()
		{
			if (Document == null)
				return;

			Document.Mark = Caret.AtStartOfDocument(Document);
			Document.Point = Caret.AtEndOfDocument(Document);

			editor.EnsureVisible();
			editor.Invalidate();
		}

		void CloseFile()
		{
			tabStrip.CloseCurrent();
		}

		void SaveAs()
		{
			SaveAs(Document);
			editor.Invalidate();
		}

		void Save()
		{
			Save(Document);
			editor.Invalidate();
		}

		void SaveAll()
		{
			foreach( Document document in tabStrip.Documents )
				Save( document );

			editor.Invalidate();
		}

		void About()
		{
			new AboutBox().ShowDialog();
		}

		void CloseAll()
		{
			tabStrip.CloseAll();
		}

		void ReloadStylers()
		{
			foreach (Document d in tabStrip.Documents)
				d.ReloadStyler();

			editor.Invalidate();
		}

		void CheckForUpdates()
		{
			UpdateManager.CheckForUpdates("Corfu", Product.ShortVersion);
		}
	}
}
