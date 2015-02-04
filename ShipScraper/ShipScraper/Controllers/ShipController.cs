using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using ShipScraper.Models;

namespace ShipScraper.Controllers
{
    public class ShipController : ApiController
    {
        readonly DatabaseRepo _repo = new DatabaseRepo();

        // GET api/ships
        public IEnumerable<string> Get()
        {
            return _repo.GetAllShips().Select(s => s.Name).ToArray();
        }
    }
}