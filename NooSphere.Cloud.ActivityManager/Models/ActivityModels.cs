using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NooSphere.Cloud.ActivityManager.Models
{
    public class AddActivityModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }


    }
    public class GetAllActivities
    {
        
    }
}