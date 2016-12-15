using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RSM.Integration.Lubrizol.Proxy;
using RSM.Service.Library;
using RSM.Service.Library.Interfaces;
using RSM.Service.Library.Model.Reflection;
using RSM.Support;
using Person = RSM.Service.Library.Model.Person;
using UserSession = RSM.Service.Library.Model.UserSession;

using RSM.Integration.Lubrizol.Model;
using LzEmployee = RSM.Integration.Lubrizol.Model.tblzILMData;
using R1Employee = RSM.Integration.Lubrizol.Model.Lubrizol_Employee;


namespace RSM.Integration.Lubrizol
{
	public class API : IAPI, IAuthentication, IExportPeople
	{
		public string Name { get; private set; }
		public string Version { get; private set; }
		public ModelMapper<R1Employee> Mapper { get; set; }

		public Employees.Config ImportConfig { get; set; }
		public Export.People.Config ExportConfig { get; set; }

		public API()
		{
			Name = "Lubrizol Import API";
			Version = "1.0";
		}

		public Result<R1Employee> GetEmployee(string id, bool internalLookup = false)
		{
			var result = Result<R1Employee>.Success();

			if (internalLookup)
			{
				using (var context = new RSMLubrizolDataModelDataContext())
				{
					var entity = context.Lubrizol_Employees.FirstOrDefault(x => x.EmployeeID == id);

					if (entity == null)
						return result.Fail(string.Format("Unable to locate Employee id ({0})", id), "NotFound");

					result.Entity = entity;
				}
			}
			else
			{
				var connectionName = ImportConfig.ConnectionString;

				if(string.IsNullOrWhiteSpace(connectionName))
					return result.Fail(string.Format("Unable to reach the database for Employee id ({0})", id), "NotFound");

				using (var context = new LubrizolDataModelDataContext(connectionName))
				{
					var entity = context.tblzILMDatas.FirstOrDefault(x => x.EmployeeID == id);

					if (entity == null)
						return result.Fail(string.Format("Unable to locate Employee id ({0})", id), "NotFound");

					var employee = new R1Employee(entity);

					result.Entity = employee;
				}
			}
			return result;
		}

		private static FieldInformation ServiceField(string displayName, string value, string internalName = null, FieldType type = FieldType.Text)
		{
			return new FieldInformation { Id = Guid.NewGuid(), DisplayName = displayName, InternalName = string.IsNullOrWhiteSpace(internalName) ? displayName : internalName, Type = type, Value = value };
		}

		private Result<Person> ExportPicture(Person entity, R1Employee employee)
		{
			var results = new Result<Person>();

			if (entity == null)
				return results.Merge(new Result<Person>(ResultType.ValidationError, "Missing Person Data"));

			if (employee == null)
				return results.Merge(new Result<Person>(ResultType.ValidationError, "Missing Employee Data"));

			if (entity.Image == null)
				return results.Merge(new Result<Person>(ResultType.ValidationError, "Missing Person Image"));

			var libraryName = (employee.EmployeeStatus.Equals('T') || employee.EmployeeStatus.Equals('R'))
			                  	? ExportConfig.InactiveEmployeeLibrary.TrimEnd('/')
			                  	: ExportConfig.ActiveEmployeeLibrary.TrimEnd('/'); 
			
			var destinationFile = string.Format("{0}/{1}/{2}.jpg", ExportConfig.Link.TrimEnd('/'), Uri.EscapeDataString(libraryName), Uri.EscapeDataString(employee.Initials.Trim()));
			var destinationUrls = new[] { destinationFile };

			#region SharePoint Fields
			//Author  Single line of text
			//Comments  Multiple lines of text Image  
			//Company  Single line of text Image  
			//Copyright  Single line of text Image  
			//Country  Single line of text Image  
			//Date Picture Taken  Date and Time Image  
			//Department  Single line of text Image  
			//DepartmentName  Single line of text Image  
			//Division  Single line of text Image  
			//EmployeeClassDesc  Single line of text Image  
			//EmployeeID  Single line of text Image  
			//EmployeeStatus  Single line of text Image  
			//EmployeeStatusDesc  Single line of text Image  
			//FirstName  Single line of text Image  
			//FullName  Single line of text Image  
			//Initials  Single line of text Image  
			//JobDescr  Single line of text Image  
			//Keywords  Multiple lines of text Image  
			//LastName  Single line of text Image  
			//LegalEntity  Single line of text Image  
			//MiddleName  Single line of text Image  
			//PhysicalLocation  Single line of text Image  
			//PhysicalLocationName  Single line of text Image  
			//Preview Image URL  Hyperlink or Picture  
			//ReportingLocation  Single line of text Image  
			//ReportingLocationName  Single line of text Image  
			//Scheduling End Date  Publishing Schedule End Date  
			//Scheduling Start Date  Publishing Schedule Start Date  
			//SupervisorID  Single line of text Image  
			//SupervisorInitials  Single line of text Image  
			//SupervisorName  Single line of text Image  
			//Created By  Person or Group   
			//Modified By  Person or Group   
			//Checked Out To  Person  
			#endregion

			FieldInformation[] metaData = 
								{ 
									ServiceField("Company", employee.Company),
									ServiceField("Country", employee.Country),
									ServiceField("Department", employee.Department),
									ServiceField("DepartmentName", employee.DepartmentName),
									ServiceField("Division", employee.Division),
									ServiceField("EmployeeClassDesc", employee.EmployeeClassDesc),
									ServiceField("EmployeeID", employee.EmployeeID),
									ServiceField("EmployeeStatus", employee.EmployeeStatus.ToString()),
									ServiceField("EmployeeStatusDesc", employee.EmployeeStatusDesc),
									ServiceField("FirstName", employee.FirstName),
									ServiceField("FullName", employee.Name),
									ServiceField("Initials", employee.Initials),
									ServiceField("JobDescr", employee.JobDescr),
									ServiceField("LastName", employee.LastName),
									ServiceField("LegalEntity", employee.LegalEntity),
									ServiceField("MiddleName", employee.MiddleName),
									ServiceField("PhysicalLocation", employee.PhysicalLocation),
									ServiceField("PhysicalLocationName", employee.PhysicalLocationName),
									ServiceField("ReportingLocation", employee.ReportingLocation),
									ServiceField("ReportingLocationName", employee.ReportingLocationName),
									ServiceField("SupervisorID", employee.SupervisorID),
									ServiceField("SupervisorInitials", employee.SupervisorInitials),
									ServiceField("SupervisorName", employee.SupervisorName),
									ServiceField("ImageCreateDate", entity.udf1)
								};

			try
			{
				var proxyWs = new Copy { Url = string.Format("{0}/_vti_bin/copy.asmx", ExportConfig.Link.TrimEnd('/')) };

				if (string.IsNullOrWhiteSpace(ExportConfig.Username) || string.IsNullOrWhiteSpace(ExportConfig.Password))
				{
					proxyWs.UseDefaultCredentials = true;
				}
				else
				{
					proxyWs.UseDefaultCredentials = false;
					proxyWs.Credentials = CreateCredentials(ExportConfig.Username, ExportConfig.Password, true);
				}

				CopyResult[] result;
				proxyWs.CopyIntoItems("http://null", destinationUrls, metaData, entity.Image, out result);

				if (result != null)
				{
					var row = 0;
					foreach (var copyResult in result.Where(copyResult => copyResult.ErrorCode != CopyErrorCode.Success))
					{
						results.Fail(copyResult.ErrorMessage, string.Format("E{0}", row));
						row++;
					}
				}

				return results;
			}
			catch (Exception ex)
			{
				var errors = new Dictionary<string, string> {{"StackTrace", ex.StackTrace}};
				return results.Merge(new Result<Person>(ResultType.TechnicalError, ex.Message, errors));
			}
		}

		public Result<Person> ExportPerson(Person entity)
		{
			var result = Result<Person>.Success();

			//Get matching employee details
			var employee = GetEmployee(entity.ExternalId, true);

			return employee.Succeeded ? ExportPicture(entity, employee.Entity) : result.Merge(employee);
		}

		public Result<UserSession> Login(string username, string password, string uri = null)
		{
			//TODO: Add login logic if needed.  This is called once at the start of each task.
			return Result<UserSession>.Success();
		}

		public void Logoff(UserSession session)
		{
			//TODO: Add logoff logic if needed.  This is called once at the end of each task.
			return;
		}

		private NetworkCredential CreateCredentials(string user, string pass, bool encryptedPassword)
		{
			if (encryptedPassword && !string.IsNullOrWhiteSpace(pass))
			{
				var crypt = new QuickAES();

				var decPass = crypt.DecryptString(pass);

				return new NetworkCredential(user, decPass);
			}

			return new NetworkCredential(user, pass);
		}
	}
}
