using System;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using Macellum.Models;

namespace Macellum
{
    public class MvcApplication : System.Web.HttpApplication
    {

        private static CacheItemRemovedCallback OnCacheRemove = null;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AddTask("FishUpdate",3600);
        }

        private void AddTask(string name, int seconds)
        {
            OnCacheRemove = new CacheItemRemovedCallback(CacheItemRemoved);
            HttpRuntime.Cache.Insert(name, seconds, null,
                DateTime.Now.AddSeconds(seconds), Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable, OnCacheRemove);
        }

        public void CacheItemRemoved(string k, object v, CacheItemRemovedReason r)
        {
            if (k == "FishUpdate")
            {
                var dataClass = new DataClass();
                dataClass.dataLoad();
            }

            // do stuff here if it matches our taskname, like WebRequest
            // re-add our task so it recurs
            AddTask(k, Convert.ToInt32(v));
        }
        protected void Session_Start(Object sender, EventArgs e)
        {
            Session["init"] = 0;
        }
    }
}
