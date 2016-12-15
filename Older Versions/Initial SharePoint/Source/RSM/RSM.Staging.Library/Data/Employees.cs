using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSMDB = RSM.Integration.Lubrizol.Model;

namespace RSM.Staging.Library.Data
{
	public class Employees
	{
		//R1 Employees
		public static RSMDB.tblzILMData R1Employee1 = Factory.CreateEmployee(
			company: "Lubrizol",
			country: "USA",
			department: "US1912340",
			departmentName: "Human Resources",
			division: "TST_DIV",
			employeeClassDesc: "Technician",
			employeeId: "10019876",
			employeeStatus: 'A',
			employeeStatusDesc: "Active",
			firstName: "Joe",
			initials: "JKS",
			jobDescr: "Payroll Specialist",
			lastLoadDate: DateTime.Now,
			lastName: "Smith",
			legalEntity: "USA",
			middleName: "KnowIt",
			name: "Joe Smith",
			physicalLocation: "P029",
			physicalLocationName: "Austin, TX",
			reportingLocation: "P029",
			reportingLocationName: "Austin, TX",
			supervisorId: "1114566",
			supervisorInitials: "NAP",
			supervisorName: "Not A Person");

		public static RSMDB.tblzILMData R1Employee2 = Factory.CreateEmployee(
			company: "Lubrizol",
			country: "USA",
			department: "US1912342",
			departmentName: "Human Resources",
			division: "TST_DIV2",
			employeeClassDesc: "Tester",
			employeeId: "10055576",
			employeeStatus: 'A',
			employeeStatusDesc: "Active",
			firstName: "Kelly",
			initials: "KHH",
			jobDescr: "Test Specialist",
			lastLoadDate: DateTime.Now,
			lastName: "Hope",
			legalEntity: "USA",
			middleName: "Has",
			name: "Kelly Has Hope",
			physicalLocation: "P029",
			physicalLocationName: "Austin, TX",
			reportingLocation: "P029",
			reportingLocationName: "Austin, TX",
			supervisorId: "1114566",
			supervisorInitials: "NAP",
			supervisorName: "Not A Person");
	}
}
