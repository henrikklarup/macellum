using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Globalization;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace ShipScraper.Models
{
    public class DatabaseRepo
    {
        readonly LocalDBContainer _repo = new LocalDBContainer();
        public void AddShip(Ship ship)
        {
            _repo.Ships.Add(ship);
            Save();
        }

        public IEnumerable<Ship> GetAllShips()
        {
            return _repo.Ships;
        }

        public void GetContent(string url)
        {
            var allShips = new List<Ship>();
            var web = new HtmlWeb();
            var doc = web.Load(url);

            var nodes = doc.DocumentNode.SelectNodes("//table/tr");
            var table = new DataTable("MyTable");

            table.Columns.Add("Country");
            table.Columns.Add("ShipId");
            table.Columns.Add("Name");

            var rows = nodes.Skip(1).Select(tr => tr.Elements("td"));

            foreach (var row in rows)
            {
                var innerText = row.Select(td => td.InnerText.Trim()).ToArray();
                var innerHtml = row.Select(td => td.InnerHtml.Trim()).ToList()[9].Split('/')[5].Split(':')[1];
                innerText[0] = innerHtml;

                table.Rows.Add(row);

                var newShip = new Ship
                {
                    ShipId = HttpUtility.HtmlDecode(innerText[0]),
                    Country = "DK",
                    Name = HttpUtility.HtmlDecode(innerText[3])
                };

                /*
                 * intet -> id
                    IMO
                    MMSI
                    Navn
                    Billede
                    Intet
                    Mål
                    Fart
                    dest
                 */
                if(!_repo.Ships.Any(s=>s.ShipId == newShip.ShipId))
                    allShips.Add(newShip);

            }
            _repo.Ships.AddRange(allShips);
            Save();
        }

        /*TODO*/
        public void FetchShipPosition(string shipId)
        {
            var web = new HtmlWeb();
            var url = "http://www.marinetraffic.com/dk/ais/details/ships/shipid:" + shipId;
            var doc = web.Load(url);

            var nodes = doc.DocumentNode.SelectNodes("//span");
            var table = new DataTable("MyTable");

            table.Columns.Add("ShipId");
            table.Columns.Add("Position");

            var rows = nodes;

            var shipPos = rows[36].InnerText.Replace("&deg;","").Replace("/",",").Replace(" ","");

            var sp = new ShipPosition
            {
                Position = shipPos.Trim(),
                ShipId = shipId
            };

            if (_repo.ShipPositions.Any(s => s.ShipId == shipId))
                _repo.ShipPositions.FirstOrDefault(s => s.ShipId == shipId).Position = shipPos.Trim();
            else
            {
                _repo.ShipPositions.Add(sp);
            }
        }

        public string GetNameFromShipId(string shipId)
        {
            return _repo.Ships.FirstOrDefault(s => s.ShipId == shipId).Name;
        }

        public IEnumerable<ShipPosition> GetAllShipPositions()
        {
            return _repo.ShipPositions;
        }


        public void Save()
        {
            _repo.SaveChanges();
        }
    }
}