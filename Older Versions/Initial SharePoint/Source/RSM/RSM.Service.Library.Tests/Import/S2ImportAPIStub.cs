using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSM.Service.Library.Model;
using RSM.Service.Library.Interfaces;
using RSM.Integration.S2;
using System.IO;
using System.Reflection;
using RSM.Service.Library.Model.Reflection;

namespace RSM.Service.Library.Tests.Import
{
	public class S2ImportAPIStub : IAPI, IAuthentication, IImportAccessHistory, IImportPeople
	{
		public string Name { get; private set; }
		public string Version { get; private set; }

		private List<Person> People;
		private ModelMapper<Person> Mapper;

		public S2ImportAPIStub() : base()
		{
			Name = "S2 Import STUB API";
			Version = "0.1";
			Mapper = new ModelMapper<Person>(new string[]{});
			loadContent();
		}

		private void loadContent()
		{
			People = new List<Person>();

			var lastUpdated = DateTime.Now.Subtract(TimeSpan.FromMinutes(10));

			People.Add(CreatePerson("Person1", ExternalSystem.S2In, "Contractor1", "Smith", middleName: "J", externalUpdated: lastUpdated, UDFs: new Dictionary<int, string> { { 4, "Contractor Co1" } }));
			People.Add(CreatePerson("2", ExternalSystem.S2In, "Contractor2", "Doe", middleName: "J", externalUpdated: lastUpdated, UDFs: new Dictionary<int, string> { { 4, "Contractor Co2" } }));
			People.Add(CreatePerson("4", ExternalSystem.S2In, "Employee1", "Jones", middleName: "J", externalUpdated: lastUpdated));

		}

		public Person CreatePerson(
			string extId,
			ExternalSystem system,
			string firstName,
			string lastName,
			string middleName = null,
			DateTime? added = null,
			DateTime? externalUpdated = null,
			Dictionary<int, string> UDFs = null
			)
		{
			var entity = Factory.CreatePerson(extId, system);
			entity.FirstName = firstName;
			entity.LastName = lastName;
			entity.MiddleName = middleName;
			entity.Added = added != null ? (DateTime)added : entity.Added;
			entity.ExternalUpdated = externalUpdated != null ? (DateTime)externalUpdated : entity.ExternalUpdated;

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

			result.Entity = new List<AccessLog>()
			{
				new AccessLog { ExternalId = "1", ExternalSystem = ExternalSystem.S2In, ExternalSystemId = ExternalSystem.S2In.Id,
					Person = Factory.CreatePerson("Person1", ExternalSystem.S2In),
					Reader = Factory.CreateReader("Reader1", ExternalSystem.S2In),
					Portal = Factory.CreatePortal("Portal1", ExternalSystem.S2In),
					AccessType = (int)AccessType.Valid,
					Accessed = start.Subtract(TimeSpan.FromMinutes(5))},

					//invalid access type
				new AccessLog { ExternalId = "2", ExternalSystem = ExternalSystem.S2In, ExternalSystemId = ExternalSystem.S2In.Id,
					Person = Factory.CreatePerson("2", ExternalSystem.S2In),
					Reader = Factory.CreateReader("2", ExternalSystem.S2In),
					Portal = Factory.CreatePortal("2", ExternalSystem.S2In),
					AccessType = (int)AccessType.Invalid,
					Accessed = start.Subtract(TimeSpan.FromMinutes(6))},

					//empty Person ID
				new AccessLog { ExternalId = "3", ExternalSystem = ExternalSystem.S2In, ExternalSystemId = ExternalSystem.S2In.Id,
					Person = Factory.CreatePerson("", ExternalSystem.S2In),
					Reader = Factory.CreateReader("3", ExternalSystem.S2In),
					Portal = Factory.CreatePortal("3", ExternalSystem.S2In),
					AccessType = (int)AccessType.Valid,
					Accessed = start.Subtract(TimeSpan.FromMinutes(7))},

					//Employee access should be ignored but Person entity imported
				new AccessLog { ExternalId = "4", ExternalSystem = ExternalSystem.S2In, ExternalSystemId = ExternalSystem.S2In.Id,
					Person = Factory.CreatePerson("4", ExternalSystem.S2In),
					Reader = Factory.CreateReader("4", ExternalSystem.S2In),
					Portal = Factory.CreatePortal("4", ExternalSystem.S2In),
					AccessType = (int)AccessType.Valid,
					Accessed = start.Subtract(TimeSpan.FromMinutes(8))},

					//invalid portal
				new AccessLog { ExternalId = "5", ExternalSystem = ExternalSystem.S2In, ExternalSystemId = ExternalSystem.S2In.Id,
					Person = Factory.CreatePerson("2", ExternalSystem.S2In),
					Reader = Factory.CreateReader("Reader1", ExternalSystem.S2In),
					Portal = Factory.CreatePortal("BadPortal", ExternalSystem.S2In),
					AccessType = (int)AccessType.Valid,
					Accessed = start.Subtract(TimeSpan.FromMinutes(7))},

					//invalid Reader
				new AccessLog { ExternalId = "6", ExternalSystem = ExternalSystem.S2In, ExternalSystemId = ExternalSystem.S2In.Id,
					Person = Factory.CreatePerson("2", ExternalSystem.S2In),
					Reader = Factory.CreateReader("BadReader", ExternalSystem.S2In),
					Portal = Factory.CreatePortal("Portal1", ExternalSystem.S2In),
					AccessType = (int)AccessType.Valid,
					Accessed = start.Subtract(TimeSpan.FromMinutes(7))},

					//invalid Person
				new AccessLog { ExternalId = "7", ExternalSystem = ExternalSystem.S2In, ExternalSystemId = ExternalSystem.S2In.Id,
					Person = Factory.CreatePerson("BadPerson", ExternalSystem.S2In),
					Reader = Factory.CreateReader("Reader1", ExternalSystem.S2In),
					Portal = Factory.CreatePortal("Portal1", ExternalSystem.S2In),
					AccessType = (int)AccessType.Valid,
					Accessed = start.Subtract(TimeSpan.FromMinutes(7))},

			};

			return result;
		}

		public Result<Person> RetrievePerson(string id)
		{
			var result = Result<Person>.Success();

			var person = People.FirstOrDefault(x => x.ExternalId == id);

			if (person != null)
				result.Entity = Mapper.Clone(person);

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

		public Result<List<Person>> GetPeople(ref string nextKey)
		{
			var result = Result<List<Person>>.Success();
			result.Entity = new List<Person>();
			People.ForEach(x => result.Entity.Add(Mapper.Clone(x)));
			return result;
		}

		public Result<Person> RetrievePersonDetail(string id, bool includeImage = false)
		{
			var result = Result<Person>.Success();

			result.Entity = People.FirstOrDefault(x => x.ExternalId == id);

			if (result.Entity != null)
			{
				result.Entity = Mapper.Clone(result.Entity);
				if (includeImage)
				{
					var imageFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"content\missing.jpg");
					var image = File.ReadAllBytes(imageFile);
					result.Entity.Image = image;
				}
			}

			return result;
		}


	}
}
