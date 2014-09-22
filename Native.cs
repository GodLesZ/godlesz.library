using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GodLesZ.Library {
	internal partial class Native {

		public delegate int EnumWindowsProcDelegate(int hWnd, int lParam);

		public const int WM_SYSCOMMAND = 0x0112;
		public const int SC_CLOSE = 0xF060;

		private const int GWL_EXSTYLE = (-20);
		private const int WS_EX_TOOLWINDOW = 0x80;
		private const int WS_EX_APPWINDOW = 0x40000;

		public const int GW_HWNDFIRST = 0;
		public const int GW_HWNDLAST = 1;
		public const int GW_HWNDNEXT = 2;
		public const int GW_HWNDPREV = 3;
		public const int GW_OWNER = 4;
		public const int GW_CHILD = 5;

		[DllImport("user32")]
		public static extern int FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32")]
		public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

		[DllImport("user32")]
		public static extern int SetForegroundWindow(int hWnd);

		[DllImport("user32")]
		public static extern int EnumWindows(EnumWindowsProcDelegate lpEnumFunc, int lParam);

		[DllImport("user32")]
		public static extern void GetWindowText(int h, StringBuilder s, int nMaxCount);

		[DllImport("user32", EntryPoint = "GetWindowLongA")]
		public static extern int GetWindowLongPtr(int hwnd, int nIndex);

		[DllImport("user32")]
		public static extern int GetParent(int hwnd);

		[DllImport("user32")]
		public static extern int GetWindow(int hwnd, int wCmd);

		[DllImport("user32")]
		public static extern int IsWindowVisible(int hwnd);

		[DllImport("user32")]
		public static extern int GetDesktopWindow();


		#region Hotkey Helper
		[DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int RegisterHotKey(IntPtr Hwnd, int ID, int Modifiers, int Key);
		[DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int UnregisterHotKey(IntPtr Hwnd, int ID);
		[DllImport("kernel32", EntryPoint = "GlobalAddAtomA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern short GlobalAddAtom(string IDString);
		[DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern short GlobalDeleteAtom(short Atom);
		#endregion

	}

}
