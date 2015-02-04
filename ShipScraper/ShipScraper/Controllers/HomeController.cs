using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using ShipScraper.Models;

namespace ShipScraper.Controllers
{
    public class HomeController : Controller
    {
        DatabaseRepo _repo = new DatabaseRepo();

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult FetchShips()
        {
            _repo.GetContent("http://www.marinetraffic.com/dk/ais/index/ships/range/flag:DK/ship_type:2/page:1/per_page:50");
            _repo.GetContent("http://www.marinetraffic.com/dk/ais/index/ships/range/flag:DK/ship_type:2/page:2/per_page:50");
            _repo.GetContent("http://www.marinetraffic.com/dk/ais/index/ships/range/flag:DK/ship_type:2/page:3/per_page:50");
            _repo.GetContent("http://www.marinetraffic.com/dk/ais/index/ships/range/flag:DK/ship_type:2/page:4/per_page:50");

            return RedirectToAction("Index");
        }

        public ActionResult AllShips()
        {
            var model = _repo.GetAllShips();//.DistinctBy(s=>s.ShipId);
            return View(model);
        }

        public ActionResult AllPositions()
        {
            var model = _repo.GetAllShipPositions();
            return View(model);
        }

        public ActionResult FetchShipPosition()
        {
            foreach (var ship in _repo.GetAllShips())
            {
                _repo.FetchShipPosition(ship.ShipId);
            }
            _repo.Save();
            return RedirectToAction("Index");
        }
    }
}
