using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RSM.Artifacts;
using RSM.Models.Settings;
using RSM.Support;

namespace RSM.Controllers
{
	public class SettingsController : BaseController
	{
		[Authorize(Roles = "admin")]
		public ActionResult Index()
		{
			var serviceController = new Service.Library.Controllers.Settings();

			var results = serviceController.Search(o => o.Viewable);

			if(results.Failed || results.Entity == null || !results.Entity.Any())
				throw new ApplicationException(results.Message);

			var model = new SettingsViewModel();

			model.SidebarMenu = BuildSidebarMenu("Admin", "Settings");

			var settings = results.Entity;
			var groupings = settings.Select(x => x.ExternalSystem).Distinct().ToList();
			var groupCollection = new GroupingCollection { Groupings = new List<Grouping>(), R1SMGroup = null };

			if (settings.Any(x => x.ExternalSystem.Name == Constants.R1SMSystemName && x.Viewable))
			{
				var r1SMGroup = new Grouping
				                	{
				                		Name = Constants.R1SMSystemName,
				                		Label = "The settings below control how the Resource One Security Manager operates.",
				                		SettingCollection =
				                			settings.Where(x => x.ExternalSystem.Name == Constants.R1SMSystemName).Select(
				                				x => new SettingModel(x)).ToList()
				                	};

				groupCollection.R1SMGroup = r1SMGroup;
			}


			foreach (var grouping in groupings.Where(x => x.Name != Constants.R1SMSystemName).OrderBy(x => x.Name))
			{
				if (!settings.Any(x => x.SystemId == grouping.Id && x.Viewable)) continue;

				var group = new Grouping
				            	{
				            		Name = grouping.Name,
				            		Label = string.Format(
				            			"The settings below control how the Resource One Security Manager interfaces with {0} for {1} data.",
				            			grouping.Name, grouping.DirectionLabel.ToLower()),
				            		SettingCollection =
				            			settings.Where(x => x.SystemId == grouping.Id).Select(x => new SettingModel(x)).ToList()
				            	};

				groupCollection.Groupings.Add(group);
			}

			model.GroupingCollection = groupCollection;

			return View(model);
		}

		[HttpPost, ValidateInput(false)]
		[Authorize(Roles = "admin")]
		public ActionResult Index(FormCollection collection)
		{
			var serviceController = new Service.Library.Controllers.Settings();

			var results = serviceController.Search(o => o.Viewable);

			if (results.Failed || results.Entity == null || !results.Entity.Any())
				throw new ApplicationException(results.Message);

			foreach (var setting in results.Entity.Select(x => new SettingModel(x)))
			{
				if (!collection.AllKeys.Contains(setting.FullName)) continue;

				var value = collection[setting.FullName];

				if(string.IsNullOrWhiteSpace(value))
				{
					if (setting.InputType != InputTypes.Checkbox && setting.InputType != InputTypes.Password)
						ModelState.AddModelError(setting.FullName, setting.ValidationMessage);
	
					continue;
				}
				else
				{
					if (setting.InputType == InputTypes.Password)
					{
						var crypt = new QuickAES();

						value = crypt.EncryptToString(value);
					}
				}
				
				if (setting.InputType == InputTypes.Checkbox)
					value = collection.Get(setting.FullName).ToLower().Contains("t") ? "true" : "false";

				if (!setting.Value.Equals(value))
					serviceController.Set(setting.Id, value);
			}

			if (ModelState.IsValid)
				return RedirectToAction("Index");
			
			return Index();
		}
	}
}
