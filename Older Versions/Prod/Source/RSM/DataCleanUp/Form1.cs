using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DataCleanUp
{
    public partial class Form1 : Form
    {
        DataCleaner _dataCleaner;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = "C:\\prj\\resourceone";
            ofd.Filter = "csv file|*.csv|all files|*.*";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _dataCleaner = new DataCleaner(ofd.FileName);
                _dataCleaner.LoadFile();
                lblRecordCount.Text = _dataCleaner.RecordCount.ToString();
            }
        }

        private void btnDups_Click(object sender, EventArgs e)
        {
            lblDupes.Text = _dataCleaner.FindDuplicates().ToString();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnMismatch_Click(object sender, EventArgs e)
        {
            lblMismatch.Text = _dataCleaner.FindMismatchedImages().ToString();
            lblRematched.Text = _dataCleaner.FixMismatchedImages().ToString();
            lblTooMany.Text = _dataCleaner.TooMany.ToString();
            lblNoMatch.Text = _dataCleaner.NoMatch.ToString();

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _dataCleaner.SaveFile();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = "C:\\prj\\resourceone";
            ofd.Filter = "csv file|*.csv|all files|*.*";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _dataCleaner = new DataCleaner(ofd.FileName);
                _dataCleaner.LoadHotStampFile();
                lblRecordCount.Text = _dataCleaner.RecordCount.ToString();
            }
        }
    }
}

