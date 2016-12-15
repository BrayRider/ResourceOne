using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSM.Service.Library.Model;
using RSM.Service.Library.Interfaces;
using RSM.Integration.S2;

namespace RSM.Service.Library.Tests.Import
{
	public class S2ImportAPIStub : IAPI, IAuthentication, IAccessHistory
	{
		public string Name { get; private set; }
		public string Version { get; private set; }

		private List<Person> People;

		public S2ImportAPIStub() : base()
		{
			Name = "S2 Import STUB API";
			Version = "0.1";
			loadContent();
		}

		private void loadContent()
		{
			People = new List<Person>();

			People.Add(CreatePerson("Person1", ExternalSystem.S2In, "Contractor1", "Smith", UDFs: new Dictionary<int, string> { { 4, "S & B" } }));
			People.Add(CreatePerson("2", ExternalSystem.S2In, "Contractor In Filter", "Doe", UDFs: new Dictionary<int, string> { { 4, "Mustang" } }));
			People.Add(CreatePerson("4", ExternalSystem.S2In, "Employee1", "Jones"));
            People.Add(CreatePerson("5", ExternalSystem.S2In, "Contractor Out Filter", "Doe", UDFs: new Dictionary<int, string> { { 4, "Not taken" } }));

		}

		public Person CreatePerson(
			string extId,
			ExternalSystem system,
			string firstName,
			string lastName,
			string middleName = null,
			DateTime? added = null,
			Dictionary<int, string> UDFs = null
			)
		{
			var entity = Factory.CreatePerson(extId, system);
			entity.FirstName = firstName;
			entity.LastName = lastName;
			entity.MiddleName = middleName;
			entity.Added = added != null ? (DateTime)added : DateTime.Now;

			if (UDFs != null)
			{
				entity.udf1 = UDFs.ContainsKey(1) ? UDFs[1] : null;
				entity.udf2 = UDFs.ContainsKey(2) ? UDFs[2] : null;
				entity.udf3 = UDFs.ContainsKey(3) ? UDFs[3] : null;
				entity.udf4 = UDFs.ContainsKey(4) ? UDFs[4] : null;
				entity.udf5 = UDFs.ContainsKey(5) ? UDFs[5] : null;
				entity.udf6 = UDFs.ContainsKey(6) ? UDFs[6] : null;
				entity.udf7 = UDFs.ContainsKey(7) ? UDFs[7] : null;
				entity.udf8 = UDFs.ContainsKey(8) ? UDFs[8] : null;
				entity.udf9 = UDFs.ContainsKey(9) ? UDFs[9] : null;
				entity.udf10 = UDFs.ContainsKey(10) ? UDFs[10] : null;
				entity.udf11 = UDFs.ContainsKey(11) ? UDFs[11] : null;
				entity.udf12 = UDFs.ContainsKey(12) ? UDFs[11] : null;
				entity.udf13 = UDFs.ContainsKey(13) ? UDFs[13] : null;
				entity.udf14 = UDFs.ContainsKey(14) ? UDFs[14] : null;
				entity.udf15 = UDFs.ContainsKey(15) ? UDFs[15] : null;
				entity.udf16 = UDFs.ContainsKey(16) ? UDFs[16] : null;
				entity.udf17 = UDFs.ContainsKey(17) ? UDFs[17] : null;
				entity.udf18 = UDFs.ContainsKey(18) ? UDFs[18] : null;
				entity.udf19 = UDFs.ContainsKey(19) ? UDFs[19] : null;
				entity.udf20 = UDFs.ContainsKey(20) ? UDFs[20] : null;
			}

			return entity;
		}

		public Result<UserSession> Login(string username, string password, string uri)
		{
			var result = Result<UserSession>.Success();

			result.Entity = new UserSession
			{
				Id = "1234",
				Username = username
			};

			return result;
		}

		public void Logoff(UserSession session)
		{ }

		public Result<List<AccessLog>> GetAccessHistory(DateTime From, DateTime? To = null, string LastKey = null)
		{
			var result = Result<List<AccessLog>>.Success();

			var start = DateTime.Now;

		    result.Entity = new List<AccessLog>();

            // valid
		    var accessLog = Factory.CreateAccessLog("1", ExternalSystem.S2In, "Person1", "Portal1", "Reader1",
		        start.Subtract(TimeSpan.FromMinutes(5)), (int) AccessType.Valid, 1, 1, 1, 1);
            result.Entity.Add(accessLog);

            // valid entry, but Contractor Filtered Out access should be ignored but Person entity imported
            accessLog = Factory.CreateAccessLog("1.1", ExternalSystem.S2In, "5", "Portal1", "Reader1",
                start.Subtract(TimeSpan.FromMinutes(4)), (int)AccessType.Valid, 11, 1, 1, 1);
            result.Entity.Add(accessLog);

            // valid entry, and Contractor Filtered In 
            accessLog = Factory.CreateAccessLog("1.2", ExternalSystem.S2In, "2", "Portal1", "Reader1",
                start.Subtract(TimeSpan.FromMinutes(4)), (int)AccessType.Valid, 11, 1, 1, 1);
            result.Entity.Add(accessLog);

            // invalid access type
		    accessLog = Factory.CreateAccessLog("2", ExternalSystem.S2In, "2", "2", "2",
		        start.Subtract(TimeSpan.FromMinutes(6)), (int) AccessType.Valid, 2, 1, 1, 1);
            result.Entity.Add(accessLog);

			// empty Person ID
		    accessLog = Factory.CreateAccessLog("3", ExternalSystem.S2In, "", "3", "3",
		        start.Subtract(TimeSpan.FromMinutes(7)), (int) AccessType.Valid, 3, 1, 1, 1);
            result.Entity.Add(accessLog);

			// Employee access should be ignored but Person entity imported
		    accessLog = Factory.CreateAccessLog("4", ExternalSystem.S2In, "4", "4", "4",
		        start.Subtract(TimeSpan.FromMinutes(8)), (int) AccessType.Valid, 4, 1, 1, 1);
            result.Entity.Add(accessLog);

			// invalid portal
		    accessLog = Factory.CreateAccessLog("5", ExternalSystem.S2In, "2", "Reader1", "BadPortal",
		        start.Subtract(TimeSpan.FromMinutes(9)), (int) AccessType.Valid, 4, 1, 1, 1);
            result.Entity.Add(accessLog);

			// invalid Reader
		    accessLog = Factory.CreateAccessLog("6", ExternalSystem.S2In, "2", "BadReader", "Portal1",
		        start.Subtract(TimeSpan.FromMinutes(9)), (int) AccessType.Valid, 4, 1, 1, 1);
            result.Entity.Add(accessLog);

            // invalid Person
            accessLog = Factory.CreateAccessLog("7", ExternalSystem.S2In, "BadPerson", "Reader1", "Portal1",
		        start.Subtract(TimeSpan.FromMinutes(9)), (int) AccessType.Valid, 0, 1, 1, 1);
            result.Entity.Add(accessLog);
			
			return result;
		}

		public Result<Person> RetrievePerson(string id)
		{
			var result = Result<Person>.Success();

			result.Entity = People.FirstOrDefault(x => x.ExternalId == id);

			return result;
		}

		public Result<Portal> RetrievePortal(string id)
		{
			var result = Result<Portal>.Success();

			var portal = Factory.CreatePortal("Portal1", ExternalSystem.S2In);
			portal.Name = "Portal1";

			result.Entity = portal;

			return result;
		}

		public Result<Reader> RetrieveReader(string id)
		{
			var result = Result<Reader>.Success();

			var reader = Factory.CreateReader( "Reader1", ExternalSystem.S2In);
			reader.Name = "Reader 1";

			result.Entity = reader;

			return result;
		}
	}
}
