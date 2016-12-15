using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RSM.Support;
using RSM.Support.PeopleSoft;
using System.IO;

namespace RSM
{
    class Program
    {
        static void Main(string[] args)
        {
            int numFiles = 0;
            bool requireApproval = true;

            if(args.Count() == 0)
            {
                Console.WriteLine("Usage: RSMImporter <folder_with_CSV>");
                return;
            }

            string ArcFolder = Path.Combine(args[0], "archive");

            if (!(Directory.Exists(ArcFolder)))
            {
                Directory.CreateDirectory(ArcFolder);
            }

            string ErrFolder = Path.Combine(args[0], "error");

            if (!(Directory.Exists(ErrFolder)))
            {
                Directory.CreateDirectory(ErrFolder);
            }

            if (args.Length > 1)
            {
                requireApproval = (args[1].ToLower() == "green") ? true : false;
            }

            string[] files = Directory.GetFiles(args[0]);

            FileStream f;
            PeopleSoftImporter imp = new PeopleSoftImporter();
            string newFile;

            foreach (string filename in files)
            {
                try
                {
                    Console.Out.WriteLine("Processing {0}...", filename);
                    f = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
                    // If we could open it with exclusive rights then it's done transferring
                    f.Close();

                    if (imp.ImportCSV(filename))
                    {
                        numFiles++;
                        newFile = GetUniqueName(Path.GetFileName(filename), ArcFolder);
                    }
                    else
                    {
                        
                        newFile = GetUniqueName(Path.GetFileName(filename), ErrFolder);
                    }

                    
                    if (newFile != null)
                        File.Move(filename, newFile);

                }
                catch (Exception)
                {
                    // skip the file
                }
            }

            if (numFiles > 0)
            {
                imp.ImportNewDepts();
                imp.ImportNewJobs();
                imp.ImportNewLocations();
                RoleAssignmentEngine engine = new RoleAssignmentEngine(requireApproval);
                engine.ProcessDirtyPeople();
            }

        }

        static string GetUniqueName(string file, string folder)
        {
            string fullFile = Path.Combine(folder, file);

            int x = 1;
            if (!File.Exists(fullFile))
                return fullFile;

            while (x < int.MaxValue )
            {
                fullFile = Path.Combine(folder, file + "." + x.ToString());
                if (!File.Exists(fullFile))
                    return fullFile;

                x++;
            }
            
            // No way in hell this is ever getting hit.
            return null;
        }
    }
}
