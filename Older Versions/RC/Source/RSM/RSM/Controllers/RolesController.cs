using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using RSM.Support;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;


namespace RSM.Controllers
{
    public class RolesController : Controller
    {
        public class GridDataRow
        {
            public string ID {get; set;}
            public string Name { get; set; }
            public string Description { get; set; }

        }

        //
        // GET: /Roles/
        [Authorize]
        public ActionResult Index()
        {

            ViewBag.IsAdmin = false;
            if (User.IsInRole("admin") || User.Identity.Name == "admin")
            {
                ViewBag.IsAdmin = true;
            }
            return View();
        }

        //
        // GET: /Roles/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {

            var context = new RSMDataModelDataContext();
            Role role = (from p in context.Roles
                         where p.RoleID  == id
                         select p).First();
            ViewBag.IsAdmin = false;
            ViewBag.AssLevelsURL = Url.Action("AssignedLevels", new { ID = role.RoleID });
            ViewBag.Role = role;
            if (User.IsInRole("admin") || User.Identity.Name == "admin")
            {
                ViewBag.IsAdmin = true;
            }
            return View(role);
        }

        //
        // GET: /Roles/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            Role role = new Role();
            ViewBag.Role = role;
            ViewBag.IsAdmin = false;
            if (User.IsInRole("admin") || User.Identity.Name == "admin")
            {
                ViewBag.IsAdmin = true;
            }
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult RwV()
        {
            Role role = new Role();
            ViewBag.Role = role;
            ViewBag.IsAdmin = false;
            if (User.IsInRole("admin") || User.Identity.Name == "admin")
            {
                ViewBag.IsAdmin = true;
            }
            return View();
        }

        public ActionResult RoleList(string sidx, string sord, int page, int rows)
        {
            var context = new RSMDataModelDataContext();
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = context.Roles.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            string sortBy = sidx;

            switch (sidx)
            {
                case "Description":
                    sortBy = "RoleDesc";
                    break;
                case "Name":
                    sortBy = "RoleName";
                    break;

            }


            var roles = context.Roles
                            .OrderBy(sortBy + " " + sord)
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize);


            // colNames: ['Status', 'Name', 'BadgeNumber', 'Department', 'Job', 'Facility', 'LastUpdated'],
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (
                    from role in roles
                    select new
                    {
                        i = role.RoleID,
                        cell = new string[] {
                            role.RoleID.ToString(), role.RoleName.ToString(), role.RoleDesc.ToString()
                        }
                    }).ToArray()

            };

            return Json(jsonData);
        }

        //
        // POST: /Roles/Create

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                
                string jsonGridData = collection["gridData"];
                List<GridDataRow> gridData = JsonConvert.DeserializeObject<List<GridDataRow>>(jsonGridData);

                UpdateRole(0, collection["RoleName"], collection["RoleDesc"], gridData);


                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
     

        public ActionResult AvailableLevels(int id)
        {

            var context = new RSMDataModelDataContext();
          

            var AvailLevels = context.LevelsAvailableForRole(id);
            var jsonAvailLevels = new
            {
                total = 1,
                page = 1,
                records = 1,
                rows = (
                    from l in AvailLevels
                    select new
                    {
                        i = l.AccessLevelID,
                        cell = new string[] {
                            l.AccessLevelID.ToString(), l.AccessLevelName.ToString(), l.AccessLevelDesc.ToString()

                        }
                    }).ToArray()

            };

            return Json(jsonAvailLevels);
        }

        public ActionResult AssignedLevels(int id)
        {
            if (id == 0)
                return Json("");

            var context = new RSMDataModelDataContext();
            Role role = (from p in context.Roles
                         where p.RoleID == id
                         select p).First();


            var jsonAssLevels = new
            {
                total = 1,
                page = 1,
                records = role.AccessLevelRoles.Count,
                rows = (
                    from l in role.AccessLevelRoles
                    select new
                    {
                        i = l.AccessLevelID,
                        cell = new string[] {
                            l.AccessLevelID.ToString(), l.AccessLevel.AccessLevelName.ToString(), l.AccessLevel.AccessLevelDesc.ToString()

                        }
                    }).ToArray()

            };

            return Json(jsonAssLevels);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id)
        {
            var context = new RSMDataModelDataContext();
            Role role = (from p in context.Roles
                         where p.RoleID == id
                         select p).First();

            ViewBag.AvailLevelsURL = Url.Action("AvailableLevels", new { ID = role.RoleID });
            ViewBag.AssLevelsURL = Url.Action("AssignedLevels", new { ID = role.RoleID });    
            ViewBag.Role = role;
            ViewBag.IsAdmin = false;
            if (User.IsInRole("admin") || User.Identity.Name == "admin")
            {
                ViewBag.IsAdmin = true;
            }
            return View(role);
        }

        //
        // POST: /Roles/Edit/5

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                string jsonGridData = collection["gridData"];
                List<GridDataRow> gridData = JsonConvert.DeserializeObject<List<GridDataRow>>(jsonGridData);

                UpdateRole(id, collection["RoleName"], collection["Description"], gridData);


                return RedirectToAction("Index");
            }
            catch
            {

                return RedirectToAction("Edit", new { ID=id});
            }
        }

        void UpdateRole(int id, string name, string description, List<GridDataRow> levels)
        {
            Role role;
            var context = new RSMDataModelDataContext();
            string action = "modified";
            if (description == null) 
            {
                description = "";
            }

            if (id > 0)
            {
                role = (from p in context.Roles
                        where p.RoleID == id
                        select p).Single();
            }
            else
            {
                action = "created";
                role = new Role();
                context.Roles.InsertOnSubmit(role);
            }

            role.RoleName = name;
            role.RoleDesc = description;

            var thisRoleLevels = from l in context.AccessLevelRoles
                         where l.RoleID == role.RoleID
                         select l;
            if (thisRoleLevels.Count() > 0)
            {
                context.AccessLevelRoles.DeleteAllOnSubmit(thisRoleLevels);
            }

            context.SubmitChanges();

            foreach( GridDataRow row in levels)
            {
                AccessLevelRole alr = new AccessLevelRole {AccessLevelID = int.Parse(row.ID), RoleID = role.RoleID};

                role.AccessLevelRoles.Add(alr);
            }

            foreach (var p in context.PeopleWithRole(id))
            {
                p.NeedsRulePass = true;
                p.NeedsUpload = true;
            }

            context.SubmitChanges();

            

            context.Syslog(RSMDataModelDataContext.LogSources.USER,
                           RSMDataModelDataContext.LogSeverity.INFO,
                           string.Format("{0} {1} role {2}", User.Identity.Name, action, role.RoleName), "");
        }

        //
        // GET: /Roles/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id)
        {
            ViewBag.IsAdmin = false;
            if (User.IsInRole("admin") || User.Identity.Name == "admin")
            {
                ViewBag.IsAdmin = true;
            }
            return View();
        }

        //
        // POST: /Roles/Delete/5

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                RSMDataModelDataContext context = new RSMDataModelDataContext();
                context.DeleteRole(id);
                foreach (var p in context.PeopleWithRole(id))
                {
                    p.NeedsRulePass = true;
                    p.NeedsUpload = true;
                }

                context.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
