using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace XmlIde.Editor
{
	public class ViewCaret
	{
		readonly IntPtr handle;
		readonly Control control;

		public ViewCaret( Control control )
		{
			this.control = control;
			handle = control.Handle;
		}

		bool visible;
		public bool Visible
		{
			get { return visible; }

			set
			{
				if (value == visible)
					return;

				if (visible = value)
				{
					CreateCaret(handle, IntPtr.Zero, 0, control.Font.Height);
					ShowCaret(handle);
				}
				else
				{
					HideCaret(handle);
					DestroyCaret();
				}
			}
		}

		Point location;

		public Point Location
		{
			set
			{
				location = value;
				SetCaretPos(value.X, value.Y);

				LocationChanged(this, EventArgs.Empty);
			}

			get { return location; }
		}

		public event EventHandler<EventArgs> LocationChanged = delegate { };

		[DllImport("User32.dll")]
		static extern int CreateCaret(IntPtr handle, IntPtr bitmap, int width, int height);

		[DllImport("User32.dll")]
		static extern int ShowCaret(IntPtr handle);

		[DllImport("User32.dll")]
		static extern int HideCaret(IntPtr handle);

		[DllImport("User32.dll")]
		static extern int SetCaretPos(int X, int Y);

		[DllImport("User32.dll")]
		static extern int DestroyCaret();
	}
}
