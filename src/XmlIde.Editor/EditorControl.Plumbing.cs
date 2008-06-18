using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using XmlIde.Editor.Commands;

namespace XmlIde.Editor
{
	partial class EditorControl
	{
		protected KeyBindings keyBindings = new KeyBindings();

		void SetKeyBindings()
		{
			keyBindings.Offer("MoveLeft", SelectionMovement(_ => document.MovePoint(Direction.Left)));
			keyBindings.Offer("MoveRight", SelectionMovement(_ => document.MovePoint(Direction.Right)));
			keyBindings.Offer("MoveUp", SelectionMovement(_ => document.MovePoint(Direction.Up)));
			keyBindings.Offer("MoveDown", SelectionMovement(_ => document.MovePoint(Direction.Down)));

			keyBindings.Offer("MoveByWordLeft", SelectionMovement(_ => MoveByWord(Direction.Left)));
			keyBindings.Offer("MoveByWordRight", SelectionMovement(_ => MoveByWord(Direction.Right)));

			keyBindings.Offer("MoveDocumentStart", SelectionMovement(_ => document.MovePoint(Direction.DocumentStart)));
			keyBindings.Offer("MoveDocumentEnd", SelectionMovement(_ => document.MovePoint(Direction.DocumentEnd)));

			keyBindings.Offer("MoveLineStart", SelectionMovement(_ => document.MovePoint(Direction.LineStart)));
			keyBindings.Offer("MoveLineEnd", SelectionMovement(_ => document.MovePoint(Direction.LineEnd)));

			keyBindings.Offer("MoveLineUp", SelectionMovement(_ => document.MovePoint(Direction.Up, VisibleLines)));
			keyBindings.Offer("MoveLineDown", SelectionMovement(_ => document.MovePoint(Direction.Down, VisibleLines)));
			keyBindings.Offer("Unselect", _ => document.Mark = document.Point);
			keyBindings.Offer("ScrollUp", _ => FirstVisibleLine = document.ClampLineNumber(FirstVisibleLine - 1));
			keyBindings.Offer("ScrollDown", _ => FirstVisibleLine = document.ClampLineNumber(FirstVisibleLine + 1));

			keyBindings.Offer("Delete", _ =>
			{
				if (document.Mark == document.Point)
					document.Apply(new OneSidedDelete(document, false));
				else
					document.Apply(new ReplaceText(document, null));
			});

			keyBindings.Offer("KillLine", _ =>
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

			keyBindings.Offer("Indent", _ =>
			{
				if (document.Point.Line != document.Mark.Line)
					document.Apply(new IndentBlockCommand(document));
				else
					document.Apply(new ReplaceText(document, "\t"));
			});

			keyBindings.Offer("Unindent", _ =>
			{
				if (document.Point.Line != document.Mark.Line)
					document.Apply(new UnindentBlockCommand(document));
				else if (document.Point == document.Mark)
					document.Apply(new UntabCommand(document));
			});

			keyBindings.Offer("RefreshStyles", _ =>
			{
				keyBindings.Reload();
				styleProvider.Reload();
				gutter.Reload();
				Invalidate();
			});
		}

		static Regex moveByWordPatternForward = new Regex(@"^(\s+|[a-zA-Z0-9_]+|.)\s*");
		static Regex moveByWordPatternBackward = new Regex(@"^\s*([a-zA-Z0-9_]+|.)?");

		void MoveByWord(Direction d)
		{
			if (d != Direction.Left && d != Direction.Right)
				throw new InvalidOperationException("You're doing it wrong.");

			var frag = (d == Direction.Left) ? document.Point.TextBefore.Reverse() : document.Point.TextAfter;
			var pattern = (d == Direction.Left) ? moveByWordPatternBackward : moveByWordPatternForward;

			document.MovePoint(d, pattern.Match(frag).Length.Clamp(1, int.MaxValue));
		}

		Action<Keys> SelectionMovement(Action<Keys> d)
		{
			return k =>
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
			if (e.Button != MouseButtons.Left || !isSelecting) return;

			InvalidateSelectedRegion(document.Selection);
			document.Point = Caret.AtVirtualPosition(document, GetVirtualPosition(e.Location));
			if (updateMark) document.Mark = document.Point;
			EnsureVisible();
			InvalidateSelectedRegion(document.Selection);
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
				MoveByWord(Direction.Left);
				document.Mark = document.Point;
			}

			MoveByWord(Direction.Right);
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