namespace Editor
{
	partial class CloseConfirmationDialog
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
			this.itemList = new System.Windows.Forms.CheckedListBox();
			this.cancelButton = new System.Windows.Forms.Button();
			this.noButton = new System.Windows.Forms.Button();
			this.yesButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// itemList
			// 
			this.itemList.FormattingEnabled = true;
			this.itemList.Location = new System.Drawing.Point(12, 12);
			this.itemList.Name = "itemList";
			this.itemList.Size = new System.Drawing.Size(346, 184);
			this.itemList.TabIndex = 0;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(283, 204);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// noButton
			// 
			this.noButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.noButton.DialogResult = System.Windows.Forms.DialogResult.No;
			this.noButton.Location = new System.Drawing.Point(202, 204);
			this.noButton.Name = "noButton";
			this.noButton.Size = new System.Drawing.Size(75, 23);
			this.noButton.TabIndex = 2;
			this.noButton.Text = "Discard";
			this.noButton.UseVisualStyleBackColor = true;
			// 
			// yesButton
			// 
			this.yesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.yesButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.yesButton.Location = new System.Drawing.Point(121, 204);
			this.yesButton.Name = "yesButton";
			this.yesButton.Size = new System.Drawing.Size(75, 23);
			this.yesButton.TabIndex = 3;
			this.yesButton.Text = "Save";
			this.yesButton.UseVisualStyleBackColor = true;
			// 
			// CloseConfirmationDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(370, 239);
			this.Controls.Add(this.yesButton);
			this.Controls.Add(this.noButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.itemList);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CloseConfirmationDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Save changes?";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckedListBox itemList;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button noButton;
		private System.Windows.Forms.Button yesButton;
	}
}