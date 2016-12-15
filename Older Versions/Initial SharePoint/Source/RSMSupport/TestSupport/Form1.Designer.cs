namespace TestSupport
{
    partial class Form1
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
			this.ImportPeopleFromPS = new System.Windows.Forms.Button();
			this.txtOutput = new System.Windows.Forms.TextBox();
			this.GetLevelIDsFromS2 = new System.Windows.Forms.Button();
			this.GetLevelOneFromS2 = new System.Windows.Forms.Button();
			this.ImportLevelsFromS2 = new System.Windows.Forms.Button();
			this.btnExportPerson = new System.Windows.Forms.Button();
			this.btnGetPerson = new System.Windows.Forms.Button();
			this.ImportJobsFromPS = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.button6 = new System.Windows.Forms.Button();
			this.button7 = new System.Windows.Forms.Button();
			this.button8 = new System.Windows.Forms.Button();
			this.GetPictureFromS2 = new System.Windows.Forms.Button();
			this.button10 = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnGetPeople = new System.Windows.Forms.Button();
			this.GetHistories = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.personPicture = new System.Windows.Forms.PictureBox();
			this.Stager = new System.Windows.Forms.Button();
			this.ExitAllButton = new System.Windows.Forms.Button();
			this.ClearButton = new System.Windows.Forms.Button();
			this.Closer = new System.Windows.Forms.Button();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.ExportHistoryToTrack = new System.Windows.Forms.Button();
			this.OpenServiceTester = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.personPicture)).BeginInit();
			this.groupBox5.SuspendLayout();
			this.SuspendLayout();
			// 
			// ImportPeopleFromPS
			// 
			this.ImportPeopleFromPS.Location = new System.Drawing.Point(6, 19);
			this.ImportPeopleFromPS.Name = "ImportPeopleFromPS";
			this.ImportPeopleFromPS.Size = new System.Drawing.Size(117, 23);
			this.ImportPeopleFromPS.TabIndex = 0;
			this.ImportPeopleFromPS.Text = "Import People";
			this.ImportPeopleFromPS.UseVisualStyleBackColor = true;
			this.ImportPeopleFromPS.Click += new System.EventHandler(this.ImportPeopleFromPSClick);
			// 
			// txtOutput
			// 
			this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtOutput.BackColor = System.Drawing.Color.Navy;
			this.txtOutput.ForeColor = System.Drawing.Color.White;
			this.txtOutput.Location = new System.Drawing.Point(12, 160);
			this.txtOutput.Multiline = true;
			this.txtOutput.Name = "txtOutput";
			this.txtOutput.ReadOnly = true;
			this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtOutput.Size = new System.Drawing.Size(496, 209);
			this.txtOutput.TabIndex = 1;
			// 
			// GetLevelIDsFromS2
			// 
			this.GetLevelIDsFromS2.Location = new System.Drawing.Point(118, 77);
			this.GetLevelIDsFromS2.Name = "GetLevelIDsFromS2";
			this.GetLevelIDsFromS2.Size = new System.Drawing.Size(101, 23);
			this.GetLevelIDsFromS2.TabIndex = 2;
			this.GetLevelIDsFromS2.Text = "Get Level IDs";
			this.GetLevelIDsFromS2.UseVisualStyleBackColor = true;
			this.GetLevelIDsFromS2.Click += new System.EventHandler(this.GetLevelIDsFromS2Click);
			// 
			// GetLevelOneFromS2
			// 
			this.GetLevelOneFromS2.Location = new System.Drawing.Point(118, 106);
			this.GetLevelOneFromS2.Name = "GetLevelOneFromS2";
			this.GetLevelOneFromS2.Size = new System.Drawing.Size(101, 23);
			this.GetLevelOneFromS2.TabIndex = 3;
			this.GetLevelOneFromS2.Text = "Get Level 1";
			this.GetLevelOneFromS2.UseVisualStyleBackColor = true;
			this.GetLevelOneFromS2.Click += new System.EventHandler(this.GetLevelOneFromS2Click);
			// 
			// ImportLevelsFromS2
			// 
			this.ImportLevelsFromS2.Location = new System.Drawing.Point(6, 19);
			this.ImportLevelsFromS2.Name = "ImportLevelsFromS2";
			this.ImportLevelsFromS2.Size = new System.Drawing.Size(101, 23);
			this.ImportLevelsFromS2.TabIndex = 4;
			this.ImportLevelsFromS2.Text = "Import Levels";
			this.ImportLevelsFromS2.UseVisualStyleBackColor = true;
			this.ImportLevelsFromS2.Click += new System.EventHandler(this.ImportLevelsFromS2Click);
			// 
			// btnExportPerson
			// 
			this.btnExportPerson.Location = new System.Drawing.Point(16, 19);
			this.btnExportPerson.Name = "btnExportPerson";
			this.btnExportPerson.Size = new System.Drawing.Size(101, 23);
			this.btnExportPerson.TabIndex = 5;
			this.btnExportPerson.Text = "Export All People";
			this.btnExportPerson.UseVisualStyleBackColor = true;
			this.btnExportPerson.Click += new System.EventHandler(this.ExportPersonClick);
			// 
			// btnGetPerson
			// 
			this.btnGetPerson.Location = new System.Drawing.Point(118, 19);
			this.btnGetPerson.Name = "btnGetPerson";
			this.btnGetPerson.Size = new System.Drawing.Size(101, 23);
			this.btnGetPerson.TabIndex = 6;
			this.btnGetPerson.Text = "Get Person";
			this.btnGetPerson.UseVisualStyleBackColor = true;
			this.btnGetPerson.Click += new System.EventHandler(this.GetPersonClick);
			// 
			// ImportJobsFromPS
			// 
			this.ImportJobsFromPS.Location = new System.Drawing.Point(6, 48);
			this.ImportJobsFromPS.Name = "ImportJobsFromPS";
			this.ImportJobsFromPS.Size = new System.Drawing.Size(117, 23);
			this.ImportJobsFromPS.TabIndex = 7;
			this.ImportJobsFromPS.Text = "Import Jobs";
			this.ImportJobsFromPS.UseVisualStyleBackColor = true;
			this.ImportJobsFromPS.Click += new System.EventHandler(this.ImportJobsFromPSClick);
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(6, 106);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(117, 23);
			this.button5.TabIndex = 8;
			this.button5.Text = "Import Departments";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point(6, 77);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(117, 23);
			this.button6.TabIndex = 9;
			this.button6.Text = "Import Locations";
			this.button6.UseVisualStyleBackColor = true;
			this.button6.Click += new System.EventHandler(this.button6_Click);
			// 
			// button7
			// 
			this.button7.Location = new System.Drawing.Point(6, 48);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(101, 23);
			this.button7.TabIndex = 10;
			this.button7.Text = "Test Rules";
			this.button7.UseVisualStyleBackColor = true;
			this.button7.Click += new System.EventHandler(this.button7_Click);
			// 
			// button8
			// 
			this.button8.Location = new System.Drawing.Point(6, 19);
			this.button8.Name = "button8";
			this.button8.Size = new System.Drawing.Size(101, 23);
			this.button8.TabIndex = 11;
			this.button8.Text = "Process Dirty";
			this.button8.UseVisualStyleBackColor = true;
			this.button8.Click += new System.EventHandler(this.button8_Click);
			// 
			// GetPictureFromS2
			// 
			this.GetPictureFromS2.Location = new System.Drawing.Point(118, 48);
			this.GetPictureFromS2.Name = "GetPictureFromS2";
			this.GetPictureFromS2.Size = new System.Drawing.Size(101, 23);
			this.GetPictureFromS2.TabIndex = 12;
			this.GetPictureFromS2.Text = "Get Picture";
			this.GetPictureFromS2.UseVisualStyleBackColor = true;
			this.GetPictureFromS2.Click += new System.EventHandler(this.GetPictureFromS2Click);
			// 
			// button10
			// 
			this.button10.Location = new System.Drawing.Point(6, 77);
			this.button10.Name = "button10";
			this.button10.Size = new System.Drawing.Size(101, 23);
			this.button10.TabIndex = 13;
			this.button10.Text = "Display Creds";
			this.button10.UseVisualStyleBackColor = true;
			this.button10.Click += new System.EventHandler(this.button10_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.ImportPeopleFromPS);
			this.groupBox1.Controls.Add(this.ImportJobsFromPS);
			this.groupBox1.Controls.Add(this.button6);
			this.groupBox1.Controls.Add(this.button5);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(138, 142);
			this.groupBox1.TabIndex = 14;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "From PeopleSoft";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnGetPeople);
			this.groupBox2.Controls.Add(this.GetHistories);
			this.groupBox2.Controls.Add(this.GetLevelIDsFromS2);
			this.groupBox2.Controls.Add(this.GetLevelOneFromS2);
			this.groupBox2.Controls.Add(this.ImportLevelsFromS2);
			this.groupBox2.Controls.Add(this.GetPictureFromS2);
			this.groupBox2.Controls.Add(this.btnGetPerson);
			this.groupBox2.Location = new System.Drawing.Point(156, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(225, 142);
			this.groupBox2.TabIndex = 15;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "From S2";
			// 
			// btnGetPeople
			// 
			this.btnGetPeople.Location = new System.Drawing.Point(6, 77);
			this.btnGetPeople.Name = "btnGetPeople";
			this.btnGetPeople.Size = new System.Drawing.Size(101, 23);
			this.btnGetPeople.TabIndex = 14;
			this.btnGetPeople.Text = "Get People";
			this.btnGetPeople.UseVisualStyleBackColor = true;
			this.btnGetPeople.Click += new System.EventHandler(this.btnGetPeople_Click);
			// 
			// GetHistories
			// 
			this.GetHistories.Location = new System.Drawing.Point(6, 48);
			this.GetHistories.Name = "GetHistories";
			this.GetHistories.Size = new System.Drawing.Size(101, 23);
			this.GetHistories.TabIndex = 13;
			this.GetHistories.Text = "Get Histories";
			this.GetHistories.UseVisualStyleBackColor = true;
			this.GetHistories.Click += new System.EventHandler(this.GetHistoriesClick);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.button8);
			this.groupBox3.Controls.Add(this.button7);
			this.groupBox3.Controls.Add(this.button10);
			this.groupBox3.Location = new System.Drawing.Point(387, 12);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(121, 142);
			this.groupBox3.TabIndex = 16;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "R1SM";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.btnExportPerson);
			this.groupBox4.Location = new System.Drawing.Point(514, 12);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(140, 56);
			this.groupBox4.TabIndex = 17;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "To S2";
			// 
			// personPicture
			// 
			this.personPicture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.personPicture.Location = new System.Drawing.Point(514, 160);
			this.personPicture.Name = "personPicture";
			this.personPicture.Size = new System.Drawing.Size(259, 209);
			this.personPicture.TabIndex = 18;
			this.personPicture.TabStop = false;
			// 
			// Stager
			// 
			this.Stager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.Stager.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Stager.ForeColor = System.Drawing.Color.DarkGreen;
			this.Stager.Location = new System.Drawing.Point(668, 40);
			this.Stager.Name = "Stager";
			this.Stager.Size = new System.Drawing.Size(101, 23);
			this.Stager.TabIndex = 12;
			this.Stager.Text = "Stage Data";
			this.Stager.UseVisualStyleBackColor = true;
			this.Stager.Click += new System.EventHandler(this.StagerClick);
			// 
			// ExitAllButton
			// 
			this.ExitAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ExitAllButton.BackColor = System.Drawing.SystemColors.Control;
			this.ExitAllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ExitAllButton.ForeColor = System.Drawing.Color.Maroon;
			this.ExitAllButton.Location = new System.Drawing.Point(668, 127);
			this.ExitAllButton.Name = "ExitAllButton";
			this.ExitAllButton.Size = new System.Drawing.Size(101, 23);
			this.ExitAllButton.TabIndex = 28;
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
			this.ClearButton.Location = new System.Drawing.Point(668, 69);
			this.ClearButton.Name = "ClearButton";
			this.ClearButton.Size = new System.Drawing.Size(101, 23);
			this.ClearButton.TabIndex = 27;
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
			this.Closer.Location = new System.Drawing.Point(668, 98);
			this.Closer.Name = "Closer";
			this.Closer.Size = new System.Drawing.Size(101, 23);
			this.Closer.TabIndex = 26;
			this.Closer.Text = "Close";
			this.Closer.UseVisualStyleBackColor = false;
			this.Closer.Click += new System.EventHandler(this.CloseButtonClick);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.ExportHistoryToTrack);
			this.groupBox5.Location = new System.Drawing.Point(515, 75);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(139, 79);
			this.groupBox5.TabIndex = 29;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "groupBox5";
			// 
			// ExportHistoryToTrack
			// 
			this.ExportHistoryToTrack.Location = new System.Drawing.Point(15, 19);
			this.ExportHistoryToTrack.Name = "ExportHistoryToTrack";
			this.ExportHistoryToTrack.Size = new System.Drawing.Size(101, 23);
			this.ExportHistoryToTrack.TabIndex = 6;
			this.ExportHistoryToTrack.Text = "Export History";
			this.ExportHistoryToTrack.UseVisualStyleBackColor = true;
			this.ExportHistoryToTrack.Click += new System.EventHandler(this.ExportHistoryToTrackClick);
			// 
			// OpenServiceTester
			// 
			this.OpenServiceTester.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.OpenServiceTester.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.OpenServiceTester.ForeColor = System.Drawing.Color.Olive;
			this.OpenServiceTester.Location = new System.Drawing.Point(668, 12);
			this.OpenServiceTester.Name = "OpenServiceTester";
			this.OpenServiceTester.Size = new System.Drawing.Size(101, 23);
			this.OpenServiceTester.TabIndex = 30;
			this.OpenServiceTester.Text = "Service Tester";
			this.OpenServiceTester.UseVisualStyleBackColor = true;
			this.OpenServiceTester.Click += new System.EventHandler(this.OpenServiceTester_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(781, 381);
			this.Controls.Add(this.OpenServiceTester);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.ExitAllButton);
			this.Controls.Add(this.ClearButton);
			this.Controls.Add(this.Closer);
			this.Controls.Add(this.Stager);
			this.Controls.Add(this.personPicture);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.txtOutput);
			this.Name = "Form1";
			this.Text = "R1SM Test Module";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.personPicture)).EndInit();
			this.groupBox5.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ImportPeopleFromPS;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button GetLevelIDsFromS2;
        private System.Windows.Forms.Button GetLevelOneFromS2;
        private System.Windows.Forms.Button ImportLevelsFromS2;
        private System.Windows.Forms.Button btnExportPerson;
        private System.Windows.Forms.Button btnGetPerson;
        private System.Windows.Forms.Button ImportJobsFromPS;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button GetPictureFromS2;
        private System.Windows.Forms.Button button10;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.PictureBox personPicture;
		private System.Windows.Forms.Button GetHistories;
		private System.Windows.Forms.Button Stager;
		private System.Windows.Forms.Button ExitAllButton;
		private System.Windows.Forms.Button ClearButton;
		private System.Windows.Forms.Button Closer;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Button ExportHistoryToTrack;
		private System.Windows.Forms.Button btnGetPeople;
		private System.Windows.Forms.Button OpenServiceTester;
    }
}

