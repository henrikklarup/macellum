using System.Linq;
using System.Web.Mvc;
using FiskePriser.Models;

namespace FiskePriser.Controllers
{
    public class AdminController : Controller
    {
        readonly DatabaseRepo _repo = new DatabaseRepo();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadData()
        {
            const string thyborøn = @"http://www.fiskerforum.dk/auktionspriser/default.asp?auId=8026";
            const string hvidesande = @"http://www.fiskerforum.dk/auktionspriser/default.asp?auId=8016";
            const string thorsminde = @"http://www.fiskerforum.dk/auktionspriser/default.asp?auId=8126";

            var allFish = _repo.GetContent(thyborøn, _repo.GetHavnId("Thyborøn")).ToList();
            allFish.AddRange(_repo.GetContent(hvidesande, _repo.GetHavnId("Hvide Sande")).ToList());
            allFish.AddRange(_repo.GetContent(thorsminde, _repo.GetHavnId("Thorsminde")).ToList());

            _repo.AddFisk(allFish);

            return RedirectToAction("Index");
        }
    }
}