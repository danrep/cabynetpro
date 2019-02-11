using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using CabynetPro.Core.Utility;
using CabynetPro.EnumLibrary;
using CabynetPro.EnumLibrary.Dictionary;
using CabynetPro.Web.Engines;
using CabynetPro.Web.Models;

namespace CabynetPro.Web.Controllers
{
    public class UserManagementController : BaseController
    {
        // GET: Administration/User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddUser()
        {
            return View();
        }

        public ActionResult ManageUser(string username)
        {
            using (var data = new Entities())
            {
                ViewBag.Username = username;
                return View(data.Credentials.FirstOrDefault(x => !x.IsDeleted && x.Username == username));
            }
        }

        public ActionResult EditUser(string username)
        {
            using (var data = new Entities())
            {
                ViewBag.Username = username;
                return View(data.Credentials.FirstOrDefault(x => !x.IsDeleted && x.Username == username));
            }
        }

        [HttpGet]
        public JsonResult GetUsers()
        {
            try
            {
                using (var data = new Entities())
                {
                    var registeredUsersByRole =
                        data.CredentialMaps.Where(x => x.IsDeleted == false).ToList();

                    var usersRawPrimary = data.Credentials.Where(x => x.IsDeleted == false).ToList();

                    var usersRaw =
                        usersRawPrimary.Where(x => registeredUsersByRole.Select(y => y.CredentialId).Contains(x.Id))
                            .ToList();

                    var users = usersRaw.Select(
                                s =>
                                    new
                                    {
                                        UserInfo = s,
                                        UserRole =
                                            (s.Id != 0 ? ((UserRoles)
                                                registeredUsersByRole.FirstOrDefault(r => r.CredentialId == s.Id).RoleId)
                                                .DisplayName() : "Select a Role")
                                    })
                            .OrderBy(o => o.UserInfo.Username).ThenBy(o => o.UserInfo.Surname)
                            .ToList();

                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = $"Found {users.Count} User(s)", Data = users },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult()
                {
                    Data = new { Status = false, Message = ex.Message, Data = string.Empty },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        [HttpGet]
        public JsonResult GetUser(string username)
        {
            try
            {
                using (var data = new Entities())
                {
                    var credo = data.Credentials.FirstOrDefault(x => x.Username == username);
                    return new JsonResult()
                    {
                        Data =
                            new
                            {
                                Status = true,
                                Message = $"Found User",
                                Data =
                                    new
                                    {
                                        UserInformation = credo
                                    }
                            },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult()
                {
                    Data = new { Status = false, Message = ex.Message, Data = string.Empty },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        [HttpPost]
        public JsonResult CreateUser(Credential userInformation)
        {
            try
            {
                if (userInformation.UserRoles == 0)
                    return new JsonResult()
                    {
                        Data = new
                        {
                            Status = false,
                            Message = "Please select a Role. If the Role has not changed, select the current Role"
                        }
                    };

                using (var data = new Entities())
                {
                    if (data.Credentials.FirstOrDefault(x => x.Username == userInformation.Username.ToLower()) != null)
                        return new JsonResult() { Data = new { Status = false, Message = "Username has been Used" } };

                    var passwordSalt = Encryption.GetUniqueKey(10);
                    var credential = data.Credentials.Add(new Credential()
                    {
                        DateCreated = DateTime.Now,
                        Surname = userInformation.Surname,
                        IsDeleted = false,
                        OtherNames = userInformation.OtherNames,
                        PasswordData = Encryption.SaltEncrypt(Encryption.GetUniqueKey(6), passwordSalt),
                        PasswordSalt = passwordSalt,
                        UserState = (int)UserStates.New,
                        Username = userInformation.Username.ToLower()
                    });
                    data.SaveChanges();

                    data.CredentialMaps.Add(new CredentialMap()
                    {
                        IsDeleted = false,
                        CredentialId = credential.Id,
                        DateAssigned = DateTime.Now,
                        RoleId = userInformation.UserRoles
                    });
                    data.SaveChanges();

                    new Thread(() =>
                    {
                        if (Messenger.SendNewCredentials(userInformation.Username))
                            ActivityLogger.Log("INFO", $"New Credentials have been sent Successfully");
                    }).Start();

                    ActivityLogger.Log("INFO", $"{userInformation.Username}: {userInformation.Surname}, {userInformation.OtherNames} has been created");
                }

                return new JsonResult() { Data = new { Status = true, Message = "Successful" } };
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult() { Data = new { Status = false, Message = ex.Message } };
            }
        }

        [HttpPost]
        public JsonResult UpdateUser(Credential userInformation)
        {
            try
            {
                if (userInformation.UserRoles == 0)
                    return new JsonResult()
                    {
                        Data = new
                        {
                            Status = false,
                            Message = "Please select a Role. If the Role has not changed, select the current Role"
                        }
                    };

                using (var data = new Entities())
                {
                    var existingCredo =
                        data.Credentials.FirstOrDefault(x => x.Username == userInformation.Username.ToLower());

                    if (existingCredo == null)
                        return new JsonResult() { Data = new { Status = false, Message = "This Credential is not Recognised. Please try to Add the User" } };

                    existingCredo.Surname = userInformation.Surname;
                    existingCredo.OtherNames = userInformation.OtherNames;

                    data.Entry(existingCredo).State = EntityState.Modified;
                    data.SaveChanges();

                    var existingCredoMap = data.CredentialMaps.FirstOrDefault(x => !x.IsDeleted && x.CredentialId == existingCredo.Id);
                    if (existingCredoMap != null)
                    {
                        data.CredentialMaps.Remove(existingCredoMap);
                        data.SaveChanges();
                    }

                    data.CredentialMaps.Add(new CredentialMap()
                    {
                        IsDeleted = false,
                        CredentialId = existingCredo.Id,
                        DateAssigned = DateTime.Now,
                        RoleId = userInformation.UserRoles
                    });
                    data.SaveChanges();

                    ActivityLogger.Log("INFO", $"{userInformation.Username}: {userInformation.Surname}, {userInformation.OtherNames} has been Updated");
                }

                return new JsonResult() { Data = new { Status = true, Message = "Successful" } };
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult() { Data = new { Status = false, Message = ex.Message } };
            }
        }

        [HttpGet]
        public JsonResult DeleteUser(string username)
        {
            try
            {
                using (var data = new Entities())
                {
                    var user =
                        data.Credentials.FirstOrDefault(x => x.Username == username.ToLower() && x.IsDeleted == false);
                    if (user == null)
                        return new JsonResult()
                        {
                            Data = new { Status = false, Message = "Username has NOT been Configured" }
                        };

                    var credentialMap = data.CredentialMaps.FirstOrDefault(x => x.CredentialId == user.Id);

                    if (credentialMap != null)
                        data.CredentialMaps.Remove(credentialMap);

                    user.IsDeleted = true;

                    data.Entry(user).State = EntityState.Modified;
                    data.SaveChanges();

                    ActivityLogger.Log("INFO", $"{username} has been deleted");
                }

                return new JsonResult()
                {
                    Data = new { Status = true, Message = "Successful" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult()
                {
                    Data = new { Status = false, Message = ex.Message },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        [HttpGet]
        public JsonResult SendUserCredentials(string username)
        {
            try
            {
                using (var data = new Entities())
                {
                    var user = data.Credentials.FirstOrDefault(x => x.Username == username.ToLower() && x.IsDeleted == false);
                    if (user == null)
                        return new JsonResult()
                        {
                            Data = new { Status = false, Message = "Username has NOT been Configured" }
                        };

                    new Thread(() =>
                    {
                        if (Messenger.SendCredentials(username))
                            ActivityLogger.Log("INFO", $"Credentials have been sent Successfully");
                    }).Start();

                    ActivityLogger.Log("INFO", $"{username} has been deleted");
                }

                return new JsonResult() { Data = new { Status = true, Message = "Successful" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult() { Data = new { Status = false, Message = ex.Message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
    }
}