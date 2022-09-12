using Sitecore.Buckets.Managers;
using Sitecore.BulkItemUpdate.Models;
using Sitecore.Collections;
using Sitecore.ContentSearch.Maintenance;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sitecore.BulkItemUpdate.Helpers
{
    public static class SitecoreBulkImport
    {
        #region BulkImportMethods
        /// <summary>
        /// Create bulk dummy model
        /// </summary>
        /// <returns></returns>
        public static List<BulkModel> GetBulkModels(string modelCount)
        {
            List<BulkModel> bulkModels = new List<BulkModel>();

            int bulkItemCount = Int32.Parse(modelCount);

            try
            {
                // Buld model with random value
                Random rand = new Random();
                Random rand1 = new Random();
                Random rand2 = new Random();
                Random rand3 = new Random();
                Random rand4 = new Random();

                List<string> listReferenceItemName = new List<string>()
                {
                    "One","Two","Three","Four"
                };
                List<bool> boolReferenceList = new List<bool>() { true, false };

                List<string> listReferenceMediaLinks = new List<string>()
                {
                    "/sitecore/media library/Default Website/cover",
                    "/sitecore/media library/Default Website/sc_logo"
                };

                Func<DateTime> RandomDay = () =>
                {
                    Random gen = new Random();
                    DateTime start = new DateTime(1995, 1, 1);
                    int range = (DateTime.Today - start).Days;
                    return start.AddDays(gen.Next(range));
                };

                for (int i = 0; i < bulkItemCount; i++)
                {
                    string guid = Guid.NewGuid().ToString();

                    bulkModels.Add(new BulkModel()
                    {
                        Action = "Create",
                        ItemId = "",
                        ItemName = Sitecore.Data.Items.ItemUtil.ProposeValidItemName(guid),

                        Field_SIngleLineText = string.Concat("Field_SIngleLineText - ", guid),
                        Field_Multiline = string.Concat("Field_Multiline - ", guid, Environment.NewLine, "SecondLineText"),
                        Field_RichText = string.Concat("<h1>", guid, "</h1>", Environment.NewLine, "<p>I am rich text</p>"),

                        Field_Integer = rand.Next(200, 400),
                        Field_Number = rand.Next(0, 500),

                        Field_DateTime = RandomDay(),

                        Field_DropList = listReferenceItemName[rand.Next(0, 3)],
                        Field_DropLink = listReferenceItemName[rand1.Next(0, 3)],
                        Field_MultiList = new List<string> { listReferenceItemName[rand2.Next(0, 3)],
                            listReferenceItemName[rand2.Next(0, 3)] },

                        Field_Checkbox = boolReferenceList[rand3.Next(0, 1)],

                        Field_Image = listReferenceMediaLinks[rand4.Next(0, 1)],

                        Field_Link = listReferenceMediaLinks[rand4.Next(0, 1)]
                    });

                }
            }
            catch (Exception ex)
            {

            }

            return bulkModels;

        }

        /// <summary>
        /// Update item init method
        /// </summary>
        /// <param name="bulkModel"></param>
        public static void UpdateSitecoreItem(List<BulkModel> bulkModel,string itemsPerPageText)
        {
            try
            {
                string scUserStr = @"sitecore\admin";
                string rootItemPath = @"/sitecore/content/SitecoreBulkItem/RootItem";
                string templateID = "{C4A7A3C9-879F-49E0-BDBF-0789C027AC2D}";
                string database = "master";
                string primaryLanguageStr = "EN";
                string listItemPath = "/sitecore/content/SitecoreBulkItem/SourceListItems";

                Database masterDB = Sitecore.Configuration.Factory.GetDatabase(database);
                var primaryLanguage = Sitecore.Globalization.Language.Parse(primaryLanguageStr);
                var allLanguage = LanguageManager.GetLanguages(masterDB);


                Item parentItem = masterDB.GetItem(rootItemPath);

                bool isBucketDisableRequired = false;

                var template = masterDB.Templates[templateID];

                // Allow only i any action needs to perform
                if (bulkModel.Exists(x => (x.Action.Equals("Create") || x.Action.Equals("Delete") || x.Action.Equals("Update"))))
                {
                    if (Sitecore.Security.Accounts.User.Exists(scUserStr))
                    {
                        //Getting Sitecore User Object with UserName
                        Sitecore.Security.Accounts.User scUser = Sitecore.Security.Accounts.User.FromName(scUserStr, false);

                        //Event type
                        var listRefernceIds = SitecoreHelpers.GetSourceListRefernceIdsByItemName(listItemPath);

                        //Switching Context User
                        using (new Sitecore.Security.Accounts.UserSwitcher(scUser))
                        //using (new SecurityDisabler())
                        {
                            // Pause index to avoid everytime generation
                            // This will improve performance while updating items
                            IndexCustodian.PauseIndexing();

                            // Supports bulkUpdate context
                            using (new BulkUpdateContext())
                            {
                                // Turn off bucket
                                if (isBucketDisableRequired) { BucketManager.Unbucket(parentItem); }

                                //Process Items as a chunks
                                // This is to efficiently create batch by batch processing
                                int itemsPerPage = Int32.Parse(itemsPerPageText);
                                int currentPage = 0;
                                int totalPages = (int)Math.Ceiling((decimal)bulkModel.Count() / (decimal)itemsPerPage);

                                {
                                    do
                                    {
                                        try
                                        {
                                            List<BulkModel> batchRecords =
                                           bulkModel.Skip(itemsPerPage * currentPage).Take(itemsPerPage).ToList();
                                            // Core item upgrade process
                                            UpdateSitecoreItemChunks(batchRecords, parentItem, template, listRefernceIds, masterDB, allLanguage, currentPage, primaryLanguageStr, listItemPath);

                                            currentPage++;
                                        }
                                        catch (Exception ex)
                                        {
                                            // Handle exception
                                            // If this batch fails , let continue other batch
                                        }
                                    } while (currentPage < totalPages);

                                }

                                if (isBucketDisableRequired) { BucketManager.CreateBucket(parentItem); }
                            }

                            // Resume index
                            IndexCustodian.ResumeIndexing();
                        }
                    }
                }
                else
                {
                    // No records to process
                }
            }
            catch (Exception ex)
            {
                // Handle exception if gloabl method error 
            }

        }


        /// <summary>
        /// Core update method
        /// </summary>
        private static Action<List<BulkModel>, Item, TemplateItem, Dictionary<string, string>,
            Database, LanguageCollection, int, string, string> UpdateSitecoreItemChunks =
            (bulkItemChunk, parentItem, template, listRefernceIds, masterDB, allLanguage, currentPage, primaryLanguageStr, listItemPath) =>
            {

                int successfullItemProcessed = 0;
                int failedItemProcessed = 0;
                string parentItemFullPath = parentItem.Paths.FullPath;



                var listRefernceIdList = SitecoreHelpers.GetSourceListRefernceIdsByItemName(listItemPath);

                // Maximize the faster results
                Parallel.ForEach(bulkItemChunk, (eachItem) =>
                //foreach (var events in bulkItemChunk)
                {
                    //Process based on action
                    switch (eachItem.Action)
                    {
                        case "None":
                            break;
                        case "Create":
                            #region create action
                            Item newItem = null;
                            try
                            {
                                // Primary language item update
                                newItem = parentItem.Add(eachItem.ItemName, template);

                                newItem.Editing.BeginEdit();
                                SitecoreHelpers.SetSitecoreTextFieldValue(newItem, "Field_SIngleLineText", eachItem.Field_SIngleLineText);
                                SitecoreHelpers.SetSitecoreTextFieldValue(newItem, "Field_Multiline", eachItem.Field_Multiline);
                                SitecoreHelpers.SetSitecoreTextFieldValue(newItem, "Field_RichText", eachItem.Field_RichText);

                                SitecoreHelpers.SetSitecoreNumbericFieldValue(newItem, "Field_Integer", eachItem.Field_Integer);
                                SitecoreHelpers.SetSitecoreNumbericFieldValue(newItem, "Field_Number", eachItem.Field_Number);

                                SitecoreHelpers.SetSitecoreDateFieldValue(newItem, "Field_DateTime", eachItem.Field_DateTime);

                                var refernceItem =
                                listRefernceIdList.Where(y => y.Key.Equals(eachItem.Field_DropList, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();


                                var refernceIdMultiList = (from list1 in listRefernceIdList
                                                           join list2 in eachItem.Field_MultiList on list1.Key equals list2
                                                           where list1.Key == list2
                                                           select list1.Value).Distinct().ToList();

                                SitecoreHelpers.SetSitecoreReferenceFieldValue(newItem, "Field_DropList", refernceItem.Key);
                                SitecoreHelpers.SetSitecoreReferenceFieldValue(newItem, "Field_DropLink", refernceItem.Value);

                                SitecoreHelpers.SetSitecoreMultiReferenceFieldValue(newItem, "Field_MultiList", refernceIdMultiList);

                                SitecoreHelpers.SetSitecoreCheckBoxFieldValue(newItem, "Field_Checkbox", eachItem.Field_Checkbox);

                                SitecoreHelpers.SetSitecoreImageFieldValue(newItem, masterDB, "Field_Image", eachItem.Field_Image);

                                SitecoreHelpers.SetSitecoreLinkFieldValue(newItem, masterDB, "Field_Link", "media", eachItem.Field_Link);
                                newItem.Editing.EndEdit();

                                // Secondary language item update
                                // Skip primary language
                                foreach (var individualLanguage in allLanguage.Where(x => (!x.Name.Equals(primaryLanguageStr,StringComparison.InvariantCultureIgnoreCase))))
                                {
                                    var secondaryLangItem = masterDB.GetItem(newItem.Paths.FullPath, individualLanguage);
                                    var secondaryLangItemVersion = secondaryLangItem.Versions.AddVersion();

                                    secondaryLangItemVersion.Editing.BeginEdit();
                                    // Update only non shared field
                                    SitecoreHelpers.SetSitecoreTextFieldValue(secondaryLangItemVersion, "Field_SIngleLineText", "SecondaryLanguage" + eachItem.Field_SIngleLineText);
                                    SitecoreHelpers.SetSitecoreTextFieldValue(secondaryLangItemVersion, "Field_Multiline", "SecondaryLanguage" + eachItem.Field_Multiline);
                                    SitecoreHelpers.SetSitecoreTextFieldValue(secondaryLangItemVersion, "Field_RichText", "SecondaryLanguage" + eachItem.Field_RichText);
                                    secondaryLangItemVersion.Editing.EndEdit();
                                }

                                successfullItemProcessed++;

                            }
                            catch (Exception ex)
                            {
                                // Handle exception                               
                            }
                            #endregion
                            break;
                        case "Delete":
                            #region delete action
                            Item deletableItem = null;
                            try
                            {
                                bool isItemUpdated = false;

                                if (!string.IsNullOrEmpty(eachItem.ItemName))
                                {
                                    deletableItem = masterDB.GetItem(eachItem.ItemId, parentItem.Language);
                                    if (deletableItem != null)
                                    {
                                        deletableItem.Delete();
                                        successfullItemProcessed++;
                                        isItemUpdated = true;
                                    }
                                    else
                                    {
                                        failedItemProcessed++;
                                    }

                                }
                                else
                                {
                                    failedItemProcessed++;
                                }
                            }
                            catch (Exception ex)
                            {
                                // Handle exception
                            }
                            #endregion
                            break;
                        case "Update":
                            #region update action
                            Item updateItem = null;
                            try
                            {
                                bool isItemUpdated = false;

                                if (!string.IsNullOrEmpty(eachItem.ItemId))
                                {
                                    updateItem = masterDB.GetItem(eachItem.ItemId, parentItem.Language);
                                    if (updateItem != null)
                                    {
                                        updateItem.Editing.BeginEdit();

                                        SitecoreHelpers.SetSitecoreTextFieldValue(updateItem, "Field_SIngleLineText", eachItem.Field_SIngleLineText);
                                        SitecoreHelpers.SetSitecoreTextFieldValue(updateItem, "Field_Multiline", eachItem.Field_Multiline);
                                        SitecoreHelpers.SetSitecoreTextFieldValue(updateItem, "Field_RichText", eachItem.Field_RichText);

                                        SitecoreHelpers.SetSitecoreNumbericFieldValue(updateItem, "Field_Integer", eachItem.Field_Integer);
                                        SitecoreHelpers.SetSitecoreNumbericFieldValue(updateItem, "Field_Number", eachItem.Field_Number);

                                        SitecoreHelpers.SetSitecoreDateFieldValue(updateItem, "Field_DateTime", eachItem.Field_DateTime);

                                        var refernceItem =
                                 listRefernceIdList.Where(y => y.Key.Equals(eachItem.Field_DropList, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();


                                        var refernceIdMultiList = (from list1 in listRefernceIdList
                                                                   join list2 in eachItem.Field_MultiList on list1.Key equals list2
                                                                   where list1.Key == list2
                                                                   select list1.Value).Distinct().ToList();

                                        SitecoreHelpers.SetSitecoreReferenceFieldValue(updateItem, "Field_DropList", refernceItem.Key);
                                        SitecoreHelpers.SetSitecoreReferenceFieldValue(updateItem, "Field_DropLink", refernceItem.Value);

                                        SitecoreHelpers.SetSitecoreMultiReferenceFieldValue(updateItem, "Field_MultiList", refernceIdMultiList);

                                        SitecoreHelpers.SetSitecoreCheckBoxFieldValue(updateItem, "Field_Checkbox", eachItem.Field_Checkbox);

                                        SitecoreHelpers.SetSitecoreImageFieldValue(updateItem, masterDB, "Field_Image", eachItem.Field_Image);

                                        SitecoreHelpers.SetSitecoreLinkFieldValue(updateItem, masterDB, "Field_Link", "media", eachItem.Field_Link);

                                        updateItem.Editing.EndEdit();

                                        // Secondary language item update
                                        // Skip primary language
                                        foreach (var individualLanguage in allLanguage.Where(x => (!x.Name.Equals(primaryLanguageStr, StringComparison.InvariantCultureIgnoreCase))))
                                        {
                                            var secondaryLangItem = masterDB.GetItem(updateItem.Paths.FullPath, individualLanguage);
                                            var secondaryLangItemVersion = secondaryLangItem.Versions.AddVersion();

                                            secondaryLangItemVersion.Editing.BeginEdit();
                                            // Update only non shared field
                                            SitecoreHelpers.SetSitecoreTextFieldValue(secondaryLangItemVersion, "Field_SIngleLineText", eachItem.Field_SIngleLineText);
                                            SitecoreHelpers.SetSitecoreTextFieldValue(secondaryLangItemVersion, "Field_Multiline", eachItem.Field_Multiline);
                                            SitecoreHelpers.SetSitecoreTextFieldValue(secondaryLangItemVersion, "Field_RichText", eachItem.Field_RichText);
                                            secondaryLangItemVersion.Editing.EndEdit();
                                        }
                                        successfullItemProcessed++;

                                    }

                                }
                                else
                                {
                                    failedItemProcessed++;
                                }



                            }
                            catch (Exception ex)
                            {
                                failedItemProcessed++;
                            }
                            #endregion
                            break;
                    }
                });


            };

        #endregion BulkImportMethods
    }
}
