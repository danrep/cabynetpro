using System;
using System.Web.Mvc;
using CabynetPro.Core.Utility;
using CabynetPro.EnumLibrary;
using CabynetPro.Web.Models;

namespace CabynetPro.Web.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (!UserInformation.IsSessionValid)
                {
                    UserInformation.DeactivateSession();
                    filterContext.Result = RedirectToAction("Index", "Security", new { area = "" });
                    return;
                }

                //if (UserInformation.UserInformationCredential.UserState == (int)UserStates.New)
                //{
                //    if (filterContext.ActionDescriptor.ActionName != "MyProfile")
                //    {
                //        filterContext.Result = RedirectToAction("MyProfile", "MyCabynet", new { area = "" });
                //        return;
                //    }
                //}

                var area = filterContext.RouteData.DataTokens["area"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(area))
                {
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                filterContext.Result = RedirectToAction("Index", "Security", new { area = "" });
                return;
            }
        }
    }
}