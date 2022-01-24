using System;
using System.Collections.Generic;

namespace BLL.Models
{
    public class StorageItemsModel
    {
        public Guid Id { get; set; }
        public DirectoryModel Directory { get; set; }
        public IEnumerable<StorageItemModel> StorageItems { get; set; }
    }
}
