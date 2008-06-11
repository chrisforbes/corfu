namespace Editor
{
	partial class ExternalChangesDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.yesButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.itemList = new System.Windows.Forms.CheckedListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// yesButton
			// 
			this.yesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.yesButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.yesButton.Location = new System.Drawing.Point(202, 273);
			this.yesButton.Name = "yesButton";
			this.yesButton.Size = new System.Drawing.Size(75, 23);
			this.yesButton.TabIndex = 6;
			this.yesButton.Text = "Load";
			this.yesButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(283, 273);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// itemList
			// 
			this.itemList.FormattingEnabled = true;
			this.itemList.Location = new System.Drawing.Point(12, 60);
			this.itemList.Name = "itemList";
			this.itemList.Size = new System.Drawing.Size(346, 199);
			this.itemList.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(345, 33);
			this.label1.TabIndex = 7;
			this.label1.Text = "These open documents have been modified by another program. Checked items will be" +
				" reloaded.";
			// 
			// ExternalChangesDialog
			// 
			this.AcceptButton = this.yesButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(370, 308);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.yesButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.itemList);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ExternalChangesDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Reload externally modified files?";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button yesButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.CheckedListBox itemList;
		private System.Windows.Forms.Label label1;
	}
}