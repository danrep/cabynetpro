using System;
using System.Linq;
using System.Web;
using CabynetPro.Core.Utility;
using CabynetPro.EnumLibrary;
using CabynetPro.EnumLibrary.Dictionary;

namespace CabynetPro.Web.Models
{
    public static class UserInformation
    {
        public static void ActivateSession(Credential userData)
        {
            try
            {
                HttpContext.Current.Session["UserData"] = userData;
                using (var entities = new Entities())
                {
                    var userMap = entities.CredentialMaps.FirstOrDefault(x => x.CredentialId == userData.Id && !x.IsDeleted);

                    if(userMap != null)
                        Role = (UserRoles)userMap.RoleId;
                    else
                    {
                        Role = userData.Id == 0 ? UserRoles.SystemAdministrator : UserRoles.Staff;
                    }
                }
            }
            catch (Exception e)
            {
                ActivityLogger.Log(e);
            }
        }

        public static bool IsSessionValid => HttpContext.Current.Session["UserData"] != null;

        public static Credential UserInformationCredential => ((Credential)HttpContext.Current.Session["UserData"]);

        public static void DeactivateSession()
        {
            HttpContext.Current.Session["UserData"] = null;
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.RemoveAll();
        }

        public static string RoleName => Role.DisplayName();

        public static UserRoles Role { get; private set; }
    }
}
