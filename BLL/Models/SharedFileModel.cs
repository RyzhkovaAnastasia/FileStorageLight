using System;

namespace BLL.Models
{
    public class SharedFileModel
    {
        public Guid Id { get; set; }

        public Guid FileId { get; set; }
        public virtual FileModel File { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual UserModel ApplicationUser { get; set; }
    }
}
