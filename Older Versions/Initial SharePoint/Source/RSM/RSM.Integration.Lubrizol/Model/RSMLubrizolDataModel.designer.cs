﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.296
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RSM.Integration.Lubrizol.Model
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="RSM_SharePoint")]
	public partial class RSMLubrizolDataModelDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertLubrizol_Employee(Lubrizol_Employee instance);
    partial void UpdateLubrizol_Employee(Lubrizol_Employee instance);
    partial void DeleteLubrizol_Employee(Lubrizol_Employee instance);
    #endregion
		
		public RSMLubrizolDataModelDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public RSMLubrizolDataModelDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public RSMLubrizolDataModelDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public RSMLubrizolDataModelDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Lubrizol_Employee> Lubrizol_Employees
		{
			get
			{
				return this.GetTable<Lubrizol_Employee>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Lubrizol_Employees")]
	public partial class Lubrizol_Employee : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _EmployeeID;
		
		private string _Initials;
		
		private string _Name;
		
		private string _ReportingLocation;
		
		private string _ReportingLocationName;
		
		private string _PhysicalLocation;
		
		private string _PhysicalLocationName;
		
		private System.Nullable<char> _EmployeeStatus;
		
		private string _EmployeeStatusDesc;
		
		private string _Department;
		
		private string _DepartmentName;
		
		private string _EmployeeClassDesc;
		
		private string _Division;
		
		private string _LegalEntity;
		
		private string _Company;
		
		private string _JobDescr;
		
		private string _SupervisorID;
		
		private string _SupervisorInitials;
		
		private string _SupervisorName;
		
		private string _FirstName;
		
		private string _MiddleName;
		
		private string _LastName;
		
		private string _Country;
		
		private System.Nullable<System.DateTime> _LastLoadDate;
		
		private System.DateTime _LastUpdated;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnEmployeeIDChanging(string value);
    partial void OnEmployeeIDChanged();
    partial void OnInitialsChanging(string value);
    partial void OnInitialsChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnReportingLocationChanging(string value);
    partial void OnReportingLocationChanged();
    partial void OnReportingLocationNameChanging(string value);
    partial void OnReportingLocationNameChanged();
    partial void OnPhysicalLocationChanging(string value);
    partial void OnPhysicalLocationChanged();
    partial void OnPhysicalLocationNameChanging(string value);
    partial void OnPhysicalLocationNameChanged();
    partial void OnEmployeeStatusChanging(System.Nullable<char> value);
    partial void OnEmployeeStatusChanged();
    partial void OnEmployeeStatusDescChanging(string value);
    partial void OnEmployeeStatusDescChanged();
    partial void OnDepartmentChanging(string value);
    partial void OnDepartmentChanged();
    partial void OnDepartmentNameChanging(string value);
    partial void OnDepartmentNameChanged();
    partial void OnEmployeeClassDescChanging(string value);
    partial void OnEmployeeClassDescChanged();
    partial void OnDivisionChanging(string value);
    partial void OnDivisionChanged();
    partial void OnLegalEntityChanging(string value);
    partial void OnLegalEntityChanged();
    partial void OnCompanyChanging(string value);
    partial void OnCompanyChanged();
    partial void OnJobDescrChanging(string value);
    partial void OnJobDescrChanged();
    partial void OnSupervisorIDChanging(string value);
    partial void OnSupervisorIDChanged();
    partial void OnSupervisorInitialsChanging(string value);
    partial void OnSupervisorInitialsChanged();
    partial void OnSupervisorNameChanging(string value);
    partial void OnSupervisorNameChanged();
    partial void OnFirstNameChanging(string value);
    partial void OnFirstNameChanged();
    partial void OnMiddleNameChanging(string value);
    partial void OnMiddleNameChanged();
    partial void OnLastNameChanging(string value);
    partial void OnLastNameChanged();
    partial void OnCountryChanging(string value);
    partial void OnCountryChanged();
    partial void OnLastLoadDateChanging(System.Nullable<System.DateTime> value);
    partial void OnLastLoadDateChanged();
    partial void OnLastUpdatedChanging(System.DateTime value);
    partial void OnLastUpdatedChanged();
    #endregion
		
		public Lubrizol_Employee()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EmployeeID", DbType="NChar(8) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string EmployeeID
		{
			get
			{
				return this._EmployeeID;
			}
			set
			{
				if ((this._EmployeeID != value))
				{
					this.OnEmployeeIDChanging(value);
					this.SendPropertyChanging();
					this._EmployeeID = value;
					this.SendPropertyChanged("EmployeeID");
					this.OnEmployeeIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Initials", DbType="NVarChar(30) NOT NULL", CanBeNull=false)]
		public string Initials
		{
			get
			{
				return this._Initials;
			}
			set
			{
				if ((this._Initials != value))
				{
					this.OnInitialsChanging(value);
					this.SendPropertyChanging();
					this._Initials = value;
					this.SendPropertyChanged("Initials");
					this.OnInitialsChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ReportingLocation", DbType="NVarChar(4)")]
		public string ReportingLocation
		{
			get
			{
				return this._ReportingLocation;
			}
			set
			{
				if ((this._ReportingLocation != value))
				{
					this.OnReportingLocationChanging(value);
					this.SendPropertyChanging();
					this._ReportingLocation = value;
					this.SendPropertyChanged("ReportingLocation");
					this.OnReportingLocationChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ReportingLocationName", DbType="NVarChar(30)")]
		public string ReportingLocationName
		{
			get
			{
				return this._ReportingLocationName;
			}
			set
			{
				if ((this._ReportingLocationName != value))
				{
					this.OnReportingLocationNameChanging(value);
					this.SendPropertyChanging();
					this._ReportingLocationName = value;
					this.SendPropertyChanged("ReportingLocationName");
					this.OnReportingLocationNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PhysicalLocation", DbType="NVarChar(4)")]
		public string PhysicalLocation
		{
			get
			{
				return this._PhysicalLocation;
			}
			set
			{
				if ((this._PhysicalLocation != value))
				{
					this.OnPhysicalLocationChanging(value);
					this.SendPropertyChanging();
					this._PhysicalLocation = value;
					this.SendPropertyChanged("PhysicalLocation");
					this.OnPhysicalLocationChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PhysicalLocationName", DbType="NVarChar(30)")]
		public string PhysicalLocationName
		{
			get
			{
				return this._PhysicalLocationName;
			}
			set
			{
				if ((this._PhysicalLocationName != value))
				{
					this.OnPhysicalLocationNameChanging(value);
					this.SendPropertyChanging();
					this._PhysicalLocationName = value;
					this.SendPropertyChanged("PhysicalLocationName");
					this.OnPhysicalLocationNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EmployeeStatus", DbType="NChar(1)")]
		public System.Nullable<char> EmployeeStatus
		{
			get
			{
				return this._EmployeeStatus;
			}
			set
			{
				if ((this._EmployeeStatus != value))
				{
					this.OnEmployeeStatusChanging(value);
					this.SendPropertyChanging();
					this._EmployeeStatus = value;
					this.SendPropertyChanged("EmployeeStatus");
					this.OnEmployeeStatusChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EmployeeStatusDesc", DbType="NVarChar(40)")]
		public string EmployeeStatusDesc
		{
			get
			{
				return this._EmployeeStatusDesc;
			}
			set
			{
				if ((this._EmployeeStatusDesc != value))
				{
					this.OnEmployeeStatusDescChanging(value);
					this.SendPropertyChanging();
					this._EmployeeStatusDesc = value;
					this.SendPropertyChanged("EmployeeStatusDesc");
					this.OnEmployeeStatusDescChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Department", DbType="NVarChar(10)")]
		public string Department
		{
			get
			{
				return this._Department;
			}
			set
			{
				if ((this._Department != value))
				{
					this.OnDepartmentChanging(value);
					this.SendPropertyChanging();
					this._Department = value;
					this.SendPropertyChanged("Department");
					this.OnDepartmentChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DepartmentName", DbType="NVarChar(40)")]
		public string DepartmentName
		{
			get
			{
				return this._DepartmentName;
			}
			set
			{
				if ((this._DepartmentName != value))
				{
					this.OnDepartmentNameChanging(value);
					this.SendPropertyChanging();
					this._DepartmentName = value;
					this.SendPropertyChanged("DepartmentName");
					this.OnDepartmentNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EmployeeClassDesc", DbType="NVarChar(20)")]
		public string EmployeeClassDesc
		{
			get
			{
				return this._EmployeeClassDesc;
			}
			set
			{
				if ((this._EmployeeClassDesc != value))
				{
					this.OnEmployeeClassDescChanging(value);
					this.SendPropertyChanging();
					this._EmployeeClassDesc = value;
					this.SendPropertyChanged("EmployeeClassDesc");
					this.OnEmployeeClassDescChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Division", DbType="NVarChar(40)")]
		public string Division
		{
			get
			{
				return this._Division;
			}
			set
			{
				if ((this._Division != value))
				{
					this.OnDivisionChanging(value);
					this.SendPropertyChanging();
					this._Division = value;
					this.SendPropertyChanged("Division");
					this.OnDivisionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LegalEntity", DbType="NVarChar(4)")]
		public string LegalEntity
		{
			get
			{
				return this._LegalEntity;
			}
			set
			{
				if ((this._LegalEntity != value))
				{
					this.OnLegalEntityChanging(value);
					this.SendPropertyChanging();
					this._LegalEntity = value;
					this.SendPropertyChanged("LegalEntity");
					this.OnLegalEntityChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Company", DbType="NVarChar(30)")]
		public string Company
		{
			get
			{
				return this._Company;
			}
			set
			{
				if ((this._Company != value))
				{
					this.OnCompanyChanging(value);
					this.SendPropertyChanging();
					this._Company = value;
					this.SendPropertyChanged("Company");
					this.OnCompanyChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_JobDescr", DbType="NVarChar(40)")]
		public string JobDescr
		{
			get
			{
				return this._JobDescr;
			}
			set
			{
				if ((this._JobDescr != value))
				{
					this.OnJobDescrChanging(value);
					this.SendPropertyChanging();
					this._JobDescr = value;
					this.SendPropertyChanged("JobDescr");
					this.OnJobDescrChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SupervisorID", DbType="NChar(8)")]
		public string SupervisorID
		{
			get
			{
				return this._SupervisorID;
			}
			set
			{
				if ((this._SupervisorID != value))
				{
					this.OnSupervisorIDChanging(value);
					this.SendPropertyChanging();
					this._SupervisorID = value;
					this.SendPropertyChanged("SupervisorID");
					this.OnSupervisorIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SupervisorInitials", DbType="NVarChar(30)")]
		public string SupervisorInitials
		{
			get
			{
				return this._SupervisorInitials;
			}
			set
			{
				if ((this._SupervisorInitials != value))
				{
					this.OnSupervisorInitialsChanging(value);
					this.SendPropertyChanging();
					this._SupervisorInitials = value;
					this.SendPropertyChanged("SupervisorInitials");
					this.OnSupervisorInitialsChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SupervisorName", DbType="NVarChar(50)")]
		public string SupervisorName
		{
			get
			{
				return this._SupervisorName;
			}
			set
			{
				if ((this._SupervisorName != value))
				{
					this.OnSupervisorNameChanging(value);
					this.SendPropertyChanging();
					this._SupervisorName = value;
					this.SendPropertyChanged("SupervisorName");
					this.OnSupervisorNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FirstName", DbType="NVarChar(40)")]
		public string FirstName
		{
			get
			{
				return this._FirstName;
			}
			set
			{
				if ((this._FirstName != value))
				{
					this.OnFirstNameChanging(value);
					this.SendPropertyChanging();
					this._FirstName = value;
					this.SendPropertyChanged("FirstName");
					this.OnFirstNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_MiddleName", DbType="NVarChar(40)")]
		public string MiddleName
		{
			get
			{
				return this._MiddleName;
			}
			set
			{
				if ((this._MiddleName != value))
				{
					this.OnMiddleNameChanging(value);
					this.SendPropertyChanging();
					this._MiddleName = value;
					this.SendPropertyChanged("MiddleName");
					this.OnMiddleNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastName", DbType="NVarChar(40)")]
		public string LastName
		{
			get
			{
				return this._LastName;
			}
			set
			{
				if ((this._LastName != value))
				{
					this.OnLastNameChanging(value);
					this.SendPropertyChanging();
					this._LastName = value;
					this.SendPropertyChanged("LastName");
					this.OnLastNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Country", DbType="NVarChar(40)")]
		public string Country
		{
			get
			{
				return this._Country;
			}
			set
			{
				if ((this._Country != value))
				{
					this.OnCountryChanging(value);
					this.SendPropertyChanging();
					this._Country = value;
					this.SendPropertyChanged("Country");
					this.OnCountryChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastLoadDate", DbType="DateTime")]
		public System.Nullable<System.DateTime> LastLoadDate
		{
			get
			{
				return this._LastLoadDate;
			}
			set
			{
				if ((this._LastLoadDate != value))
				{
					this.OnLastLoadDateChanging(value);
					this.SendPropertyChanging();
					this._LastLoadDate = value;
					this.SendPropertyChanged("LastLoadDate");
					this.OnLastLoadDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastUpdated", DbType="DateTime NOT NULL")]
		public System.DateTime LastUpdated
		{
			get
			{
				return this._LastUpdated;
			}
			set
			{
				if ((this._LastUpdated != value))
				{
					this.OnLastUpdatedChanging(value);
					this.SendPropertyChanging();
					this._LastUpdated = value;
					this.SendPropertyChanged("LastUpdated");
					this.OnLastUpdatedChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
