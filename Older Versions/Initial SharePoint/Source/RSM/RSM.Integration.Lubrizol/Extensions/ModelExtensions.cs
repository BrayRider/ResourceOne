using System;
using System.Transactions;

using RSM.Service.Library;
using RSM.Service.Library.Model.Reflection;

using RSM.Integration.Lubrizol.Model;
using LzEmployee = RSM.Integration.Lubrizol.Model.tblzILMData;
using R1Employee = RSM.Integration.Lubrizol.Model.Lubrizol_Employee;

namespace RSM.Integration.Lubrizol.Extensions
{
	public static class ModelExtensions
	{
		public static TimeSpan TransactionTimeout = TimeSpan.FromMinutes(10);

		public static Result<R1Employee> Get(this R1Employee from)
		{
			var result = Result<R1Employee>.Success();

			try
			{
				using (var db = new RSMLubrizolDataModelDataContext())
				{
					var row = from.Select(db);

					if (row == null)
						return result.Fail("R1Employee not found", "NotFound");

					result.Entity = row;
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Get R1Employee failed. {0}", e.ToString());
			}

			return result;
		}

		public static Result<R1Employee> Add(this R1Employee from)
		{
			var result = Result<R1Employee>.Success();

			var exists = from.Get();
			if (exists.Succeeded)
			{
				exists.Set(ResultType.Warning, "R1Employee already exists {0}", from.EmployeeID);
				return exists;
			}

			try
			{
				using (var db = new RSMLubrizolDataModelDataContext())
				{
					using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionTimeout))
					{
						var row = from.Insert(db);
						if (row == null)
							return result.Fail("Add R1Employee failed");

						result.Entity = row;

						transaction.Complete();
					}
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Add R1Employee failed {0}", e.ToString());
			}

			return result;
		}

		public static Result<R1Employee> Update(this R1Employee from, ModelMapper<R1Employee> mapper = null)
		{
			var result = Result<R1Employee>.Success();

			try
			{
				using (var db = new RSMLubrizolDataModelDataContext())
				{
					using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionTimeout))
					{
						var row = DataExtensions.Select(from, db);
						if (row == null)
							return result.Fail("Update R1Employee failed");

						if (!row.SameAs(from))
						{
							if (mapper != null)
							{
								mapper.MapProperties(from, row);
							}
							else
							{
								row.FirstName = from.FirstName;
								row.LastName = from.LastName;
								row.MiddleName = from.MiddleName;
								row.Company = from.Company;
								row.Country = from.Country;
								row.Department = from.Department;
								row.DepartmentName = from.DepartmentName;
								row.Division = from.Division;
								row.EmployeeClassDesc = from.EmployeeClassDesc;
								row.EmployeeStatus = from.EmployeeStatus;
								row.EmployeeStatusDesc = from.EmployeeStatusDesc;
								row.Initials = from.Initials;
								row.JobDescr = from.JobDescr;
								row.LastLoadDate = from.LastLoadDate;
								row.LegalEntity = from.LegalEntity;
								row.Name = from.Name;
								row.PhysicalLocation = from.PhysicalLocation;
								row.PhysicalLocationName = from.PhysicalLocationName;
								row.ReportingLocation = from.ReportingLocation;
								row.ReportingLocationName = from.ReportingLocationName;
								row.SupervisorID = from.SupervisorID;
								row.SupervisorInitials = from.SupervisorInitials;
								row.SupervisorName = from.SupervisorName;
							}

							row.LastUpdated = DateTime.Now;
							db.SubmitChanges();

							transaction.Complete();

							result.Entity = row;
						}
						else
						{
							result.Entity = from;
						}
					}
				}
			}
			catch (Exception e)
			{
				return result.Set(ResultType.TechnicalError, e, "Update R1Employee failed {0}", e.ToString());
			}

			return result;
		}

		public static bool SameAs(this R1Employee from, R1Employee to)
		{
			try
			{
				return to.FirstName == from.FirstName
				       && to.LastName == from.LastName
				       && to.MiddleName == from.MiddleName
				       && to.Company == from.Company
				       && to.Country == from.Country
				       && to.Department == from.Department
				       && to.DepartmentName == from.DepartmentName
				       && to.Division == from.Division
				       && to.EmployeeClassDesc == from.EmployeeClassDesc
				       && to.EmployeeStatus == from.EmployeeStatus
				       && to.EmployeeStatusDesc == from.EmployeeStatusDesc
				       && to.Initials == from.Initials
				       && to.JobDescr == from.JobDescr
				       && to.LegalEntity == from.LegalEntity
				       && to.Name == from.Name
				       && to.PhysicalLocation == from.PhysicalLocation
				       && to.PhysicalLocationName == from.PhysicalLocationName
				       && to.ReportingLocation == from.ReportingLocation
				       && to.ReportingLocationName == from.ReportingLocationName
				       && to.SupervisorID == from.SupervisorID
				       && to.SupervisorInitials == from.SupervisorInitials
				       && to.SupervisorName == from.SupervisorName;
			}
			catch (Exception e)
			{
				return false;
			}
		}
	}
}
