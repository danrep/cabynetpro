using System;
using System.Web.Mvc;
using CabynetPro.Core.Utility;
using CabynetPro.Web.Engines;
using CabynetPro.Web.Models;

namespace CabynetPro.Web.Controllers
{
    public class MyCabynetController : BaseController
    {
        // GET: MyCabynet
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyProfile()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            UserInformation.DeactivateSession();
            return RedirectToAction("Index", "Security", new {area = ""});
        }

        [HttpGet]
        public JsonResult ProcessGetFiles(string query)
        {
            try
            {
                return new JsonResult
                {
                    Data = new
                    {
                        Status = true,
                        Message = "Successful",
                        Data = SearchManager.Query(query)
                    },
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
    }
}