using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RSM.Models;
using RSM.Service.Library.Controllers;
using RSM.Support;

namespace RSM.Controllers
{
	public class BaseController : Controller
	{
		#region Properties
		public Logger EventLogger { get; set; }

		public RSMDataModelDataContext DbContext { get; set; }

		internal ExternalSystem OwnedSystem { get; set; }

		public string OwnedSystemName { get; set; }

		public bool IsAdmin
		{
			get
			{
				return (User.IsInRole("admin"));
			}
		}
		#endregion

		#region Constructors
		public void ControllerSetup()
		{
			EventLogger = new Logger();
			DbContext = new RSMDataModelDataContext(ConfigurationManager.ConnectionStrings[Artifacts.Constants.ConnectionStringName].ConnectionString);

			OwnedSystem = DbContext.ExternalSystems.FirstOrDefault(x => x.Name == OwnedSystemName);
		}
		public BaseController()
		{
			ControllerSetup();
		}
		#endregion

		#region Membership Update
		public static void UpdateLastAccess(bool ignoreError = false)
		{
			try
			{
				var u = System.Web.Security.Membership.GetUser();

				if (u == null) return;

				u.LastActivityDate = DateTime.Now;
				System.Web.Security.Membership.UpdateUser(u);
			}
			catch (Exception ex)
			{
				if (!ignoreError)
					throw;
			}
		}
		#endregion

		#region Security
		public bool UserInRole(string role)
		{
			return User.IsInRole(role);
		}

		public bool UserInAllRoles(List<string> roles)
		{
			return roles.All(role => User.IsInRole(role));
		}

		public bool UserInAnyRoles(List<string> roles)
		{
			return roles.Any(role => User.IsInRole(role));
		}
		#endregion

		#region Sidebar Menu
		public MenuCollection BuildSidebarMenu(string menuTitle, string selected)
		{
			var associateMenuTitles = new List<string> {"Associates", "New Hires", "Terminations", "Changed"};
			var adminMenuTitles = new List<string> {"Admin", "Activity Log", "Job Codes", "Roles", "Rules", "Reports", "Settings"};

			var items = new List<MenuRoutedLink>();

			items.Add(new MenuRoutedLink { Text = "Home", Action = "Index", Controller = "Home" });
			items.Add(new MenuRoutedLink
			          	{
			          		Text = "Associates",
			          		Action = "Index",
			          		Controller = "People"
			          	});

			if (associateMenuTitles.Any(x => x.Equals(selected)))
			{
				items.Add(new MenuRoutedLink { Text = "New Hires", Action = "ReviewQueue", Controller = "People", RouteValuesCollection = new { status = "Hire" }, Indented = 2 });
				items.Add(new MenuRoutedLink { Text = "Terminations", Action = "ReviewQueue", Controller = "People", RouteValuesCollection = new { status = "Fire" }, Indented = 2 });
				items.Add(new MenuRoutedLink { Text = "Changed", Action = "ReviewQueue", Controller = "People", RouteValuesCollection = new { status = "Changed" }, Indented = 2 });
			}

			items.Add(new MenuRoutedLink {Text = "Admin", Action = "Index", Controller = "Admin"});

			if (adminMenuTitles.Any(x => x.Equals(selected)))
			{
				if (IsAdmin)
				{
					items.Add(new MenuRoutedLink {Text = "Activity Log", Action = "ActivityLog", Controller = "Admin", Indented = 2});

					var ruleAdmin = Settings.GetValueAsBool(Artifacts.Constants.R1SMSystemName, "R1SM.RuleEngineAllow");

					if (ruleAdmin)
					{
						items.Add(new MenuRoutedLink
						          	{
						          		Text = "Job Codes",
						          		Action = "JobCodes",
						          		Controller = "Admin",
						          		Indented = 2
						          	});
						items.Add(new MenuRoutedLink
						          	{
						          		Text = "Roles",
						          		Action = "Index",
						          		Controller = "Roles",
						          		Indented = 2
						          	});
						items.Add(new MenuRoutedLink
						          	{
						          		Text = "Rules",
						          		Action = "Index",
						          		Controller = "JCLRule",
						          		Indented = 2
						          	});
						items.Add(new MenuRoutedLink
						{
							Text = "Reports",
							Action = "Reports",
							Controller = "Admin",
							Indented = 2
						});
					}
					items.Add(new MenuRoutedLink
					          	{
					          		Text = "Settings",
					          		Action = "Index",
					          		Controller = "Settings",
					          		Indented = 2
					          	});
				}
			}

			var menu = new MenuCollection
			           	{
							Title = menuTitle,
							SelectedMenuText = selected,
			           		LinkCollection = items
			           	};

			return menu;
		}

		#endregion

		#region Cookies

		public int CookieValueAsInt(string settingName, int defaultValue = 0)
		{
			var httpCookie = Request.Cookies[settingName];

			if (httpCookie == null)
			{
				return defaultValue;
			}

			int value;
			return int.TryParse(httpCookie.Value, out value) ? value : defaultValue;
		}

		public void SaveCookie(string settingName, string value)
		{
			var httpCookie = Response.Cookies[settingName];

			if (httpCookie == null)
			{
				Response.Cookies.Add(new HttpCookie(settingName, value));
			}
			else
			{
				Response.Cookies[settingName].Value = value;
			}
		}
		#endregion
	}
}
