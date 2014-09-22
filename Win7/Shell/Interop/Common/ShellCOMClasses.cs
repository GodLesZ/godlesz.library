

using System;
using System.Runtime.InteropServices;

namespace GodLesZ.Library.Win7.Shell {
	[ComImport,
	Guid(ShellIIDGuid.IShellLibrary),
	CoClass(typeof(ShellLibraryCoClass))]
	internal interface INativeShellLibrary : IShellLibrary {
	}

	[ComImport,
	ClassInterface(ClassInterfaceType.None),
	TypeLibType(TypeLibTypeFlags.FCanCreate),
	Guid(ShellCLSIDGuid.ShellLibrary)]
	internal class ShellLibraryCoClass {
	}
}
