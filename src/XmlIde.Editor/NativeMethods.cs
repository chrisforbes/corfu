using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace XmlIde.Editor
{
	static class NativeMethods
	{
		[DllImport("user32.dll")]
		static extern int ScrollWindowEx(
			IntPtr handle,
			int dx,
			int dy,
			IntPtr rect,
			IntPtr clip,
			IntPtr updatedRegion,
			IntPtr updatedRect,
			uint flags);

		const uint swInvalidate = 0x2;

		public static void ScrollWindow(Control c, int dx, int dy)
		{
			ScrollWindowEx(c.Handle,
				dx, dy,
				IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, swInvalidate);
		}
	}
}
