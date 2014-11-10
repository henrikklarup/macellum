using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using FiskePriser.Models;
using MoreLinq;
using Newtonsoft.Json;

namespace FiskePriser.Controllers
{
    public class TripController : Controller
    {
        private readonly DatabaseRepo _repo = new DatabaseRepo();
        private readonly Helper _helper = new Helper();
        // GET: Trip
        [HttpGet]
        public ActionResult Index()
        {
            var allArters = _repo.GetAllArters().ToList().OrderByDescending(s=>s.Navn.Normalize()).Reverse().ToList();

            allArters = _helper.PerformSwap(allArters, "Kulmule", 4);
            allArters = _helper.PerformSwap(allArters, "Jomfruhummer", 3);
            allArters = _helper.PerformSwap(allArters, "Mørksej", 2);
            allArters = _helper.PerformSwap(allArters, "Torsk", 1);
            allArters = _helper.PerformSwap(allArters, "Rødspætter", 0);

            var a = new Trip(new List<Fisk>(), DateTime.UtcNow, allArters);
            return View(a);
        }

        [HttpPost]
        public ActionResult AddFish(Trip model)
        {
            model.Fangst = model.CurFangst != "[]" ? JsonConvert.DeserializeObject<List<Fisk>>(model.CurFangst) : new List<Fisk>();
            if (!model.CurFishId.IsEmpty() && model.CurFishAmount.IsDecimal() && model.CurFishSort.IsDecimal())
            {
                var art = _repo.GetArtFromId(int.Parse(model.CurFishId));
                var newFish = new Fisk {Amount = model.CurFishAmount, Sort = model.CurFishSort, Arters = new Arter{Id = int.Parse(model.CurFishId),Navn = art.Navn}};
                var testThing = model.Fangst.FirstOrDefault(s => s.Arters.Id == newFish.Arters.Id && s.Sort == newFish.Sort);
                if (testThing == null)
                {
                    model.Fangst.Add(newFish);   
                }
                else
                {
                    var x =
                        model.Fangst.FirstOrDefault(s => s.Arters.Id == newFish.Arters.Id && s.Sort == newFish.Sort)
                            .Amount;
                    model.Fangst.FirstOrDefault(s=>s.Arters.Id == newFish.Arters.Id && s.Sort == newFish.Sort).Amount = (decimal.Parse(x) + decimal.Parse(newFish.Amount)).ToString();
                }
            }
            model.AlleArters = _repo.GetAllArters().ToList();
            //return PartialView("TripPartial", model);
            return View("Trip", model);
        }

        [HttpGet]
        public ActionResult Trip(Trip model)
        {
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveFishes(Trip model)
        {
            model.Fangst = model.CurFangst != "[]" ? JsonConvert.DeserializeObject<List<Fisk>>(model.CurFangst) : new List<Fisk>();  

            var fangstStr = JsonConvert.SerializeObject(model.Fangst);
            var fishCookie = new HttpCookie("fishCookie") {Value = fangstStr, Expires = DateTime.Now.AddHours(2)};
            Response.Cookies.Add(fishCookie);

            return RedirectToAction("Index", "Home");
        }
    }
}