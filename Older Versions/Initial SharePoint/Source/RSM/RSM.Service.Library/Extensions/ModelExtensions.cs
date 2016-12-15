using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using RSM.Service.Library.Model;
using System.Diagnostics;
using System.Transactions;
using RSMDB = RSM.Support;

namespace RSM.Service.Library.Extensions
{
	public enum SelectKeys
	{
		Internal,
		External
	}

	public static class ModelExtensions
	{
		public static TimeSpan TransactionTimeout = TimeSpan.FromMinutes(10);

		#region ExternalSystem
		public static Result<ExternalSystem> Get(this ExternalSystem from)
		{
			var result = Result<ExternalSystem>.Success();

			try
			{
				using (var db = new RSMDB.RSMDataModelDataContext())
				{
					var row = from.Select(db);
					if (row == null)
						return result.Fail("ExternalSystem not found");

					result.Entity = row.ToModel();
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Get ExternalSystem failed. {0}", e.ToString());
			}

			return result;
		}
		#endregion

		#region ExternalEntity
		/// <summary>
		/// Adds keys for an external entity to the database.  No existence check is performed prior to inserting.
		/// </summary>
		/// <param name="from">Entity to add</param>
		/// <returns></returns>
		public static Result<ExternalEntity> Add(this ExternalEntity from)
		{
			var result = Result<ExternalEntity>.Success();

			try
			{
				using (var db = new RSMDB.RSMDataModelDataContext())
				{
					using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionTimeout))
					{
						if (from.KeysAdded == DateTime.MinValue)
							from.KeysAdded = DateTime.Now;

						var row = from.InsertKeys(db);
						if (row == null)
							return result.Fail("Add external entity keys failed");

						result.Entity = new ExternalEntity();
						result.Entity = row.ToModel(result.Entity);

						transaction.Complete();
					}
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Add external entity keys failed {0}", e.ToString());
			}

			return result;
		}
		/// <summary>
		/// Get External entity keys.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="keyType"><typeparamref name="SelectKeys"/></param>
		/// <param name="replace">If true, criteria entity will be replaced with values retrieved.</param>
		/// <returns></returns>
		public static Result<ExternalEntity> GetKeys(this ExternalEntity from, SelectKeys keyType = SelectKeys.External, bool replace = false)
		{
			var result = Result<ExternalEntity>.Success();

			try
			{
				using (var db = new RSMDB.RSMDataModelDataContext())
				{
					var row = keyType == SelectKeys.External
						? from.SelectExternal(db)
						: from.Select(db);

					if (row == null)
						return result.Fail("ExternalEntity not found", "NotFound");

					Debug.Assert(row.ExternalSystem != null, "ExternalSystem instance was not fetched!");

					result.Entity = row.ToModel<ExternalEntity>(replace ? from : null);
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Get ExternalEntity failed. {0}", e.ToString());
			}

			return result;
		}

		public static bool KeysExist(this ExternalEntity from, SelectKeys keyType = SelectKeys.External)
		{
			return from.GetKeys(keyType).Succeeded;
		}

		public static ExternalEntity MapKeys(this ExternalEntity from, ExternalEntity to)
		{
			Debug.Assert(from.EntityType == to.EntityType, "Attempting to map keys between different entity types!!");

			to.InternalId = from.InternalId;
			to.ExternalId = from.ExternalId;
			to.ExternalSystemId = from.ExternalSystemId;
			to.KeysAdded = from.KeysAdded;
			to.ExternalUpdated = from.ExternalUpdated;

			if (from.ExternalSystem != null)
				to.ExternalSystem = from.ExternalSystem;

			return to;
		}

		/// <summary>
		/// Search by specific criteria and optional date range on when Added.
		/// </summary>
		/// <param name="criteria">Fields considered: ExternalSystemId, EntityType</param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public static Result<List<ExternalEntity>> SearchKeys(this ExternalEntity criteria, DateTime? from = null, DateTime? to = null)
		{
			var result = new Result<List<ExternalEntity>>();

			try
			{
				using (var db = new RSMDB.RSMDataModelDataContext())
				{
					var type = Enum.GetName(typeof(EntityType), criteria.EntityType);

					var rows = criteria.SearchKeys(db, x => x.SystemId == criteria.ExternalSystemId
						&& x.EntityType == type
						&& (from == null || x.Added > from)
						&& (to == null || x.Added < to));
						
					result.Entity = rows.Select(x => x.ToModel<ExternalEntity>()).ToList();
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Get AccessLog failed. {0}", e.ToString());
			}

			return result;
		}

		public static Result<ExternalEntity> MostRecent(this ExternalEntity criteria, DateTime? from = null, DateTime? to = null)
		{
			var result = new Result<ExternalEntity>();

			try
			{
				using (var db = new RSMDB.RSMDataModelDataContext())
				{
					var type = Enum.GetName(typeof(EntityType), criteria.EntityType);

					var rows = criteria.SearchKeys(db, x => x.SystemId == criteria.ExternalSystemId
						&& x.EntityType == type
						&& (from == null || x.Added > from)
						&& (to == null || x.Added < to)).OrderByDescending(o => o.Added);

					result.Entity = rows.Select(x => x.ToModel<ExternalEntity>()).FirstOrDefault();

					if (result.Entity == null)
						return result.Fail("most recent external entity not found.");
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Get AccessLog failed. {0}", e.ToString());
			}

			return result;
		}

		public static Result<List<ExternalEntity>> PushList(this ExternalEntity src, ExternalEntity to)
		{
			var result = new Result<List<ExternalEntity>>();

			if (src.EntityType != to.EntityType)
				return result.Fail("cannot derive a push list on different entity types.");

			if (src.ExternalSystemId == to.ExternalSystemId)
				return result.Fail("cannot derive a push list from the same system.");

			try
			{
				using (var db = new RSMDB.RSMDataModelDataContext())
				{
					var type = Enum.GetName(typeof(EntityType), src.EntityType);

					var toQuery = db.ExternalApplicationKeys.Where(k => k.EntityType == type && k.SystemId == to.ExternalSystemId);
					var srcQuery = db.ExternalApplicationKeys.Where(k => k.EntityType == type && k.SystemId == src.ExternalSystemId);
					var rows = srcQuery.Where(s => ! toQuery.Any(t => t.ExternalId == s.ExternalId));

					result.Entity = rows.Select(x => x.ToModel<ExternalEntity>(null)).ToList();
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Get push list failed. {0}", e.ToString());
			}

			return result;
		}

		#endregion

		#region Person
		public static Result<Person> Get(this Person from, SelectKeys keyType = SelectKeys.External)
		{
			var result = Result<Person>.Success();

			var keys = (from as ExternalEntity).GetKeys(keyType);
			if (keys.Failed)
				return result.Merge(keys);

			try
			{
				keys.Entity.MapKeys(from);

				using (var db = new RSMDB.RSMDataModelDataContext())
				{
					var row = from.Select(db);

					if (row == null)
						return result.Fail("Person not found", "NotFound");

					result.Entity = row.ToModel();
					keys.Entity.MapKeys(result.Entity);
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Get Person failed. {0}", e.ToString());
			}

			return result;
		}

		public static Result<Person> Add(this Person from)
		{
			var result = Result<Person>.Success();

			var exists = from.Get();
			if (exists.Succeeded)
			{
				exists.Set(ResultType.Warning, "Person already exists {0}", from.InternalId.ToString());
				return exists;
			}

			try
			{
				using (var db = new RSMDB.RSMDataModelDataContext())
				{
					using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionTimeout))
					{
						var row = from.Insert(db);
						if (row == null)
							return result.Fail("Add Person failed");

						from.InternalId = row.PersonID;
						from.KeysAdded = row.Added;

						var keys = (from as ExternalEntity).InsertKeys(db);
						if (keys == null)
							return result.Fail("Add Person failed to save keys");

						result.Entity = row.ToModel();
						keys.ToModel(result.Entity);

						transaction.Complete();
					}
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Add Person failed {0}", e.ToString());
			}

			return result;
		}

		public static Result<Person> Update(this Person from, bool overrideLastUpdated = true)
		{
			var result = Result<Person>.Success();

			try
			{
				using (var db = new RSMDB.RSMDataModelDataContext())
				{
					using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionTimeout))
					{
						var row = from.Update(db, overrideLastUpdated);
						if (row == null)
							return result.Fail("Update Person failed");

						var keys = (from as ExternalEntity).UpdateKeys(db);
						if (keys == null)
							return result.Fail("Update Person failed to save keys");

						result.Entity = row.ToModel();
						keys.ToModel(result.Entity);

						transaction.Complete();
					}
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Update Person failed {0}", e.ToString());
			}

			return result;
		}

		public static Result<List<Person>> Search(this Person criteria, DateTime? updatedFrom = null)
		{
			var result = new Result<List<Person>>();

			try
			{
				using (var db = new RSMDB.RSMDataModelDataContext())
				{
					var where = criteria.Search(db, x => true);
					
					if (updatedFrom != null)
						where = where.Where(x => x.LastUpdated > updatedFrom);

					if (criteria.ExternalSystemId != 0)
					{
						var type = Enum.GetName(typeof(EntityType), EntityType.Person);

						where = where.Where(x => db.ExternalApplicationKeys.Any(k => k.InternalId == x.PersonID
							&& k.SystemId == criteria.ExternalSystemId
							&& k.EntityType == type));
					}

					where = where.OrderBy(x => x.LastUpdated);

					result.Entity = where.Select(x => x.ToModel(null)).ToList();

					if (criteria.ExternalSystemId != 0)
					{
						//hydrate the keys
						foreach (var entity in result.Entity)
						{
							// Add the missing External Key to the entity
							entity.ExternalSystemId = criteria.ExternalSystemId;
		
							((ExternalEntity)entity).GetKeys(SelectKeys.Internal, true);
						}
					}
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Search people failed. {0}", e.ToString());
			}

			return result;
		}
		#endregion

		#region Location
		public static Result<Location> Get(this Location from, SelectKeys keyType = SelectKeys.External)
		{
			var result = Result<Location>.Success();

			var keys = (from as ExternalEntity).GetKeys(keyType);
			if (keys.Failed)
				return result.Merge(keys);

			try
			{
				keys.Entity.MapKeys(from);

				using (var db = new RSMDB.RSMDataModelDataContext())
				{
					var row = from.Select(db);

					if (row == null)
						return result.Fail("Location not found");

					result.Entity = row.ToModel();
					keys.Entity.MapKeys(result.Entity);
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Get Location failed. {0}", e.ToString());
			}

			return result;
		}
		#endregion

		#region Portal
		public static Result<Portal> Get(this Portal from, SelectKeys keyType = SelectKeys.External)
		{
			var result = Result<Portal>.Success();

			var keys = (from as ExternalEntity).GetKeys(keyType);
			if (keys.Failed)
				return result.Merge(keys);

			try
			{
				keys.Entity.MapKeys(from);

				using (var db = new RSMDB.RSMDataModelDataContext())
				{
					var row = from.Select(db);

					if (row == null)
						return result.Fail("Portal not found");

					result.Entity = row.ToModel();
					keys.Entity.MapKeys(result.Entity);
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Get Portal failed. {0}", e.ToString());
			}

			return result;
		}
		#endregion

		#region Reader
		public static Result<Reader> Get(this Reader from, SelectKeys keyType = SelectKeys.External)
		{
			var result = Result<Reader>.Success();

			var keys = (from as ExternalEntity).GetKeys(keyType);
			if (keys.Failed)
				return result.Merge(keys);

			try
			{
				keys.Entity.MapKeys(from);

				using (var db = new RSMDB.RSMDataModelDataContext())
				{
					var row = from.Select(db);

					if (row == null)
						return result.Fail("Reader not found");

					result.Entity = row.ToModel();
					keys.Entity.MapKeys(result.Entity);
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Get Reader failed. {0}", e.ToString());
			}

			return result;
		}
		#endregion

		#region AccessLog
		public static Result<AccessLog> Get(this AccessLog from, SelectKeys keyType = SelectKeys.External)
		{
			var result = Result<AccessLog>.Success();

			var keys = (from as ExternalEntity).GetKeys(keyType);
			if (keys.Failed)
				return result.Merge(keys);

			try
			{
				keys.Entity.MapKeys(from);

				using (var db = new RSMDB.RSMDataModelDataContext())
				{
					var row = from.Select(db);

					if (row == null)
						return result.Fail("AccessLog not found");

					result.Entity = row.ToModel();
					keys.Entity.MapKeys(result.Entity);
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Get AccessLog failed. {0}", e.ToString());
			}

			return result;
		}
		public static Result<AccessLog> Add(this AccessLog from)
		{
			var result = Result<AccessLog>.Success();

			var exists = from.Get();
			if (exists.Succeeded)
			{
				exists.Set(ResultType.Warning, "AccessLog already exists {0}", from.InternalId.ToString());
				return exists;
			}

			try
			{
				using (var db = new RSMDB.RSMDataModelDataContext())
				{
					using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionTimeout))
					{
						var row = from.Insert(db);
						if (row == null)
							return result.Fail("Add AccessLog failed");

						from.InternalId = row.Id;
						from.KeysAdded = DateTime.Now;

						var keys = (from as ExternalEntity).InsertKeys(db);

						if (row == null || keys == null)
							return result.Fail("Add AccessLog failed");

						result.Entity = row.ToModel();
						keys.ToModel(result.Entity);

						transaction.Complete();
					}
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Add AccessLog failed. {0}", e.ToString());
			}

			return result;
		}
		#endregion

	}
}
