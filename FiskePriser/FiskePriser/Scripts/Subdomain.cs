using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Macellum.Scripts
{
    public class Subdomain : RouteBase
    {

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var url = httpContext.Request.Headers["HOST"];
            var index = url.IndexOf(".", System.StringComparison.Ordinal);

            if (index < 0)
                return null;

            var subDomain = url.Substring(0, index);

            if (subDomain != "blog") return null;
            var routeData = new RouteData(this, new MvcRouteHandler());
            routeData.Values.Add("controller", "Blog"); //Goes to the User1Controller class
            routeData.Values.Add("action", "Index"); //Goes to the Index action on the User1Controller

            return routeData;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            //Implement your formating Url formating here
            return null;
        }
    }
}