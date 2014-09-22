using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace GodLesZ.Library {

	public class MethodInvokeHelper {

		public MethodInfo MethodInfo;
		public object MethodObject = null;
		public object[] MethodArguments = null;


		public MethodInvokeHelper(MethodInfo info)
			: this(info, null, null) {
		}

		public MethodInvokeHelper(MethodInfo info, object[] args)
			: this(info, null, args) {
		}

		public MethodInvokeHelper(MethodInfo info, object obj, object[] args) {
			MethodInfo = info;
			MethodObject = obj;
			MethodArguments = args;
		}


		/// <summary>
		/// Wrapper for <see cref="MethodInfo.Invoke();"/>
		/// </summary>
		public void Invoke() {
			MethodInfo.Invoke(MethodObject, MethodArguments);
		}

	}


	public static class MethodInvokeExtensions {

		public static void Invoke<ObjType>(this object ObjInstance, string MethodName, object[] MethodArgs) {
			MethodInfo info = typeof(ObjType).GetMethod(MethodName);
			new MethodInvokeHelper(info, ObjInstance, MethodArgs).Invoke();
		}

		public static void Invoke<ObjType>(this object ObjInstance, string MethodName) {
			ObjInstance.Invoke<ObjType>(MethodName, null);
		}

	}

}
