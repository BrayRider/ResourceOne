using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RSM.Models;
using RSM.Service.Library.Controllers;

namespace RSM.Controllers
{
	public class HomeController : BaseController
	{
		[Authorize]
		public ActionResult Index()
		{
			var model = new Models.Home.LandingPageModel
							{
								IsAdmin = IsAdmin,
								SidebarMenu = BuildSidebarMenu("Menu", "Home"),
								PageTitle = "Resource One Security Manager",
								BoxedLinkCollections = new List<MenuCollection>()
							};

			var personMenu = new MenuCollection
			                 	{
									Title = "Associates",
			                 		LinkCollection = new List<MenuRoutedLink>
			                 		                 	{
			                 		                 		new MenuRoutedLink
			                 		                 			{
			                 		                 				Text = "All Associates",
			                 		                 				Action = "Index",
			                 		                 				Controller = "People"
			                 		                 			},
			                 		                 		new MenuRoutedLink
			                 		                 			{
			                 		                 				Text = "New Hires",
			                 		                 				Action = "ReviewQueue",
			                 		                 				Controller = "People",
			                 		                 				RouteValuesCollection = new {status = "Hire"},
			                 		                 				Indented = 2
			                 		                 			},
			                 		                 		new MenuRoutedLink
			                 		                 			{
			                 		                 				Text = "Terminations",
			                 		                 				Action = "ReviewQueue",
			                 		                 				Controller = "People",
			                 		                 				RouteValuesCollection = new {status = "Fire"},
			                 		                 				Indented = 2
			                 		                 			},
			                 		                 		new MenuRoutedLink
			                 		                 			{
			                 		                 				Text = "Changed",
			                 		                 				Action = "ReviewQueue",
			                 		                 				Controller = "People",
			                 		                 				RouteValuesCollection = new {status = "Changed"},
			                 		                 				Indented = 2
			                 		                 			}
			                 		                 	}
			                 	};
			model.BoxedLinkCollections.Add(personMenu);

			if(IsAdmin)
			{
				var ruleAdmin = Settings.GetValueAsBool(Artifacts.Constants.R1SMSystemName, "R1SM.RuleEngineAllow");
				var adminMenu = new MenuCollection
				                	{
										Title = "Admin",
										LinkCollection = new List<MenuRoutedLink>
				                		                 	{
				                		                 		new MenuRoutedLink
				                		                 			{
				                		                 				Text = "Activity Log",
				                		                 				Action = "ActivityLog",
				                		                 				Controller = "Admin",
				                		                 				Indented = 2
				                		                 			}
				                		                 	}
				                	};

				if(ruleAdmin)
				{
					adminMenu.LinkCollection.Add(new MenuRoutedLink
													{
														Text = "Job Codes",
														Action = "JobCodes",
														Controller = "Admin",
														Indented = 2
													});
					adminMenu.LinkCollection.Add(new MenuRoutedLink
													{
														Text = "Roles",
														Action = "Index",
														Controller = "Roles",
														Indented = 2
													});
					adminMenu.LinkCollection.Add(new MenuRoutedLink
													{
														Text = "Rules",
														Action = "Index",
														Controller = "JCLRule",
														Indented = 2
													});
					adminMenu.LinkCollection.Add(new MenuRoutedLink
													{
														Text = "Reports",
														Action = "Reports",
														Controller = "Admin",
														Indented = 2
													});
				}
				model.BoxedLinkCollections.Add(adminMenu);
			}

			return View(model);
		}

		public ActionResult About()
		{
			ViewBag.IsAdmin = IsAdmin;

			return View();
		}
	}
}
