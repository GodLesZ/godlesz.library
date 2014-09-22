using System;
using System.Runtime.InteropServices;

namespace GodLesZ.Library.Win7.Shell {
	internal static class IntPtrExtensions {
		public static T MarshalAs<T>(this IntPtr ptr) {
			return (T)Marshal.PtrToStructure(ptr, typeof(T));
		}
	}
}
