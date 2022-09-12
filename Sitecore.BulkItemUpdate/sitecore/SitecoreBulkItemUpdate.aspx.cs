using System;
using System.Linq;
using Sitecore.BulkItemUpdate.Helpers;

namespace Sitecore.BulkItemUpdate
{
    public partial class SitecoreBulkItemUpdate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void BulkImport_Click(object sender, EventArgs e)
        {
            var bulkModel = SitecoreBulkImport.GetBulkModels(TextBox2.Text);

           
            var watch = System.Diagnostics.Stopwatch.StartNew();

            SitecoreBackupItem.CustomPackageGenerator.GeneratePackage();
            SitecoreBulkImport.UpdateSitecoreItem(bulkModel,TextBox3.Text);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            TextBox1.Text = String.Concat("Total execution time: " , elapsedMs);
           
        }

        protected void ReadItemRecursively_Click(object sender, EventArgs e)
        {
            var model = ReadSitecoreItem.ReadItemsRecursively();
           
        }
    }
}