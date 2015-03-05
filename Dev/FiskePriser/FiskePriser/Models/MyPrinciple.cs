using System.Security.Principal;

namespace Macellum.Models
{
    public class MyPrinciple : IPrincipal
    {
        readonly DatabaseRepo _repo = new DatabaseRepo();

        public bool IsInRoleAttribute { get; private set; }
        public MyPrinciple(MyIdentity identity)
        {
            Identity = identity;
        }

        public bool IsInRole(string role)
        {
            var usrRole = _repo.GetRoleByUsername(SimpleSessionPersister.Username);
            var isRole = usrRole.Equals(role);
            IsInRoleAttribute = isRole;
            return isRole;
        }

        public IIdentity Identity { get; private set; }
    }
}