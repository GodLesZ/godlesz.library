using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;

namespace GodLesZ.Library {

	/// <summary>
	/// Console Color Array
	/// </summary>
	public enum EConsoleColor {
		/// <summary>
		/// Basic gray
		/// </summary>
		None = ConsoleColor.Gray,
		/// <summary>
		/// Basic gGray
		/// </summary>
		Gray = ConsoleColor.Gray,
		/// <summary>
		/// Lime green
		/// </summary>
		Status = ConsoleColor.Green,
		/// <summary>
		/// Bright white
		/// </summary>
		Info = ConsoleColor.White,
		/// <summary>
		/// Dark yellow
		/// </summary>
		Warning = ConsoleColor.Yellow,
		/// <summary>
		/// Bright magenta
		/// </summary>
		Error = ConsoleColor.Magenta,
		/// <summary>
		/// Bright cyan
		/// </summary>
		Debug = ConsoleColor.Cyan,


		// ConsoleColor orgins
		DarkBlue = ConsoleColor.DarkBlue,
		DarkGreen = ConsoleColor.DarkGreen,
		DarkCyan = ConsoleColor.DarkCyan,
		DarkRed = ConsoleColor.DarkRed,
		DarkMagenta = ConsoleColor.DarkMagenta,
		DarkYellow = ConsoleColor.DarkYellow,
		DarkGray = ConsoleColor.DarkGray,
		Blue = ConsoleColor.Blue,
		Red = ConsoleColor.Red,
		Yellow = ConsoleColor.Yellow,

		Black = ConsoleColor.Black,
	}


	/// <summary>
	/// Console Class overwrite to allow colored output
	/// </summary>
	public class CConsole {
		private static object mLock = new object();
		protected static EConsoleColor mBackgroundColor = EConsoleColor.Black;
		protected static string mPrefix = "[";
		protected static string mSufix = "] ";
		protected static bool mColoredPreSuf = true;
		protected static string mDateString = "[HH:mm:ss]";


		public static EConsoleColor BackgroundColor {
			get { return mBackgroundColor; }
			set {
				mBackgroundColor = value;
				System.Console.BackgroundColor = (ConsoleColor)value;
			}
		}
		/// <summary>
		/// Status Prefix
		/// </summary>
		public static string Prefix {
			get { return mPrefix; }
			set { mPrefix = value; }
		}
		/// <summary>
		/// Status Sufix
		/// </summary>
		public static string Sufix {
			get { return mSufix; }
			set { mSufix = value; }
		}
		/// <summary>
		/// Using Prefix/Sufix
		/// </summary>
		public static bool ColoredPreSuf {
			get { return mColoredPreSuf; }
			set { mColoredPreSuf = value; }
		}
		/// <summary>
		/// String to Format the DateTime
		/// </summary>
		public static string DateString {
			get { return mDateString; }
			set { mDateString = value; }
		}



		protected static void WriteColored(string Text, EConsoleColor ColorPart) {
			if (DateString != null) {
				string s = DateTime.Now.ToString(DateString, CultureInfo.CreateSpecificCulture("de-DE"));
				System.Console.Write(s);
			}


			if (ColoredPreSuf == true)
				Text = String.Format("{0}{1}{2}", Prefix, Text, Sufix);
			else
				System.Console.Write(Prefix);

			System.Console.ForegroundColor = (ConsoleColor)ColorPart;
			System.Console.Write(Text);
			System.Console.ResetColor();
			System.Console.BackgroundColor = (ConsoleColor)mBackgroundColor;

			if (ColoredPreSuf == false)
				System.Console.Write(Sufix);
		}


		#region base Method defines
		public static void Write(object Object) {
			System.Console.Write(Object);
		}

		public static void Write(string Text) {
			System.Console.Write(Text);
		}

		public static void Write(string Text, params object[] args) {
			Write(string.Format(Text, args));
		}

		public static void WriteLine(object Object) {
			System.Console.WriteLine(Object);
		}

		public static void WriteLine(string Text) {
			// Check for \r at the end of line
			// IN that case, we cant just write the text + line feed or the \r will be ignored
			// WE have to use simple Write() without line feed!
			if (Text.IndexOf('\r') != -1) {
				Write(Text);
				return;
			}
			System.Console.WriteLine(Text);
		}

		public static void WriteLine(string Text, params object[] args) {
			WriteLine(string.Format(Text, args));
		}

		public static int Read() {
			return System.Console.Read();
		}
		#endregion


		#region basic colored Write/WriteLine
		public static void Write(EConsoleColor ColorPart, string Text) {
			System.Console.ForegroundColor = (ConsoleColor)ColorPart;
			Write(Text);
			System.Console.ResetColor();
			System.Console.BackgroundColor = (ConsoleColor)mBackgroundColor;
		}

		public static void Write(EConsoleColor ColorPart, string Text, params object[] args) {
			System.Console.ForegroundColor = (ConsoleColor)ColorPart;
			Write(String.Format(Text, args));
			System.Console.ResetColor();
			System.Console.BackgroundColor = (ConsoleColor)mBackgroundColor;
		}

		public static void WriteLine(EConsoleColor ColorPart, string Text) {
			System.Console.ForegroundColor = (ConsoleColor)ColorPart;
			WriteLine(Text);
			System.Console.ResetColor();
			System.Console.BackgroundColor = (ConsoleColor)mBackgroundColor;
		}

		public static void WriteLine(EConsoleColor ColorPart, string Text, params object[] args) {
			System.Console.ForegroundColor = (ConsoleColor)ColorPart;
			WriteLine(String.Format(Text, args));
			System.Console.ResetColor();
			System.Console.BackgroundColor = (ConsoleColor)mBackgroundColor;
		}
		#endregion

		#region Status [EConsoleColor.Status]
		public static void Status(string Text) {
			lock (mLock) {
				WriteColored("State", EConsoleColor.Status);
				Write(Text);
			}
		}
		public static void Status(string Text, params object[] arg) {
			lock (mLock) {
				WriteColored("State", EConsoleColor.Status);
				Write(String.Format(Text, arg));
			}
		}
		public static void StatusLine(string Text) {
			lock (mLock) {
				WriteColored("State", EConsoleColor.Status);
				WriteLine(Text);
			}
		}
		public static void StatusLine(string Text, params object[] arg) {
			lock (mLock) {
				WriteColored("State", EConsoleColor.Status);
				WriteLine(String.Format(Text, arg));
			}
		}
		#endregion

		#region Info [EConsoleColor.Info]
		public static void Info(string Text) {
			lock (mLock) {
				WriteColored("Info", EConsoleColor.Info);
				Write(Text);
			}
		}
		public static void Info(string Text, params object[] arg) {
			lock (mLock) {
				WriteColored("Info", EConsoleColor.Info);
				Write(String.Format(Text, arg));
			}
		}
		public static void InfoLine(string Text) {
			lock (mLock) {
				WriteColored("Info", EConsoleColor.Info);
				WriteLine(Text);
			}
		}
		public static void InfoLine(string Text, params object[] arg) {
			lock (mLock) {
				WriteColored("Info", EConsoleColor.Info);
				WriteLine(String.Format(Text, arg));
			}
		}
		#endregion

		#region Warning [EConsoleColor.Warning]
		public static void Warning(string Text) {
			lock (mLock) {
				WriteColored("Warning", EConsoleColor.Warning);
				Write(Text);
			}
		}
		public static void Warning(string Text, params object[] arg) {
			lock (mLock) {
				WriteColored("Warning", EConsoleColor.Warning);
				Write(String.Format(Text, arg));
			}
		}
		public static void WarningLine(string Text) {
			lock (mLock) {
				WriteColored("Warning", EConsoleColor.Warning);
				WriteLine(Text);
			}
		}
		public static void WarningLine(string Text, params object[] arg) {
			lock (mLock) {
				WriteColored("Error", EConsoleColor.Warning);
				WriteLine(String.Format(Text, arg));
			}
		}
		#endregion

		#region Error [EConsoleColor.Error]
		public static void Error(string Text) {
			lock (mLock) {
				WriteColored("Error", EConsoleColor.Error);
				Write(Text);
			}
		}
		public static void Error(string Text, params object[] arg) {
			lock (mLock) {
				WriteColored("Error", EConsoleColor.Error);
				Write(String.Format(Text, arg));
			}
		}
		public static void ErrorLine(string Text) {
			lock (mLock) {
				WriteColored("Error", EConsoleColor.Error);
				WriteLine(Text);
			}
		}
		public static void ErrorLine(string Text, params object[] arg) {
			lock (mLock) {
				WriteColored("Error", EConsoleColor.Error);
				WriteLine(String.Format(Text, arg));
			}
		}
		#endregion

		#region Debug [EConsoleColor.Debug]
		public static void Debug(string Text) {
			lock (mLock) {
				WriteColored("Debug", EConsoleColor.Debug);
				Write(Text);
			}
		}
		public static void Debug(string Text, params object[] arg) {
			lock (mLock) {
				WriteColored("Debug", EConsoleColor.Debug);
				Write(String.Format(Text, arg));
			}
		}
		public static void DebugLine(string Text) {
			lock (mLock) {
				WriteColored("Debug", EConsoleColor.Debug);
				WriteLine(Text);
			}
		}
		public static void DebugLine(string Text, params object[] arg) {
			lock (mLock) {
				WriteColored("Debug", EConsoleColor.Debug);
				WriteLine(String.Format(Text, arg));
			}
		}
		#endregion

	}


}
