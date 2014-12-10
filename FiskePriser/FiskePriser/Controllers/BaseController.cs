using System.Web.Mvc;
using Macellum.Models;

namespace Macellum.Controllers
{
    public class BaseController : Controller
    {
        private readonly DatabaseRepo _repo = new DatabaseRepo();

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!string.IsNullOrEmpty(SimpleSessionPersister.Username))
            {
                var sessionId = _repo.GetSessionId(SimpleSessionPersister.Username);

                if(sessionId == System.Web.HttpContext.Current.Session.SessionID)
                    filterContext.HttpContext.User = new MyPrinciple(new MyIdentity(SimpleSessionPersister.Username));
                else
                {
                    SimpleSessionPersister.Username = null;
                }
            }

            base.OnAuthorization(filterContext);

            /*
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("~/User/Login");
                return;
            }

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectResult("~/User/Rejected");
                return;
            }*/
        }
    }
}