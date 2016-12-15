using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using RSM.Artifacts;
using RSM.Artifacts.Interfaces;
using RSM.Support;

using EntityType = RSM.Service.Library.Model.EntityType;

namespace RSM.Staging.Library
{
	public enum EntityAction
	{
		Create,
		InsertOnSubmit,
		InsertAndSubmit
	}

	public class Factory
	{
		#region Properties
		public RSMDataModelDataContext Context { get; set; }
		#endregion

		public Factory()
		{
			Context = new RSMDataModelDataContext { DeferredLoadingEnabled = false };
		}

		public Factory(RSMDataModelDataContext context)
		{
			Context = context;
		}

		public void DbAction<T>(T entity, EntityAction action)
			where T : class
		{
			if (action == EntityAction.InsertOnSubmit || action == EntityAction.InsertAndSubmit)
			{
				var table = Context.GetTable<T>();
				table.InsertOnSubmit(entity);
			}

			if (action == EntityAction.InsertAndSubmit)
				Context.SubmitChanges();
		}

		#region External Systems
		public static ExternalSystem CreateExternalSystem(
			int id,
			string name,
			ExternalSystemDirection direction
			)
		{
			var entity = new ExternalSystem
							 {
								 Id = id,
								 Name = name,
								 Direction = (int)direction
							 };

			return entity;
		}
		public ExternalSystem createExternalSystem(
			int id,
			string name,
			ExternalSystemDirection direction,
			EntityAction action = EntityAction.InsertOnSubmit
			)
		{
			var entity = Factory.CreateExternalSystem(id, name, direction);

			DbAction(entity, action);

			return entity;
		}
		#endregion

		#region Setting
		public static Setting CreateSetting(
			int id,
			string name,
			string label,
			string value,
			int orderBy = 0,
			bool viewable = true,
			string inputType = InputTypes.Text,
			ExternalSystem system = null
			)
		{
			var entity = new Setting
							 {
								 Id = id,
								 Name = name,
								 Label = label,
								 Value = value,
								 OrderBy = orderBy,
								 Viewable = viewable,
								 InputType = inputType,
								 ExternalSystem = system,
								 SystemId = (system == null ? default(int) : system.Id)
							 };

			return entity;
		}

		public Setting createSetting(
			int id,
			string name,
			string label,
			string value,
			int orderBy = 0,
			bool viewable = true,
			string inputType = InputTypes.Text,
			ExternalSystem system = null,
			EntityAction action = EntityAction.InsertOnSubmit
			)
		{
			var entity = Factory.CreateSetting(id, name, label, value, orderBy, viewable, inputType, system);

			DbAction(entity, action);

			return entity;
		}
		#endregion

		#region ApplicationKeys
		public static ExternalApplicationKey CreateExternalApplicationKey(
			EntityType entityType,
			string externalId,
			int externalSystemId,
			int internalId
			)
		{
			return new ExternalApplicationKey
			{
				EntityType = Enum.GetName(typeof(EntityType), entityType),
				ExternalId = externalId,
				SystemId = externalSystemId,
				InternalId = internalId,
				Added = DateTime.Now
			};
		}
		public ExternalApplicationKey createExternalApplicationKey(
			EntityType entityType,
			string externalId,
			int externalSystemId,
			int internalId,
			EntityAction action = EntityAction.InsertOnSubmit
			)
		{
			var entity = Factory.CreateExternalApplicationKey(entityType, externalId, externalSystemId, internalId);

			DbAction(entity, action);

			return entity;
		}
		#endregion

		#region Person
		public static byte[] HexToByte(string hexString)
		{
			var returnBytes = new byte[hexString.Length / 2];

			for (var i = 0; i < returnBytes.Length; i++)
				returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);

			return returnBytes;
		}

		public static string EncryptPassword(string password, string validationKey)
		{
			if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(validationKey)) return null;

			var hash = new HMACSHA512 { Key = HexToByte(validationKey) };

			var hash1 = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(string.Format("{0}.rSmSa1t{1}", password, password.Length))));
			var hash2 = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(string.Format("{0}an0tH3r5alt!{1}", hash1, password.Length))));

			return Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(hash2)));
		}

		public static string EncryptPassword(string password)
		{
			if (string.IsNullOrWhiteSpace(password)) return null;

			// Get encryption and decryption key information from the configuration.
			var cfg = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
			var machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

			return EncryptPassword(password, machineKey.ValidationKey);
		}

		public static Person CreatePerson(
			string firstName,
			string lastName,
			string middleName,
			int? id = null,
			bool isAdmin = false,
			bool isLockedOut = false,
			string username = null,
			string password = null,
			bool encryptPassword = false,
			string credentials = null,
			string nickname = null,
			string badgeNumber = null,
			DateTime? added = null,
			Dictionary<int, string> UDFs = null
			)
		{
			var entity = new Person
			{
				FirstName = firstName,
				LastName = lastName,
				MiddleName = middleName,
				Added = added != null ? (DateTime)added : DateTime.Now,
				IsAdmin = isAdmin,
				LockedOut = isLockedOut,
				username = username,
				password = encryptPassword ? EncryptPassword(password) : password,
				Credentials = credentials,
				NickFirst = nickname,
				BadgeNumber = badgeNumber,
				LastUpdated = DateTime.Now,
				Active = true
			};
			entity.PersonID = id != null ? (int)id : entity.PersonID;

			if (UDFs != null)
			{
				entity.UDF1 = UDFs.ContainsKey(1) ? UDFs[1] : null;
				entity.UDF2 = UDFs.ContainsKey(2) ? UDFs[2] : null;
				entity.UDF3 = UDFs.ContainsKey(3) ? UDFs[3] : null;
				entity.UDF4 = UDFs.ContainsKey(4) ? UDFs[4] : null;
				entity.UDF5 = UDFs.ContainsKey(5) ? UDFs[5] : null;
				entity.UDF6 = UDFs.ContainsKey(6) ? UDFs[6] : null;
				entity.UDF7 = UDFs.ContainsKey(7) ? UDFs[7] : null;
				entity.UDF8 = UDFs.ContainsKey(8) ? UDFs[8] : null;
				entity.UDF9 = UDFs.ContainsKey(9) ? UDFs[9] : null;
				entity.UDF10 = UDFs.ContainsKey(10) ? UDFs[10] : null;
				entity.UDF11 = UDFs.ContainsKey(11) ? UDFs[11] : null;
				entity.UDF12 = UDFs.ContainsKey(12) ? UDFs[11] : null;
				entity.UDF13 = UDFs.ContainsKey(13) ? UDFs[13] : null;
				entity.UDF14 = UDFs.ContainsKey(14) ? UDFs[14] : null;
				entity.UDF15 = UDFs.ContainsKey(15) ? UDFs[15] : null;
				entity.UDF16 = UDFs.ContainsKey(16) ? UDFs[16] : null;
				entity.UDF17 = UDFs.ContainsKey(17) ? UDFs[17] : null;
				entity.UDF18 = UDFs.ContainsKey(18) ? UDFs[18] : null;
				entity.UDF19 = UDFs.ContainsKey(19) ? UDFs[19] : null;
				entity.UDF20 = UDFs.ContainsKey(20) ? UDFs[20] : null;
			}

			return entity;
		}
		public Person createPerson(
			string firstName,
			string lastName,
			string middleName,
			int? id = null,
			bool isAdmin = false,
			bool isLockedOut = false,
			string username = null,
			string password = null,
			bool encryptPassword = false,
			string credentials = null,
			string nickname = null,
			string badgeNumber = null,
			string externalId = null,
			ExternalSystem system = null,
			DateTime? added = null,
			Dictionary<int, string> UDFs = null,
			EntityAction action = EntityAction.InsertOnSubmit
			)
		{
			var entity = Factory.CreatePerson(firstName, lastName, middleName, id, isAdmin, isLockedOut, username, password, encryptPassword, credentials, nickname, badgeNumber, added, UDFs);

			DbAction(entity, action);

			if (!string.IsNullOrWhiteSpace(externalId) && system != null && action == EntityAction.InsertAndSubmit)
			{
				var keys = CreateExternalApplicationKey(EntityType.Person, externalId, system.Id, entity.Id);
				DbAction(keys, action);
			}

			return entity;
		}
		#endregion

		#region Location
		public static Location CreateLocation(
			string name,
			int? id = null,
			DateTime? added = null
			)
		{
			var entity = new Location
			{
				LocationName = name,
				DateAdded = added != null ? (DateTime)added : DateTime.Now,
			};
			entity.LocationID = id != null ? (int)id : entity.LocationID;

			return entity;
		}

		public Location createLocation(
			string name,
			int? id = null,
			DateTime? added = null,
			EntityAction action = EntityAction.InsertOnSubmit
			)
		{
			var entity = Factory.CreateLocation(name, id, added);

			DbAction(entity, action);

			return entity;
		}

		#endregion

		#region Portal
		public static Portal CreatePortal(
			string name,
			int locationId,
			int? id = null,
			DateTime? added = null)
		{
			var entity = new Portal
			{
				Name = name,
				LocationId = locationId,
				Added = added != null ? (DateTime)added : DateTime.Now,
			};
			entity.Id = id != null ? (int)id : entity.Id;

			return entity;
		}
		public Portal createPortal(
			string name,
			int locationId,
			int? id = null,
			DateTime? added = null,
			EntityAction action = EntityAction.InsertOnSubmit
			)
		{
			var entity = Factory.CreatePortal(name, locationId, id, added);

			DbAction(entity, action);

			return entity;
		}

		#endregion

		#region Reader
		public static Reader CreateReader(
			string name,
			int portalId,
			int? id = null,
			DateTime? added = null,
			ReaderDirection direction = ReaderDirection.Neutral
			)
		{
			var entity = new Reader
			{
				Name = name,
				PortalId = portalId,
				Direction = (int)direction,
				Added = added != null ? (DateTime)added : DateTime.Now,
			};
			entity.Id = id != null ? (int)id : entity.Id;

			return entity;
		}

		public Reader createReader(
			string name,
			int portalId,
			int? id = null,
			DateTime? added = null,
			ReaderDirection direction = ReaderDirection.Neutral,
			EntityAction action = EntityAction.InsertOnSubmit
			)
		{
			var entity = Factory.CreateReader(name, portalId, id, added, direction);

			DbAction(entity, action);

			return entity;
		}

		#endregion

		#region AccessHistory
		public static AccessHistory CreateAccessHistory(
			string name,
			int personId,
			int portalId,
			int readerId,
			int accessType,
			int? reason = null,
			int? id = null,
			DateTime? accessed = null
			)
		{
			var entity = new AccessHistory
			{
				PersonId = personId,
				PortalId = portalId,
				ReaderId = readerId,
				Reason = reason,
				Type = accessType,
				Accessed = accessed != null ? (DateTime)accessed : DateTime.Now.Subtract(TimeSpan.FromMinutes(30)),
			};
			entity.Id = id != null ? (int)id : entity.Id;

			return entity;
		}
		public AccessHistory createAccessHistory(
			string name,
			int personId,
			int portalId,
			int readerId,
			int accessType,
			int? reason = null,
			int? id = null,
			DateTime? accessed = null,
			EntityAction action = EntityAction.InsertOnSubmit
			)
		{
			var entity = Factory.CreateAccessHistory(name, personId, portalId, readerId, accessType, reason, id, accessed);

			DbAction(entity, action);

			return entity;
		}
		#endregion

		#region Batch History
		public static BatchHistory CreateBatchHistory(
			int id,
			int systemId,
			DateTime runStart,
			DateTime runEnd,
			string filename = null,
			string message = "Success",
			BatchOutcome outcome = BatchOutcome.Success
			)
		{
			var entity = new BatchHistory
			{
				Id = id,
				SystemId = systemId,
				RunStart = runStart,
				RunEnd = runEnd,
				Filename = filename,
				Message = message,
				Outcome = (int)outcome
			};

			return entity;
		}

		public BatchHistory createBatchHistory(
			int id,
			int systemId,
			DateTime runStart,
			DateTime runEnd,
			string filename = null,
			string message = "Success",
			BatchOutcome outcome = BatchOutcome.Success,
			EntityAction action = EntityAction.InsertOnSubmit
			)
		{
			var entity = Factory.CreateBatchHistory(id, systemId, runStart, runEnd, filename, message, outcome);

			DbAction(entity, action);

			return entity;
		}
		#endregion

		#region Generic Deletes
		public static void Delete<T>(RSMDataModelDataContext context, T entity) where T : class, IEntity
		{
			if (context == null) return;
			if (entity == null) return;

			var deleted = context.GetTable<T>().FirstOrDefault(x => x.Id.Equals(entity.Id));
			if (deleted == null) return;

			context.GetTable<T>().DeleteOnSubmit(deleted);
		}

		public static void Delete<T>(RSMDataModelDataContext context, List<T> entityCollection) where T : class, IEntity
		{
			if (context == null) return;
			if (entityCollection == null) return;
			if (!entityCollection.Any()) return;

			var dataSet = context.GetTable<T>();

			foreach (var entity in entityCollection)
			{
				var deleted = context.GetTable<T>().FirstOrDefault(x => x.Id.Equals(entity.Id));
				if (deleted == null) continue;

				dataSet.DeleteOnSubmit(deleted);
			}
		}

		public static ExternalApplicationKey DeleteKeys(RSMDataModelDataContext context, EntityType type, int sysId, string extId = null, int? Id = null)
		{
			if (context == null) return null;
			if (extId == null && Id == null) return null;

			var entityType = Enum.GetName(typeof(EntityType), type);
			var keys = context.ExternalApplicationKeys.FirstOrDefault(x => x.SystemId == sysId && x.EntityType == entityType
				&& ((extId != null && x.ExternalId == extId) || (Id != null && x.InternalId == Id)));

			if (keys == null) return null;

			context.ExternalApplicationKeys.DeleteOnSubmit(keys);
			return keys;
		}

		public static void DeleteWithKeys<T>(RSMDataModelDataContext context, EntityType type, string extId, int sysId) where T : class, IEntity
		{
			if (context == null) return;

			var keys = DeleteKeys(context, type, sysId, extId);
			if (keys == null) return;

			//TODO: clean this up once People, Location are refactored to conform to IEntity
			if (type == EntityType.Person)
			{
				var entity = context.Persons.FirstOrDefault(x => x.PersonID.Equals(keys.InternalId));
				if (entity != null)
					context.Persons.DeleteOnSubmit(entity);
			}
			else if (type == EntityType.Location)
			{
				var entity = context.Locations.FirstOrDefault(x => x.LocationID.Equals(keys.InternalId));
				if (entity != null)
					context.Locations.DeleteOnSubmit(entity);
			}
			else
			{
				var eTable = context.GetTable<T>();
				var entity = eTable.FirstOrDefault(x => x.Id.Equals(keys.InternalId));
				if (entity != null)
					eTable.DeleteOnSubmit(entity);
			}
		}

		#endregion

		#region Generic Inserts
		public static void Insert<T>(RSMDataModelDataContext context, T entity) where T : class, IEntity
		{
			if (context == null) return;
			if (entity == null) return;

			var check = context.GetTable<T>().FirstOrDefault(x => x.Id.Equals(entity.Id));
			if (check == null) return;

			context.GetTable<T>().InsertOnSubmit(check);
		}

		public static void Insert<T>(RSMDataModelDataContext context, List<T> entityCollection) where T : class, IEntity
		{
			if (context == null) return;
			if (entityCollection == null) return;
			if (!entityCollection.Any()) return;

			foreach (var entity in entityCollection)
			{
				Insert(context, entity);
			}
		}
		#endregion
	}
}
