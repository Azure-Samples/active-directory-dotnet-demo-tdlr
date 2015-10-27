using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Tdlr.DAL;
using Tdlr.Utils;

namespace Tdlr.Controllers
{
    public class TasksApiController : ApiController
    {
        [HttpGet]
        [Authorize]
        public async Task<List<Models.Task>> GetAll()
        {
            List<object> tasks = new List<object>();
            ClaimsIdentity userClaimsId = ClaimsPrincipal.Current.Identity as ClaimsIdentity;
            List<string> userGroupsAndId = await ClaimHelper.GetGroups(userClaimsId);
            string userObjectId = userClaimsId.FindFirst(Globals.ObjectIdClaimType).Value;
            userGroupsAndId.Add(userObjectId);
            return TasksDbHelper.GetAllTasks(userGroupsAndId);
        }

        [HttpGet]
        [Authorize]
        public Models.Task Get(int id)
        {
            List<object> tasks = new List<object>();
            return TasksDbHelper.GetTask(id);
        }


        [HttpPost]
        [Authorize]
        public Models.Task Create(string text)
        {
            // Create a new task
            if (text != null && text.Length != 0)
            {
                return TasksDbHelper.AddTask(text,
                    ClaimsPrincipal.Current.FindFirst(Globals.ObjectIdClaimType).Value,
                    ClaimsPrincipal.Current.FindFirst(Globals.GivennameClaimType).Value + ' '
                    + ClaimsPrincipal.Current.FindFirst(Globals.SurnameClaimType).Value);

            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        [HttpPut]
        [Authorize]
        public Models.Task Update(int id, Models.Task task)
        {
            return TasksDbHelper.UpdateTask(id, task.Status);
        }

        [HttpDelete]
        [Authorize]
        public void Delete(int id)
        {
            TasksDbHelper.DeleteTask(id);
        }

        [HttpPut]
        [Authorize]
        public void UpdateShares(int id, List<Models.Share> shares)
        {
            TasksDbHelper.UpdateShares(id, shares);
        }

        [HttpGet]
        [Authorize]
        public List<Models.Share> GetShares(int id)
        {
            Models.Task task = TasksDbHelper.GetTask(id);
            List<Models.Share> shares = new List<Models.Share>();
            foreach (Models.AadObject share in task.SharedWith)
            {
                if (share.AadObjectID != ClaimsPrincipal.Current.FindFirst(Globals.ObjectIdClaimType).Value)
                {
                    shares.Add(new Models.Share{ objectID = share.AadObjectID, displayName = share.DisplayName });
                }
            }
            return shares;
        }
    }
}
