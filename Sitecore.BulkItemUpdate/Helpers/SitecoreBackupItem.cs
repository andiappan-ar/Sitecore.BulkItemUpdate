using Sitecore.Configuration;
using Sitecore.Install.Files;
using Sitecore.Install.Framework;
using Sitecore.Install.Items;
using Sitecore.Install.Zip;
using Sitecore.Install;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.BulkItemUpdate.Models;
using Sitecore.Security.Accounts;

namespace Sitecore.BulkItemUpdate.Helpers
{
    public static class SitecoreBackupItem
    {
        public static class CustomPackageGenerator
        {           
            private static string PackageName { get; set; }
            private static string Version { get; set; }
            private static string Author { get; set; }
            private static string Publisher { get; set; }
            private static string FileType { get; set; }

            public static void GeneratePackage()
            {
                try
                {                   
                    PackageName = "Custom package for bulk item update";
                    Version = "1.0";
                    Author = User.Current.Name;
                    Publisher = User.Current.Name;

                    string rootItemPath = @"/sitecore/content/SitecoreBulkItem/RootNestedItem";
                    string packageGeneratorFolder = System.Web.HttpContext.Current.Server.MapPath("~/packageGeneratorFolder/");

                    var items = new List<BackupItemModel>()
                {
                    new BackupItemModel() { ItemPath =rootItemPath , IncludeSubItem = true }
                };

                    var packageProject = new PackageProject
                    {
                        Metadata =
                {
                    PackageName = PackageName,
                    Author = Author,
                    Version = Version,
                    Publisher = Publisher
                }
                    };

                    var packageFileSource = new ExplicitFileSource
                    {
                        Name = "Custom File Source"
                    };

                    var packageItemSource = new ExplicitItemSource
                    {
                        Name = "Custom Item Source"
                    };

                    var sourceCollection = new SourceCollection<PackageEntry>();

                    sourceCollection.Add(packageItemSource);

                    foreach (var item in items)
                    {
                        var itemUri = Factory.GetDatabase(Settings.GetSetting("SourceDatabase")).Items.GetItem(item.ItemPath);

                        if (itemUri != null)
                        {
                            if (item.IncludeSubItem)
                            {
                                sourceCollection.Add(new ItemSource()
                                {
                                    SkipVersions = true,
                                    Database = itemUri.Uri.DatabaseName,
                                    Root = itemUri.Uri.ItemID.ToString()
                                });
                            }
                            else
                            {
                                packageItemSource.Entries.Add(new ItemReference(itemUri.Uri, false).ToString());
                            }
                        }

                    }

                    if (packageFileSource.Entries.Count > 0)
                    {
                        packageProject.Sources.Add(packageFileSource);
                    }

                    if (packageItemSource.Entries.Count > 0 || sourceCollection.Sources.Count > 0)
                    {
                        packageProject.Sources.Add(sourceCollection);
                    }

                    packageProject.SaveProject = true;


                    using (var writer = new PackageWriter(MainUtil.MapPath(string.Format("{0}/{1}/{2}.zip",
                        Settings.PackagePath, packageGeneratorFolder, PackageName))))
                    {
                        Context.SetActiveSite("shell");

                        writer.Initialize(Installer.CreateInstallationContext());

                        PackageGenerator.GeneratePackage(packageProject, writer);

                        Context.SetActiveSite("website");
                    }
                }
                catch(Exception ex)
                {
                    //Handle your exception
                }
                
            }
        }
    }
}