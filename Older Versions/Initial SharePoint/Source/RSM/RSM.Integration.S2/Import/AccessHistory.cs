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
            var load = new Config(this).Load();
            if (load.Failed)
                return true;

            var config = load.Entity as Config;

			if (string.IsNullOrWhiteSpace(log.Person.ExternalId) || 
				string.IsNullOrWhiteSpace(log.Portal.ExternalId) || 
				string.IsNullOrWhiteSpace(log.Reader.ExternalId))
			{
				LogError("cannot process Access record {0}. It has an invalid ID. Person ({1}), Portal ({2}), Reader({3})", log.ExternalId, log.Person.ExternalId, log.Portal.ExternalId, log.Reader.ExternalId);
				return false;
			}

			if (log.AccessType != (int)AccessType.Valid && log.AccessType != (int)AccessType.ElevatorValid)
				return false;

			//Only contractor activity matters. Any value in UDF4 means it is contractor
		    if (log.Person != null && log.Person.InternalId != 0 && string.IsNullOrWhiteSpace(log.Person.udf4))
		    {

                return false;		        
		    }

			return true;
		}
	}
}
