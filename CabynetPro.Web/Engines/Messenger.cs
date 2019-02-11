using System.Linq;
using CabynetPro.Core.Utility;

namespace CabynetPro.Web.Engines
{
    public static class Messenger
    {
        public static bool SendNewCredentials(string username)
        {
            using (var data = new Entities())
            {
                var user = data.Credentials.FirstOrDefault(x => x.Username == username);
                if (user == null)
                    return false;

                var message = $"Welcome to Cabynet Pro {user.Surname}, {user.OtherNames}. <br/>";
                message += $"Username: {username} <br/>";
                message += $"Password: {Encryption.SaltDecrypt(user.PasswordData, user.PasswordSalt)}<br/>";
                message += $"Best Regards";

                return Messaging.SendMail(username, "", "support@codesistance.com", "Registration", message, "");
            }
        }

        public static bool SendCredentials(string username)
        {
            using (var data = new Entities())
            {
                var user = data.Credentials.FirstOrDefault(x => x.Username == username);
                if (user == null)
                    return false;

                var message = $"Hello {user.Surname}, {user.OtherNames}. You have requested that your Credentials be resent. Find them below.<br/>";
                message += $"Username: {username} <br/>";
                message += $"Password: {Encryption.SaltDecrypt(user.PasswordData, user.PasswordSalt)}<br/>";
                message += $"Best Regards";

                return Messaging.SendMail(username, "", "support@codesistance.com", "Credential Request", message, "");
            }
        }
    }
}
