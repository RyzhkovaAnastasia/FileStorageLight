using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.Models
{
    public class StorageItemModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage ="Name is required.")]
        [MaxLength(100, ErrorMessage = "Name must be less 100 characters")]
        public string Name { get; set; }
        public  string Location { get; set; }
        [Display(Name = "Upload date")]
        public DateTime UploadDate { get; set; }
        [MaxLength(1000, ErrorMessage = "Description must be less 1000 characters")]
        public string Description { get; set; }
        public bool IsFile { get; set; }
    }
}
