using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Macellum.Models;

namespace Macellum.Controllers
{
    public class BlogController : BaseController
    {
        DatabaseRepo _repo = new DatabaseRepo();

        // GET: Blog
        public ActionResult Index()
        {
            //var model = _repo.BlogPosts().ToList();
            var model = _repo.BlogPosts(10).ToList();
            return View(model);
        }

        // GET: Blog
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var newBlog = new Nyhede();
            return View(newBlog);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Nyhede blog)
        {
            _repo.AddBlog(blog);
            return RedirectToAction("Index");
        }
    }
}