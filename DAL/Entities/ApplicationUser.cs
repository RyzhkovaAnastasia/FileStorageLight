using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using DAL.IdentityManagers;
using Microsoft.AspNet.Identity;

namespace DAL.Entities
{
    public class ApplicationUser: IdentityUser
    {
        [Index(IsUnique = true)]
        public override string Email { get => base.Email; set => base.Email = value; }
        public virtual ICollection<Directory> Directories { get; set; }
        public virtual ICollection<SharedFile> ReceivedFiles { get; set; }
    }
}
