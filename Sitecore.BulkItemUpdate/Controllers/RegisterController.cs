using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sitecore.BulkItemUpdate.Controllers
{
    public class RegisterController : Controller
    {
        public ActionResult GetContextLanguage()
        {
            var sitecoreContextLanguage = (Sitecore.Context.Language != null) ? Sitecore.Context.Language.Name : "Sitecore context language is null";

            var text = Sitecore.Configuration.Factory.GetDatabase("master").
                GetItem("/sitecore/content/SitecoreBulkItem/AllFieldsSample").Fields["Field_SingleLineText"].Value;

            var sitecoreContextItem = Sitecore.Context.Item;
            var sitecoreContextSite = Sitecore.Context.Site;

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