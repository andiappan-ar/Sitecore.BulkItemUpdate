using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.BulkItemUpdate.Models
{
    public class BulkModel
    {   
        // Helper field
        public string Action { get; set; }
        public string ItemName { get; set; }
        public string ItemId { get; set; }

        // Text fields
        public string Field_SIngleLineText { get; set; }
        public string Field_Multiline { get; set; }
        public string Field_RichText { get; set; }

        // Number fields 
        public int Field_Integer { get; set; }
        public int Field_Number { get; set; }

        // Date field        
        public DateTime Field_DateTime { get; set; }
       

        // Reference fields
        public string Field_DropList { get; set; }
        public string Field_DropLink { get; set; }

        public List<string> Field_MultiList { get; set; }

        // Checkbox field    
        public bool Field_Checkbox { get; set; }

        // Image field
        public string Field_Image { get; set; }

        // Reference field
        public string Field_Link { get; set; }

        public List<BulkModel> Child { get; set; }
    }
}