namespace Editor
{
	partial class EditorForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorForm));
			this.workspace = new System.Windows.Forms.ToolStripContainer();
			this.documentTypeLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.workspace.BottomToolStripPanel.SuspendLayout();
			this.workspace.SuspendLayout();
			this.SuspendLayout();
			// 
			// workspace
			// 
			// workspace.ContentPanel
			// 
			this.workspace.ContentPanel.AutoScroll = true;
			this.workspace.ContentPanel.Size = new System.Drawing.Size(822, 470);
			this.workspace.Dock = System.Windows.Forms.DockStyle.Fill;
			this.workspace.Location = new System.Drawing.Point(0, 0);
			this.workspace.Name = "workspace";
			this.workspace.Size = new System.Drawing.Size(822, 517);
			this.workspace.TabIndex = 4;
			this.workspace.Text = "toolStripContainer1";
			// 
			// documentTypeLabel
			// 
			this.documentTypeLabel.Name = "documentTypeLabel";
			this.documentTypeLabel.Size = new System.Drawing.Size(0, 17);
			// 
			// EditorForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(822, 517);
			this.Controls.Add(this.workspace);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "EditorForm";
			this.Text = "EditorForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnCloseWindow);
			this.workspace.BottomToolStripPanel.ResumeLayout(false);
			this.workspace.BottomToolStripPanel.PerformLayout();
			this.workspace.ResumeLayout(false);
			this.workspace.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripContainer workspace;
		private System.Windows.Forms.ToolStripStatusLabel documentTypeLabel;


	}
}
