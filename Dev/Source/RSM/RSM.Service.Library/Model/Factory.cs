using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ExternalSystemDirection = RSM.Support.ExternalSystemDirection;

namespace RSM.Service.Library.Model
{
	public class Factory
	{
        public static T CreateExternalEntity<T>(EntityType type, string externalId = null, ExternalSystem system = null, int internalId = 0)
            where T : ExternalEntity, new()
        {
            var entity = new T()
            {
                EntityType = type,
                ExternalId = externalId,
                ExternalSystem = system,
                ExternalSystemId = system.Id,
                InternalId = internalId
            };
            return entity;
        }
        public static Person CreatePerson(string externalId = null, ExternalSystem system = null, int internalId = 0)
		{
			var entity = CreateExternalEntity<Person>(EntityType.Person, externalId: externalId, system: system, internalId: internalId);
			return entity;
		}
        public static Portal CreatePortal(string externalId = null, ExternalSystem system = null, int internalId = 0)
		{
            var entity = CreateExternalEntity<Portal>(EntityType.Portal, externalId: externalId, system: system, internalId: internalId);
			return entity;
		}
        public static Reader CreateReader(string externalId = null, ExternalSystem system = null, int internalId = 0)
		{
            var entity = CreateExternalEntity<Reader>(EntityType.Reader, externalId: externalId, system: system, internalId: internalId);
			return entity;
		}
        public static Location CreateLocation(string externalId = null, ExternalSystem system = null, int internalId = 0)
		{
            var entity = CreateExternalEntity<Location>(EntityType.Location, externalId: externalId, system: system, internalId: internalId);
			return entity;
		}
        public static AccessLog CreateAccessLog(string externalId = null, ExternalSystem system = null,
            string personExtId = null,
            string portalExtId = null,
            string readerExtId = null,
            DateTime? accessed = null,
            int accessType = 0,
            int accessId = 0,
            int personId = 0,
            int portalId = 0,
            int readerId = 0)
        {
            var entity = CreateExternalEntity<AccessLog>(EntityType.AccessLog
                , externalId: externalId, system: system, internalId: accessId);

            entity.Person = personExtId != null ? CreatePerson(personExtId, system, personId) : entity.Person;
            entity.Portal = portalExtId != null ? CreatePortal(portalExtId, system, portalId) : entity.Portal;
            entity.Reader = readerExtId != null ? CreateReader(readerExtId, system, readerId) : entity.Reader;
            entity.Accessed = accessed != null ? (DateTime)accessed : entity.Accessed;
            entity.AccessType = accessType;

            return entity;
        }

		public static ExternalSystem CreateExternalSystem(int id, string name = "", 
			ExternalSystemDirection direction = ExternalSystemDirection.None)
		{
			return new ExternalSystem {
				Id = id,
				Name = name,
				Direction = direction
			};
		}
	}
}
