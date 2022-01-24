using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class File
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public string Type { get; set; }
        [NotMapped]
        public string Location { get; set; }
        [Required]
        [Range(1, 104857600)]
        public long Size { get; set; } // bytes
        public DateTime UploadDate { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        public bool IsImportant { get; set; }
        [ForeignKey("DirectoryId")]
        public virtual Directory Directory { get; set; }
        public Guid? DirectoryId { get; set; }
        public virtual ICollection<SharedFile> Recipients { get; set; }
        public virtual FileShortLink Link { get; set; }

        public File()
        {
            UploadDate = DateTime.Now;
            IsImportant = false;
        }
    }
}
