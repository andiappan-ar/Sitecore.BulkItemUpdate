using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.BulkItemUpdate.Models
{
    public class BackupItemModel
    {
        public string ItemPath { get; set; }

        public bool IncludeSubItem { get; set; }
    }
  
}