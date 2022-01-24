using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace BLL.Models
{
    public class RoleModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public IEnumerable<IdentityRole> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }
        public RoleModel()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}
