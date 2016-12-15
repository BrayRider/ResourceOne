using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using RSM.Service.Library;
using AccessHistoryBase = RSM.Service.Library.Import.AccessHistory;
using RSM.Service.Library.Model;

namespace RSM.Integration.S2.Import
{
	public class AccessHistory : AccessHistoryBase
	{
		public AccessHistory()
			: base()
		{
			base.ExternalSystem = RSM.Service.Library.Model.ExternalSystem.S2In;
		}

		public override bool Filter(AccessLog log)
		{
			if (string.IsNullOrWhiteSpace(log.Person.ExternalId) || 
				string.IsNullOrWhiteSpace(log.Portal.ExternalId) || 
				string.IsNullOrWhiteSpace(log.Reader.ExternalId))
			{
				LogError("cannot process Access record {0}. It has an invalid ID. Person ({1}), Portal ({2}), Reader({3})", log.ExternalId, log.Person.ExternalId, log.Portal.ExternalId, log.Reader.ExternalId);
				return false;
			}

			if (log.AccessType != (int)AccessType.Valid && log.AccessType != (int)AccessType.ElevatorValid)
				return false;

            //EventLog.WriteEntry("R1SM.S2Import.Debug", string.Format("Filter Access: {0}", log.InternalId));
            //EventLog.WriteEntry("R1SM.S2Import.Debug", string.Format("Filter Person is null: {0}", log.Person == null));
            //EventLog.WriteEntry("R1SM.S2Import.Debug", string.Format("Filter Person Internal ID: {0}", log.Person.InternalId));
            //EventLog.WriteEntry("R1SM.S2Import.Debug", string.Format("Filter UDF4: [{0}] in List [{1}]", log.Person.udf4, string.Join(",", Configuration.ImportCompanies.ToArray())));

		    if (log.Person == null || log.Person.InternalId == 0) return true;

            //Only contractor activity matters. Any value in UDF4 means it is contractor
            if (string.IsNullOrWhiteSpace(log.Person.udf4)) return false;

            var allOrFoundInList = Configuration == null || Configuration.ImportCompanies == null || Configuration.ImportCompanies.Length <= 0 ||
                   Configuration.ImportCompanies.Any(company => String.Equals(company, log.Person.udf4, StringComparison.CurrentCultureIgnoreCase));

		    return allOrFoundInList;
		}

        public override bool FilterPerson(Person person)
        {
            if (person == null)
                return true;

            if (string.IsNullOrWhiteSpace(person.ExternalId))
            {
                LogError("cannot process person record {0}. It has an invalid ID. Person ({1})", person.ExternalId);
                return false;
            }

            //EventLog.WriteEntry("R1SM.S2Import.Debug", string.Format("Filter UDF4: [{0}] in List [{1}]", person.udf4, string.Join(",", Configuration.ImportCompanies.ToArray())));

            //Only contractor activity matters. Any value in UDF4 means it is contractor
            if (string.IsNullOrWhiteSpace(person.udf4)) return false;

            var allOrFoundInList = Configuration == null || Configuration.ImportCompanies == null || Configuration.ImportCompanies.Length <= 0 ||
                   Configuration.ImportCompanies.Any(company => String.Equals(company, person.udf4, StringComparison.CurrentCultureIgnoreCase));

            return allOrFoundInList;
        }
    }
}
