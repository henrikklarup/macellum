namespace Macellum.Models
{
    public class MyIdentity : System.Security.Principal.IIdentity
    {
        public MyIdentity(string name)
        {
            Name = name;
        }


        public string Name { get; private set; }

        public string AuthenticationType
        {
            get { return "Custom"; }
        }

        public bool IsAuthenticated
        {
            get { return !string.IsNullOrEmpty(Name); } 
        }
    }
}