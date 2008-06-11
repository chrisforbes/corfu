using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using IjwFramework;
using XmlIde.Editor;

namespace Editor
{
	partial class AboutBox : Form
	{
		public AboutBox()
		{
			InitializeComponent();

			Text = "About {0}".F(ProductName);
			labelProductName.Text = ProductName;
			labelVersion.Text = Product.Version;
			labelCopyright.Text = Product.Copyright;
			labelCompanyName.Text = Product.Company;
			textBoxDescription.Text = Product.Description;
		}
	}
}
