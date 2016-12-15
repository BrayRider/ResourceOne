using System.Web.Mvc;
using System.Web.Routing;

namespace RSM
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Admin",
				"Admin/ActivityLog/{id}",
				new { controller = "Admin", action = "ActivityLog", id = UrlParameter.Optional },
				new { id = @"\d+" }
			);

			routes.MapRoute(
				"AssociateDetails",
				"People/Details/{id}",
				new { controller = "People", action = "Details", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				"Associates",
				"People/ReviewQueue/{status}",
				new { controller = "People", action = "ReviewQueue", status = UrlParameter.Optional }
			);

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);

		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}
	}
}