using System.Web.Mvc;
using System.Configuration;
using System.Web.Configuration;
using RSM.Staging.Web.Models;

namespace RSM.Staging.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Choose data to stage.";

			var model = new StagingToolsModel { R1SM = true, S2Incoming = true, S2IncomingHistory = true, TrackOutgoing = true, TrackOutgoingHistory = true, CleanAllPeople = true, CleanAllAccessHistory = true, CleanAllExternalAppKeys = true };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(string command, StagingToolsModel model)
        {
            ViewBag.Message = command;
            var stage = new Library.DataLoad();

			model.ValidationKey = model.ValidationKey == null
				? (ConfigurationManager.GetSection("system.web/machineKey") as MachineKeySection).ValidationKey
				: model.ValidationKey;

            if(command == "Run")
                stage.Run(model);

            if (command == "Clean")
                stage.Clean(model);

            return View(model);
        }

        public ActionResult NotAvailable()
        {
            return View();
        }
    }
}
