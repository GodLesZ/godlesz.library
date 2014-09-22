

using GodLesZ.Library.Win7.Shell;
namespace GodLesZ.Library.Win7.Taskbar {
	/// <summary>
	/// Interface for jump list items
	/// </summary>
	public interface IJumpListItem {
		/// <summary>
		/// Gets or sets this item's path
		/// </summary>
		string Path { get; set; }
	}

	/// <summary>
	/// Interface for jump list tasks
	/// </summary>
	public abstract class JumpListTask {
		internal abstract IShellLinkW NativeShellLink { get; }
	}
}
