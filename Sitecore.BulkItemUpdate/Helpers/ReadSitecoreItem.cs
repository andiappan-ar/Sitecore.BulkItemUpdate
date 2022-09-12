using Sitecore.Buckets.Managers;
using Sitecore.BulkItemUpdate.Models;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Sitecore.BulkItemUpdate.Helpers
{
    public static class ReadSitecoreItem
    {
        public static BulkModel ReadItemsRecursively()
        {
            BulkModel resultRoot = new BulkModel();
            StringBuilder str = new StringBuilder();

            try
            {
                string rootItemPath = @"/sitecore/content/SitecoreBulkItem/RootNestedItem";
                string templateID = "{C4A7A3C9-879F-49E0-BDBF-0789C027AC2D}";

                Database masterDB = Sitecore.Configuration.Factory.GetDatabase("master");
                
                Item siteRoot = masterDB.GetItem(rootItemPath);

                Action<ChildList, BulkModel> _GetChildItem = null;
                Action<Item, BulkModel> _GetCurrentItem = null;

                _GetChildItem = (allChildItem, parentItem) =>
                {
                    foreach (Item individualChildItem in allChildItem)
                    {
                        _GetCurrentItem(individualChildItem, parentItem);
                    }
                };

                _GetCurrentItem = (currentItem,parentItem) =>
                {
                    var eventTypeModel = new BulkModel();

                    if (currentItem != null)
                    {
                        if (TemplateManager.GetTemplate(currentItem).InheritsFrom(templateID))
                        {
                            eventTypeModel.ItemName = currentItem.Name;

                            if (parentItem.Child != null)
                            {
                                parentItem.Child.Add(eventTypeModel);
                            }
                            else
                            {
                                parentItem.Child = new List<BulkModel>() { eventTypeModel };
                            }
                            
                        }

                        if (currentItem.Children.Any() && currentItem.Children.Count() > 0)
                            _GetChildItem(currentItem.Children, eventTypeModel);
                    }
                };
              
                _GetCurrentItem(siteRoot, resultRoot);
            }
            catch(Exception ex)
            {

            }

            return resultRoot;
        }

       
    }
}