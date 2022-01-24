using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class FileShortLink
    {
        [ForeignKey("File")]
        public Guid Id { get; set; } 
        public virtual File File { get; set; }
        public string FullLink { get; set; }
        public string ShortLink { get; set; }
    }
}
