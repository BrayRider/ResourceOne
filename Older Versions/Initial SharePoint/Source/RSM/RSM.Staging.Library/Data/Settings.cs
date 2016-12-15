using RSM.Artifacts;
using RSM.Support;

namespace RSM.Staging.Library.Data
{
	public class Settings
	{
		// ExternalSystems.R1SM
		public class R1SM
		{
			public static string DefaultPrefix = "R1SM";
			public static Setting RuleEngineAllow = Factory.CreateSetting("RuleEngineAllow", "Allow the R1SM rule engine to assign roles.", "false", 0, false, InputTypes.Checkbox, ExternalSystems.R1SM);
			public static Setting JobCodesFirst = Factory.CreateSetting("JobCodesFirst", "Show job codes before job titles when editing rules.", "false", 1, false, InputTypes.Checkbox, ExternalSystems.R1SM);
			public static Setting RequireAccessApproval = Factory.CreateSetting("RequireAccessApproval", "Require approval of changes made by the rule engine", "false", 2, false, InputTypes.Checkbox, ExternalSystems.R1SM);
			public static Setting AdminPass = Factory.CreateSetting("AdminPass", "New Admin Password", "Testing", 3, true, InputTypes.Password, ExternalSystems.R1SM);
		}

		// ExternalSystems.S2In - S2Import
		public class S2Import
		{
			public static string DefaultPrefix = "S2Import";
			public static Setting Available = Factory.CreateSetting("Available", "Is the S2 system available.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.S2In);
			public static Setting Repeat = Factory.CreateSetting("Repeat", "Allow S2 import task to repeat.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.S2In);
			public static Setting RepeatInterval = Factory.CreateSetting("RepeatInterval", "S2 import repeat interval in minutes.", "3", 0, true, InputTypes.Text, ExternalSystems.S2In);
			public static Setting ServiceAddress = Factory.CreateSetting("ServiceAddress", "Appliance Address", "http://localhost:8766/RSM.Integration.S2.Stub/RestService.svc/test", 0, true, InputTypes.Text, ExternalSystems.S2In);
			public static Setting ServiceAccount = Factory.CreateSetting("ServiceAccount", "S2 Service User Id", "admin", 0, true, InputTypes.Text, ExternalSystems.S2In);
			public static Setting ServicePassword = Factory.CreateSetting("ServicePassword", "S2 Service Password", "072159245241245031239120017047219193126250124056", 0, true, InputTypes.Password, ExternalSystems.S2In);
			public static Setting PersonImport = Factory.CreateSetting("PersonImport", "Allow importing of People from S2.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.S2In);
			public static Setting LastAccessed = Factory.CreateSetting("LastAccessEvent", "Date time on last S2 record imported.", "", 0, false, InputTypes.Text, ExternalSystems.S2In);
			public static Setting S2ServiceTimeout = Factory.CreateSetting("ServiceTimeout", "Timeout in seconds of S2 web service", "6000", 5, true, InputTypes.Text, ExternalSystems.S2In);
		}

		// ExternalSystems.S2In - S2PeopleImport
		public class S2PeopleImport
		{
			public static string DefaultPrefix = "S2PeopleImport";
			public static Setting Available = Factory.CreateSetting("Available", "Is the S2 people system available.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.S2In);
			public static Setting Repeat = Factory.CreateSetting("Repeat", "Allow S2 people import task to repeat.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.S2In);
			public static Setting RepeatInterval = Factory.CreateSetting("RepeatInterval", "S2 people import repeat interval in minutes.", "3", 0, true, InputTypes.Text, ExternalSystems.S2In);
			public static Setting ServiceAddress = Factory.CreateSetting("ServiceAddress", "Appliance Address", "http://localhost:8766/RSM.Integration.S2.Stub/RestService.svc/test", 0, true, InputTypes.Text, ExternalSystems.S2In);
			public static Setting ServiceAccount = Factory.CreateSetting("ServiceAccount", "S2 Service User Id", "admin", 0, true, InputTypes.Text, ExternalSystems.S2In);
			public static Setting ServicePassword = Factory.CreateSetting("ServicePassword", "S2 Service Password", "072159245241245031239120017047219193126250124056", 0, true, InputTypes.Password, ExternalSystems.S2In);
			public static Setting ImageImport = Factory.CreateSetting("ImageImport", "Allow importing of images from S2.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.S2In);
			public static Setting FieldsToUpdate = Factory.CreateSetting("FieldsToUpdate", "List of Person fields used in R1SM updates", "FirstName,MiddleName,LastName,ExternalUpdated,Image,"
				+ "udf1,udf2,udf3,udf4,udf5,udf6,udf7,udf8,udf9,udf10,udf11,udf12,udf13,udf14,udf15,udf16,udf17,udf18,udf19,udf20", 0, true, InputTypes.Text, ExternalSystems.S2In);
		}

		// ExternalSystems.TrackOut
		public class TrackExport
		{
			public static string DefaultPrefix = "TrackExport";
			public static Setting Available = Factory.CreateSetting("Available", "Is the track system available.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.TrackOut);
			public static Setting Repeat = Factory.CreateSetting("Repeat", "Allow export task to repeat.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.TrackOut);
			public static Setting RepeatInterval = Factory.CreateSetting("RepeatInterval", "Export repeat interval in minutes.", "3", 0, true, InputTypes.Text, ExternalSystems.TrackOut);
			public static Setting ServiceAddress = Factory.CreateSetting("ServiceAddress", "Appliance Address", "http://localhost", 2, true, InputTypes.Text, ExternalSystems.TrackOut);
			public static Setting Account = Factory.CreateSetting("ServiceAccount", "Service User Id", "admin", 3, true, InputTypes.Text, ExternalSystems.TrackOut);
			public static Setting Password = Factory.CreateSetting("ServicePassword", "Service Password", "admin", 4, true, InputTypes.Password, ExternalSystems.TrackOut);
			public static Setting SourceSystem = Factory.CreateSetting("SourceSystem", "System whose data will be exported to Track.", ExternalSystems.S2In.Id.ToString(), 0, false, InputTypes.Text, ExternalSystems.TrackOut);
			public static Setting PersonExport = Factory.CreateSetting("PersonExport", "Allow export of People.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.TrackOut);
			public static Setting LocationExport = Factory.CreateSetting("LocationExport", "Allow exporting of Locations to Track", "true", 1, true, InputTypes.Checkbox, ExternalSystems.TrackOut);
			public static Setting AccessExport = Factory.CreateSetting("AccessExport", "Allow exporting of Access History to Track", "true", 2, true, InputTypes.Checkbox, ExternalSystems.TrackOut);
			public static Setting CompanyExport = Factory.CreateSetting("CompanyExport", "Allow exporting of Companies to Track", "true", 1, true, InputTypes.Checkbox, ExternalSystems.TrackOut);
			public static Setting LastAccessEvent = Factory.CreateSetting("LastAccessEvent", "Date time on last record exported.", "", 0, false, InputTypes.Text, ExternalSystems.TrackOut);
			public static Setting LocationId = Factory.CreateSetting("LocationId", "Location Id value for export to Track.", "", 0, true, InputTypes.Text, ExternalSystems.TrackOut);
			public static Setting EventCode = Factory.CreateSetting("EventCode", "Event Code value for export to Track.", "8", 0, true, InputTypes.Text, ExternalSystems.TrackOut);
			public static Setting SysId = Factory.CreateSetting("SysId", "System Id value for export to Track.", "1", 0, true, InputTypes.Text, ExternalSystems.TrackOut);
			public static Setting DataSource = Factory.CreateSetting("DataSource", "DataSource value for export to Track.", "TSTLBZDB", 0, true, InputTypes.Text, ExternalSystems.TrackOut);
			public static Setting TrackExportTimeout = Factory.CreateSetting("ServiceTimeout", "Timeout in seconds of S2 web service", "6000", 5, true, InputTypes.Text, ExternalSystems.TrackOut);
		}

		// ExternalSystems.S2Out
		public class S2Export
		{
			public static string DefaultPrefix = "S2Export";
			public static Setting PersonExport = Factory.CreateSetting("PersonExport", "Allow exporting of user data and roles to S2", "false", 0, false, InputTypes.Checkbox, ExternalSystems.S2Out);
			public static Setting Available = Factory.CreateSetting("Available", "Denotes whether the S2 export service is available to use", "true", 5, true, InputTypes.Checkbox, ExternalSystems.S2Out);
		}

		// ExternalSystems.LubrizolIn - LubrizolImport
		public class LubrizolImport
		{
			public static string DefaultPrefix = "LubrizolImport";
			public static Setting Available = Factory.CreateSetting("Available", "Is the Lubrizol system available.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.LubrizolIn);
			public static Setting Repeat = Factory.CreateSetting("Repeat", "Allow import task to repeat.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.LubrizolIn);
			public static Setting RepeatInterval = Factory.CreateSetting("RepeatInterval", "Import repeat interval in minutes.", "3", 0, true, InputTypes.Text, ExternalSystems.LubrizolIn);
			public static Setting SqlConnection = Factory.CreateSetting("SqlConnection", "Connection string to be able to access the SQL table.", "Data Source=.;Initial Catalog=LubrizolConnector;Integrated Security=True", 0, true, InputTypes.Text, ExternalSystems.LubrizolIn);
	}

		// ExternalSystems.LubrizolOut - LubrizolExport
		public class LubrizolExport
		{
			public static string DefaultPrefix = "LubrizolExport";
			public static Setting Available = Factory.CreateSetting("Available", "Is the Lubrizol export available.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.LubrizolOut);
			public static Setting Repeat = Factory.CreateSetting("Repeat", "Allow export task to repeat.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.LubrizolOut);
			public static Setting RepeatInterval = Factory.CreateSetting("RepeatInterval", "Export repeat interval in minutes.", "3", 0, true, InputTypes.Text, ExternalSystems.LubrizolOut);
			public static Setting ServiceAddress = Factory.CreateSetting("ServiceAddress", "Service Address", "http://cencle06:10807/", 2, true, InputTypes.Text, ExternalSystems.LubrizolOut);
			public static Setting Account = Factory.CreateSetting("ServiceAccount", "Service User Id", "", 3, true, InputTypes.Text, ExternalSystems.LubrizolOut);
			public static Setting Password = Factory.CreateSetting("ServicePassword", "Service Password", "", 4, true, InputTypes.Password, ExternalSystems.LubrizolOut);
			public static Setting LastUpdated = Factory.CreateSetting("LastUpdated", "Last updated timestamp on last person record exported.", "01/01/2013", 0, false, InputTypes.Text, ExternalSystems.LubrizolOut);
			public static Setting SourceSystem = Factory.CreateSetting("SourceSystem", "System whose data will be exported.", ExternalSystems.S2In.Id.ToString(), 0, false, InputTypes.Text, ExternalSystems.LubrizolOut);
			public static Setting ActiveEmployeeLibrary = Factory.CreateSetting("ActiveEmployeeLibrary", "Active Employees Library Url", "Active Employees", 7, true, InputTypes.Text, ExternalSystems.LubrizolOut);
			public static Setting InactiveEmployeeLibrary = Factory.CreateSetting("InactiveEmployeeLibrary", "Inactive Employees Library Url", "TermRetire Employees", 7, true, InputTypes.Text, ExternalSystems.LubrizolOut);
			public static Setting SqlConnection = Factory.CreateSetting("SqlConnection", "Connection string to be able to access the SQL table.", "Data Source=.;Initial Catalog=LubrizolConnector;Integrated Security=True", 0, true, InputTypes.Text, ExternalSystems.LubrizolOut);
		}
	}
}

