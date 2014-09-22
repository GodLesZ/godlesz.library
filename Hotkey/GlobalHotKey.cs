using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace GodLesZ.Library.Hotkey {

	public class GlobalHotKey : IMessageFilter {

		private Form mForm;
		private const int WM_HOTKEY = 786;
		private Dictionary<short, GlobalHotKeyObject> mHotKeyList = new Dictionary<short, GlobalHotKeyObject>();
		private Dictionary<string, short> mHotKeyIDList = new Dictionary<string, short>();

		public Form OwnerForm {
			get { return mForm; }
			set { mForm = value; }
		}


		public event HotKeyPressedEventHandler HotKeyPressed;
		public delegate void HotKeyPressedEventHandler(string HotKeyID);


		public GlobalHotKey(Form Owner) {
			mForm = Owner;
			Application.AddMessageFilter(this);
		}


		public void AddHotKey(Keys KeyCode, EModiferKey Modifiers, string HotKeyID) {
			if (mHotKeyIDList.ContainsKey(HotKeyID) == true)
				return;

			short ID = Native.GlobalAddAtom(HotKeyID);
			mHotKeyIDList.Add(HotKeyID, ID);
			mHotKeyList.Add(ID, new GlobalHotKeyObject(KeyCode, Modifiers, HotKeyID));
			Native.RegisterHotKey(mForm.Handle, (int)ID, (int)mHotKeyList[ID].Modifier, (int)mHotKeyList[ID].HotKey);
		}


		public void RemoveHotKey(string HotKeyID) {
			if (mHotKeyIDList.ContainsKey(HotKeyID) == false)
				return;

			short ID = mHotKeyIDList[HotKeyID];
			mHotKeyIDList.Remove(HotKeyID);
			mHotKeyList.Remove(ID);
			Native.UnregisterHotKey(mForm.Handle, (int)ID);
			Native.GlobalDeleteAtom(ID);
		}


		public void RemoveAllHotKeys() {
			List<string> IDList = new List<string>();
			foreach (KeyValuePair<string, short> KVP in mHotKeyIDList)
				IDList.Add(KVP.Key);

			for (int i = 0; i < IDList.Count; i++)
				RemoveHotKey(IDList[i]);
		}


		public bool PreFilterMessage(ref Message m) {
			if (m.Msg == WM_HOTKEY) {
				if (HotKeyPressed != null)
					HotKeyPressed(mHotKeyList[(short)m.WParam].HotKeyID);
			}
			return false;
		}

	}

}