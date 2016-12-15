using RSM.Artifacts;
using RSM.Support;

namespace RSM.Staging.Library.Data
{
	public class Settings
	{
		// ExternalSystems.R1SM
		public static Setting R1SMRuleEngineAllow = Factory.CreateSetting(1, "R1SM.RuleEngineAllow", "Allow the R1SM rule engine to assign roles.", "false", 0, true, InputTypes.Checkbox, ExternalSystems.R1SM);
		public static Setting R1SMJobCodesFirst = Factory.CreateSetting(2, "R1SM.JobCodesFirst", "Show job codes before job titles when editing rules.", "false", 1, true, InputTypes.Checkbox, ExternalSystems.R1SM);
		public static Setting R1SMRequireAccessApproval = Factory.CreateSetting(3, "R1SM.RequireAccessApproval", "Require approval of changes made by the rule engine", "false", 2, false, InputTypes.Checkbox, ExternalSystems.R1SM);
		public static Setting R1SMAdminPass = Factory.CreateSetting(4, "R1SM.AdminPass", "New Admin Password", "Testing", 3, true, InputTypes.Password, ExternalSystems.R1SM);

		// ExternalSystems.S2In
		public static Setting S2LevelImport = Factory.CreateSetting(5, "S2Import.LevelImport", "Allow importing of levels from S2.", "false", 1, true, InputTypes.Text, ExternalSystems.S2In);
		public static Setting S2ServiceAddress = Factory.CreateSetting(6, "S2Import.ServiceAddress", "Appliance Address", "http://localhost:8766/RSM.Integration.S2.Stub/RestService.svc/test", 2, true, InputTypes.Text, ExternalSystems.S2In);
		public static Setting S2PersonImport = Factory.CreateSetting(7, "S2Import.PersonImport", "Allow importing of People from S2.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.S2In);
		public static Setting S2ServiceAccount = Factory.CreateSetting(11, "S2Import.ServiceAccount", "S2 Service User Id", "admin", 3, true, InputTypes.Text, ExternalSystems.S2In);
		public static Setting S2ServicePassword = Factory.CreateSetting(12, "S2Import.ServicePassword", "S2 Service Password", "072159245241245031239120017047219193126250124056", 4, true, InputTypes.Password, ExternalSystems.S2In);
		public static Setting S2ServiceTimeout = Factory.CreateSetting(31, "S2Import.ServiceTimeout", "Timeout in seconds of S2 web service", "6000", 5, true, InputTypes.Text, ExternalSystems.S2In);
		public static Setting S2ImportRepeat = Factory.CreateSetting(14, "S2Import.Repeat", "Allow S2 import task to repeat.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.S2In);
		public static Setting S2ImportRepeatInterval = Factory.CreateSetting(15, "S2Import.RepeatInterval", "S2 import repeat interval in minutes.", "3", 0, true, InputTypes.Text, ExternalSystems.S2In);
		public static Setting S2ImportLastAccessed = Factory.CreateSetting(16, "S2Import.LastAccessEvent", "Date time of last access record imported from S2.", "", 0, false, InputTypes.Text, ExternalSystems.S2In);
		public static Setting S2InAvailable = Factory.CreateSetting(28, "S2Import.Available", "Denotes whether the S2 import service is available to use", "true", 5, true, InputTypes.Checkbox, ExternalSystems.S2In);

		// ExternalSystems.TrackOut
		public static Setting TrackPersonExport = Factory.CreateSetting(8, "TrackExport.PersonExport", "Allow exporting of People to Track", "true", 0, true, InputTypes.Checkbox, ExternalSystems.TrackOut);
		public static Setting TrackLocationExport = Factory.CreateSetting(9, "TrackExport.LocationExport", "Allow exporting of Locations to Track", "true", 1, true, InputTypes.Checkbox, ExternalSystems.TrackOut);
		public static Setting TrackAccessExport = Factory.CreateSetting(10, "TrackExport.AccessExport", "Allow exporting of Access History to Track", "true", 2, true, InputTypes.Checkbox, ExternalSystems.TrackOut);
		public static Setting TrackExportServiceAddress = Factory.CreateSetting(17, "TrackExport.ServiceAddress", "Appliance Address", "http://localhost", 2, true, InputTypes.Text, ExternalSystems.TrackOut);
		public static Setting TrackExportAccount = Factory.CreateSetting(18, "TrackExport.ServiceAccount", "Track Service User Id", "asdfasasdfasd", 3, true, InputTypes.Text, ExternalSystems.TrackOut);
		public static Setting TrackExportPassword = Factory.CreateSetting(19, "TrackExport.ServicePassword", "Track Service Password", "admin", 4, true, InputTypes.Password, ExternalSystems.TrackOut);
		public static Setting TrackExportTimeout = Factory.CreateSetting(32, "TrackExport.ServiceTimeout", "Timeout in seconds of S2 web service", "6000", 5, true, InputTypes.Text, ExternalSystems.TrackOut);
		public static Setting TrackExportLastAccessEvent = Factory.CreateSetting(20, "TrackExport.LastAccessEvent", "Date time of last access record exported to Track.", "", 0, false, InputTypes.Text, ExternalSystems.TrackOut);
		public static Setting TrackExportSourceSystem = Factory.CreateSetting(21, "TrackExport.SourceSystem", "System whose data will be exported to Track.", "", 0, true, InputTypes.Text, ExternalSystems.TrackOut);
		public static Setting TrackExportRepeat = Factory.CreateSetting(22, "TrackExport.Repeat", "Allow Track export task to repeat.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.TrackOut);
		public static Setting TrackExportRepeatInterval = Factory.CreateSetting(23, "TrackExport.RepeatInterval", "Track export repeat interval in minutes.", "3", 0, true, InputTypes.Text, ExternalSystems.TrackOut);
		public static Setting TrackExportLocationId = Factory.CreateSetting(24, "TrackExport.LocationId", "Location Id value for export to Track.", "", 0, true, InputTypes.Text, ExternalSystems.TrackOut);
		public static Setting TrackExportEventCode = Factory.CreateSetting(25, "TrackExport.EventCode", "Event Code value for export to Track.", "", 0, true, InputTypes.Text, ExternalSystems.TrackOut);
		public static Setting TrackExportSysId = Factory.CreateSetting(26, "TrackExport.SysId", "System Id value for export to Track.", "", 0, true, InputTypes.Text, ExternalSystems.TrackOut);
		public static Setting TrackExportDataSource = Factory.CreateSetting(27, "TrackExport.DataSource", "DataSource value for export to Track.", "", 0, true, InputTypes.Text, ExternalSystems.TrackOut);
		public static Setting TrackAvailable = Factory.CreateSetting(29, "Track.Available", "Denotes whether the Track export service is available to use", "true", 0, true, InputTypes.Checkbox, ExternalSystems.TrackOut);

		// ExternalSystems.S2Out
		public static Setting S2PersonExport = Factory.CreateSetting(13, "S2Export.PersonExport", "Allow exporting of user data and roles to S2", "false", 0, false, InputTypes.Checkbox, ExternalSystems.S2Out);
		public static Setting S2OutAvailable = Factory.CreateSetting(30, "S2Export.Available", "Denotes whether the S2 export service is available to use", "true", 5, true, InputTypes.Checkbox, ExternalSystems.S2Out);

	}
}
