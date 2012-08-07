﻿#region License

// Copyright (c) 2012 Steven Houben(shou@itu.dk) and Søren Nielsen(snielsen@itu.dk)
// 
// Pervasive Interaction Technology Laboratory (pIT lab)
// IT University of Copenhagen
// 
// This library is free software; you can redistribute it and/or 
// modify it under the terms of the GNU GENERAL PUBLIC LICENSE V3 or later, 
// as published by the Free Software Foundation. Check 
// http://www.gnu.org/licenses/gpl.html for details.

#endregion

#region

using System.ComponentModel.DataAnnotations;

#endregion

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