using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using Tdlr.Models;

namespace Tdlr.DAL
{
    public class TasksDbHelper
    {
        public static List<Task> GetAllTasks(List<string> objectIds)
        {
            // Get all tasks that the user has created or has been authorized to view.
            TdlrContext db = new TdlrContext();
            return db.Tasks.Where(
                t => t.SharedWith.Any(
                    a => objectIds.Contains(a.AadObjectID)))
                    .ToList();
        }

        public static Task GetTask(int taskId)
        {
            // Get a specific Task from the db.
            TdlrContext db = new TdlrContext();
            Task task = db.Tasks.Find(taskId);
            var captureSharedWith = task.SharedWith;
            return task;
        }

        public static Task AddTask(string taskText, string userObjectId, string userName)
        {
            // Add a new task to the db
            TdlrContext db = new TdlrContext();
            Task newTask = new Task
            {
                Status = "NotStarted",
                TaskText = taskText,
                Creator = userObjectId,
                SharedWith = new List<AadObject>(),
                CreatorName = userName,
            };

            // Get the AadObject representing from the user if it exists
            AadObject user = db.AadObjects.Find(userObjectId);
            if (user != null)
            {
                // Update the user's display name in case it has changed
                user.DisplayName = userName;
            }
            else
            {
                user = new AadObject
                {
                    AadObjectID = userObjectId,
                    DisplayName = userName,
                };
            }

            newTask.SharedWith.Add(user);
            db.Tasks.Add(newTask);
            db.SaveChanges();

            return newTask;
        }

        public static Task UpdateTask(int taskId, string status)
        {
            // Update an existing task in the db
            TdlrContext db = new TdlrContext();
            Task task = db.Tasks.Find(taskId);
            var captureSharedWith = task.SharedWith;
            if (task == null)
                throw new Exception("Task Not Found in DB");
            task.Status = status;
            db.SaveChanges();
            return task;
        }

        public static void DeleteTask(int taskId)
        {
            //Delete a task in the db
            TdlrContext db = new TdlrContext();
            Task task = db.Tasks.Find(taskId);
            db.Tasks.Remove(task);
            db.SaveChanges();
        }

        public static void UpdateShares(int taskId, List<Share> shares)
        {
            //Share a task with a user or group
            TdlrContext db = new TdlrContext();
            Task task = db.Tasks.Find(taskId);

            // Maintain that the task is shared with the owner
            AadObject user = task.SharedWith.Where(u => u.AadObjectID == task.Creator).FirstOrDefault();
            task.SharedWith = new List<AadObject>();
            task.SharedWith.Add(user);

            foreach (Share share in shares)
            {
                AadObject aadObject = db.AadObjects.Find(share.objectId);
                if (aadObject != null)
                {
                    aadObject.DisplayName = share.displayName;
                }
                else
                {
                    aadObject = new AadObject
                    {
                        AadObjectID = share.objectId,
                        DisplayName = share.displayName,
                    };
                }
                task.SharedWith.Add(aadObject);
            }
            db.SaveChanges();
        }
    }
}