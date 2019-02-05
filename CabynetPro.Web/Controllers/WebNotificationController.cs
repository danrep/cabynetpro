using System.Web.Mvc;

namespace CabynetPro.Web.Controllers
{
    public class WebNotificationController : BaseController
    {
        // GET: WebNotification
        public JsonResult GeneralInformation()
        {
            return Json(new
            {
                Status = true,
                Data = string.Empty
            }, JsonRequestBehavior.AllowGet);
        }
    }
}