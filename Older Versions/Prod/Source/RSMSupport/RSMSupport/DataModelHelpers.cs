using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;


namespace RSM.Support
{
    


    public partial class RSMDataModelDataContext
    {
        public enum LogSources
        {
            S2IMPORT,
            S2EXPORT,
            PSIMPORT,
            USER
        }
        public enum LogSeverity
        {
            INFO,
            WARN,
            ERROR,
            DEBUG
        }

        public int CountLogEntriesWithStatus(LogSources source, LogSeverity sev, DateTime since)
        {
            return  (from l in LogEntries
                     where ((l.Source == (int)source) &&
                           (l.EventDate >= since) &&
                           (l.Severity == (int)sev))
                     select l).Count();
        }


        public void Syslog(LogSources src, LogSeverity sev, string message, string details)
        {
            LogEntry ent = new LogEntry
                               {
                                   EventDate = DateTime.Now,
                                   Source = (int) src,
                                   Severity = (int) sev,
                                   Message = message,
                                   Details = details
                               };

            try
            {
                LogEntries.InsertOnSubmit(ent);
                SubmitChanges();
            }
            catch (Exception)
            {
                // Don't choke if we can't log.
            }
        }


    }

    public partial class Person
    {
        protected Job _job;
        public Job Job
        {
            get
            {
                if(_job == null)
                {
                    var ctx = new RSMDataModelDataContext();
                    _job = ctx.Jobs.Where( c => c.JobCode == JobCode).First();
                }
                
                return _job;
            }
        }

        public string DisplayName
        {
            get {
                return NickFirst == FirstName ? string.Format("{0}, {1} {2}", LastName, FirstName, MiddleName) : string.Format("{0}, {1} ({3}) {2}", LastName, FirstName, MiddleName, NickFirst);
            }
        }

        public string DisplayCredentials
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Credentials))
                    {
                        return Credentials.ToUpper() == "DONOTPRINT" ? " " : Credentials;
                    }
                    if ((Job.Credentials != null) && (Job.Credentials.Length > 0))
                        return Job.Credentials;
                }
                catch (Exception)
                { }
                return "";
            }
        }
    }

    public partial class LogEntry
    {
        public string SourceName
        {
            get
            {
                switch (Source)
                {
                    case (int)RSMDataModelDataContext.LogSources.S2IMPORT:
                        return "S2 Importer";
                    case (int)RSMDataModelDataContext.LogSources.S2EXPORT:
                        return "S2 Exporter";
                    case (int)RSMDataModelDataContext.LogSources.PSIMPORT:
                        return "PeopleSoft Importer";
                    case (int)RSMDataModelDataContext.LogSources.USER:
                        return "User Activity";

                }
                return "Unkown";
            }
        }

        public string SeverityName
        {
            get
            {
                switch (Severity)
                {
                    case (int)RSMDataModelDataContext.LogSeverity.DEBUG:
                        return "Debug Information";
                    case (int)RSMDataModelDataContext.LogSeverity.ERROR:
                        return "Error";
                    case (int)RSMDataModelDataContext.LogSeverity.WARN:
                        return "Warning";
                    case (int)RSMDataModelDataContext.LogSeverity.INFO:
                        return "Informational";

                }
                return "Unknown";
            }
        }

    }

    public partial class PeopleRole

    {
        public PeopleRole(Person p,  Role roleIn)
        {
            this.Role = roleIn;
            this.Person = p;
        }
    }


    public class RoleMetaData
    {
        [Required(ErrorMessage = "You must supply a name for this role.")]
        public string RoleName { get; set; }
    }

    [MetadataType(typeof(RoleMetaData))]
    public partial class Role
    {
        
        public string Description
        {
            get { return _RoleDesc; }
            set { _RoleDesc = value; }
        }
    }


    public partial class Job
    {
        public string DisplayName
        {
            get
            {
                return ((this.JobCode == "0") ? this.JobDescription : string.Format("{0} ({1})", this.JobDescription, this.JobCode));
            }
        }
        public string DisplayNameCodeFirst
        {
            get
            {
                return ((this.JobCode == "0") ? this.JobDescription : string.Format("{1} - {0}", this.JobDescription, this.JobCode));
            }
        }
    }

    public partial class Department
    {
        public string DisplayName
        {
            get
            {
                return ((this.DeptID == "0") ? this.DeptDescr : string.Format("{0} ({1})", this.DeptDescr, this.DeptID));
            }
        }


    }
}
