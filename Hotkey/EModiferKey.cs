using System;
using System.Collections.Generic;
using System.Text;

namespace GodLesZ.Library.Hotkey {

	[Flags()]
	public enum EModiferKey : int {
		None = 0,

		Alt = 1,
		Contol = 2,
		Shift = 4,
		Win = 8
	}

}
