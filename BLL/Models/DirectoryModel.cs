using System;
using System.Collections.Generic;

namespace BLL.Models
{
    public class DirectoryModel : StorageItemModel
    {
        public IEnumerable<FileModel> Files { get; set; }
        public string ApplicationUserId { get; set; }
        public UserModel ApplicationUser { get; set; }
        public Guid ParentDirectoryId { get; set; }
    }
}
