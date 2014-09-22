using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GodLesZ.Library.Hotkey {

	public class GlobalHotKeyObject {
		private Keys mHotKey;
		private EModiferKey mModifier;
		private string mHotKeyID;
		private short mAtomID;

		public Keys HotKey {
			get { return mHotKey; }
			set { mHotKey = value; }
		}

		public EModiferKey Modifier {
			get { return mModifier; }
			set { mModifier = value; }
		}

		public string HotKeyID {
			get { return mHotKeyID; }
			set { mHotKeyID = value; }
		}

		public short AtomID {
			get { return mAtomID; }
			set { mAtomID = value; }
		}

		public GlobalHotKeyObject(Keys NewHotKey, EModiferKey NewModifier, string NewHotKeyID) {
			mHotKey = NewHotKey;
			mModifier = NewModifier;
			mHotKeyID = NewHotKeyID;
		}

	}

}
