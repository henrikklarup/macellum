using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using HtmlAgilityPack;

namespace FiskePriser.Models
{
    public class DatabaseRepo
    {
        readonly DBConnectionContainer _repo = new DBConnectionContainer();

        internal class FishWithPrices
        {
            public int Id;
            public int Amount;
            public string Name;

            public FishWithPrices(int id, string name)
            {
                this.Id = id;
                this.Name = name;
            }
        }


        public decimal GetFishPrices(List<Fisk> fangst, int havneId)
        {
            var newestFish = GetAllFishNewest(havneId);

            var lal =
                newestFish.Where(
                    first => 
                        fangst.Any(second => 
                            first.Arters.Id == second.Arters.Id &&
                            first.Sort.Normalize(NormalizationForm.FormKC).Trim() == second.Sort.Normalize(NormalizationForm.FormKC).Trim()));
            
            var query = lal.Select(x =>
            {
                x.Amount = fangst.FirstOrDefault(s => s.Arters.Id == x.Arters.Id).Amount;
                return x;
            });

            var returnPrice = query.Sum(s =>
                decimal.Parse(s.Amount.Normalize(NormalizationForm.FormKC)) * 
                decimal.Parse(s.AvgPrice.Normalize(NormalizationForm.FormKC).Split(' ')[0].Replace(',','.')));

            return returnPrice;
        }

        public decimal GetFishPricesMax(List<Fisk> fangst, int havneId)
        {
            var newestFish = GetAllFishNewest(havneId);

            var lal =
                newestFish.Where(
                    first =>
                        fangst.Any(second =>
                            first.Arters.Id == second.Arters.Id &&
                            first.Sort.Normalize(NormalizationForm.FormKC).Trim() == second.Sort.Normalize(NormalizationForm.FormKC).Trim()));

            var query = lal.Select(x =>
            {
                x.Amount = fangst.FirstOrDefault(s => s.Arters.Id == x.Arters.Id).Amount;
                return x;
            });

            var returnPrice = query.Sum(s =>
                decimal.Parse(s.Amount.Normalize(NormalizationForm.FormKC)) *
                decimal.Parse(s.MaxPrice.Normalize(NormalizationForm.FormKC).Split(' ')[0].Replace(',', '.')));

            return returnPrice;
        }

        public IEnumerable<Fisk> GetAllFish()
        {
            return _repo.Fisks;
        }

        public IEnumerable<Arter> GetAllArters()
        {
            return _repo.Arters;
        }

        public IEnumerable<Fisk> GetAllFish(int havnId)
        {
            return _repo.Fisks.Where(s => s.Havne.Id == havnId);
        }

        public IEnumerable<Fisk> GetAllFishFromDay(DateTime date)
        {
            return _repo.Fisks.Where(s => DateTime.Parse(s.Dato).Date.Equals(date.Date));
        }

        public IEnumerable<Fisk> GetAllFishFromDay(DateTime date, int havnId)
        {
            return _repo.Fisks.Where(s => DateTime.Parse(s.Dato).Date.Equals(date.Date) && s.Havne.Id == havnId);
        }

        public IEnumerable<Fisk> GetAllFishNewest()
        {
            var firstOrDefault = _repo.Fisks.OrderByDescending(s => DateTime.Parse(s.Dato).Date).FirstOrDefault();
            if (firstOrDefault == null) return null;
            var initialDate = DateTime.Parse(firstOrDefault.Dato).Date;
            return _repo.Fisks.Where(s => DateTime.Parse(s.Dato).Date.Equals(initialDate));
        }

        internal class TestClass
        {
            public DateTime _dateTime;
            public Fisk _Fisk;

            public TestClass(DateTime dateTime)
            {
                this._dateTime = dateTime;

            }

            public TestClass()
            {
            }
        }

        public IEnumerable<Fisk> GetAllFishNewest(int havneId)
        {
            //var firstOrDefault = _repo.Fisks.ToList().OrderByDescending(s => Convert.ToDateTime(s.Dato).Date).FirstOrDefault();
            var DateQuery = _repo.Fisks.ToList().Select(o => new TestClass()
            {
                _dateTime = DateTime.Parse(o.Dato),
                _Fisk = o
            });


            var firstOrDefault = DateQuery.OrderByDescending(s=>s._dateTime).FirstOrDefault();

            if (firstOrDefault == null) return null;
            var initialDate = firstOrDefault._dateTime;

            var returnFish =
                DateQuery.Where(s => s._dateTime.Date.Equals(initialDate.Date) && s._Fisk.Havne.Id == havneId).ToList().Select(s=>s._Fisk).ToList();

            return returnFish;
            //return _repo.Fisks.Where(s => DateTime.Parse(s.Dato).Date.Equals(initialDate) && s.Havne.Id == havneId);
        }

        public IEnumerable<Fisk> GetAllFishNewest(DateTime date, int havnId)
        {
            var firstOrDefault = _repo.Fisks.OrderByDescending(s => DateTime.Parse(s.Dato).Date).FirstOrDefault();
            if (firstOrDefault == null) return null;
            var initialDate = DateTime.Parse(firstOrDefault.Dato).Date;
            return _repo.Fisks.Where(s => DateTime.Parse(s.Dato).Date.Equals(initialDate) && s.Havne.Id == havnId);
        }

        public int GetHavnId(string name)
        {
            var firstOrDefault = _repo.Havns.FirstOrDefault(s => s.Navn == name);
            if (firstOrDefault != null)
                return firstOrDefault.Id;
            return -1;
        }

        internal class ArtTableElement
        {
            public int Id;
            public string Name;

            public ArtTableElement(int id, string name)
            {
                this.Id = id;
                this.Name = name;
            }
        }

        public Arter GetArtFromName(string art)
        {
            var artTest = art.Trim().ToLower().Substring(0, art.Trim().Length < 8 ? art.Trim().Length : 7);

            var artElements = new List<ArtTableElement>();

            _repo.Arters.ToList().ForEach(x=> artElements.Add(new ArtTableElement(x.Id,x.Navn)));
			
			var k = artElements.FirstOrDefault(s => s.Name.Trim().ToLower().Substring(0, s.Name.Trim().Length < 8 ? s.Name.Trim().Length : 7) == artTest);
            return k == null ? null : _repo.Arters.FirstOrDefault(s => s.Id == k.Id);
        }

        public Arter GetArtFromId(int id)
        {
            return _repo.Arters.FirstOrDefault(s => s.Id == id);
        }

        public Havn GetHavnFromName(string name)
        {
            return _repo.Havns.FirstOrDefault(s => String.Equals(s.Navn, name.Trim(), StringComparison.CurrentCultureIgnoreCase));
        }

        public Arter AddArt(Arter art)
        {
            var retArt = _repo.Arters.Add(art);
            Save();
            return retArt;
        }

        public Havn GetHavnFromId(int id)
        {
            return _repo.Havns.FirstOrDefault(s => s.Id == id);
        }

        public void AddFisk(Fisk fish)
        {
            _repo.Fisks.Add(fish);
            Save();
        }

        public void AddFisk(IEnumerable<Fisk> fish)
        {
            _repo.Fisks.AddRange(fish);
            Save();
        }

        public IEnumerable<Fisk> GetContent(string url, int havnId)
        {
            var allFish = new List<Fisk>();
            var web = new HtmlWeb();
            var doc = web.Load(url);

            var nodes = doc.DocumentNode.SelectNodes("//table/tr");
            var table = new DataTable("MyTable");

            /*
            var headers = nodes[0]
                .Elements("th")
                .Select(th => th.InnerText.Trim());
            foreach (var header in headers)
            {
                table.Columns.Add(header);
            }*/
            table.Columns.Add("Art");
            table.Columns.Add("Sort");
            table.Columns.Add("Gns.Pris");
            table.Columns.Add("Max.Kr");
            table.Columns.Add("Mængde");


            var rows = nodes.Skip(8).Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToArray());

            foreach (var row in rows.TakeWhile(row => row.Count() >= 5))
            {
                table.Rows.Add(row);
                var art = GetArtFromName(HttpUtility.HtmlDecode(row[0]));
                if (art == null)
                {
                    var artToAdd = new Arter {Navn = HttpUtility.HtmlDecode(row[0])};
                    art = AddArt(artToAdd);
                }
                var havn = GetHavnFromId(havnId);
                var dato = DateTime.UtcNow;
                var addFish = new Fisk
                {
                    Arters = art,
                    Havne = havn,
                    Dato = dato.ToString(new CultureInfo("da-dk")),
                    Sort = HttpUtility.HtmlDecode(row[1]),
                    AvgPrice = HttpUtility.HtmlDecode(row[2]),
                    MaxPrice = HttpUtility.HtmlDecode(row[3]),
                    Amount = HttpUtility.HtmlDecode(row[4])
                };
                allFish.Add(addFish);
            }

            return allFish;
        }

        public void Save()
        {
            _repo.SaveChanges();
        }
    }
}