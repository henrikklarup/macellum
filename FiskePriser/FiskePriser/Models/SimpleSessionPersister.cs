using System.Web;

namespace Macellum.Models
{
    public class SimpleSessionPersister
    {
// ReSharper disable ConvertToConstant.Local
        private static string usernameSessionVar = "username";
// ReSharper restore ConvertToConstant.Local

        public static string Username
        {
            get
            {
                if (HttpContext.Current == null) return string.Empty;
                var sessionVar = HttpContext.Current.Session[usernameSessionVar];
                if (sessionVar != null)
                    return sessionVar as string;
                return null;
            }
            set { HttpContext.Current.Session[usernameSessionVar] = value; }
        }
    }
}