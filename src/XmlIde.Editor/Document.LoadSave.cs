using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace XmlIde.Editor
{
	partial class Document
	{
		bool needSaveAs;
		readonly FileType fileType;

		public FileType FileType { get { return fileType; } }

		IEnumerable<string> GetFileContent(string path)
		{
			try
			{
				return File.ReadAllLines(path);
			}
			catch (IOException)
			{
				// make a shadow copy of the file
				this.needSaveAs = true;
				string shadowName = Path.GetTempFileName();
				File.Copy(path, shadowName, true);

				return File.ReadAllLines(shadowName);
			}
		}

		public Document(string filePath, bool needSaveAs)
		{
			this.needSaveAs = needSaveAs;
		
			if (!File.Exists(filePath))
				throw new FileNotFoundException("\"{0}\" does not exist.".F(filePath), filePath);

			try
			{
				foreach (string line in GetFileContent(filePath))
					lines.Add(new Line(line, LineModification.Clean, this));
			}
			catch
			{
				throw new FileNotFoundException("\"{0}\" could not be opened.".F(filePath), filePath);
			}

			point = new Caret(this);
			mark = new Caret(this);

			this.filePath = filePath;
			this.fileType = Config.ChooseFileType(filePath);

			if (lines.Count == 0)
				lines.Add(new Line("", LineModification.Clean, this));

			lastModified = File.GetLastWriteTime(filePath);
		}

		public void Save() { SaveAs(FilePath); }
		public void SaveAs(string path)
		{
			using (TextWriter writer = new StreamWriter(path))
			{
				writer.NewLine = Environment.NewLine;

				foreach (Line line in lines)
					writer.WriteLine(line.Text);

				writer.Flush();
			}

			FilePath = path;
			needSaveAs = false;
			Dirty = false;

			lastModified = File.GetLastWriteTime(FilePath);
		}

		string filePath;

		public string Filename { get { return Path.GetFileName(filePath); } }

		public string FilePath
		{
			get { return needSaveAs ? null : filePath; }
			set
			{
				if (value != filePath)
				{
					filePath = value;
					dirty = true;
					FilenameChanged();
				}
			}
		}

		bool dirty = false;
		public bool Dirty
		{
			get { return dirty; }

			internal set
			{
				if (dirty == value) return;
				dirty = value;

				if (!dirty)
					foreach (Line line in lines)
						if (line.Dirty == LineModification.Unsaved)
							line.Dirty = LineModification.Saved;

				DirtyChanged();
			}
		}

		public event MethodInvoker DirtyChanged = delegate { };
		public event MethodInvoker FilenameChanged = delegate { };

		DateTime lastModified;

		public string DisplayName { get { return Filename + (Dirty ? "*" : ""); } }

		public bool Stale
		{
			get
			{
				if (FilePath == null)
					return false;

				return lastModified < File.GetLastWriteTime(FilePath);
			}

			set
			{
				if (FilePath == null)
					throw new InvalidOperationException("A document that has not yet been saved cannot be stale");
				lastModified = File.GetLastWriteTime(FilePath);
			}
		}
	}
}
