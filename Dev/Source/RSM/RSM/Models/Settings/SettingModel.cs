
using RSM.Artifacts;

namespace RSM.Models.Settings
{
	public class SettingModel : Support.Setting
	{
		public string FullName { get; set; }

		public string ValidationMessage { get; set; }

		public SettingModel(Support.Setting setting)
		{
			InputType = setting.InputType;
			ExternalSystem = setting.ExternalSystem;
			FullName = string.Format("{0}___{1}", setting.ExternalSystem.Name, setting.Name);
			Id = setting.Id;
			Label = setting.Label;
			Name = setting.Name;
			OrderBy = setting.OrderBy;
			SystemId = setting.SystemId;
			Value = setting.Value;
			Viewable = setting.Viewable;

			if (InputType != InputTypes.Checkbox && InputType != InputTypes.Password)
				ValidationMessage = string.Format("'{0}' is required.", Label);
		}
	}
}