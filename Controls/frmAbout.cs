using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace GodLesZ.Library.Controls {
	partial class frmAbout : Form {

		public frmAbout(Form parent, ELanguage lang, IEnumerable<string> text) {
			InitializeComponent();

			// header
			Text = String.Format("About {0}", AssemblyTitle);
			int i = 0;
			int lastY = label3.Location.Y;
			Graphics g = Graphics.FromHwnd(parent.Handle);
			foreach (string line in text) {
				lastY += 16;
				Label lbl = new Label();
				lbl.AutoSize = true;
				lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				lbl.Location = new System.Drawing.Point((int)Width / 2 - (int)g.MeasureString(line, lbl.Font).Width / 2, lastY);
				lbl.Name = "dynLabel" + i;
				lbl.TabIndex = 27 + i;
				lbl.Text = line;

				i++;
			}
		}


		private void okButton_Click(object sender, EventArgs e) {
			Close();
		}



		public string AssemblyTitle {
			get {
				object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (customAttributes.Length > 0) {
					AssemblyTitleAttribute attribute = (AssemblyTitleAttribute)customAttributes[0];
					if (attribute.Title != "")
						return attribute.Title;
				}
				return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}


	}

}
