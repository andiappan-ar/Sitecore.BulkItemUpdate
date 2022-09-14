using Sitecore.Data.Items;
using Sitecore.Mvc.Controllers;
using System.Web.Mvc;

namespace Sitecore.BulkItemUpdate.Controllers
{
    public class MyHomeController : SitecoreController
    {
        public ActionResult GetContextItem()
        {
            var sitecoreContextSite = Sitecore.Context.Site; // This will provide Context.SiteSite
            string requestPath = HttpContext.Request.UrlReferrer.LocalPath; //This will provide path like this "/mainpage"

            Item contextItem = Sitecore.Configuration.Factory.GetDatabase("master").
                GetItem(
                Sitecore.Context.Site.RootPath // /sitecore/content/SitecoreBulkItem
                + requestPath // /mainpage
                );          

            return Json(new { ContextItemId = contextItem.ID.ToString(), ContextItemName = contextItem.Name }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetContextLanguage()
        {
          
            var sitecoreContextLanguage = (Sitecore.Context.Language !=null) ?Sitecore.Context.Language.Name:"Sitecore context language is null";
            
            var text = Sitecore.Configuration.Factory.GetDatabase("master").
                GetItem("/sitecore/content/SitecoreBulkItem/AllFieldsSample").Fields["Field_SingleLineText"].Value;

            var sitecoreContextItem = Sitecore.Context.Item;

            return Json(new { ContextLanguageName = Sitecore.Context.Language.Name, Text = text }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult PostContextLanguage()
        {
            var sitecoreContextItem = Sitecore.Context.Item;
            var sitecoreContextSite = Sitecore.Context.Site;

            var sitecoreContextLanguage = (Sitecore.Context.Language != null) ? Sitecore.Context.Language.Name : "Sitecore context language is null";

            var text = Sitecore.Configuration.Factory.GetDatabase("master").
                GetItem("/sitecore/content/SitecoreBulkItem/AllFieldsSample").Fields["Field_SingleLineText"].Value;

            return Json(new { ContextLanguageName = Sitecore.Context.Language.Name, Text = text });
        }

    }
}
