using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.Models
{
    public class FileModel: StorageItemModel
    {
       
        public string Type { get; set; }
        [Required]
        [Range(1, 104857600)]
        public long Size { get; set; } // bytes
        public Guid? DirectoryId { get; set; }
        public DirectoryModel Directory { get; set; }
        public string ApplicationUserId { get; set; }
        public UserModel ApplicationUser { get; set; }
        [Display(Name = "Important")]
        public bool IsImportant { get; set; }
        public IEnumerable<SharedFileModel> Recipients { get; set; }
        public LinkModel Link { get; set; }
    }
}
