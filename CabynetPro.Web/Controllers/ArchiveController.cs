using System;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Hosting;
using System.Web.Mvc;
using CabynetPro.Core.Utility;
using CabynetPro.EnumLibrary;
using CabynetPro.Web.Engines;
using CabynetPro.Web.Models;
using Newtonsoft.Json;

namespace CabynetPro.Web.Controllers
{
    public class ArchiveController : BaseController
    {
        // GET: Archive
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DataUpload()
        {
            return View();
        }

        [HttpPost]
        public bool ProcessDumpFile()
        {
            try
            {
                foreach (string fileName in Request.Files)
                {
                    var file = Request.Files[fileName];

                    if (file == null || file.ContentLength <= 0)
                        continue;

                    var location =
                        $"{HostingEnvironment.ApplicationPhysicalPath}\\CabynetProDataArchive\\Prep";

                    if (!Directory.Exists(location))
                        Directory.CreateDirectory(location);

                    var directory = new DirectoryInfo(location);

                    var path = $"{directory.FullName}\\{file.FileName}";
                    file.SaveAs(path);
                }

                return true;
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return false;
            }
        }

        [HttpPost]
        public JsonResult ProcessSaveFiles(FileManifest fileManifest)
        {
            var userInformation = UserInformation.UserInformationCredential;

            new Thread(() =>
            {
                ActivityLogger.Log("INFO",
                    $"{Thread.CurrentThread.ManagedThreadId} >> Running File Save Operation on {JsonConvert.SerializeObject(fileManifest, Formatting.Indented)}");

                try
                {
                    using (var data = new Entities())
                    {
                        var prepLocation =
                            $"{HostingEnvironment.ApplicationPhysicalPath}\\CabynetProDataArchive\\Prep";
                        var permanentLocation =
                            $"{HostingEnvironment.ApplicationPhysicalPath}\\CabynetProDataArchive\\Store";

                        if (!Directory.Exists(permanentLocation))
                            Directory.CreateDirectory(permanentLocation);

                        var prepDirectory = new DirectoryInfo(prepLocation);
                        var permanentDirectory = new DirectoryInfo(permanentLocation);

                        foreach (var fileName in fileManifest.Files.Select(x => x).Distinct().ToList())
                        {
                            if (string.IsNullOrEmpty(fileName))
                                continue;

                            var prepFileInfo = new FileInfo($"{prepDirectory.FullName}\\{fileName}");

                            if (!prepFileInfo.Exists)
                            {
                                ActivityLogger.Log("WARN",
                                    $"Cannot find FILE {fileName}. The Operation has been skipped");
                                continue;
                            }

                            var newName = Guid.NewGuid().ToString().ToUpper() + prepFileInfo.Extension;

                            var fileInformation = new FileInformation
                            {
                                DateCreated = DateTime.Now,
                                IsDeleted = false,
                                FileNameSource = prepFileInfo.Name,
                                FileNameDescription = fileManifest.Description,
                                FileNameSystem = newName,
                                CategoryId = fileManifest.Category,
                                FileSize = prepFileInfo.Length,
                                FileTag = JsonConvert.SerializeObject(prepFileInfo.Attributes),
                                FileType = prepFileInfo.Extension,
                                FolderId = 0,
                                UserCreated = userInformation.Id
                            };
                            data.FileInformations.Add(fileInformation);
                            data.SaveChanges();

                            AuditManager.LogFileAudit(fileInformation.Id, AuditStates.Upload, userInformation.Id);

                            System.IO.File.Move(prepFileInfo.FullName, $"{permanentDirectory.FullName}\\{newName}");
                        }

                        foreach (var file in fileManifest.Files)
                            System.IO.File.Delete($"{prepDirectory.FullName}\\{file}");
                    }
                }
                catch (Exception ex)
                {
                    ActivityLogger.Log(ex);
                }

                ActivityLogger.Log("INFO",
                    $"{Thread.CurrentThread.ManagedThreadId} >> File Save Operation Completed Successfully");
            }).Start();

            return new JsonResult
            {
                Data = new {Status = true, Message = "Successful"},
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpGet]
        public JsonResult ProcessDeleteFile(long fileInformationId)
        {
            try
            {
                var permanentLocation =
                    $"{HostingEnvironment.ApplicationPhysicalPath}\\CabynetProDataArchive\\Store";
                var archiveLocation =
                    $"{HostingEnvironment.ApplicationPhysicalPath}\\CabynetProDataArchive\\Archive";

                if (!Directory.Exists(archiveLocation))
                    Directory.CreateDirectory(archiveLocation);

                var permanentDirectory =
                    new DirectoryInfo(permanentLocation);
                var archiveDirectory =
                    new DirectoryInfo(archiveLocation);

                using (var data = new Entities())
                {
                    var fileInformation = data.FileInformations
                        .FirstOrDefault(x => x.Id == fileInformationId && !x.IsDeleted);

                    if (fileInformation == null)
                        return new JsonResult
                        {
                            Data = new {Status = true, Message = "Successful"},
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    System.IO.File.Move($"{permanentDirectory.FullName}\\{fileInformation.FileNameSystem}",
                        $"{archiveDirectory.FullName}\\{fileInformation.FileNameSystem}");

                    //Todo: Zip Utility

                    fileInformation.IsDeleted = true;
                    data.Entry(fileInformation).State = EntityState.Modified;
                    data.SaveChanges();

                    AuditManager.LogFileAudit(fileInformation.Id, AuditStates.Upload,
                        UserInformation.UserInformationCredential.Id);
                }

                return new JsonResult
                {
                    Data = new {Status = true, Message = "Successful"},
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult
                {
                    Data = new {Status = false, ex.Message},
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        [HttpGet]
        public JsonResult ProcessGetFiles(string name = "", string fromDate = "", string toDate ="")
        {
            try
            {
                using (var data = new Entities())
                {
                    var fileInformation = data.FileInformations.Where(x => !x.IsDeleted);

                    if (!string.IsNullOrEmpty(name))
                        fileInformation = fileInformation.Where(x => x.FileNameSource.Contains(name));

                    if (!string.IsNullOrEmpty(fromDate))
                        fileInformation = fileInformation.Where(x =>
                            x.DateCreated >= DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture));

                    if (!string.IsNullOrEmpty(toDate))
                        fileInformation = fileInformation.Where(x =>
                            x.DateCreated >= DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture));

                    return new JsonResult
                    {
                        Data = new
                        {
                            Status = true,
                            Message = "Successful",
                            Data = fileInformation.Take(100).OrderBy(x => x.DateCreated).ToList()
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult
                {
                    Data = new { Status = false, ex.Message },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
    }
}