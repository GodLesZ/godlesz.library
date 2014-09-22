

using MS.WindowsAPICodePack.Internal;

namespace GodLesZ.Library.Win7.Shell {
	/// <summary>
	/// Represents a saved search
	/// </summary>
	public class ShellSavedSearchCollection : ShellSearchCollection {
		internal ShellSavedSearchCollection(IShellItem2 shellItem)
			: base(shellItem) {
			CoreHelpers.ThrowIfNotVista();
		}
	}
}
