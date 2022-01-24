using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class Directory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public DateTime UploadDate { get; set; } 

        [NotMapped]
        public string Location { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }
        public virtual ICollection<File> Files { get; set; }

        [ForeignKey("ParentDirectoryId")]
        public virtual Directory ParentDirectory { get; set; }
        public Guid? ParentDirectoryId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }

        public Directory()
        {
            UploadDate = DateTime.Now;
        }
    }
}
