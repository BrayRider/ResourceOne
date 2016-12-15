using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Xml;
using Newtonsoft.Json;
using RSM.Artifacts;
using RSM.Artifacts.Log;
using RSM.Artifacts.Requests;
using RSM.Models;
using RSM.Models.Enums;
using RSM.Models.People;
using RSM.Service.Library;
using RSM.Service.Library.Controllers;
using RSM.Support;
using RSM.Support.Interfaces;
using RSM.Support.S2;
using RSM.Web.Library;
using LinqKit;

namespace RSM.Controllers
{
	public class PeopleController : BaseController
	{
		#region Properties
		
		private S2API api;
		public S2API API
		{
			get {
				if(api == null)
				{
					var s2Host = Settings.GetValue("S2", "ServiceAddress");
					api = new S2API(String.Format("{0}/goforms/nbapi", s2Host),
									 Settings.GetValue("S2", "ServiceAccount"),
									 Settings.GetValue("S2", "ServicePassword"));
				}

				return api;
			}
		}

		public bool AllowRuleAdministration { get; set; }

		#endregion

		public PeopleController()
		{
			AllowRuleAdministration = Settings.GetValueAsBool(Constants.R1SMSystemName, "RuleEngineAllow");
			
			OwnedSystemName = Constants.R1SMSystemName;

			ControllerSetup();
		}

		#region Actions

		#region List Pages
		[Authorize]
		public ActionResult Index()
		{
			var pageSize = CookieValueAsInt(CookieNames.GridPageSize, Constants.DefaultPageSize);

			var model = new AssociateCollection
			            	{
			            		PageTitle = "Associates",
			            		DataAction = "EmployeeListSearch",
			            		ReviewAction = "Details",
			            		AreaTitle = "Associates",
			            		AreaMessage = "The grid below contains all associates that have been imported.",
			            		IsAdmin = IsAdmin,
			            		IsReview = false,
			            		ReviewMode = ReviewModes.None,
			            		PagingModel = new PagingModel(0, pageSize, 0),
								GridCaption = "All Associataes",
			            		SidebarMenu = BuildSidebarMenu("Associates", "Associates")
			            	};

			return View(model);
		}

		[Authorize]
		public ActionResult ReviewQueue(string status)
		{
			var pageSize = CookieValueAsInt(CookieNames.GridPageSize, Constants.DefaultPageSize);

			var model = new AssociateCollection
			{
				IsAdmin = IsAdmin,
				IsReview = true,
				PagingModel = new PagingModel(0, pageSize, 0),
				GridCaption = "Associate Review Queue"
			};

			switch (status.ToLower())
			{
				case "hire":
					model.CurrentMenuItem = "New Hires";
					model.ReviewMode = ReviewModes.Hire;
					model.PageTitle = "New Hires";
					model.DataAction = "NewEmployeesSearch";
					model.ReviewAction = "ReviewHire";
					model.AreaTitle = "Recently Hired Associates";
					model.AreaMessage =
						"The associates in the grid below are new hires that have had roles assigned to them by the Resource One Security Manager rule engine.";
					break;

				case "fire":
					model.CurrentMenuItem = "Terminations";
					model.ReviewMode = ReviewModes.Fire;
					model.PageTitle = "Recent Terminations";
					model.DataAction = "TermEmployeesSearch";
					model.ReviewAction = "ReviewTerm";
					model.AreaTitle = "Recently Terminated Associates";
					model.AreaMessage =
						"The associates in the grid below have been terminated and had their access removed by the Resource One Security Manager rule engine.";
					break;

				case "changed":
					model.CurrentMenuItem = "Changed";
					model.ReviewMode = ReviewModes.Changed;
					model.PageTitle = "Changed Associates";
					model.DataAction = "ChangedEmployeesSearch";
					model.ReviewAction = "Review";
					model.AreaTitle = "Recently Changed Associates";
					model.AreaMessage =
						"The associates in the grid below have had roles assigned to them by the Resource One Security Manager rule engine due to changes in job, department or location.";
					break;

				default:
					RedirectToAction("Index");
					break;
			}

			model.SidebarMenu = BuildSidebarMenu("Associates", model.CurrentMenuItem);

			return View(model);
		}
		#endregion

		#region Review
		[Authorize]
		public ActionResult Review(int id)
		{
			return DisplayOrReview(id, ReviewModes.Changed, "ReviewQueue", "Review", "Changed");
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult Review(int id, FormCollection collection)
		{
			return ApproveAccess(id, collection, "ReviewQueue");
		}

		[Authorize]
		public ActionResult ReviewHire(int id)
		{
			return DisplayOrReview(id, ReviewModes.Hire, "ReviewQueue", "ReviewHire", "Hire");
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult ReviewHire(int id, FormCollection collection)
		{
			return ApproveAccess(id, collection, "ReviewQueue");
		}

		[Authorize]
		public ActionResult ReviewTerm(int id)
		{
			return DisplayOrReview(id, ReviewModes.Fire, "ReviewQueue", "ReviewTerm", "Fire");
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult ReviewTerm(int id, FormCollection collection)
		{
			return ApproveAccess(id, collection, "ReviewQueue");
		}
		
		#endregion

		#region Details
		[Authorize]
		public ActionResult Details(int id)
		{
			return DisplayOrReview(id, ReviewModes.None, "Index", "Details");
		}

		[Authorize]
		public ActionResult Edit(int id, string back)
		{
			return Edit(id, false, back);
		}

		[Authorize]
		public ActionResult ReviewEdit(int id)
		{
			return Edit(id, true, "Index");
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult ReviewEdit(int id, FormCollection collection)
		{
			try
			{
				ProcessEditForm(id, collection);
				return RedirectToAction("Index", "Home");
			}
			catch
			{
				return Edit(id, true, "Index");
			}

		}

		[HttpPost, ValidateInput(false)]
		public ActionResult Edit(int id, FormCollection collection)
		{
			try
			{
				ProcessEditForm(id, collection);
				return RedirectToAction("index");
			}
			catch
			{

				return Edit(id, "Index");
			}
		}
		#endregion
		
		#endregion

		#region Json Results

		[Authorize]
		public JsonResult AvailableRoles(int id)
		{
			var availRoles = DbContext.RolesAvialableForPerson(id);

			var jsonAvailRoles = new
			{
				total = 1,
				page = 1,
				records = 1,
				rows = (
					from r in availRoles
					select new
					{
						i = r.RoleID,
						cell = new [] {
							r.RoleID.ToString(CultureInfo.InvariantCulture), r.RoleName, r.RoleDesc

						}
					}).ToArray()
			};

			return Json(jsonAvailRoles);
		}

		[Authorize]
		public JsonResult AssignedRoles(int id)
		{
			var person = DbContext.Persons.First(p => p.PersonID == id);

			var jsonAssLevels = new
			{
				total = 1,
				page = 1,
				records = person.PeopleRoles.Count,
				rows = (
					from r in person.PeopleRoles
					select new
					{
						i = r.ID,
						cell = new[] {
							r.RoleID.ToString(CultureInfo.InvariantCulture), r.Role.RoleName, r.Role.RoleDesc, ((r.IsException) ? 1 : 0).ToString(CultureInfo.InvariantCulture)
						}
					}).ToArray()
			};

			return Json(jsonAssLevels);
		}

		[Authorize]
		public JsonResult EmployeeListSearch(string sidx, string sord, int page = 0, int rows = 1, bool _search = false, string searchField = null, string searchOper = null, string searchString = null)
		{
			var sortBy = Person.SortFieldName(sidx);
			var request = new PagedRequest {Page = page, PageSize = rows, SortField = sortBy, SortDirection = sord, PageIndex = page - 1};
			SaveCookie(CookieNames.GridPageSize, rows.ToString(CultureInfo.InvariantCulture));

			Expression<Func<Person, bool>> whereClause = null;

			if (_search)
			{
				switch (searchField)
				{
					case "Name":
						whereClause = p => (p.LastName.Contains(searchString) || p.FirstName.Contains(searchString));
						break;

					case "Department":
						whereClause = p => p.DeptDescr.Contains(searchString);
						break;

					case "Position":
						whereClause = p => p.JobDescr.Contains(searchString);
						break;

					case "Job":
						whereClause = p => p.JobDescr.Contains(searchString);
						break;

					case "Facility":
						whereClause = p => p.Facility.Contains(searchString);
						break;
				}
			}

			var dataController = new People();

			var results = dataController.Search(request, whereClause);

			if (results.Failed)
				return null;

			request.ResetPages(page, rows, results.RowsReturned);

			// colNames: ['Status', 'Name', 'BadgeNumber', 'Department', 'Job', 'Facility', 'LastUpdated'],
			var jsonData = new
			               	{
			               		total = request.TotalPages,
			               		page = request.Page,
			               		records = results.RowsReturned,
			               		rows = (
			               		       	from emp in results.Entity
			               		       	select new
			               		       	       	{
			               		       	       		i = emp.PersonID,
			               		       	       		cell = new[]
			               		       	       		       	{
			               		       	       		       		emp.Active.ToString(CultureInfo.InvariantCulture),
																emp.DisplayName,
			               		       	       		       		string.IsNullOrWhiteSpace(emp.BadgeNumber) ? "" : emp.BadgeNumber,
			               		       	       		       		emp.DeptDescr,
			               		       	       		       		emp.JobDescr,
			               		       	       		       		emp.Facility,
			               		       	       		       		emp.LastUpdated.ToString("d"),
			               		       	       		       		emp.LastUpdateMask.ToString(CultureInfo.InvariantCulture),
			               		       	       		       		emp.PersonID.ToString(CultureInfo.InvariantCulture)
			               		       	       		       	}
			               		       	       	}).ToArray()
			               	};

			return Json(jsonData);
		}

		//[Authorize]
		//public JsonResult ChangedEmployeesSearch(string sidx, string sord, int page = 0, int rows = 1, bool _search = false, string searchField = null, string searchOper = null, string searchString = null)
		//{
		//    var sortBy = Person.SortFieldName(sidx);
		//    var request = new PagedRequest { Page = page, PageSize = rows, SortField = sortBy, SortDirection = sord };
		//    SaveCookie(CookieNames.GridPageSize, rows.ToString(CultureInfo.InvariantCulture));

		//    var predicate = PredicateBuilder.True<Person>();
		//    predicate = predicate.And((p => p.NeedsApproval || (((p.LastUpdateMask & 16) == 0) && (p.LastUpdateMask > 0))));

		//    if (_search)
		//    {
		//        switch (searchField)
		//        {
		//            case "Name":
		//                predicate.And(p => p.LastName.Contains(searchString) || p.FirstName.Contains(searchString));
		//                break;

		//            case "Department":
		//                predicate.And(p => p.NeedsApproval && p.DeptDescr.Contains(searchString));
		//                break;

		//            case "Position":
		//                predicate.And(p => p.NeedsApproval && p.JobDescr.Contains(searchString));
		//                break;

		//            case "Job":
		//                predicate.And(p => p.NeedsApproval && p.JobDescr.Contains(searchString));
		//                break;

		//            case "Facility":
		//                predicate.And(p => p.NeedsApproval && p.Facility.Contains(searchString));
		//                break;
		//        }
		//    }

		//    var dataController = new People();

		//    var results = dataController.Search(request, predicate);

		//    if (results.Failed)
		//        return null;

		//    request.ResetPages(page, rows, results.RowsReturned);

		//    // colNames: ['Status', 'Name', 'BadgeNumber', 'Department', 'Job', 'Facility', 'LastUpdated'],
		//    var jsonData = new
		//    {
		//        total = request.TotalPages,
		//        page = request.Page,
		//        records = results.RowsReturned,
		//        rows = (
		//                from emp in results.Entity
		//                select new
		//                {
		//                    i = emp.PersonID,
		//                    cell = new[]
		//                                                    {
		//                                                        emp.Active.ToString(CultureInfo.InvariantCulture),
		//                                                        string.IsNullOrWhiteSpace(emp.MiddleName)
		//                                                            ? string.Format("{0}, {1}", emp.LastName, emp.FirstName)
		//                                                            : string.Format("{0}, {1} {2}", emp.LastName, emp.FirstName,
		//                                                                            emp.MiddleName),
		//                                                        emp.BadgeNumber,
		//                                                        emp.DeptDescr,
		//                                                        emp.JobDescr,
		//                                                        emp.Facility,
		//                                                        emp.LastUpdated.ToString("d"),
		//                                                        emp.LastUpdateMask.ToString(CultureInfo.InvariantCulture),
		//                                                        emp.PersonID.ToString(CultureInfo.InvariantCulture)
		//                                                    }
		//                }).ToArray()
		//    };

		//    return Json(jsonData);
		//}

		[Authorize]
		public JsonResult ChangedEmployeesSearch(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
		{
			var sortBy = Person.SortFieldName(sidx);
			var request = new PagedRequest { Page = page, PageSize = rows, SortField = sortBy, SortDirection = sord, PageIndex = page - 1 };
			SaveCookie(CookieNames.GridPageSize, rows.ToString(CultureInfo.InvariantCulture));

			Expression<Func<EmpWithNonStatusChange, bool>> whereClause = null;

			if (_search)
			{
				switch (searchField)
				{
					case "Department":
						whereClause = p => p.DeptDescr.Contains(searchString);
						break;

					case "Name":
						whereClause = p => (p.LastName.Contains(searchString) || p.FirstName.Contains(searchString));
						break;

					case "Job":
						whereClause = p => p.JobDescr.Contains(searchString);
						break;

					case "Status":
						whereClause = p => p.Active != string.IsNullOrWhiteSpace(searchString) && !searchString.Contains("false");
						break;
				}
			}

			var dataController = new EmpWithNonStatusChanges();

			var results = dataController.Search(request, whereClause);

			if (results.Failed)
				return null;

			request.ResetPages(page, rows, results.RowsReturned);

			// colNames: ['Status', 'Name', 'BadgeNumber', 'Department', 'Job', 'Facility', 'LastUpdated'],
			var jsonData = new
							{
								total = request.TotalPages,
								page = request.Page,
								records = results.RowsReturned,
								rows = (
										from emp in results.Entity
										select new
												{
													i = emp.PersonID,
													cell = new[]
		                                                    {
		                                                        emp.Active.ToString(CultureInfo.InvariantCulture),
		                                                        string.IsNullOrWhiteSpace(emp.MiddleName)
		                                                            ? string.Format("{0}, {1}", emp.LastName, emp.FirstName)
		                                                            : string.Format("{0}, {1} {2}", emp.LastName, emp.FirstName,
		                                                                            emp.MiddleName),
			               		       	       		       		string.IsNullOrWhiteSpace(emp.BadgeNumber) ? "" : emp.BadgeNumber,
		                                                        emp.DeptDescr,
		                                                        emp.JobDescr,
		                                                        emp.Facility,
		                                                        emp.LastUpdated.HasValue ? emp.LastUpdated.Value.ToString("d") : "",
		                                                        emp.LastUpdateMask.ToString(CultureInfo.InvariantCulture),
		                                                        emp.PersonID.ToString(CultureInfo.InvariantCulture)
		                                                    }
												}).ToArray()
							};

			return Json(jsonData);
		}

		[Authorize]
		public JsonResult NewEmployeesSearch(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
		{
			var sortBy = Person.SortFieldName(sidx);
			var request = new PagedRequest { Page = page, PageSize = rows, SortField = sortBy, SortDirection = sord, PageIndex = page - 1 };
			SaveCookie(CookieNames.GridPageSize, rows.ToString(CultureInfo.InvariantCulture));

			Expression<Func<NewHire, bool>> whereClause = null;

			if (_search)
			{
				switch (searchField)
				{
					case "Department":
						whereClause = p => p.DeptDescr.Contains(searchString);
						break;

					case "Name":
						whereClause = p => (p.LastName.Contains(searchString) || p.FirstName.Contains(searchString));
						break;

					case "Job":
						whereClause = p => p.JobDescr.Contains(searchString);
						break;

					case "Status":
						whereClause = p => p.Active != string.IsNullOrWhiteSpace(searchString) && !searchString.Contains("false");
						break;
				}
			}

			var dataController = new NewHires();

			var results = dataController.Search(request, whereClause);

			if (results.Failed)
				return null;

			request.ResetPages(page, rows, results.RowsReturned);

			// colNames: ['Status', 'Name', 'BadgeNumber', 'Department', 'Job', 'Facility', 'LastUpdated'],
			var jsonData = new
			               	{
			               		total = request.TotalPages,
			               		page = request.Page,
			               		records = results.RowsReturned,
			               		rows = (
			               		       	from emp in results.Entity
			               		       	select new
			               		       	       	{
			               		       	       		i = emp.PersonID,
			               		       	       		cell = new[]
			               		       	       		       	{
			               		       	       		       		emp.Active.ToString(CultureInfo.InvariantCulture),
			               		       	       		       		string.IsNullOrWhiteSpace(emp.MiddleName)
			               		       	       		       			? string.Format("{0}, {1}", emp.LastName, emp.FirstName)
			               		       	       		       			: string.Format("{0}, {1} {2}", emp.LastName, emp.FirstName,
			               		       	       		       			                emp.MiddleName),
			               		       	       		       		string.IsNullOrWhiteSpace(emp.BadgeNumber) ? "" : emp.BadgeNumber,
			               		       	       		       		emp.DeptDescr,
			               		       	       		       		emp.JobDescr,
			               		       	       		       		emp.Facility,
																emp.LastUpdated.HasValue ? emp.LastUpdated.Value.ToString("d") : "",
			               		       	       		       		emp.LastUpdateMask.ToString(CultureInfo.InvariantCulture),
			               		       	       		       		emp.PersonID.ToString(CultureInfo.InvariantCulture)
			               		       	       		       	}
			               		       	       	}).ToArray()
			               	};

			return Json(jsonData);
		}

		//[Authorize]
		//public JsonResult NewEmployeesSearch(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
		//{
		//    var sortBy = Person.SortFieldName(sidx);
		//    var request = new PagedRequest { Page = page, PageSize = rows, SortField = sortBy, SortDirection = sord };
		//    SaveCookie(CookieNames.GridPageSize, rows.ToString(CultureInfo.InvariantCulture));

		//    Expression<Func<NewHire, bool>> whereClause = null;

		//    if (_search)
		//    {
		//        switch (searchField)
		//        {
		//            case "Department":
		//                whereClause = p => p.DeptDescr.Contains(searchString);
		//                break;

		//            case "Name":
		//                whereClause = p => (p.LastName.Contains(searchString) || p.FirstName.Contains(searchString));
		//                break;

		//            case "Job":
		//                whereClause = p => p.JobDescr.Contains(searchString);
		//                break;

		//            case "Status":
		//                whereClause = p => p.Active != string.IsNullOrWhiteSpace(searchString) && !searchString.Contains("false");
		//                break;
		//        }
		//    }

		//    var dataController = new NewHires();

		//    var results = dataController.Search(request, whereClause);

		//    if (results.Failed)
		//        return null;

		//    request.ResetPages(page, rows, results.RowsReturned);

		//    // colNames: ['Status', 'Name', 'BadgeNumber', 'Department', 'Job', 'Facility', 'LastUpdated'],
		//    var jsonData = new
		//    {
		//        total = request.TotalPages,
		//        page = request.Page,
		//        records = results.RowsReturned,
		//        rows = (
		//                from emp in results.Entity
		//                select new
		//                {
		//                    i = emp.PersonID,
		//                    cell = new[]
		//                                                    {
		//                                                        emp.Active.ToString(CultureInfo.InvariantCulture),
		//                                                        string.IsNullOrWhiteSpace(emp.MiddleName)
		//                                                            ? string.Format("{0}, {1}", emp.LastName, emp.FirstName)
		//                                                            : string.Format("{0}, {1} {2}", emp.LastName, emp.FirstName,
		//                                                                            emp.MiddleName),
		//                                                        emp.BadgeNumber,
		//                                                        emp.DeptDescr,
		//                                                        emp.JobDescr,
		//                                                        emp.Facility,
		//                                                        emp.LastUpdated.HasValue ? emp.LastUpdated.Value.ToString("d") : "",
		//                                                        emp.LastUpdateMask.ToString(CultureInfo.InvariantCulture),
		//                                                        emp.PersonID.ToString(CultureInfo.InvariantCulture)
		//                                                    }
		//                }).ToArray()
		//    };

		//    return Json(jsonData);
		//}

		[Authorize]
		public JsonResult TermEmployeesSearch(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
		{
			var sortBy = Person.SortFieldName(sidx);
			var request = new PagedRequest { Page = page, PageSize = rows, SortField = sortBy, SortDirection = sord, PageIndex = page - 1 };
			SaveCookie(CookieNames.GridPageSize, rows.ToString(CultureInfo.InvariantCulture));

			Expression<Func<NewFire, bool>> whereClause = null;

			if (_search)
			{
				switch (searchField)
				{
					case "Department":
						whereClause = p => p.DeptDescr.Contains(searchString);
						break;

					case "Name":
						whereClause = p => (p.LastName.Contains(searchString) || p.FirstName.Contains(searchString));
						break;

					case "Job":
						whereClause = p => p.JobDescr.Contains(searchString);
						break;

					case "Status":
						whereClause = p => p.Active != string.IsNullOrWhiteSpace(searchString) && !searchString.Contains("false");
						break;
				}
			}

			var dataController = new NewFires();

			var results = dataController.Search(request, whereClause);

			if (results.Failed)
				return null;

			request.ResetPages(page, rows, results.RowsReturned);

			// colNames: ['Status', 'Name', 'BadgeNumber', 'Department', 'Job', 'Facility', 'LastUpdated'],
			var jsonData = new
			{
				total = request.TotalPages,
				page = request.Page,
				records = results.RowsReturned,
				rows = (
						from emp in results.Entity
						select new
						{
							i = emp.PersonID,
							cell = new[]
			               		       	       		       	{
			               		       	       		       		emp.Active.ToString(CultureInfo.InvariantCulture),
			               		       	       		       		string.IsNullOrWhiteSpace(emp.MiddleName)
			               		       	       		       			? string.Format("{0}, {1}", emp.LastName, emp.FirstName)
			               		       	       		       			: string.Format("{0}, {1} {2}", emp.LastName, emp.FirstName,
			               		       	       		       			                emp.MiddleName),
			               		       	       		       		emp.BadgeNumber,
			               		       	       		       		emp.DeptDescr,
			               		       	       		       		emp.JobDescr,
			               		       	       		       		emp.Facility,
																emp.LastUpdated.HasValue ? emp.LastUpdated.Value.ToString("d") : "",
			               		       	       		       		emp.LastUpdateMask.ToString(CultureInfo.InvariantCulture),
			               		       	       		       		emp.PersonID.ToString(CultureInfo.InvariantCulture)
			               		       	       		       	}
						}).ToArray()
			};

			return Json(jsonData);
		}

		#endregion

		#region Private Helpers
		private void UpdateRoles(Person person, IEnumerable<GridDataRow> roles)
		{
			var thisPersonRoles = DbContext.PeopleRoles.Where(r => r.PersonID == person.PersonID).ToList();
			
			if (thisPersonRoles.Any())
			{
				DbContext.PeopleRoles.DeleteAllOnSubmit(thisPersonRoles);
			}

			DbContext.SubmitChanges();

			foreach (var role in roles.Select(row => new PeopleRole {RoleID = int.Parse(row.ID), PersonID = person.PersonID, IsException = (row.Exception != "0")}))
			{
				person.PeopleRoles.Add(role);
			}

			DbContext.SubmitChanges();
		}

		private void ProcessEditForm(int id, FormCollection collection)
		{
			var allowUpload = Settings.GetValueAsBool("S2", "PersonExport");

			var jsonGridData = collection["gridData"];
			var gridData = JsonConvert.DeserializeObject<List<GridDataRow>>(jsonGridData);

			var dataAccess = new People(DbContext);

			var results = dataAccess.Get(id);

			if (results.Failed)
			{
				EventLogger.LogSystemActivity(OwnedSystem,
					                            Severity.Error,
					                            string.Format("Error getting person with id of {0} from R1SM", id),
					                            results.Message);

				RedirectToAction("Index", "People");
			}

			var person = results.Entity;

			if (AllowRuleAdministration)
			{
				UpdateRoles(person, gridData);
			}

			// If an admin edited this we need to see if any of the RSM specific stuff has changed.
			if (User.IsInRole("admin"))
			{
				person.IsAdmin = collection.GetValueAsBool("IsAdmin");

				person.LockedOut = collection.GetValueAsBool("Person.LockedOut");

				person.username = collection["Person.username"];
				var newPass = collection["Person.password"];

				if ((newPass.Length > 0) && (newPass != person.password))
				{
					// Get encryption and decryption key information from the configuration.
					var cfg = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
					var machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

					var hash = new HMACSHA512 { Key = Utilities.HexToByte(machineKey.ValidationKey) };

					var hash1 = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(collection["Person.password"] + ".rSmSa1t" + newPass.Length.ToString())));
					var hash2 = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(hash1 + "an0tH3r5alt!" + newPass.Length.ToString())));

					person.password =
					   Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(hash2)));
				}

				EventLogger.LogUserActivity(Severity.Informational,
							   User.Identity.Name + " modified access for " + person.DisplayName, "");
			}

			// Saving the person implies acceptance of the levels as assigned.
			person.NeedsApproval = false;
			person.Credentials = collection["Person.Credentials"];
			person.NickFirst = collection["Person.NickFirst"];
			DbContext.SubmitChanges();

			try
			{
				if (allowUpload)
				{
					// Now update the S2 box with the new employee record.
					this.API.SavePerson(person);
				}
				else
				{
					person.NeedsUpload = true;
					DbContext.SubmitChanges();
				}
			}
			catch
			{
				// If the update fails (likely due to a network issue)
				// queue up the person to be uploaded by the service later.
				person.NeedsUpload = true;
				DbContext.SubmitChanges();
			}

		}

		private ActionResult Edit(int id, bool fromReview, string back)
		{
			UpdateLastAccess();

			var dataAccess = new People();

			var results = dataAccess.Get(id);

			if (results.Failed)
			{
				EventLogger.LogSystemActivity(OwnedSystem,
											  Severity.Error,
											  string.Format("Error getting person with id of {0} from R1SM", id),
											  results.Message);

				RedirectToAction("Index", "People");
			}

			var person = results.Entity;

			var model = new Edit
			            	{
			            		Person = person,
			            		PictureUrl = Url.Content(GetPersonPicture(person)),
			            		AllowRuleAdministration = AllowRuleAdministration
			            	};
			if (AllowRuleAdministration)
			{
				model.AssignedRolesUrl = Url.Action("AssignedRoles", new { ID = person.PersonID });
				model.AvailableRolesUrl = Url.Action("AvailableRoles", new { ID = person.PersonID });
			}

			model.IsAdmin = IsAdmin;
			model.IsReview = fromReview;
			model.BackView = back;

			model.BreadcrumbStatus = string.Empty;
			model.BreadcrumbText = string.Empty;
			model.SidebarMenu = BuildSidebarMenu("Associates", "Associates");

			return View("Edit", model);

		}

		private ActionResult DisplayOrReview(int id, ReviewModes reviewMode, string returnView, string thisView, string returnStatus = null)
		{
			UpdateLastAccess();

			var dataAccess = new People();

			var results = dataAccess.Get(id);

			if (results.Failed)
			{
				EventLogger.LogSystemActivity(OwnedSystem,
				                              Severity.Error,
				                              string.Format("Error getting person with id of {0} from R1SM", id),
				                              results.Message);

				RedirectToAction("Index", "People");
			}

			var person = results.Entity;

			var model = new Detail
			{
				Person = person,
				PictureUrl = Url.Content(GetPersonPicture(person)),
				AllowRuleAdministration = AllowRuleAdministration
			};

			if (AllowRuleAdministration)
			{
				model.AssignedRolesUrl = Url.Action("AssignedRoles", new {ID = person.PersonID});
			}

			model.IsAdmin = IsAdmin;
			model.ReviewMode = reviewMode;
			model.ReturnView = returnView;
			model.ReturnStatus = returnStatus;
			model.IsReview = (reviewMode != ReviewModes.None);
			model.ReturnUrl = model.IsReview ? Url.Action(model.ReturnView, "People", new {status = model.ReturnStatus}) : Url.Action(model.ReturnView, "People");
			model.BackView = thisView;

			switch (reviewMode)
			{
				case ReviewModes.None:
					model.BreadcrumbStatus = string.Empty;
					model.BreadcrumbText = string.Empty;
					model.SidebarMenu = BuildSidebarMenu("Associates", "Associates");
					break;

				case ReviewModes.Hire:
					model.BreadcrumbStatus = "Hire";
					model.BreadcrumbText = "New Hires";
					model.SidebarMenu = BuildSidebarMenu("Associates", "New Hires");
					break;

				case ReviewModes.Fire:
					model.BreadcrumbStatus = "Fire";
					model.BreadcrumbText = "Terminations";
					model.SidebarMenu = BuildSidebarMenu("Associates", "Terminations");
					break;

				case ReviewModes.Changed:
					model.BreadcrumbStatus = "Changed";
					model.BreadcrumbText = "Changed";
					model.SidebarMenu = BuildSidebarMenu("Associates", "Changed");
					break;
			}

			return View("Details", model);
		}

		private ActionResult ApproveAccess(int id, FormCollection collection, string returnView)
		{
			// The submit button on this form is the "approve access" button
			// So we need to grab the person and submit them to the security hardware.

			var context = new RSMDataModelDataContext();
			var person = (from p in context.Persons
						  where p.PersonID == id
						  select p).Single();

			person.NeedsApproval = false;

			try
			{
				person.LastUpdateMask = 0;
				context.SubmitChanges();
			}

			catch (Exception e)
			{
				EventLogger.LogSystemActivity(OwnedSystem,
							  Severity.Error,
							  "Error exporting " + person.DisplayName + " to S2.",
							  e.ToString());

				RedirectToAction("Index", "People", new { ID = id });
			}

			try
			{

				API.SavePerson(person);
			}
			catch (Exception e)
			{
				EventLogger.LogSystemActivity(OwnedSystem,
							   Severity.Error,
							   "Error exporting " + person.DisplayName + " to S2.",
							   e.ToString());

				RedirectToAction("Index", "People", new { ID = id });
			}

			return RedirectToAction(returnView);
		}

		private string GetPersonPicture(Person person)
		{
			var userImage = "~/Content/images/employees/missing.jpg";
			var available = Settings.GetValueAsBool("S2In", "S2InAvailable");

			if (available)
			{
				try
				{
					var nd = API.GetPicture(person.PersonID.ToString(CultureInfo.InvariantCulture));

					if (nd == null)
						return userImage;

					if (nd["PICTUREURL"] == null)
						return userImage;

					if (nd["PICTURE"] == null)
						return userImage;

					var fileName = string.Format("~/Content/Images/employees/{0}", nd["PICTUREURL"].InnerText);
					var imageData = Convert.FromBase64String(nd["PICTURE"].InnerText);

					var filePath = Server.MapPath(Url.Content(fileName));

					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						using (var writer = new BinaryWriter(stream))
						{
							writer.Write(imageData);
							writer.Close();
						}
					}

					userImage = fileName;
				}
				catch (Exception)
				{
				}
			}

			return userImage;
		}
		#endregion
	}
}
