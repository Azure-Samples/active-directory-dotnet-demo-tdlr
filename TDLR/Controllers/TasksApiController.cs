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
        [HostAuthentication("AADBearer")]
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
        [HostAuthentication("AADBearer")]
        [Authorize]
        public Models.Task Get(int id)
        {
            List<object> tasks = new List<object>();
            return TasksDbHelper.GetTask(id);
        }


        [HttpPost]
        [HostAuthentication("AADBearer")]
        [Authorize]
        public Models.Task Create(Models.Task task)
        {
            // Create a new task
            if (task.TaskText != null && task.TaskText.Length != 0)
            {
                return TasksDbHelper.AddTask(task.TaskText,
                    ClaimsPrincipal.Current.FindFirst(Globals.ObjectIdClaimType).Value,
                    ClaimsPrincipal.Current.FindFirst(Globals.GivennameClaimType).Value + ' '
                    + ClaimsPrincipal.Current.FindFirst(Globals.SurnameClaimType).Value);

            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        [HttpPut]
        [HostAuthentication("AADBearer")]
        [Authorize]
        public Models.Task Update(int id, Models.Task task)
        {
            return TasksDbHelper.UpdateTask(id, task.Status);
        }

        [HttpDelete]
        [HostAuthentication("AADBearer")]
        [Authorize]
        public void Delete(int id)
        {
            TasksDbHelper.DeleteTask(id);
        }   

        [HttpPut]
        [HostAuthentication("AADBearer")]
        [Authorize]
        public void UpdateShares(int id, List<Models.Share> shares)
        {
            TasksDbHelper.UpdateShares(id, shares);
        }

        [HttpGet]
        [HostAuthentication("AADBearer")]
        [Authorize]
        public List<Models.Share> GetShares(int id)
        {
            Models.Task task = TasksDbHelper.GetTask(id);
            List<Models.Share> shares = new List<Models.Share>();
            foreach (Models.AadObject share in task.SharedWith)
            {
                if (share.AadObjectID != ClaimsPrincipal.Current.FindFirst(Globals.ObjectIdClaimType).Value)
                {
                    shares.Add(new Models.Share{ objectId = share.AadObjectID, displayName = share.DisplayName });
                }
            }
            return shares;
        }
    }
}
