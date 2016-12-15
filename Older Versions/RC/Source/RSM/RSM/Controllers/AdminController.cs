using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using RSM.Models;
using RSM.Support;

namespace RSM.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        [Authorize(Roles="admin")]
        public ActionResult ActivityLog(int? Filter)
        {
            
            HttpCookie ck;
            ViewBag.IsAdmin = false;
            if (Filter.HasValue)
            {

                ViewBag.Filter = Filter;
                
                switch (Filter)
                {
                    case -1:
                        ck = new HttpCookie("LastS2Imp", DateTime.Now.ToString());
                        Response.Cookies.Add(ck);
                        ck = new HttpCookie("LastS2Exp", DateTime.Now.ToString());
                        Response.Cookies.Add(ck);
                        ck = new HttpCookie("LastPSImp", DateTime.Now.ToString());
                        Response.Cookies.Add(ck);
                        break;
                    case (int)RSMDataModelDataContext.LogSources.PSIMPORT:
                        ck = new HttpCookie("LastPSImp", DateTime.Now.ToString());
                        Response.Cookies.Add(ck);
                        break;
                    case (int)RSMDataModelDataContext.LogSources.S2IMPORT:
                        ck = new HttpCookie("LastS2Imp", DateTime.Now.ToString());
                        Response.Cookies.Add(ck);
                        break;
                    case (int)RSMDataModelDataContext.LogSources.S2EXPORT:
                        ck = new HttpCookie("LastS2Exp", DateTime.Now.ToString());
                        Response.Cookies.Add(ck);
                        break;

                }
            }
            else
            {
                ck = new HttpCookie("LastS2Imp", DateTime.Now.ToString());
                Response.Cookies.Add(ck);
                ck = new HttpCookie("LastS2Exp", DateTime.Now.ToString());
                Response.Cookies.Add(ck);
                ck = new HttpCookie("LastPSImp", DateTime.Now.ToString());
                Response.Cookies.Add(ck);
                ViewBag.Filter = -1;
            }



            if (User.IsInRole("admin") || User.Identity.Name == "admin")
            {
                ViewBag.IsAdmin = true;
            }
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost, ValidateInput(false)]
        public ActionResult Reports(FormCollection collection)
        {
            MemoryStream memStream = new MemoryStream(100);
            TextWriter tw = new StreamWriter(memStream);
            RSMDataModelDataContext context = new RSMDataModelDataContext();
           

            switch (collection["ReportName"])
            {
                case "1":
                    tw.WriteLine("Access Level Name,Access Level Description,People");
                    foreach(AccessLevelReport alRpt in context.AccessLevelReports.OrderBy("AccessLevelName"))
                    {
                        tw.WriteLine(String.Format("{0},\"{1}\",\"{2}\"", alRpt.AccessLevelName, alRpt.AccessLevelDesc, alRpt.people));

                    }
                    memStream.Seek(0, SeekOrigin.Begin);
                    return File(memStream, "text/csv", "Access Level Report.csv");
                 case "2":
                    tw.WriteLine("Last Name, First Name, Department, Position, Levels");
                    foreach(EmpAccessLevelReport  elRpt in context.EmpAccessLevelReports.OrderBy("lastname"))
                    {
                        tw.WriteLine(String.Format("{1},{0},\"{2}\",{3},\"{4}\"", elRpt.firstname,elRpt.lastname,elRpt.Deptdescr,elRpt.jobdescr,elRpt.levels ));

                    }
                    memStream.Seek(0, SeekOrigin.Begin);
                    return File(memStream, "text/csv", "Employee Access Report.csv");
            };

            return RedirectToAction("Reports");
        }
        

        [Authorize(Roles = "admin")]
        public ActionResult Reports()
        {
            SelectList ReportList;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Access Levels",
                Value = "1"
            });

            items.Add(new SelectListItem
            {
                Text = "Associates",
                Value = "2"
            });

            ReportList = new SelectList(items, "Value", "Text");

            ViewBag.ReportList = ReportList;


            return View();
        }


        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            RSMDataModelDataContext context = new RSMDataModelDataContext();
            ViewBag.IsAdmin = false;

            DateTime lastS2Imp;
            DateTime lastS2Exp;
            DateTime lastPSImp;
            int count;
         

            lastPSImp = (Request.Cookies["LastPSImp"] != null) ? DateTime.Parse(Request.Cookies["LastPSImp"].Value) : DateTime.Parse("8/27/1970");
            ViewBag.PSImportStatus = (int)RSMDataModelDataContext.LogSeverity.INFO;

            // Check the event log for each area to see if there have been any new errors or warnings
            count = context.CountLogEntriesWithStatus(RSMDataModelDataContext.LogSources.PSIMPORT,
                                                      RSMDataModelDataContext.LogSeverity.ERROR,
                                                      lastPSImp);
            if (count == 0)
            {
                // No errors look for warnings
                count = context.CountLogEntriesWithStatus(RSMDataModelDataContext.LogSources.PSIMPORT,
                                                          RSMDataModelDataContext.LogSeverity.WARN,
                                                          lastPSImp);
                if (count !=0)
                    ViewBag.PSImportStatus = (int)RSMDataModelDataContext.LogSeverity.WARN;
            }
            else
            {
                // Yep we errored out.
                ViewBag.PSImportStatus = (int)RSMDataModelDataContext.LogSeverity.ERROR;
            }
            
            
            
            lastS2Imp = (Request.Cookies["LastS2Imp"] != null) ? DateTime.Parse(Request.Cookies["LastS2Imp"].Value) : DateTime.Parse("8/27/1970");
            ViewBag.S2ImportStatus = (int)RSMDataModelDataContext.LogSeverity.INFO;

           
            count = context.CountLogEntriesWithStatus(RSMDataModelDataContext.LogSources.S2IMPORT,
                                                      RSMDataModelDataContext.LogSeverity.ERROR,
                                                      lastS2Imp);
            if (count == 0)
            {
                // No errors look for warnings
                count = context.CountLogEntriesWithStatus(RSMDataModelDataContext.LogSources.S2IMPORT,
                                                          RSMDataModelDataContext.LogSeverity.WARN,
                                                          lastS2Imp);
                if (count != 0)
                    ViewBag.S2ImportStatus = (int)RSMDataModelDataContext.LogSeverity.WARN;
            }
            else
            {
                // Yep we errored out.
                ViewBag.S2ImportStatus = (int)RSMDataModelDataContext.LogSeverity.ERROR;
            }
            
            
            lastS2Exp = (Request.Cookies["LastS2Exp"] != null) ? DateTime.Parse(Request.Cookies["LastS2Exp"].Value) : DateTime.Parse("8/27/1970");
            ViewBag.S2ExportStatus = (int)RSMDataModelDataContext.LogSeverity.INFO;


            count = context.CountLogEntriesWithStatus(RSMDataModelDataContext.LogSources.S2EXPORT,
                                                      RSMDataModelDataContext.LogSeverity.ERROR,
                                                      lastS2Exp);
            if (count == 0)
            {
                // No errors look for warnings
                count = context.CountLogEntriesWithStatus(RSMDataModelDataContext.LogSources.S2EXPORT,
                                                          RSMDataModelDataContext.LogSeverity.WARN,
                                                          lastS2Exp);
                if (count != 0)
                    ViewBag.S2ExportStatus = (int)RSMDataModelDataContext.LogSeverity.WARN;
            }
            else
            {
                // Yep we errored out.
                ViewBag.S2ExportStatus = (int)RSMDataModelDataContext.LogSeverity.ERROR;
            }

            
            ViewBag.IsAdmin = true;
            
            return View();
        }
        
        [Authorize(Roles = "admin")]
        public ActionResult Settings()
        {
            SettingsModel model = new SettingsModel();

            ViewBag.IsAdmin = false;
            if (User.IsInRole("admin") || User.Identity.Name == "admin")
            {
                ViewBag.IsAdmin = true;
            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public ActionResult JobCodes()
        {
            ViewBag.IsAdmin = false;
            if (User.IsInRole("admin") || User.Identity.Name == "admin")
            {
                ViewBag.IsAdmin = true;
            }
            return View();
        }
        
        [Authorize(Roles = "admin")]
        public ActionResult JobCode(string id)
        {
            RSMDataModelDataContext context = new RSMDataModelDataContext();

            try
            {
                var model = (from e in context.Jobs
                             where e.JobCode == id
                             select e).Single();


                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("JobCodes");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost, ValidateInput(false)]
        public ActionResult JobCode(string id, FormCollection collection)
        {
            RSMDataModelDataContext context = new RSMDataModelDataContext();

            try
            {
                var model = (from e in context.Jobs
                             where e.JobCode == id
                             select e).Single();
                model.DisplayDescription = collection["DisplayDescription"];
                model.Credentials = collection["Credentials"];

                var people = (from p in context.Persons where p.JobCode == id select p).ToArray();
                
                foreach (var p in people)
                {
                    p.NeedsUpload = true;
                }

                context.SubmitChanges();
                
            }
            catch (Exception)
            {
                return RedirectToAction("JobCode", new { id = id });
            }

            return RedirectToAction("JobCodes");
        }

        [Authorize(Roles = "admin")]
        public ActionResult EventDetails(int ID, int? Filter)
        {
            ViewBag.IsAdmin = false;
            if (Filter.HasValue)
                ViewBag.Filter = Filter;
            else
                ViewBag.Filter = -1;

            
            ViewBag.IsAdmin = true;
            
            RSMDataModelDataContext context = new RSMDataModelDataContext();

            try
            {
                var entry = (from e in context.LogEntries
                             where e.ID == ID
                             select e).Single();


                return View(entry);
            }
            catch (Exception)
            {
                return  RedirectToAction("ActivityLog", new {filter=Filter}); 
            }
        }

        [HttpPost, ValidateInput(false)]
        [Authorize(Roles = "admin")]
        public ActionResult Settings(FormCollection collection)
        {
            SettingsModel model = new SettingsModel();
            model.RuleEngineAllow = (collection["RuleEngineAllow"].ToLower().StartsWith("true"));
//            model.RequireApproval = (collection["RequireApproval"].ToLower().StartsWith("true"));
            if(collection["AdminPass"].Length > 0)
                model.AdminPass = collection["AdminPass"];

            model.JobCodesFirst = (collection["JobCodesFirst"].ToLower().StartsWith("true"));
            model.S2Address = collection["S2Address"];
            model.S2RSMAccountName = collection["S2RSMAccountName"];
            if(collection["S2RSMAccountPassword"].Length > 0)
                model.S2RSMAccountPassword = collection["S2RSMAccountPassword"];
            model.S2AllowExport = (collection["S2AllowExport"].ToLower().StartsWith("true"));
            model.S2AllowImport = (collection["S2AllowImport"].ToLower().StartsWith("true"));


            model.PSAllowImport = (collection["PSAllowImport"].ToLower().StartsWith("true"));
            model.RedFolder = collection["RedFolder"];
            model.GreenFolder = collection["GreenFolder"];

            
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "admin")]
        public ActionResult SysLog(int filter, string sidx, string sord, int page, int rows)
        {
            var context = new RSMDataModelDataContext();
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = context.LogEntries.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);


            List<LogEntry> entries;
                
            
            if(filter >= 0)
            {
                entries = (from e in context.LogEntries
                           where e.Source == filter
                           select e).OrderBy("EventDate " + sord)
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize).ToList();
            }
            else
            {
                entries = context.LogEntries
                            .OrderBy("EventDate " + sord)
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize).ToList();
            }

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
                        i = entry.ID,
                        cell = new string[] {
                            entry.ID.ToString(),
                            ((int)entry.Severity).ToString(),
                            entry.EventDate.ToString(),
                            entry.SourceName,
                            entry.Message 

                        }
                    }).ToArray()

            };

            return Json(jsonData);
        }


        [Authorize(Roles = "admin")]
        public ActionResult JobCodeList(string sidx, string sord, int page, int rows)
        {
            var context = new RSMDataModelDataContext();
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = context.Jobs.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);


            List<Job> entries;


            entries = context.Jobs
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

    }
}