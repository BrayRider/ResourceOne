using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Linq;
using System.Text;

using RSM.Service.Library.Model;
using RSMDB = RSM.Support;

namespace RSM.Service.Library.Extensions
{
	public static class DataExtensions
	{
		#region Mappers
		public static ExternalSystem ToModel(this RSMDB.ExternalSystem from, ExternalSystem existing = null)
		{
			var entity = existing == null ? new ExternalSystem() : existing;

			entity.Id = from.Id;
			entity.Name = from.Name;
			entity.Direction = (RSMDB.ExternalSystemDirection)from.Direction;

			return entity;
		}

		public static T ToModel<T>(this RSMDB.ExternalApplicationKey from, T existing = null)
			where T: ExternalEntity, new()
		{
			var entity = existing == null ? new T() : existing;

			entity.InternalId = from.InternalId;
			entity.ExternalId = from.ExternalId;
			entity.ExternalSystemId = from.SystemId;
			entity.EntityType = (EntityType)Enum.Parse(typeof(EntityType), from.EntityType);
			entity.KeysAdded = from.Added;
			entity.ExternalUpdated = from.ExternalEntityLastUpdated ?? default(DateTime);

			if (from.ExternalSystem != null)
				entity.ExternalSystem = from.ExternalSystem.ToModel(entity.ExternalSystem);

			return entity;
		}

		public static Person ToModel(this RSMDB.Person from, Person existing = null)
		{
			var entity = (existing == null) ? new Person() : existing;

			entity.InternalId = from.PersonID;
			entity.FirstName = from.FirstName;
			entity.LastName = from.LastName;
			entity.MiddleName = from.MiddleName;
			entity.Active = from.Active;
			entity.Added = from.Added;
			entity.Updated = from.LastUpdated;
			entity.BadgeNumber = from.BadgeNumber;
			entity.udf1 = from.UDF1;
			entity.udf2 = from.UDF2;
			entity.udf3 = from.UDF3;
			entity.udf4 = from.UDF4;
			entity.udf5 = from.UDF5;
			entity.udf6 = from.UDF6;
			entity.udf7 = from.UDF7;
			entity.udf8 = from.UDF8;
			entity.udf9 = from.UDF9;
			entity.udf10 = from.UDF10;
			entity.udf11 = from.UDF11;
			entity.udf12 = from.UDF12;
			entity.udf13 = from.UDF13;
			entity.udf14 = from.UDF14;
			entity.udf15 = from.UDF15;
			entity.udf16 = from.UDF16;
			entity.udf17 = from.UDF17;
			entity.udf18 = from.UDF18;
			entity.udf19 = from.UDF19;
			entity.udf20 = from.UDF20;
			entity.Image = (from.Image != null) ? from.Image.ToArray() : null;

			return entity;
		}

		public static Reader ToModel(this RSMDB.Reader from, Reader existing = null)
		{
			var entity = (existing == null) ? new Reader() : existing;

			entity.EntityType = EntityType.Reader;
			entity.InternalId = from.Id;
			entity.Added = from.Added;
			entity.Name = from.Name;
			entity.PortalId = from.PortalId;
			entity.Direction = from.Direction;

			if (from.Portal != null)
				entity.Portal = from.Portal.ToModel(entity.Portal);

			return entity;
		}

		public static Portal ToModel(this RSMDB.Portal from, Portal existing = null)
		{
			var entity = (existing == null) ? new Portal() : existing;

			entity.EntityType = EntityType.Portal;
			entity.InternalId = from.Id;
			entity.Name = from.Name;
			entity.LocationId = from.LocationId;
			entity.NetworkAddress = from.NetworkAddress;
			entity.Capabilities = from.Capabilities;
			entity.DeviceType = from.DeviceType;
			entity.ReaderCount = from.Readers.Count;

			if (from.Location != null)
				entity.Location = from.Location.ToModel(entity.Location);

			return entity;
		}

		public static Location ToModel(this RSMDB.Location from, Location existing = null)
		{
			var entity = (existing == null) ? new Location() : existing;

			entity.EntityType = EntityType.Location;
			entity.InternalId = from.LocationID;
			entity.Added = from.DateAdded;
			entity.Name = from.LocationName;

			return entity;
		}

		public static AccessLog ToModel(this RSMDB.AccessHistory from, AccessLog existing = null)
		{
			var entity = (existing == null) ? new AccessLog() : existing;

			entity.EntityType = EntityType.AccessLog;
			entity.InternalId = from.Id;
			entity.AccessType = from.Type;
			entity.Reason = from.Reason != null ? (int)from.Reason : entity.Reason;
			entity.Accessed = from.Accessed;

			entity.PersonId = from.PersonId;
			if (from.Person != null)
				entity.Person = from.Person.ToModel(entity.Person);

			entity.PortalId = from.PortalId;
			if (from.Portal != null)
				entity.Portal = from.Portal.ToModel(entity.Portal);

			entity.ReaderId = from.ReaderId;
			if (from.Reader != null)
				entity.Reader = from.Reader.ToModel(entity.Reader);

			return entity;
		}
		#endregion

		#region DB Access
		#region ExternalSystem
		public static RSMDB.ExternalSystem Select(this ExternalSystem entity, RSMDB.RSMDataModelDataContext context)
		{
			return context.ExternalSystems.FirstOrDefault(x => x.Id == entity.Id);
		}
		#endregion

		#region ExternalApplicationKey
		public static RSMDB.ExternalApplicationKey Select(this ExternalEntity entity, RSMDB.RSMDataModelDataContext context)
		{
			var type = Enum.GetName(typeof(EntityType), entity.EntityType);

			return context.ExternalApplicationKeys.FirstOrDefault(x =>
				x.EntityType == type
				&& x.SystemId == entity.ExternalSystemId
				&& x.InternalId == entity.InternalId);
		}
		public static RSMDB.ExternalApplicationKey SelectExternal(this ExternalEntity entity, RSMDB.RSMDataModelDataContext context)
		{
			var type = Enum.GetName(typeof(EntityType), entity.EntityType);

			return context.ExternalApplicationKeys.FirstOrDefault(x =>
				x.EntityType == type
				&& x.SystemId == entity.ExternalSystemId
				&& x.ExternalId == entity.ExternalId);
		}
		public static RSMDB.ExternalApplicationKey InsertKeys(this ExternalEntity entity, RSMDB.RSMDataModelDataContext context)
		{
			var row = new RSMDB.ExternalApplicationKey
			{
				InternalId = entity.InternalId,
				SystemId = entity.ExternalSystemId,
				EntityType = Enum.GetName(typeof(EntityType), entity.EntityType),
				ExternalId = entity.ExternalId,
				Added = entity.KeysAdded
			};

			if (entity.ExternalUpdated != default(DateTime))
				row.ExternalEntityLastUpdated = entity.ExternalUpdated;

			context.ExternalApplicationKeys.InsertOnSubmit(row);

			context.SubmitChanges();

			return row;
		}
		public static IEnumerable<RSMDB.ExternalApplicationKey> SearchKeys(this ExternalEntity entity, RSMDB.RSMDataModelDataContext context,
			Expression<Func<RSMDB.ExternalApplicationKey, bool>> filterExpression)
		{
			return context.ExternalApplicationKeys.Where(filterExpression.Compile());
		}
		public static RSMDB.ExternalApplicationKey UpdateKeys(this ExternalEntity entity, RSMDB.RSMDataModelDataContext context)
		{
			var row = DataExtensions.Select(entity, context);
			if (row == null)
				return row;

			if (entity.ExternalUpdated == default(DateTime))
				row.ExternalEntityLastUpdated = null;
			else
				row.ExternalEntityLastUpdated = entity.ExternalUpdated;

			context.SubmitChanges();

			return row;
		}
		#endregion

		#region Person
		public static RSMDB.Person Select(this Person entity, RSMDB.RSMDataModelDataContext context)
		{
			return context.Persons.FirstOrDefault(x => x.PersonID == entity.InternalId);
		}
		public static RSMDB.Person Insert(this Person entity, RSMDB.RSMDataModelDataContext context)
		{
			var ts = DateTime.Now;
			var row = new RSMDB.Person
			{
				FirstName = entity.FirstName,
				LastName = entity.LastName,
				MiddleName = entity.MiddleName,
				Active = entity.Active,
				Added = ts,
				LastUpdated = ts,
				UDF1 = entity.udf1,
				UDF2 = entity.udf2,
				UDF3 = entity.udf3,
				UDF4 = entity.udf4,
				UDF5 = entity.udf5,
				UDF6 = entity.udf6,
				UDF7 = entity.udf7,
				UDF8 = entity.udf8,
				UDF9 = entity.udf9,
				UDF10 = entity.udf10,
				UDF11 = entity.udf11,
				UDF12 = entity.udf12,
				UDF13 = entity.udf13,
				UDF14 = entity.udf14,
				UDF15 = entity.udf15,
				UDF16 = entity.udf16,
				UDF17 = entity.udf17,
				UDF18 = entity.udf18,
				UDF19 = entity.udf19,
				UDF20 = entity.udf20,
				BadgeNumber = entity.BadgeNumber,
				Image = entity.Image != null ? entity.Image : default(Binary),
			};

			context.Persons.InsertOnSubmit(row);

			context.SubmitChanges();

			return row;
		}
		public static RSMDB.Person Update(this Person entity, RSMDB.RSMDataModelDataContext context, bool overrideLastUpdate = true)
		{
			var row = Select(entity, context);

			if (row == null) return null;

			row.LastUpdated = overrideLastUpdate ? DateTime.Now : entity.LastUpdated;

			row.FirstName = entity.FirstName;
			row.LastName = entity.LastName;
			row.MiddleName = entity.MiddleName;
			row.Active = entity.Active;
			row.UDF1 = entity.udf1;
			row.UDF2 = entity.udf2;
			row.UDF3 = entity.udf3;
			row.UDF4 = entity.udf4;
			row.UDF5 = entity.udf5;
			row.UDF6 = entity.udf6;
			row.UDF7 = entity.udf7;
			row.UDF8 = entity.udf8;
			row.UDF9 = entity.udf9;
			row.UDF10 = entity.udf10;
			row.UDF11 = entity.udf11;
			row.UDF12 = entity.udf12;
			row.UDF13 = entity.udf13;
			row.UDF14 = entity.udf14;
			row.UDF15 = entity.udf15;
			row.UDF16 = entity.udf16;
			row.UDF17 = entity.udf17;
			row.UDF18 = entity.udf18;
			row.UDF19 = entity.udf19;
			row.UDF20 = entity.udf20;
			row.BadgeNumber = entity.BadgeNumber;

			if(entity.Image != null)
				row.Image = entity.Image;

			context.SubmitChanges();

			return row;
		}
		public static IQueryable<RSMDB.Person> Search(this Person entity, RSMDB.RSMDataModelDataContext context,
			Expression<Func<RSMDB.Person, bool>> filterExpression)
		{
			return context.Persons.Where(filterExpression);
		}
		#endregion

		#region Reader
		public static RSMDB.Reader Select(this Reader entity, RSMDB.RSMDataModelDataContext context)
		{
			return context.Readers.FirstOrDefault(x => x.Id == entity.InternalId);
		}
		#endregion

		#region Portal
		public static RSMDB.Portal Select(this Portal entity, RSMDB.RSMDataModelDataContext context)
		{
			return context.Portals.FirstOrDefault(x => x.Id == entity.InternalId);
		}
		#endregion

		#region Location
		public static RSMDB.Location Select(this Location entity, RSMDB.RSMDataModelDataContext context)
		{
			return context.Locations.FirstOrDefault(x => x.LocationID == entity.InternalId);
		}
		#endregion

		#region AccessHistory
		public static RSMDB.AccessHistory Select(this AccessLog entity, RSMDB.RSMDataModelDataContext context)
		{
			return context.AccessHistories.FirstOrDefault(x => x.Id == entity.InternalId);
		}
		public static RSMDB.AccessHistory Insert(this AccessLog entity, RSMDB.RSMDataModelDataContext context)
		{
			var row = new RSMDB.AccessHistory
			{
				PersonId = entity.PersonId,
				PortalId = entity.PortalId,
				ReaderId = entity.ReaderId,
				Reason = entity.Reason,
				Type = entity.AccessType,
				Accessed = entity.Accessed,
			};

			context.AccessHistories.InsertOnSubmit(row);

			context.SubmitChanges();

			return row;
		}
		#endregion

		#endregion
	}
}
