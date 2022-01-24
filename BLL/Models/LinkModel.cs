using System;

namespace BLL.Models
{
    public class LinkModel
    {
        public Guid Id { get; set; }
        public string FullLink { get; set; }
        public string ShortLink { get; set; }
    }
}
