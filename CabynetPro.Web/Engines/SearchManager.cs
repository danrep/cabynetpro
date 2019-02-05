using System;
using System.Linq;
using CabynetPro.Core.Utility;
using CabynetPro.EnumLibrary;
using CabynetPro.EnumLibrary.Dictionary;

namespace CabynetPro.Web.Engines
{
    public class SearchManager
    {
        public static object Query(string query)
        {
            try
            {
                using (var entites = new Entities())
                {
                    var listOfFileInformation =
                        entites.FileInformations.Where(x =>
                            !x.IsDeleted && (x.FileNameDescription.Contains(query) ||
                                             x.FileNameSource.Contains(query))).Take(10);

                    if (!listOfFileInformation.Any())
                    {
                        var queryItems = query.Split(' ');

                        listOfFileInformation = entites.FileInformations.Where(x =>
                            x.FileNameDescription.Split(' ').Select(xElement => xElement)
                                .Any(fileEntryPart => queryItems.Contains(fileEntryPart)) &&
                            x.FileNameSource.Split(' ').Select(xElement => xElement)
                                .Any(fileEntryPart => queryItems.Contains(fileEntryPart))).Take(5);
                    }

                    return ProjectResults(listOfFileInformation);
                }
            }
            catch (Exception e)
            {
                ActivityLogger.Log(e);
                return null;
            }
        }

        private static object ProjectResults(IQueryable<FileInformation> fileInformations)
        {
            var fileInfoData = fileInformations.ToList();
            fileInformations = null;

            return fileInfoData.Select(x => new
            {
                x.FileNameSource,
                x.FileNameSystem,
                x.FileSize,
                x.DateCreated,
                x.FileNameDescription,
                Category = ((FileCategory) x.CategoryId).DisplayName()
            }).ToList();
        }
    }
}