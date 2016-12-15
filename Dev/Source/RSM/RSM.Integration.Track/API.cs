using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using RSM.Service.Library;
using RSM.Service.Library.Model;
using RSM.Service.Library.Interfaces;

namespace RSM.Integration.Track
{
	public class API : IAPI, IAuthentication, IExportAccessEvent
	{
		public string Name { get; protected set; }
		public string Version { get; protected set; }

		public UserSession UserSession { get; private set; }

		public Export.AccessEvents.Config Config { get; set; }

		private Proxy.ACS2TrackWebSvc Proxy { get; set; }
		private Mapper Mapper { get; set; }

		public API()
		{
			Name = "Track Export API";
			Version = "1.0";
			Proxy = new Proxy.ACS2TrackWebSvc();
		}

		/// <summary>
		/// Must be invoked prior to any other method in this API.
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="uri"></param>
		/// <returns></returns>
		public Result<UserSession> Login(string username, string password, string uri = null)
		{
			var result = Result<UserSession>.Success();

			Proxy.Credentials = new System.Net.NetworkCredential(username, password);
			Proxy.Url = uri;

			Mapper = new Mapper(Config.Task);

			result.Entity = new UserSession { Id = "n/a", Username = username };

			return result;
		}

		public void Logoff(UserSession session)
		{
			Proxy.Dispose();
		}

		public bool PushEvent(AccessLog log)
		{
			return true;  //accept all
		}

		public Result<AccessLog> ExportEvent(AccessLog log)
		{
			var result = Result<AccessLog>.Success();
			var error = string.Empty;

		    if (log.Person == null)
		    {
                EventLog.WriteEntry("Application", string.Format("Export Person Missing!, Date: [{0}]", log.Accessed));
                result.Entity = log;
                result.Fail(string.Format("Export Person Missing!, Date: [{0}]", log.Accessed));
                return result;
            }

            if (log.Reader == null)
            {
                EventLog.WriteEntry("Application", string.Format("Export Reader Missing!, Date: [{0}]", log.Accessed));
                result.Entity = log;
                result.Fail(string.Format("Export Reader Missing!, Date: [{0}]", log.Accessed));
                return result;
            }

            if (log.Portal == null)
            {
                EventLog.WriteEntry("Application", string.Format("Export Portal Missing!, Date: [{0}]", log.Accessed));
                result.Entity = log;
                result.Fail(string.Format("Export Portal Missing!, Date: [{0}]", log.Accessed));
                return result;
            }

            if (Proxy.AddEventByExtEmpRefByLocation(Config.DataSource,
				Mapper.EventDate(log.Accessed),
				Mapper.ExtEmpRef(log.Person.ExternalId),
				Config.SysId,
				Mapper.EventLocId(log.Portal.ExternalId),
				Mapper.DeviceId(log.Reader.ExternalId, log.Reader.InternalId),
				Config.EventCode,
				ref error,
				Mapper.LocationID(log.Portal.Location),
				Mapper.CardID(log.Person.BadgeNumber)))
			{
				result.Entity = Factory.CreateAccessLog(log.ExternalId, ExternalSystem.TrackOut);
                //EventLog.WriteEntry("Application", string.Format("Success Returned: Date: [{0}], Emp ID: [{1}], Card ID: [{2}]", log.Accessed, log.Person.ExternalId, log.Person.BadgeNumber));
            }
			else
			{
                EventLog.WriteEntry("Application", string.Format("Error Returned: Error: [{3}], Date: [{0}], Emp ID: [{1}], Card ID: [{2}]", log.Accessed, log.Person.ExternalId, log.Person.BadgeNumber, error));
				result.Fail(error);
			}

			result.Entity = log;

			return result;
		}

		public Result<Person> ExportPerson(Person person, AccessLog log)
		{
			var result = Result<Person>.Success();
			var error = string.Empty;

			if (Proxy.AddPerson(Config.DataSource,
				Mapper.LastName(person.LastName),
				Mapper.FirstName(person.FirstName),
				Mapper.MiddleName(person.MiddleName),
				Mapper.ExtEmpRef(person.ExternalId),
				Mapper.ExtCmpRef(person.udf4),
				Mapper.LocationID(log.Portal.Location),
				string.Empty, //SSN
				Mapper.CardID(person.BadgeNumber),
				string.Empty, //sPayGroupName
				ref error))
			{
				// No Track id available so use the one from the source system
				result.Entity = Factory.CreatePerson(person.ExternalId, ExternalSystem.TrackOut);
			}
			else
				result.Fail(error);

			result.Entity = person;

			return result;
		}

		public Result<Reader> ExportReader(Reader reader, AccessLog log)
		{
			var result = Result<Reader>.Success();
			var error = string.Empty;

			if (Proxy.AddReaderDevice(Config.DataSource,
				Mapper.ReaderName(reader.Name),
				Config.SysId,
				Mapper.EventLocId(log.Portal.ExternalId),
				Mapper.DeviceId(reader.ExternalId, reader.InternalId),
				reader.Direction,
				ref error))
			{
				result.Entity = Factory.CreateReader(reader.ExternalId, ExternalSystem.TrackOut);
			}
			else
				result.Fail(error);

			result.Entity = reader;

			return result;
		}

		public Result<Portal> ExportPortal(Portal portal, AccessLog log)
		{
			var result = Result<Portal>.Success();
			var error = string.Empty;

			if (Proxy.AddOrUpdateDeviceControlUnits(Config.DataSource,
				Mapper.PortalId(portal.ExternalId, portal.InternalId),
				Mapper.PortalName(portal.Name),
				portal.ReaderCount,
				Mapper.PortalNetworkAddress(portal.NetworkAddress),
				Mapper.PortalType(portal.DeviceType),
				Mapper.PortalCapabilities(portal.Capabilities),
				Config.SysId,
				ref error))
			{
				result.Entity = Factory.CreatePortal(portal.ExternalId, ExternalSystem.TrackOut);
			}
			else
				result.Fail(error);

			result.Entity = portal;

			return result;
		}

		public Result<Location> ExportLocation(Location location, AccessLog log)
		{
			return Result<Location>.Success();
		}

		public Result<string> ExportCompany(AccessLog log)
		{
			var result = Result<string>.Success();
			var error = string.Empty;

			var companyName = log.Person.udf4;
			result.Entity = companyName;

			if (string.IsNullOrWhiteSpace(companyName))
				return result.Fail("person ({0}) does not have a company value. UDF 4 field is empty.", log.Person.InternalId.ToString());

			Proxy.AddCompany(Config.DataSource,
				Mapper.LocationName(companyName),
				Mapper.ExtCmpRef(companyName),
				Mapper.LocationID(log.Portal.Location),
				ref error);

			return result;
		}
	}
}
