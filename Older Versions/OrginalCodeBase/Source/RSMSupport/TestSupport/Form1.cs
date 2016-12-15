using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RSM.Support.IO.Csv;
using RSM.Support.SRMC;
using System.IO;
using RSM.Support.S2;
using RSM.Support;
using System.Xml;


namespace TestSupport
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static string GetUniqueName(string file, string folder)
        {
            string fullFile = Path.Combine(folder, file);

            int x = 1;
            if (!File.Exists(fullFile))
                return fullFile;

            while (x < int.MaxValue)
            {
                fullFile = Path.Combine(folder, file + "." + x.ToString());
                if (!File.Exists(fullFile))
                    return fullFile;

                x++;
            }

            // No way in hell this is ever getting hit.
            return null;
        }

        void ImportAssociates(string path, bool requireApproval)
        {
            int numFiles = 0;

            string ArcFolder = Path.Combine(path, "archive");

            if (!(Directory.Exists(ArcFolder)))
            {
                //WriteToEventLog(string.Format("Created archive folder at: {0}", ArcFolder));
                Directory.CreateDirectory(ArcFolder);
            }

            string ErrFolder = Path.Combine(path, "error");

            if (!(Directory.Exists(ErrFolder)))
            {
                //WriteToEventLog(string.Format("Created error folder at: {0}", ErrFolder));
                Directory.CreateDirectory(ErrFolder);
            }



            string[] files = Directory.GetFiles(path);

            FileStream f;
            SRMCImporter imp = new SRMCImporter();
            string newFile;

            foreach (string filename in files)
            {
                try
                {


                    f = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
                    // If we could open it with exclusive rights then it's done transferring
                    f.Close();


                    if (imp.ImportCSV(filename, requireApproval))
                    {
                        //WriteToEventLog(string.Format("Imported {0}.", filename));
                        numFiles++;
                        newFile = GetUniqueName(Path.GetFileName(filename), ArcFolder);
                    }
                    else
                    {
                        //WriteToEventLogError(string.Format("Failed to import {0}.", filename));
                        newFile = GetUniqueName(Path.GetFileName(filename), ErrFolder);
                    }


                    if (newFile != null)
                        File.Move(filename, newFile);

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }

          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ImportAssociates("C:\\temp", false);
        }

        private void btnLevels_Click(object sender, EventArgs e)
        {
            S2API api = new S2API("http://64.129.189.200/goforms/nbapi", "admin", "admin");

            txtOutput.Text = ConvertStringArrayToString(api.GetLevelIDS());

        }

        private void btnLevel1_Click(object sender, EventArgs e)
        {
            S2API api = new S2API("http://64.129.189.200/goforms/nbapi", "admin", "admin");

            txtOutput.Text = api.GetAccessLevel("2").InnerXml;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            S2API api = new S2API("http://64.129.189.200/goforms/nbapi", "admin", "admin");
            S2Importer importer = new S2Importer(api);
            importer.ImportLevels();
        }

        static string ConvertStringArrayToString(string[] array)
        {
            //
            // Concatenate all the elements into a StringBuilder.
            //
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
                builder.Append('.');
            }
            return builder.ToString();
        }
    
        private void exportPerson_Click(object sender, EventArgs e)
        {
            /*RSMDataModelDataContext db = new RSMDataModelDataContext();

            Person person = (from p in db.Persons
                             where p.PersonID == 0
                             select p).First();
            S2API api = new S2API("http://64.129.189.200/goforms/nbapi");
            api.SavePerson(person);
             */
            ExportAll();
        }

        private void ExportAll()
        {
            S2API api = new S2API("http://64.129.189.200/goforms/nbapi", "admin", "admin");
            RSMDataModelDataContext db = new RSMDataModelDataContext();

            foreach (Person person in db.Persons)
            {
                api.SavePerson(person);
            }
            txtOutput.Text  = "Done exporting";
        }


        private void button3_Click(object sender, EventArgs e)
        {
            S2API api = new S2API("http://64.129.189.200/goforms/nbapi", "admin", "admin");
            XmlNode nd =  api.GetPerson("0");
            string imgURL = nd["PICTUREURL"].InnerText ;

            txtOutput.Text = nd.InnerXml;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SRMCImporter imp = new SRMCImporter();

            int count = imp.ImportNewJobs();
            txtOutput.Text = string.Format("Imported {0} jobs.", count);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SRMCImporter imp = new SRMCImporter();

            int count = imp.ImportNewDepts();
            txtOutput.Text = string.Format("Imported {0} departments.", count);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SRMCImporter imp = new SRMCImporter();

            int count = imp.ImportNewLocations();
            txtOutput.Text = string.Format("Imported {0} locations.", count);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            RoleAssignmentEngine engine = new RoleAssignmentEngine();

            engine.ProcessPerson(6380);
            txtOutput.Text = "Done processing person";
            
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            RoleAssignmentEngine engine = new RoleAssignmentEngine();

            engine.ProcessDirtyPeople(null);
            txtOutput.Text = "Done processing dirty";
        }

        private void button9_Click(object sender, EventArgs e)
        {

            S2API api = new S2API("http://10.1.1.233/goforms/nbapi", "admin", "072159245241245031239120017047219193126250124056");
            XmlNode nd = api.GetPicture("4086");
            string fileName = nd["PICTUREURL"].InnerText;
            byte[] imageData = Convert.FromBase64String(nd["PICTURE"].InnerText);
            txtOutput.Text = fileName;

            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(imageData);
                    writer.Close();
                }
            }
            
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var ID = 2;
            var context = new RSMDataModelDataContext();
            Person person = (from p in context.Persons
                             where p.PersonID == ID
                             select p).Single();

            txtOutput.Text = person.DisplayCredentials;
        }
    }
}
