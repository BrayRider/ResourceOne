using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
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
using RSM.Models;

namespace RSM.Controllers
{
    public class PeopleController : Controller
    {
        public enum ReviewModes
        {
            NONE,
            HIRE,
            FIRE,
            CHANGE

        }
        S2API _api;

        S2API API
        {
            get {
                if(_api == null)
                {
                   
                    string s2Host = ConfigurationManager.AppSettings["S2Address"];
                    _api = new S2API(String.Format("{0}/goforms/nbapi", s2Host),
                                     ConfigurationManager.AppSettings["S2RSMAccountName"],
                                     ConfigurationManager.AppSettings["S2RSMAccountPassword"]);
                }

                return _api;
            }
        }

        void UpdateLastAccess()
        {
            MembershipUser u = Membership.GetUser();
            
            u.LastActivityDate = DateTime.Now;
            Membership.UpdateUser(u);
        }

        public class GridDataRow
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Exception { get; set; }

        }
        //
        // GET: /People/
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.IsAdmin = false;
            if (User.IsInRole("admin") || User.Identity.Name == "admin")
            {
                ViewBag.IsAdmin = true;
            }
            ViewBag.IsReview = false;
            ViewBag.ReviewMode = ReviewModes.NONE;

            ViewBag.PageSize = (Request.Cookies["EmployeePageSize"] != null) ? int.Parse(Request.Cookies["EmployeePageSize"].Value) : 50;

            return View();
        }

        [Authorize]
        public ActionResult Review(int ID)
        {
            return DisplayOrReview(ID, ReviewModes.CHANGE, "ReviewQueue", "Review");   
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Review(int id, FormCollection collection)
        {
            return ApproveAccess(id, collection, "ReviewQueue");
        }

        [Authorize]
        public ActionResult ReviewHire(int ID)
        {
            return DisplayOrReview(ID, ReviewModes.HIRE, "NewHires", "ReviewHire");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ReviewHire(int id, FormCollection collection)
        {
            return ApproveAccess(id, collection, "NewHires");
        }

        [Authorize]
        public ActionResult ReviewTerm(int ID)
        {
            return DisplayOrReview(ID, ReviewModes.FIRE, "NewTerms", "ReviewTerm");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ReviewTerm(int id, FormCollection collection)
        {
            return ApproveAccess(id, collection, "NewTerms");
        }
        
        ActionResult ApproveAccess(int id, FormCollection collection, string returnView)
        {
            // The submit button on this form is the "approve access" button
            // So we need to grab the person and submit them to the security hardware.

            var context = new RSMDataModelDataContext();
            Person person = (from p in context.Persons
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
                context.Syslog(RSMDataModelDataContext.LogSources.S2EXPORT,
                              RSMDataModelDataContext.LogSeverity.ERROR,
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
                context.Syslog(RSMDataModelDataContext.LogSources.S2EXPORT,
                               RSMDataModelDataContext.LogSeverity.ERROR,
                               "Error exporting " + person.DisplayName + " to S2.",
                               e.ToString());

                RedirectToAction("Index", "People", new { ID = id } );
            }

            return RedirectToAction(returnView);
        }


        [Authorize]
        public ActionResult Details(int ID)
        {
            return DisplayOrReview(ID, ReviewModes.NONE, "Index", "Details");

        }

        ActionResult DisplayOrReview(int ID, ReviewModes reviewMode, string returnView, string thisView)
        {
            UpdateLastAccess();
            var context = new RSMDataModelDataContext();
            Person person = (from p in context.Persons
                             where p.PersonID == ID
                             select p).Single();

            string s2Host = ConfigurationManager.AppSettings["S2Address"];
            try
            {
                UpdateLastAccess();
            }
            catch (Exception)
            {
                // Failure to update last access should not be an show stopper
            }

            try
            {

                XmlNode nd = API.GetPicture(person.PersonID.ToString());
                string fileName = string.Format("~/Content/Images/employees/{0}", nd["PICTUREURL"].InnerText);
                byte[] imageData = Convert.FromBase64String(nd["PICTURE"].InnerText);

                string filePath = Server.MapPath(Url.Content(fileName));

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(imageData);
                        writer.Close();
                    }
                }

                ViewBag.PicURL = Url.Content(fileName);

            }
            catch (Exception)
            {
                string userImage = string.Format("~/Content/images/employees/missing.jpg", ID);
                ViewBag.PicURL = Url.Content(userImage);
            }

            ViewBag.AssRolesURL = Url.Action("AssignedRoles", new { ID = person.PersonID });
            ViewBag.IsAdmin = false;
            ViewBag.ReviewMode = reviewMode;
            ViewBag.ReturnView = returnView;
            if (User.IsInRole("admin") || User.Identity.Name == "admin")
            {
                ViewBag.IsAdmin = true;
            }

            ViewBag.IsReview = (reviewMode != ReviewModes.NONE);

            ViewBag.BackView = thisView;
            
            return View("Details", person);
            
        }

        ActionResult Edit(int ID, bool fromReview, string back)
        {
            string s2Host = ConfigurationManager.AppSettings["S2Address"];
            var context = new RSMDataModelDataContext();
            Person person = (from p in context.Persons
                             where p.PersonID == ID
                             select p).First();

            try
            {
                UpdateLastAccess();
            }
            catch (Exception)
            {
                // Failure to update last access should not be a show stopper
            }

            try
            {
                //S2API api = new S2API(String.Format("{0}/goforms/nbapi", s2Host));
                XmlNode nd = API.GetPicture(person.PersonID.ToString());
                string fileName = string.Format("~/Content/Images/employees/{0}", nd["PICTUREURL"].InnerText);
                byte[] imageData = Convert.FromBase64String(nd["PICTURE"].InnerText);

                string filePath = Server.MapPath(Url.Content(fileName));

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(imageData);
                        writer.Close();
                    }
                }

                ViewBag.PicURL = Url.Content(fileName);
            }
            catch (Exception)
            {
                string userImage = string.Format("~/Content/images/employees/missing.jpg", ID);
                ViewBag.PicURL = Url.Content(userImage);
            }



            ViewBag.AvailRolesURL = Url.Action("AvailableRoles", new { ID = person.PersonID });
            ViewBag.AssRolesURL = Url.Action("AssignedRoles", new { ID = person.PersonID });
            ViewBag.IsAdmin = false;
            if (User.IsInRole("admin") || User.Identity.Name == "admin")
            {
                ViewBag.IsAdmin = true;
            }

            ViewBag.BackView = back;
            ViewBag.IsReview = fromReview;
            return View( "Edit", person);

        }


        [Authorize]
        public ActionResult Edit(int ID, string back)
        {
            return Edit(ID, false, back);
        }

        [Authorize]
        public ActionResult ReviewEdit(int ID)
        {
            return Edit(ID, true, "Index");

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


        void ProcessEditForm(int id, FormCollection collection)
        {
            bool allowUpload = bool.Parse(ConfigurationManager.AppSettings["S2AllowExport"]);

            string jsonGridData = collection["gridData"];
            List<GridDataRow> gridData = JsonConvert.DeserializeObject<List<GridDataRow>>(jsonGridData);
            
            var context = new RSMDataModelDataContext();
            Person person = (from p in context.Persons
                                 where p.PersonID == id
                                 select p).First();
            // Update the roles
            UpdateRoles(person, gridData);
            
            // If an admin edited this we need to see if any of the RSM specific stuff has changed.
            if (User.IsInRole("admin"))
            {
                
                
                string check = collection["IsAdmin"];
                if (check.Contains(","))
                {
                    person.IsAdmin = bool.Parse(check.Split(',')[0]);
                }

                check = collection["LockedOut"];
                if (check.Contains(","))
                {
                    person.LockedOut = bool.Parse(check.Split(',')[0]);
                }

                
                person.username = collection["username"];
                string newPass = collection["password"];
                if ((newPass.Length > 0) && (newPass != person.password))
                {
                    MachineKeySection machineKey;
                    // Get encryption and decryption key information from the configuration.
                    Configuration cfg =
                      WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                    machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

                    HMACSHA512 hash = new HMACSHA512();
                    hash.Key = HexToByte(machineKey.ValidationKey);

                    string hash1 = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(collection["password"] + ".rSmSa1t" + collection["password"].Length.ToString())));
                    string hash2 = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(hash1 + "an0tH3r5alt!" + collection["password"].Length.ToString())));
                    person.password =
                       Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(hash2)));
                }

                context.Syslog(RSMDataModelDataContext.LogSources.USER,
                               RSMDataModelDataContext.LogSeverity.INFO,
                               User.Identity.Name + " modified access for " + person.DisplayName, "");

                
            }

            // Saving the person implies acceptance of the levels as assigned.
            person.NeedsApproval = false;
            person.Credentials = collection["Credentials"];
            person.NickFirst = collection["NickFirst"];
            context.SubmitChanges();

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
                    context.SubmitChanges();
                }
            }
            catch
            {
                // If the update fails (likely due to a network issue)
                // queue up the person to be uploaded by the service later.
                person.NeedsUpload  = true;
                context.SubmitChanges();
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

        [Authorize]
        public ActionResult AvailableRoles(int id)
        {

            var context = new RSMDataModelDataContext();


            var AvailRoles = context.RolesAvialableForPerson(id);
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
            var context = new RSMDataModelDataContext();
            Person person = (from p in context.Persons
                             where p.PersonID == id
                             select p).First();

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
                        cell = new string[] {
                            r.RoleID.ToString(), r.Role.RoleName.ToString(), r.Role.RoleDesc.ToString(), ((r.IsException) ? 1 : 0).ToString()
                        }
                    }).ToArray()

            };

            return Json(jsonAssLevels);
        }

        [Authorize]
        public ActionResult EmployeeListSearch(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            var context = new RSMDataModelDataContext();
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            Response.Cookies.Add(new HttpCookie("EmployeePageSize", rows.ToString()));
            List<Person> results;
            List<Person> allResults;
            string sortBy = sidx;
            int totalRecords;
            int totalPages;


            switch (sidx)
            {
                case "Department":
                    sortBy = "DeptDescr";
                    break;
                case "Name":
                    sortBy = "LastName";
                    break;
                case "Job":
                    sortBy = "JobDescr";
                    break;
                case "Status":
                    sortBy = "Active";
                    break;

            }

            // Santize the search string to prevent searches that could inject SQL.
            if(searchString != null)
                searchString = searchString.Replace("'", "''");

            if (_search)
            {
                switch (searchField)
                {
                    case "Name":
                        allResults = context.Persons.Where(p => (p.LastName.Contains(searchString) || p.FirstName.Contains(searchString)))
                                                    .OrderBy(sortBy + " " + sord)
                                                    .ToList();
                        break;
                    case "Department":
                        allResults = context.Persons.Where(p => p.DeptDescr.Contains(searchString))
                                                    .OrderBy(sortBy + " " + sord)
                                                    .ToList();
                        break;
                    case "Position":
                        allResults = context.Persons.Where(p => p.JobDescr.Contains(searchString))
                                                    .OrderBy(sortBy + " " + sord)
                                                    .ToList();
                        break;
                    case "Job":
                        allResults = context.Persons.Where(p => p.JobDescr.Contains(searchString))
                                                    .OrderBy(sortBy + " " + sord)
                                                    .ToList();
                        break;
                    case "Facility":
                        allResults = context.Persons.Where(p => p.Facility.Contains(searchString))
                                                    .OrderBy(sortBy + " " + sord)
                                                    .ToList();
                        break;
                    default:
                        allResults = context.Persons.OrderBy(sortBy + " " + sord)
                                                    .ToList();
                        break;
                };

         
               
            }
            else
            {
                allResults = context.Persons.OrderBy(sortBy + " " + sord)
                                            .ToList();
            }

            totalRecords = allResults.Count();
            results = allResults.Skip(pageIndex * pageSize).Take(pageSize).ToList();


            if (totalRecords > 0)
                totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            else
                totalPages = 0;


            // colNames: ['Status', 'Name', 'BadgeNumber', 'Department', 'Job', 'Facility', 'LastUpdated'],
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (


                    from emp in results
                    select new
                    {
                        i = emp.PersonID,
                        cell = new string[] {
                            emp.Active.ToString(), emp.LastName.ToString() + ", " + emp.FirstName.ToString() + " " + emp.MiddleName.ToString(),
                            emp.BadgeNumber.ToString(), emp.DeptDescr.ToString(), emp.JobDescr.ToString(), emp.Facility.ToString(), emp.LastUpdated.ToString("d"),
                            emp.LastUpdateMask.ToString(), emp.PersonID.ToString()

                        }
                    }).ToArray()

            };

            return Json(jsonData);
        }

       
        

        [Authorize]
        public ActionResult ChangedEmployeesSearch(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            var context = new RSMDataModelDataContext();
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            List<EmpWithNonStatusChange> allResults; // All the data
            List<EmpWithNonStatusChange> results;    // The data after .skip and .take
            string sortBy = sidx;
            int totalRecords;
            int totalPages;
            Response.Cookies.Add(new HttpCookie("EmployeePageSize", rows.ToString()));

            // The grid sends display names not real column names
            switch (sidx)
            {
                case "Department":
                    sortBy = "DeptDescr";
                    break;
                case "Name":
                    sortBy = "LastName";
                    break;
                case "Job":
                    sortBy = "JobDescr";
                    break;
                case "Status":
                    sortBy = "Active";
                    break;

            }


            if (_search)
            {
                switch (searchField)
                {
                    case "Name":
                        allResults = context.EmpWithNonStatusChanges.Where(p => (p.LastName.Contains(searchString) || p.FirstName.Contains(searchString)))
                                                                    .OrderBy(sortBy + " " + sord)
                                                                    .ToList();
                        break;
                    case "Department":
                        allResults = context.EmpWithNonStatusChanges.Where(p => p.DeptDescr.Contains(searchString))
                                                                    .OrderBy(sortBy + " " + sord)
                                                                    .ToList();
                        break;
                    case "Position":
                    case "Job":
                        allResults = context.EmpWithNonStatusChanges.Where(p => p.JobDescr.Contains(searchString))
                                                                    .OrderBy(sortBy + " " + sord)
                                                                    .ToList();
                        break;
                  
                    default:
                        allResults = context.EmpWithNonStatusChanges.OrderBy(sortBy + " " + sord)
                                                                    .ToList();
                        break;
                };

               
              
            }
            else
            {
                allResults = context.EmpWithNonStatusChanges
                                    .OrderBy(sortBy + " " + sord)
                                    .Skip(pageIndex * pageSize)
                                    .ToList();
            }

            totalRecords = allResults.Count();
            results = allResults.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            
            if (totalRecords > 0)
                totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            else
                totalPages = 0;



            // colNames: ['Status', 'Name', 'BadgeNumber', 'Department', 'Job', 'Facility', 'LastUpdated'],
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (


                    from emp in results
                    select new
                    {
                        i = emp.PersonID,
                        cell = new string[] {
                            emp.Active.ToString(), emp.LastName.ToString() + ", " + emp.FirstName.ToString() + " " + emp.MiddleName.ToString(),
                            emp.BadgeNumber.ToString(), emp.DeptDescr.ToString(), emp.JobDescr.ToString(), emp.Facility.ToString(), emp.LastUpdated.Value.ToString("d"),
                            emp.LastUpdateMask.ToString(), emp.PersonID.ToString()

                        }
                    }).ToArray()

            };

            return Json(jsonData);
        }


        [Authorize]
        public ActionResult NewEmployeesSearch(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            var context = new RSMDataModelDataContext();
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;

            List<NewHire> results;
            List<NewHire> allResults;
            string sortBy = sidx;
            int totalRecords;
            int totalPages;

            Response.Cookies.Add(new HttpCookie("EmployeePageSize", rows.ToString()));

            switch (sidx)
            {
                case "Department":
                    sortBy = "DeptDescr";
                    break;
                case "Name":
                    sortBy = "LastName";
                    break;
                case "Job":
                    sortBy = "JobDescr";
                    break;
                case "Status":
                    sortBy = "Active";
                    break;

            }


            if (_search)
            {
                switch (searchField)
                {
                    case "Name":
                        allResults = context.NewHires.Where(p => (p.LastName.Contains(searchString) || p.FirstName.Contains(searchString)))
                                                     .OrderBy(sortBy + " " + sord)
                                                     .ToList();
                        break;
                    case "Department":
                        allResults = context.NewHires.Where(p => p.DeptDescr.Contains(searchString))
                                                     .OrderBy(sortBy + " " + sord)
                                                     .ToList();
                        break;
                    case "Position":
                    case "Job":
                        allResults = context.NewHires.Where(p => p.JobDescr.Contains(searchString))
                                                     .OrderBy(sortBy + " " + sord)
                                                     .ToList();
                        break;
                    
                    default:
                        allResults = context.NewHires.OrderBy(sortBy + " " + sord)
                                                     .ToList();
                        break;
                };



            }
            else
            {
                allResults = context.NewHires.OrderBy(sortBy + " " + sord)
                                             .ToList();
            }

            totalRecords = allResults.Count();
            results = allResults.Skip(pageIndex * pageSize).Take(pageSize).ToList();


            if (totalRecords > 0)
                totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            else
                totalPages = 0;


            // colNames: ['Status', 'Name', 'BadgeNumber', 'Department', 'Job', 'Facility', 'LastUpdated'],
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (


                    from emp in results
                    select new
                    {
                        i = emp.PersonID,
                        cell = new string[] {
                            emp.Active.ToString(), emp.LastName.ToString() + ", " + emp.FirstName.ToString() + " " + emp.MiddleName.ToString(),
                            emp.BadgeNumber.ToString(), emp.DeptDescr.ToString(), emp.JobDescr.ToString(), emp.Facility.ToString(), emp.LastUpdated.Value.ToString("d"),
                            emp.LastUpdateMask.ToString(), emp.PersonID.ToString()

                        }
                    }).ToArray()

            };

            return Json(jsonData);
        }

        [Authorize]
        public ActionResult TermEmployeesSearch(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            var context = new RSMDataModelDataContext();
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            List<NewFire> results;
            List<NewFire> allResults;
            string sortBy = sidx;
            int totalRecords;
            int totalPages;

            Response.Cookies.Add(new HttpCookie("EmployeePageSize", rows.ToString()));

            switch (sidx)
            {
                case "Department":
                    sortBy = "DeptDescr";
                    break;
                case "Name":
                    sortBy = "LastName";
                    break;
                case "Job":
                    sortBy = "JobDescr";
                    break;
                case "Status":
                    sortBy = "Active";
                    break;

            }
            if (_search)
            {
                switch (searchField)
                {
                    case "Name":
                        allResults = context.NewFires.Where(p => (p.LastName.Contains(searchString) || p.FirstName.Contains(searchString)))
                                                     .OrderBy(sortBy + " " + sord)
                                                     .ToList();
                        break;
                    case "Department":
                        allResults = context.NewFires.Where(p => p.DeptDescr.Contains(searchString))
                                                     .OrderBy(sortBy + " " + sord)
                                                     .ToList();
                        break;
                    case "Position":
                    case "Job":
                        allResults = context.NewFires.Where(p => p.JobDescr.Contains(searchString))
                                                     .OrderBy(sortBy + " " + sord)
                                                     .ToList();
                        break;
                   
                    default:
                        allResults = context.NewFires.OrderBy(sortBy + " " + sord)
                                                     .ToList();
                        break;
                };



            }
            else
            {
                allResults = context.NewFires.OrderBy(sortBy + " " + sord)
                                             .ToList();
            }

            
            totalRecords = allResults.Count();
            results = allResults.Skip(pageIndex * pageSize).Take(pageSize).ToList();


            if (totalRecords > 0)
                totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            else
                totalPages = 0;

            // colNames: ['Status', 'Name', 'BadgeNumber', 'Department', 'Job', 'Facility', 'LastUpdated'],
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = (


                    from emp in results
                    select new
                    {
                        i = emp.PersonID,
                        cell = new string[] {
                            emp.Active.ToString(), emp.LastName.ToString() + ", " + emp.FirstName.ToString() + " " + emp.MiddleName.ToString(),
                            emp.BadgeNumber.ToString(), emp.DeptDescr.ToString(), emp.JobDescr.ToString(), emp.Facility.ToString(), emp.LastUpdated.Value.ToString("d"),
                            emp.LastUpdateMask.ToString(), emp.PersonID.ToString()

                        }
                    }).ToArray()

            };

            return Json(jsonData);
        }

        void UpdateRoles(Person person, List<GridDataRow> roles)
        {
            var context = new RSMDataModelDataContext();
      
            var thisPersonRoles = from r in context.PeopleRoles 
                                  where r.PersonID  == person.PersonID
                                  select r;
            if (thisPersonRoles.Count() > 0)
            {
                context.PeopleRoles.DeleteAllOnSubmit(thisPersonRoles);
            }

            context.SubmitChanges();

            PeopleRole role;

            
            foreach (GridDataRow row in roles)
            {
                role = new PeopleRole();
                role.RoleID  = int.Parse(row.ID);
                role.PersonID = person.PersonID;
                role.IsException = (row.Exception != "0");

                person.PeopleRoles.Add(role);
            }



            context.SubmitChanges();
        }

        [Authorize]
        public ActionResult ReviewQueue()
        {
            ViewBag.IsAdmin = false;
            if (User.IsInRole("admin") || User.Identity.Name == "admin")
            {
                ViewBag.IsAdmin = true;
            }
            ViewBag.GridDataURL = Url.Action("ChangedEmployeesSearch", "People");
            ViewBag.ReviewMode = ReviewModes.CHANGE;
            ViewBag.ReviewURL = Url.Action("Review", "People");
            ViewBag.AreaBlurb = "The associates in the grid below have had roles assigned to them by the Resource One Security Manager rule engine due to changes in job, department or location.";
            ViewBag.AreaTitle = "Recently Changed Associates";
            ViewBag.PageSize = (Request.Cookies["EmployeePageSize"] != null) ? int.Parse(Request.Cookies["EmployeePageSize"].Value) : 50;
            return View();
        }

        [Authorize]
        public ActionResult NewHires()
        {
            ViewBag.IsAdmin = false;
            if (User.IsInRole("admin") || User.Identity.Name == "admin")
            {
                ViewBag.IsAdmin = true;
            }
            ViewBag.GridDataURL = Url.Action("NewEmployeesSearch", "People");// "../People/NewEmployeesSearch/";
            ViewBag.ReviewMode = ReviewModes.HIRE;
            ViewBag.ReviewURL = Url.Action("ReviewHire", "People"); // "../People/ReviewHire/";
            ViewBag.AreaTitle = "Recently Hired Associates";
            ViewBag.AreaBlurb = "The associates in the grid below are new hires that have had roles assigned to them by the Resource One Security Manager rule engine.";
            ViewBag.PageSize = (Request.Cookies["EmployeePageSize"] != null) ? int.Parse(Request.Cookies["EmployeePageSize"].Value) : 50;
            return View("ReviewQueue");
        }

        [Authorize]
        public ActionResult NewTerms()
        {
            ViewBag.IsAdmin = false;
            if (User.IsInRole("admin") || User.Identity.Name == "admin")
            {
                ViewBag.IsAdmin = true;
            }
            ViewBag.GridDataURL = Url.Action("TermEmployeesSearch", "People");
            ViewBag.ReviewMode = ReviewModes.FIRE;
            ViewBag.ReviewURL = Url.Action("ReviewTerm", "People");
            ViewBag.AreaTitle = "Recently Terminated Associates";
            ViewBag.AreaBlurb = "The associates in the grid below have been terminated and had their access removed by the Resource One Security Manager rule engine.";
            ViewBag.PageSize = (Request.Cookies["EmployeePageSize"] != null) ? int.Parse(Request.Cookies["EmployeePageSize"].Value) : 50;
            return View("ReviewQueue");
        }

        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }


    }
    


}
