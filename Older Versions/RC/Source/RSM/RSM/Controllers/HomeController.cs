using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using RSM.Support;

namespace RSM.Controllers
{
    public class HomeController : Controller
    {
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

        public ActionResult About()
        {
            ViewBag.IsAdmin = false;
            if (User.IsInRole("admin") || User.Identity.Name == "admin")
            {
                ViewBag.IsAdmin = true;
            }
            return View();
        }



    }
}
