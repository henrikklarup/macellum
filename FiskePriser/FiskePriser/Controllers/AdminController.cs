using System;
using System.Linq;
using System.Web.Mvc;
using Macellum.Models;

namespace Macellum.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        readonly DatabaseRepo _repo = new DatabaseRepo();

        // GET: Admin
        public ActionResult Index()
        {
            var updated = _repo.GetAllFishNewest().FirstOrDefault().Dato;
            ViewBag.LastUpdate = updated;
            return View();
        }

        public ActionResult LoadData()
        {
            var dataclass = new DataClass();
            dataclass.dataLoad();

            return RedirectToAction("Index");
        }

        public ActionResult IpLogs()
        {
            var iplogs = _repo.GetAllIpLogs();
            return View(iplogs);
        }
    }
}