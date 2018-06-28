using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowPositionPersisterLibrary
{

	internal static class WindowPositionFlags
	{
		internal const uint SWP_ASYNCWINDOWPOS = 0x4000; // call async
		internal const uint SWP_DEFERERASE = 0x2000;  // prevent generation of repaint message
		internal const uint SWP_NOACTIVATE = 0x0010;  // don't activate
		internal const uint SWP_NOCOPYBITS = 0x0100;  // umm.. sounds like this should be passed (don't copy client area back and forth)
													  //internal const uint SWP_NOOWNERZORDER = 0x0200;  // don't change z order.
		internal const uint SWP_NOZORDER = 0x0004;  // ignore z order param

		internal static uint Flags => SWP_ASYNCWINDOWPOS | SWP_DEFERERASE | SWP_NOACTIVATE | SWP_NOCOPYBITS | SWP_NOZORDER;
	}

	public static class Native
	{
		internal delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lParam);
		internal delegate IntPtr HookHandlerDelegate(int nCode, IntPtr wParam, ref KeyInfoStruct lParam);

		[DllImport("user32.dll")]
		internal static extern int EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lParam);

		[DllImport("user32.dll")]
		internal static extern int GetClassName(IntPtr hWnd, [Out] StringBuilder class_name, int length);

		[DllImport("user32.dll")]
		internal static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder title, int length);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("user32.dll")]
		internal static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, UInt32 uFlags);

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;        // x position of upper-left corner
			public int Top;         // y position of upper-left corner
			public int Right;       // x position of lower-right corner
			public int Bottom;      // y position of lower-right corner
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct KeyInfoStruct
		{
			public int vkCode;
			public int flags;
		}

		public struct /*what's the*/ POINT
		{
			public Int32 x;
			public Int32 Y;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct MSG
		{
			public IntPtr hwnd;
			public UInt32 message;
			public UIntPtr wParam;
			public UIntPtr lParam;
			public UInt32 time;
			public POINT pt;
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr SetWindowsHookEx(int idHook, HookHandlerDelegate lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KeyInfoStruct lParam);

		[DllImport("user32.dll")]
		internal static extern int GetMessage(out MSG msg, IntPtr hWnd, uint mMin, uint mMax);

		[DllImport("user32.dll")]
		internal static extern void TranslateMessage(ref MSG msg);

		[DllImport("user32.dll")]
		internal static extern void DispatchMessage(ref MSG msg);

		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(string className, string windowText);
		[DllImport("user32.dll")]
		public static extern int ShowWindow(IntPtr hwnd, int command);

		public const int SW_HIDE = 0;
		public const int SW_SHOW = 1;

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SystemParametersInfo(
												int uiAction,
												int uiParam,
												ref RECT pvParam,
												int fWinIni);

		public const Int32 SPIF_SENDWININICHANGE = 2;
		public const Int32 SPIF_UPDATEINIFILE = 1;
		public const Int32 SPIF_change = SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE;
		public const Int32 SPI_SETWORKAREA = 47;
		public const Int32 SPI_GETWORKAREA = 48;

		[DllImport("kernel32.dll")]
		public static extern IntPtr GetConsoleWindow();
	}

}
