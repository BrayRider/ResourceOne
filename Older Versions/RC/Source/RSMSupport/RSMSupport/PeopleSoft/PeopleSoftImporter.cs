using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RSM.Support.IO.Csv;
using RSM.Support.SRMC;
using System.IO;


namespace RSM.Support.SRMC
{
    public class SRMCImporter
    {
        RSMDataModelDataContext db;

        public enum fileTypes
        {
            FT_PEOPLESOFT,
            FT_PHYSICIANS,
            FT_VOLUNTEERS
        }

        public SRMCImporter(string connectionStr)
        {
            db = new RSMDataModelDataContext(connectionStr);
        }

        public SRMCImporter()
        {
            db = new RSMDataModelDataContext();
        }


       public int ImportNewJobs()
        {
            int newJobCount = 0;
            List<NewJob> newJobs = db.NewJobs.ToList();
            foreach (NewJob newJob in newJobs)
            {
                Job jobToAdd = new Job();
                jobToAdd.DateAdded = DateTime.Today;
                jobToAdd.JobCode = newJob.JobCode;
                jobToAdd.JobDescription = newJob.JobDescr;
                jobToAdd.DisplayDescription = newJob.JobDescr;
                jobToAdd.Credentials = "";
                newJobCount++;
                db.Jobs.InsertOnSubmit(jobToAdd);
                db.SubmitChanges();
            }

            if (newJobCount > 0)
            {
                
                db.Syslog(RSMDataModelDataContext.LogSources.PSIMPORT,
                          RSMDataModelDataContext.LogSeverity.WARN,
                          string.Format("Associate import discovered {0} new jobs.", newJobCount), "");

                
            }

            return newJobCount;
        }


       public int ImportNewDepts()
       {
           int newDepCount = 0;
           List<NewDepartment> newDepts = db.NewDepartments.ToList();
           foreach (NewDepartment newDep in newDepts)
           {
               Department ToAdd = new Department
                                      {
                                          DateAdded = DateTime.Today,
                                          DeptID = newDep.DeptID,
                                          DeptDescr = newDep.DeptDescr
                                      };
               newDepCount++;
               db.Departments.InsertOnSubmit(ToAdd);
           }

           if (newDepCount > 0)
           {
               db.Syslog(RSMDataModelDataContext.LogSources.PSIMPORT,
                         RSMDataModelDataContext.LogSeverity.WARN,
                         string.Format("Associate import discovered {0} new departments.", newDepCount), "");
               db.SubmitChanges();
           }

           return newDepCount;
       }

       public int ImportNewLocations()
       {
           int newLocCount = 0;
           List<NewLocation> newLocs = db.NewLocations.ToList();
           foreach (NewLocation  newLoc in newLocs )
           {
               Location ToAdd = new Location {DateAdded = DateTime.Today, LocationName = newLoc.Facility.ToString()};
               newLocCount++;
               db.Locations.InsertOnSubmit(ToAdd);
           }

           if (newLocCount > 0)
           {
               db.Syslog(RSMDataModelDataContext.LogSources.PSIMPORT,
                         RSMDataModelDataContext.LogSeverity.WARN,
                         string.Format("Associate import discovered {0} new facilities.", newLocCount), "");
               db.SubmitChanges();
           }

           return newLocCount;
       }

        public bool ImportCSV(string path, bool reqApproval)
        {
            CsvReader rdr = new CsvReader(new StreamReader(path), false);
            int newCount = 0;
            int changeCount = 0;
            bool firstRecordSeen = false;
            bool skip = false; 
            SRMCImporter.fileTypes fileType = fileTypes.FT_PEOPLESOFT;


            using (rdr)
            {
                while (rdr.ReadNextRecord())
                {
                    if (!firstRecordSeen)
                    {
                        // check the first line of the file to determine what kind of file it is
                        firstRecordSeen = true;
                        if (rdr.FieldCount == 12)
                        {
                            if (rdr[(int)UserRecord.PhyCSVColumns.Role1].StartsWith("All Medical Staff"))
                            {
                                fileType = fileTypes.FT_PHYSICIANS;
                                skip = true;
                                while (rdr[0] != "Last Name")
                                {
                                    rdr.ReadNextRecord();
                                }
                            }
                            else
                            {
                                fileType = fileTypes.FT_PEOPLESOFT;
                            }
                        }
                        else
                        {
                            fileType = fileTypes.FT_PEOPLESOFT;
                        }
                        //switch (rdr[0])
                        //{
                        //    case "Last name, First name":
                        //        fileType = fileTypes.FT_VOLUNTEERS;
                        //        skip = true;
                        //        break;
                        //    case "lastname_of_providers":
                              
                        //        break;
                        //    default:
                        //        fileType = fileTypes.FT_PEOPLESOFT;
                        //        break;
                        //}

                    }
                    if (!skip)
                    {

                        UserRecord user = new UserRecord(rdr, fileType);
                        int changeMask = 0;

                        // First let's try to find the user

                        Person person;
                        try
                        {
                            person = (from p in db.Persons
                                      where p.EmployeeID == user.EmployeeID
                                      select p).First();
                        }
                        catch (Exception)
                        {

                            person = null;
                        }

                        if (person != null)
                        {

                            // Now let's see if they've changed any of the key fields.
                            if (person.DeptID != user.DeptID)
                                changeMask = (changeMask | (int)UserRecord.KeyColumnMask.DeptID);

                            if (person.JobCode != user.JobCode)
                                changeMask = (changeMask | (int)UserRecord.KeyColumnMask.JobCode);

                            if (person.Facility != user.Facility)
                                changeMask = (changeMask | (int)UserRecord.KeyColumnMask.Facility);

                            if (person.Active != user.Active)
                                changeMask = (changeMask | (int)UserRecord.KeyColumnMask.Status);

                            if (changeMask != 0)
                                changeCount++;

                        }
                        else
                        {
                            person = new Person();
                            db.Persons.InsertOnSubmit(person);
                            changeMask = (int)UserRecord.KeyColumnMask.Status;
                            newCount++;
                        }

                        if (changeMask != 0)
                        {
                            person.LastUpdated = DateTime.Today;
                            person.LastUpdateMask = changeMask;
                            person.NeedsRulePass = true;
                            person.NeedsApproval = reqApproval;
                        }

                        person.EmployeeID = user.EmployeeID;
                        person.FirstName = user.FirstName;
                        person.LastName = user.LastName;
                        person.MiddleName = user.MiddleName;
                        person.NickFirst = user.FirstName;
                        person.DeptID = user.DeptID;
                        person.DeptDescr = user.DeptDescription;
                        person.JobCode = user.JobCode;
                        person.JobDescr = user.JobDescription;
                        person.BadgeNumber = user.BadgeNumber;
                        person.Facility = user.Facility;
                        person.Active = user.Active;
                        person.Credentials = (user.HasCredentials && (user.Credentials.Length > 0)) ? user.Credentials : person.Credentials;

                        try
                        {
                            db.SubmitChanges();
                        }
                        catch (Exception e)
                        {
                            db.Syslog(RSMDataModelDataContext.LogSources.PSIMPORT,
                                      RSMDataModelDataContext.LogSeverity.ERROR,
                                      string.Format("Associate import failed on file {0}.", path), e.ToString());

                            return false;
                        }
                    }
                    else
                    {
                        skip = false; // Only skip the first row.
                    }

                }
               
            
            }
            db.Syslog(RSMDataModelDataContext.LogSources.PSIMPORT,
                      RSMDataModelDataContext.LogSeverity.INFO ,
                      string.Format("Imported {1} new, {2} changed associates from {0}.", path, newCount, changeCount), "");
            return true;
        }
    }
}
