

namespace GodLesZ.Library.Win7.Shell {
	/// <summary>
	/// Represents a non filesystem item (e.g. virtual items inside Control Panel)
	/// </summary>
	public class ShellNonFileSystemItem : ShellObject {
		internal ShellNonFileSystemItem(IShellItem2 shellItem) {
			nativeShellItem = shellItem;
		}
	}
}
