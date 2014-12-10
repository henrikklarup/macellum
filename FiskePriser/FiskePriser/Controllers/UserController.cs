﻿using System.Web.Mvc;
using Macellum.Models;

namespace Macellum.Controllers
{
    public class UserController : BaseController
    {

        private readonly DatabaseRepo _repo = new DatabaseRepo();

        // GET: User
        [Authorize(Roles = "Admin,User")]
        public ActionResult Edit(User user)
        {
            if(user == null)
                user = new User();
            user.Password.pass = "";
            user.Password.ConfirmPassword = "";
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(User user)
        {
            if (!ModelState.IsValid)
                return View();

            var sucess = _repo.AddUser(user);
            if (!sucess)
            {
                ModelState.AddModelError("User already exsists","User with username " + user.Username+" already exists");

                return View();
            }

            return View("Edit", user);
        }

        public ActionResult Rejected()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        //This one calls by Custom Authentication to validate username/password
        [HttpPost]
        public ActionResult Login(User user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password.pass))
            {
                ModelState.AddModelError("FAIL","No username or password");
                return View();
            }

            if (_repo.AuthenticateUser(user))
            {
                if (HttpContext.Session != null)
                {
                    SimpleSessionPersister.Username = user.Username;
                    var sessionId = System.Web.HttpContext.Current.Session.SessionID;
                    _repo.SaveSessionId(user.Username, sessionId);
                }
            }

            return Redirect(Request.QueryString["ReturnUrl"]);
        }
    }
}