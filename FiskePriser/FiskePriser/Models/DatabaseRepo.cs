using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using HtmlAgilityPack;
using MoreLinq;

namespace Macellum.Models
{
    public class DatabaseRepo
    {
        readonly DBConnectionContainer _repo = new DBConnectionContainer();

        #region AuthSingleUser
        public string GetSessionId(string username)
        {
            var usrId = GetUserByName(username).Id.ToString(CultureInfo.InvariantCulture);
            var activeSessionId = _repo.ActiveSessionIds.FirstOrDefault(s => s.UserId == usrId);
            return activeSessionId != null ? activeSessionId.SessionId : null;
        }

        public bool SaveSessionId(string username, string sessionId)
        {
            var usrId = GetUserByName(username).Id.ToString(CultureInfo.InvariantCulture);
            if (_repo.ActiveSessionIds.Any(s => s.UserId == usrId))
            {
// ReSharper disable PossibleNullReferenceException
                _repo.ActiveSessionIds.FirstOrDefault(s => s.UserId == usrId).SessionId = sessionId;
// ReSharper restore PossibleNullReferenceException
            }
            else
            {
                _repo.ActiveSessionIds.Add(new ActiveSessionId { SessionId = sessionId, UserId = usrId });
            }
            _repo.SaveChanges();
            return true;
        }
        #endregion

        #region User
        #region GET
        public User GetUserByName(string name)
        {
            var usersLocal = _repo.Users;
// ReSharper disable LoopCanBeConvertedToQuery
            foreach (var user in usersLocal)
// ReSharper restore LoopCanBeConvertedToQuery
            {
                var usrNameNorm = user.Username.Normalize().ToLower();
                if (usrNameNorm.Equals(name.Normalize().ToLower()))
                {
                    return user;
                }
            }
            return null;
        }

        public User GetUserById(int id)
        {
            return _repo.Users.FirstOrDefault(s => s.Id == id);
        }
        #endregion
        #endregion

        #region Role
        #region GET

        public string GetRoleByUsername(string userName)
        {
            return GetUserByName(userName).Role.Name;
        }

        public string GetRoleByUserId(int id)
        {
            return GetUserById(id).Role.Name;
        }
        #endregion
        #endregion

        #region Fish
        #region Fish With Prices class
        internal class FishWithPrices
        {
            public int Id;
            public int Amount;
            public string Name;

            public FishWithPrices(int id, string name)
            {
                Id = id;
                Name = name;
            }
        }
        #endregion

        #region Fish Prices
        #region Avg Prices
        public decimal GetFishPrices(List<Fisk> fangst, int havneId)
        {
            var newestFish = GetAllFishNewest(havneId);

            var lal =
                newestFish.Where(
                    first => 
                        fangst.Any(second => 
                            first.Arters.Id == second.Arters.Id &&
                            first.Sort.Normalize(NormalizationForm.FormKC).Trim() == second.Sort.Normalize(NormalizationForm.FormKC).Trim()));
            
// ReSharper disable PossibleMultipleEnumeration
            var query = lal.Select(x =>
// ReSharper restore PossibleMultipleEnumeration
            {
                x.Amount = fangst.FirstOrDefault(s => s.Arters.Id == x.Arters.Id).Amount;
                return x;
            });

            var returnPrice = query.Sum(s =>
                decimal.Parse(s.Amount.Normalize(NormalizationForm.FormKC)) * 
                decimal.Parse(s.AvgPrice.Normalize(NormalizationForm.FormKC).Split(' ')[0].Replace(',','.')));

// ReSharper disable PossibleMultipleEnumeration
            var turbis = fangst.Except(lal);
// ReSharper restore PossibleMultipleEnumeration
            var fisk = GetArtFromName("SkidtfiskLUNDE".Normalize()).Fisk.FirstOrDefault();

            var rndPris = decimal.Parse(fisk.AvgPrice.Normalize(NormalizationForm.FormKC).Split(' ')[0].Replace(',','.'));
            var turbisPris = turbis.Sum(s => decimal.Parse(s.Amount.Normalize(NormalizationForm.FormKC)))*rndPris;

            return returnPrice + turbisPris;
        }
        #endregion
        #region Max Prices
        public decimal GetFishPricesMax(List<Fisk> fangst, int havneId)
        {
            var newestFish = GetAllFishNewest(havneId);

            var lal =
                newestFish.Where(
                    first =>
                        fangst.Any(second =>
                            first.Arters.Id == second.Arters.Id &&
                            first.Sort.Normalize(NormalizationForm.FormKC).Trim() == second.Sort.Normalize(NormalizationForm.FormKC).Trim()));

// ReSharper disable PossibleMultipleEnumeration
            var query = lal.Select(x =>
// ReSharper restore PossibleMultipleEnumeration
            {
// ReSharper disable PossibleNullReferenceException
                x.Amount = fangst.FirstOrDefault(s => s.Arters.Id == x.Arters.Id).Amount;
// ReSharper restore PossibleNullReferenceException
                return x;
            });

            var returnPrice = query.Sum(s =>
                decimal.Parse(s.Amount.Normalize(NormalizationForm.FormKC)) *
                decimal.Parse(s.MaxPrice.Normalize(NormalizationForm.FormKC).Split(' ')[0].Replace(',', '.')));

// ReSharper disable PossibleMultipleEnumeration
            var turbis = fangst.Except(lal);
// ReSharper restore PossibleMultipleEnumeration
            var fisk = GetArtFromName("SkidtfiskLUNDE".Normalize()).Fisk.FirstOrDefault();

// ReSharper disable PossibleNullReferenceException
            var rndPris = decimal.Parse(fisk.AvgPrice.Normalize(NormalizationForm.FormKC).Split(' ')[0].Replace(',', '.'));
// ReSharper restore PossibleNullReferenceException
            var turbisPris = turbis.Sum(s => decimal.Parse(s.Amount.Normalize(NormalizationForm.FormKC))) * rndPris;



            return returnPrice + turbisPris;
        }
        #endregion
        #endregion

        #region Get all fish
        public IEnumerable<Fisk> GetAllFish()
        {
            return _repo.Fisks;
        }

        public IEnumerable<Fisk> GetAllFish(int havnId)
        {
            return _repo.Fisks.Where(s => s.Havne.Id == havnId);
        }

        public IEnumerable<Fisk> GetAllFishFromDay(DateTime date)
        {
            return _repo.Fisks.Where(s => DateTime.ParseExact(s.Dato, "dd-MM-yyyy HH:mm:ss", null).Date.Equals(date.Date));
        }

        public IEnumerable<Fisk> GetAllFishFromDay(DateTime date, int havnId)
        {
            return _repo.Fisks.Where(s => DateTime.ParseExact(s.Dato, "dd-MM-yyyy HH:mm:ss", null).Date.Equals(date.Date) && s.Havne.Id == havnId);
        }

        public IEnumerable<Fisk> GetAllFishNewest()
        {
            //var firstOrDefault = _repo.Fisks.ToList().OrderByDescending(s => Convert.ToDateTime(s.Dato).Date).FirstOrDefault();
            var dateQuery = _repo.Fisks.ToList().Select(o => new FishDateTimeClass
            {
                DateTime = DateTime.ParseExact(o.Dato, "dd-MM-yyyy HH:mm:ss", null),
                Fisk = o
            });

            var fisk = dateQuery.OrderByDescending(s => s.DateTime);
            var firstOrDefault = fisk.FirstOrDefault();

            if (firstOrDefault == null) return null;
            var tst = fisk.DistinctBy(s => new { s.Fisk.Arters.Navn, s.Fisk.Sort });
            var returnFish = tst.ToList().Select(s => s.Fisk).ToList();

            //var returnFish = DateQuery.Where(s => s._dateTime.Date.Equals(initialDate.Date) && s._Fisk.Havne.Id == havneId).ToList().Select(s => s._Fisk).ToList();

            return returnFish;
            //return _repo.Fisks.Where(s => DateTime.Parse(s.Dato).Date.Equals(initialDate) && s.Havne.Id == havneId);
        }

        public IEnumerable<Fisk> GetAllFishNewest(int havneId)
        {
            //var firstOrDefault = _repo.Fisks.ToList().OrderByDescending(s => Convert.ToDateTime(s.Dato).Date).FirstOrDefault();
            var dateQuery = _repo.Fisks.ToList().Select(o => new FishDateTimeClass
            {
                DateTime = DateTime.ParseExact(o.Dato, "dd-MM-yyyy HH:mm:ss", null),
                Fisk = o
            });

            var fisk = dateQuery.Where(s=>s.Fisk.Havne.Id == havneId).OrderByDescending(s => s.DateTime);
            var firstOrDefault = fisk.FirstOrDefault();

            if (firstOrDefault == null) return null;
            var tst = fisk.DistinctBy(s => new {s.Fisk.Arters.Navn, s.Fisk.Sort});
            var returnFish = tst.ToList().Select(s => s.Fisk).ToList();

            //var returnFish = DateQuery.Where(s => s._dateTime.Date.Equals(initialDate.Date) && s._Fisk.Havne.Id == havneId).ToList().Select(s => s._Fisk).ToList();

            return returnFish;
            //return _repo.Fisks.Where(s => DateTime.Parse(s.Dato).Date.Equals(initialDate) && s.Havne.Id == havneId);
        }

        public IEnumerable<Fisk> GetAllFishNewest(DateTime date, int havnId)
        {
            var firstOrDefault = _repo.Fisks.OrderByDescending(s => DateTime.ParseExact(s.Dato, "dd-MM-yyyy HH:mm:ss", null).Date).FirstOrDefault();
            if (firstOrDefault == null) return null;
            var initialDate = DateTime.ParseExact(firstOrDefault.Dato, "dd-MM-yyyy HH:mm:ss", null).Date;
            return _repo.Fisks.Where(s => DateTime.ParseExact(s.Dato, "dd-MM-yyyy HH:mm:ss", null).Date.Equals(initialDate) && s.Havne.Id == havnId);
        }
        #endregion
        #endregion

        #region Arter
        #region Get all arter
        public IEnumerable<Arter> GetAllArters()
        {
            return _repo.Arters;
        }

        public Arter GetArtFromName(string art)
        {
            var artTest = art.Trim().ToLower().Substring(0, art.Trim().Length < 8 ? art.Trim().Length : 7);

            var artElements = new List<ArtTableElement>();

            _repo.Arters.ToList().ForEach(x => artElements.Add(new ArtTableElement(x.Id, x.Navn)));

            var k = artElements.FirstOrDefault(s => s.Name.Trim().ToLower().Substring(0, s.Name.Trim().Length < 8 ? s.Name.Trim().Length : 7) == artTest);
            return k == null ? null : _repo.Arters.FirstOrDefault(s => s.Id == k.Id);
        }

        public Arter GetArtFromId(int id)
        {
            return _repo.Arters.FirstOrDefault(s => s.Id == id);
        }

        public Arter AddArt(Arter art)
        {
            var retArt = _repo.Arters.Add(art);
            Save();
            return retArt;
        }
        #endregion
        #endregion

        #region Trip
        public bool UserHasOpenTrip(User user)
        {
            if (user == null) return false;
// ReSharper disable LoopCanBeConvertedToQuery
            foreach (var user1 in _repo.Users)
// ReSharper restore LoopCanBeConvertedToQuery
            {
                var normUser = user1.Username.Normalize().Trim();
                if (user.Username.Normalize().Trim() == normUser)
                {
                    return user1.Trips.Any(s => s.Open);
                }
            }
            return false;
        }

        public bool UserHasOpenTrip(string user)
        {
            if (user == null) return false;
// ReSharper disable LoopCanBeConvertedToQuery
            foreach (var user1 in _repo.Users)
// ReSharper restore LoopCanBeConvertedToQuery
            {
                var normUser = user1.Username.Normalize().Trim();
                if (user.Normalize().Trim() == normUser)
                {
                    return user1.Trips.Any(s => s.Open);
                }
            }
            return false;
        }

        public Trip GetCurrentTrip(string user)
        {
            var dbUsr = GetUserByName(user);
// ReSharper disable LoopCanBeConvertedToQuery
            foreach (var user1 in _repo.Users)
// ReSharper restore LoopCanBeConvertedToQuery
            {
                if(dbUsr.Id == user1.Id)
                    return user1.Trips.FirstOrDefault(s => s.Open);
            }
            return null;
        }

        public void UpdateTrip(Trip trip)
        {
            var dbTrip = _repo.Trips.FirstOrDefault(s => s.Id == trip.Id);
            if (dbTrip != null) dbTrip.FishList = trip.FishList;
            Save();
        }

        public List<Trip> GetAllTrips(string user)
        {
            var dbUsr = GetUserByName(user);
            return dbUsr.Trips.ToList();
        }

        public Trip GetTripById(int id)
        {
            return _repo.Trips.FirstOrDefault(s => s.Id == id);
        }
        #endregion

        #region Blog
        public IEnumerable<Nyhede> BlogPosts()
        {
            return _repo.Nyhedes;
        }

        public IEnumerable<Nyhede> BlogPosts(int n)
        {
            return _repo.Nyhedes.OrderByDescending(s => s.Date).Take(n);
        }

        public void AddBlog(Nyhede blog)
        {
            _repo.Nyhedes.Add(blog);
            Save();
        }
        #endregion

        #region Internal Classes
        #region FishDateTimeClass
        internal class FishDateTimeClass
        {
            public DateTime DateTime;
            public Fisk Fisk;

            public FishDateTimeClass(DateTime dateTime)
            {
                DateTime = dateTime;

            }

            public FishDateTimeClass()
            {
            }
        }
        #endregion

        #region ArtTableElement Class
        internal class ArtTableElement
        {
            public int Id;
            public string Name;

            public ArtTableElement(int id, string name)
            {
                Id = id;
                Name = name;
            }
        }
        #endregion
        #endregion

        #region Havne
        public int GetHavnId(string name)
        {
            var firstOrDefault = _repo.Havns.FirstOrDefault(s => s.Navn == name);
            if (firstOrDefault != null)
                return firstOrDefault.Id;
            return -1;
        }


        public Havn GetHavnFromName(string name)
        {
            return _repo.Havns.FirstOrDefault(s => String.Equals(s.Navn, name.Trim(), StringComparison.CurrentCultureIgnoreCase));
        }

        public Havn GetHavnFromId(int id)
        {
            return _repo.Havns.FirstOrDefault(s => s.Id == id);
        }
        #endregion

        #region Add
        #region FISH
        #region AddFish
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
        #endregion
        #endregion

        #region USER
        #region AddUser
        public bool AddUser(User user)
        {
            var usersLocal = _repo.Users.Select(s=>s.Username);
            var normUser = user.Username.Normalize().Trim().ToLower();
            var found = false;
// ReSharper disable LoopCanBeConvertedToQuery
            foreach (var usr in usersLocal)
            {
                var normUsr = usr.Normalize().ToLower().Trim();
                if (normUser.Equals(normUsr))
                {
                    found = true;
                    break;
                }
            }

            if (found)
                return false;

            var userRoles = _repo.Roles;
            var normRole = "User".Normalize();
            Role usrRole = null;
            foreach (var userRole in userRoles)
// ReSharper restore LoopCanBeConvertedToQuery
            {
                var userRoleDb = userRole.Name.Normalize();
                if (userRoleDb.Equals(normRole))
                {
                    usrRole = userRole;
                    break;
                }
            }

            //HASH PASSWORD
            var hashedPas = Helper.HashPassword(user.Password.pass);
            user.Password.pass = hashedPas;
            user.Password.ConfirmPassword = hashedPas;

            user.Role = usrRole;
            _repo.Users.Add(user);
            Save();
            return true;
        }
        #endregion

        #region Authenticate
        public bool AuthenticateUser(User user)
        {
            var localUsers = _repo.Users;
// ReSharper disable LoopCanBeConvertedToQuery
            foreach (var localUser in localUsers)
// ReSharper restore LoopCanBeConvertedToQuery
            {
                var localUserNameNorm = localUser.Username.Normalize().ToLower().Trim();
                if (localUserNameNorm != user.Username.Normalize().ToLower().Trim()) continue;
                var localPasswordNorm = localUser.Password.pass.Normalize().ToLower().Trim();
                if (localPasswordNorm == Helper.HashPassword(user.Password.pass).Normalize().ToLower().Trim())
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        #endregion

        #region Trip
        public void AddTrip(Trip trip)
        {
            _repo.Trips.Add(trip);
            Save();
        }
        #endregion
        #endregion

        #region Data
        #region Get Date from webpages
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
        #endregion
        #region Get Date from webpages
        public IEnumerable<Fisk> GetContentFiskeContent(string url, int havnId)
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
            table.Columns.Add("Havn");
            table.Columns.Add("Art");
            table.Columns.Add("Sort");
            table.Columns.Add("Gns.Pris");
            table.Columns.Add("Max.Kr");
            table.Columns.Add("Mængde");


            var rows = nodes.Skip(5).Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToArray());

            foreach (var row in rows.TakeWhile(row => row.Count() >= 3))
            {
                table.Rows.Add(row);
                var art = GetArtFromName(HttpUtility.HtmlDecode(row[1]));
                if (art == null)
                {
                    var artToAdd = new Arter { Navn = HttpUtility.HtmlDecode(row[1]) };
                    art = AddArt(artToAdd);
                }
                var havn = GetHavnFromId(havnId);
                var dato = DateTime.UtcNow;
                var addFish = new Fisk
                {
                    Arters = art,
                    Havne = havn,
                    Dato = dato.ToString(new CultureInfo("da-dk")),
                    Sort = HttpUtility.HtmlDecode(row[2]),
                    AvgPrice = HttpUtility.HtmlDecode(row[3]).Normalize().Split(' ')[1].Replace('.',',') + " kr.",
                    MaxPrice = HttpUtility.HtmlDecode(row[4]).Normalize().Split(' ')[1].Replace('.', ',') + " kr.",
                    Amount = HttpUtility.HtmlDecode(row[5])
                };
                allFish.Add(addFish);
            }

            return allFish;
        }
        #endregion
        #endregion

        #region IpLogging

        public IEnumerable<IpLog> GetAllIpLogs()
        {
            return _repo.IpLogs.OrderByDescending(s => s.Id);
        }

        public void AddIpLog(IpLog ipLog)
        {
            _repo.IpLogs.Add(ipLog);
            Save();
        }

        #endregion

        #region Save Datebase
        public void Save()
        {
            _repo.SaveChanges();
        }
        #endregion
    }
}