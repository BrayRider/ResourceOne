using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSM.Integration.Lubrizol.Model;
using LzEmployee = RSM.Integration.Lubrizol.Model.tblzILMData;
using R1Employee = RSM.Integration.Lubrizol.Model.Lubrizol_Employee;

namespace RSM.Integration.Lubrizol.Extensions
{
	public static class DataExtensions
	{
		public static R1Employee Select(this R1Employee entity, RSMLubrizolDataModelDataContext context)
		{
			return context.Lubrizol_Employees.FirstOrDefault(x => x.EmployeeID == entity.EmployeeID);
		}
		public static R1Employee Insert(this R1Employee entity, RSMLubrizolDataModelDataContext context)
		{
			context.Lubrizol_Employees.InsertOnSubmit(entity);

			context.SubmitChanges();

			return entity;
		}
	}
}
