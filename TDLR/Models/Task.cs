using System;
using System.Collections.Generic;
//The following libraries were added to this sample.
using System.IO;
using System.Web.Hosting;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Tdlr.Models
{
    public class Task
    {
        //Every Task entry has a Task, a Status, and a TaskID
        public int TaskID { get; set; }
        [Required]
        public string TaskText { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string Creator { get; set; }
        [Required]
        public string CreatorName { get; set; }
        [Required]
        [JsonIgnore]
        public virtual ICollection<AadObject> SharedWith { get; set; }
    }
}