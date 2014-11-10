using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FiskePriser.Models;
using Newtonsoft.Json;

namespace FiskePriser.Controllers
{
    public class HomeController : Controller
    {

        private readonly DatabaseRepo _repo = new DatabaseRepo();

        /* GET: Home
        public ActionResult Index()
        {
            return View();
        }*/

        // GET: Home
        public ActionResult Index()
        {
            var gfa = new GraphArray();
            var fangst = new List<Fisk>();
            if (Request.Cookies["fishCookie"] != null)
            {   
                var roll = Request.Cookies["fishCookie"].Value; //For First Way
                fangst = JsonConvert.DeserializeObject<List<Fisk>>(roll);
                gfa.GraphNumbers.Add(new GraphInfo("Tyborøn", _repo.GetFishPrices(fangst, 1), _repo.GetFishPricesMax(fangst, 1)));
                gfa.GraphNumbers.Add(new GraphInfo("Hvide Sande", _repo.GetFishPrices(fangst, 2), _repo.GetFishPricesMax(fangst, 2)));
                gfa.GraphNumbers.Add(new GraphInfo("Thorsminde", _repo.GetFishPrices(fangst, 3), _repo.GetFishPricesMax(fangst, 3)));
            }
            return View(gfa);
        }
    }
}