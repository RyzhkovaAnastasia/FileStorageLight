using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class Log
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string Level { get; set; }
        [Required]
        public string Logger { get; set; }
        [Required]
        public string Exception { get; set; }
        [Required]
        public string Callsite { get; set; }
        public string Properties { get; set; }
    }
}
