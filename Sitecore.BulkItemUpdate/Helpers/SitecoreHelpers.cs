using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;


namespace Sitecore.BulkItemUpdate
{
    public static class SitecoreHelpers
    {
        #region Set sitecore field value
        /// <summary>
        /// SetSitecoreTextFieldValue
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetSitecoreTextFieldValue(Item item, string fieldName, string value)
        {
            bool result = false;

            try
            {
                if (item != null)
                {
                    item[fieldName] = value;
                }
            }
            catch (Exception ex) { }

            return result;
        }

        /// <summary>
        /// SetSitecoreNumbericFieldValue
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetSitecoreNumbericFieldValue(Item item, string fieldName, int value)
        {
            bool result = false;

            try
            {
                if (item != null)
                {
                    item[fieldName] = value.ToString();
                }
            }
            catch (Exception ex) { }

            return result;
        }

        /// <summary>
        /// SetSitecoreDateFieldValue
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        public static bool SetSitecoreDateFieldValue(Item item, string fieldName, DateTime dateValue)
        {
            bool result = false;

            try
            {
                if (item != null)
                {
                    item[fieldName] = Sitecore.DateUtil.ToIsoDate(dateValue);
                }
            }
            catch (Exception ex) { }

            return result;
        }

        /// <summary>
        /// SetSitecoreDropListFieldValue
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <param name="referenceId"></param>
        /// <returns></returns>
        public static bool SetSitecoreReferenceFieldValue(Item item, string fieldName, string referenceId)
        {
            bool result = false;

            try
            {
                if (item != null)
                {
                    item[fieldName] = referenceId;
                }
            }
            catch (Exception ex) { }

            return result;
        }

        /// <summary>
        /// SetSitecoreMultiReferenceFieldValue
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <param name="referenceIdList"></param>
        /// <returns></returns>
        public static bool SetSitecoreMultiReferenceFieldValue(Item item, string fieldName, List<string> referenceIdList)
        {
            bool result = false;

            try
            {
                if (item != null)
                {
                    Sitecore.Data.Fields.MultilistField multiselectField = item.Fields[fieldName];

                    foreach(string referenceId in referenceIdList)
                    {
                        if(!string.IsNullOrEmpty(referenceId))
                        multiselectField.Add(referenceId);
                    }
                    
                }
            }
            catch (Exception ex) { }

            return result;
        }

        /// <summary>
        /// SetSitecoreCheckBoxFieldValue
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public static bool SetSitecoreCheckBoxFieldValue(Item item, string fieldName, bool isActive)
        {
            bool result = false;

            try
            {
                if (item != null)
                {
                    Sitecore.Data.Fields.CheckboxField checkboxField = item.Fields[fieldName];
                    checkboxField.Checked = isActive;
                }
            }
            catch (Exception ex) { }

            return result;
        }

        /// <summary>
        /// SetSitecoreImageFieldValue
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        public static bool SetSitecoreImageFieldValue(Item item,Database database, string fieldName, string imageMediaItem)
        {
            bool result = false;

            try
            {
                if (item != null)
                {
                    Sitecore.Data.Items.Item sampleItem = database.GetItem(imageMediaItem);

                    Sitecore.Data.Items.MediaItem sampleMedia = new Sitecore.Data.Items.MediaItem(sampleItem);

                    Sitecore.Data.Fields.ImageField imageField = item.Fields[fieldName];

                    if (imageField.MediaID != sampleMedia.ID)
                    {
                        
                        imageField.Clear();
                        imageField.MediaID = sampleMedia.ID;

                        if (!String.IsNullOrEmpty(sampleMedia.Alt))
                        {
                            imageField.Alt = sampleMedia.Alt;
                        }
                        else
                        {
                            imageField.Alt = sampleMedia.DisplayName;
                        }
                    }
                }
            }
            catch (Exception ex) { }

            return result;
        }

        /// <summary>
        /// SetSitecoreLinkFieldValue
        /// </summary>
        /// <param name="item"></param>
        /// <param name="database"></param>
        /// <param name="fieldName"></param>
        /// <param name="linkType"></param>
        /// <returns></returns>
        public static bool SetSitecoreLinkFieldValue(Item item, Database database, string fieldName, string linkType, string linkValue)
        {
            bool result = false;

            try
            {
                if (item != null)
                {
                    Sitecore.Data.Fields.LinkField linkField = item.Fields[fieldName];

                    switch (linkType)
                    {
                        case "media":                            
                            Sitecore.Data.Items.Item mediaItem = database.GetItem(linkValue);
                            linkField.Clear();
                            linkField.LinkType = "media";
                            linkField.Url = mediaItem.Paths.MediaPath;
                            linkField.TargetID = mediaItem.ID;
                            break;
                        case "internal":
                            Sitecore.Data.Items.Item internalItem = database.GetItem(linkValue);
                            linkField.Clear();
                            linkField.LinkType = "internal";
                            var urlOptions =  Sitecore.Links.LinkManager.GetDefaultUrlBuilderOptions();
                            urlOptions.AlwaysIncludeServerUrl = false;
                            linkField.Url = Sitecore.Links.LinkManager.GetItemUrl(internalItem, urlOptions);
                            linkField.TargetID = internalItem.ID;
                            break;
                        case "external":
                            linkField.Clear();
                            linkField.LinkType = "external";
                            //Sample : http://sitecore.net
                            linkField.Url = linkValue;
                            break;
                        case "anchor":
                            linkField.Clear();
                            linkField.LinkType = "anchor";
                            //Sample : http://sitecore.net
                            linkField.Url = linkValue;
                            break;
                        case "mailto":
                            linkField.Clear();
                            linkField.LinkType = "mailto";
                            //Sample : "mailto:email@domain.tld"
                            linkField.Url = linkValue;
                            break;
                        case "javascript":
                            linkField.Clear();
                            linkField.Text = "";
                            linkField.LinkType = "javascript";
                            // Sample : @"javascript:alert('javascript')"
                            linkField.Url = linkValue;
                            break;
                    }
                }
            }
            catch (Exception ex) { }

            return result;
        }
        #endregion

        #region Get sitecore field value
        /// <summary>
        /// SetSitecoreTextFieldValue
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetSitecoreTextFieldValue(Item item, string fieldName)
        {
            string result = String.Empty;

            try
            {
                if (item != null)
                {
                    result = item[fieldName].ToString();
                }
            }
            catch (Exception ex) { }

            return result;
        }

        /// <summary>
        /// SetSitecoreDateFieldValue
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        public static string GetSitecoreDateFieldValue(Item item, string fieldName)
        {
            string result = String.Empty;

            try
            {
                if (item != null)
                {
                    Sitecore.Data.Fields.DateField dateTimeField = item.Fields[fieldName];

                    result = dateTimeField.DateTime.ToString("MM/ddd/YYYY");
                }
            }
            catch (Exception ex)
            { //Handle exception
              //
            }

            return result;
        }

        public static string GetSitecoreDropListFieldValue(Item item, string fieldName, string targetFieldName)
        {
            string result = string.Empty;

            try
            {
                if (item != null)
                {
                    Sitecore.Data.Fields.ReferenceField referenceField = item.Fields[fieldName];
                    if (referenceField != null && referenceField.TargetItem != null)
                    {
                        Sitecore.Data.Items.Item referencedItem = referenceField.TargetItem;
                        result = referencedItem[targetFieldName];
                    }
                }
            }
            catch (Exception ex)
            {
                //Handle exception
            }

            return result;
        }

        public static string GetSitecoreDropListFieldValueByItemName(Item item, string fieldName)
        {
            string result = string.Empty;

            try
            {
                if (item != null)
                {
                    Sitecore.Data.Fields.ReferenceField referenceField = item.Fields[fieldName];
                    if (referenceField != null && referenceField.TargetItem != null)
                    {
                        Sitecore.Data.Items.Item referencedItem = referenceField.TargetItem;
                        result = referencedItem.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                //Handle exception
            }

            return result;
        }

        public static bool GetSitecoreCheckBoxFieldValue(Item item, string fieldName)
        {
            bool result = false;

            try
            {
                if (item != null)
                {
                    Sitecore.Data.Fields.CheckboxField checkboxField = item.Fields[fieldName];
                    if (checkboxField != null)
                    {
                        result = checkboxField.Checked;
                    }
                }
            }
            catch (Exception ex)
            {
                //Handle exception
            }

            return result;
        }


        #endregion

        #region helper method
        public static Dictionary<string, string> GetSourceListRefernceIdsByField(string sourceItemPath, string uniqueIdFieldName, string db = "master")
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            try
            {
                Sitecore.Data.Database masterDB = Sitecore.Configuration.Factory.GetDatabase(db);
                Item parentItem = masterDB.GetItem(sourceItemPath);

                if (parentItem != null)
                {
                    foreach (Item childItem in parentItem.Children)
                    {
                        if (childItem != null && childItem[uniqueIdFieldName] != null)
                        {
                            try { result.Add(childItem[uniqueIdFieldName], childItem.ID.ToString()); }
                            catch (Exception ex) { }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //Handle exception             
            }

            return result;
        }

        public static Dictionary<string, string> GetSourceListRefernceIdsByItemName(string sourceItemPath, string db = "master")
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            try
            {
                Sitecore.Data.Database masterDB = Sitecore.Configuration.Factory.GetDatabase(db);
                Item parentItem = masterDB.GetItem(sourceItemPath);

                if (parentItem != null)
                {
                    foreach (Item childItem in parentItem.Children)
                    {
                        if (childItem != null)
                        {
                            try { result.Add(childItem.Name, childItem.ID.ToString()); }
                            catch (Exception ex) { }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //Handle exception
            }

            return result;
        }

        #endregion
    }
}