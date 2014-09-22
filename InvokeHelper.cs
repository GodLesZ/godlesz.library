
namespace GodLesZ.Library {

	public class InvokeHelper {
		public delegate void InvokeDelegate();

		public static void Invoke(System.Windows.Forms.Control con, InvokeDelegate callback) {
			if (con.InvokeRequired) {
				con.Invoke(callback);
			} else {
				callback();
			}
		}

		public static void Invoke(System.Windows.Forms.ToolStripItem con, InvokeDelegate callback) {
			callback();
		}

	}

}
