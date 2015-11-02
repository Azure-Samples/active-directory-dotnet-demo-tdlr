using System;
using System.Web.Mvc;

//The following libraries were added to the sample
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

//The following libraries were defined and added to this sample.
using Tdlr.DAL;
using Tdlr.Utils;
using System.Net;
using System.Web;

namespace Tdlr.Controllers
{
    public class TasksController : Controller
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Index()
        {
            string userObjectId = ClaimsPrincipal.Current.FindFirst(Globals.ObjectIdClaimType).Value;
            ViewData["userId"] = userObjectId;
            ViewData["tenant"] = ClaimsPrincipal.Current.FindFirst(Globals.TenantIdClaimType).Value;
            ViewData["tasks"] = TasksDbHelper.GetAllTasks(new List<string> { userObjectId });
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Create(string text)
        {
            // Create a new task
            if (text != null && text.Length != 0)
            {
                Models.Task task = TasksDbHelper.AddTask(text,
                    ClaimsPrincipal.Current.FindFirst(Globals.ObjectIdClaimType).Value,
                    ClaimsPrincipal.Current.FindFirst(Globals.GivennameClaimType).Value + ' '
                    + ClaimsPrincipal.Current.FindFirst(Globals.SurnameClaimType).Value);

                return RedirectToAction("Index");
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        
    }
}