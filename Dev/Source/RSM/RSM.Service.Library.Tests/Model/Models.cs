using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSM.Integration.S2;
using RSM.Integration.S2.Import;
using RSM.Service.Library.Extensions;
using RSM.Service.Library.Model;
using RSMDB = RSM.Support;
using DataFactory = RSM.Staging.Library.Factory;

namespace RSM.Service.Library.Tests.Model
{
	[TestClass]
	public class Models : Test
	{
		[TestMethod]
		public void ExternalSystem_Get()
		{
			var criteria = new ExternalSystem { Id = S2In.Id };
			var result = criteria.Get();

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, result.ToString());
			Assert.IsNotNull(result.Entity, "Missing entity");
			Assert.IsTrue(result.Entity.EntityType == S2In.EntityType, "EntityType mismatch");
		}

		[TestMethod]
		public void ExternalEntity_Get_External()
		{
			var externalId = "50";
			var id = 1;
			var keys = DataFactory.CreateExternalApplicationKey(EntityType.Person, externalId, S2In.Id, id);

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				context.ExternalApplicationKeys.InsertOnSubmit(keys);
				context.SubmitChanges();
			}

			var criteria = new ExternalEntity {
				EntityType = EntityType.Person, 
				ExternalSystemId = S2In.Id,
				ExternalId = externalId };

			var result = criteria.GetKeys();

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, result.ToString());
			Assert.IsNotNull(result.Entity, "Missing entity");
			Assert.IsNotNull(result.Entity.ExternalSystem, "Missing ExternalSystem entity");
			Assert.IsTrue(result.Entity.EntityType == EntityType.Person, "EntityType mismatch");
		}

		public void ExternalEntity_Get_Internal()
		{
			var externalId = "50";
			var id = 1;
			var keys = DataFactory.CreateExternalApplicationKey(EntityType.Person, externalId, S2In.Id, id);

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				context.ExternalApplicationKeys.InsertOnSubmit(keys);
				context.SubmitChanges();
			}

			var criteria = new ExternalEntity
			{
				EntityType = EntityType.Person,
				ExternalSystemId = S2In.Id,
				InternalId = id
			};

			var result = criteria.GetKeys(SelectKeys.Internal);

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, result.ToString());
			Assert.IsNotNull(result.Entity, "Missing entity");
			Assert.IsNotNull(result.Entity.ExternalSystem, "Missing ExternalSystem entity");
			Assert.IsTrue(result.Entity.EntityType == EntityType.Person, "EntityType mismatch");
		}

		[TestMethod]
		public void Person_Add()
		{
			var externalId = "50";

			var criteria = new Person
			{
				EntityType = EntityType.Person,
				ExternalSystemId = S2In.Id,
				ExternalId = externalId,
				FirstName = "FirstName",
				LastName = "Last",
				Added = DateTime.Now,
				udf4 = "Contractor company A"
			};

			var result = criteria.Add();

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, result.ToString());
			Assert.IsNotNull(result.Entity, "Missing entity");
			Assert.IsNotNull(result.Entity.ExternalSystem, "Missing ExternalSystem entity");
			Assert.IsTrue(result.Entity.EntityType == EntityType.Person, "EntityType mismatch");
			Assert.IsTrue(result.Entity.InternalId > 0, "Invalid id");

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				var keys = context.ExternalApplicationKeys.FirstOrDefault(x => x.ExternalId == externalId
					&& x.SystemId == S2In.Id && x.EntityType == Enum.GetName(typeof(EntityType), EntityType.Person));
				Assert.IsNotNull(keys, "Keys not created");

				var row = context.Persons.FirstOrDefault(x => x.PersonID == keys.InternalId);
				Assert.IsNotNull(row, "Person not created");
			}
		}

        [TestMethod]
        public void Person_Update()
        {

            var externalId = "50";

            var criteria = new Person
            {
                EntityType = EntityType.Person,
                ExternalSystemId = S2In.Id,
                ExternalId = externalId,
                FirstName = "FirstName",
                LastName = "Last",
                Added = DateTime.Now,
                udf4 = "Contractor company A"
            };

            // Not testing Add
            var result = criteria.Add();

            result.Entity.FirstName = "Updated First Name";
            var test = result.Entity;
            test.InternalId = 0;

            var updateResult = test.Update();

            Assert.IsNotNull(updateResult, "Missing results");
            Assert.IsTrue(updateResult.Succeeded, updateResult.ToString());
            Assert.IsNotNull(updateResult.Entity, "Missing entity");
            Assert.IsNotNull(updateResult.Entity.ExternalSystem, "Missing ExternalSystem entity");
            Assert.IsTrue(updateResult.Entity.EntityType == EntityType.Person, "EntityType mismatch");
            Assert.IsTrue(updateResult.Entity.InternalId > 0, "Invalid id");
            Assert.IsTrue(updateResult.Entity.FirstName == "Updated First Name", "Name not changed");

            using (var context = new RSMDB.RSMDataModelDataContext())
            {
                var keys = context.ExternalApplicationKeys.FirstOrDefault(x => x.ExternalId == externalId
                    && x.SystemId == S2In.Id && x.EntityType == Enum.GetName(typeof(EntityType), EntityType.Person));
                Assert.IsNotNull(keys, "Keys not created");

                var row = context.Persons.FirstOrDefault(x => x.PersonID == keys.InternalId);
                Assert.IsNotNull(row, "Person not created");
            }
        }

        [TestMethod]
		public void Person_Get_External()
		{
			var externalId = "50";
			var id = 1;
			var person = DataFactory.CreatePerson("John", "Smith", "middle");

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				context.Persons.InsertOnSubmit(person);
				context.SubmitChanges();

				id = person.PersonID;

				var keys = DataFactory.CreateExternalApplicationKey(EntityType.Person, externalId, S2In.Id, id);
				context.ExternalApplicationKeys.InsertOnSubmit(keys);
				context.SubmitChanges();
			}

			var criteria = new Person
			{
				EntityType = EntityType.Person,
				ExternalSystemId = S2In.Id,
				ExternalId = externalId
			};

			var result = criteria.Get();

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, result.ToString());
			Assert.IsNotNull(result.Entity, "Missing entity");
			Assert.IsNotNull(result.Entity.ExternalSystem, "Missing ExternalSystem entity");
			Assert.IsTrue(result.Entity.EntityType == EntityType.Person, "EntityType mismatch");
			Assert.IsTrue((result.Entity as ExternalEntity).InternalId == id, "Incorrect id for entity");
		}

		[TestMethod]
		public void Portal_Get_External()
		{
			var externalId = "Portal50";
			var id = 1;
			var locationId = 1;

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				var location = DataFactory.CreateLocation("Location1");
				context.Locations.InsertOnSubmit(location);
				context.SubmitChanges();
				locationId = location.LocationID;

				var portal = DataFactory.CreatePortal("Portal1", locationId);
				context.Portals.InsertOnSubmit(portal);
				context.SubmitChanges();

				id = portal.Id;

				var keys = DataFactory.CreateExternalApplicationKey(EntityType.Portal, externalId, S2In.Id, id);
				context.ExternalApplicationKeys.InsertOnSubmit(keys);
				context.SubmitChanges();
			}

			var criteria = new Portal
			{
				EntityType = EntityType.Portal,
				ExternalSystemId = S2In.Id,
				ExternalId = externalId
			};

			var result = criteria.Get();

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, result.ToString());
			Assert.IsNotNull(result.Entity, "Missing entity");
			Assert.IsNotNull(result.Entity.ExternalSystem, "Missing ExternalSystem entity");
			Assert.IsTrue(result.Entity.EntityType == EntityType.Portal, "EntityType mismatch");
			Assert.IsTrue((result.Entity as ExternalEntity).InternalId == id, "Incorrect id for entity");
		}

		[TestMethod]
		public void Reader_Get_External()
		{
			var externalId = "Reader50";
			var id = 1;
			var locationId = 1;

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				var location = DataFactory.CreateLocation("Location1");
				context.Locations.InsertOnSubmit(location);
				context.SubmitChanges();
				locationId = location.LocationID;

				var portal = DataFactory.CreatePortal("Portal1", locationId);
				context.Portals.InsertOnSubmit(portal);
				context.SubmitChanges();
				var keys = DataFactory.CreateExternalApplicationKey(EntityType.Portal, externalId, S2In.Id, portal.Id);
				context.ExternalApplicationKeys.InsertOnSubmit(keys);

				var reader = DataFactory.CreateReader("Reader50", portal.Id);
				context.Readers.InsertOnSubmit(reader);
				context.SubmitChanges();
				keys = DataFactory.CreateExternalApplicationKey(EntityType.Reader, externalId, S2In.Id, reader.Id);
				context.ExternalApplicationKeys.InsertOnSubmit(keys);

				context.SubmitChanges();

				id = reader.Id;
			}

			var criteria = new Reader
			{
				EntityType = EntityType.Reader,
				ExternalSystemId = S2In.Id,
				ExternalId = externalId
			};

			var result = criteria.Get();

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, result.ToString());
			Assert.IsNotNull(result.Entity, "Missing entity");
			Assert.IsNotNull(result.Entity.ExternalSystem, "Missing ExternalSystem entity");
			Assert.IsTrue(result.Entity.EntityType == EntityType.Reader, "EntityType mismatch");
			Assert.IsTrue((result.Entity as ExternalEntity).InternalId == id, "Incorrect id for entity");
		}

		[TestMethod]
		public void AccessLog_Get_External()
		{
			var externalId = "Access50";
			var id = 1;
			var locationId = 1;

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				var person = DataFactory.CreatePerson("first", "last", "middle");
				context.Persons.InsertOnSubmit(person);
				context.SubmitChanges();
				var keys = DataFactory.CreateExternalApplicationKey(EntityType.Person, externalId, S2In.Id, person.PersonID);
				context.ExternalApplicationKeys.InsertOnSubmit(keys);

				var location = DataFactory.CreateLocation("Location1");
				context.Locations.InsertOnSubmit(location);
				context.SubmitChanges();
				locationId = location.LocationID;

				var portal = DataFactory.CreatePortal("Portal1", locationId);
				context.Portals.InsertOnSubmit(portal);
				context.SubmitChanges();
				keys = DataFactory.CreateExternalApplicationKey(EntityType.Portal, externalId, S2In.Id, portal.Id);
				context.ExternalApplicationKeys.InsertOnSubmit(keys);

				var reader = DataFactory.CreateReader("Reader50", portal.Id);
				context.Readers.InsertOnSubmit(reader);
				context.SubmitChanges();
				keys = DataFactory.CreateExternalApplicationKey(EntityType.Reader, externalId, S2In.Id, reader.Id);
				context.ExternalApplicationKeys.InsertOnSubmit(keys);

				var access = DataFactory.CreateAccessHistory("Access50", person.PersonID, portal.Id, reader.Id, 30);
				context.AccessHistories.InsertOnSubmit(access);
				context.SubmitChanges();
				keys = DataFactory.CreateExternalApplicationKey(EntityType.AccessLog, externalId, S2In.Id, access.Id);
				context.ExternalApplicationKeys.InsertOnSubmit(keys);
				context.SubmitChanges();

				id = access.Id;
			}

			var criteria = new AccessLog
			{
				EntityType = EntityType.AccessLog,
				ExternalSystemId = S2In.Id,
				ExternalId = externalId
			};

			var result = criteria.Get();

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, result.ToString());
			Assert.IsNotNull(result.Entity, "Missing entity");
			Assert.IsNotNull(result.Entity.ExternalSystem, "Missing ExternalSystem entity");
			Assert.IsTrue(result.Entity.EntityType == EntityType.AccessLog, "EntityType mismatch");
			Assert.IsTrue((result.Entity as ExternalEntity).InternalId == id, "Incorrect id for entity");
		}

		[TestMethod]
		public void AccessLog_Add()
		{
			var externalId = "Access50";
			var id = 1;
			var locationId = 1;

			var criteria = new AccessLog
			{
				EntityType = EntityType.AccessLog,
				ExternalSystemId = S2In.Id,
				ExternalId = externalId
			};

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				var person = DataFactory.CreatePerson("first", "last", "middle");
				context.Persons.InsertOnSubmit(person);
				context.SubmitChanges();
				var keys = DataFactory.CreateExternalApplicationKey(EntityType.Person, externalId, S2In.Id, person.PersonID);
				context.ExternalApplicationKeys.InsertOnSubmit(keys);

				var location = DataFactory.CreateLocation("Location1");
				context.Locations.InsertOnSubmit(location);
				context.SubmitChanges();
				locationId = location.LocationID;

				var portal = DataFactory.CreatePortal("Portal1", locationId);
				context.Portals.InsertOnSubmit(portal);
				context.SubmitChanges();
				keys = DataFactory.CreateExternalApplicationKey(EntityType.Portal, externalId, S2In.Id, portal.Id);
				context.ExternalApplicationKeys.InsertOnSubmit(keys);

				var reader = DataFactory.CreateReader("Reader50", portal.Id);
				context.Readers.InsertOnSubmit(reader);
				context.SubmitChanges();
				keys = DataFactory.CreateExternalApplicationKey(EntityType.Reader, externalId, S2In.Id, reader.Id);
				context.ExternalApplicationKeys.InsertOnSubmit(keys);

				criteria.PersonId = person.PersonID;
				criteria.PortalId = portal.Id;
				criteria.ReaderId = reader.Id;
				criteria.AccessType = 123;
				criteria.Accessed = DateTime.Now.Subtract(TimeSpan.FromMinutes(10));
			}

			var result = criteria.Add();

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, result.ToString());
			Assert.IsNotNull(result.Entity, "Missing entity");
			Assert.IsNotNull(result.Entity.ExternalSystem, "Missing ExternalSystem entity");
			Assert.IsTrue(result.Entity.EntityType == EntityType.AccessLog, "EntityType mismatch");
			Assert.IsTrue(result.Entity.InternalId > 0, "Invalid id");

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				var keys = context.ExternalApplicationKeys.FirstOrDefault(x => x.ExternalId == externalId
					&& x.SystemId == S2In.Id && x.EntityType == Enum.GetName(typeof(EntityType), EntityType.AccessLog));
				Assert.IsNotNull(keys, "Keys not created");

				var row = context.AccessHistories.FirstOrDefault(x => x.Id == keys.InternalId);
				Assert.IsNotNull(row, "AccessHistory not created");
			}
		}

        [TestMethod]
        public void AccessLog_AddBadFilter()
        {
            var externalId = "Access50";
            var id = 1;
            var locationId = 1;

            var criteria = new AccessLog
            {
                EntityType = EntityType.AccessLog,
                ExternalSystemId = S2In.Id,
                ExternalId = externalId
            };

            using (var context = new RSMDB.RSMDataModelDataContext())
            {
                var person = DataFactory.CreatePerson("first", "last", "middle", null, false, false, null, null, false, null, null, "Test", null, new Dictionary<int, string>() { {4, "S & B"} });
                context.Persons.InsertOnSubmit(person);
                context.SubmitChanges();
                var keys = DataFactory.CreateExternalApplicationKey(EntityType.Person, externalId, S2In.Id, person.PersonID);
                context.ExternalApplicationKeys.InsertOnSubmit(keys);
                context.SubmitChanges();

                var location = DataFactory.CreateLocation("Location1");
                context.Locations.InsertOnSubmit(location);
                context.SubmitChanges();
                locationId = location.LocationID;
                context.SubmitChanges();

                var portal = DataFactory.CreatePortal("Portal1", locationId);
                context.Portals.InsertOnSubmit(portal);
                context.SubmitChanges();
                keys = DataFactory.CreateExternalApplicationKey(EntityType.Portal, externalId, S2In.Id, portal.Id);
                context.ExternalApplicationKeys.InsertOnSubmit(keys);
                context.SubmitChanges();

                var reader = DataFactory.CreateReader("Reader50", portal.Id);
                context.Readers.InsertOnSubmit(reader);
                context.SubmitChanges();
                keys = DataFactory.CreateExternalApplicationKey(EntityType.Reader, externalId, S2In.Id, reader.Id);
                context.ExternalApplicationKeys.InsertOnSubmit(keys);
                context.SubmitChanges();

                criteria.PersonId = person.PersonID;
                criteria.PortalId = portal.Id;
                criteria.ReaderId = reader.Id;
                criteria.AccessType = (int)AccessType.Valid;
                criteria.Accessed = DateTime.Now.Subtract(TimeSpan.FromMinutes(10));
            }

            var result = criteria.Add();
            result.Entity.Person.ExternalId = externalId;
            result.Entity.Reader.ExternalId = externalId;
            result.Entity.Portal.ExternalId = externalId;

            var filterTest = new Integration.S2.Import.AccessHistory();
//            filterTest.Configuration = 
            filterTest.Load();

            Assert.IsFalse(filterTest.Filter(result.Entity), "Filter did not remove!");

        }
    }
}
