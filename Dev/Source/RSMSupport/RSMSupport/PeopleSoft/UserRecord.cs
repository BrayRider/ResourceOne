using System;
using RSM.Support.IO.Csv;

namespace RSM.Support.SRMC
{
	/// <summary>
	/// Represents a PeopleSoft user record as exported for RSM.
	/// </summary>
	public class UserRecord
	{
		private const int PS_CSV_FIELD_COUNT = 11;

		const int PHY_JOB_CODE = 6000;
		const int PHY_DEPT_CODE = 6000;
		const int PHY_CSV_FIELD_COUNT = 12;
		public enum PhyCSVColumns : int
		{
			Lastname,
			Middlename,
			Firstname,
			Degrees,
			BadgeNumber,
			EmployeeID,
			Status,
			Specialty,
			JobCode,
			Role1,
			Role2
		}

		const int VOL_JOB_CODE = 5000;
		const int VOL_DEPT_CODE = 5000;
		const int VOL_CSV_FIELD_COUNT = 4;
		
		/// <summary>
		///  Identifies which if any of the key user data fields have changed in our biut mask.
		/// </summary>
		[Flags]
		public enum KeyColumnMask
		{
			DeptID = 0x01,
			JobCode = 0x02,
			BadgeNumber = 0x04,
			Facility = 0x08,
			Status = 0x10
		}

		#region Properties
		public string EmployeeID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string MiddleName { get; set; }
		public string DeptID { get; set; }
		public string DeptDescription { get; set; }
		public string JobCode { get; set; }
		public string JobDescription { get; set; }
		public string BadgeNumber { get; set; }
		public string Facility { get; set; }
		public bool Active { get; set; }
		public string Credentials { get; set; }
		public bool HasCredentials { get; set; }
		#endregion

		/// <summary>
		/// Construct a record from a CSV reader
		/// </summary>
		/// <remarks>Assume that the reader is positioned on a valid record.</remarks>
		public UserRecord(CsvReader csv, FileTypes fileType)
		{
			FromCSV(csv, fileType);
		}

		public void FromCSV(CsvReader csv, FileTypes fileType)
		{
			switch (fileType)
			{
				case FileTypes.PeopleSoft:
					FromPeopleSoftCSV(csv);
					break;
				case FileTypes.Physicians:
					FromMDCSV(csv);
					break;
				case FileTypes.Volunteers:
					FromVolCSV(csv);
					break;
			}
		}

		public void FromVolCSV(CsvReader csv)
		{
			string fullName;
			HasCredentials = false;
			if (csv.FieldCount != VOL_CSV_FIELD_COUNT)
			{
				throw new ArgumentException("CSV record has an invalid number of columns");
			}

			if (char.IsLetter(csv[(int)VolCsvColumns.EmployeeID][0]))
				EmployeeID = csv[(int)VolCsvColumns.EmployeeID];
			else
				EmployeeID = string.Format("V{0}", csv[(int)VolCsvColumns.EmployeeID]);

			fullName = csv[(int)VolCsvColumns.Name];

			FirstName = fullName.Split(',')[1].Trim();
			LastName = fullName.Split(',')[0].TrimEnd();
			MiddleName = "";
			DeptID = VOL_DEPT_CODE.ToString();
			DeptDescription = "Volunteer";
			JobCode = VOL_JOB_CODE.ToString();
			JobDescription = "Volunteer";

			if (csv[(int)VolCsvColumns.BadgeNumber].Length > 0)
			{
				BadgeNumber = csv[(int)VolCsvColumns.BadgeNumber];
			}
			else
			{
				BadgeNumber = "";
			}

			Facility = "SRMC";

			Active = (csv[(int)VolCsvColumns.Status] == "A") ? true : false;
		}

		public void FromMDCSV(CsvReader csv)
		{
			HasCredentials = true;
			if (csv.FieldCount != PHY_CSV_FIELD_COUNT)
			{
				throw new ArgumentException("CSV record has an invalid number of columns");
			}

			if (char.IsLetter(csv[(int)PhyCSVColumns.EmployeeID][0]))
				EmployeeID = csv[(int)PhyCSVColumns.EmployeeID];
			else
				EmployeeID = string.Format("M{0}", csv[(int)PhyCSVColumns.EmployeeID]);

			FirstName = csv[(int)PhyCSVColumns.Firstname];
			LastName = csv[(int)PhyCSVColumns.Lastname];
			MiddleName = csv[(int)PhyCSVColumns.Middlename];
			DeptID = PHY_DEPT_CODE.ToString();
			DeptDescription = "Physician";
			JobCode = csv[(int)PhyCSVColumns.JobCode];
			JobDescription = csv[(int)PhyCSVColumns.Specialty];
			Credentials = csv[(int)PhyCSVColumns.Degrees];

			//if (csv[(int)PhyCSVColumns.BadgeNumber].Length > 0)
			//{
			//    BadgeNumber = csv[(int)PhyCSVColumns.BadgeNumber];
			//}
			//else
			//{
			BadgeNumber = "";
			//}

			Facility = "SRMC";

			Active = csv[(int)PhyCSVColumns.Status].ToUpper().StartsWith("A");
		}

		public void FromPeopleSoftCSV(CsvReader csv)
		{
			HasCredentials = false;
			if (csv.FieldCount != PS_CSV_FIELD_COUNT && csv.FieldCount != PS_CSV_FIELD_COUNT + 1)
			{
				//throw new ArgumentException(string.Format("Peoplesoft CSV record has an invalid number of columns {0}", csv.FieldCount);
			}

			if(char.IsLetter(csv[(int)CsvColumns.EmployeeID][0]))
				EmployeeID = csv[(int)CsvColumns.EmployeeID];
			else
				EmployeeID = string.Format("E{0}", csv[(int)CsvColumns.EmployeeID]);

			FirstName = csv[(int)CsvColumns.Firstname];
			LastName = csv[(int)CsvColumns.Lastname];
			MiddleName = csv[(int)CsvColumns.Middlename];
			DeptID = csv[(int)CsvColumns.DeptID];
			DeptDescription = csv[(int)CsvColumns.DeptDesc];
			JobCode = csv[(int)CsvColumns.JobCode];
			JobDescription = csv[(int)CsvColumns.JobDesc];
			if (csv[(int)CsvColumns.BadgeNumber].Length > 0)
			{
				BadgeNumber = csv[(int)CsvColumns.BadgeNumber];
			}
			else
			{
				BadgeNumber = "";
			}

			Facility = csv[(int)CsvColumns.Facility];

			Active = (csv[(int)CsvColumns.Status] == "A");
		}
	}
}
