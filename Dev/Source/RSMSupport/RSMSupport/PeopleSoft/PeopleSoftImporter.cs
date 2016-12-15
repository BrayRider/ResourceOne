using System;
using System.IO;
using System.Linq;
using RSM.Support.IO.Csv;

namespace RSM.Support.SRMC
{
	public class SRMCImporter
	{
		RSMDataModelDataContext db;

		public ExternalSystem OwningSystem { get; set; }

		public SRMCImporter(string connectionStr)
		{
			db = new RSMDataModelDataContext(connectionStr);

			OwningSystem = db.ExternalSystems.FirstOrDefault(x => x.Name == "PeopleSoft");
		}

		public SRMCImporter()
		{
			db = new RSMDataModelDataContext();

			OwningSystem = db.ExternalSystems.FirstOrDefault(x => x.Name == "PeopleSoft");
		}

	   public int ImportNewJobs()
		{
			var newJobCount = 0;
			var newJobs = db.NewJobs.ToList();
			foreach (var jobToAdd in newJobs.Select(newJob => new Job
			                                                  	{
			                                                  		DateAdded = DateTime.Today,
			                                                  		JobCode = newJob.JobCode,
			                                                  		JobDescription = newJob.JobDescr,
			                                                  		DisplayDescription = newJob.JobDescr,
			                                                  		Credentials = ""
			                                                  	}))
			{
				newJobCount++;
				db.Jobs.InsertOnSubmit(jobToAdd);
				db.SubmitChanges();
			}

			if (newJobCount > 0)
			{
				db.Syslog(OwningSystem,
						  Artifacts.Log.Severity.Warning,
						  string.Format("Associate import discovered {0} new jobs.", newJobCount), "");
			}

			return newJobCount;
		}

	   public int ImportNewDepts()
	   {
		   var newDepCount = 0;
		   var newDepts = db.NewDepartments.ToList();
		   foreach (var toAdd in newDepts.Select(newDep => new Department
		                                                   	{
		                                                   		DateAdded = DateTime.Today,
		                                                   		DeptID = newDep.DeptID,
		                                                   		DeptDescr = newDep.DeptDescr
		                                                   	}))
		   {
			   newDepCount++;
			   db.Departments.InsertOnSubmit(toAdd);
		   }

		   if (newDepCount > 0)
		   {
			   db.Syslog(OwningSystem,
						 Artifacts.Log.Severity.Warning,
						 string.Format("Associate import discovered {0} new departments.", newDepCount), "");
			   
			   db.SubmitChanges();
		   }

		   return newDepCount;
	   }

	   public int ImportNewLocations()
	   {
		   var newLocCount = 0;
		   var newLocs = db.NewLocations.ToList();
		   foreach (var  newLoc in newLocs )
		   {
			   var toAdd = new Location {DateAdded = DateTime.Today, LocationName = newLoc.Facility};

			   newLocCount++;
			   db.Locations.InsertOnSubmit(toAdd);
		   }

		   if (newLocCount > 0)
		   {
			   db.Syslog(OwningSystem,
						 Artifacts.Log.Severity.Warning,
						 string.Format("Associate import discovered {0} new facilities.", newLocCount), "");

			   db.SubmitChanges();
		   }

		   return newLocCount;
	   }

		public bool ImportCSV(string path, bool reqApproval)
		{
			var rdr = new CsvReader(new StreamReader(path), false);
			var newCount = 0;
			var changeCount = 0;
			var firstRecordSeen = false;
			var skip = false; 
			var fileType = FileTypes.PeopleSoft;

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
								fileType = FileTypes.Physicians;
								skip = true;
								while (rdr[0] != "Last Name")
								{
									rdr.ReadNextRecord();
								}
							}
							else
							{
								fileType = FileTypes.PeopleSoft;
							}
						}
						else
						{
							fileType = FileTypes.PeopleSoft;
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

						var user = new UserRecord(rdr, fileType);
						var changeMask = 0;

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
							db.Syslog(OwningSystem,
									  Artifacts.Log.Severity.Error,
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

			db.Syslog(OwningSystem,
					  Artifacts.Log.Severity.Informational,
					  string.Format("Imported {1} new, {2} changed associates from {0}.", path, newCount, changeCount), "");
			return true;
		}
	}
}
