using System;

namespace RSM.Support.Interfaces
{
	public interface IPerson
	{
		int PersonID { get; set; }
		string FirstName { get; set; }
		string LastName { get; set; }
		string MiddleName { get; set; }
		string DeptDescr { get; set; }
		string JobDescr { get; set; }
		string Facility { get; set; }
		bool Active { get; set; }
		string BadgeNumber { get; set; }
		DateTime LastUpdated { get; set; }
		int LastUpdateMask { get; set; }
	}
}