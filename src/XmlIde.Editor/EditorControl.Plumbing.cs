using System;
using System.Windows.Forms;
using XmlIde.Editor.Commands;
using System.Drawing;
using IjwFramework.Types;

namespace XmlIde.Editor
{
	partial class EditorControl
	{
		protected KeyBindings keyBindings = new KeyBindings();

		void SetKeyBindings()
		{
			keyBindings.Offer("MoveLeft", SelectionMovement(delegate { document.MovePoint(Direction.Left); }));
			keyBindings.Offer("MoveRight", SelectionMovement(delegate { document.MovePoint(Direction.Right); }));
			keyBindings.Offer("MoveUp", SelectionMovement(delegate { document.MovePoint(Direction.Up); }));
			keyBindings.Offer("MoveDown", SelectionMovement(delegate { document.MovePoint(Direction.Down); }));

			keyBindings.Offer("MoveByWordLeft", SelectionMovement(delegate { MoveByWord(new ReverseLineNavigator(document)); }));
			keyBindings.Offer("MoveByWordRight", SelectionMovement(delegate { MoveByWord(new ForwardLineNavigator(document)); }));

			keyBindings.Offer("MoveDocumentStart", SelectionMovement(delegate { document.MovePoint(Direction.DocumentStart); }));
			keyBindings.Offer("MoveDocumentEnd", SelectionMovement(delegate { document.MovePoint(Direction.DocumentEnd); }));

			keyBindings.Offer("MoveLineStart", SelectionMovement(delegate { document.MovePoint(Direction.LineStart); }));
			keyBindings.Offer("MoveLineEnd", SelectionMovement(delegate { document.MovePoint(Direction.LineEnd); }));

			keyBindings.Offer("MoveLineUp", SelectionMovement(delegate { document.MovePoint(Direction.Up, VisibleLines); }));
			keyBindings.Offer("MoveLineDown", SelectionMovement(delegate { document.MovePoint(Direction.Down, VisibleLines); }));
			keyBindings.Offer("Unselect", delegate { document.Mark = document.Point; });
			keyBindings.Offer("ScrollUp", delegate { FirstVisibleLine = document.ClampLineNumber(FirstVisibleLine - 1); });
			keyBindings.Offer("ScrollDown", delegate { FirstVisibleLine = document.ClampLineNumber(FirstVisibleLine + 1); });

			keyBindings.Offer("Delete", delegate
			{
				if (document.Mark == document.Point)
					document.Apply(new OneSidedDelete(document, false));
				else
					document.Apply(new ReplaceText(document, null));
			});

			keyBindings.Offer("KillLine", delegate
			{
				document.MovePoint(Direction.AbsoluteLineStart);
				document.Mark = document.Point;
				int line = document.Point.Line;
				document.MovePoint(Direction.Down);
				if (document.Point.Line == line)
					document.MovePoint(Direction.LineEnd);

				if (document.Mark != document.Point)
					Clipboard.SetText(document.Selection.Content);

				document.Apply(new ReplaceText(document, null));
			});

			keyBindings.Offer("Indent", delegate
			{
				if (document.Point.Line != document.Mark.Line)
					document.Apply(new IndentBlockCommand(document));
				else
					document.Apply(new ReplaceText(document, "\t"));
			});

			keyBindings.Offer("Unindent", delegate
			{
				if (document.Point.Line != document.Mark.Line)
					document.Apply(new UnindentBlockCommand(document));
				else if (document.Point == document.Mark)
					document.Apply(new UntabCommand(document));
			});

			keyBindings.Offer("RefreshStyles",
			delegate
			{
				keyBindings.Reload();
				styleProvider.Reload();
				gutter.Reload();
				Invalidate();
			});
		}

		void MoveByWord(ILineNavigator navigator)
		{
			if (navigator.AtEnd)
			{
				navigator.Move();
				return;
			}

			while (!navigator.AtEnd && char.IsWhiteSpace(navigator.NextChar))
				navigator.Move();

			if (navigator.AtEnd)
				return;

			navigator.Move();

			while (!navigator.AtEnd && char.IsLetterOrDigit(navigator.NextChar))
				navigator.Move();
		}

		Action<Keys> SelectionMovement(Action<Keys> d)
		{
			return delegate(Keys k)
			{
				InvalidateSelectedRegion(document.Selection);

				d(k);
				if ((k & Keys.Shift) == 0)
					document.Mark = document.Point;

				InvalidateSelectedRegion(document.Selection);
			};
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyBindings.OnKeyPress(keyData))
			{
				if (keyData != (Keys.Up | Keys.Control) && keyData != (Keys.Down | Keys.Control))
					EnsureVisible();
				return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		protected virtual bool HandleKey(char c)
		{
			if (!char.IsControl(c))
			{
				document.Apply(new ReplaceText(document, "" + c));
				return true;
			}

			if (c == '\n' || c == '\r')
			{
				document.Apply(new ReplaceText(document, "\n"));
				return true;
			}

			if (c == '\b')
			{
				if (document.Mark == document.Point)
					document.Apply(new OneSidedDelete(document, true));
				else
					document.Apply(new ReplaceText(document, null));

				return true;
			}

			return false;
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (HandleKey(e.KeyChar))
			{
				e.Handled = true;
				document.Mark = document.Point;
				
				EnsureVisible();
				return;
			}
			base.OnKeyPress(e);
		}

		protected override bool IsInputKey(Keys keyData) { return true; }

		public Point GetVirtualPosition(PointF pixel)
		{
			float x = pixel.X - gutter.Width + FirstVisibleColumn * geometry.CharWidth;
			int line = document.ClampLineNumber((int)pixel.Y / Font.Height + FirstVisibleLine);
			int column = geometry.GetVirtualColumn(x - geometry.CharWidth/2, document.Lines[line].Text);

			return new Point(line, column);
		}

		protected virtual void HandleMouse(MouseEventArgs e, bool updateMark)
		{
			if (e.Button == MouseButtons.Left && isSelecting)
			{
				Point location = GetVirtualPosition(e.Location);
				document.Point = Caret.AtVirtualPosition(document, location.X, location.Y);

				if (updateMark)
					document.Mark = document.Point;

				EnsureVisible();
			}
		}

		bool isSelecting;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			isSelecting = true;
			HandleMouse(e, true);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			HandleMouse(e, false);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			isSelecting = false;
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);
			HandleMouse(e, true);

			if (!document.Point.StartOfLine)
			{
				MoveByWord(new ReverseLineNavigator(document));
				document.Mark = document.Point;
			}

			MoveByWord(new ForwardLineNavigator(document));
			EnsureVisible();
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			int d = (e.Delta > 0) ? -3 : 3;
			FirstVisibleLine = document.ClampLineNumber(FirstVisibleLine + d);
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			Focus();
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			caret.Visible = true;
			Invalidate();
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			caret.Visible = false;
		}
	}
}
