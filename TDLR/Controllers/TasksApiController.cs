using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Tdlr.DAL;
using Tdlr.Utils;

namespace Tdlr.Controllers
{
    public class TasksApiController : ApiController
    {
        [HttpGet]
        [HostAuthentication("AADBearer")] // Ensures that the proper middleware is used for API requests
        [Authorize]
        public List<Models.Task> GetAll()
        {
            string userObjectId = ClaimsPrincipal.Current.FindFirst(Globals.ObjectIdClaimType).Value;
            return TasksDbHelper.GetAllTasks(new List<string> { userObjectId });
        }

        [HttpGet]
        [HostAuthentication("AADBearer")]
        [Authorize]
        public Models.Task Get(int id)
        {
            EnsureAccessToTask(id);
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

            throw new HttpResponseException(HttpStatusCode.BadRequest);
        }

        [HttpPut]
        [HostAuthentication("AADBearer")]
        [Authorize]
        public Models.Task Update(int id, Models.Task task)
        {
            // Update an existing task
            EnsureAccessToTask(id);
            return TasksDbHelper.UpdateTask(id, task.Status);
        }

        [HttpDelete]
        [HostAuthentication("AADBearer")]
        [Authorize]
        public void Delete(int id)
        {
            // Delete an existing task
            EnsureOwnerOfTask(id);
            TasksDbHelper.DeleteTask(id);
        }   

        [HttpPut]
        [HostAuthentication("AADBearer")]
        [Authorize]
        public void UpdateShares(int id, List<Models.Share> shares)
        {
            // Update the list of shares for the task
            EnsureOwnerOfTask(id);
            TasksDbHelper.UpdateShares(id, shares);
        }

        [HttpGet]
        [HostAuthentication("AADBearer")]
        [Authorize]
        public List<Models.Share> GetShares(int id)
        {
            // Read the list of shares for the task
            EnsureAccessToTask(id);
            Models.Task task = TasksDbHelper.GetTask(id);
            List<Models.Share> shares = new List<Models.Share>();
            foreach (Models.AadObject share in task.SharedWith)
            {
                // Don't show the client that the task is shared with the owner
                if (share.AadObjectID != ClaimsPrincipal.Current.FindFirst(Globals.ObjectIdClaimType).Value)
                {
                    shares.Add(new Models.Share{ objectId = share.AadObjectID, displayName = share.DisplayName });
                }
            }
            return shares;
        }

        private void EnsureAccessToTask(int taskId)
        {
            // Check if the user has permission to access the task
            string userObjectId = ClaimsPrincipal.Current.FindFirst(Globals.ObjectIdClaimType).Value;
            List<Models.Task> tasks = TasksDbHelper.GetAllTasks(new List<string> { userObjectId });
            if (tasks.Where(t => t.TaskID == taskId).Count() == 0)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
        }

        private void EnsureOwnerOfTask(int taskId)
        {
            // Check if the user is the owner of the task
            Models.Task task = TasksDbHelper.GetTask(taskId);
            string userObjectId = ClaimsPrincipal.Current.FindFirst(Globals.ObjectIdClaimType).Value;
            if (task.Creator != userObjectId)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
        }
    }
}
