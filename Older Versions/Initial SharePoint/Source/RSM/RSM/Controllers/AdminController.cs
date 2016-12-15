using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using RSM.Artifacts;
using RSM.Artifacts.Log;
using RSM.Models;
using RSM.Models.Admin;
using RSM.Support;

namespace RSM.Controllers
{
	public class AdminController : BaseController
	{
		#region Constructor
		public AdminController() : base()
		{
		   
		}
		#endregion

		#region Activities

		[Authorize(Roles="admin")]
		public ActionResult ActivityLog(string id)
		{
			id = id ?? "-1";

			var collection = DbContext.ExternalSystems.Where(x => x.Name != Constants.R1SMSystemName).OrderBy(x => x.Name)
										.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(CultureInfo.InvariantCulture) }).ToList();

			collection.Insert(0, new SelectListItem { Value = "0", Text = "User Activity" });
			collection.Insert(0, new SelectListItem { Value = "", Text = "All" });

			var model = new ActivityLogCollectionModel
			            	{
			            		IsAdmin = IsAdmin,
			            		System = id,
			            		SystemCollection = new SelectList(collection, "Value", "Text", id),
			            		SidebarMenu = BuildSidebarMenu("Admin", "Activity Log")
			            	};


			return View(model);
		}

		private BosiStatusModel CreateBosiStatusModel(ExternalSystem system, BatchHistory batch, Severity severity)
		{
			var defaultDate = DateTime.Parse("01/01/1970");

			var count = EventLogger.CountLogEntriesWithStatus(system.Id, severity, batch == null ? defaultDate : batch.RunEnd);

			return count > 0 ? new BosiStatusModel(system, severity, batch) { LogCount = count } : null;
		}

		[Authorize(Roles = "admin")]
		public JsonResult CommunicationStatusCollection(string sidx, string sord, int page = 1, int rows = 20)
		{
			page = page == default(int) ? 0 : page;

			var pageSize = rows;

			var statusCollection = new List<BosiStatusModel>();

			foreach (var system in DbContext.ExternalSystems.Where(x => x.Name != Constants.R1SMSystemName))
			{
				var lastBatch = DbContext.BatchHistories.Where(x => x.SystemId == system.Id).OrderByDescending(x => x.RunEnd).FirstOrDefault();

				var bosiModel = CreateBosiStatusModel(system, lastBatch, Severity.Error);
				if (bosiModel != null)
					statusCollection.Add(bosiModel);

				bosiModel = CreateBosiStatusModel(system, lastBatch, Severity.Warning);
				if (bosiModel != null)
					statusCollection.Add(bosiModel);

				bosiModel = CreateBosiStatusModel(system, lastBatch, Severity.Informational);
				if (bosiModel != null)
					statusCollection.Add(bosiModel);
			}

			var totalRecords = statusCollection.Count();
			var totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

			// colNames: ['SystemName', 'Direction', 'SeverityName', 'LastAction', 'LogCount']
			var jsonData = new
			{
				total = totalPages,
				page = page,
				records = totalRecords,
				rows = (
					from entry in statusCollection
					select new
					{
						i = entry.SystemId.ToString(CultureInfo.InvariantCulture),
						cell = new string[] {
							entry.SystemId.ToString(CultureInfo.InvariantCulture),
							((Severity)entry.Severity).ToString().ToLower(),
							entry.SystemName,
							entry.Direction == ExternalSystemDirection.Incoming ? "Import" : entry.Direction == ExternalSystemDirection.Outgoing ? "Export" : "",
							entry.LastAction.ToShortDateString(),
							entry.Outcome.ToString(),
							entry.Message,
							entry.LogCount.ToString(CultureInfo.InvariantCulture)
						}
					}).ToArray()

			};

			return Json(jsonData);
		}

		[Authorize(Roles = "admin")]
		public ActionResult Index()
		{
			var model = new BosiStatusCollectionModel { IsAdmin = IsAdmin, SidebarMenu = BuildSidebarMenu("Admin", "Admin") };

			return View(model);
		}

		[Authorize(Roles = "admin")]
		public ActionResult EventDetails(int id, int? filter)
		{
			var system = (filter.HasValue && filter.Value >= 0 ? filter.Value.ToString(CultureInfo.InvariantCulture) : "");
			try
			{
				var entry = DbContext.LogEntries.First(e => e.ID == id);

				var model = new LogEntryModel(entry, system) { IsAdmin = IsAdmin, PageTitle = "Event Details", SidebarMenu = BuildSidebarMenu("Admin", "Activity Log") };

				return View(model);
			}
			catch (Exception)
			{
				return RedirectToAction("ActivityLog", new { id = filter });
			}
		}

		[Authorize(Roles = "admin")]
		public ActionResult SysLog(int id, string sidx, string sord, int page, int rows)
		{
			var pagingModel = new PagingModel(page, rows, DbContext.LogEntries.Count());

			List<LogEntry> entries;

			if (id >= 0)
			{
				entries = DbContext.LogEntries
					.Where(x => x.Source == id)
					.OrderBy("EventDate " + sord)
					.Skip(pagingModel.PageIndex * pagingModel.PageSize)
					.Take(pagingModel.PageSize).ToList();
			}
			else
			{
				entries = DbContext.LogEntries
							.OrderBy("EventDate " + sord)
							.Skip(pagingModel.PageIndex * pagingModel.PageSize)
							.Take(pagingModel.PageSize).ToList();
			}

			// colNames: ['Status', 'Name', 'BadgeNumber', 'Department', 'Job', 'Facility', 'LastUpdated'],
			var jsonData = new
			{
				total = pagingModel.TotalPages,
				page = pagingModel.Page,
				records = pagingModel.TotalRecords,
				rows = (
					from entry in entries
					select new
					{
						i = entry.ID,
						cell = new string[] {
							entry.ID.ToString(),
							entry.Severity.ToString(),
							entry.EventDate.ToString(),
							entry.SourceName,
							entry.Message
						}
					}).ToArray()

			};

			return Json(jsonData);
		}

		#endregion

		#region Reports
		[Authorize(Roles = "admin")]
		[HttpPost, ValidateInput(false)]
		public ActionResult Reports(FormCollection collection)
		{
			var memStream = new MemoryStream(100);
			TextWriter tw = new StreamWriter(memStream);
			var context = new RSMDataModelDataContext();

			switch (collection["ReportName"])
			{
				case "1":
					tw.WriteLine("Access Level Name,Access Level Description,People");
					foreach(var alRpt in context.AccessLevelReports.OrderBy("AccessLevelName"))
					{
						tw.WriteLine(String.Format("{0},\"{1}\",\"{2}\"", alRpt.AccessLevelName, alRpt.AccessLevelDesc, alRpt.people));

					}
					memStream.Seek(0, SeekOrigin.Begin);
					return File(memStream, "text/csv", "Access Level Report.csv");
				 
				case "2":
					tw.WriteLine("Last Name, First Name, Department, Position, Levels");
					foreach(var  elRpt in context.EmpAccessLevelReports.OrderBy("lastname"))
					{
						tw.WriteLine(String.Format("{1},{0},\"{2}\",{3},\"{4}\"", elRpt.firstname,elRpt.lastname,elRpt.Deptdescr,elRpt.jobdescr,elRpt.levels ));

					}
					memStream.Seek(0, SeekOrigin.Begin);
					return File(memStream, "text/csv", "Employee Access Report.csv");
			}

			return RedirectToAction("Reports");
		}
		
		[Authorize(Roles = "admin")]
		public ActionResult Reports()
		{
			var items = new List<SelectListItem>
			            	{
			            		new SelectListItem
			            			{
			            				Text = "Access Levels",
			            				Value = "1"
			            			},
			            		new SelectListItem
			            			{
			            				Text = "Associates",
			            				Value = "2"
			            			}
			            	};

			var reportList = new SelectList(items, "Value", "Text");

			ViewBag.ReportList = reportList;

			return View();
		}
		#endregion

		#region Job Codes
	
		[Authorize(Roles = "admin,rule administrator")]
		public ActionResult JobCodes()
		{
			var pageSize = CookieValueAsInt(CookieNames.GridPageSize, Constants.DefaultPageSize);

			var model = new JobCodeCollection
			            	{
								IsAdmin = IsAdmin,
								PageTitle = "Job Codes",
								DataAction = "JobCodeList",
								ReviewAction = "JobCode",
								PagingModel = new PagingModel(0, pageSize, 0),
								GridCaption = "All Associataes",
								SidebarMenu = BuildSidebarMenu("Admin", "Job Codes")
			            	};

			return View(model);
		}

		[Authorize(Roles = "admin,rule administrator")]
		public ActionResult JobCode(string id)
		{
			try
			{
				var model = new JobModel
				            	{
				            		Job = DbContext.Jobs.FirstOrDefault(e => e.JobCode == id),
									SidebarMenu = BuildSidebarMenu("Admin", "Job Codes")
								};

				return View(model);
			}
			catch (Exception)
			{
				return RedirectToAction("JobCodes");
			}
		}

		[Authorize(Roles = "admin,rule administrator")]
		[HttpPost, ValidateInput(false)]
		public ActionResult JobCode(string id, FormCollection collection)
		{
			try
			{
				var model = new JobModel
				            	{
				            		Job = DbContext.Jobs.FirstOrDefault(e => e.JobCode == id)
				            	};

				model.Job.DisplayDescription = collection["DisplayDescription"];
				model.Job.Credentials = collection["Credentials"];

				var people = DbContext.Persons.Where(p => p.JobCode == id).ToArray();
				
				foreach (var p in people)
				{
					p.NeedsUpload = true;
				}

				DbContext.SubmitChanges();	
			}
			catch (Exception)
			{
				return RedirectToAction("JobCode", new { id = id });
			}

			return RedirectToAction("JobCodes");
		}

		[Authorize(Roles = "admin,rule administrator")]
		public ActionResult JobCodeList(string sidx, string sord, int page, int rows)
		{
			var pageIndex = Convert.ToInt32(page) - 1;
			var pageSize = rows;
			var totalRecords = DbContext.Jobs.Count();
			var totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

			var entries = DbContext.Jobs
				.OrderBy(sidx + " " + sord)
				.Skip(pageIndex * pageSize)
				.Take(pageSize).ToList();

			// colNames: ['Status', 'Name', 'BadgeNumber', 'Department', 'Job', 'Facility', 'LastUpdated'],
			var jsonData = new
			{
				total = totalPages,
				page = page,
				records = totalRecords,
				rows = (
					from entry in entries
					select new
					{
						i = entry.JobCode,
						cell = new string[] {
							entry.JobCode,
							entry.JobDescription,
							entry.DisplayDescription
						}
					}).ToArray()

			};

			return Json(jsonData);
		}

		#endregion

	}
}