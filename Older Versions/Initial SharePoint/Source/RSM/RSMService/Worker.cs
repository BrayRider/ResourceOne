using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RSM.Support;
using System.Xml;
using System.Reflection;
using System.Diagnostics;
using RSM.Support.SRMC;
using RSM.Support.S2;


namespace RSM.Service
{
    class Worker
    {
        string _logSource;
        string _logName;
        string _appPath;
        S2API _api = null;
        bool _running = false;


        // These are from the web.config file
        bool _allowRuleEngine;
        bool _allowS2Import;
        bool _allowPSImport;
        string _redFolder;
        string _greenFolder;
        string _s2URI;
        string _s2RSMAccount;
        string _s2RSMPassword;
        
        
        public S2API API
        {
            get
            {
                if(_api == null)
                    _api = new S2API(_s2URI, _s2RSMAccount, _s2RSMPassword);

                return _api;
            }

        }


        public Worker()
        {
            _logSource = "R1SM";
            _logName = "Application";
            
            if (!EventLog.SourceExists(_logSource))
                EventLog.CreateEventSource(_logSource, _logName);
            //
            
            _appPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Worker)).CodeBase);
                if (_appPath.StartsWith("file:\\"))
                {
                    _appPath = _appPath.Remove(0, 6);
                }
                //WriteToEventLog("ev2");
          
                WriteToEventLog(_appPath);
                GetConfig(true);
              
        }

        void WriteToEventLog(string eventText)
        {
            try
            {
                EventLog.WriteEntry(_logSource, eventText, EventLogEntryType.Information);
            }
            catch (Exception)
            {
                // Don't crash if the event log is full
            }
        }

        void WriteToEventLogError(string eventText)
        {
            try
            {
                EventLog.WriteEntry(_logSource, eventText, EventLogEntryType.Error);
            }
            catch (Exception)
            {
                // Don't crash if the event log is full
            }
        }

        void GetConfig(bool logIt)
        {
            // We're going to read in the web.config from the RSM website and get out settings from it.
            StreamReader reader = new StreamReader( Path.Combine(_appPath, "web.config"));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(reader.ReadToEnd().Trim());
            reader.Close();

            XmlNode appSettings =  doc["configuration"]["appSettings"];
            foreach (XmlNode setting in appSettings.ChildNodes)
            {
                switch (setting.Attributes["key"].InnerText)
                {
                    case "RuleEngineAllow":
                        _allowRuleEngine = bool.Parse(setting.Attributes["value"].InnerText);
                        if(logIt)
                            WriteToEventLog(string.Format("Config: RuleEngineAllow == {0}", _allowRuleEngine));
                        break;
                    case "PSAllowImport":
                        _allowPSImport = bool.Parse(setting.Attributes["value"].InnerText);
                        if (logIt) 
                            WriteToEventLog(string.Format("Config: PSAllowImport == {0}", _allowPSImport));
                        break;
                        
                    case "RedImportFolder":
                        _redFolder  = setting.Attributes["value"].InnerText;
                        if (logIt) 
                            WriteToEventLog(string.Format("Config: RedImportFolder == {0}", _redFolder));
                        break;
                    case "GreenImportFolder":
                        _greenFolder  = setting.Attributes["value"].InnerText;
                        if (logIt) 
                            WriteToEventLog(string.Format("Config: GreenImportFolder == {0}", _greenFolder));
                        break;

                    case "S2Address":
                        _s2URI = string.Format("{0}/goforms/nbapi", setting.Attributes["value"].InnerText);
                        if (logIt) 
                            WriteToEventLog(string.Format("Config: S2Address == {0}", _s2URI));
                        break;
                    case "S2RSMAccountName":
                        _s2RSMAccount  = setting.Attributes["value"].InnerText;
                        if (logIt)
                            WriteToEventLog(string.Format("Config: S2Account == {0}", _s2RSMAccount));
                        break;

                    case "S2RSMAccountPassword":
                        _s2RSMPassword  = setting.Attributes["value"].InnerText;
                        break;

                    case "S2AllowImport":
                        _allowS2Import  = bool.Parse(setting.Attributes["value"].InnerText);
                        if (logIt)
                            WriteToEventLog(string.Format("Config: S2AllowImport == {0}", _allowS2Import));
                        break;

                }
            }
        }

        public void Run(Object stateInfo)
        {
            if (_running)
                return;
            _running = true;
            GetConfig(false);
            try
            {
                if(_allowPSImport)
                    ImportAssociates(_greenFolder, false);
            }
            catch (Exception e)
            {
                WriteToEventLogError("Exception thrown importing green folder: " + e.ToString());
            }

            try
            {
                if (_allowPSImport)
                    ImportAssociates(_redFolder, true);
            }
            catch (Exception e)
            {
                WriteToEventLogError("Exception thrown importing red folder: " + e.ToString());
            }
            
            try
            {
                if(_allowS2Import)
                    ImportS2Levels();
            }
            catch (Exception e)
            {
                WriteToEventLogError("Exception thrown importing S2 Levels: " + e.ToString());
            }

            try
            {
                RoleAssignmentEngine eng = new RoleAssignmentEngine();
                eng.ProcessDirtyPeople(null);
            }
            catch (Exception e)
            {
                WriteToEventLogError("Exception thrown processing dirty people: " + e.ToString());
            }

            try
            {
                UploadAssociates();
            }
            catch (Exception e)
            {
                WriteToEventLogError("Exception thrown uploading associates: " + e.ToString());
            }

            _running = false;
        }

        void UploadAssociates()
        {
            RSMDataModelDataContext db = new RSMDataModelDataContext();
            

            var people = (from p in db.Persons
                          where p.NeedsUpload == true
                          select p);

            if (people.Count() > 0)
            {
                try
                {

                    S2API api = new S2API(_s2URI, _s2RSMAccount, _s2RSMPassword);
                    foreach (Person p in people)
                    {
                        if (api.SavePerson(p))
                            p.NeedsUpload = false;

                    }
                    api.LogOut();
                }
                catch (Exception e)
                {
                    WriteToEventLogError("Exception thrown uploading people: " + e.ToString());
                }
                db.SubmitChanges();
            }
        }


        void ImportAssociates(string path, bool requireApproval)
        {
            int numFiles = 0;

            string ArcFolder = Path.Combine(path, "archive");

            if (!(Directory.Exists(ArcFolder)))
            {
                WriteToEventLog(string.Format("Created archive folder at: {0}", ArcFolder));
                Directory.CreateDirectory(ArcFolder);
            }

            string ErrFolder = Path.Combine(path, "error");

            if (!(Directory.Exists(ErrFolder)))
            {
                WriteToEventLog(string.Format("Created error folder at: {0}", ErrFolder));
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
                        WriteToEventLog(string.Format("Imported {0}.", filename));
                        numFiles++;
                        newFile = GetUniqueName(Path.GetFileName(filename), ArcFolder);
                    }
                    else
                    {
                        WriteToEventLogError(string.Format("Failed to import {0}.", filename));
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

        void ImportS2Levels()
        {
            S2API api = new S2API(_s2URI, _s2RSMAccount, _s2RSMPassword);
            S2Importer imp = new S2Importer(api);
            int count = 0;
            try
            {
                count = imp.ImportLevels();
            }
            catch (Exception e)
            {
                WriteToEventLogError("Error importing levels from S2: " + e.ToString());
            }

            if(count > 0)
                WriteToEventLog(string.Format("Imported {0} levels from S2 hardware at {1}.", count, _s2URI));
        }
    }
}
