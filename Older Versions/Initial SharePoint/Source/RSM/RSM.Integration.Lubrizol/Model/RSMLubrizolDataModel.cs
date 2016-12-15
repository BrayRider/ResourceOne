using System.Configuration;
using RSM.Artifacts;

namespace RSM.Integration.Lubrizol.Model
{
	partial class RSMLubrizolDataModelDataContext
	{
		public RSMLubrizolDataModelDataContext()
			: base(ConfigurationManager.ConnectionStrings[Constants.ConnectionStringName].ConnectionString)
		{
		}

		partial void OnCreated()
		{
		}
	}

	public partial class Lubrizol_Employee
	{
		public Lubrizol_Employee(tblzILMData source)
		{
			Company = source.Company;
			Country = source.Country;
			Department = source.Department;
			DepartmentName = source.DepartmentName;
			Division = source.Division;
			EmployeeClassDesc = source.EmployeeClassDesc;
			EmployeeID = source.EmployeeID;
			EmployeeStatus = source.EmployeeStatus;
			EmployeeStatusDesc = source.EmployeeStatusDesc;
			FirstName = source.FirstName;
			Initials = source.Initials;
			JobDescr = source.JobDescr;
			LastLoadDate = source.LastLoadDate;
			LastName = source.LastName;
			LegalEntity = source.LegalEntity;
			MiddleName = source.MiddleName;
			Name = source.Name;
			PhysicalLocation = source.PhysicalLocation;
			PhysicalLocationName = source.PhysicalLocationName;
			ReportingLocation = source.ReportingLocation;
			ReportingLocationName = source.ReportingLocationName;
			SupervisorID = source.SupervisorID;
			SupervisorInitials = source.SupervisorInitials;
			SupervisorName = source.SupervisorName;
		}
	}
}
