namespace TestSupport
{
	partial class Staging
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
			this.SettingsGroup = new System.Windows.Forms.GroupBox();
			this.R1SMSettings = new System.Windows.Forms.Button();
			this.S2InSettings = new System.Windows.Forms.Button();
			this.TrackOutSettings = new System.Windows.Forms.Button();
			this.ElementsGroup = new System.Windows.Forms.GroupBox();
			this.StageReaders = new System.Windows.Forms.Button();
			this.StagePortals = new System.Windows.Forms.Button();
			this.StageLocations = new System.Windows.Forms.Button();
			this.StageExternalSystems = new System.Windows.Forms.Button();
			this.StagePeople = new System.Windows.Forms.Button();
			this.Closer = new System.Windows.Forms.Button();
			this.StatusText = new System.Windows.Forms.TextBox();
			this.ClearButton = new System.Windows.Forms.Button();
			this.ExitAllButton = new System.Windows.Forms.Button();
			this.StageAccessHistory = new System.Windows.Forms.Button();
			this.SettingsGroup.SuspendLayout();
			this.ElementsGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// SettingsGroup
			// 
			this.SettingsGroup.Controls.Add(this.R1SMSettings);
			this.SettingsGroup.Controls.Add(this.S2InSettings);
			this.SettingsGroup.Controls.Add(this.TrackOutSettings);
			this.SettingsGroup.Location = new System.Drawing.Point(240, 12);
			this.SettingsGroup.Name = "SettingsGroup";
			this.SettingsGroup.Size = new System.Drawing.Size(118, 110);
			this.SettingsGroup.TabIndex = 20;
			this.SettingsGroup.TabStop = false;
			this.SettingsGroup.Text = "Settings";
			// 
			// R1SMSettings
			// 
			this.R1SMSettings.Location = new System.Drawing.Point(6, 19);
			this.R1SMSettings.Name = "R1SMSettings";
			this.R1SMSettings.Size = new System.Drawing.Size(101, 23);
			this.R1SMSettings.TabIndex = 14;
			this.R1SMSettings.Text = "R1SM";
			this.R1SMSettings.UseVisualStyleBackColor = true;
			this.R1SMSettings.Click += new System.EventHandler(this.R1SMSettingsClick);
			// 
			// S2InSettings
			// 
			this.S2InSettings.Location = new System.Drawing.Point(6, 74);
			this.S2InSettings.Name = "S2InSettings";
			this.S2InSettings.Size = new System.Drawing.Size(101, 23);
			this.S2InSettings.TabIndex = 13;
			this.S2InSettings.Text = "S2 - In";
			this.S2InSettings.UseVisualStyleBackColor = true;
			this.S2InSettings.Click += new System.EventHandler(this.S2InSettingsClick);
			// 
			// TrackOutSettings
			// 
			this.TrackOutSettings.Location = new System.Drawing.Point(6, 48);
			this.TrackOutSettings.Name = "TrackOutSettings";
			this.TrackOutSettings.Size = new System.Drawing.Size(101, 23);
			this.TrackOutSettings.TabIndex = 12;
			this.TrackOutSettings.Text = "Track - Out";
			this.TrackOutSettings.UseVisualStyleBackColor = true;
			this.TrackOutSettings.Click += new System.EventHandler(this.TrackOutSettingsClick);
			// 
			// ElementsGroup
			// 
			this.ElementsGroup.Controls.Add(this.StageAccessHistory);
			this.ElementsGroup.Controls.Add(this.StageReaders);
			this.ElementsGroup.Controls.Add(this.StagePortals);
			this.ElementsGroup.Controls.Add(this.StageLocations);
			this.ElementsGroup.Controls.Add(this.StageExternalSystems);
			this.ElementsGroup.Controls.Add(this.StagePeople);
			this.ElementsGroup.Location = new System.Drawing.Point(12, 12);
			this.ElementsGroup.Name = "ElementsGroup";
			this.ElementsGroup.Size = new System.Drawing.Size(222, 110);
			this.ElementsGroup.TabIndex = 21;
			this.ElementsGroup.TabStop = false;
			this.ElementsGroup.Text = "Data Elements";
			// 
			// StageReaders
			// 
			this.StageReaders.Location = new System.Drawing.Point(113, 19);
			this.StageReaders.Name = "StageReaders";
			this.StageReaders.Size = new System.Drawing.Size(101, 23);
			this.StageReaders.TabIndex = 18;
			this.StageReaders.Text = "Readers";
			this.StageReaders.UseVisualStyleBackColor = true;
			this.StageReaders.Click += new System.EventHandler(this.StageReadersClick);
			// 
			// StagePortals
			// 
			this.StagePortals.Location = new System.Drawing.Point(6, 74);
			this.StagePortals.Name = "StagePortals";
			this.StagePortals.Size = new System.Drawing.Size(101, 23);
			this.StagePortals.TabIndex = 17;
			this.StagePortals.Text = "Portals";
			this.StagePortals.UseVisualStyleBackColor = true;
			this.StagePortals.Click += new System.EventHandler(this.StagePortalsClick);
			// 
			// StageLocations
			// 
			this.StageLocations.Location = new System.Drawing.Point(6, 48);
			this.StageLocations.Name = "StageLocations";
			this.StageLocations.Size = new System.Drawing.Size(101, 23);
			this.StageLocations.TabIndex = 16;
			this.StageLocations.Text = "Locations";
			this.StageLocations.UseVisualStyleBackColor = true;
			this.StageLocations.Click += new System.EventHandler(this.StageLocationsClick);
			// 
			// StageExternalSystems
			// 
			this.StageExternalSystems.Location = new System.Drawing.Point(6, 19);
			this.StageExternalSystems.Name = "StageExternalSystems";
			this.StageExternalSystems.Size = new System.Drawing.Size(101, 23);
			this.StageExternalSystems.TabIndex = 15;
			this.StageExternalSystems.Text = "External Systems";
			this.StageExternalSystems.UseVisualStyleBackColor = true;
			this.StageExternalSystems.Click += new System.EventHandler(this.StageExternalSystemsClick);
			// 
			// StagePeople
			// 
			this.StagePeople.Location = new System.Drawing.Point(113, 48);
			this.StagePeople.Name = "StagePeople";
			this.StagePeople.Size = new System.Drawing.Size(101, 23);
			this.StagePeople.TabIndex = 14;
			this.StagePeople.Text = "People";
			this.StagePeople.UseVisualStyleBackColor = true;
			this.StagePeople.Click += new System.EventHandler(this.StagePeopleClick);
			// 
			// Closer
			// 
			this.Closer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.Closer.BackColor = System.Drawing.SystemColors.Control;
			this.Closer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Closer.ForeColor = System.Drawing.Color.Maroon;
			this.Closer.Location = new System.Drawing.Point(371, 60);
			this.Closer.Name = "Closer";
			this.Closer.Size = new System.Drawing.Size(101, 23);
			this.Closer.TabIndex = 22;
			this.Closer.Text = "Close";
			this.Closer.UseVisualStyleBackColor = false;
			this.Closer.Click += new System.EventHandler(this.CloserClick);
			// 
			// StatusText
			// 
			this.StatusText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.StatusText.BackColor = System.Drawing.Color.MidnightBlue;
			this.StatusText.ForeColor = System.Drawing.Color.White;
			this.StatusText.Location = new System.Drawing.Point(12, 144);
			this.StatusText.Multiline = true;
			this.StatusText.Name = "StatusText";
			this.StatusText.ReadOnly = true;
			this.StatusText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.StatusText.Size = new System.Drawing.Size(460, 218);
			this.StatusText.TabIndex = 23;
			// 
			// ClearButton
			// 
			this.ClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ClearButton.BackColor = System.Drawing.SystemColors.Control;
			this.ClearButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ClearButton.ForeColor = System.Drawing.Color.MidnightBlue;
			this.ClearButton.Location = new System.Drawing.Point(371, 31);
			this.ClearButton.Name = "ClearButton";
			this.ClearButton.Size = new System.Drawing.Size(101, 23);
			this.ClearButton.TabIndex = 24;
			this.ClearButton.Text = "Clear";
			this.ClearButton.UseVisualStyleBackColor = false;
			this.ClearButton.Click += new System.EventHandler(this.ClearButtonClick);
			// 
			// ExitAllButton
			// 
			this.ExitAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ExitAllButton.BackColor = System.Drawing.SystemColors.Control;
			this.ExitAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ExitAllButton.ForeColor = System.Drawing.Color.Maroon;
			this.ExitAllButton.Location = new System.Drawing.Point(371, 86);
			this.ExitAllButton.Name = "ExitAllButton";
			this.ExitAllButton.Size = new System.Drawing.Size(101, 23);
			this.ExitAllButton.TabIndex = 25;
			this.ExitAllButton.Text = "Exit All";
			this.ExitAllButton.UseVisualStyleBackColor = false;
			this.ExitAllButton.Click += new System.EventHandler(this.ExitAllButtonClick);
			// 
			// StageAccessHistory
			// 
			this.StageAccessHistory.Location = new System.Drawing.Point(113, 74);
			this.StageAccessHistory.Name = "StageAccessHistory";
			this.StageAccessHistory.Size = new System.Drawing.Size(101, 23);
			this.StageAccessHistory.TabIndex = 19;
			this.StageAccessHistory.Text = "Access History";
			this.StageAccessHistory.UseVisualStyleBackColor = true;
			this.StageAccessHistory.Click += new System.EventHandler(this.StageAccessHistoryClick);
			// 
			// Staging
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(484, 374);
			this.Controls.Add(this.ExitAllButton);
			this.Controls.Add(this.ClearButton);
			this.Controls.Add(this.StatusText);
			this.Controls.Add(this.Closer);
			this.Controls.Add(this.ElementsGroup);
			this.Controls.Add(this.SettingsGroup);
			this.Name = "Staging";
			this.Text = "R1SM Data Staging Module";
			this.SettingsGroup.ResumeLayout(false);
			this.ElementsGroup.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox SettingsGroup;
		private System.Windows.Forms.Button S2InSettings;
		private System.Windows.Forms.Button TrackOutSettings;
		private System.Windows.Forms.GroupBox ElementsGroup;
		private System.Windows.Forms.Button StageExternalSystems;
		private System.Windows.Forms.Button StagePeople;
		private System.Windows.Forms.Button Closer;
		private System.Windows.Forms.Button StageLocations;
		private System.Windows.Forms.Button StagePortals;
		private System.Windows.Forms.Button StageReaders;
		private System.Windows.Forms.Button R1SMSettings;
		private System.Windows.Forms.TextBox StatusText;
		private System.Windows.Forms.Button ClearButton;
		private System.Windows.Forms.Button ExitAllButton;
		private System.Windows.Forms.Button StageAccessHistory;
	}
}