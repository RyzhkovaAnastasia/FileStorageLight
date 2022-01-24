using System.Collections.Generic;

namespace BLL.Models
{
    public class Recipients
    {
        public IEnumerable<UserModel> AllUsers { get; set; }
        public IEnumerable<string> SelectedUsersId { get; set; }
    }
}
