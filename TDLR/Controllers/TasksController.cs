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

namespace Tdlr.Controllers
{
    public class TasksController : Controller
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Index()
        {
            ClaimsIdentity userClaimsId = ClaimsPrincipal.Current.Identity as ClaimsIdentity;
            string userObjectId = userClaimsId.FindFirst(Globals.ObjectIdClaimType).Value;
            ViewData["userId"] = userObjectId;
            ViewData["tenant"] = ClaimsPrincipal.Current.FindFirst(Globals.TenantIdClaimType).Value;
            List<string> userGroupsAndId = await ClaimHelper.GetGroups(userClaimsId);
            userGroupsAndId.Add(userObjectId);
            ViewData["tasks"] = TasksDbHelper.GetAllTasks(userGroupsAndId);
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Get()
        {
            List<object> tasks = new List<object>();
            ClaimsIdentity userClaimsId = ClaimsPrincipal.Current.Identity as ClaimsIdentity;
            List<string> userGroupsAndId = await ClaimHelper.GetGroups(userClaimsId);
            string userObjectId = userClaimsId.FindFirst(Globals.ObjectIdClaimType).Value;
            userGroupsAndId.Add(userObjectId);
            foreach (Models.Task task in TasksDbHelper.GetAllTasks(userGroupsAndId))
            {
                tasks.Add(new
                {
                    Creator = task.Creator,
                    CreatorName = task.CreatorName,
                    Status = task.Status,
                    TaskID = task.TaskID,
                    TaskText = task.TaskText
                });
            }
            return Json(tasks, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Get(int id)
        {
            List<object> tasks = new List<object>();
            Models.Task task = TasksDbHelper.GetTask(id);
            tasks.Add(new
            {
                Creator = task.Creator,
                CreatorName = task.CreatorName,
                Status = task.Status,
                TaskID = task.TaskID,
                TaskText = task.TaskText
            });
                
            return Json(tasks, JsonRequestBehavior.AllowGet);
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

                var newTask = new
                {
                    Creator = task.Creator,
                    CreatorName = task.CreatorName,
                    Status = task.Status,
                    TaskID = task.TaskID,
                    TaskText = task.TaskText
                };

                if (HttpContext.Request.Headers["Accept"].Contains("application/json"))
                    return Json(newTask);

                return RedirectToAction("Index");
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpPatch]
        [Authorize]
        public ActionResult Update(int id, string status)
        { 
            Models.Task task = TasksDbHelper.UpdateTask(id, status);
            var updatedTask = new
            {
                Creator = task.Creator,
                CreatorName = task.CreatorName,
                Status = task.Status,
                TaskID = task.TaskID,
                TaskText = task.TaskText
            };

            return Json(updatedTask);
        }

        [HttpDelete]
        [Authorize]
        public ActionResult Delete(int id)
        {
            TasksDbHelper.DeleteTask(id);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPatch]
        [Authorize]
        public ActionResult UpdateShares(int id, List<Models.Share> shares)
        {
            TasksDbHelper.UpdateShares(id, shares);
            return Json(new { });
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetShares(int id)
        {
            Models.Task task = TasksDbHelper.GetTask(id);
            List<object> shares = new List<object>();
            foreach (Models.AadObject share in task.SharedWith)
            {
                if (share.AadObjectID != ClaimsPrincipal.Current.FindFirst(Globals.ObjectIdClaimType).Value)
                {
                    shares.Add(new { objectId = share.AadObjectID, displayName = share.DisplayName });
                }
            }
            return Json(shares, JsonRequestBehavior.AllowGet);
        }
    }
}