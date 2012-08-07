#region License

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

using System;
using System.Web.Mvc;
using NooSphere.Cloud.ActivityManager.Controllers.Api;

#endregion

namespace NooSphere.Cloud.ActivityManager.Controllers
{
    public class AdminController : Controller
    {
        private readonly ActivityController ActivityController = new ActivityController();
        private readonly DeviceController DeviceController = new DeviceController();
        private readonly FriendController FriendController = new FriendController();
        private readonly UserController UserController = new UserController();

        public ActionResult Users()
        {
            return View(UserController.GetExtendedUsers());
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ClearAll()
        {
            UserController.Clear();
            FriendController.Clear();
            ActivityController.Clear();
            DeviceController.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult ClearAllKeepUsers()
        {
            ActivityController.Clear();
            DeviceController.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid id)
        {
            UserController.RemoveUser(id);
            return RedirectToAction("Index");
        }
    }
}