namespace TestSupport
{
	partial class ServiceTester
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
			this.CallLubrizolExport = new System.Windows.Forms.Button();
			this.txtOutput = new System.Windows.Forms.TextBox();
			this.ExitAllButton = new System.Windows.Forms.Button();
			this.ClearButton = new System.Windows.Forms.Button();
			this.Closer = new System.Windows.Forms.Button();
			this.CallS2Import = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// CallLubrizolExport
			// 
			this.CallLubrizolExport.Location = new System.Drawing.Point(13, 13);
			this.CallLubrizolExport.Name = "CallLubrizolExport";
			this.CallLubrizolExport.Size = new System.Drawing.Size(133, 23);
			this.CallLubrizolExport.TabIndex = 0;
			this.CallLubrizolExport.Text = "Lubrizol Export";
			this.CallLubrizolExport.UseVisualStyleBackColor = true;
			this.CallLubrizolExport.Click += new System.EventHandler(this.CallLubrizolExport_Click);
			// 
			// txtOutput
			// 
			this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtOutput.BackColor = System.Drawing.Color.Navy;
			this.txtOutput.ForeColor = System.Drawing.Color.White;
			this.txtOutput.Location = new System.Drawing.Point(259, 3);
			this.txtOutput.Multiline = true;
			this.txtOutput.Name = "txtOutput";
			this.txtOutput.ReadOnly = true;
			this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtOutput.Size = new System.Drawing.Size(496, 306);
			this.txtOutput.TabIndex = 2;
			// 
			// ExitAllButton
			// 
			this.ExitAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ExitAllButton.BackColor = System.Drawing.SystemColors.Control;
			this.ExitAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ExitAllButton.ForeColor = System.Drawing.Color.Maroon;
			this.ExitAllButton.Location = new System.Drawing.Point(12, 277);
			this.ExitAllButton.Name = "ExitAllButton";
			this.ExitAllButton.Size = new System.Drawing.Size(101, 23);
			this.ExitAllButton.TabIndex = 31;
			this.ExitAllButton.Text = "Exit All";
			this.ExitAllButton.UseVisualStyleBackColor = false;
			this.ExitAllButton.Click += new System.EventHandler(this.ExitAllButtonClick);
			// 
			// ClearButton
			// 
			this.ClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ClearButton.BackColor = System.Drawing.SystemColors.Control;
			this.ClearButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ClearButton.ForeColor = System.Drawing.Color.MidnightBlue;
			this.ClearButton.Location = new System.Drawing.Point(12, 219);
			this.ClearButton.Name = "ClearButton";
			this.ClearButton.Size = new System.Drawing.Size(101, 23);
			this.ClearButton.TabIndex = 30;
			this.ClearButton.Text = "Clear";
			this.ClearButton.UseVisualStyleBackColor = false;
			this.ClearButton.Click += new System.EventHandler(this.ClearButtonClick);
			// 
			// Closer
			// 
			this.Closer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.Closer.BackColor = System.Drawing.SystemColors.Control;
			this.Closer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Closer.ForeColor = System.Drawing.Color.Maroon;
			this.Closer.Location = new System.Drawing.Point(12, 248);
			this.Closer.Name = "Closer";
			this.Closer.Size = new System.Drawing.Size(101, 23);
			this.Closer.TabIndex = 29;
			this.Closer.Text = "Close";
			this.Closer.UseVisualStyleBackColor = false;
			this.Closer.Click += new System.EventHandler(this.CloseButtonClick);
			// 
			// CallS2Import
			// 
			this.CallS2Import.Location = new System.Drawing.Point(12, 42);
			this.CallS2Import.Name = "CallS2Import";
			this.CallS2Import.Size = new System.Drawing.Size(133, 23);
			this.CallS2Import.TabIndex = 32;
			this.CallS2Import.Text = "S2 Import";
			this.CallS2Import.UseVisualStyleBackColor = true;
			this.CallS2Import.Click += new System.EventHandler(this.CallS2Import_Click);
			// 
			// ServiceTester
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(767, 321);
			this.Controls.Add(this.CallS2Import);
			this.Controls.Add(this.ExitAllButton);
			this.Controls.Add(this.ClearButton);
			this.Controls.Add(this.Closer);
			this.Controls.Add(this.txtOutput);
			this.Controls.Add(this.CallLubrizolExport);
			this.Name = "ServiceTester";
			this.Text = "ServiceTester";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button CallLubrizolExport;
		private System.Windows.Forms.TextBox txtOutput;
		private System.Windows.Forms.Button ExitAllButton;
		private System.Windows.Forms.Button ClearButton;
		private System.Windows.Forms.Button Closer;
		private System.Windows.Forms.Button CallS2Import;
	}
}