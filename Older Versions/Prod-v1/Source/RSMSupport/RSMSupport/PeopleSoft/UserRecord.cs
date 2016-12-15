using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RSM.Support.IO.Csv;


namespace RSM.Support.SRMC
{
    /// <summary>
	/// Represents a PeopleSoft user record as exported for RSM.
	/// </summary>
	public class UserRecord
    {
        const int PS_CSV_FIELD_COUNT = 11;
        enum PSCSVColumns : int 
        {
            EmployeeID,
            Firstname,
            Lastname,
            Middlename,
            DeptID,
            DeptDesc,
            JobCode,
            JobDesc,
            BadgeNumber,
            Facility,
            Status
        }

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
        enum VolCSVColumns : int
        {
            Name,
            BadgeNumber,
            EmployeeID,
            Status
        }

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

        /// <summary>
	    /// Construct a record from a CSV reader
	    /// </summary>
	    /// <remarks>Assume that the reader is positioned on a valid record.</remarks>
        public UserRecord(CsvReader csv, SRMCImporter.fileTypes fileType)
        {
            FromCSV(csv, fileType);
        }


        public void FromCSV(CsvReader csv, SRMCImporter.fileTypes fileType)
        {
            switch (fileType)
            {
                case SRMCImporter.fileTypes.FT_PEOPLESOFT:
                    FromPeopleSoftCSV(csv);
                    break;
                case SRMCImporter.fileTypes.FT_PHYSICIANS:
                    FromMDCSV(csv);
                    break;
                case SRMCImporter.fileTypes.FT_VOLUNTEERS:
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

            if (char.IsLetter(csv[(int)VolCSVColumns.EmployeeID][0]))
                EmployeeID = csv[(int)VolCSVColumns.EmployeeID];
            else
                EmployeeID = string.Format("V{0}", csv[(int)VolCSVColumns.EmployeeID]);

            fullName = csv[(int)VolCSVColumns.Name];

            FirstName = fullName.Split(',')[1].Trim();
            LastName = fullName.Split(',')[0].TrimEnd();
            MiddleName = "";
            DeptID = VOL_DEPT_CODE.ToString();
            DeptDescription = "Volunteer";
            JobCode = VOL_JOB_CODE.ToString();
            JobDescription = "Volunteer";
            if (csv[(int)VolCSVColumns.BadgeNumber].Length > 0)
            {
                BadgeNumber = csv[(int)VolCSVColumns.BadgeNumber];
            }
            else
            {
                BadgeNumber = "";
            }

            Facility = "SRMC";

            Active = (csv[(int)VolCSVColumns.Status] == "A") ? true : false;

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

            if(char.IsLetter(csv[(int)PSCSVColumns.EmployeeID][0]))
                EmployeeID = csv[(int)PSCSVColumns.EmployeeID];
            else
                EmployeeID = string.Format("E{0}", csv[(int)PSCSVColumns.EmployeeID]);

            FirstName = csv[(int)PSCSVColumns.Firstname];
            LastName = csv[(int)PSCSVColumns.Lastname];
            MiddleName = csv[(int)PSCSVColumns.Middlename];
            DeptID = csv[(int)PSCSVColumns.DeptID];
            DeptDescription = csv[(int)PSCSVColumns.DeptDesc];
            JobCode = csv[(int)PSCSVColumns.JobCode];
            JobDescription = csv[(int)PSCSVColumns.JobDesc];
            if (csv[(int)PSCSVColumns.BadgeNumber].Length > 0)
            {
                BadgeNumber = csv[(int)PSCSVColumns.BadgeNumber];
            }
            else
            {
                BadgeNumber = "";
            }
                
            Facility = csv[(int)PSCSVColumns.Facility];

            Active = (csv[(int)PSCSVColumns.Status] == "A") ? true : false;

        }


    }
}
