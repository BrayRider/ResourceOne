using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSM.Service.Library.Model;
using RSM.Service.Library.Interfaces;
using RSM.Integration.Track;
using AccessEvents = RSM.Integration.Track.Export.AccessEvents;

namespace RSM.Service.Library.Tests.Export
{
	public class TrackExportAPIStub : API
	{
		public TrackExportAPIStub()
			: base()
		{
			Name = "Track export STUB API";
			Version = "0.1";
		}

		public new Result<UserSession> Login(string username, string password, string uri)
		{
			var result = Result<UserSession>.Success();

			result.Entity = new UserSession
			{
				Id = "1234",
				Username = username
			};

			return result;
		}

		public new void Logoff(UserSession session)
		{ }


		public new bool PushEvent(AccessLog log)
		{
			return true;
		}

		public new Result<AccessLog> ExportEvent(AccessLog log)
		{
			var result = Result<AccessLog>.Success();

			return result;
		}

		public new Result<Person> ExportPerson(Person person, AccessLog log)
		{
			var result = Result<Person>.Success();

			return result;
		}

		public new Result<Reader> ExportReader(Reader reader, AccessLog log)
		{
			var result = Result<Reader>.Success();

			return result;
		}

		public new Result<Portal> ExportPortal(Portal portal, AccessLog log)
		{
			var result = Result<Portal>.Success();

			return result;
		}

		public new Result<Location> ExportLocation(Location location, AccessLog log)
		{
			var result = Result<Location>.Success();

			return result;
		}
	}
}
