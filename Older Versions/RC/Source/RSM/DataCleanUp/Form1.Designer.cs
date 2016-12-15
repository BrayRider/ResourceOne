namespace DataCleanUp
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
            this.btnImport = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblRecordCount = new System.Windows.Forms.Label();
            this.btnDups = new System.Windows.Forms.Button();
            this.lblDupes = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnMismatch = new System.Windows.Forms.Button();
            this.lblMismatch = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblRematched = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblTooMany = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblNoMatch = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(12, 12);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 0;
            this.btnImport.Text = "Load Data";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(98, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "# of Records:";
            // 
            // lblRecordCount
            // 
            this.lblRecordCount.AutoSize = true;
            this.lblRecordCount.Location = new System.Drawing.Point(176, 74);
            this.lblRecordCount.Name = "lblRecordCount";
            this.lblRecordCount.Size = new System.Drawing.Size(13, 13);
            this.lblRecordCount.TabIndex = 2;
            this.lblRecordCount.Text = "0";
            // 
            // btnDups
            // 
            this.btnDups.Location = new System.Drawing.Point(121, 11);
            this.btnDups.Name = "btnDups";
            this.btnDups.Size = new System.Drawing.Size(75, 23);
            this.btnDups.TabIndex = 3;
            this.btnDups.Text = "Dups";
            this.btnDups.UseVisualStyleBackColor = true;
            this.btnDups.Click += new System.EventHandler(this.btnDups_Click);
            // 
            // lblDupes
            // 
            this.lblDupes.AutoSize = true;
            this.lblDupes.Location = new System.Drawing.Point(176, 102);
            this.lblDupes.Name = "lblDupes";
            this.lblDupes.Size = new System.Drawing.Size(13, 13);
            this.lblDupes.TabIndex = 5;
            this.lblDupes.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(113, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "# of Dups:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // btnMismatch
            // 
            this.btnMismatch.Location = new System.Drawing.Point(202, 12);
            this.btnMismatch.Name = "btnMismatch";
            this.btnMismatch.Size = new System.Drawing.Size(75, 23);
            this.btnMismatch.TabIndex = 6;
            this.btnMismatch.Text = "Mismatch";
            this.btnMismatch.UseVisualStyleBackColor = true;
            this.btnMismatch.Click += new System.EventHandler(this.btnMismatch_Click);
            // 
            // lblMismatch
            // 
            this.lblMismatch.AutoSize = true;
            this.lblMismatch.Location = new System.Drawing.Point(176, 129);
            this.lblMismatch.Name = "lblMismatch";
            this.lblMismatch.Size = new System.Drawing.Size(13, 13);
            this.lblMismatch.TabIndex = 8;
            this.lblMismatch.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(90, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "# of Mismtatch:";
            // 
            // lblRematched
            // 
            this.lblRematched.AutoSize = true;
            this.lblRematched.Location = new System.Drawing.Point(176, 153);
            this.lblRematched.Name = "lblRematched";
            this.lblRematched.Size = new System.Drawing.Size(13, 13);
            this.lblRematched.TabIndex = 10;
            this.lblRematched.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(100, 153);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "# rematched:";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(283, 11);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblTooMany
            // 
            this.lblTooMany.AutoSize = true;
            this.lblTooMany.Location = new System.Drawing.Point(176, 178);
            this.lblTooMany.Name = "lblTooMany";
            this.lblTooMany.Size = new System.Drawing.Size(13, 13);
            this.lblTooMany.TabIndex = 13;
            this.lblTooMany.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(112, 178);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Too Many:";
            // 
            // lblNoMatch
            // 
            this.lblNoMatch.AutoSize = true;
            this.lblNoMatch.Location = new System.Drawing.Point(176, 209);
            this.lblNoMatch.Name = "lblNoMatch";
            this.lblNoMatch.Size = new System.Drawing.Size(13, 13);
            this.lblNoMatch.TabIndex = 15;
            this.lblNoMatch.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(113, 209);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "No Match:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(332, 55);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(134, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "LoadHotStamp";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 266);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblNoMatch);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblTooMany);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblRematched);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblMismatch);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnMismatch);
            this.Controls.Add(this.lblDupes);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnDups);
            this.Controls.Add(this.lblRecordCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnImport);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblRecordCount;
        private System.Windows.Forms.Button btnDups;
        private System.Windows.Forms.Label lblDupes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnMismatch;
        private System.Windows.Forms.Label lblMismatch;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblRematched;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblTooMany;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblNoMatch;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button1;
    }
}

