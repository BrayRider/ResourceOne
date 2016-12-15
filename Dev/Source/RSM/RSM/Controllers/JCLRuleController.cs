using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using RSM.Artifacts.Log;
using RSM.Service.Library.Controllers;
using RSM.Support;
using RSM.Support.S2;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;
using System.Xml;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Configuration;
using System.Web.Security;


namespace RSM.Controllers
{
    public class JCLRuleController : Controller
    {
        public class GridDataRow
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }

        }

        //
        // GET: /JCLRule/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /JCLRule/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /JCLRule/Create
        [Authorize]
        public ActionResult Create()
        {
            JCLRoleRule rule = new JCLRoleRule();
            RSMDataModelDataContext context = new RSMDataModelDataContext();
            rule.JobCode = "0";
            rule.DeptID = "0";
          
            ViewBag.Model = rule;

            List<Job> jobs;
            //bool jobCodesFirst = bool.Parse(ConfigurationManager.AppSettings["JobCodesFirst"]);
            var jobCodesFirst = Service.Library.Controllers.Settings.GetValueAsBool("R1SM", "JobCodesFirst");
            if (jobCodesFirst)
            {
                jobs = (from j in context.Jobs
                        select j).OrderBy("JobCode ASC").ToList();
            }
            else
            {
                jobs = (from j in context.Jobs
                        select j).OrderBy("JobDescription ASC").ToList();
            }

            ViewBag.JobCodesFirst = jobCodesFirst;
            ViewBag.Jobs = jobs;


            List<Department> depts = (from d in context.Departments 
                                      select d).OrderBy("DeptDescr ASC").ToList();
            ViewBag.Depts = depts;
            
           
            List<Location> locs = (from l in context.Locations 
                                   select l).OrderBy("LocationName ASC").ToList();
            ViewBag.Locs = locs;
              
       
            ViewBag.AvailRolesURL = Url.Action("AvailableRoles", new { ID = 0 });
            ViewBag.AssRolesURL = Url.Action("AssignedRoles", new { ID = 0 });


            return View(rule);
        } 

        //
        // POST: /JCLRule/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                string jsonGridData = collection["gridData"];
                List<GridDataRow> gridData = JsonConvert.DeserializeObject<List<GridDataRow>>(jsonGridData);

                UpdateRule(0,
                           collection["JobCode"],
                           collection["DeptID"],
                           int.Parse(collection["Location"]),
                           gridData);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: /JCLRule/Clone/5
        [Authorize]
        public ActionResult Clone(int id)
        {
            RSMDataModelDataContext context = new RSMDataModelDataContext();

            JCLRoleRule source = (from j in context.JCLRoleRules
                                 where j.ID == id
                                 select j).Single();
            
            JCLRoleRule dest = new JCLRoleRule();
            JCLRole newRole;

            dest.JobCode = source.JobCode;
            dest.DeptID = source.DeptID;
            dest.Location = source.Location;

            context.JCLRoleRules.InsertOnSubmit(dest);
            context.SubmitChanges();


            foreach(JCLRole role in source.JCLRoles )
            {
                newRole = new JCLRole();
                newRole.RoleID = role.RoleID;
                newRole.JCLRoleRule = dest;
                context.JCLRoles.InsertOnSubmit(newRole);
            }

            context.SubmitChanges();

            return RedirectToAction("Edit", new { id = dest.ID, message = "You are now editing a new copy of your previous rule." });
        }

        //
        // GET: /JCLRule/Edit/5
        [Authorize]
        public ActionResult Edit(int id, string message = null)
        {
            RSMDataModelDataContext context = new RSMDataModelDataContext();

            JCLRoleRule model = (from j in context.JCLRoleRules
                                 where j.ID == id
                                 select j).Single();
            ViewBag.Model = model;

            List<Job> jobs;
            //bool jobCodesFirst = bool.Parse(ConfigurationManager.AppSettings["JobCodesFirst"]);
            var jobCodesFirst = Service.Library.Controllers.Settings.GetValueAsBool("R1SM", "JobCodesFirst");
            if (jobCodesFirst)
            {
                jobs = (from j in context.Jobs
                        select j).OrderBy("JobCode ASC").ToList();
            }
            else
            {
                jobs = (from j in context.Jobs
                        select j).OrderBy("JobDescription ASC").ToList();
            }

            ViewBag.JobCodesFirst = jobCodesFirst;
            ViewBag.Jobs = jobs;

            List<Department> depts = (from d in context.Departments 
                                      select d).OrderBy("DeptDescr ASC").ToList();
            ViewBag.Depts = depts;
            
           
            List<Location> locs = (from l in context.Locations 
                                   select l).OrderBy("LocationName ASC").ToList();
            ViewBag.Locs = locs;
            ViewBag.Message = message;
       
            ViewBag.AvailRolesURL = Url.Action("AvailableRoles", new { ID = id });
            ViewBag.AssRolesURL = Url.Action("AssignedRoles", new { ID = id });
            
            return View(model);
        }

        //
        // POST: /JCLRule/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                string jsonGridData = collection["gridData"];
                List<GridDataRow> gridData = JsonConvert.DeserializeObject<List<GridDataRow>>(jsonGridData);
                
                UpdateRule(id, 
                           collection["JobCode"], 
                           collection["DeptID"], 
                           int.Parse(collection["Location"]),
                           gridData);
                
               
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        int UpdateRule(int id, string jobCode, string deptID, int location, List<GridDataRow> roles)
        {
            JCLRoleRule rule;
            var context = new RSMDataModelDataContext();
            var action = "modified";

            if (id > 0)
            {
                rule = (from r in context.JCLRoleRules
                        where r.ID == id
                        select r).Single();
            }
            else
            {
                action = "created";
                rule = new JCLRoleRule();
                context.JCLRoleRules.InsertOnSubmit(rule);
            }

            rule.JobCode = jobCode;
            rule.DeptID = deptID;
            rule.Location = location;

            var thisRuleRoles = from r in context.JCLRoles
                                where r.RuleID == id
                                select r;
            if (thisRuleRoles.Any())
            {
                context.JCLRoles.DeleteAllOnSubmit(thisRuleRoles);
            }

            context.SubmitChanges();

            foreach (var row in roles)
            {
                var role = new JCLRole {RoleID = int.Parse(row.ID), RuleID = id};

                rule.JCLRoles.Add(role);
            }

            context.SubmitChanges();

            //bool reqApproval = bool.Parse( ConfigurationManager.AppSettings["RequireAccessApproval"]);
            var reqApproval = Settings.GetValueAsBool("R1SM", "RequireAccessApproval");
            
            var engine = new RoleAssignmentEngine(context);
            var count = engine.FlagPeopleWithRule(rule.ID, reqApproval);

            //context.Syslog(RSMDataModelDataContext.LogSources.USER,
            //               RSMDataModelDataContext.LogSeverity.INFO,
            //               string.Format("{0} {1} rule for {2}, {3}, {4}",
            //                             User.Identity.Name,
            //                             action,
            //                             rule.Job.DisplayName,
            //                             rule.Departments.DisplayName,
            //                             rule.Locations.LocationName), 
            //               string.Format("The {0} associates affected by last rule edit will be processed by the rule engine next pass.", count));

            var logger = new Logger();
            logger.LogUserActivity(Severity.Informational,
               string.Format("{0} {1} rule for {2}, {3}, {4}",
                             User.Identity.Name,
                             action,
                             rule.Job.DisplayName,
                             rule.Departments.DisplayName,
                             rule.Locations.LocationName),
               string.Format("The {0} associates affected by last rule edit will be processed by the rule engine next pass.", count));

            return rule.ID;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var context = new RSMDataModelDataContext();
                var engine = new RoleAssignmentEngine(context);

                var reqApproval = Settings.GetValueAsBool("R1SM", "RequireAccessApproval");

                var count = engine.FlagPeopleWithRule(id, reqApproval);

                context.DeleteRule(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public ActionResult Search(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            var context = new RSMDataModelDataContext();
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            
            List<JCLRuleDisplay> displayResults;
            List<JCLRuleDisplay> results;
            string sortBy = sidx;
            int totalRecords;
            int totalPages;

            switch (sidx)
            {
                case "Position":
                    sortBy = "JobCode";
                    break;
                case "Department":
                    sortBy = "DeptDescr";
                    break;
                case "Facility":
                    sortBy = "LocationName";
                    break;
            }

            // Santize the search string to prevent searches that could inject SQL.
            if (searchString != null)
                searchString = searchString.Replace("'", "''");

            if (_search)
            {
                switch (searchField)
                {
                    case "Position":
                        displayResults = context.JCLRuleDisplays.Where(p => p.JobDescription.Contains(searchString)).OrderBy(sortBy + " " + sord).ToList();
                        //results = context.JCLRoleRules.Where(p => p.Job.JobDescription.Contains(searchString)).OrderBy(sortBy + " " + sord).ToList();
                        break;

                    case "Department":
                        displayResults = context.JCLRuleDisplays.Where(p => p.DeptDescr.Contains(searchString)).OrderBy(sortBy + " " + sord).ToList();
                        //results = context.JCLRoleRules.Where(p => p.Departments.DeptDescr.Contains(searchString)).OrderBy(sortBy + " " + sord).ToList();
                        break;

                    case "Facility":
                        displayResults = context.JCLRuleDisplays.Where(p => p.LocationName.Contains(searchString)).OrderBy(sortBy + " " + sord).ToList();
                        //results = context.JCLRoleRules.Where(p => p.Locations.LocationName.Contains(searchString)).OrderBy(sortBy + " " + sord).ToList();
                        break;
                    
                    default:
                        displayResults = context.JCLRuleDisplays.ToList();
                        //results = context.JCLRoleRules.ToList();
                        break;

                };
                totalRecords = displayResults.Count;
                results = displayResults.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            else
            {
                displayResults = context.JCLRuleDisplays.OrderBy(sortBy + " " + sord).ToList();
                //results = context.JCLRoleRules.ToList();
                totalRecords = displayResults.Count;
                results = displayResults.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                
            }

            if (totalRecords > 0)
                totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            else
                totalPages = 1;

            // colNames: ['Status', 'Name', 'BadgeNumber', 'Department', 'Job', 'Facility', 'LastUpdated'],
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (


                    from rule in results
                    select new
                    {
                        i = rule.ID,
                        cell = new string[] {
                            rule.ID.ToString(), 
                            ((rule.JobCode == "0") ? rule.JobDescription : string.Format("{0} ({1})", rule.JobDescription, rule.JobCode)),
                            rule.DeptDescr, rule.LocationName
                            
                        }
                    }).ToArray()

            };

            return Json(jsonData);
        }

        [Authorize]
        public ActionResult AvailableRoles(int id)
        {
            var context = new RSMDataModelDataContext();

            var AvailRoles = context.RolesAvialableForRule(id);
            var jsonAvailRoles = new
            {
                total = 1,
                page = 1,
                records = 1,
                rows = (
                    from r in AvailRoles
                    select new
                    {
                        i = r.RoleID,
                        cell = new string[] {
                            r.RoleID.ToString(), r.RoleName.ToString(), r.RoleDesc.ToString()

                        }
                    }).ToArray()

            };

            return Json(jsonAvailRoles);
        }

        [Authorize]
        public ActionResult AssignedRoles(int id)
        {
            if (id == 0)
                return Json("");

            var context = new RSMDataModelDataContext();
            JCLRoleRule rule = (from r in context.JCLRoleRules
                                where r.ID == id
                                select r).Single();

            var jsonAssLevels = new
            {
                total = 1,
                page = 1,
                records = rule.JCLRoles.Count,  
                rows = (
                    from r in rule.JCLRoles
                    select new
                    {
                        i = r.ID,
                        cell = new string[] {
                            r.RoleID.ToString(), r.Role.RoleName, r.Role.RoleDesc.ToString()

                        }
                    }).ToArray()

            };

            return Json(jsonAssLevels);
        }
    }
}
