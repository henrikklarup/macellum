﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using Macellum.Models;
using Newtonsoft.Json;

namespace Macellum.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class TripController : BaseController
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

            
            var a = new TripClass(new List<Fisk>(), DateTime.UtcNow, allArters)
            {
                TripOpen = _repo.UserHasOpenTrip(SimpleSessionPersister.Username)
            };
            a.CurrentTrip = a.TripOpen ? _repo.GetCurrentTrip(SimpleSessionPersister.Username) : new Trip();

            if (a.TripOpen) a.CurFangst = a.CurrentTrip.FishList;
            else a.CurrentTrip.FishList = "[]";
            if (a.TripOpen) a.Fangst = JsonConvert.DeserializeObject<List<Fisk>>(a.CurrentTrip.FishList);

            return View(a);
        }

        [HttpPost]
        public ActionResult AddFish(TripClass model)
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
                    var firstOrDefault = model.Fangst.FirstOrDefault(s => s.Arters.Id == newFish.Arters.Id && s.Sort == newFish.Sort);
                    if (firstOrDefault != null)
                    {
                        var x =
                            firstOrDefault
                                .Amount;
                        firstOrDefault.Amount = (decimal.Parse(x) + decimal.Parse(newFish.Amount)).ToString(CultureInfo.InvariantCulture);
                    }
                }
            }
            model.AlleArters = _repo.GetAllArters().ToList();
            //return PartialView("TripPartial", model);


            #region OldSaveFish
            var fangstStr = JsonConvert.SerializeObject(model.Fangst);

            if (!_repo.UserHasOpenTrip(SimpleSessionPersister.Username))
            {
                var newTrip = new Trip
                {
                    Dato = DateTime.UtcNow,
                    FishList = fangstStr,
                    User = _repo.GetUserByName(SimpleSessionPersister.Username),
                    Open = true
                };
                _repo.AddTrip(newTrip);
            }
            else
            {
                var updTrip = _repo.GetCurrentTrip(SimpleSessionPersister.Username);
                updTrip.FishList = fangstStr;
                _repo.UpdateTrip(updTrip);
            }
            #endregion

            if (!model.TripOpen)
            {
                var updTrip = _repo.GetCurrentTrip(SimpleSessionPersister.Username);
                updTrip.FishList = fangstStr;
                updTrip.Open = false;
                _repo.UpdateTrip(updTrip);
            }
            else
            {
                model.TripOpen = _repo.UserHasOpenTrip(SimpleSessionPersister.Username);
            }

            model.CurrentTrip = model.TripOpen ? _repo.GetCurrentTrip(SimpleSessionPersister.Username) : new Trip();
            if (!model.TripOpen) model.CurrentTrip.FishList = "[]";


            return View("Trip", model);
        }

        [HttpGet]
        public ActionResult Trip(TripClass model)
        {
            return View(model);
        }

        [HttpGet]
        public ActionResult SletFish(int fishId, string fishSort )
        {
            var currentTrip = _repo.GetCurrentTrip(SimpleSessionPersister.Username);
            if (currentTrip == null)
                return RedirectToAction("Index", "Home");

            var fishList = JsonConvert.DeserializeObject<List<Fisk>>(currentTrip.FishList);
            fishList.RemoveAll(s => s.Arters.Id == fishId && s.Sort == fishSort);
            currentTrip.FishList = JsonConvert.SerializeObject(fishList);
            _repo.UpdateTrip(currentTrip);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Historik()
        {
            var x = _repo.GetAllTrips(SimpleSessionPersister.Username);
            var k = x.OrderByDescending(s => s.Dato);
            return View(k);
        }

        [HttpGet]
        public ActionResult ReOpenTrip(int tripId)
        {
            var updTrip = _repo.GetCurrentTrip(SimpleSessionPersister.Username);
            if(updTrip != null)
                updTrip.Open = false;

            var reopnTrip = _repo.GetTripById(tripId);
            reopnTrip.Open = true;
            reopnTrip.Dato = DateTime.UtcNow;

            _repo.Save();

            return RedirectToAction("Historik");
        }

        [HttpPost]
        public ActionResult SaveFishes(TripClass model)
        {
            model.Fangst = model.CurFangst != "[]" ? JsonConvert.DeserializeObject<List<Fisk>>(model.CurFangst) : new List<Fisk>();

            var fangstStr = JsonConvert.SerializeObject(model.Fangst);

            if (!_repo.UserHasOpenTrip(SimpleSessionPersister.Username))
            {
                var newTrip = new Trip
                {
                    Dato = DateTime.UtcNow,
                    FishList = fangstStr,
                    User = _repo.GetUserByName(SimpleSessionPersister.Username),
                    Open = true
                };
                _repo.AddTrip(newTrip);
            }
            else
            {
                var updTrip = _repo.GetCurrentTrip(SimpleSessionPersister.Username);
                updTrip.FishList = fangstStr;
                _repo.UpdateTrip(updTrip);
            }
            //var fishCookie = new HttpCookie("fishCookie") {Value = fangstStr, Expires = DateTime.Now.AddHours(2)};
            //Response.Cookies.Add(fishCookie);

            return RedirectToAction("Index", "Home");
        }
    }
}