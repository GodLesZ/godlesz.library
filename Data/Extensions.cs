﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;

namespace GodLesZ.Library.Data {

	public static class Extensions {
		/// <summary>
		/// Extension method for adding in a bunch of parameters
		/// </summary>
		public static void AddParams(this DbCommand cmd, params object[] args) {
			foreach (var item in args) {
				AddParam(cmd, item);
			}
		}
		/// <summary>
		/// Extension for adding single parameter
		/// </summary>
		public static void AddParam(this DbCommand cmd, object item) {
			var p = cmd.CreateParameter();
			p.ParameterName = string.Format("@{0}", cmd.Parameters.Count);
			if (item == null) {
				p.Value = DBNull.Value;
			} else {
				if (item is Guid) {
					p.Value = item.ToString();
					p.DbType = DbType.String;
					p.Size = 4000;
				} else if (item is int) {
					p.Value = item;
					p.DbType = DbType.Int32;
				} else if (item is short) {
					p.Value = item;
					p.DbType = DbType.Int16;
				} else if (item is ExpandoObject) {
					var d = (IDictionary<string, object>)item;
					p.Value = d.Values.FirstOrDefault();
				} else {
					p.Value = item;
				}
				var itemString = item as string;
				if (itemString != null) {
					p.Size = itemString.Length > 4000 ? -1 : 4000;
				}
			}
			cmd.Parameters.Add(p);
		}
		/// <summary>
		/// Turns an IDataReader to a Dynamic list of things
		/// </summary>
		public static List<dynamic> ToExpandoList(this IDataReader rdr) {
			var result = new List<dynamic>();
			while (rdr.Read()) {
				result.Add(rdr.RecordToExpando());
			}
			return result;
		}
		public static dynamic RecordToExpando(this IDataReader rdr) {
			dynamic e = new ExpandoObject();
			var d = e as IDictionary<string, object>;
			for (var i = 0; i < rdr.FieldCount; i++) {
				d.Add(rdr.GetName(i), DBNull.Value.Equals(rdr[i]) ? null : rdr[i]);
			}
			return e;
		}

		/// <summary>
		/// Turns the object into an ExpandoObject
		/// </summary>
		public static dynamic ToExpando(this object o) {
			var result = new ExpandoObject();
			var d = result as IDictionary<string, object>; //work with the Expando as a Dictionary
			if (o is ExpandoObject) {
				return o; //shouldn't have to... but just in case
			}

			if (o.GetType() == typeof(NameValueCollection) || o.GetType().IsSubclassOf(typeof(NameValueCollection))) {
				var nv = (NameValueCollection)o;
				nv.Cast<string>()
					.Select(key => new KeyValuePair<string, object>(key, nv[key]))
					.ToList()
					.ForEach(d.Add);
			} else {
				var props = o.GetType().GetProperties();
				foreach (var item in props) {
					d.Add(item.Name, item.GetValue(o, null));
				}
			}
			return result;
		}

		/// <summary>
		/// Turns the object into a Dictionary
		/// </summary>
		public static IDictionary<string, object> ToDictionary(this object thingy) {
			return (IDictionary<string, object>)thingy.ToExpando();
		}

	}

}